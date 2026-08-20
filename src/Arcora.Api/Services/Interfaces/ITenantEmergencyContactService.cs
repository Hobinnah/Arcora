// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface ITenantEmergencyContactService
    {
        /// <summary>
        /// Retrieves all tenant emergency contacts with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<TenantEmergencyContactDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a tenant emergency contact by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<TenantEmergencyContactDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new tenant emergency contact entry.
        /// </summary>
        /// <param name = "tenantEmergencyContactDto"></param>
        /// <returns></returns>
        Task<TenantEmergencyContactDto> CreateTenantEmergencyContact(TenantEmergencyContactDto tenantEmergencyContactDto);
        /// <summary>
        /// Updates an existing tenant emergency contact entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "tenantemergencycontactDto"></param>
        /// <returns></returns>
        Task<TenantEmergencyContactDto?> UpdateTenantEmergencyContact(Guid id, TenantEmergencyContactDto tenantemergencycontactDto);
        /// <summary>
        /// Deletes a tenantemergencycontact entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteTenantEmergencyContact(Guid ID);
    }
}