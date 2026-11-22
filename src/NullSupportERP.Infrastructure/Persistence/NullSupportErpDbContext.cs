using NullSupportERP.Domain.Dispatch;
using Microsoft.EntityFrameworkCore;

namespace NullSupportERP.Infrastructure.Persistence;

public sealed class NullSupportErpDbContext : DbContext
{
    public NullSupportErpDbContext(
        DbContextOptions<NullSupportErpDbContext> options)
        : base(options)
    {
    }

    public DbSet<WorkOrderSchedule> WorkOrderSchedules =>
        Set<WorkOrderSchedule>();

    public DbSet<WorkOrderScheduleTechnician> WorkOrderScheduleTechnicians =>
        Set<WorkOrderScheduleTechnician>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(NullSupportErpDbContext).Assembly);
    }
}
