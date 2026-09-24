using System.ComponentModel.DataAnnotations;

namespace EduSupport.Web.Models;

public class TicketComment
{
    public int Id { get; set; }


    public int TicketId { get; set; }

    public Ticket Ticket { get; set; } = null!;


    public int UserId { get; set; }

    public User User { get; set; } = null!;


    [Required]
    public string Message { get; set; } = string.Empty;


    public DateTime CreatedAt { get; set; }
        = DateTime.UtcNow;
}