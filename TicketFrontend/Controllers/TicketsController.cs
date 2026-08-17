using Microsoft.AspNetCore.Mvc;
using TicketFrontend.Models;
using TicketFrontend.Services;

namespace TicketFrontend.Controllers;

public class TicketsController : Controller
{
    private readonly TicketApiService _ticketApiService;

    public TicketsController(
        TicketApiService ticketApiService)
    {
        _ticketApiService = ticketApiService;
    }

    // ============================================================
    // Ticket List
    // ============================================================
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction(
                "Login",
                "Account");
        }

        var tickets =
            await _ticketApiService.GetTicketsAsync();

        return View(tickets);
    }

    // ============================================================
    // Create Ticket - GET
    // ============================================================
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction(
                "Login",
                "Account");
        }

        var model = new CreateTicketViewModel();

        await LoadDropdownData(model);

        // Automatically use logged-in user
        model.CreatedById =
            HttpContext.Session
                .GetInt32("UserId") ?? 0;

        return View(model);
    }

    // ============================================================
    // Create Ticket - POST
    // ============================================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateTicketViewModel model)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction(
                "Login",
                "Account");
        }

        // Always use logged-in user
        model.CreatedById =
            HttpContext.Session
                .GetInt32("UserId") ?? 0;

        if (!ModelState.IsValid)
        {
            await LoadDropdownData(model);

            return View(model);
        }

        var request = new CreateTicketApiRequest
        {
            Title = model.Title,

            Description = model.Description,

            CategoryId = model.CategoryId,

            PriorityId = model.PriorityId,

            CreatedById = model.CreatedById,

            AssignedToId = model.AssignedToId
        };

        var ticket =
            await _ticketApiService
                .CreateTicketAsync(request);

        if (ticket == null)
        {
            ModelState.AddModelError(
                string.Empty,
                "Unable to create the ticket.");

            await LoadDropdownData(model);

            return View(model);
        }

        return RedirectToAction(
            nameof(Details),
            new { id = ticket.Id });
    }

    // ============================================================
    // Ticket Details
    // ============================================================
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction(
                "Login",
                "Account");
        }

        var ticket =
            await _ticketApiService
                .GetTicketAsync(id);

        if (ticket == null)
        {
            return NotFound();
        }

        var model =
            await BuildTicketDetailsModel(ticket);

        return View(model);
    }

    // ============================================================
    // Add Comment
    // ============================================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(
        int ticketId,
        CreateCommentViewModel model)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction(
                "Login",
                "Account");
        }

        // Automatically use logged-in user
        model.UserId =
            HttpContext.Session
                .GetInt32("UserId") ?? 0;

        if (string.IsNullOrWhiteSpace(model.Comment))
        {
            TempData["ErrorMessage"] =
                "Comment cannot be empty.";

            return RedirectToAction(
                nameof(Details),
                new { id = ticketId });
        }

        var result =
            await _ticketApiService
                .AddCommentAsync(
                    ticketId,
                    model);

        if (result == null)
        {
            TempData["ErrorMessage"] =
                "Unable to add the comment.";

            return RedirectToAction(
                nameof(Details),
                new { id = ticketId });
        }

        TempData["SuccessMessage"] =
            "Comment added successfully.";

        return RedirectToAction(
            nameof(Details),
            new { id = ticketId });
    }

    // ============================================================
    // Update Status
    // ============================================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(
        int ticketId,
        UpdateStatusViewModel model)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction(
                "Login",
                "Account");
        }

        var success =
            await _ticketApiService
                .UpdateTicketStatusAsync(
                    ticketId,
                    model.StatusId);

        if (!success)
        {
            TempData["ErrorMessage"] =
                "Unable to update ticket status.";

            return RedirectToAction(
                nameof(Details),
                new { id = ticketId });
        }

        TempData["SuccessMessage"] =
            "Ticket status updated successfully.";

        return RedirectToAction(
            nameof(Details),
            new { id = ticketId });
    }

    // ============================================================
    // Assign Ticket
    // ============================================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Assign(
        int ticketId,
        AssignTicketViewModel model)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction(
                "Login",
                "Account");
        }

        var success =
            await _ticketApiService
                .AssignTicketAsync(
                    ticketId,
                    model.AssignedToId);

        if (!success)
        {
            TempData["ErrorMessage"] =
                "Unable to assign the ticket.";

            return RedirectToAction(
                nameof(Details),
                new { id = ticketId });
        }

        TempData["SuccessMessage"] =
            "Ticket assigned successfully.";

        return RedirectToAction(
            nameof(Details),
            new { id = ticketId });
    }

    // ============================================================
    // Load Create Ticket dropdowns
    // ============================================================
    private async Task LoadDropdownData(
        CreateTicketViewModel model)
    {
        model.Categories =
            await _ticketApiService
                .GetCategoriesAsync();

        model.Priorities =
            await _ticketApiService
                .GetPrioritiesAsync();

        model.Users =
            await _ticketApiService
                .GetUsersAsync();
    }

    // ============================================================
    // Build Details Model
    // ============================================================
    private async Task<TicketDetailsViewModel>
        BuildTicketDetailsModel(
            TicketViewModel ticket)
    {
        var comments =
            await _ticketApiService
                .GetCommentsAsync(ticket.Id);

        var statuses =
            await _ticketApiService
                .GetStatusesAsync();

        var users =
            await _ticketApiService
                .GetUsersAsync();

        return new TicketDetailsViewModel
        {
            Ticket = ticket,

            Comments = comments,

            Statuses = statuses,

            Users = users,

            NewComment =
                new CreateCommentViewModel
                {
                    UserId =
                        HttpContext.Session
                            .GetInt32("UserId") ?? 0
                },

            UpdateStatus =
                new UpdateStatusViewModel
                {
                    StatusId = ticket.StatusId
                },

            AssignTicket =
                new AssignTicketViewModel
                {
                    AssignedToId =
                        ticket.AssignedToId ?? 0
                }
        };
    }

    // ============================================================
    // Check Login
    // ============================================================
    private bool IsLoggedIn()
    {
        return HttpContext.Session
            .GetInt32("UserId")
            .HasValue;
    }
}
