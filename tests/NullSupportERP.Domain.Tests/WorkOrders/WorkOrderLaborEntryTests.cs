using NullSupportERP.Domain.WorkOrders;

namespace NullSupportERP.Domain.Tests.WorkOrders;

public sealed class WorkOrderLaborEntryTests
{
    private static readonly Guid WorkOrderId =
        Guid.Parse(
            "11111111-1111-1111-1111-111111111111");

    private static readonly Guid CompanyId =
        Guid.Parse(
            "22222222-2222-2222-2222-222222222222");

    private static readonly Guid CustomerId =
        Guid.Parse(
            "33333333-3333-3333-3333-333333333333");

    private static readonly Guid LocationId =
        Guid.Parse(
            "44444444-4444-4444-4444-444444444444");

    private static readonly Guid EquipmentId =
        Guid.Parse(
            "55555555-5555-5555-5555-555555555555");

    private static readonly Guid UserId =
        Guid.Parse(
            "66666666-6666-6666-6666-666666666666");

    private static readonly Guid TechnicianId =
        Guid.Parse(
            "77777777-7777-7777-7777-777777777777");

    private static readonly Guid SecondTechnicianId =
        Guid.Parse(
            "77777777-7777-7777-7777-777777777778");

    private static readonly Guid LaborEntryId =
        Guid.Parse(
            "88888888-8888-8888-8888-888888888888");

    private static readonly DateTimeOffset CreatedAtUtc =
        new(
            2026,
            7,
            28,
            12,
            0,
            0,
            TimeSpan.Zero);

    [Fact]
    public void AddLaborEntry_WithValidValues_AddsLaborEntry()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        DateTimeOffset startedAtUtc =
            CreatedAtUtc.AddHours(1);

        DateTimeOffset endedAtUtc =
            startedAtUtc.AddHours(2);

        WorkOrderLaborEntry laborEntry =
            workOrder.AddLaborEntry(
                LaborEntryId,
                TechnicianId,
                WorkOrderLaborType.Regular,
                startedAtUtc,
                endedAtUtc,
                true,
                125m,
                "  Diagnosed failed capacitor.  ",
                CreatedAtUtc.AddHours(4),
                UserId);

        Assert.Single(
            workOrder.LaborEntries);

        Assert.Equal(
            LaborEntryId,
            laborEntry.Id);

        Assert.Equal(
            TechnicianId,
            laborEntry.TechnicianUserId);

        Assert.Equal(
            WorkOrderLaborType.Regular,
            laborEntry.Type);

        Assert.Equal(
            startedAtUtc,
            laborEntry.StartedAtUtc);

        Assert.Equal(
            endedAtUtc,
            laborEntry.EndedAtUtc);

        Assert.True(
            laborEntry.IsBillable);

        Assert.Equal(
            125m,
            laborEntry.HourlyRate);

        Assert.Equal(
            "Diagnosed failed capacitor.",
            laborEntry.Notes);

        Assert.Equal(
            2m,
            laborEntry.DurationHours);

