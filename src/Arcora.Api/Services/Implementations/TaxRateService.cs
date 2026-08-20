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
    public class TaxRateService : ITaxRateService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<TaxRateService> logger;
        private readonly ITaxRateRepository taxrateRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public TaxRateService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<TaxRateService> logger, ITaxRateRepository taxrateRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.taxrateRepository = taxrateRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<TaxRateDto>> GetAll(Paging paging)
        {
            IEnumerable<TaxRate> entities;
            try
            {
                entities = cache.Get<IEnumerable<TaxRate>>(Cache.TAXRATES.ToString()) ?? new List<TaxRate>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.taxrateRepository.GetAll())?.Where(x => x != null).Cast<TaxRate>().ToList() ?? new List<TaxRate>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<TaxRate>>(Cache.TAXRATES.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching TaxRate by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<TaxRateDto>
                {
                    Data = new List<TaxRateDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<TaxRate> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.TaxID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<TaxRateDto>>(pagedEntities);
            return new PagedResult<TaxRateDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<TaxRateDto?> GetID(int ID)
        {
            try
            {
                IEnumerable<TaxRate> entities = cache.Get<IEnumerable<TaxRate>>(Cache.TAXRATES.ToString()) ?? new List<TaxRate>();
                TaxRate? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.TaxID == ID);
                }
                else
                {
                    match = await this.taxrateRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<TaxRateDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching TaxRate by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<TaxRateDto> CreateTaxRate(TaxRateDto taxrateDto)
        {
            TaxRate taxRate = new TaxRate();
            IEnumerable<TaxRate?> checkEntity;
            try
            {
                checkEntity = await this.taxrateRepository.Find(x => x.Name!.ToLower().Trim() == taxrateDto.Name!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    taxRate = this.mapper.Map<TaxRate>(taxrateDto);
                    taxRate.CapturedDate = DateTime.UtcNow;
                    taxRate = await taxrateRepository.Create(taxRate) ?? new TaxRate();
                    await taxrateRepository.Save();
                    cache.Remove(Cache.TAXRATES.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating TaxRate. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<TaxRateDto>(taxRate);
        }

        /// <inheritdoc/>
        public async Task<TaxRateDto?> UpdateTaxRate(int id, TaxRateDto taxrateDto)
        {
            try
            {
                var existing = await this.taxrateRepository.GetByID(id);
                if (existing == null)
                    return null;
                TaxRate taxRate = this.mapper.Map<TaxRate>(taxrateDto);
                taxRate = await taxrateRepository.Update(taxRate) ?? new TaxRate();
                await taxrateRepository.Save();
                cache.Remove(Cache.TAXRATES.ToString());
                taxrateDto = this.mapper.Map<TaxRateDto>(taxRate);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating TaxRate. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return taxrateDto;
        }

        /// <inheritdoc/>
        public async Task DeleteTaxRate(int ID)
        {
            try
            {
                var taxRate = await this.taxrateRepository.GetByID(ID);
                if (taxRate == null)
                    throw new KeyNotFoundException("TaxRate with the specified ID was not found.");
                await taxrateRepository.Delete(taxRate);
                await taxrateRepository.Save();
                cache.Remove(Cache.TAXRATES.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting TaxRate . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}