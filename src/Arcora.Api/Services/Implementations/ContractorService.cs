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
    public class ContractorService : IContractorService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<ContractorService> logger;
        private readonly IContractorRepository contractorRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public ContractorService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<ContractorService> logger, IContractorRepository contractorRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.contractorRepository = contractorRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<ContractorDto>> GetAll(Paging paging)
        {
            IEnumerable<Contractor> entities;
            try
            {
                entities = cache.Get<IEnumerable<Contractor>>(Cache.CONTRACTORS.ToString()) ?? new List<Contractor>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.contractorRepository.GetContractorAsync())?.Where(x => x != null) ?? new List<Contractor>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<Contractor>>(Cache.CONTRACTORS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Contractor by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<ContractorDto>
                {
                    Data = new List<ContractorDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<Contractor> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.CompanyName) && x.CompanyName.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.ContractorID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<ContractorDto>>(pagedEntities);
            return new PagedResult<ContractorDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<ContractorDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<Contractor> entities = cache.Get<IEnumerable<Contractor>>(Cache.CONTRACTORS.ToString()) ?? new List<Contractor>();
                Contractor? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.ContractorID == ID);
                }
                else
                {
                    match = await this.contractorRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<ContractorDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Contractor by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ContractorDto> CreateContractor(ContractorDto contractorDto)
        {
            Contractor contractor = new Contractor();
            IEnumerable<Contractor?> checkEntity;
            try
            {
                checkEntity = await this.contractorRepository.Find(x => x.CompanyName!.ToLower().Trim() == contractorDto.CompanyName!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    contractor = this.mapper.Map<Contractor>(contractorDto);
                    contractor.ContractorID = Guid.NewGuid();
                    contractor.CategoryID = contractorDto.CategoryID == 0 ? null : contractorDto.CategoryID;
                    contractor.CapturedDate = DateTime.UtcNow;
                    contractor = await contractorRepository.Create(contractor) ?? new Contractor();
                    await contractorRepository.Save();
                    cache.Remove(Cache.CONTRACTORS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating Contractor. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<ContractorDto>(contractor);
        }

        /// <inheritdoc/>
        public async Task<ContractorDto?> UpdateContractor(Guid id, ContractorDto contractorDto)
        {
            try
            {
                var existing = await this.contractorRepository.GetByID(id);
                if (existing == null)
                    return null;
                Contractor contractor = this.mapper.Map<Contractor>(contractorDto);
                contractor = await contractorRepository.Update(contractor) ?? new Contractor();
                await contractorRepository.Save();
                cache.Remove(Cache.CONTRACTORS.ToString());
                contractorDto = this.mapper.Map<ContractorDto>(contractor);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating Contractor. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return contractorDto;
        }

        /// <inheritdoc/>
        public async Task DeleteContractor(Guid ID)
        {
            try
            {
                var contractor = await this.contractorRepository.GetByID(ID);
                if (contractor == null)
                    throw new KeyNotFoundException("Contractor with the specified ID was not found.");
                await contractorRepository.Delete(contractor);
                await contractorRepository.Save();
                cache.Remove(Cache.CONTRACTORS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting Contractor . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ContractorDto?> UpdateContractorStatus(Guid id, string status)
        {
            var contractor = await contractorRepository.GetByID(id);
            if (contractor == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                contractor.Status = "Pending";
            }
            else
            {
                contractor.Status = status;
            }

            await contractorRepository.Update(contractor);
            await contractorRepository.Save();
            cache.Remove(Cache.CONTRACTORS.ToString());
            return this.mapper.Map<ContractorDto>(contractor);
        }
    }
}