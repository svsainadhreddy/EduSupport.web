using System.ComponentModel.DataAnnotations;
using EduSupport.Web.Models.Enums;

namespace EduSupport.Web.Models;

public class SlaPolicy
{
    public int Id { get; set; }


    [Required]
    public TicketPriority Priority { get; set; }


    // Maximum time allowed before the first staff response.
    [Range(1, 10080)]
    public int FirstResponseMinutes { get; set; }


    // Maximum time allowed to resolve the ticket.
    [Range(1, 43200)]
    public int ResolutionMinutes { get; set; }


    public bool IsActive { get; set; } = true;


    public DateTime CreatedAt { get; set; }
        = DateTime.UtcNow;


    public DateTime? UpdatedAt { get; set; }
}