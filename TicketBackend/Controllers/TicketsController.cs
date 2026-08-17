using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TicketBackend.Data;
using TicketBackend.DTOs;
using TicketBackend.Models;

namespace TicketBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly TicketDbContext _context;
    private readonly IAmazonSQS _sqs;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TicketsController> _logger;

    public TicketsController(
        TicketDbContext context,
        IAmazonSQS sqs,
        IConfiguration configuration,
        ILogger<TicketsController> logger)
    {
        _context = context;
        _sqs = sqs;
        _configuration = configuration;
        _logger = logger;
    }

    // ============================================================
    // GET: api/tickets
    // Get all tickets
    // ============================================================
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TicketResponse>>> GetTickets()
    {
        var tickets = await _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Priority)
            .Include(t => t.Status)
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .OrderByDescending(t => t.CreatedDate)
            .Select(t => new TicketResponse
            {
                Id = t.Id,
                TicketNumber = t.TicketNumber,
                Title = t.Title,
                Description = t.Description,

                CategoryId = t.CategoryId,
                Category = t.Category.Name,

                PriorityId = t.PriorityId,
                Priority = t.Priority.Name,

                StatusId = t.StatusId,
                Status = t.Status.Name,

                CreatedById = t.CreatedById,
                CreatedBy = t.CreatedBy.FullName,

                AssignedToId = t.AssignedToId,
                AssignedTo = t.AssignedTo != null
                    ? t.AssignedTo.FullName
                    : null,

                CreatedDate = t.CreatedDate,
                UpdatedDate = t.UpdatedDate,
                ClosedDate = t.ClosedDate
            })
            .ToListAsync();

        return Ok(tickets);
    }

    // ============================================================
    // GET: api/tickets/{id}
    // Get a specific ticket
    // ============================================================
    [HttpGet("{id}")]
    public async Task<ActionResult<TicketResponse>> GetTicket(int id)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Priority)
            .Include(t => t.Status)
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (ticket == null)
        {
            return NotFound(new
            {
                message = $"Ticket with ID {id} was not found."
            });
        }

        var response = new TicketResponse
        {
            Id = ticket.Id,
            TicketNumber = ticket.TicketNumber,
            Title = ticket.Title,
            Description = ticket.Description,

            CategoryId = ticket.CategoryId,
            Category = ticket.Category.Name,

            PriorityId = ticket.PriorityId,
            Priority = ticket.Priority.Name,

            StatusId = ticket.StatusId,
            Status = ticket.Status.Name,

            CreatedById = ticket.CreatedById,
            CreatedBy = ticket.CreatedBy.FullName,

            AssignedToId = ticket.AssignedToId,
            AssignedTo = ticket.AssignedTo?.FullName,

            CreatedDate = ticket.CreatedDate,
            UpdatedDate = ticket.UpdatedDate,
            ClosedDate = ticket.ClosedDate
        };

        return Ok(response);
    }

    // ============================================================
    // POST: api/tickets
    // Create a new ticket
    // ============================================================
    [HttpPost]
    public async Task<ActionResult<TicketResponse>> CreateTicket(
        CreateTicketRequest request)
    {
        var creator = await _context.Users
            .FindAsync(request.CreatedById);

        if (creator == null || !creator.IsActive)
        {
            return BadRequest(new
            {
                message = "Invalid or inactive CreatedBy user."
            });
        }

        var category = await _context.Categories
            .FindAsync(request.CategoryId);

        if (category == null)
        {
            return BadRequest(new
            {
                message = "Invalid CategoryId."
            });
        }

        var priority = await _context.Priorities
            .FindAsync(request.PriorityId);

        if (priority == null)
        {
            return BadRequest(new
            {
                message = "Invalid PriorityId."
            });
        }

        if (request.AssignedToId.HasValue)
        {
            var assignedUser = await _context.Users
                .FindAsync(request.AssignedToId.Value);

            if (assignedUser == null || !assignedUser.IsActive)
            {
                return BadRequest(new
                {
                    message = "Invalid or inactive AssignedTo user."
                });
            }
        }

        var ticket = new Ticket
        {
            TicketNumber = await GenerateTicketNumber(),
            Title = request.Title,
            Description = request.Description,
            CategoryId = request.CategoryId,
            PriorityId = request.PriorityId,
            StatusId = 1,
            CreatedById = request.CreatedById,
            AssignedToId = request.AssignedToId,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        _context.Tickets.Add(ticket);

        await _context.SaveChangesAsync();

        await _context.Entry(ticket)
            .Reference(t => t.Category)
            .LoadAsync();

        await _context.Entry(ticket)
            .Reference(t => t.Priority)
            .LoadAsync();

        await _context.Entry(ticket)
            .Reference(t => t.Status)
            .LoadAsync();

        await _context.Entry(ticket)
            .Reference(t => t.CreatedBy)
            .LoadAsync();

        if (ticket.AssignedToId.HasValue)
        {
            await _context.Entry(ticket)
                .Reference(t => t.AssignedTo)
                .LoadAsync();
        }

        var response = new TicketResponse
        {
            Id = ticket.Id,
            TicketNumber = ticket.TicketNumber,
            Title = ticket.Title,
            Description = ticket.Description,

            CategoryId = ticket.CategoryId,
            Category = ticket.Category.Name,

            PriorityId = ticket.PriorityId,
            Priority = ticket.Priority.Name,

            StatusId = ticket.StatusId,
            Status = ticket.Status.Name,

            CreatedById = ticket.CreatedById,
            CreatedBy = ticket.CreatedBy.FullName,

            AssignedToId = ticket.AssignedToId,
            AssignedTo = ticket.AssignedTo?.FullName,

            CreatedDate = ticket.CreatedDate,
            UpdatedDate = ticket.UpdatedDate,
            ClosedDate = ticket.ClosedDate
        };

        return CreatedAtAction(
            nameof(GetTicket),
            new { id = ticket.Id },
            response);
    }

    // ============================================================
    // PUT: api/tickets/{id}/status
    // Update ticket status
    // ============================================================
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        UpdateTicketStatusRequest request)
    {
        var ticket = await _context.Tickets
            .FirstOrDefaultAsync(t => t.Id == id);

        if (ticket == null)
        {
            return NotFound(new
            {
                message = $"Ticket with ID {id} was not found."
            });
        }

        var status = await _context.TicketStatuses
            .FindAsync(request.StatusId);

        if (status == null)
        {
            return BadRequest(new
            {
                message = "Invalid StatusId."
            });
        }

        ticket.StatusId = request.StatusId;
        ticket.UpdatedDate = DateTime.UtcNow;

        if (status.Name == "Closed")
        {
            ticket.ClosedDate = DateTime.UtcNow;
        }
        else
        {
            ticket.ClosedDate = null;
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Ticket status updated successfully.",
            ticketId = ticket.Id,
            ticketNumber = ticket.TicketNumber,
            statusId = status.Id,
            status = status.Name
        });
    }

    // ============================================================
    // PUT: api/tickets/{id}/assign
    // Assign or reassign ticket
    // ============================================================
    [HttpPut("{id}/assign")]
    public async Task<IActionResult> AssignTicket(
        int id,
        AssignTicketRequest request)
    {
        var ticket = await _context.Tickets
            .FirstOrDefaultAsync(t => t.Id == id);

        if (ticket == null)
        {
            return NotFound(new
            {
                message = $"Ticket with ID {id} was not found."
            });
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u =>
                u.Id == request.AssignedToId &&
                u.IsActive);

        if (user == null)
        {
            return BadRequest(new
            {
                message = "Invalid or inactive user."
            });
        }

        ticket.AssignedToId = user.Id;

        if (ticket.StatusId == 1)
        {
            ticket.StatusId = 2;
        }

        ticket.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var currentStatus = await _context.TicketStatuses
            .FindAsync(ticket.StatusId);

        return Ok(new
        {
            message = "Ticket assigned successfully.",
            ticketId = ticket.Id,
            ticketNumber = ticket.TicketNumber,
            assignedToId = user.Id,
            assignedTo = user.FullName,
            statusId = ticket.StatusId,
            status = currentStatus?.Name
        });
    }

    // ============================================================
    // POST: api/tickets/{id}/comments
    // Add a comment and queue email notifications
    // ============================================================
    [HttpPost("{id}/comments")]
    public async Task<ActionResult<CommentResponse>> AddComment(
        int id,
        CreateCommentRequest request)
    {
        // --------------------------------------------------------
        // Find ticket + creator + assigned user
        // --------------------------------------------------------
        var ticket = await _context.Tickets
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (ticket == null)
        {
            return NotFound(new
            {
                message = $"Ticket with ID {id} was not found."
            });
        }

        // --------------------------------------------------------
        // Validate commenting user
        // --------------------------------------------------------
        var user = await _context.Users
            .FirstOrDefaultAsync(u =>
                u.Id == request.UserId &&
                u.IsActive);

        if (user == null)
        {
            return BadRequest(new
            {
                message = "Invalid or inactive user."
            });
        }

        // --------------------------------------------------------
        // Validate comment
        // --------------------------------------------------------
        if (string.IsNullOrWhiteSpace(request.Comment))
        {
            return BadRequest(new
            {
                message = "Comment cannot be empty."
            });
        }

        // --------------------------------------------------------
        // Create comment
        // --------------------------------------------------------
        var comment = new TicketComment
        {
            TicketId = id,
            UserId = request.UserId,
            Comment = request.Comment.Trim(),
            CreatedDate = DateTime.UtcNow
        };

        _context.TicketComments.Add(comment);

        ticket.UpdatedDate = DateTime.UtcNow;

        // --------------------------------------------------------
        // Save comment first
        // --------------------------------------------------------
        await _context.SaveChangesAsync();

        // --------------------------------------------------------
        // Determine notification recipients
        // --------------------------------------------------------
        var recipients = new List<User>();

        // Notify ticket creator unless they added the comment
        if (ticket.CreatedBy != null &&
            ticket.CreatedBy.Id != user.Id &&
            ticket.CreatedBy.IsActive &&
            !string.IsNullOrWhiteSpace(ticket.CreatedBy.Email))
        {
            recipients.Add(ticket.CreatedBy);
        }

        // Notify assigned user unless:
        // - they added the comment
        // - already included as creator
        if (ticket.AssignedTo != null &&
            ticket.AssignedTo.Id != user.Id &&
            ticket.AssignedTo.IsActive &&
            !string.IsNullOrWhiteSpace(ticket.AssignedTo.Email) &&
            !recipients.Any(r => r.Id == ticket.AssignedTo.Id))
        {
            recipients.Add(ticket.AssignedTo);
        }

        // --------------------------------------------------------
        // Queue email notifications
        // --------------------------------------------------------
        var queueUrl = _configuration["Notification:QueueUrl"];

        if (string.IsNullOrWhiteSpace(queueUrl))
        {
            _logger.LogWarning(
                "Notification:QueueUrl is not configured. " +
                "Email notification skipped for ticket {TicketNumber}.",
                ticket.TicketNumber);
        }
        else
        {
            foreach (var recipient in recipients)
            {
                try
                {
                    var notification = new
                    {
                        ticketId = ticket.Id,
                        ticketNumber = ticket.TicketNumber,
                        ticketTitle = ticket.Title,

                        commenterId = user.Id,
                        commenterName = user.FullName,

                        recipientId = recipient.Id,
                        recipientName = recipient.FullName,
                        recipientEmail = recipient.Email,

                        commentId = comment.Id,
                        comment = comment.Comment,

                        createdDate = comment.CreatedDate
                    };

                    var messageBody =
                        JsonSerializer.Serialize(notification);

                    await _sqs.SendMessageAsync(
                        new SendMessageRequest
                        {
                            QueueUrl = queueUrl,
                            MessageBody = messageBody
                        });

                    _logger.LogInformation(
                        "Email notification queued for ticket {TicketNumber} " +
                        "to {RecipientEmail}.",
                        ticket.TicketNumber,
                        recipient.Email);
                }
                catch (Exception ex)
                {
                    // The comment is already saved.
                    // Do not return an error to the user if SQS fails.
                    _logger.LogError(
                        ex,
                        "Failed to queue email notification for ticket " +
                        "{TicketNumber} to {RecipientEmail}.",
                        ticket.TicketNumber,
                        recipient.Email);
                }
            }
        }

        // --------------------------------------------------------
        // Return comment response
        // --------------------------------------------------------
        var response = new CommentResponse
        {
            Id = comment.Id,
            TicketId = comment.TicketId,
            UserId = comment.UserId,
            UserName = user.FullName,
            Comment = comment.Comment,
            CreatedDate = comment.CreatedDate
        };

        return Ok(response);
    }

    // ============================================================
    // GET: api/tickets/{id}/comments
    // Get comments for a ticket
    // ============================================================
    [HttpGet("{id}/comments")]
    public async Task<ActionResult<IEnumerable<CommentResponse>>> GetComments(
        int id)
    {
        var ticketExists = await _context.Tickets
            .AnyAsync(t => t.Id == id);

        if (!ticketExists)
        {
            return NotFound(new
            {
                message = $"Ticket with ID {id} was not found."
            });
        }

        var comments = await _context.TicketComments
            .Where(c => c.TicketId == id)
            .Include(c => c.User)
            .OrderBy(c => c.CreatedDate)
            .Select(c => new CommentResponse
            {
                Id = c.Id,
                TicketId = c.TicketId,
                UserId = c.UserId,
                UserName = c.User.FullName,
                Comment = c.Comment,
                CreatedDate = c.CreatedDate
            })
            .ToListAsync();

        return Ok(comments);
    }

    // ============================================================
    // Generate Ticket Number
    // Example: INC000001
    // ============================================================
    private async Task<string> GenerateTicketNumber()
    {
        var lastTicket = await _context.Tickets
            .OrderByDescending(t => t.Id)
            .FirstOrDefaultAsync();

        var nextNumber = (lastTicket?.Id ?? 0) + 1;

        return $"INC{nextNumber:D6}";
    }
}
