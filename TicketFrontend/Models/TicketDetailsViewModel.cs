namespace TicketFrontend.Models;

public class TicketDetailsViewModel
{
    public TicketViewModel Ticket { get; set; } = new();

    public List<CommentViewModel> Comments { get; set; } = new();

    public List<StatusViewModel> Statuses { get; set; } = new();

    public List<UserViewModel> Users { get; set; } = new();

    public CreateCommentViewModel NewComment { get; set; } = new();

    public UpdateStatusViewModel UpdateStatus { get; set; } = new();

    public AssignTicketViewModel AssignTicket { get; set; } = new();
}
