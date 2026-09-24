namespace EduSupport.Web.ViewModels;

public class SlaMonitoringViewModel
{
    public int TotalOpen { get; set; }

    public int Normal { get; set; }

    public int AtRisk { get; set; }

    public int Breached { get; set; }

    public List<SlaTicketViewModel> Tickets { get; set; }
        = new();
}