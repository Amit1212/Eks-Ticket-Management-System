using System.ComponentModel.DataAnnotations;

namespace TicketBackend.DTOs;

public class UpdateTicketStatusRequest
{
    [Required]
    public int StatusId { get; set; }
}
