using Application.DTOs.Ticket.Request;
using Application.DTOs.Ticket.Response;

namespace Application.Interfaces.Ticket
{
    public interface ITicketService
    {
        Task<TicketResponse> CreateAsync(TicketCreateRequest request, CancellationToken cancellationToken);

        Task<IReadOnlyList<TicketResponse>> GetAsync(int? userId, CancellationToken cancellationToken);
    }
}
