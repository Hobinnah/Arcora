// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.DTOs;
using Arcora.Api.Models;
using Arcora.Api.Enums;
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Services.Interfaces;
using Arcora.Api.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Services.Implementations
{
    public class ListingService : IListingService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<ListingService> logger;
        private readonly IListingRepository listingRepository;
        private readonly IRatingRepository ratingRepository;
        private readonly IAddressService addressService;
        private readonly IPropertyService propertyService;
        private readonly IRentalUnitService rentalUnitService;
        private readonly IUnitTypeService unitTypeService;
        private readonly IListingTypeService listingTypeService;
        private readonly IAmenityCatalogService amenityCatalogService;
        private readonly IListingAmenityService listingAmenityService;
        private readonly IListingPhotoService listingPhotoService;
        private readonly ArcoraDbContext dbContext;
        private readonly IOptions<CacheConfiguration> _options;
        public ListingService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<ListingService> logger, IListingRepository listingRepository, IRatingRepository ratingRepository,
            IAddressService addressService, IPropertyService propertyService, IRentalUnitService rentalUnitService, IUnitTypeService unitTypeService, IListingTypeService listingTypeService,
            IAmenityCatalogService amenityCatalogService, IListingAmenityService listingAmenityService, IListingPhotoService listingPhotoService, ArcoraDbContext dbContext)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.listingRepository = listingRepository;
            this.ratingRepository = ratingRepository;
            this.addressService = addressService;
            this.propertyService = propertyService;
            this.rentalUnitService = rentalUnitService;
            this.unitTypeService = unitTypeService;
            this.listingTypeService = listingTypeService;
            this.amenityCatalogService = amenityCatalogService;
            this.listingAmenityService = listingAmenityService;
            this.listingPhotoService = listingPhotoService;
            this.dbContext = dbContext;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <summary>
        /// Populates the aggregated <see cref="ListingDto.Rating"/> and <see cref="ListingDto.Reviews"/>
        /// values from public ratings, grouped by the listing they belong to via the lease.
        /// The aggregate dictionary is cached so it is only recalculated when the cache expires
        /// or is invalidated.
        /// </summary>
        private async Task ApplyRatingAggregatesAsync(IEnumerable<ListingDto> listings)
        {
            var targets = listings?.Where(x => x?.ListingID != null).ToList();
            if (targets == null || targets.Count == 0)
                return;

            var aggregates = await this.GetRatingAggregatesAsync();

            foreach (var listing in targets)
            {
                if (aggregates.TryGetValue(listing.ListingID!.Value, out var agg))
                {
                    listing.Rating = agg.Average;
                    listing.Reviews = agg.Count;
                }
            }
        }

        /// <summary>
        /// Processes an import-style payload from the frontend and creates address, property, rental units,
        /// listings and listing amenities/photos reconciliation. Returns created IDs so the client can
        /// reconcile state.
        /// </summary>
        public async Task<ListingImportResultDto> ImportListingPayload(ListingImportDto payload)
        {
            var result = new ListingImportResultDto();
            if (payload == null)
                return result;

            // The whole import is a single business unit of work. All the service/repository calls
            // below share the same scoped ArcoraDbContext, so wrapping them in one transaction makes
            // the import atomic: a failure part-way through rolls back every write instead of leaving
            // orphaned address/property/unit records behind. CreateExecutionStrategy keeps this
            // compatible with SQL Server retry-on-failure if it is ever enabled.
            var strategy = this.dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await this.dbContext.Database.BeginTransactionAsync();
                try
                {
                    // 1) Address
                    if (payload.Property?.Address != null)
                    {
                        payload.Property.Address.AddressType = "PROPERTY";
                        payload.Property.Address.OrganizationID = payload.Property.OrganizationID;
                        var createdAddress = await this.addressService.CreateAddress(payload.Property.Address);
                        result.AddressID = createdAddress.AddressID;
                        // attach to property for creation
                        payload.Property.AddressID = createdAddress.AddressID ?? Guid.Empty;
                        payload.Property.Address = null;
                    }

                    // 2) Property
                    var createdProperty = await this.propertyService.CreateProperty(payload.Property);
                    result.PropertyID = createdProperty.PropertyID;

                    // 3+4) For each unit -> create rental unit then listing
                    var units = payload.Units?.OfType<UnitImportDto>() ?? Enumerable.Empty<UnitImportDto>();
                    foreach (var unit in units)
                    {
                        // resolve or create unit type
                        Guid? unitTypeId = null;
                        if (!string.IsNullOrWhiteSpace(unit.UnitTypeName))
                        {
                            var utPage = await this.unitTypeService.GetAll(new Paging { Search = unit.UnitTypeName, PageNumber = 1, PageSize = 1 });
                            var ut = utPage.Data.FirstOrDefault();
                            if (ut == null)
                            {
                                var createdUt = await this.unitTypeService.CreateUnitType(new UnitTypeDto { Name = unit.UnitTypeName, CapturedBy = unit.RentalUnit.CapturedBy ?? payload.Property.CapturedBy });
                                unitTypeId = createdUt.UnitTypeID;
                            }
                            else
                            {
                                unitTypeId = ut.UnitTypeID;
                            }
                        }

                        // create rental unit
                        unit.RentalUnit.PropertyID = createdProperty.PropertyID ?? Guid.Empty;
                        if (unitTypeId.HasValue)
                            unit.RentalUnit.UnitTypeID = unitTypeId;
                        unit.RentalUnit.FloorNumber = "1";
                        var createdUnit = await this.rentalUnitService.CreateRentalUnit(unit.RentalUnit);

                        var unitResult = new UnitImportResultItem { UnitNumber = createdUnit.UnitNumber, RentalUnitID = createdUnit.RentalUnitID };

                        // resolve or create listing type
                        Guid? listingTypeId = null;
                        if (!string.IsNullOrWhiteSpace(unit.ListingTypeName))
                        {
                            var ltPage = await this.listingTypeService.GetAll(new Paging { Search = unit.ListingTypeName, PageNumber = 1, PageSize = 1 });
                            var lt = ltPage.Data.FirstOrDefault();
                            if (lt == null)
                            {
                                var createdLt = await this.listingTypeService.CreateListingType(new ListingTypeDto { Name = unit.ListingTypeName, CapturedBy = unit.Listing.CapturedBy ?? payload.Property.CapturedBy });
                                listingTypeId = createdLt.ListingTypeID;
                            }
                            else
                            {
                                listingTypeId = lt.ListingTypeID;
                            }
                        }

                        // create listing: map the relaxed import DTO into the full ListingDto
                        var listingDto = new ListingDto();
                        listingDto.Title = unit.Listing.Title;
                        listingDto.Description = unit.Listing.Description;
                        listingDto.CheckInDoorCode = unit.Listing.CheckInDoorCode;
                        listingDto.Currency = unit.Listing.Currency;
                        listingDto.AvailableFrom = unit.Listing.AvailableFrom;
                        listingDto.AvailableTo = unit.Listing.AvailableTo;
                        listingDto.MinimumLeaseMonths = unit.Listing.MinimumLeaseMonths;
                        listingDto.MaximumLeaseMonths = unit.Listing.MaximumLeaseMonths;
                        listingDto.ApplicationDeadline = unit.Listing.ApplicationDeadline;
                        listingDto.Notes = unit.Listing.Notes;
                        listingDto.IsFurnished = unit.Listing.IsFurnished;
                        listingDto.IsPetFriendly = unit.Listing.IsPetFriendly;
                        listingDto.CapturedBy = unit.Listing.CapturedBy ?? payload.Property.CapturedBy;
                        listingDto.CapturedDate = unit.Listing.CapturedDate;
                        listingDto.WIFINetwork = unit.Listing.WIFINetwork;
                        listingDto.WIFIPassword = unit.Listing.WIFIPassword;
                        listingDto.AcceptingApplications = unit.Listing.AcceptingApplications;
                        listingDto.SquareFeet = unit.Listing.SquareFeet;
                        listingDto.Bedrooms = unit.RentalUnit.Bedrooms;
                        listingDto.Bathrooms = unit.RentalUnit.Bathrooms;
                        listingDto.SquareFeet = unit.Listing.SquareFeet;
                        listingDto.BaseMonthlyRentAmount = unit.Listing.BaseMonthlyRentAmount;
                        listingDto.SecurityDepositAmount = unit.Listing.SecurityDepositAmount;
                        listingDto.OrganizationID = createdProperty.OrganizationID;
                        listingDto.RentalUnitID = createdUnit.RentalUnitID;
                        listingDto.YearBuilt = payload.Property.YearBuilt;

                        if (listingTypeId.HasValue)
                            listingDto.ListingTypeID = listingTypeId;

                        var createdListing = await this.CreateListing(listingDto);
                        unitResult.ListingID = createdListing.ListingID;

                        // 5) Listing amenities
                        if (unit.SelectedAmenities != null && unit.SelectedAmenities.Any() && createdListing.ListingID != null)
                        {
                            foreach (var amenityName in unit.SelectedAmenities)
                            {
                                if (string.IsNullOrWhiteSpace(amenityName))
                                    continue;
                                var aPage = await this.amenityCatalogService.GetAll(new Paging { Search = amenityName, PageNumber = 1, PageSize = 1 });
                                var amen = aPage.Data.FirstOrDefault();

                                // resolve or create the amenity in the catalog so it can be linked to the listing
                                Guid? amenityId = amen?.AmenityID;
                                if (amenityId == null || amenityId == Guid.Empty)
                                {
                                    var createdAmenity = await this.amenityCatalogService.CreateAmenityCatalog(new AmenityCatalogDto
                                    {
                                        Name = amenityName.Trim(),
                                        IsActive = true,
                                        CapturedBy = createdListing.CapturedBy ?? payload.Property.CapturedBy
                                    });
                                    amenityId = createdAmenity.AmenityID;
                                }

                                if (amenityId != null && amenityId != Guid.Empty)
                                {
                                    var lam = new ListingAmenityDto
                                    {
                                        ListingID = createdListing.ListingID.Value,
                                        AmenityID = amenityId.Value,
                                        CapturedBy = createdListing.CapturedBy ?? payload.Property.CapturedBy
                                    };
                                    await this.listingAmenityService.CreateListingAmenity(lam);
                                }
                            }
                        }

                        // 6) Listing photo reconciliation
                        if (unit.ListingPhotos != null && unit.ListingPhotos.Any() && createdListing.ListingID != null)
                        {
                            // fetch all photos and match by CapturedBy pattern
                            string? capturedByPattern = unit.ListingPhotos?.FirstOrDefault()?.CapturedBy ?? string.Empty;
                            var allPhotos = await this.listingPhotoService.GetPhotosByCapturedBy(capturedByPattern);

                            foreach (var incoming in allPhotos)
                            {
                                if (string.IsNullOrWhiteSpace(incoming?.CapturedBy))
                                    continue;

                                var matches = allPhotos.Where(p => p.CapturedBy == incoming.CapturedBy && (p.ListingID == null || p.ListingID == Guid.Empty)).ToList();
                                foreach (var match in matches)
                                {
                                    if (match?.ListingPhotoID == null)
                                        continue;
                                    match.ListingID = createdListing.ListingID;
                                    match.CapturedBy = payload.Property.CapturedBy;
                                    await this.listingPhotoService.UpdateListingPhoto(match.ListingPhotoID.Value, match);
                                }
                            }
                        }

                        result.Units.Add(unitResult);
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    // reset partially-populated result so callers do not receive IDs for rolled-back records
                    result.AddressID = null;
                    result.PropertyID = null;
                    result.Units.Clear();
                    logger.LogError(ex, "An error occurred while importing listing payload. Timestamp: {Timestamp}", DateTime.UtcNow);
                    throw;
                }
            });

            return result;
        }

        /// <summary>
        /// Returns the per-listing rating aggregates, using a cached copy when available
        /// and rebuilding from the Rating table only when the cache is empty.
        /// </summary>
        private async Task<IReadOnlyDictionary<Guid, (decimal Average, int Count)>> GetRatingAggregatesAsync()
        {
            var cached = cache.Get<IReadOnlyDictionary<Guid, (decimal Average, int Count)>>(Cache.LISTINGRATINGAGGREGATES.ToString());
            if (cached != null)
                return cached;

            IEnumerable<Rating> ratings;
            try
            {
                ratings = await this.ratingRepository.GetRatingAsync() ?? new List<Rating>();
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Ratings for listing aggregates. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new Dictionary<Guid, (decimal Average, int Count)>();
            }

            var aggregates = ratings
                .Where(r => r.IsPublic && r.Lease != null)
                .GroupBy(r => r.Lease!.ListingID)
                .ToDictionary(
                    g => g.Key,
                    g => (Average: Math.Round((decimal)g.Average(r => r.OverallRating), 1), Count: g.Count()));

            cache.Set<IReadOnlyDictionary<Guid, (decimal Average, int Count)>>(
                Cache.LISTINGRATINGAGGREGATES.ToString(),
                aggregates,
                DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));

            return aggregates;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<ListingDto>> GetAll(Paging paging)
        {
            IEnumerable<Listing> entities;
            try
            {
                entities = cache.Get<IEnumerable<Listing>>(Cache.LISTINGS.ToString()) ?? new List<Listing>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.listingRepository.GetListingsAsync())?.Where(x => x != null) ?? new List<Listing>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<Listing>>(Cache.LISTINGS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Listing by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<ListingDto>
                {
                    Data = new List<ListingDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<Listing> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Title) && x.Title.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.CapturedDate).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<ListingDto>>(pagedEntities).ToList();
            await this.ApplyRatingAggregatesAsync(pagedDtos);
            return new PagedResult<ListingDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ListingDto>> GetListingsByOrganization(Guid organizationId)
        {
            try
            {
                var entities = await this.listingRepository.GetListingsByOrganizationAsync(organizationId) ?? new List<Listing>();
                var dtos = this.mapper.Map<IEnumerable<ListingDto>>(entities).ToList();
                await this.ApplyRatingAggregatesAsync(dtos);
                return dtos;
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Listings for Organization {OrganizationID}. Timestamp: {Timestamp}", organizationId, DateTime.UtcNow);
                return new List<ListingDto>();
            }
        }

        /// <inheritdoc/>
        public async Task<int> GetListingsCountByOrganization(Guid organizationId)
        {
            try
            {
                return await this.listingRepository.CountListingsByOrganizationAsync(organizationId);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while counting Listings for Organization {OrganizationID}. Timestamp: {Timestamp}", organizationId, DateTime.UtcNow);
                return 0;
            }
        }

        /// <inheritdoc/>
        public async Task<PagedResult<ListingDto>> SearchListings(ListingSearchCriteria criteria)
        {
            try
            {
                var (items, totalCount) = await this.listingRepository.SearchListingsAsync(criteria ?? new ListingSearchCriteria());
                var dtos = this.mapper.Map<IEnumerable<ListingDto>>(items).ToList();
                await this.ApplyRatingAggregatesAsync(dtos);
                return new PagedResult<ListingDto>
                {
                    Data = dtos,
                    TotalCount = totalCount
                };
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while searching Listings. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<ListingDto>
                {
                    Data = new List<ListingDto>(),
                    TotalCount = 0
                };
            }
        }

        /// <inheritdoc/>
        public async Task<ListingDto?> GetID(Guid ID)
        {
            try
            {
                // The LISTINGS cache is populated by GetAll via GetListingsAsync, which only
                // eager-loads photos for performance. The detail view needs the full child graph
                // (amenities, rules, policies, term prices, calendar events), so always load it
                // through GetListingAsync rather than the partially-populated list cache.
                Listing? match = await this.listingRepository.GetListingAsync(ID);

                if (match == null)
                    return null;

                var dto = this.mapper.Map<ListingDto>(match);
                await this.ApplyRatingAggregatesAsync(new[] { dto });
                return dto;
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Listing by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ListingDto> CreateListing(ListingDto listingDto)
        {
            Listing listing = new Listing();
            IEnumerable<Listing?> checkEntity;
            try
            {
                var normalizedTitle = listingDto.Title?.ToLower().Trim();
                checkEntity = await this.listingRepository.Find(x => x.Title != null && x.Title.ToLower().Trim() == normalizedTitle);
                if (checkEntity == null || !checkEntity.Any())
                {
                    listing = this.mapper.Map<Listing>(listingDto);
                    listing.ListingID = Guid.NewGuid();
                    listing.CapturedDate = DateTime.UtcNow;
                    listing = await listingRepository.Create(listing) ?? new Listing();
                    await listingRepository.Save();
                    cache.Remove(Cache.LISTINGS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating Listing. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<ListingDto>(listing);
        }

        /// <inheritdoc/>
        public async Task<ListingDto?> UpdateListing(Guid id, ListingDto listingDto)
        {
            try
            {
                var existing = await this.listingRepository.GetByID(id);
                if (existing == null)
                    return null;
                Listing listing = this.mapper.Map<Listing>(listingDto);
                listing = await listingRepository.Update(listing) ?? new Listing();
                await listingRepository.Save();
                cache.Remove(Cache.LISTINGS.ToString());
                listingDto = this.mapper.Map<ListingDto>(listing);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating Listing. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return listingDto;
        }

        /// <inheritdoc/>
        public async Task DeleteListing(Guid ID)
        {
            try
            {
                var listing = await this.listingRepository.GetByID(ID);
                if (listing == null)
                    throw new KeyNotFoundException("Listing with the specified ID was not found.");
                await listingRepository.Delete(listing);
                await listingRepository.Save();
                cache.Remove(Cache.LISTINGS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting Listing . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ListingDto?> UpdateListingStatus(Guid id, string status)
        {
            var listing = await listingRepository.GetByID(id);
            if (listing == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                listing.Status = "Pending";
            }
            else
            {
                listing.Status = status;
            }

            await listingRepository.Update(listing);
            await listingRepository.Save();
            cache.Remove(Cache.LISTINGS.ToString());
            return this.mapper.Map<ListingDto>(listing);
        }
    }
}