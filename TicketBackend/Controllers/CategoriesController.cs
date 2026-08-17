using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketBackend.Data;
using TicketBackend.Models;

namespace TicketBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly TicketDbContext _context;

    public CategoriesController(TicketDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
    {
        return Ok(await _context.Categories
            .OrderBy(c => c.Name)
            .ToListAsync());
    }
}
