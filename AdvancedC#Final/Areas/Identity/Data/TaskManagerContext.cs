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
    }

    public DbSet<Ticket> Tickets { get; set; } = default!;
    public DbSet<DeveloperTicket> DeveloperTickets { get; set; } = default!;
}
