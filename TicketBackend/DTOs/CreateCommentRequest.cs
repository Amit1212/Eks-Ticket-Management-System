using System.ComponentModel.DataAnnotations;

namespace TicketBackend.DTOs;

public class CreateCommentRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public string Comment { get; set; } = string.Empty;
}
