using EduSupport.Web.Models;
using EduSupport.Web.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace EduSupport.Web.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context)
    {
        await context.Database.MigrateAsync();


        await SeedDepartmentsAsync(context);

        await SeedCategoriesAsync(context);

        await SeedSlaPoliciesAsync(context);

        await SeedUsersAsync(context);
    }


    // =====================================================
    // Departments
    // =====================================================

    private static async Task SeedDepartmentsAsync(
        ApplicationDbContext context)
    {
        if (await context.Departments.AnyAsync())
            return;


        var departments = new List<Department>
        {
            new()
            {
                Name = "Computer Science",
                IsActive = true
            },

            new()
            {
                Name = "Accounts",
                IsActive = true
            },

            new()
            {
                Name = "Examination",
                IsActive = true
            },

            new()
            {
                Name = "Administration",
                IsActive = true
            },

            new()
            {
                Name = "IT Support",
                IsActive = true
            }
        };


        await context.Departments.AddRangeAsync(departments);

        await context.SaveChangesAsync();
    }


    // =====================================================
    // Categories
    // =====================================================

    private static async Task SeedCategoriesAsync(
        ApplicationDbContext context)
    {
        if (await context.Categories.AnyAsync())
            return;


        var categories = new List<Category>
        {
            new()
            {
                Name = "Fees",
                Description = "Fee payments, receipts and outstanding amounts."
            },

            new()
            {
                Name = "Attendance",
                Description = "Attendance corrections and attendance related requests."
            },

            new()
            {
                Name = "ID Card",
                Description = "Student ID card related requests."
            },

            new()
            {
                Name = "Certificates",
                Description = "Bonafide, course completion and other certificates."
            },

            new()
            {
                Name = "Documents",
                Description = "Academic and administrative document requests."
            },

            new()
            {
                Name = "Academic",
                Description = "Academic and course related requests."
            },

            new()
            {
                Name = "Technical Support",
                Description = "Portal, login and technical issues."
            },

            new()
            {
                Name = "Other",
                Description = "Other student support requests."
            }
        };


        await context.Categories.AddRangeAsync(categories);

        await context.SaveChangesAsync();
    }


    // =====================================================
    // SLA Policies
    // =====================================================

    private static async Task SeedSlaPoliciesAsync(
        ApplicationDbContext context)
    {
        if (await context.SlaPolicies.AnyAsync())
            return;


        var policies = new List<SlaPolicy>
        {
            new()
            {
                Priority = TicketPriority.Critical,
                FirstResponseMinutes = 30,
                ResolutionMinutes = 240,
                IsActive = true
            },

            new()
            {
                Priority = TicketPriority.High,
                FirstResponseMinutes = 60,
                ResolutionMinutes = 480,
                IsActive = true
            },

            new()
            {
                Priority = TicketPriority.Medium,
                FirstResponseMinutes = 240,
                ResolutionMinutes = 1440,
                IsActive = true
            },

            new()
            {
                Priority = TicketPriority.Low,
                FirstResponseMinutes = 480,
                ResolutionMinutes = 4320,
                IsActive = true
            }
        };


        await context.SlaPolicies.AddRangeAsync(policies);

        await context.SaveChangesAsync();
    }


    // =====================================================
    // Users
    // =====================================================

    private static async Task SeedUsersAsync(
        ApplicationDbContext context)
    {
        if (await context.Users.AnyAsync())
            return;


        var departments =
            await context.Departments.ToDictionaryAsync(
                d => d.Name,
                d => d.Id);


        var users = new List<User>
        {
            // Student

            new()
            {
                FullName = "Rahul Sharma",
                Email = "rahul@student.edu",
                Role = UserRole.Student,
                StudentId = "STU2026001",
                DepartmentId = departments["Computer Science"],
                IsActive = true
            },


            // Staff - Accounts

            new()
            {
                FullName = "Priya Kumar",
                Email = "priya.accounts@edusupport.edu",
                Role = UserRole.Staff,
                EmployeeId = "EMP1001",
                DepartmentId = departments["Accounts"],
                IsActive = true
            },


            // Staff - Examination

            new()
            {
                FullName = "Arun Kumar",
                Email = "arun.exam@edusupport.edu",
                Role = UserRole.Staff,
                EmployeeId = "EMP1002",
                DepartmentId = departments["Examination"],
                IsActive = true
            },


            // Staff - IT

            new()
            {
                FullName = "Sai Kumar",
                Email = "sai.it@edusupport.edu",
                Role = UserRole.Staff,
                EmployeeId = "EMP1003",
                DepartmentId = departments["IT Support"],
                IsActive = true
            },


            // Manager

            new()
            {
                FullName = "Meena Rao",
                Email = "meena.manager@edusupport.edu",
                Role = UserRole.Manager,
                EmployeeId = "MGR1001",
                DepartmentId = departments["Administration"],
                IsActive = true
            },


            // Admin

            new()
            {
                FullName = "System Administrator",
                Email = "admin@edusupport.edu",
                Role = UserRole.Admin,
                EmployeeId = "ADM1001",
                DepartmentId = departments["Administration"],
                IsActive = true
            }
        };


        await context.Users.AddRangeAsync(users);

        await context.SaveChangesAsync();
    }
}