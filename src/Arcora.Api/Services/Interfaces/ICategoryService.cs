// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface ICategoryService
    {
        /// <summary>
        /// Retrieves all categories with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<CategoryDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a category by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<CategoryDto?> GetID(int ID);
        /// <summary>
        /// Creates a new category entry.
        /// </summary>
        /// <param name = "categoryDto"></param>
        /// <returns></returns>
        Task<CategoryDto> CreateCategory(CategoryDto categoryDto);
        /// <summary>
        /// Updates an existing category entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "categoryDto"></param>
        /// <returns></returns>
        Task<CategoryDto?> UpdateCategory(int id, CategoryDto categoryDto);
        /// <summary>
        /// Deletes a category entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteCategory(int ID);
    }
}