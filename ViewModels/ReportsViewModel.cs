using EduSupport.Web.Models.Enums;

namespace EduSupport.Web.ViewModels;

public class ReportsViewModel
{
    public int TotalTickets { get; set; }

    public int OpenTickets { get; set; }

    public int PendingTickets { get; set; }

    public int ResolvedTickets { get; set; }

    public int ClosedTickets { get; set; }

    public int SlaAtRisk { get; set; }

    public int SlaBreached { get; set; }

    public int UnassignedTickets { get; set; }

    public List<PriorityReportItem> ByPriority { get; set; } = new();

    public List<DepartmentReportItem> ByDepartment { get; set; } = new();

    public List<StaffWorkloadItem> StaffWorkload { get; set; } = new();
}

public class PriorityReportItem
{
    public TicketPriority Priority { get; set; }

    public int Count { get; set; }
}

public class DepartmentReportItem
{
    public string DepartmentName { get; set; } = string.Empty;

    public int Count { get; set; }
}

public class StaffWorkloadItem
{
    public string StaffName { get; set; } = string.Empty;

    public int AssignedTickets { get; set; }

    public int OpenTickets { get; set; }

    public int PendingTickets { get; set; }

    public int ResolvedTickets { get; set; }
}