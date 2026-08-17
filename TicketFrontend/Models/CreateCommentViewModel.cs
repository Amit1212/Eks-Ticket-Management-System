using System.ComponentModel.DataAnnotations;

namespace TicketFrontend.Models;

public class CreateCommentViewModel
{
    [Required(ErrorMessage = "Please select a user.")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Comment is required.")]
    public string Comment { get; set; } = string.Empty;
}
