using EduSupport.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EduSupport.Web.Data;

public class ApplicationIdentityDbContext
    : IdentityDbContext<
        ApplicationUser,
        IdentityRole<int>,
        int>
{
    public ApplicationIdentityDbContext(
        DbContextOptions<ApplicationIdentityDbContext> options)
        : base(options)
    {
    }
}