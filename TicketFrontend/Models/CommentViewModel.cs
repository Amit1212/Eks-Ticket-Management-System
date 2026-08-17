namespace TicketFrontend.Models;

public class CommentViewModel
{
    public int Id { get; set; }

    public int TicketId { get; set; }

    public int UserId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }
}
