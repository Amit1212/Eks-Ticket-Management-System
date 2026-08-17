using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketBackend.Data;
using TicketBackend.DTOs;
using TicketBackend.Models;

namespace TicketBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TicketDbContext _context;

    private readonly PasswordHasher<User>
        _passwordHasher;

    public AuthController(TicketDbContext context)
    {
        _context = context;

        _passwordHasher =
            new PasswordHasher<User>();
    }

    // ============================================================
    // POST: api/auth/login
    // ============================================================

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request)
    {
        var username =
            request.Username.Trim();

        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new LoginResponse
            {
                Success = false,
                Message =
                    "Username and password are required."
            });
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u =>
                u.Username == username);

        if (user == null)
        {
            return Unauthorized(
                new LoginResponse
                {
                    Success = false,
                    Message =
                        "Invalid username or password."
                });
        }

        if (!user.IsActive)
        {
            return Unauthorized(
                new LoginResponse
                {
                    Success = false,
                    Message =
                        "User account is inactive."
                });
        }

        var passwordResult =
            _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

        if (passwordResult ==
            PasswordVerificationResult.Failed)
        {
            return Unauthorized(
                new LoginResponse
                {
                    Success = false,
                    Message =
                        "Invalid username or password."
                });
        }

        // Upgrade the password hash if ASP.NET
        // recommends a newer hash format.
        if (passwordResult ==
            PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash =
                _passwordHasher.HashPassword(
                    user,
                    request.Password);

            await _context.SaveChangesAsync();
        }

        return Ok(new LoginResponse
        {
            Success = true,

            Message = "Login successful.",

            UserId = user.Id,

            Username = user.Username,

            FullName = user.FullName,

            Email = user.Email,

            Role = user.Role
        });
    }
}
