using HvacERP.Domain.Dispatch;
using HvacERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HvacERP.Infrastructure.Tests.Persistence;

public sealed class HvacErpDbContextTests
{
    [Fact]
    public void Model_ContainsWorkOrderScheduleEntity()
    {
        using HvacErpDbContext context =
            CreateContext();

        IEntityType? entityType =
            context.Model.FindEntityType(
                typeof(WorkOrderSchedule));

        Assert.NotNull(
            entityType);

        Assert.Equal(
            "work_order_schedules",
            entityType.GetTableName());

        Assert.Equal(
            "dispatch",
            entityType.GetSchema());
    }

    [Fact]
    public void Model_ContainsScheduleTechnicianEntity()
    {
        using HvacErpDbContext context =
            CreateContext();

        IEntityType? entityType =
            context.Model.FindEntityType(
                typeof(WorkOrderScheduleTechnician));

        Assert.NotNull(
            entityType);

        Assert.Equal(
            "work_order_schedule_technicians",
            entityType.GetTableName());

        Assert.Equal(
            "dispatch",
            entityType.GetSchema());
    }

    [Fact]
    public void Model_ConfiguresScheduleToTechnicianRelationship()
    {
        using HvacErpDbContext context =
            CreateContext();

        IEntityType technicianEntityType =
            context.Model.FindEntityType(
                typeof(WorkOrderScheduleTechnician))
            ?? throw new InvalidOperationException(
                "Technician entity configuration was not found.");

        IForeignKey foreignKey =
            Assert.Single(
                technicianEntityType.GetForeignKeys());

        Assert.Equal(
            typeof(WorkOrderSchedule),
            foreignKey.PrincipalEntityType.ClrType);

        Assert.Equal(
            DeleteBehavior.Cascade,
            foreignKey.DeleteBehavior);
    }

    [Fact]
    public void Model_ConfiguresUniqueWorkOrderSchedule()
    {
        using HvacErpDbContext context =
            CreateContext();

        IEntityType scheduleEntityType =
            context.Model.FindEntityType(
                typeof(WorkOrderSchedule))
            ?? throw new InvalidOperationException(
                "Schedule entity configuration was not found.");

        IIndex workOrderIndex =
            scheduleEntityType
                .GetIndexes()
                .Single(
                    index =>
                        index.Properties.Count == 1 &&
                        index.Properties[0].Name ==
                        nameof(WorkOrderSchedule.WorkOrderId));

        Assert.True(
            workOrderIndex.IsUnique);
    }

    [Fact]
    public void Model_ConfiguresUniqueTechnicianPerSchedule()
    {
        using HvacErpDbContext context =
            CreateContext();

        IEntityType technicianEntityType =
            context.Model.FindEntityType(
                typeof(WorkOrderScheduleTechnician))
            ?? throw new InvalidOperationException(
                "Technician entity configuration was not found.");

        IIndex assignmentIndex =
            technicianEntityType
                .GetIndexes()
                .Single(
                    index =>
                        index.Properties.Count == 2 &&
                        index.Properties[0].Name ==
                        nameof(
                            WorkOrderScheduleTechnician.WorkOrderScheduleId) &&
                        index.Properties[1].Name ==
                        nameof(
                            WorkOrderScheduleTechnician.TechnicianUserId));

        Assert.True(
            assignmentIndex.IsUnique);
    }

    [Fact]
    public void Model_UsesStringConversionsForEnums()
    {
        using HvacErpDbContext context =
            CreateContext();

        IEntityType scheduleEntityType =
            context.Model.FindEntityType(
                typeof(WorkOrderSchedule))
            ?? throw new InvalidOperationException(
                "Schedule entity configuration was not found.");

        IProperty statusProperty =
            scheduleEntityType.FindProperty(
                nameof(WorkOrderSchedule.Status))
            ?? throw new InvalidOperationException(
                "Schedule status configuration was not found.");

        Assert.Equal(
            typeof(string),
            statusProperty.GetProviderClrType());
    }

    private static HvacErpDbContext CreateContext()
    {
        DbContextOptions<HvacErpDbContext> options =
            new DbContextOptionsBuilder<HvacErpDbContext>()
                .UseNpgsql(
                    "Host=localhost;Database=hvac_erp_model_tests;Username=postgres;Password=postgres")
                .Options;

        return new HvacErpDbContext(
            options);
    }
}
