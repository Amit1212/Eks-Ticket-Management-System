using System.ComponentModel.DataAnnotations;

namespace TicketFrontend.Models;

public class AssignTicketViewModel
{
    [Required]
    public int AssignedToId { get; set; }
}
