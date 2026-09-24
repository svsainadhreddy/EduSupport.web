using EduSupport.Web.Data;
using EduSupport.Web.Models.Enums;
using EduSupport.Web.Services.Interfaces;
using EduSupport.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduSupport.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ISlaService _slaService;

    public DashboardController(
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
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        var slaAtRisk = tickets
            .Where(t => _slaService.IsResolutionAtRisk(t))
            .ToList();

        var slaBreached = tickets
            .Where(t => _slaService.IsResolutionBreached(t))
            .ToList();

        var today = DateTime.UtcNow.Date;

        var viewModel = new DashboardViewModel
        {
            TotalTickets = tickets.Count,

            OpenTickets = tickets.Count(t =>
                t.Status != TicketStatus.Resolved &&
                t.Status != TicketStatus.Closed),

            PendingTickets = tickets.Count(t =>
                t.Status == TicketStatus.Pending),

            ResolvedTickets = tickets.Count(t =>
                t.Status == TicketStatus.Resolved),

            ClosedTickets = tickets.Count(t =>
                t.Status == TicketStatus.Closed),

            SlaAtRisk = slaAtRisk.Count,

            SlaBreached = slaBreached.Count,

            UnassignedTickets = tickets.Count(t =>
                !t.AssignedToId.HasValue &&
                t.Status != TicketStatus.Closed),

            ResolvedToday = tickets.Count(t =>
                t.ResolvedAt.HasValue &&
                t.ResolvedAt.Value.Date == today),

            RecentTickets = tickets
                .Take(8)
                .ToList(),

            SlaAlerts = tickets
                    .Where(t =>
                        _slaService.IsResolutionBreached(t) ||
                        _slaService.IsResolutionAtRisk(t))
                    .OrderBy(t => t.DueAt)
                    .Take(5)
                    .Select(t => new TicketSlaAlertViewModel
                    {
                        Ticket = t,
                        IsBreached = _slaService.IsResolutionBreached(t),
                        IsAtRisk = _slaService.IsResolutionAtRisk(t)
                    })
                    .ToList(),

            PendingActions = tickets
                .Where(t =>
                    t.Status == TicketStatus.Pending ||
                    !t.AssignedToId.HasValue)
                .OrderBy(t => t.CreatedAt)
                .Take(5)
                .ToList()
        };

        return View(viewModel);
    }
}