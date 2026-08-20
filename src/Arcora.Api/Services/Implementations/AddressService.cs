// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.DTOs;
using Arcora.Api.Models;
using Arcora.Api.Enums;
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Services.Interfaces;
using Arcora.Api.Configurations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Services.Implementations
{
    public class AddressService : IAddressService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<AddressService> logger;
        private readonly IAddressRepository addressRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public AddressService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<AddressService> logger, IAddressRepository addressRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.addressRepository = addressRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<AddressDto>> GetAll(Paging paging)
        {
            IEnumerable<Address> entities;
            try
            {
                entities = cache.Get<IEnumerable<Address>>(Cache.ADDRESSES.ToString()) ?? new List<Address>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.addressRepository.GetAddressAsync())?.Where(x => x != null) ?? new List<Address>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<Address>>(Cache.ADDRESSES.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Address by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<AddressDto>
                {
                    Data = new List<AddressDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<Address> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Line1) && x.Line1.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.AddressID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<AddressDto>>(pagedEntities);
            return new PagedResult<AddressDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<AddressDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<Address> entities = cache.Get<IEnumerable<Address>>(Cache.ADDRESSES.ToString()) ?? new List<Address>();
                Address? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.AddressID == ID);
                }
                else
                {
                    match = await this.addressRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<AddressDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Address by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<AddressDto> CreateAddress(AddressDto addressDto)
        {
            Address address = new Address();
            IEnumerable<Address?> checkEntity;
            try
            {
                checkEntity = await this.addressRepository.Find(x => x.Line1!.ToLower().Trim() == addressDto.Line1!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    address = this.mapper.Map<Address>(addressDto);
                    address.AddressID = Guid.NewGuid();
                    address.OrganizationID = addressDto.OrganizationID == Guid.Empty ? null : addressDto.OrganizationID;
                    address.PlaceProviderReferenceID = string.IsNullOrEmpty(addressDto.PlaceProviderReferenceID) ? null : addressDto.PlaceProviderReferenceID;
                    address.CapturedDate = DateTime.UtcNow;
                    address = await addressRepository.Create(address) ?? new Address();
                    await addressRepository.Save();
                    cache.Remove(Cache.ADDRESSES.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating Address. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<AddressDto>(address);
        }

        /// <inheritdoc/>
        public async Task<AddressDto?> UpdateAddress(Guid id, AddressDto addressDto)
        {
            try
            {
                var existing = await this.addressRepository.GetByID(id);
                if (existing == null)
                    return null;
                Address address = this.mapper.Map<Address>(addressDto);
                address = await addressRepository.Update(address) ?? new Address();
                await addressRepository.Save();
                cache.Remove(Cache.ADDRESSES.ToString());
                addressDto = this.mapper.Map<AddressDto>(address);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating Address. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return addressDto;
        }

        /// <inheritdoc/>
        public async Task DeleteAddress(Guid ID)
        {
            try
            {
                var address = await this.addressRepository.GetByID(ID);
                if (address == null)
                    throw new KeyNotFoundException("Address with the specified ID was not found.");
                await addressRepository.Delete(address);
                await addressRepository.Save();
                cache.Remove(Cache.ADDRESSES.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting Address . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}