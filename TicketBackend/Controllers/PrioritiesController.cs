using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketBackend.Data;
using TicketBackend.Models;

namespace TicketBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrioritiesController : ControllerBase
{
    private readonly TicketDbContext _context;

    public PrioritiesController(TicketDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Priority>>> GetPriorities()
    {
        return Ok(await _context.Priorities
            .OrderBy(p => p.Level)
            .ToListAsync());
    }
}
