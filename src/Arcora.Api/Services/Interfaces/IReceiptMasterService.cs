// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IReceiptMasterService
    {
        /// <summary>
        /// Retrieves all receipt masters with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<ReceiptMasterDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a receipt master by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<ReceiptMasterDto?> GetID(long ID);
        /// <summary>
        /// Creates a new receipt master entry.
        /// </summary>
        /// <param name = "receiptMasterDto"></param>
        /// <returns></returns>
        Task<ReceiptMasterDto> CreateReceiptMaster(ReceiptMasterDto receiptMasterDto);
        /// <summary>
        /// Updates an existing receipt master entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "receiptmasterDto"></param>
        /// <returns></returns>
        Task<ReceiptMasterDto?> UpdateReceiptMaster(long id, ReceiptMasterDto receiptmasterDto);
        /// <summary>
        /// Deletes a receiptmaster entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteReceiptMaster(long ID);
    }
}