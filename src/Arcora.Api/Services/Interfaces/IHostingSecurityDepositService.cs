using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IHostingSecurityDepositService
    {
        Task<HostSecurityDepositListResponseDto> GetHostSecurityDeposits(long actorUserID, int pageSize, int pageNumber, string? status);
        Task<PagedResult<SecurityDepositTransactionDto>> GetTransactionsBySecurityDeposit(Guid securityDepositID, long actorUserID, int pageSize, int pageNumber);
        Task<SecurityDepositEvidenceUploadResponseDto> UploadEvidence(Guid securityDepositID, long actorUserID, IFormFile file, CancellationToken cancellationToken);
        Task<SecurityDepositSettlementNoticeResponseDto> SendSettlementNotice(Guid securityDepositID, long actorUserID, string? idempotencyKey, SecurityDepositSettlementNoticeRequestDto request);
        Task<SecurityDepositReturnResponseDto> ReturnDeposit(Guid securityDepositID, long actorUserID, SecurityDepositReturnRequestDto request);
    }
}
