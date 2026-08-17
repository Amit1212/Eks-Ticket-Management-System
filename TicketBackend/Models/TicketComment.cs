namespace TicketBackend.Models;

public class TicketComment
{
    public int Id { get; set; }

    public int TicketId { get; set; }

    public int UserId { get; set; }

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public Ticket Ticket { get; set; } = null!;

    public User User { get; set; } = null!;
}
