namespace TicketBackend.Models;

public class Ticket
{
    public int Id { get; set; }

    public string TicketNumber { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public int PriorityId { get; set; }

    public int StatusId { get; set; }

    public int CreatedById { get; set; }

    public int? AssignedToId { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? ClosedDate { get; set; }

    public Category Category { get; set; } = null!;

    public Priority Priority { get; set; } = null!;

    public TicketStatus Status { get; set; } = null!;

    public User CreatedBy { get; set; } = null!;

    public User? AssignedTo { get; set; }

    public ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
}
