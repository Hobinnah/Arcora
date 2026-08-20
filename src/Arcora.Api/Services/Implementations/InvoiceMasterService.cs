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
    public class InvoiceMasterService : IInvoiceMasterService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<InvoiceMasterService> logger;
        private readonly IInvoiceMasterRepository invoicemasterRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public InvoiceMasterService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<InvoiceMasterService> logger, IInvoiceMasterRepository invoicemasterRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.invoicemasterRepository = invoicemasterRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<InvoiceMasterDto>> GetAll(Paging paging)
        {
            IEnumerable<InvoiceMaster> entities;
            try
            {
                entities = cache.Get<IEnumerable<InvoiceMaster>>(Cache.INVOICEMASTERS.ToString()) ?? new List<InvoiceMaster>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.invoicemasterRepository.GetInvoiceMasterAsync())?.Where(x => x != null) ?? new List<InvoiceMaster>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<InvoiceMaster>>(Cache.INVOICEMASTERS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching InvoiceMaster by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<InvoiceMasterDto>
                {
                    Data = new List<InvoiceMasterDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<InvoiceMaster> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.InvoiceNumber) && x.InvoiceNumber.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.InvoiceMasterID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<InvoiceMasterDto>>(pagedEntities);
            return new PagedResult<InvoiceMasterDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<InvoiceMasterDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<InvoiceMaster> entities = cache.Get<IEnumerable<InvoiceMaster>>(Cache.INVOICEMASTERS.ToString()) ?? new List<InvoiceMaster>();
                InvoiceMaster? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.InvoiceMasterID == ID);
                }
                else
                {
                    match = await this.invoicemasterRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<InvoiceMasterDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching InvoiceMaster by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<InvoiceMasterDto> CreateInvoiceMaster(InvoiceMasterDto invoicemasterDto)
        {
            InvoiceMaster invoiceMaster = new InvoiceMaster();
            IEnumerable<InvoiceMaster?> checkEntity;
            try
            {
                checkEntity = await this.invoicemasterRepository.Find(x => x.InvoiceNumber!.ToLower().Trim() == invoicemasterDto.InvoiceNumber!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    invoiceMaster = this.mapper.Map<InvoiceMaster>(invoicemasterDto);
                    invoiceMaster.InvoiceMasterID = Guid.NewGuid();
                    invoiceMaster.LeaseRenewalID = invoicemasterDto.LeaseRenewalID == Guid.Empty ? null : invoicemasterDto.LeaseRenewalID;
                    invoiceMaster.CapturedDate = DateTime.UtcNow;
                    invoiceMaster = await invoicemasterRepository.Create(invoiceMaster) ?? new InvoiceMaster();
                    await invoicemasterRepository.Save();
                    cache.Remove(Cache.INVOICEMASTERS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating InvoiceMaster. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<InvoiceMasterDto>(invoiceMaster);
        }

        /// <inheritdoc/>
        public async Task<InvoiceMasterDto?> UpdateInvoiceMaster(Guid id, InvoiceMasterDto invoicemasterDto)
        {
            try
            {
                var existing = await this.invoicemasterRepository.GetByID(id);
                if (existing == null)
                    return null;
                InvoiceMaster invoiceMaster = this.mapper.Map<InvoiceMaster>(invoicemasterDto);
                invoiceMaster = await invoicemasterRepository.Update(invoiceMaster) ?? new InvoiceMaster();
                await invoicemasterRepository.Save();
                cache.Remove(Cache.INVOICEMASTERS.ToString());
                invoicemasterDto = this.mapper.Map<InvoiceMasterDto>(invoiceMaster);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating InvoiceMaster. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return invoicemasterDto;
        }

        /// <inheritdoc/>
        public async Task DeleteInvoiceMaster(Guid ID)
        {
            try
            {
                var invoiceMaster = await this.invoicemasterRepository.GetByID(ID);
                if (invoiceMaster == null)
                    throw new KeyNotFoundException("InvoiceMaster with the specified ID was not found.");
                await invoicemasterRepository.Delete(invoiceMaster);
                await invoicemasterRepository.Save();
                cache.Remove(Cache.INVOICEMASTERS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting InvoiceMaster . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<InvoiceMasterDto?> UpdateInvoiceMasterStatus(Guid id, string status)
        {
            var invoiceMaster = await invoicemasterRepository.GetByID(id);
            if (invoiceMaster == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                invoiceMaster.Status = "Pending";
            }
            else
            {
                invoiceMaster.Status = status;
            }

            await invoicemasterRepository.Update(invoiceMaster);
            await invoicemasterRepository.Save();
            cache.Remove(Cache.INVOICEMASTERS.ToString());
            return this.mapper.Map<InvoiceMasterDto>(invoiceMaster);
        }
    }
}