        Assert.Equal(
            250m,
            laborEntry.BillableAmount);
    }

    [Fact]
    public void AddLaborEntry_WithBlankNotes_StoresNull()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        WorkOrderLaborEntry laborEntry =
            workOrder.AddLaborEntry(
                LaborEntryId,
                TechnicianId,
                WorkOrderLaborType.Regular,
                CreatedAtUtc.AddHours(1),
                CreatedAtUtc.AddHours(2),
                true,
                100m,
                " ",
                CreatedAtUtc.AddHours(3),
                UserId);

        Assert.Null(
            laborEntry.Notes);
    }

    [Fact]
    public void AddLaborEntry_WithPartialHour_CalculatesRoundedDuration()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        WorkOrderLaborEntry laborEntry =
            workOrder.AddLaborEntry(
                LaborEntryId,
                TechnicianId,
                WorkOrderLaborType.Diagnostic,
                CreatedAtUtc.AddHours(1),
                CreatedAtUtc
                    .AddHours(2)
                    .AddMinutes(20),
                true,
                120m,
                null,
                CreatedAtUtc.AddHours(3),
                UserId);

        Assert.Equal(
            1.33m,
            laborEntry.DurationHours);

        Assert.Equal(
            159.60m,
            laborEntry.BillableAmount);
    }

    [Fact]
    public void AddLaborEntry_WhenNonBillable_ReturnsZeroBillableAmount()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        WorkOrderLaborEntry laborEntry =
            workOrder.AddLaborEntry(
                LaborEntryId,
                TechnicianId,
                WorkOrderLaborType.Warranty,
                CreatedAtUtc.AddHours(1),
                CreatedAtUtc.AddHours(3),
                false,
                125m,
                "Warranty callback.",
                CreatedAtUtc.AddHours(4),
                UserId);

        Assert.False(
            laborEntry.IsBillable);

        Assert.Equal(
            2m,
            laborEntry.DurationHours);

        Assert.Equal(
            0m,
            laborEntry.BillableAmount);
    }

    [Fact]
    public void AddLaborEntry_WithZeroHourlyRate_IsAllowed()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        WorkOrderLaborEntry laborEntry =
            workOrder.AddLaborEntry(
                LaborEntryId,
                TechnicianId,
                WorkOrderLaborType.Training,
                CreatedAtUtc.AddHours(1),
                CreatedAtUtc.AddHours(2),
                false,
                0m,
                null,
                CreatedAtUtc.AddHours(3),
                UserId);

        Assert.Equal(
            0m,
            laborEntry.HourlyRate);

        Assert.Equal(
            0m,
            laborEntry.BillableAmount);
    }

    [Fact]
    public void AddLaborEntry_WithDuplicateId_ThrowsInvalidOperationException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        AddStandardLaborEntry(
            workOrder);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => AddStandardLaborEntry(
                    workOrder));

        Assert.Equal(
            "A work order labor entry with the same ID already exists.",
            exception.Message);
    }

    [Fact]
    public void AddLaborEntry_WithEmptyId_ThrowsArgumentException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => workOrder.AddLaborEntry(
                    Guid.Empty,
                    TechnicianId,
                    WorkOrderLaborType.Regular,
                    CreatedAtUtc.AddHours(1),
                    CreatedAtUtc.AddHours(2),
                    true,
                    100m,
                    null,
                    CreatedAtUtc.AddHours(3),
                    UserId));

        Assert.Equal(
            "laborEntryId",
            exception.ParamName);
    }

    [Fact]
    public void AddLaborEntry_WithEmptyTechnicianId_ThrowsArgumentException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => workOrder.AddLaborEntry(
                    LaborEntryId,
                    Guid.Empty,
                    WorkOrderLaborType.Regular,
                    CreatedAtUtc.AddHours(1),
                    CreatedAtUtc.AddHours(2),
                    true,
                    100m,
                    null,
                    CreatedAtUtc.AddHours(3),
                    UserId));

        Assert.Equal(
            "technicianUserId",
            exception.ParamName);
    }

    [Fact]
    public void AddLaborEntry_WithInvalidType_ThrowsArgumentOutOfRangeException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => workOrder.AddLaborEntry(
                LaborEntryId,
                TechnicianId,
                (WorkOrderLaborType)500,
                CreatedAtUtc.AddHours(1),
                CreatedAtUtc.AddHours(2),
                true,
                100m,
                null,
                CreatedAtUtc.AddHours(3),
                UserId));
    }

    [Fact]
    public void AddLaborEntry_WithEndBeforeStart_ThrowsArgumentException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => workOrder.AddLaborEntry(
                    LaborEntryId,
                    TechnicianId,
                    WorkOrderLaborType.Regular,
                    CreatedAtUtc.AddHours(2),
                    CreatedAtUtc.AddHours(1),
                    true,
                    100m,
                    null,
                    CreatedAtUtc.AddHours(3),
                    UserId));

        Assert.Equal(
            "endedAtUtc",
            exception.ParamName);
    }

    [Fact]
    public void AddLaborEntry_WithMatchingStartAndEnd_ThrowsArgumentException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        DateTimeOffset time =
            CreatedAtUtc.AddHours(1);

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => workOrder.AddLaborEntry(
                    LaborEntryId,
                    TechnicianId,
                    WorkOrderLaborType.Regular,
                    time,
                    time,
                    true,
                    100m,
                    null,
                    CreatedAtUtc.AddHours(3),
                    UserId));

        Assert.Equal(
            "endedAtUtc",
            exception.ParamName);
    }

    [Fact]
    public void AddLaborEntry_ExceedingTwentyFourHours_ThrowsArgumentException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => workOrder.AddLaborEntry(
                    LaborEntryId,
                    TechnicianId,
                    WorkOrderLaborType.Regular,
                    CreatedAtUtc,
                    CreatedAtUtc.AddHours(25),
                    true,
                    100m,
                    null,
                    CreatedAtUtc.AddHours(26),
                    UserId));

        Assert.Equal(
            "endedAtUtc",
            exception.ParamName);
    }

    [Fact]
    public void AddLaborEntry_WithNegativeHourlyRate_ThrowsArgumentOutOfRangeException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        ArgumentOutOfRangeException exception =
            Assert.Throws<ArgumentOutOfRangeException>(
                () => workOrder.AddLaborEntry(
                    LaborEntryId,
                    TechnicianId,
                    WorkOrderLaborType.Regular,
                    CreatedAtUtc.AddHours(1),
                    CreatedAtUtc.AddHours(2),
                    true,
                    -1m,
                    null,
                    CreatedAtUtc.AddHours(3),
                    UserId));

        Assert.Equal(
            "hourlyRate",
            exception.ParamName);
    }

    [Fact]
    public void AddLaborEntry_WithExcessiveHourlyRate_ThrowsArgumentOutOfRangeException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        ArgumentOutOfRangeException exception =
            Assert.Throws<ArgumentOutOfRangeException>(
                () => workOrder.AddLaborEntry(
                    LaborEntryId,
                    TechnicianId,
                    WorkOrderLaborType.Regular,
                    CreatedAtUtc.AddHours(1),
                    CreatedAtUtc.AddHours(2),
                    true,
                    10_001m,
                    null,
                    CreatedAtUtc.AddHours(3),
                    UserId));

        Assert.Equal(
            "hourlyRate",
            exception.ParamName);
    }

    [Fact]
    public void UpdateLaborEntry_WithValidValues_UpdatesEntry()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        AddStandardLaborEntry(
            workOrder);

        DateTimeOffset newStart =
            CreatedAtUtc.AddHours(2);

        DateTimeOffset newEnd =
            CreatedAtUtc.AddHours(5);

        workOrder.UpdateLaborEntry(
            LaborEntryId,
            SecondTechnicianId,
            WorkOrderLaborType.Overtime,
            newStart,
            newEnd,
            true,
            187.50m,
            "  Emergency overtime repair.  ",
            CreatedAtUtc.AddHours(6),
            UserId);

        WorkOrderLaborEntry laborEntry =
            Assert.Single(
                workOrder.LaborEntries);

        Assert.Equal(
            SecondTechnicianId,
            laborEntry.TechnicianUserId);

        Assert.Equal(
            WorkOrderLaborType.Overtime,
            laborEntry.Type);

        Assert.Equal(
            newStart,
            laborEntry.StartedAtUtc);

        Assert.Equal(
            newEnd,
            laborEntry.EndedAtUtc);

        Assert.Equal(
            3m,
            laborEntry.DurationHours);

        Assert.Equal(
            187.50m,
            laborEntry.HourlyRate);

        Assert.Equal(
            562.50m,
            laborEntry.BillableAmount);

        Assert.Equal(
            "Emergency overtime repair.",
            laborEntry.Notes);
    }

    [Fact]
    public void UpdateLaborEntry_WhenEntryDoesNotExist_ThrowsInvalidOperationException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => workOrder.UpdateLaborEntry(
                    LaborEntryId,
                    TechnicianId,
                    WorkOrderLaborType.Regular,
                    CreatedAtUtc.AddHours(1),
                    CreatedAtUtc.AddHours(2),
                    true,
                    100m,
                    null,
                    CreatedAtUtc.AddHours(3),
                    UserId));

        Assert.Equal(
            "The work order labor entry does not exist.",
            exception.Message);
    }

    [Fact]
    public void RemoveLaborEntry_WhenEntryExists_RemovesEntry()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        AddStandardLaborEntry(
            workOrder);

        workOrder.RemoveLaborEntry(
            LaborEntryId,
            CreatedAtUtc.AddHours(4),
            UserId);

        Assert.Empty(
            workOrder.LaborEntries);
    }

    [Fact]
    public void RemoveLaborEntry_WhenEntryDoesNotExist_ThrowsInvalidOperationException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => workOrder.RemoveLaborEntry(
                    LaborEntryId,
                    CreatedAtUtc.AddHours(1),
                    UserId));

        Assert.Equal(
            "The work order labor entry does not exist.",
            exception.Message);
    }

    [Fact]
    public void LaborSummaries_WithMultipleEntries_ReturnCorrectTotals()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        workOrder.AddLaborEntry(
            Guid.Parse(
                "88888888-8888-8888-8888-888888888881"),
            TechnicianId,
            WorkOrderLaborType.Regular,
            CreatedAtUtc.AddHours(1),
            CreatedAtUtc.AddHours(3),
            true,
            100m,
            null,
            CreatedAtUtc.AddHours(4),
            UserId);

        workOrder.AddLaborEntry(
            Guid.Parse(
                "88888888-8888-8888-8888-888888888882"),
            TechnicianId,
            WorkOrderLaborType.Travel,
            CreatedAtUtc.AddHours(3),
            CreatedAtUtc.AddHours(4),
            false,
            100m,
            null,
            CreatedAtUtc.AddHours(5),
            UserId);

        workOrder.AddLaborEntry(
            Guid.Parse(
                "88888888-8888-8888-8888-888888888883"),
            TechnicianId,
            WorkOrderLaborType.Overtime,
            CreatedAtUtc.AddHours(4),
            CreatedAtUtc.AddHours(5.5),
            true,
            150m,
            null,
            CreatedAtUtc.AddHours(6),
            UserId);

        Assert.Equal(
            4.5m,
            workOrder.TotalLaborHours);

        Assert.Equal(
            3.5m,
            workOrder.BillableLaborHours);

        Assert.Equal(
            1m,
            workOrder.NonBillableLaborHours);

        Assert.Equal(
            425m,
            workOrder.LaborSubtotal);
    }

    [Fact]
    public void WorkOrderSubtotal_IncludesLineItemsAndLabor()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        workOrder.AddLineItem(
            Guid.Parse(
                "99999999-9999-9999-9999-999999999999"),
            WorkOrderLineItemType.Part,
            "CAP-45-5",
            "Dual run capacitor",
            2,
            35.50m,
            true,
            CreatedAtUtc.AddHours(1),
            UserId);

        workOrder.AddLaborEntry(
            LaborEntryId,
            TechnicianId,
            WorkOrderLaborType.Regular,
            CreatedAtUtc.AddHours(1),
            CreatedAtUtc.AddHours(3),
            true,
            100m,
            null,
            CreatedAtUtc.AddHours(4),
            UserId);

        Assert.Equal(
            71m,
            workOrder.LineItemSubtotal);

        Assert.Equal(
            200m,
            workOrder.LaborSubtotal);

        Assert.Equal(
            271m,
            workOrder.WorkOrderSubtotal);
    }

    [Fact]
    public void AddLaborEntry_WhenWorkOrderCompleted_ThrowsInvalidOperationException()
    {
        WorkOrder workOrder =
            CreateCompletedWorkOrder();

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => AddStandardLaborEntry(
                    workOrder));

        Assert.Equal(
            "A completed or cancelled work order cannot be changed.",
            exception.Message);
    }

    [Fact]
    public void UpdateLaborEntry_WhenWorkOrderCancelled_ThrowsInvalidOperationException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        AddStandardLaborEntry(
            workOrder);

        workOrder.Cancel(
            "Customer cancelled.",
            CreatedAtUtc.AddHours(4),
            UserId);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => workOrder.UpdateLaborEntry(
                    LaborEntryId,
                    TechnicianId,
                    WorkOrderLaborType.Regular,
                    CreatedAtUtc.AddHours(1),
                    CreatedAtUtc.AddHours(2),
                    true,
                    100m,
                    null,
                    CreatedAtUtc.AddHours(5),
                    UserId));

        Assert.Equal(
            "A completed or cancelled work order cannot be changed.",
            exception.Message);
    }

    [Fact]
    public void RemoveLaborEntry_WhenWorkOrderCompleted_ThrowsInvalidOperationException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        AddStandardLaborEntry(
            workOrder);

        CompleteWorkOrder(
            workOrder);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => workOrder.RemoveLaborEntry(
                    LaborEntryId,
                    CreatedAtUtc.AddHours(8),
                    UserId));

        Assert.Equal(
            "A completed or cancelled work order cannot be changed.",
            exception.Message);
    }

    [Fact]
    public void AddLaborEntry_UpdatesAuditInformation()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        DateTimeOffset modifiedAtUtc =
            CreatedAtUtc.AddHours(4);

        workOrder.AddLaborEntry(
            LaborEntryId,
            TechnicianId,
            WorkOrderLaborType.Regular,
            CreatedAtUtc.AddHours(1),
            CreatedAtUtc.AddHours(2),
            true,
            100m,
            null,
            modifiedAtUtc,
            UserId);

        Assert.Equal(
            modifiedAtUtc,
            workOrder.LastModifiedAtUtc);

        Assert.Equal(
            UserId,
            workOrder.LastModifiedByUserId);
    }

    private static void AddStandardLaborEntry(
        WorkOrder workOrder)
    {
        workOrder.AddLaborEntry(
            LaborEntryId,
            TechnicianId,
            WorkOrderLaborType.Regular,
            CreatedAtUtc.AddHours(1),
            CreatedAtUtc.AddHours(3),
            true,
            125m,
            "Diagnosed failed capacitor.",
            CreatedAtUtc.AddHours(4),
            UserId);
    }

    private static WorkOrder CreateWorkOrder()
    {
        return WorkOrder.Create(
            WorkOrderId,
            CompanyId,
            CustomerId,
            LocationId,
            EquipmentId,
            "WO-1001",
            WorkOrderType.ServiceCall,
            WorkOrderPriority.Normal,
            "No cooling",
            "System is running but not producing cold air.",
            CreatedAtUtc.AddDays(1),
            CreatedAtUtc,
            UserId);
    }

    private static WorkOrder CreateCompletedWorkOrder()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        CompleteWorkOrder(
            workOrder);

        return workOrder;
    }

    private static void CompleteWorkOrder(
        WorkOrder workOrder)
    {
        workOrder.Open(
            CreatedAtUtc.AddHours(1),
            UserId);

        workOrder.Schedule(
            CreatedAtUtc.AddDays(1),
            CreatedAtUtc.AddDays(1).AddHours(2),
            TechnicianId,
            CreatedAtUtc.AddHours(2),
            UserId);

        workOrder.Dispatch(
            CreatedAtUtc.AddHours(3),
            UserId);

        workOrder.Start(
            CreatedAtUtc.AddHours(4),
            UserId);

        workOrder.Complete(
            CreatedAtUtc.AddHours(5),
            UserId);
    }
}
