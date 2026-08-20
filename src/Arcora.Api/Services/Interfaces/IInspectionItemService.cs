// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IInspectionItemService
    {
        /// <summary>
        /// Retrieves all inspection items with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<InspectionItemDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a inspection item by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<InspectionItemDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new inspection item entry.
        /// </summary>
        /// <param name = "inspectionItemDto"></param>
        /// <returns></returns>
        Task<InspectionItemDto> CreateInspectionItem(InspectionItemDto inspectionItemDto);
        /// <summary>
        /// Updates an existing inspection item entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "inspectionitemDto"></param>
        /// <returns></returns>
        Task<InspectionItemDto?> UpdateInspectionItem(Guid id, InspectionItemDto inspectionitemDto);
        /// <summary>
        /// Deletes a inspectionitem entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteInspectionItem(Guid ID);
    }
}