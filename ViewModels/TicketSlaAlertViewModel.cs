using EduSupport.Web.Models;

namespace EduSupport.Web.ViewModels;

public class TicketSlaAlertViewModel
{
    public Ticket Ticket { get; set; } = null!;

    public bool IsBreached { get; set; }

    public bool IsAtRisk { get; set; }
}