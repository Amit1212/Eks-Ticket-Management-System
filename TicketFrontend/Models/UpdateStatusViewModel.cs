using System.ComponentModel.DataAnnotations;

namespace TicketFrontend.Models;

public class UpdateStatusViewModel
{
    [Required]
    public int StatusId { get; set; }
}
