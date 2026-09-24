using EduSupport.Web.Data;
using EduSupport.Web.Models;
using EduSupport.Web.Models.Enums;
using EduSupport.Web.Services.Interfaces;
using EduSupport.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EduSupport.Web.Controllers;

[Authorize]
public class TicketsController : Controller
{
    private readonly ITicketService _ticketService;
    private readonly ISlaService _slaService;
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public TicketsController(
        ITicketService ticketService,
        ISlaService slaService,
        ApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _ticketService = ticketService;
        _slaService = slaService;
        _context = context;
        _currentUserService = currentUserService;
    }

    // ============================================================
    // GET: /Tickets
    // ============================================================

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var currentUser =
            await _currentUserService.GetDomainUserAsync();

        if (currentUser == null)
            return Challenge();

        List<Ticket> tickets;

        if (User.IsInRole("Manager") ||
            User.IsInRole("Admin"))
        {
            tickets = await _ticketService.GetAllTicketsAsync();
        }
        else
        {
            tickets =
                await _ticketService.GetTicketsForUserAsync(
                    currentUser.Id);
        }

        var viewModel = new TicketListViewModel
        {
            Tickets = tickets,

            TotalTickets = tickets.Count,

            OpenTickets = tickets.Count(t =>
                t.Status != TicketStatus.Closed &&
                t.Status != TicketStatus.Resolved),

            PendingTickets = tickets.Count(t =>
                t.Status == TicketStatus.Pending),

            ResolvedTickets = tickets.Count(t =>
                t.Status == TicketStatus.Resolved)
        };

