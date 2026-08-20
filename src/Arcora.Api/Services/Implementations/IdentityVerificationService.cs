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
    public class IdentityVerificationService : IIdentityVerificationService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<IdentityVerificationService> logger;
        private readonly IIdentityVerificationRepository identityverificationRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public IdentityVerificationService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<IdentityVerificationService> logger, IIdentityVerificationRepository identityverificationRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.identityverificationRepository = identityverificationRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<IdentityVerificationDto>> GetAll(Paging paging)
        {
            IEnumerable<IdentityVerification> entities;
            try
            {
                entities = cache.Get<IEnumerable<IdentityVerification>>(Cache.IDENTITYVERIFICATIONS.ToString()) ?? new List<IdentityVerification>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.identityverificationRepository.GetIdentityVerificationAsync())?.Where(x => x != null) ?? new List<IdentityVerification>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<IdentityVerification>>(Cache.IDENTITYVERIFICATIONS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }

                if (entities is not null && paging.UserID > 0)
                {
                    entities = entities.Where(c => c.UserID == paging.UserID);
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching IdentityVerification by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<IdentityVerificationDto>
                {
                    Data = new List<IdentityVerificationDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<IdentityVerification> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.VerificationType) && x.VerificationType.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.IdentityVerificationID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<IdentityVerificationDto>>(pagedEntities);
            return new PagedResult<IdentityVerificationDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<IdentityVerificationDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<IdentityVerification> entities = cache.Get<IEnumerable<IdentityVerification>>(Cache.IDENTITYVERIFICATIONS.ToString()) ?? new List<IdentityVerification>();
                IdentityVerification? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.IdentityVerificationID == ID);
                }
                else
                {
                    match = await this.identityverificationRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<IdentityVerificationDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching IdentityVerification by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IdentityVerificationDto> CreateIdentityVerification(IdentityVerificationDto identityverificationDto)
        {
            IdentityVerification identityVerification = new IdentityVerification();
            IEnumerable<IdentityVerification?> checkEntity;
            try
            {
                checkEntity = await this.identityverificationRepository.Find(x => x.VerificationType!.ToLower().Trim() == identityverificationDto.VerificationType!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    identityVerification = this.mapper.Map<IdentityVerification>(identityverificationDto);
                    identityVerification.IdentityVerificationID = Guid.NewGuid();
                    identityVerification.ProviderReferenceID = string.IsNullOrEmpty(identityverificationDto.ProviderReferenceID) ? null : identityverificationDto.ProviderReferenceID;
                    identityVerification.CapturedDate = DateTime.UtcNow;
                    identityVerification = await identityverificationRepository.Create(identityVerification) ?? new IdentityVerification();
                    await identityverificationRepository.Save();
                    cache.Remove(Cache.IDENTITYVERIFICATIONS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating IdentityVerification. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<IdentityVerificationDto>(identityVerification);
        }

        /// <inheritdoc/>
        public async Task<IdentityVerificationDto?> UpdateIdentityVerification(Guid id, IdentityVerificationDto identityverificationDto)
        {
            try
            {
                var existing = await this.identityverificationRepository.GetByID(id);
                if (existing == null)
                    return null;
                IdentityVerification identityVerification = this.mapper.Map<IdentityVerification>(identityverificationDto);
                identityVerification = await identityverificationRepository.Update(identityVerification) ?? new IdentityVerification();
                await identityverificationRepository.Save();
                cache.Remove(Cache.IDENTITYVERIFICATIONS.ToString());
                identityverificationDto = this.mapper.Map<IdentityVerificationDto>(identityVerification);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating IdentityVerification. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return identityverificationDto;
        }

        /// <inheritdoc/>
        public async Task DeleteIdentityVerification(Guid ID)
        {
            try
            {
                var identityVerification = await this.identityverificationRepository.GetByID(ID);
                if (identityVerification == null)
                    throw new KeyNotFoundException("IdentityVerification with the specified ID was not found.");
                await identityverificationRepository.Delete(identityVerification);
                await identityverificationRepository.Save();
                cache.Remove(Cache.IDENTITYVERIFICATIONS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting IdentityVerification . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IdentityVerificationDto?> UpdateIdentityVerificationStatus(Guid id, string status)
        {
            var identityVerification = await identityverificationRepository.GetByID(id);
            if (identityVerification == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                identityVerification.Status = "Pending";
            }
            else
            {
                identityVerification.Status = status;
            }

            await identityverificationRepository.Update(identityVerification);
            await identityverificationRepository.Save();
            cache.Remove(Cache.IDENTITYVERIFICATIONS.ToString());
            return this.mapper.Map<IdentityVerificationDto>(identityVerification);
        }
    }
}