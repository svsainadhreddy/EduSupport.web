using EduSupport.Web.Data;
using EduSupport.Web.Models;
using EduSupport.Web.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EduSupport.Web.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<User?> GetDomainUserAsync()
    {
        var identityUser =
            await GetIdentityUserAsync();

        if (identityUser == null)
            return null;

        return await _context.Users
            .Include(u => u.Department)
            .FirstOrDefaultAsync(
                u => u.Id == identityUser.DomainUserId);
    }

    public async Task<int?> GetDomainUserIdAsync()
    {
        var identityUser =
            await GetIdentityUserAsync();

        return identityUser?.DomainUserId;
    }

    public async Task<bool> IsInRoleAsync(string role)
    {
        var identityUser =
            await GetIdentityUserAsync();

        if (identityUser == null)
            return false;

        return await _userManager.IsInRoleAsync(
            identityUser,
            role);
    }

    private async Task<ApplicationUser?> GetIdentityUserAsync()
    {
        var principal =
            _httpContextAccessor.HttpContext?.User;

        if (principal == null ||
            principal.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        return await _userManager.GetUserAsync(principal);
    }
}