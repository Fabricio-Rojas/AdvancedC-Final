using AdvancedC_Final.Areas.Identity.Data;
using AdvancedC_Final.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AdvancedC_Final.Data;

public class TaskManagerContext : IdentityDbContext<TaskManagerUser>
{
    public TaskManagerContext(DbContextOptions<TaskManagerContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        builder.Entity<DeveloperProject>()
            .HasOne(dp => dp.Developer)
            .WithMany()
            .HasForeignKey(dp => dp.DeveloperId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<DeveloperProject>()
            .HasOne(dp => dp.Project)
            .WithMany()
            .HasForeignKey(dp => dp.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<DeveloperTicket>()
            .HasOne(dp => dp.User)
            .WithMany()
            .HasForeignKey(dp => dp.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<DeveloperTicket>()
            .HasOne(dp => dp.Ticket)
            .WithMany()
            .HasForeignKey(dp => dp.TickedId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public DbSet<Ticket> Tickets { get; set; } = default!;
    public DbSet<DeveloperTicket> DeveloperTickets { get; set; } = default!;
    public DbSet<Project> Projects { get; set; } = default!;
    public DbSet<DeveloperProject> DeveloperProjects { get; set;} = default!;
}