using EduSupport.Web.Models;

namespace EduSupport.Web.ViewModels;

public class AdminDashboardViewModel
{
    public List<Category> Categories { get; set; } = new();

    public List<Department> Departments { get; set; } = new();

    public List<SlaPolicy> SlaPolicies { get; set; } = new();

    public List<User> Users { get; set; } = new();
}