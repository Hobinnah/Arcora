// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface ICreditReportingEnrollmentService
    {
        /// <summary>
        /// Retrieves all credit reporting enrollments with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<CreditReportingEnrollmentDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a credit reporting enrollment by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<CreditReportingEnrollmentDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new credit reporting enrollment entry.
        /// </summary>
        /// <param name = "creditReportingEnrollmentDto"></param>
        /// <returns></returns>
        Task<CreditReportingEnrollmentDto> CreateCreditReportingEnrollment(CreditReportingEnrollmentDto creditReportingEnrollmentDto);
        /// <summary>
        /// Updates an existing credit reporting enrollment entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "creditreportingenrollmentDto"></param>
        /// <returns></returns>
        Task<CreditReportingEnrollmentDto?> UpdateCreditReportingEnrollment(Guid id, CreditReportingEnrollmentDto creditreportingenrollmentDto);
        /// <summary>
        /// Deletes a creditreportingenrollment entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteCreditReportingEnrollment(Guid ID);
        /// <summary>
        /// Updates the status of a creditreportingenrollment by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the creditreportingenrollment to update.</param>
        /// <param name = "status">The new status value to assign to the creditreportingenrollment.</param>
        /// <returns>
        /// Returns <see cref = "CreditReportingEnrollmentDto"/> with the updated creditreportingenrollment if successful, or null if not found.
        /// </returns>
        Task<CreditReportingEnrollmentDto?> UpdateCreditReportingEnrollmentStatus(Guid id, string status);
    }
}