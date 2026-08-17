namespace TicketFrontend.Models;

public class DashboardViewModel
{
    public List<TicketViewModel> Tickets { get; set; } = new();

    public int TotalTickets { get; set; }

    public int OpenTickets { get; set; }

    public int AssignedTickets { get; set; }

    public int InProgressTickets { get; set; }

    public int PendingTickets { get; set; }

    public int ResolvedTickets { get; set; }

    public int ClosedTickets { get; set; }
}
