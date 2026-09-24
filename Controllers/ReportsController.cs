using EduSupport.Web.Data;
using EduSupport.Web.Models.Enums;
using EduSupport.Web.Services.Interfaces;
using EduSupport.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduSupport.Web.Controllers;

[Authorize(Roles = "Manager,Admin")]
public class ReportsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ISlaService _slaService;

    public ReportsController(
        ApplicationDbContext context,
        ISlaService slaService)
    {
        _context = context;
        _slaService = slaService;
    }

    public async Task<IActionResult> Index()
    {
        var tickets = await _context.Tickets
            .Include(t => t.Department)
            .Include(t => t.AssignedTo)
            .ToListAsync();

        foreach (var ticket in tickets)
        {
            await _slaService.EvaluateSlaAsync(ticket);
        }

        await _context.SaveChangesAsync();

        var model = new ReportsViewModel
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

            SlaAtRisk = tickets.Count(t =>
                t.IsSlaAtRisk),

            SlaBreached = tickets.Count(t =>
                t.IsSlaBreached),

            UnassignedTickets = tickets.Count(t =>
                !t.AssignedToId.HasValue &&
                t.Status != TicketStatus.Closed),

            ByPriority = tickets
                .GroupBy(t => t.Priority)
                .Select(g => new PriorityReportItem
                {
                    Priority = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Priority)
                .ToList(),

            ByDepartment = tickets
                .GroupBy(t => t.Department?.Name ?? "Unassigned")
                .Select(g => new DepartmentReportItem
                {
                    DepartmentName = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToList(),

            StaffWorkload = tickets
                .Where(t => t.AssignedTo != null)
                .GroupBy(t => new
                {
                    t.AssignedToId,
                    StaffName = t.AssignedTo!.FullName
                })
                .Select(g => new StaffWorkloadItem
                {
                    StaffName = g.Key.StaffName,

                    AssignedTickets = g.Count(),

                    OpenTickets = g.Count(t =>
                        t.Status != TicketStatus.Resolved &&
                        t.Status != TicketStatus.Closed),

                    PendingTickets = g.Count(t =>
                        t.Status == TicketStatus.Pending),

                    ResolvedTickets = g.Count(t =>
                        t.Status == TicketStatus.Resolved)
                })
                .OrderByDescending(x => x.OpenTickets)
                .ToList()
        };

        return View(model);
    }
}