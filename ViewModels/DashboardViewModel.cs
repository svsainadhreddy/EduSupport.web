using EduSupport.Web.Models;

namespace EduSupport.Web.ViewModels;

public class DashboardViewModel
{
    // =========================================================
    // TICKET SUMMARY
    // =========================================================

    public int TotalTickets { get; set; }

    public int OpenTickets { get; set; }

    public int PendingTickets { get; set; }

    public int ResolvedTickets { get; set; }

    public int ClosedTickets { get; set; }


    // =========================================================
    // SLA SUMMARY
    // =========================================================

    public int SlaAtRisk { get; set; }

    public int SlaBreached { get; set; }


    // =========================================================
    // WORKLOAD / OPERATIONS
    // =========================================================

    public int UnassignedTickets { get; set; }

    public int ResolvedToday { get; set; }


    // =========================================================
    // DASHBOARD LISTS
    // =========================================================

    public List<Ticket> RecentTickets { get; set; } = new();

    public List<TicketSlaAlertViewModel> SlaAlerts { get; set; } = new();

    public List<Ticket> PendingActions { get; set; } = new();
}