using System.ComponentModel.DataAnnotations;

namespace EduSupport.Web.ViewModels;

public class AssignTicketViewModel
{
    public int TicketId { get; set; }

    [Required]
    [Display(Name = "Assign To")]
    public int StaffId { get; set; }
}