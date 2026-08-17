namespace TicketFrontend.Models;

public class CreateTicketApiRequest
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public int PriorityId { get; set; }

    public int CreatedById { get; set; }

    public int? AssignedToId { get; set; }
}
