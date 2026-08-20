// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface ITaxRateService
    {
        /// <summary>
        /// Retrieves all tax rates with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<TaxRateDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a tax rate by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<TaxRateDto?> GetID(int ID);
        /// <summary>
        /// Creates a new tax rate entry.
        /// </summary>
        /// <param name = "taxRateDto"></param>
        /// <returns></returns>
        Task<TaxRateDto> CreateTaxRate(TaxRateDto taxRateDto);
        /// <summary>
        /// Updates an existing tax rate entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "taxrateDto"></param>
        /// <returns></returns>
        Task<TaxRateDto?> UpdateTaxRate(int id, TaxRateDto taxrateDto);
        /// <summary>
        /// Deletes a taxrate entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteTaxRate(int ID);
    }
}