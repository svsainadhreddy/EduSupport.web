using System.ComponentModel.DataAnnotations;
using EduSupport.Web.Models.Enums;

namespace EduSupport.Web.ViewModels;

public class ChangeStatusViewModel
{
    public int TicketId { get; set; }

    [Required]
    [Display(Name = "Status")]
    public TicketStatus Status { get; set; }

    [Display(Name = "Pending Reason")]
    public PendingReason? PendingReason { get; set; }
}