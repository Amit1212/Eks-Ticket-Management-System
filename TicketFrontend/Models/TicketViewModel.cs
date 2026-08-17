namespace TicketFrontend.Models;

public class TicketViewModel
{
    public int Id { get; set; }

    public string TicketNumber { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public string Category { get; set; } = string.Empty;

    public int PriorityId { get; set; }

    public string Priority { get; set; } = string.Empty;

    public int StatusId { get; set; }

    public string Status { get; set; } = string.Empty;

    public int CreatedById { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public int? AssignedToId { get; set; }

    public string? AssignedTo { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public DateTime? ClosedDate { get; set; }
}
