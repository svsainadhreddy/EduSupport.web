using EduSupport.Web.Models;
using EduSupport.Web.Models.Enums;

namespace EduSupport.Web.Services.Interfaces;

public interface ITicketService
{
    Task<Ticket> CreateTicketAsync(
        Ticket ticket,
        int createdById);

    Task<Ticket?> GetTicketByIdAsync(int id);

    Task<List<Ticket>> GetAllTicketsAsync();

    Task<List<Ticket>> GetTicketsForUserAsync(int userId);

    Task AssignTicketAsync(
        int ticketId,
        int staffId,
        int performedByUserId);

    Task ChangeStatusAsync(
        int ticketId,
        TicketStatus newStatus,
        int performedByUserId,
        PendingReason? pendingReason = null);

    Task AddCommentAsync(
        int ticketId,
        int userId,
        string message);

    Task ResolveTicketAsync(
        int ticketId,
        int userId,
        string resolutionSummary);

    Task CloseTicketAsync(
        int ticketId,
        int userId);

    Task ReopenTicketAsync(
        int ticketId,
        int userId);
}