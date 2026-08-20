// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IInvoiceMasterService
    {
        /// <summary>
        /// Retrieves all invoice masters with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<InvoiceMasterDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a invoice master by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<InvoiceMasterDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new invoice master entry.
        /// </summary>
        /// <param name = "invoiceMasterDto"></param>
        /// <returns></returns>
        Task<InvoiceMasterDto> CreateInvoiceMaster(InvoiceMasterDto invoiceMasterDto);
        /// <summary>
        /// Updates an existing invoice master entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "invoicemasterDto"></param>
        /// <returns></returns>
        Task<InvoiceMasterDto?> UpdateInvoiceMaster(Guid id, InvoiceMasterDto invoicemasterDto);
        /// <summary>
        /// Deletes a invoicemaster entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteInvoiceMaster(Guid ID);
        /// <summary>
        /// Updates the status of a invoicemaster by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the invoicemaster to update.</param>
        /// <param name = "status">The new status value to assign to the invoicemaster.</param>
        /// <returns>
        /// Returns <see cref = "InvoiceMasterDto"/> with the updated invoicemaster if successful, or null if not found.
        /// </returns>
        Task<InvoiceMasterDto?> UpdateInvoiceMasterStatus(Guid id, string status);
    }
}