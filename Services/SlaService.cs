using EduSupport.Web.Data;
using EduSupport.Web.Models;
using EduSupport.Web.Models.Enums;
using EduSupport.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduSupport.Web.Services;

public class SlaService : ISlaService
{
    private readonly ApplicationDbContext _context;

    public SlaService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SlaPolicy?> GetPolicyAsync(
        TicketPriority priority)
    {
        return await _context.SlaPolicies
            .FirstOrDefaultAsync(x =>
                x.Priority == priority &&
                x.IsActive);
    }

    public async Task ApplySlaAsync(Ticket ticket)
    {
        var policy = await GetPolicyAsync(ticket.Priority);

        if (policy == null)
            return;

        ticket.FirstResponseDueAt =
            ticket.CreatedAt.AddMinutes(
                policy.FirstResponseMinutes);

        ticket.DueAt =
            ticket.CreatedAt.AddMinutes(
                policy.ResolutionMinutes);
    }
    public async Task EvaluateSlaAsync(Ticket ticket)
    {
        var responseBreached =
            IsResponseBreached(ticket);

        var resolutionBreached =
            IsResolutionBreached(ticket);

        var breached =
            responseBreached || resolutionBreached;

        var atRisk =
            !breached &&
            IsResolutionAtRisk(ticket);

        ticket.IsSlaBreached = breached;
        ticket.IsSlaAtRisk = atRisk;

        if (breached && !ticket.EscalatedAt.HasValue)
        {
            ticket.EscalatedAt = DateTime.UtcNow;
        }

        await Task.CompletedTask;
    }

    public bool IsResponseBreached(Ticket ticket)
    {
        if (ticket.FirstResponseAt.HasValue)
            return false;

        if (!ticket.FirstResponseDueAt.HasValue)
            return false;

        return DateTime.UtcNow >
               ticket.FirstResponseDueAt.Value;
    }

    public bool IsResolutionBreached(Ticket ticket)
    {
        if (ticket.Status == TicketStatus.Resolved ||
            ticket.Status == TicketStatus.Closed)
        {
            return false;
        }

        if (!ticket.DueAt.HasValue)
            return false;

        return DateTime.UtcNow >
               ticket.DueAt.Value;
    }

    public bool IsResolutionAtRisk(Ticket ticket)
    {
        if (!ticket.DueAt.HasValue)
            return false;

        if (ticket.Status == TicketStatus.Resolved ||
            ticket.Status == TicketStatus.Closed)
        {
            return false;
        }

        var remaining =
            ticket.DueAt.Value - DateTime.UtcNow;

        if (remaining <= TimeSpan.Zero)
            return false;

        // Consider ticket at risk when
        // less than 20% of SLA time remains.
        var total =
            ticket.DueAt.Value - ticket.CreatedAt;

        return remaining <=
               TimeSpan.FromTicks(
                   total.Ticks * 20 / 100);
    }

    public TimeSpan GetAge(Ticket ticket)
    {
        return DateTime.UtcNow - ticket.CreatedAt;
    }
}