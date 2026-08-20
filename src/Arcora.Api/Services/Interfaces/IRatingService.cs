// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IRatingService
    {
        /// <summary>
        /// Retrieves all ratings with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<RatingDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a rating by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<RatingDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new rating entry.
        /// </summary>
        /// <param name = "ratingDto"></param>
        /// <returns></returns>
        Task<RatingDto> CreateRating(RatingDto ratingDto);
        /// <summary>
        /// Updates an existing rating entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "ratingDto"></param>
        /// <returns></returns>
        Task<RatingDto?> UpdateRating(Guid id, RatingDto ratingDto);
        /// <summary>
        /// Deletes a rating entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteRating(Guid ID);
    }
}