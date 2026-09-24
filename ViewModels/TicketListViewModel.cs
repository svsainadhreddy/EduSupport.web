using EduSupport.Web.Models;

namespace EduSupport.Web.ViewModels;

public class TicketListViewModel
{
    public List<Ticket> Tickets { get; set; } = new();

    public string? Search { get; set; }

    public string? Status { get; set; }

    public string? Priority { get; set; }

    public int TotalTickets { get; set; }

    public int OpenTickets { get; set; }

    public int PendingTickets { get; set; }

    public int ResolvedTickets { get; set; }
}