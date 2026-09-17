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
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace Arcora.Api.Services.Implementations
{
    public class ListingPhotoService : IListingPhotoService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<ListingPhotoService> logger;
        private readonly IListingPhotoRepository listingphotoRepository;
        private readonly IFileStorageService fileStorageService;
        private readonly IOptions<CacheConfiguration> _options;
        public ListingPhotoService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<ListingPhotoService> logger, IListingPhotoRepository listingphotoRepository, IFileStorageService fileStorageService)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.listingphotoRepository = listingphotoRepository;
            this.fileStorageService = fileStorageService;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<ListingPhotoDto>> GetAll(Paging paging)
        {
            IEnumerable<ListingPhoto> entities;
            try
            {
                entities = cache.Get<IEnumerable<ListingPhoto>>(Cache.LISTINGPHOTOS.ToString()) ?? new List<ListingPhoto>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.listingphotoRepository.GetListingPhotoAsync())?.Where(x => x != null) ?? new List<ListingPhoto>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<ListingPhoto>>(Cache.LISTINGPHOTOS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ListingPhoto by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<ListingPhotoDto>
                {
                    Data = new List<ListingPhotoDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<ListingPhoto> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Url) && x.Url.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.ListingPhotoID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<ListingPhotoDto>>(pagedEntities).ToList();
            foreach (var dto in pagedDtos)
            {
                await PopulateReadUrlAsync(dto);
            }

            return new PagedResult<ListingPhotoDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<ListingPhotoDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<ListingPhoto> entities = cache.Get<IEnumerable<ListingPhoto>>(Cache.LISTINGPHOTOS.ToString()) ?? new List<ListingPhoto>();
                ListingPhoto? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.ListingPhotoID == ID);
                }
                else
                {
                    match = await this.listingphotoRepository.GetByID(ID);
                }

                if (match == null)
                    return null;

                var dto = this.mapper.Map<ListingPhotoDto>(match);
                await PopulateReadUrlAsync(dto);
                return dto;
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ListingPhoto by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<List<ListingPhotoDto>?> GetPhotosByCapturedBy(string capturedBy)
        {
            try
            {
                IEnumerable<ListingPhoto> entities = await this.listingphotoRepository.FindWhere(x => x.CapturedBy == capturedBy);

                if (entities == null || !entities.Any())
                    return null;

                var pagedDtos = this.mapper.Map<List<ListingPhotoDto>>(entities);
                foreach (var dto in pagedDtos)
                {
                    await PopulateReadUrlAsync(dto);
                }
                return pagedDtos;
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ListingPhoto by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ListingPhotoDto> CreateListingPhoto(ListingPhotoDto listingphotoDto)
        {
            ListingPhoto listingPhoto = new ListingPhoto();
            IEnumerable<ListingPhoto?> checkEntity;
            try
            {
                var normalizedUrl = listingphotoDto.Url?.ToLower().Trim();
                checkEntity = await this.listingphotoRepository.Find(x => x.Url != null && x.Url.ToLower().Trim() == normalizedUrl);
                if (checkEntity == null || !checkEntity.Any())
                {
                    listingPhoto = this.mapper.Map<ListingPhoto>(listingphotoDto);
                    listingPhoto.ListingPhotoID = Guid.NewGuid();
                    listingPhoto.CapturedDate = DateTime.UtcNow;
                    listingPhoto = await listingphotoRepository.Create(listingPhoto) ?? new ListingPhoto();
                    await listingphotoRepository.Save();
                    cache.Remove(Cache.LISTINGPHOTOS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating ListingPhoto. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<ListingPhotoDto>(listingPhoto);
        }

        /// <inheritdoc/>
        public async Task<ListingPhotoDto?> UpdateListingPhoto(Guid id, ListingPhotoDto listingphotoDto)
        {
            try
            {
                var existing = await this.listingphotoRepository.GetByID(id);
                if (existing == null)
                    return null;
                ListingPhoto listingPhoto = this.mapper.Map<ListingPhoto>(listingphotoDto);
                listingPhoto = await listingphotoRepository.Update(listingPhoto) ?? new ListingPhoto();
                await listingphotoRepository.Save();
                cache.Remove(Cache.LISTINGPHOTOS.ToString());
                listingphotoDto = this.mapper.Map<ListingPhotoDto>(listingPhoto);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating ListingPhoto. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return listingphotoDto;
        }

        /// <inheritdoc/>
        public async Task DeleteListingPhoto(Guid ID)
        {
            try
            {
                var listingPhoto = await this.listingphotoRepository.GetByID(ID);
                if (listingPhoto == null)
                    throw new KeyNotFoundException("ListingPhoto with the specified ID was not found.");

                // Remove the underlying blob so storage and metadata stay in sync.
                if (!string.IsNullOrWhiteSpace(listingPhoto.StorageReference))
                {
                    try
                    {
                        await fileStorageService.DeleteAsync(StorageCategory.Image, listingPhoto.StorageReference!);
                    }
                    catch (Exception blobEx)
                    {
                        logger.LogError(blobEx, "Failed to delete blob '{Reference}' for ListingPhoto {ID}. Timestamp: {Timestamp}", listingPhoto.StorageReference, ID, DateTime.UtcNow);
                    }
                }

                await listingphotoRepository.Delete(listingPhoto);
                await listingphotoRepository.Save();
                cache.Remove(Cache.LISTINGPHOTOS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting ListingPhoto . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ListingPhotoDto> UploadListingPhoto(Arcora.Api.Models.ListingPhotoUploadRequest request, CancellationToken cancellationToken = default)
        {
            if (request?.File == null || request.File.Length == 0)
                throw new ArgumentException("No file was provided.");

            // Allow common image types
            var allowedContentTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
            var ext = Path.GetExtension(request.File.FileName)?.ToLowerInvariant();
            var isAllowedExt = new[] { ".jpg", ".jpeg", ".png", ".webp" }.Contains(ext);
            if (!allowedContentTypes.Contains(request.File.ContentType?.ToLowerInvariant()) && !isAllowedExt)
                throw new ArgumentException("Only JPEG/PNG/WebP images are allowed for listing photos.");

            try
            {
                BlobUploadResult uploadResult;
                await using (var stream = request.File.OpenReadStream())
                {
                    uploadResult = await fileStorageService.UploadAsync(
                        Enums.StorageCategory.Image,
                        request.File.FileName,
                        stream,
                        request.File.ContentType,
                        cancellationToken);
                }

                var listingPhoto = new ListingPhoto
                {
                    ListingPhotoID = Guid.NewGuid(),
                    ListingID = request.ListingID,
                    Url = uploadResult.Uri,
                    StorageProvider = "AZURE_BLOB",
                    StorageContainer = uploadResult.Container,
                    StorageReference = uploadResult.BlobName,
                    Location = request.Location,
                    Caption = request.Caption,
                    AltText = request.AltText,
                    DisplayOrder = request.DisplayOrder,
                    IsCoverPhoto = request.IsCoverPhoto,
                    CapturedBy = request.CapturedBy,
                    UserID = request.UserID,
                    CapturedDate = DateTime.UtcNow
                };

                listingPhoto = await listingphotoRepository.Create(listingPhoto) ?? new ListingPhoto();
                await listingphotoRepository.Save();
                cache.Remove(Cache.LISTINGPHOTOS.ToString());

                var dto = this.mapper.Map<ListingPhotoDto>(listingPhoto);
                await PopulateReadUrlAsync(dto);
                return dto;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while uploading ListingPhoto. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <summary>
        /// Populates the DTO's <see cref="ListingPhotoDto.Url"/> with a freshly generated, short-lived
        /// read-only SAS URL. Failures are logged but never block the response.
        /// </summary>
        private async Task PopulateReadUrlAsync(ListingPhotoDto? dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.StorageReference))
                return;

            try
            {
                dto.Url = await fileStorageService.GetReadSasUrlAsync(StorageCategory.Image, dto.StorageReference!);
            }
            catch (Exception er)
            {
                logger.LogError(er, "Failed to generate read SAS URL for ListingPhoto {ID}. Timestamp: {Timestamp}", dto.ListingPhotoID, DateTime.UtcNow);
            }
        }
    }
}