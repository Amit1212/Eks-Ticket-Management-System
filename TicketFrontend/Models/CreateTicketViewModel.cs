using System.ComponentModel.DataAnnotations;

namespace TicketFrontend.Models;

public class CreateTicketViewModel
{
    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select a category.")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Please select a priority.")]
    public int PriorityId { get; set; }

    [Required(ErrorMessage = "Please select the ticket creator.")]
    public int CreatedById { get; set; }

    public int? AssignedToId { get; set; }

    public List<CategoryViewModel> Categories { get; set; } = new();

    public List<PriorityViewModel> Priorities { get; set; } = new();

    public List<UserViewModel> Users { get; set; } = new();
}
