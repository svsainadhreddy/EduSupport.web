using EduSupport.Web.Models;

namespace EduSupport.Web.Services.Interfaces;

public interface ICurrentUserService
{
    Task<User?> GetDomainUserAsync();

    Task<int?> GetDomainUserIdAsync();

    Task<bool> IsInRoleAsync(string role);
}