namespace Application.DTOs.SupportReply.Request
{
    public sealed record SupportReplyCreateRequest
    (
        int TicketId,
        string ReplyContent
    );
}
