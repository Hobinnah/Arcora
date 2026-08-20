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
    public class ReservationHoldService : IReservationHoldService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<ReservationHoldService> logger;
        private readonly IReservationHoldRepository reservationholdRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public ReservationHoldService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<ReservationHoldService> logger, IReservationHoldRepository reservationholdRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.reservationholdRepository = reservationholdRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<ReservationHoldDto>> GetAll(Paging paging)
        {
            IEnumerable<ReservationHold> entities;
            try
            {
                entities = cache.Get<IEnumerable<ReservationHold>>(Cache.RESERVATIONHOLDS.ToString()) ?? new List<ReservationHold>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.reservationholdRepository.GetReservationHoldAsync())?.Where(x => x != null) ?? new List<ReservationHold>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<ReservationHold>>(Cache.RESERVATIONHOLDS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ReservationHold by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<ReservationHoldDto>
                {
                    Data = new List<ReservationHoldDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<ReservationHold> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.HoldReason) && x.HoldReason.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.ReservationHoldID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<ReservationHoldDto>>(pagedEntities);
            return new PagedResult<ReservationHoldDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<ReservationHoldDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<ReservationHold> entities = cache.Get<IEnumerable<ReservationHold>>(Cache.RESERVATIONHOLDS.ToString()) ?? new List<ReservationHold>();
                ReservationHold? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.ReservationHoldID == ID);
                }
                else
                {
                    match = await this.reservationholdRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<ReservationHoldDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ReservationHold by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ReservationHoldDto> CreateReservationHold(ReservationHoldDto reservationholdDto)
        {
            ReservationHold reservationHold = new ReservationHold();
            IEnumerable<ReservationHold?> checkEntity;
            try
            {
                checkEntity = await this.reservationholdRepository.Find(x => x.HoldReason!.ToLower().Trim() == reservationholdDto.HoldReason!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    reservationHold = this.mapper.Map<ReservationHold>(reservationholdDto);
                    reservationHold.ReservationHoldID = Guid.NewGuid();
                    reservationHold.RentalApplicationID = reservationholdDto.RentalApplicationID == Guid.Empty ? null : reservationholdDto.RentalApplicationID;
                    reservationHold.TenantID = reservationholdDto.TenantID == Guid.Empty ? null : reservationholdDto.TenantID;
                    reservationHold.CapturedDate = DateTime.UtcNow;
                    reservationHold = await reservationholdRepository.Create(reservationHold) ?? new ReservationHold();
                    await reservationholdRepository.Save();
                    cache.Remove(Cache.RESERVATIONHOLDS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating ReservationHold. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<ReservationHoldDto>(reservationHold);
        }

        /// <inheritdoc/>
        public async Task<ReservationHoldDto?> UpdateReservationHold(Guid id, ReservationHoldDto reservationholdDto)
        {
            try
            {
                var existing = await this.reservationholdRepository.GetByID(id);
                if (existing == null)
                    return null;
                ReservationHold reservationHold = this.mapper.Map<ReservationHold>(reservationholdDto);
                reservationHold = await reservationholdRepository.Update(reservationHold) ?? new ReservationHold();
                await reservationholdRepository.Save();
                cache.Remove(Cache.RESERVATIONHOLDS.ToString());
                reservationholdDto = this.mapper.Map<ReservationHoldDto>(reservationHold);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating ReservationHold. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return reservationholdDto;
        }

        /// <inheritdoc/>
        public async Task DeleteReservationHold(Guid ID)
        {
            try
            {
                var reservationHold = await this.reservationholdRepository.GetByID(ID);
                if (reservationHold == null)
                    throw new KeyNotFoundException("ReservationHold with the specified ID was not found.");
                await reservationholdRepository.Delete(reservationHold);
                await reservationholdRepository.Save();
                cache.Remove(Cache.RESERVATIONHOLDS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting ReservationHold . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ReservationHoldDto?> UpdateReservationHoldStatus(Guid id, string status)
        {
            var reservationHold = await reservationholdRepository.GetByID(id);
            if (reservationHold == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                reservationHold.Status = "Pending";
            }
            else
            {
                reservationHold.Status = status;
            }

            await reservationholdRepository.Update(reservationHold);
            await reservationholdRepository.Save();
            cache.Remove(Cache.RESERVATIONHOLDS.ToString());
            return this.mapper.Map<ReservationHoldDto>(reservationHold);
        }
    }
}