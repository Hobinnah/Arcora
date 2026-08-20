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
    public class ListingAccessInstructionService : IListingAccessInstructionService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<ListingAccessInstructionService> logger;
        private readonly IListingAccessInstructionRepository listingaccessinstructionRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public ListingAccessInstructionService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<ListingAccessInstructionService> logger, IListingAccessInstructionRepository listingaccessinstructionRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.listingaccessinstructionRepository = listingaccessinstructionRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<ListingAccessInstructionDto>> GetAll(Paging paging)
        {
            IEnumerable<ListingAccessInstruction> entities;
            try
            {
                entities = cache.Get<IEnumerable<ListingAccessInstruction>>(Cache.LISTINGACCESSINSTRUCTIONS.ToString()) ?? new List<ListingAccessInstruction>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.listingaccessinstructionRepository.GetListingAccessInstructionAsync())?.Where(x => x != null) ?? new List<ListingAccessInstruction>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<ListingAccessInstruction>>(Cache.LISTINGACCESSINSTRUCTIONS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ListingAccessInstruction by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<ListingAccessInstructionDto>
                {
                    Data = new List<ListingAccessInstructionDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<ListingAccessInstruction> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.InstructionType) && x.InstructionType.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.ListingAccessInstructionID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<ListingAccessInstructionDto>>(pagedEntities);
            return new PagedResult<ListingAccessInstructionDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<ListingAccessInstructionDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<ListingAccessInstruction> entities = cache.Get<IEnumerable<ListingAccessInstruction>>(Cache.LISTINGACCESSINSTRUCTIONS.ToString()) ?? new List<ListingAccessInstruction>();
                ListingAccessInstruction? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.ListingAccessInstructionID == ID);
                }
                else
                {
                    match = await this.listingaccessinstructionRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<ListingAccessInstructionDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ListingAccessInstruction by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ListingAccessInstructionDto> CreateListingAccessInstruction(ListingAccessInstructionDto listingaccessinstructionDto)
        {
            ListingAccessInstruction listingAccessInstruction = new ListingAccessInstruction();
            IEnumerable<ListingAccessInstruction?> checkEntity;
            try
            {
                checkEntity = await this.listingaccessinstructionRepository.Find(x => x.InstructionType!.ToLower().Trim() == listingaccessinstructionDto.InstructionType!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    listingAccessInstruction = this.mapper.Map<ListingAccessInstruction>(listingaccessinstructionDto);
                    listingAccessInstruction.ListingAccessInstructionID = Guid.NewGuid();
                    listingAccessInstruction.LeaseID = listingaccessinstructionDto.LeaseID == Guid.Empty ? null : listingaccessinstructionDto.LeaseID;
                    listingAccessInstruction.CapturedDate = DateTime.UtcNow;
                    listingAccessInstruction = await listingaccessinstructionRepository.Create(listingAccessInstruction) ?? new ListingAccessInstruction();
                    await listingaccessinstructionRepository.Save();
                    cache.Remove(Cache.LISTINGACCESSINSTRUCTIONS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating ListingAccessInstruction. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<ListingAccessInstructionDto>(listingAccessInstruction);
        }

        /// <inheritdoc/>
        public async Task<ListingAccessInstructionDto?> UpdateListingAccessInstruction(Guid id, ListingAccessInstructionDto listingaccessinstructionDto)
        {
            try
            {
                var existing = await this.listingaccessinstructionRepository.GetByID(id);
                if (existing == null)
                    return null;
                ListingAccessInstruction listingAccessInstruction = this.mapper.Map<ListingAccessInstruction>(listingaccessinstructionDto);
                listingAccessInstruction = await listingaccessinstructionRepository.Update(listingAccessInstruction) ?? new ListingAccessInstruction();
                await listingaccessinstructionRepository.Save();
                cache.Remove(Cache.LISTINGACCESSINSTRUCTIONS.ToString());
                listingaccessinstructionDto = this.mapper.Map<ListingAccessInstructionDto>(listingAccessInstruction);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating ListingAccessInstruction. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return listingaccessinstructionDto;
        }

        /// <inheritdoc/>
        public async Task DeleteListingAccessInstruction(Guid ID)
        {
            try
            {
                var listingAccessInstruction = await this.listingaccessinstructionRepository.GetByID(ID);
                if (listingAccessInstruction == null)
                    throw new KeyNotFoundException("ListingAccessInstruction with the specified ID was not found.");
                await listingaccessinstructionRepository.Delete(listingAccessInstruction);
                await listingaccessinstructionRepository.Save();
                cache.Remove(Cache.LISTINGACCESSINSTRUCTIONS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting ListingAccessInstruction . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}