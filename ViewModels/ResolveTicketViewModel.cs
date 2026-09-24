using System.ComponentModel.DataAnnotations;

namespace EduSupport.Web.ViewModels;

public class ResolveTicketViewModel
{
    public int TicketId { get; set; }

    [Required]
    [StringLength(2000)]
    [Display(Name = "Resolution Summary")]
    public string ResolutionSummary { get; set; } = string.Empty;
}