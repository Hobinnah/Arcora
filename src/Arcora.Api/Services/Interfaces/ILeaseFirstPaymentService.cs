using Arcora.Api.DTOs;
using Arcora.Api.Entities;

namespace Arcora.Api.Services.Interfaces;

public interface ILeaseFirstPaymentService
{
    Task<LeaseFirstPaymentDto> GetAsync(Lease lease, CancellationToken cancellationToken = default);
    Task<LeaseFirstPaymentDto> CollectAsync(Lease lease, decimal? expectedAmount = null, CancellationToken cancellationToken = default);
    Task<LeaseFirstPaymentDto> RefreshAsync(Lease lease, CancellationToken cancellationToken = default);
}
