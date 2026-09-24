using System.ComponentModel.DataAnnotations;
using EduSupport.Web.Models.Enums;

namespace EduSupport.Web.Models;

public class User
{
    public int Id { get; set; }


    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;


    [Required]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;


    [Required]
    public UserRole Role { get; set; }


    // Student-specific identifier
    [MaxLength(50)]
    public string? StudentId { get; set; }


    // Staff/Admin identifier
    [MaxLength(50)]
    public string? EmployeeId { get; set; }


    // Organizational department

    public int? DepartmentId { get; set; }

    public Department? Department { get; set; }


    public bool IsActive { get; set; } = true;


    public DateTime CreatedAt { get; set; }
        = DateTime.UtcNow;
}