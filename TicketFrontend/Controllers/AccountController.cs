using Microsoft.AspNetCore.Mvc;
using TicketFrontend.Models;
using TicketFrontend.Services;

namespace TicketFrontend.Controllers;

public class AccountController : Controller
{
    private readonly TicketApiService _ticketApiService;

    public AccountController(
        TicketApiService ticketApiService)
    {
        _ticketApiService = ticketApiService;
    }

    // ============================================================
    // Login - GET
    // ============================================================

    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.Session
            .GetInt32("UserId")
            .HasValue)
        {
            return RedirectToAction(
                "Index",
                "Home");
        }

        return View(
            new LoginViewModel());
    }

    // ============================================================
    // Login - POST
    // ============================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var loginResult =
            await _ticketApiService
                .LoginAsync(model);

        if (loginResult == null ||
            !loginResult.Success)
        {
            ModelState.AddModelError(
                string.Empty,
                "Invalid username or password.");

            return View(model);
        }

        // ========================================================
        // Store authenticated user in session
        // ========================================================

        HttpContext.Session.SetInt32(
            "UserId",
            loginResult.UserId);

        HttpContext.Session.SetString(
            "Username",
            loginResult.Username);

        HttpContext.Session.SetString(
            "FullName",
            loginResult.FullName);

        HttpContext.Session.SetString(
            "Email",
            loginResult.Email);

        HttpContext.Session.SetString(
            "Role",
            loginResult.Role);

        return RedirectToAction(
            "Index",
            "Home");
    }

    // ============================================================
    // Logout
    // ============================================================

    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        return RedirectToAction(
            nameof(Login));
    }
}
