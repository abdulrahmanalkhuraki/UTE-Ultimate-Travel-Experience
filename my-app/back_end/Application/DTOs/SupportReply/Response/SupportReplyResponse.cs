namespace Application.DTOs.SupportReply.Response
{
    public class SupportReplyResponse
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int AdminId { get; set; }
        public string ReplyContent { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
