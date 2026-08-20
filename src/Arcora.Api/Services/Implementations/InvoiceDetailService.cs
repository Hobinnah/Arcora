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
    public class InvoiceDetailService : IInvoiceDetailService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<InvoiceDetailService> logger;
        private readonly IInvoiceDetailRepository invoicedetailRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public InvoiceDetailService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<InvoiceDetailService> logger, IInvoiceDetailRepository invoicedetailRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.invoicedetailRepository = invoicedetailRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<InvoiceDetailDto>> GetAll(Paging paging)
        {
            IEnumerable<InvoiceDetail> entities;
            try
            {
                entities = cache.Get<IEnumerable<InvoiceDetail>>(Cache.INVOICEDETAILS.ToString()) ?? new List<InvoiceDetail>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.invoicedetailRepository.GetInvoiceDetailAsync())?.Where(x => x != null) ?? new List<InvoiceDetail>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<InvoiceDetail>>(Cache.INVOICEDETAILS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching InvoiceDetail by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<InvoiceDetailDto>
                {
                    Data = new List<InvoiceDetailDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<InvoiceDetail> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Description) && x.Description.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.InvoiceDetailID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<InvoiceDetailDto>>(pagedEntities);
            return new PagedResult<InvoiceDetailDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<InvoiceDetailDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<InvoiceDetail> entities = cache.Get<IEnumerable<InvoiceDetail>>(Cache.INVOICEDETAILS.ToString()) ?? new List<InvoiceDetail>();
                InvoiceDetail? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.InvoiceDetailID == ID);
                }
                else
                {
                    match = await this.invoicedetailRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<InvoiceDetailDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching InvoiceDetail by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<InvoiceDetailDto> CreateInvoiceDetail(InvoiceDetailDto invoicedetailDto)
        {
            InvoiceDetail invoiceDetail = new InvoiceDetail();
            IEnumerable<InvoiceDetail?> checkEntity;
            try
            {
                checkEntity = await this.invoicedetailRepository.Find(x => x.Description!.ToLower().Trim() == invoicedetailDto.Description!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    invoiceDetail = this.mapper.Map<InvoiceDetail>(invoicedetailDto);
                    invoiceDetail.InvoiceDetailID = Guid.NewGuid();
                    invoiceDetail.LeaseRecurringChargeID = invoicedetailDto.LeaseRecurringChargeID == Guid.Empty ? null : invoicedetailDto.LeaseRecurringChargeID;
                    invoiceDetail.FeeID = invoicedetailDto.FeeID == Guid.Empty ? null : invoicedetailDto.FeeID;
                    invoiceDetail.CapturedDate = DateTime.UtcNow;
                    invoiceDetail = await invoicedetailRepository.Create(invoiceDetail) ?? new InvoiceDetail();
                    await invoicedetailRepository.Save();
                    cache.Remove(Cache.INVOICEDETAILS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating InvoiceDetail. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<InvoiceDetailDto>(invoiceDetail);
        }

        /// <inheritdoc/>
        public async Task<InvoiceDetailDto?> UpdateInvoiceDetail(Guid id, InvoiceDetailDto invoicedetailDto)
        {
            try
            {
                var existing = await this.invoicedetailRepository.GetByID(id);
                if (existing == null)
                    return null;
                InvoiceDetail invoiceDetail = this.mapper.Map<InvoiceDetail>(invoicedetailDto);
                invoiceDetail = await invoicedetailRepository.Update(invoiceDetail) ?? new InvoiceDetail();
                await invoicedetailRepository.Save();
                cache.Remove(Cache.INVOICEDETAILS.ToString());
                invoicedetailDto = this.mapper.Map<InvoiceDetailDto>(invoiceDetail);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating InvoiceDetail. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return invoicedetailDto;
        }

        /// <inheritdoc/>
        public async Task DeleteInvoiceDetail(Guid ID)
        {
            try
            {
                var invoiceDetail = await this.invoicedetailRepository.GetByID(ID);
                if (invoiceDetail == null)
                    throw new KeyNotFoundException("InvoiceDetail with the specified ID was not found.");
                await invoicedetailRepository.Delete(invoiceDetail);
                await invoicedetailRepository.Save();
                cache.Remove(Cache.INVOICEDETAILS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting InvoiceDetail . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}