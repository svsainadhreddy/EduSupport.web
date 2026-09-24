using System.ComponentModel.DataAnnotations;
using EduSupport.Web.Models.Enums;

namespace EduSupport.Web.Models;

public class Ticket
{
    public int Id { get; set; }


    // =====================================================
    // Basic Information
    // =====================================================

    [Required]
    [MaxLength(50)]
    public string TicketNumber { get; set; } = string.Empty;


    [Required]
    [MaxLength(200)]
    public string Subject { get; set; } = string.Empty;


    [Required]
    public string Description { get; set; } = string.Empty;


    // =====================================================
    // Workflow
    // =====================================================

    public TicketStatus Status { get; set; }
        = TicketStatus.New;


    public TicketPriority Priority { get; set; }
        = TicketPriority.Medium;
    public PendingReason? PendingReason { get; set; }

    // =====================================================
    // Category
    // =====================================================

    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;


    // =====================================================
    // Department
    // =====================================================

    public int? DepartmentId { get; set; }

    public Department? Department { get; set; }


    // =====================================================
    // Ownership
    // =====================================================

    // Student/user who created the ticket
    public int CreatedById { get; set; }

    public User CreatedBy { get; set; } = null!;


    // Staff member currently responsible for the ticket
    public int? AssignedToId { get; set; }

    public User? AssignedTo { get; set; }


    // =====================================================
    // SLA & Timing
    // =====================================================

    public DateTime CreatedAt { get; set; }
        = DateTime.UtcNow;


    public DateTime UpdatedAt { get; set; }
        = DateTime.UtcNow;


    // When the first staff response was made
    public DateTime? FirstResponseAt { get; set; }


    // SLA resolution deadline
    // Maximum time allowed for first response
    public DateTime? FirstResponseDueAt { get; set; }


    // Resolution deadline
    public DateTime? DueAt { get; set; }



    // Whether the ticket has crossed its SLA
    public bool IsSlaBreached { get; set; }


    // Whether the ticket is approaching its SLA deadline
    public bool IsSlaAtRisk { get; set; }


    // =====================================================
    // Escalation
    // =====================================================

    public DateTime? EscalatedAt { get; set; }


    // Manager/person who received the escalation
    public int? EscalatedToId { get; set; }

    public User? EscalatedTo { get; set; }


    // =====================================================
    // Resolution
    // =====================================================

    public DateTime? ResolvedAt { get; set; }


    public DateTime? ClosedAt { get; set; }


    [MaxLength(2000)]
    public string? ResolutionSummary { get; set; }


    // =====================================================
    // Navigation Properties
    // =====================================================

    public ICollection<TicketComment> Comments { get; set; }
        = new List<TicketComment>();


    public ICollection<TicketActivity> Activities { get; set; }
        = new List<TicketActivity>();
}