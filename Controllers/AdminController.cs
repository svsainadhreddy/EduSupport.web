using EduSupport.Web.Data;
using EduSupport.Web.Models;
using EduSupport.Web.Services.Interfaces;
using EduSupport.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduSupport.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AdminController(
        ApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }


    // =========================================================
    // ADMIN DASHBOARD / SETTINGS
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = new AdminDashboardViewModel
        {
            Categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync(),

            Departments = await _context.Departments
                .OrderBy(d => d.Name)
                .ToListAsync(),

            SlaPolicies = await _context.SlaPolicies
                .OrderByDescending(s => s.Priority)
                .ToListAsync(),

            Users = await _context.Users
                .Include(u => u.Department)
                .OrderBy(u => u.FullName)
                .ToListAsync()
        };

        return View(model);
    }


    // =========================================================
    // CATEGORY MANAGEMENT
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCategory(
        string name,
        string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["Error"] =
                "Category name is required.";

            return RedirectToAction(nameof(Index));
        }

        var cleanName = name.Trim();

        var exists = await _context.Categories
            .AnyAsync(c => c.Name == cleanName);

        if (exists)
        {
            TempData["Error"] =
                "Category already exists.";

            return RedirectToAction(nameof(Index));
        }

        _context.Categories.Add(new Category
        {
            Name = cleanName,
            Description = description?.Trim(),
            IsActive = true
        });

        await _context.SaveChangesAsync();

        TempData["Success"] =
            "Category added successfully.";

        return RedirectToAction(nameof(Index));
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleCategory(int id)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        category.IsActive = !category.IsActive;

        await _context.SaveChangesAsync();

        TempData["Success"] =
            category.IsActive
                ? $"{category.Name} has been activated."
                : $"{category.Name} has been deactivated.";

        return RedirectToAction(nameof(Index));
    }


    // =========================================================
    // DEPARTMENT MANAGEMENT
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddDepartment(
        string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["Error"] =
                "Department name is required.";

            return RedirectToAction(nameof(Index));
        }

        var cleanName = name.Trim();

        var exists = await _context.Departments
            .AnyAsync(d => d.Name == cleanName);

        if (exists)
        {
            TempData["Error"] =
                "Department already exists.";

            return RedirectToAction(nameof(Index));
        }

        _context.Departments.Add(new Department
        {
            Name = cleanName,
            IsActive = true
        });

        await _context.SaveChangesAsync();

        TempData["Success"] =
            "Department added successfully.";

        return RedirectToAction(nameof(Index));
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleDepartment(
        int id)
    {
        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == id);

        if (department == null)
        {
            return NotFound();
        }

        department.IsActive = !department.IsActive;

        await _context.SaveChangesAsync();

        TempData["Success"] =
            department.IsActive
                ? $"{department.Name} has been activated."
                : $"{department.Name} has been deactivated.";

        return RedirectToAction(nameof(Index));
    }


    // =========================================================
    // SLA MANAGEMENT
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSlaPolicy(
        int id,
        int firstResponseMinutes,
        int resolutionMinutes)
    {
        var policy = await _context.SlaPolicies
            .FirstOrDefaultAsync(s => s.Id == id);

        if (policy == null)
        {
            return NotFound();
        }

        if (firstResponseMinutes <= 0 ||
            resolutionMinutes <= 0)
        {
            TempData["Error"] =
                "SLA times must be greater than zero.";

            return RedirectToAction(nameof(Index));
        }

        policy.FirstResponseMinutes =
            firstResponseMinutes;

        policy.ResolutionMinutes =
            resolutionMinutes;

        policy.UpdatedAt =
            DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["Success"] =
            $"{policy.Priority} SLA policy updated successfully.";

        return RedirectToAction(nameof(Index));
    }


    // =========================================================
    // USER MANAGEMENT
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleUser(int id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return NotFound();
        }


        // -----------------------------------------------------
        // Prevent administrator from deactivating themselves
        // -----------------------------------------------------

        var currentUserId =
            await _currentUserService.GetDomainUserIdAsync();

        if (currentUserId.HasValue &&
            currentUserId.Value == user.Id)
        {
            TempData["Error"] =
                "You cannot deactivate your own account.";

            return RedirectToAction(nameof(Index));
        }


        // -----------------------------------------------------
        // Toggle account status
        // -----------------------------------------------------

        user.IsActive = !user.IsActive;

        await _context.SaveChangesAsync();


        TempData["Success"] =
            user.IsActive
                ? $"{user.FullName} has been activated."
                : $"{user.FullName} has been deactivated.";

        return RedirectToAction(nameof(Index));
    }
}