using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace EduSupport.Web.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Ticket> Tickets { get; set; }
        = new List<Ticket>();
}