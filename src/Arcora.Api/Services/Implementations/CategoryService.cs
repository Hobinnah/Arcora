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
    public class CategoryService : ICategoryService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<CategoryService> logger;
        private readonly ICategoryRepository categoryRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public CategoryService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<CategoryService> logger, ICategoryRepository categoryRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.categoryRepository = categoryRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<CategoryDto>> GetAll(Paging paging)
        {
            IEnumerable<Category> entities;
            try
            {
                entities = cache.Get<IEnumerable<Category>>(Cache.CATEGORIES.ToString()) ?? new List<Category>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.categoryRepository.GetAll())?.Where(x => x != null).Cast<Category>().ToList() ?? new List<Category>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<Category>>(Cache.CATEGORIES.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Category by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<CategoryDto>
                {
                    Data = new List<CategoryDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<Category> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.CategoryID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<CategoryDto>>(pagedEntities);
            return new PagedResult<CategoryDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<CategoryDto?> GetID(int ID)
        {
            try
            {
                IEnumerable<Category> entities = cache.Get<IEnumerable<Category>>(Cache.CATEGORIES.ToString()) ?? new List<Category>();
                Category? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.CategoryID == ID);
                }
                else
                {
                    match = await this.categoryRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<CategoryDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Category by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<CategoryDto> CreateCategory(CategoryDto categoryDto)
        {
            Category category = new Category();
            IEnumerable<Category?> checkEntity;
            try
            {
                checkEntity = await this.categoryRepository.Find(x => x.Name!.ToLower().Trim() == categoryDto.Name!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    category = this.mapper.Map<Category>(categoryDto);
                    category.CapturedDate = DateTime.UtcNow;
                    category = await categoryRepository.Create(category) ?? new Category();
                    await categoryRepository.Save();
                    cache.Remove(Cache.CATEGORIES.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating Category. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<CategoryDto>(category);
        }

        /// <inheritdoc/>
        public async Task<CategoryDto?> UpdateCategory(int id, CategoryDto categoryDto)
        {
            try
            {
                var existing = await this.categoryRepository.GetByID(id);
                if (existing == null)
                    return null;
                Category category = this.mapper.Map<Category>(categoryDto);
                category = await categoryRepository.Update(category) ?? new Category();
                await categoryRepository.Save();
                cache.Remove(Cache.CATEGORIES.ToString());
                categoryDto = this.mapper.Map<CategoryDto>(category);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating Category. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return categoryDto;
        }

        /// <inheritdoc/>
        public async Task DeleteCategory(int ID)
        {
            try
            {
                var category = await this.categoryRepository.GetByID(ID);
                if (category == null)
                    throw new KeyNotFoundException("Category with the specified ID was not found.");
                await categoryRepository.Delete(category);
                await categoryRepository.Save();
                cache.Remove(Cache.CATEGORIES.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting Category . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}