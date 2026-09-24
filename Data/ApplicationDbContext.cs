using EduSupport.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace EduSupport.Web.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }


    // =====================================================
    // DbSets
    // =====================================================

    public DbSet<User> Users => Set<User>();

    public DbSet<Ticket> Tickets => Set<Ticket>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<TicketComment> TicketComments => Set<TicketComment>();

    public DbSet<TicketActivity> TicketActivities => Set<TicketActivity>();

    public DbSet<SlaPolicy> SlaPolicies => Set<SlaPolicy>();

    // =====================================================
    // Model Configuration
    // =====================================================

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SlaPolicy>()
            .HasIndex(s => s.Priority)
            .IsUnique();


        // =================================================
        // User → Department
        // =================================================

        modelBuilder.Entity<User>()
            .HasOne(u => u.Department)
            .WithMany()
            .HasForeignKey(u => u.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);


        // =================================================
        // Ticket → Category
        // =================================================

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Category)
            .WithMany(c => c.Tickets)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);


        // =================================================
        // Ticket → Department
        // =================================================

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Department)
            .WithMany(d => d.Tickets)
            .HasForeignKey(t => t.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);


        // =================================================
        // Ticket → CreatedBy
        // =================================================

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.CreatedBy)
            .WithMany()
            .HasForeignKey(t => t.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);


        // =================================================
        // Ticket → AssignedTo
        // =================================================

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.AssignedTo)
            .WithMany()
            .HasForeignKey(t => t.AssignedToId)
            .OnDelete(DeleteBehavior.Restrict);


        // =================================================
        // Ticket → EscalatedTo
        // =================================================

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.EscalatedTo)
            .WithMany()
            .HasForeignKey(t => t.EscalatedToId)
            .OnDelete(DeleteBehavior.Restrict);


        // =================================================
        // TicketComment → Ticket
        // =================================================

        modelBuilder.Entity<TicketComment>()
            .HasOne(c => c.Ticket)
            .WithMany(t => t.Comments)
            .HasForeignKey(c => c.TicketId)
            .OnDelete(DeleteBehavior.Cascade);


        // =================================================
        // TicketComment → User
        // =================================================

        modelBuilder.Entity<TicketComment>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);


        // =================================================
        // TicketActivity → Ticket
        // =================================================

        modelBuilder.Entity<TicketActivity>()
            .HasOne(a => a.Ticket)
            .WithMany(t => t.Activities)
            .HasForeignKey(a => a.TicketId)
            .OnDelete(DeleteBehavior.Cascade);


        // =================================================
        // TicketActivity → User
        // =================================================

        modelBuilder.Entity<TicketActivity>()
            .HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);


        // =================================================
        // Indexes
        // =================================================

        modelBuilder.Entity<Ticket>()
            .HasIndex(t => t.TicketNumber)
            .IsUnique();


        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();


        modelBuilder.Entity<Ticket>()
            .HasIndex(t => t.Status);


        modelBuilder.Entity<Ticket>()
            .HasIndex(t => t.Priority);


        modelBuilder.Entity<Ticket>()
            .HasIndex(t => t.AssignedToId);


        modelBuilder.Entity<Ticket>()
            .HasIndex(t => t.CreatedAt);
    }
}