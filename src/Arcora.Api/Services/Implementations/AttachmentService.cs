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
    public class AttachmentService : IAttachmentService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<AttachmentService> logger;
        private readonly IAttachmentRepository attachmentRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public AttachmentService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<AttachmentService> logger, IAttachmentRepository attachmentRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.attachmentRepository = attachmentRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<AttachmentDto>> GetAll(Paging paging)
        {
            IEnumerable<Attachment> entities;
            try
            {
                entities = cache.Get<IEnumerable<Attachment>>(Cache.ATTACHMENTS.ToString()) ?? new List<Attachment>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.attachmentRepository.GetAll())?.Where(x => x != null).Cast<Attachment>().ToList() ?? new List<Attachment>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<Attachment>>(Cache.ATTACHMENTS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Attachment by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<AttachmentDto>
                {
                    Data = new List<AttachmentDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<Attachment> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.FileName) && x.FileName.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.AttachmentID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<AttachmentDto>>(pagedEntities);
            return new PagedResult<AttachmentDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<AttachmentDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<Attachment> entities = cache.Get<IEnumerable<Attachment>>(Cache.ATTACHMENTS.ToString()) ?? new List<Attachment>();
                Attachment? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.AttachmentID == ID);
                }
                else
                {
                    match = await this.attachmentRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<AttachmentDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Attachment by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<AttachmentDto> CreateAttachment(AttachmentDto attachmentDto)
        {
            Attachment attachment = new Attachment();
            IEnumerable<Attachment?> checkEntity;
            try
            {
                checkEntity = await this.attachmentRepository.Find(x => x.FileName!.ToLower().Trim() == attachmentDto.FileName!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    attachment = this.mapper.Map<Attachment>(attachmentDto);
                    attachment.AttachmentID = Guid.NewGuid();
                    attachment.CapturedDate = DateTime.UtcNow;
                    attachment = await attachmentRepository.Create(attachment) ?? new Attachment();
                    await attachmentRepository.Save();
                    cache.Remove(Cache.ATTACHMENTS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating Attachment. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<AttachmentDto>(attachment);
        }

        /// <inheritdoc/>
        public async Task<AttachmentDto?> UpdateAttachment(Guid id, AttachmentDto attachmentDto)
        {
            try
            {
                var existing = await this.attachmentRepository.GetByID(id);
                if (existing == null)
                    return null;
                Attachment attachment = this.mapper.Map<Attachment>(attachmentDto);
                attachment = await attachmentRepository.Update(attachment) ?? new Attachment();
                await attachmentRepository.Save();
                cache.Remove(Cache.ATTACHMENTS.ToString());
                attachmentDto = this.mapper.Map<AttachmentDto>(attachment);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating Attachment. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return attachmentDto;
        }

        /// <inheritdoc/>
        public async Task DeleteAttachment(Guid ID)
        {
            try
            {
                var attachment = await this.attachmentRepository.GetByID(ID);
                if (attachment == null)
                    throw new KeyNotFoundException("Attachment with the specified ID was not found.");
                await attachmentRepository.Delete(attachment);
                await attachmentRepository.Save();
                cache.Remove(Cache.ATTACHMENTS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting Attachment . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}