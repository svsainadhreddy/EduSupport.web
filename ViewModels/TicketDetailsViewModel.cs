using EduSupport.Web.Models;

namespace EduSupport.Web.ViewModels;

public class TicketDetailsViewModel
{
    public Ticket Ticket { get; set; } = null!;

    public List<TicketComment> Comments { get; set; } = new();

    public List<TicketActivity> Activities { get; set; } = new();

    public TimeSpan Age { get; set; }

    public bool IsResponseBreached { get; set; }

    public bool IsResolutionBreached { get; set; }

    public bool IsResolutionAtRisk { get; set; }
}