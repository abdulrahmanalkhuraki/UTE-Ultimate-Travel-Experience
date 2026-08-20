using Application.DTOs.SupportReply.Request;
using Application.DTOs.SupportReply.Response;

namespace Application.Interfaces.SupportReply
{
    public interface ISupportReplyService
    {
        Task<SupportReplyResponse> CreateAsync(SupportReplyCreateRequest request, CancellationToken cancellationToken);

        Task<IReadOnlyList<SupportReplyResponse>> GetAsync(int ticketId, CancellationToken cancellationToken);
    }
}
