using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketBackend.Data;
using TicketBackend.Models;

namespace TicketBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatusesController : ControllerBase
{
    private readonly TicketDbContext _context;

    public StatusesController(TicketDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TicketStatus>>> GetStatuses()
    {
        return Ok(await _context.TicketStatuses
            .OrderBy(s => s.Id)
            .ToListAsync());
    }
}
