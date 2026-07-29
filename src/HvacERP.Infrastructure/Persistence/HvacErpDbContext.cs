using HvacERP.Domain.Dispatch;
using Microsoft.EntityFrameworkCore;

namespace HvacERP.Infrastructure.Persistence;

public sealed class HvacErpDbContext : DbContext
{
    public HvacErpDbContext(
        DbContextOptions<HvacErpDbContext> options)
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
            typeof(HvacErpDbContext).Assembly);
    }
}
