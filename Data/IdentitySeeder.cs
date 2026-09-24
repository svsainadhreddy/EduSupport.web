using EduSupport.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EduSupport.Web.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        ApplicationDbContext context)
    {
        string[] roles =
        {
            "Student",
            "Staff",
            "Manager",
            "Admin"
        };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(
                    new IdentityRole<int>(roleName));
            }
        }

        var domainUsers = await context.Users
            .Where(u => u.IsActive)
            .ToListAsync();

        foreach (var domainUser in domainUsers)
        {
            var existingUser =
                await userManager.FindByEmailAsync(
                    domainUser.Email);

            if (existingUser != null)
                continue;

            var identityUser = new ApplicationUser
            {
                UserName = domainUser.Email,
                Email = domainUser.Email,
                EmailConfirmed = true,
                DomainUserId = domainUser.Id
            };

            var result = await userManager.CreateAsync(
                identityUser,
                GetDefaultPassword(domainUser.Role));

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    result.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"Could not create Identity account for " +
                    $"{domainUser.Email}: {errors}");
            }

            await userManager.AddToRoleAsync(
                identityUser,
                domainUser.Role.ToString());
        }
    }

    private static string GetDefaultPassword(
        Models.Enums.UserRole role)
    {
        return role switch
        {
            Models.Enums.UserRole.Student
                => "Student@123",

            Models.Enums.UserRole.Staff
                => "Staff@123",

            Models.Enums.UserRole.Manager
                => "Manager@123",

            Models.Enums.UserRole.Admin
                => "Admin@123",

            _ => "Welcome@123"
        };
    }
}