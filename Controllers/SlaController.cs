using EduSupport.Web.Data;
using EduSupport.Web.Models.Enums;
using EduSupport.Web.Services.Interfaces;
using EduSupport.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduSupport.Web.Controllers;

[Authorize(Roles = "Manager,Admin")]
public class SlaController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ISlaService _slaService;

    public SlaController(
        ApplicationDbContext context,
        ISlaService slaService)
    {
        _context = context;
        _slaService = slaService;
    }

    public async Task<IActionResult> Index()
    {
        var tickets = await _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Department)
            .Include(t => t.AssignedTo)
            .Where(t =>
                t.Status != TicketStatus.Closed &&
                t.Status != TicketStatus.Resolved)
            .OrderBy(t => t.DueAt)
            .ToListAsync();

        // Evaluate the current SLA state for every open ticket.
        foreach (var ticket in tickets)
        {
            await _slaService.EvaluateSlaAsync(ticket);
        }

        // Save updated SLA flags and escalation timestamps.
        await _context.SaveChangesAsync();

        var items = tickets.Select(ticket =>
        {
            var breached =
                _slaService.IsResponseBreached(ticket) ||
                _slaService.IsResolutionBreached(ticket);

            var atRisk =
                !breached &&
                _slaService.IsResolutionAtRisk(ticket);

            var age = _slaService.GetAge(ticket);

            return new SlaTicketViewModel
            {
                TicketId = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                Subject = ticket.Subject,
                Priority = ticket.Priority,
                Status = ticket.Status,
                AssignedTo = ticket.AssignedTo?.FullName,
                DueAt = ticket.DueAt,
                Age = age,
                IsBreached = breached,
                IsAtRisk = atRisk
            };
        }).ToList();

        var model = new SlaMonitoringViewModel
        {
            TotalOpen = items.Count,

            Normal = items.Count(x =>
                !x.IsBreached && !x.IsAtRisk),

            AtRisk = items.Count(x =>
                x.IsAtRisk),

            Breached = items.Count(x =>
                x.IsBreached),

            Tickets = items
        };

        return View(model);
    }
}