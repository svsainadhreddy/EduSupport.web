using EduSupport.Web.Models;
using EduSupport.Web.Models.Enums;

namespace EduSupport.Web.Services.Interfaces;

public interface ISlaService
{
    Task<SlaPolicy?> GetPolicyAsync(
        TicketPriority priority);

    Task EvaluateSlaAsync(Ticket ticket);

    Task ApplySlaAsync(Ticket ticket);

    bool IsResponseBreached(Ticket ticket);

    bool IsResolutionBreached(Ticket ticket);

    bool IsResolutionAtRisk(Ticket ticket);

    TimeSpan GetAge(Ticket ticket);
}