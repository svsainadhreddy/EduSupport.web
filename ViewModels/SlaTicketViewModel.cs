using EduSupport.Web.Models.Enums;

namespace EduSupport.Web.ViewModels;

public class SlaTicketViewModel
{
    public int TicketId { get; set; }

    public string TicketNumber { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public TicketPriority Priority { get; set; }

    public TicketStatus Status { get; set; }

    public string? AssignedTo { get; set; }

    public DateTime? DueAt { get; set; }

    public TimeSpan Age { get; set; }

    public bool IsBreached { get; set; }

    public bool IsAtRisk { get; set; }
}