        return View(viewModel);
    }


    // ============================================================
    // GET: /Tickets/Create
    // ============================================================

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (!User.IsInRole("Student"))
        {
            return Forbid();
        }

        await LoadTicketFormDataAsync();

        return View(new CreateTicketViewModel());
    }


    // ============================================================
    // POST: /Tickets/Create
    // ============================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateTicketViewModel model)
    {
        if (!User.IsInRole("Student"))
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            await LoadTicketFormDataAsync();
            return View(model);
        }

        var currentUser =
            await _currentUserService.GetDomainUserAsync();

        if (currentUser == null)
        {
            return Challenge();
        }

        var ticket = new Ticket
        {
            Subject = model.Subject,
            Description = model.Description,
            CategoryId = model.CategoryId,
            DepartmentId = model.DepartmentId,
            Priority = model.Priority
        };

        var createdTicket =
            await _ticketService.CreateTicketAsync(
                ticket,
                currentUser.Id);

        TempData["SuccessMessage"] =
            $"Ticket {createdTicket.TicketNumber} created successfully.";

        return RedirectToAction(
            nameof(Details),
            new { id = createdTicket.Id });
    }


    // ============================================================
    // GET: /Tickets/Details/5
    // ============================================================

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var ticket =
            await _ticketService.GetTicketByIdAsync(id);

        if (ticket == null)
        {
            return NotFound();
        }

        if (!await CanAccessTicketAsync(ticket))
        {
            return Forbid();
        }

        var viewModel = new TicketDetailsViewModel
        {
            Ticket = ticket,

            Comments = ticket.Comments
                .OrderBy(c => c.CreatedAt)
                .ToList(),

            Activities = ticket.Activities
                .OrderByDescending(a => a.CreatedAt)
                .ToList(),

            Age = _slaService.GetAge(ticket),

            IsResponseBreached =
                _slaService.IsResponseBreached(ticket),

            IsResolutionBreached =
                _slaService.IsResolutionBreached(ticket),

            IsResolutionAtRisk =
                _slaService.IsResolutionAtRisk(ticket)
        };

        return View(viewModel);
    }


    // ============================================================
    // GET: /Tickets/Assign/5
    // Manager / Admin only
    // ============================================================

    [Authorize(Roles = "Manager,Admin")]
    [HttpGet]
    public async Task<IActionResult> Assign(int id)
    {
        var ticket =
            await _ticketService.GetTicketByIdAsync(id);

        if (ticket == null)
        {
            return NotFound();
        }

        var staffUsers = await _context.Users
            .Where(u =>
                u.IsActive &&
                (u.Role == UserRole.Staff ||
                 u.Role == UserRole.Manager))
            .OrderBy(u => u.FullName)
            .ToListAsync();

        ViewBag.StaffUsers = new SelectList(
            staffUsers,
            "Id",
            "FullName",
            ticket.AssignedToId);

        return View(new AssignTicketViewModel
        {
            TicketId = ticket.Id,
            StaffId = ticket.AssignedToId ?? 0
        });
    }


    // ============================================================
    // POST: /Tickets/Assign
    // Manager / Admin only
    // ============================================================

    [Authorize(Roles = "Manager,Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Assign(
        AssignTicketViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var staffUsers = await _context.Users
                .Where(u =>
                    u.IsActive &&
                    (u.Role == UserRole.Staff ||
                     u.Role == UserRole.Manager))
                .OrderBy(u => u.FullName)
                .ToListAsync();

            ViewBag.StaffUsers = new SelectList(
                staffUsers,
                "Id",
                "FullName",
                model.StaffId);

            return View(model);
        }

        var currentUser =
            await _currentUserService.GetDomainUserAsync();

        if (currentUser == null)
        {
            return Challenge();
        }

        var staffExists = await _context.Users
            .AnyAsync(u =>
                u.Id == model.StaffId &&
                u.IsActive &&
                (u.Role == UserRole.Staff ||
                 u.Role == UserRole.Manager));

        if (!staffExists)
        {
            ModelState.AddModelError(
                nameof(model.StaffId),
                "Selected staff member is not available.");

            var staffUsers = await _context.Users
                .Where(u =>
                    u.IsActive &&
                    (u.Role == UserRole.Staff ||
                     u.Role == UserRole.Manager))
                .OrderBy(u => u.FullName)
                .ToListAsync();

            ViewBag.StaffUsers = new SelectList(
                staffUsers,
                "Id",
                "FullName",
                model.StaffId);

            return View(model);
        }

        await _ticketService.AssignTicketAsync(
            model.TicketId,
            model.StaffId,
            currentUser.Id);

        TempData["SuccessMessage"] =
            "Ticket assigned successfully.";

        return RedirectToAction(
            nameof(Details),
            new { id = model.TicketId });
    }


    // ============================================================
    // GET: /Tickets/ChangeStatus/5
    // ============================================================

    [Authorize(Roles = "Staff,Manager,Admin")]
    [HttpGet]
    public async Task<IActionResult> ChangeStatus(int id)
    {
        var ticket =
            await _ticketService.GetTicketByIdAsync(id);

        if (ticket == null)
        {
            return NotFound();
        }

        if (!await CanProcessTicketAsync(ticket))
        {
            return Forbid();
        }

        return View(new ChangeStatusViewModel
        {
            TicketId = ticket.Id,
            Status = ticket.Status,
            PendingReason = ticket.PendingReason
        });
    }


    // ============================================================
    // POST: /Tickets/ChangeStatus
    // ============================================================

    [Authorize(Roles = "Staff,Manager,Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(
        ChangeStatusViewModel model)
    {
        if (model.Status == TicketStatus.Pending &&
            !model.PendingReason.HasValue)
        {
            ModelState.AddModelError(
                nameof(model.PendingReason),
                "Please select a pending reason.");
        }

        if (model.Status != TicketStatus.Pending)
        {
            model.PendingReason = null;
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var ticket =
            await _ticketService.GetTicketByIdAsync(
                model.TicketId);

        if (ticket == null)
        {
            return NotFound();
        }

        if (!await CanProcessTicketAsync(ticket))
        {
            return Forbid();
        }

        var currentUser =
            await _currentUserService.GetDomainUserAsync();

        if (currentUser == null)
        {
            return Challenge();
        }

        await _ticketService.ChangeStatusAsync(
            model.TicketId,
            model.Status,
            currentUser.Id,
            model.PendingReason);

        TempData["SuccessMessage"] =
            "Ticket status updated.";

        return RedirectToAction(
            nameof(Details),
            new { id = model.TicketId });
    }


    // ============================================================
    // POST: /Tickets/AddComment
    // ============================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(
        AddCommentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] =
                "Please enter a valid comment.";

            return RedirectToAction(
                nameof(Details),
                new { id = model.TicketId });
        }

        var ticket =
            await _ticketService.GetTicketByIdAsync(
                model.TicketId);

        if (ticket == null)
        {
            return NotFound();
        }

        if (!await CanAccessTicketAsync(ticket))
        {
            return Forbid();
        }

        var currentUser =
            await _currentUserService.GetDomainUserAsync();

        if (currentUser == null)
        {
            return Challenge();
        }

        await _ticketService.AddCommentAsync(
            model.TicketId,
            currentUser.Id,
            model.Message);

        TempData["SuccessMessage"] =
            "Comment added successfully.";

        return RedirectToAction(
            nameof(Details),
            new { id = model.TicketId });
    }


    // ============================================================
    // GET: /Tickets/Resolve/5
    // ============================================================

    [Authorize(Roles = "Staff,Manager,Admin")]
    [HttpGet]
    public async Task<IActionResult> Resolve(int id)
    {
        var ticket =
            await _ticketService.GetTicketByIdAsync(id);

        if (ticket == null)
        {
            return NotFound();
        }

        if (!await CanProcessTicketAsync(ticket))
        {
            return Forbid();
        }

        if (ticket.Status == TicketStatus.Closed)
        {
            return BadRequest(
                "Closed ticket cannot be resolved.");
        }

        return View(new ResolveTicketViewModel
        {
            TicketId = ticket.Id
        });
    }


    // ============================================================
    // POST: /Tickets/Resolve
    // ============================================================

    [Authorize(Roles = "Staff,Manager,Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Resolve(
        ResolveTicketViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var ticket =
            await _ticketService.GetTicketByIdAsync(
                model.TicketId);

        if (ticket == null)
        {
            return NotFound();
        }

        if (!await CanProcessTicketAsync(ticket))
        {
            return Forbid();
        }

        var currentUser =
            await _currentUserService.GetDomainUserAsync();

        if (currentUser == null)
        {
            return Challenge();
        }

        await _ticketService.ResolveTicketAsync(
            model.TicketId,
            currentUser.Id,
            model.ResolutionSummary);

        TempData["SuccessMessage"] =
            "Ticket resolved successfully.";

        return RedirectToAction(
            nameof(Details),
            new { id = model.TicketId });
    }


    // ============================================================
    // POST: /Tickets/Close/5
    // Student only
    // ============================================================

    [Authorize(Roles = "Student")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Close(int id)
    {
        var ticket =
            await _ticketService.GetTicketByIdAsync(id);

        if (ticket == null)
        {
            return NotFound();
        }

        var currentUser =
            await _currentUserService.GetDomainUserAsync();

        if (currentUser == null)
        {
            return Challenge();
        }

        if (ticket.CreatedById != currentUser.Id)
        {
            return Forbid();
        }

        if (ticket.Status != TicketStatus.Resolved)
        {
            TempData["ErrorMessage"] =
                "Only resolved tickets can be closed.";

            return RedirectToAction(
                nameof(Details),
                new { id });
        }

        await _ticketService.CloseTicketAsync(
            id,
            currentUser.Id);

        TempData["SuccessMessage"] =
            "Ticket closed successfully.";

        return RedirectToAction(
            nameof(Details),
            new { id });
    }


    // ============================================================
    // POST: /Tickets/Reopen/5
    // Student only
    // ============================================================

    [Authorize(Roles = "Student")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reopen(int id)
    {
        var ticket =
            await _ticketService.GetTicketByIdAsync(id);

        if (ticket == null)
        {
            return NotFound();
        }

        var currentUser =
            await _currentUserService.GetDomainUserAsync();

        if (currentUser == null)
        {
            return Challenge();
        }

        if (ticket.CreatedById != currentUser.Id)
        {
            return Forbid();
        }

        if (ticket.Status != TicketStatus.Closed)
        {
            TempData["ErrorMessage"] =
                "Only closed tickets can be reopened.";

            return RedirectToAction(
                nameof(Details),
                new { id });
        }

        await _ticketService.ReopenTicketAsync(
            id,
            currentUser.Id);

        TempData["SuccessMessage"] =
            "Ticket reopened successfully.";

        return RedirectToAction(
            nameof(Details),
            new { id });
    }


    // ============================================================
    // Helper: Check whether user can view ticket
    // ============================================================

    private async Task<bool> CanAccessTicketAsync(
        Ticket ticket)
    {
        if (User.IsInRole("Manager") ||
            User.IsInRole("Admin"))
        {
            return true;
        }

        var currentUser =
            await _currentUserService.GetDomainUserAsync();

        if (currentUser == null)
        {
            return false;
        }

        if (User.IsInRole("Student"))
        {
            return ticket.CreatedById == currentUser.Id;
        }

        if (User.IsInRole("Staff"))
        {
            return ticket.CreatedById == currentUser.Id ||
                   ticket.AssignedToId == currentUser.Id;
        }

        return false;
    }


    // ============================================================
    // Helper: Check whether staff can process ticket
    // ============================================================

    private async Task<bool> CanProcessTicketAsync(
        Ticket ticket)
    {
        if (User.IsInRole("Manager") ||
            User.IsInRole("Admin"))
        {
            return true;
        }

        if (User.IsInRole("Staff"))
        {
            var currentUser =
                await _currentUserService.GetDomainUserAsync();

            if (currentUser == null)
            {
                return false;
            }

            return ticket.AssignedToId == currentUser.Id;
        }

        return false;
    }


    // ============================================================
    // Helper: Load Create Ticket dropdowns
    // ============================================================

    private async Task LoadTicketFormDataAsync()
    {
        ViewBag.Categories = new SelectList(
            await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync(),
            "Id",
            "Name");

        ViewBag.Departments = new SelectList(
            await _context.Departments
                .Where(d => d.IsActive)
                .OrderBy(d => d.Name)
                .ToListAsync(),
            "Id",
            "Name");
    }
}