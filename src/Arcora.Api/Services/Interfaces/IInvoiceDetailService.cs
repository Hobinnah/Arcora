// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IInvoiceDetailService
    {
        /// <summary>
        /// Retrieves all invoice details with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<InvoiceDetailDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a invoice detail by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<InvoiceDetailDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new invoice detail entry.
        /// </summary>
        /// <param name = "invoiceDetailDto"></param>
        /// <returns></returns>
        Task<InvoiceDetailDto> CreateInvoiceDetail(InvoiceDetailDto invoiceDetailDto);
        /// <summary>
        /// Updates an existing invoice detail entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "invoicedetailDto"></param>
        /// <returns></returns>
        Task<InvoiceDetailDto?> UpdateInvoiceDetail(Guid id, InvoiceDetailDto invoicedetailDto);
        /// <summary>
        /// Deletes a invoicedetail entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteInvoiceDetail(Guid ID);
    }
}