using System.ComponentModel.DataAnnotations;

namespace EduSupport.Web.Models;

public class TicketActivity
{
    public int Id { get; set; }


    public int TicketId { get; set; }

    public Ticket Ticket { get; set; } = null!;


    public int UserId { get; set; }

    public User User { get; set; } = null!;


    [Required]
    [MaxLength(100)]
    public string Action { get; set; } = string.Empty;


    [MaxLength(500)]
    public string? Details { get; set; }


    public DateTime CreatedAt { get; set; }
        = DateTime.UtcNow;
}