using System.ComponentModel.DataAnnotations;

namespace EduSupport.Web.ViewModels;

public class AddCommentViewModel
{
    public int TicketId { get; set; }

    [Required]
    [StringLength(2000)]
    public string Message { get; set; } = string.Empty;
}