// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface ICreditReportingEnrollmentRepository : IRepository<CreditReportingEnrollment>
    {
        Task<List<CreditReportingEnrollment>> GetCreditReportingEnrollmentAsync();
        Task<bool> HasCreditReportingEnrollmentsAsync();
    }
}