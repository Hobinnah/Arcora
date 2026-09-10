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
    public class LeaseDocumentsService : ILeaseDocumentsService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<LeaseDocumentsService> logger;
        private readonly ILeaseDocumentsRepository leasedocumentsRepository;
        private readonly IFileStorageService fileStorageService;
        private readonly IOptions<CacheConfiguration> _options;
        public LeaseDocumentsService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<LeaseDocumentsService> logger, ILeaseDocumentsRepository leasedocumentsRepository, IFileStorageService fileStorageService)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.leasedocumentsRepository = leasedocumentsRepository;
            this.fileStorageService = fileStorageService;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<LeaseDocumentsDto>> GetAll(Paging paging)
        {
            IEnumerable<LeaseDocuments> entities;
            try
            {
                entities = cache.Get<IEnumerable<LeaseDocuments>>(Cache.LEASEDOCUMENTS.ToString()) ?? new List<LeaseDocuments>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.leasedocumentsRepository.GetLeaseDocumentsAsync())?.Where(x => x != null) ?? new List<LeaseDocuments>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<LeaseDocuments>>(Cache.LEASEDOCUMENTS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching LeaseDocuments by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<LeaseDocumentsDto>
                {
                    Data = new List<LeaseDocumentsDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<LeaseDocuments> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.OriginalFilename) && x.OriginalFilename.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.LeaseDocumentID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<LeaseDocumentsDto>>(pagedEntities).ToList();
            foreach (var dto in pagedDtos)
            {
                await PopulateReadUrlAsync(dto);
            }

            return new PagedResult<LeaseDocumentsDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<LeaseDocumentsDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<LeaseDocuments> entities = cache.Get<IEnumerable<LeaseDocuments>>(Cache.LEASEDOCUMENTS.ToString()) ?? new List<LeaseDocuments>();
                LeaseDocuments? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.LeaseDocumentID == ID);
                }
                else
                {
                    match = await this.leasedocumentsRepository.GetByID(ID);
                }

                if (match == null)
                    return null;

                var dto = this.mapper.Map<LeaseDocumentsDto>(match);
                await PopulateReadUrlAsync(dto);
                return dto;
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching LeaseDocuments by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<LeaseDocumentsDto> CreateLeaseDocuments(LeaseDocumentsDto leasedocumentsDto)
        {
            LeaseDocuments leaseDocuments = new LeaseDocuments();
            IEnumerable<LeaseDocuments?> checkEntity;
            try
            {
                checkEntity = await this.leasedocumentsRepository.Find(x => x.OriginalFilename!.ToLower().Trim() == leasedocumentsDto.OriginalFilename!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    leaseDocuments = this.mapper.Map<LeaseDocuments>(leasedocumentsDto);
                    leaseDocuments.LeaseDocumentID = Guid.NewGuid();
                    leaseDocuments.LeaseRenewalID = leasedocumentsDto.LeaseRenewalID == Guid.Empty ? null : leasedocumentsDto.LeaseRenewalID;
                    leaseDocuments.CapturedDate = DateTime.UtcNow;
                    leaseDocuments = await leasedocumentsRepository.Create(leaseDocuments) ?? new LeaseDocuments();
                    await leasedocumentsRepository.Save();
                    cache.Remove(Cache.LEASEDOCUMENTS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating LeaseDocuments. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<LeaseDocumentsDto>(leaseDocuments);
        }

        /// <inheritdoc/>
        public async Task<LeaseDocumentsDto?> UpdateLeaseDocuments(Guid id, LeaseDocumentsDto leasedocumentsDto)
        {
            try
            {
                var existing = await this.leasedocumentsRepository.GetByID(id);
                if (existing == null)
                    return null;
                LeaseDocuments leaseDocuments = this.mapper.Map<LeaseDocuments>(leasedocumentsDto);
                leaseDocuments = await leasedocumentsRepository.Update(leaseDocuments) ?? new LeaseDocuments();
                await leasedocumentsRepository.Save();
                cache.Remove(Cache.LEASEDOCUMENTS.ToString());
                leasedocumentsDto = this.mapper.Map<LeaseDocumentsDto>(leaseDocuments);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating LeaseDocuments. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return leasedocumentsDto;
        }

        /// <inheritdoc/>
        public async Task DeleteLeaseDocuments(Guid ID)
        {
            try
            {
                var leaseDocuments = await this.leasedocumentsRepository.GetByID(ID);
                if (leaseDocuments == null)
                    throw new KeyNotFoundException("LeaseDocuments with the specified ID was not found.");

                // Remove the underlying blob so storage and metadata stay in sync.
                if (!string.IsNullOrWhiteSpace(leaseDocuments.StorageReference))
                {
                    try
                    {
                        await fileStorageService.DeleteAsync(StorageCategory.Document, leaseDocuments.StorageReference!);
                    }
                    catch (Exception blobEx)
                    {
                        logger.LogError(blobEx, "Failed to delete blob '{Reference}' for LeaseDocument {ID}. Timestamp: {Timestamp}", leaseDocuments.StorageReference, ID, DateTime.UtcNow);
                    }
                }

                await leasedocumentsRepository.Delete(leaseDocuments);
                await leasedocumentsRepository.Save();
                cache.Remove(Cache.LEASEDOCUMENTS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting LeaseDocuments . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<LeaseDocumentsDto> UploadLeaseDocument(LeaseDocumentUploadRequest request, CancellationToken cancellationToken = default)
        {
            if (request?.File == null || request.File.Length == 0)
                throw new ArgumentException("No file was provided.");

            // Enforce PDF only.
            var isPdfContentType = string.Equals(request.File.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase);
            var isPdfExtension = Path.GetExtension(request.File.FileName)?.Equals(".pdf", StringComparison.OrdinalIgnoreCase) ?? false;
            if (!isPdfContentType && !isPdfExtension)
                throw new ArgumentException("Only PDF documents are allowed for lease documents.");

            try
            {
                BlobUploadResult uploadResult;
                await using (var stream = request.File.OpenReadStream())
                {
                    uploadResult = await fileStorageService.UploadAsync(
                        StorageCategory.Document,
                        request.File.FileName,
                        stream,
                        "application/pdf",
                        cancellationToken);
                }

                var leaseDocuments = new LeaseDocuments
                {
                    LeaseDocumentID = Guid.NewGuid(),
                    LeaseID = request.LeaseID,
                    LeaseRenewalID = request.LeaseRenewalID == Guid.Empty ? null : request.LeaseRenewalID,
                    RentalApplicationID = request.RentalApplicationID == Guid.Empty ? null : request.RentalApplicationID,
                    ListingID = request.ListingID == Guid.Empty ? null : request.ListingID,
                    TenantID = request.TenantID == Guid.Empty ? null : request.TenantID,
                    DocumentType = request.DocumentType,
                    DocumentStatus = string.IsNullOrWhiteSpace(request.DocumentStatus) ? "DRAFT" : request.DocumentStatus,
                    OriginalFilename = request.File.FileName,
                    StorageProvider = "AZURE_BLOB",
                    StorageContainer = uploadResult.Container,
                    StorageReference = uploadResult.BlobName,
                    Url = uploadResult.Uri,
                    IsPrimary = request.IsPrimary,
                    GeneratedAt = DateTime.UtcNow,
                    CapturedBy = request.CapturedBy,
                    CapturedDate = DateTime.UtcNow
                };

                leaseDocuments = await leasedocumentsRepository.Create(leaseDocuments) ?? new LeaseDocuments();
                await leasedocumentsRepository.Save();
                cache.Remove(Cache.LEASEDOCUMENTS.ToString());

                var dto = this.mapper.Map<LeaseDocumentsDto>(leaseDocuments);
                await PopulateReadUrlAsync(dto);
                return dto;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while uploading LeaseDocument. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<(Stream Content, string? ContentType, string FileName)?> DownloadLeaseDocument(Guid ID, CancellationToken cancellationToken = default)
        {
            try
            {
                var leaseDocuments = await this.leasedocumentsRepository.GetByID(ID);
                if (leaseDocuments == null || string.IsNullOrWhiteSpace(leaseDocuments.StorageReference))
                    return null;

                var result = await fileStorageService.DownloadAsync(StorageCategory.Document, leaseDocuments.StorageReference!, cancellationToken);
                if (result == null)
                    return null;

                var fileName = string.IsNullOrWhiteSpace(leaseDocuments.OriginalFilename) ? "document.pdf" : leaseDocuments.OriginalFilename!;
                return (result.Value.Content, result.Value.ContentType ?? "application/pdf", fileName);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while downloading LeaseDocument {ID}. Timestamp: {Timestamp}", ID, DateTime.UtcNow);
                throw;
            }
        }

        /// <summary>
        /// Populates the DTO's <see cref="LeaseDocumentsDto.Url"/> with a freshly generated, short-lived
        /// read-only SAS URL. The persisted blob URL is private and cannot be opened directly, so a SAS
        /// URL is generated on every read. Failures are logged but never block the response.
        /// </summary>
        private async Task PopulateReadUrlAsync(LeaseDocumentsDto? dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.StorageReference))
                return;

            try
            {
                dto.Url = await fileStorageService.GetReadSasUrlAsync(StorageCategory.Document, dto.StorageReference!);
            }
            catch (Exception er)
            {
                logger.LogError(er, "Failed to generate read SAS URL for LeaseDocument {ID}. Timestamp: {Timestamp}", dto.LeaseDocumentID, DateTime.UtcNow);
            }
        }
    }
}