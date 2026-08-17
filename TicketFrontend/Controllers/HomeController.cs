using Microsoft.AspNetCore.Mvc;
using TicketFrontend.Models;
using TicketFrontend.Services;

namespace TicketFrontend.Controllers;

public class HomeController : Controller
{
    private readonly TicketApiService _ticketApiService;

    public HomeController(
        TicketApiService ticketApiService)
    {
        _ticketApiService = ticketApiService;
    }

    // ============================================================
    // Dashboard
    // ============================================================
    public async Task<IActionResult> Index()
    {
        // Require login
        if (!IsLoggedIn())
        {
            return RedirectToAction(
                "Login",
                "Account");
        }

        var tickets =
            await _ticketApiService.GetTicketsAsync();

        var dashboard = new DashboardViewModel
        {
            Tickets = tickets,

            TotalTickets = tickets.Count,

            OpenTickets = tickets.Count(t =>
                t.Status.Equals(
                    "Open",
                    StringComparison.OrdinalIgnoreCase)),

            AssignedTickets = tickets.Count(t =>
                t.Status.Equals(
                    "Assigned",
                    StringComparison.OrdinalIgnoreCase)),

            InProgressTickets = tickets.Count(t =>
                t.Status.Equals(
                    "In Progress",
                    StringComparison.OrdinalIgnoreCase)),

            PendingTickets = tickets.Count(t =>
                t.Status.Equals(
                    "Pending",
                    StringComparison.OrdinalIgnoreCase)),

            ResolvedTickets = tickets.Count(t =>
                t.Status.Equals(
                    "Resolved",
                    StringComparison.OrdinalIgnoreCase)),

            ClosedTickets = tickets.Count(t =>
                t.Status.Equals(
                    "Closed",
                    StringComparison.OrdinalIgnoreCase))
        };

        return View(dashboard);
    }

    private bool IsLoggedIn()
    {
        return HttpContext.Session
            .GetInt32("UserId")
            .HasValue;
    }
}
