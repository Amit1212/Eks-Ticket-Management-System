using System.ComponentModel.DataAnnotations;

namespace TicketBackend.DTOs;

public class CreateTicketRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public int PriorityId { get; set; }

    [Required]
    public int CreatedById { get; set; }

    public int? AssignedToId { get; set; }
}
