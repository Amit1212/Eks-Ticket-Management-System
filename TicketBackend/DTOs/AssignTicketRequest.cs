using System.ComponentModel.DataAnnotations;

namespace TicketBackend.DTOs;

public class AssignTicketRequest
{
    [Required]
    public int AssignedToId { get; set; }
}
