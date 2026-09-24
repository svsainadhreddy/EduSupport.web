using Microsoft.AspNetCore.Identity;

namespace EduSupport.Web.Models;

public class ApplicationUser : IdentityUser<int>
{
    public int DomainUserId { get; set; }
}