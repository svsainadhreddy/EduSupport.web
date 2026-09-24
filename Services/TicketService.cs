using EduSupport.Web.Data;
using EduSupport.Web.Models;
using EduSupport.Web.Models.Enums;
using EduSupport.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduSupport.Web.Services;

public class TicketService : ITicketService
{
    private readonly ApplicationDbContext _context;
    private readonly ISlaService _slaService;

    public TicketService(
        ApplicationDbContext context,
        ISlaService slaService)
    {
        _context = context;
        _slaService = slaService;
    }

    public async Task<Ticket> CreateTicketAsync(
        Ticket ticket,
        int createdById)
    {
        ticket.CreatedById = createdById;
        ticket.CreatedAt = DateTime.UtcNow;
        ticket.UpdatedAt = DateTime.UtcNow;
        ticket.Status = TicketStatus.New;

        ticket.TicketNumber =
            $"TKT-{DateTime.UtcNow:yyyyMMddHHmmssfff}";

        await _slaService.ApplySlaAsync(ticket);

        _context.Tickets.Add(ticket);

        await _context.SaveChangesAsync();

        var activity = new TicketActivity
        {
            TicketId = ticket.Id,
            UserId = createdById,
            Action = "Ticket Created",
            Details = "New support ticket created."
        };

        _context.TicketActivities.Add(activity);

        await _context.SaveChangesAsync();

        return ticket;
    }

    public async Task<Ticket?> GetTicketByIdAsync(int id)
    {
        return await _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Department)
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .Include(t => t.EscalatedTo)
            .Include(t => t.Comments)
                .ThenInclude(c => c.User)
            .Include(t => t.Activities)
                .ThenInclude(a => a.User)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<Ticket>> GetAllTicketsAsync()
    {
        return await _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Department)
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Ticket>> GetTicketsForUserAsync(
        int userId)
    {
        return await _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.AssignedTo)
            .Where(t =>
                t.CreatedById == userId ||
                t.AssignedToId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task AssignTicketAsync(
    int ticketId,
    int staffId,
    int performedByUserId)
    {
        var ticket =
            await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null)
            throw new InvalidOperationException(
                "Ticket not found.");

        ticket.AssignedToId = staffId;

        if (ticket.Status == TicketStatus.New)
            ticket.Status = TicketStatus.Assigned;

        ticket.UpdatedAt = DateTime.UtcNow;

        var activity = new TicketActivity
        {
            TicketId = ticket.Id,
            UserId = performedByUserId,
            Action = "Ticket Assigned",
            Details = $"Ticket assigned to user ID {staffId}."
        };

        _context.TicketActivities.Add(activity);

        await _context.SaveChangesAsync();
    }

    public async Task ChangeStatusAsync(
        int ticketId,
        TicketStatus newStatus,
        int performedByUserId,
        PendingReason? pendingReason = null)
    {
        var ticket =
            await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null)
            throw new InvalidOperationException(
                "Ticket not found.");

        ticket.Status = newStatus;
        ticket.PendingReason = pendingReason;
        ticket.UpdatedAt = DateTime.UtcNow;

        if (newStatus == TicketStatus.InProgress &&
            !ticket.FirstResponseAt.HasValue)
        {
            ticket.FirstResponseAt = DateTime.UtcNow;
        }

        var activity = new TicketActivity
        {
            TicketId = ticket.Id,
            UserId = performedByUserId,
            Action = "Status Changed",
            Details = $"Status changed to {newStatus}."
        };

        _context.TicketActivities.Add(activity);

        await _context.SaveChangesAsync();
    }

    public async Task AddCommentAsync(
        int ticketId,
        int userId,
        string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException(
                "Comment cannot be empty.");

        var ticket =
            await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null)
            throw new InvalidOperationException(
                "Ticket not found.");

        var comment = new TicketComment
        {
            TicketId = ticketId,
            UserId = userId,
            Message = message.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _context.TicketComments.Add(comment);

        if (!ticket.FirstResponseAt.HasValue)
        {
            ticket.FirstResponseAt = DateTime.UtcNow;
        }

        var activity = new TicketActivity
        {
            TicketId = ticketId,
            UserId = userId,
            Action = "Comment Added",
            Details = "A new comment was added to the ticket."
        };

        _context.TicketActivities.Add(activity);

        await _context.SaveChangesAsync();
    }
    public async Task ResolveTicketAsync(
    int ticketId,
    int userId,
    string resolutionSummary)
    {
        var ticket =
            await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null)
            throw new InvalidOperationException(
                "Ticket not found.");

        if (string.IsNullOrWhiteSpace(resolutionSummary))
            throw new ArgumentException(
                "Resolution summary is required.");

        ticket.Status = TicketStatus.Resolved;
        ticket.ResolutionSummary =
            resolutionSummary.Trim();

        ticket.ResolvedAt = DateTime.UtcNow;
        ticket.UpdatedAt = DateTime.UtcNow;

        var activity = new TicketActivity
        {
            TicketId = ticket.Id,
            UserId = userId,
            Action = "Ticket Resolved",
            Details = "Ticket marked as resolved."
        };

        _context.TicketActivities.Add(activity);

        await _context.SaveChangesAsync();
    }

    public async Task CloseTicketAsync(
        int ticketId,
        int userId)
    {
        var ticket =
            await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null)
            throw new InvalidOperationException(
                "Ticket not found.");

        ticket.Status = TicketStatus.Closed;
        ticket.ClosedAt = DateTime.UtcNow;
        ticket.UpdatedAt = DateTime.UtcNow;

        var activity = new TicketActivity
        {
            TicketId = ticket.Id,
            UserId = userId,
            Action = "Ticket Closed",
            Details = "Ticket closed after resolution."
        };

        _context.TicketActivities.Add(activity);

        await _context.SaveChangesAsync();
    }

    public async Task ReopenTicketAsync(
        int ticketId,
        int userId)
    {
        var ticket =
            await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null)
            throw new InvalidOperationException(
                "Ticket not found.");

        ticket.Status = TicketStatus.Reopened;
        ticket.UpdatedAt = DateTime.UtcNow;

        ticket.ResolvedAt = null;
        ticket.ClosedAt = null;

        var activity = new TicketActivity
        {
            TicketId = ticket.Id,
            UserId = userId,
            Action = "Ticket Reopened",
            Details = "Ticket reopened by user."
        };

        _context.TicketActivities.Add(activity);

        await _context.SaveChangesAsync();
    }
}