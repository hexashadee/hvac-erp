using HvacERP.Domain.WorkOrders;

namespace HvacERP.Domain.Tests.WorkOrders;

public sealed class WorkOrderTests
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
    public void Create_WithValidValues_CreatesDraftWorkOrder()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        Assert.Equal(
            WorkOrderId,
            workOrder.Id);

        Assert.Equal(
            CompanyId,
            workOrder.CompanyId);

        Assert.Equal(
            CustomerId,
            workOrder.CustomerId);

        Assert.Equal(
            LocationId,
            workOrder.CustomerLocationId);

        Assert.Equal(
            EquipmentId,
            workOrder.EquipmentId);

        Assert.Equal(
            "WO-1001",
            workOrder.WorkOrderNumber);

        Assert.Equal(
            WorkOrderType.ServiceCall,
            workOrder.Type);

        Assert.Equal(
            WorkOrderPriority.Normal,
            workOrder.Priority);

        Assert.Equal(
            WorkOrderStatus.Draft,
            workOrder.Status);

        Assert.Equal(
            "No cooling",
            workOrder.Summary);

        Assert.Equal(
            "System is running but not producing cold air.",
            workOrder.Description);

        Assert.Null(
            workOrder.AssignedTechnicianUserId);

        Assert.Null(
            workOrder.CompletedAtUtc);

        Assert.Null(
            workOrder.CancelledAtUtc);
    }

    [Fact]
    public void Create_TrimsAndNormalizesValues()
    {
        WorkOrder workOrder =
            WorkOrder.Create(
                WorkOrderId,
                CompanyId,
                CustomerId,
                LocationId,
                EquipmentId,
                "  wo-1001  ",
                WorkOrderType.ServiceCall,
                WorkOrderPriority.Normal,
                "  No cooling  ",
                "  Check outdoor unit.  ",
                null,
                CreatedAtUtc,
                UserId);

        Assert.Equal(
            "WO-1001",
            workOrder.WorkOrderNumber);

        Assert.Equal(
            "No cooling",
            workOrder.Summary);

        Assert.Equal(
            "Check outdoor unit.",
            workOrder.Description);
    }

    [Fact]
    public void Create_WithBlankOptionalDescription_StoresNull()
    {
        WorkOrder workOrder =
            WorkOrder.Create(
                WorkOrderId,
                CompanyId,
                CustomerId,
                LocationId,
                null,
                "WO-1001",
                WorkOrderType.ServiceCall,
                WorkOrderPriority.Normal,
                "No cooling",
                " ",
                null,
                CreatedAtUtc,
                UserId);

        Assert.Null(workOrder.EquipmentId);
        Assert.Null(workOrder.Description);
    }

    [Fact]
    public void Create_WithEmptyCompanyId_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => WorkOrder.Create(
                    WorkOrderId,
                    Guid.Empty,
                    CustomerId,
                    LocationId,
                    EquipmentId,
                    "WO-1001",
                    WorkOrderType.ServiceCall,
                    WorkOrderPriority.Normal,
                    "No cooling",
                    null,
                    null,
                    CreatedAtUtc,
                    UserId));

        Assert.Equal(
            "companyId",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithEmptyCustomerId_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => WorkOrder.Create(
                    WorkOrderId,
                    CompanyId,
                    Guid.Empty,
                    LocationId,
                    EquipmentId,
                    "WO-1001",
                    WorkOrderType.ServiceCall,
                    WorkOrderPriority.Normal,
                    "No cooling",
                    null,
                    null,
                    CreatedAtUtc,
                    UserId));

        Assert.Equal(
            "customerId",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithEmptyLocationId_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => WorkOrder.Create(
                    WorkOrderId,
                    CompanyId,
                    CustomerId,
                    Guid.Empty,
                    EquipmentId,
                    "WO-1001",
                    WorkOrderType.ServiceCall,
                    WorkOrderPriority.Normal,
                    "No cooling",
                    null,
                    null,
                    CreatedAtUtc,
                    UserId));

        Assert.Equal(
            "customerLocationId",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithEmptyOptionalEquipmentId_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => WorkOrder.Create(
                    WorkOrderId,
                    CompanyId,
                    CustomerId,
                    LocationId,
                    Guid.Empty,
                    "WO-1001",
                    WorkOrderType.ServiceCall,
                    WorkOrderPriority.Normal,
                    "No cooling",
                    null,
                    null,
                    CreatedAtUtc,
                    UserId));

        Assert.Equal(
            "equipmentId",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithInvalidWorkOrderNumber_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => WorkOrder.Create(
                    WorkOrderId,
                    CompanyId,
                    CustomerId,
                    LocationId,
                    EquipmentId,
                    "WO 1001",
                    WorkOrderType.ServiceCall,
                    WorkOrderPriority.Normal,
                    "No cooling",
                    null,
                    null,
                    CreatedAtUtc,
                    UserId));

        Assert.Equal(
            "workOrderNumber",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithInvalidType_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => WorkOrder.Create(
                WorkOrderId,
                CompanyId,
                CustomerId,
                LocationId,
                EquipmentId,
                "WO-1001",
                (WorkOrderType)500,
                WorkOrderPriority.Normal,
                "No cooling",
                null,
                null,
                CreatedAtUtc,
                UserId));
    }

    [Fact]
    public void Create_WithInvalidPriority_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => WorkOrder.Create(
                WorkOrderId,
                CompanyId,
                CustomerId,
                LocationId,
                EquipmentId,
                "WO-1001",
                WorkOrderType.ServiceCall,
                (WorkOrderPriority)500,
                "No cooling",
                null,
                null,
                CreatedAtUtc,
                UserId));
    }

    [Fact]
    public void UpdateDetails_UpdatesWorkOrderAndAuditInformation()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        DateTimeOffset modifiedAtUtc =
            CreatedAtUtc.AddHours(1);

        workOrder.UpdateDetails(
            WorkOrderType.Emergency,
            WorkOrderPriority.Emergency,
            "  No heat  ",
            "  Furnace will not ignite.  ",
            CreatedAtUtc.AddDays(1),
            modifiedAtUtc,
            UserId);

        Assert.Equal(
            WorkOrderType.Emergency,
            workOrder.Type);

        Assert.Equal(
            WorkOrderPriority.Emergency,
            workOrder.Priority);

        Assert.Equal(
            "No heat",
            workOrder.Summary);

        Assert.Equal(
            "Furnace will not ignite.",
            workOrder.Description);

        Assert.Equal(
            CreatedAtUtc.AddDays(1),
            workOrder.RequestedForUtc);

        Assert.Equal(
            modifiedAtUtc,
            workOrder.LastModifiedAtUtc);

        Assert.Equal(
            UserId,
            workOrder.LastModifiedByUserId);
    }

    [Fact]
    public void ChangeLocation_ChangesLocationAndEquipment()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        Guid newLocationId =
            Guid.Parse(
                "88888888-8888-8888-8888-888888888888");

        Guid newEquipmentId =
            Guid.Parse(
                "99999999-9999-9999-9999-999999999999");

        workOrder.ChangeLocation(
            newLocationId,
            newEquipmentId,
            CreatedAtUtc.AddHours(1),
            UserId);

        Assert.Equal(
            newLocationId,
            workOrder.CustomerLocationId);

        Assert.Equal(
            newEquipmentId,
            workOrder.EquipmentId);
    }

    [Fact]
    public void SetEquipment_CanAddAndRemoveEquipment()
    {
        WorkOrder workOrder =
            WorkOrder.Create(
                WorkOrderId,
                CompanyId,
                CustomerId,
                LocationId,
                null,
                "WO-1001",
                WorkOrderType.ServiceCall,
                WorkOrderPriority.Normal,
                "No cooling",
                null,
                null,
                CreatedAtUtc,
                UserId);

        workOrder.SetEquipment(
            EquipmentId,
            CreatedAtUtc.AddHours(1),
            UserId);

        Assert.Equal(
            EquipmentId,
            workOrder.EquipmentId);

        workOrder.SetEquipment(
            null,
            CreatedAtUtc.AddHours(2),
            UserId);

        Assert.Null(workOrder.EquipmentId);
    }

    [Fact]
    public void Open_FromDraft_SetsStatusToOpen()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        workOrder.Open(
            CreatedAtUtc.AddHours(1),
            UserId);

        Assert.Equal(
            WorkOrderStatus.Open,
            workOrder.Status);
    }

    [Fact]
    public void Open_WhenNotDraft_ThrowsInvalidOperationException()
    {
        WorkOrder workOrder =
            CreateOpenWorkOrder();

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => workOrder.Open(
                    CreatedAtUtc.AddHours(2),
                    UserId));

        Assert.Equal(
            "Only a draft work order can be opened.",
            exception.Message);
    }

    [Fact]
    public void Schedule_WithValidValues_SchedulesWorkOrder()
    {
        WorkOrder workOrder =
            CreateOpenWorkOrder();

        DateTimeOffset scheduledStartUtc =
            CreatedAtUtc.AddDays(1);

        DateTimeOffset scheduledEndUtc =
            scheduledStartUtc.AddHours(2);

        workOrder.Schedule(
            scheduledStartUtc,
            scheduledEndUtc,
            TechnicianId,
            CreatedAtUtc.AddHours(2),
            UserId);

        Assert.Equal(
            WorkOrderStatus.Scheduled,
            workOrder.Status);

        Assert.Equal(
            scheduledStartUtc,
            workOrder.ScheduledStartUtc);

        Assert.Equal(
            scheduledEndUtc,
            workOrder.ScheduledEndUtc);

        Assert.Equal(
            TechnicianId,
            workOrder.AssignedTechnicianUserId);
    }

    [Fact]
    public void Schedule_WithEndBeforeStart_ThrowsArgumentException()
    {
        WorkOrder workOrder =
            CreateOpenWorkOrder();

        DateTimeOffset scheduledStartUtc =
            CreatedAtUtc.AddDays(1);

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => workOrder.Schedule(
                    scheduledStartUtc,
                    scheduledStartUtc.AddMinutes(-1),
                    TechnicianId,
                    CreatedAtUtc.AddHours(2),
                    UserId));

        Assert.Equal(
            "scheduledEndUtc",
            exception.ParamName);
    }

    [Fact]
    public void AssignTechnician_AssignsTechnician()
    {
        WorkOrder workOrder =
            CreateOpenWorkOrder();

        workOrder.AssignTechnician(
            TechnicianId,
            CreatedAtUtc.AddHours(2),
            UserId);

        Assert.Equal(
            TechnicianId,
            workOrder.AssignedTechnicianUserId);
    }

    [Fact]
    public void UnassignTechnician_RemovesTechnician()
    {
        WorkOrder workOrder =
            CreateOpenWorkOrder();

        workOrder.AssignTechnician(
            TechnicianId,
            CreatedAtUtc.AddHours(2),
            UserId);

        workOrder.UnassignTechnician(
            CreatedAtUtc.AddHours(3),
            UserId);

        Assert.Null(
            workOrder.AssignedTechnicianUserId);
    }

    [Fact]
    public void Dispatch_WithAssignedTechnician_DispatchesWorkOrder()
    {
        WorkOrder workOrder =
            CreateScheduledWorkOrder();

        DateTimeOffset dispatchedAtUtc =
            CreatedAtUtc.AddHours(3);

        workOrder.Dispatch(
            dispatchedAtUtc,
            UserId);

        Assert.Equal(
            WorkOrderStatus.Dispatched,
            workOrder.Status);

        Assert.Equal(
            dispatchedAtUtc,
            workOrder.DispatchedAtUtc);
    }

    [Fact]
    public void Dispatch_WithoutAssignedTechnician_ThrowsInvalidOperationException()
    {
        WorkOrder workOrder =
            CreateOpenWorkOrder();

        workOrder.Schedule(
            CreatedAtUtc.AddDays(1),
            CreatedAtUtc.AddDays(1).AddHours(2),
            null,
            CreatedAtUtc.AddHours(2),
            UserId);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => workOrder.Dispatch(
                    CreatedAtUtc.AddHours(3),
                    UserId));

        Assert.Equal(
            "A technician must be assigned before dispatch.",
            exception.Message);
    }

    [Fact]
    public void Start_FromDispatched_StartsWorkOrder()
    {
        WorkOrder workOrder =
            CreateDispatchedWorkOrder();

        DateTimeOffset startedAtUtc =
            CreatedAtUtc.AddHours(4);

        workOrder.Start(
            startedAtUtc,
            UserId);

        Assert.Equal(
            WorkOrderStatus.InProgress,
            workOrder.Status);

        Assert.Equal(
            startedAtUtc,
            workOrder.StartedAtUtc);
    }

    [Fact]
    public void Start_WithoutAssignedTechnician_ThrowsInvalidOperationException()
    {
        WorkOrder workOrder =
            CreateOpenWorkOrder();

        workOrder.Schedule(
            CreatedAtUtc.AddDays(1),
            CreatedAtUtc.AddDays(1).AddHours(2),
            null,
            CreatedAtUtc.AddHours(2),
            UserId);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => workOrder.Start(
                    CreatedAtUtc.AddHours(3),
                    UserId));

        Assert.Equal(
            "A technician must be assigned before work can begin.",
            exception.Message);
    }

    [Fact]
    public void PlaceOnHold_FromInProgress_SetsStatusToOnHold()
    {
        WorkOrder workOrder =
            CreateInProgressWorkOrder();

        workOrder.PlaceOnHold(
            CreatedAtUtc.AddHours(5),
            UserId);

        Assert.Equal(
            WorkOrderStatus.OnHold,
            workOrder.Status);
    }

    [Fact]
    public void Start_FromOnHold_ResumesWorkWithoutReplacingOriginalStartTime()
    {
        WorkOrder workOrder =
            CreateInProgressWorkOrder();

        DateTimeOffset originalStartedAtUtc =
            workOrder.StartedAtUtc!.Value;

        workOrder.PlaceOnHold(
            CreatedAtUtc.AddHours(5),
            UserId);

        workOrder.Start(
            CreatedAtUtc.AddHours(6),
            UserId);

        Assert.Equal(
            WorkOrderStatus.InProgress,
            workOrder.Status);

        Assert.Equal(
            originalStartedAtUtc,
            workOrder.StartedAtUtc);
    }

    [Fact]
    public void Complete_FromInProgress_CompletesWorkOrder()
    {
        WorkOrder workOrder =
            CreateInProgressWorkOrder();

        DateTimeOffset completedAtUtc =
            CreatedAtUtc.AddHours(5);

        workOrder.Complete(
            completedAtUtc,
            UserId);

        Assert.Equal(
            WorkOrderStatus.Completed,
            workOrder.Status);

        Assert.Equal(
            completedAtUtc,
            workOrder.CompletedAtUtc);

        Assert.Equal(
            completedAtUtc,
            workOrder.LastModifiedAtUtc);
    }

    [Fact]
    public void Complete_WhenNotInProgress_ThrowsInvalidOperationException()
    {
        WorkOrder workOrder =
            CreateOpenWorkOrder();

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => workOrder.Complete(
                    CreatedAtUtc.AddHours(2),
                    UserId));

        Assert.Equal(
            "Only an in-progress work order can be completed.",
            exception.Message);
    }

    [Fact]
    public void Cancel_WithValidReason_CancelsWorkOrder()
    {
        WorkOrder workOrder =
            CreateOpenWorkOrder();

        DateTimeOffset cancelledAtUtc =
            CreatedAtUtc.AddHours(2);

        workOrder.Cancel(
            "  Customer no longer needs service.  ",
            cancelledAtUtc,
            UserId);

        Assert.Equal(
            WorkOrderStatus.Cancelled,
            workOrder.Status);

        Assert.Equal(
            cancelledAtUtc,
            workOrder.CancelledAtUtc);

        Assert.Equal(
            "Customer no longer needs service.",
            workOrder.CancellationReason);
    }

    [Fact]
    public void Cancel_WithBlankReason_ThrowsArgumentException()
    {
        WorkOrder workOrder =
            CreateOpenWorkOrder();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => workOrder.Cancel(
                    " ",
                    CreatedAtUtc.AddHours(2),
                    UserId));

        Assert.Equal(
            "cancellationReason",
            exception.ParamName);
    }

    [Fact]
    public void UpdateDetails_WhenCompleted_ThrowsInvalidOperationException()
    {
        WorkOrder workOrder =
            CreateInProgressWorkOrder();

        workOrder.Complete(
            CreatedAtUtc.AddHours(5),
            UserId);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => workOrder.UpdateDetails(
                    WorkOrderType.ServiceCall,
                    WorkOrderPriority.Low,
                    "Updated summary",
                    null,
                    null,
                    CreatedAtUtc.AddHours(6),
                    UserId));

        Assert.Equal(
            "A completed or cancelled work order cannot be changed.",
            exception.Message);
    }

    [Fact]
    public void ChangeLocation_WhenCancelled_ThrowsInvalidOperationException()
    {
        WorkOrder workOrder =
            CreateOpenWorkOrder();

        workOrder.Cancel(
            "Customer cancelled.",
            CreatedAtUtc.AddHours(2),
            UserId);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => workOrder.ChangeLocation(
                    LocationId,
                    null,
                    CreatedAtUtc.AddHours(3),
                    UserId));

        Assert.Equal(
            "A completed or cancelled work order cannot be changed.",
            exception.Message);
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

    private static WorkOrder CreateOpenWorkOrder()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        workOrder.Open(
            CreatedAtUtc.AddHours(1),
            UserId);

        return workOrder;
    }

    private static WorkOrder CreateScheduledWorkOrder()
    {
        WorkOrder workOrder =
            CreateOpenWorkOrder();

        workOrder.Schedule(
            CreatedAtUtc.AddDays(1),
            CreatedAtUtc.AddDays(1).AddHours(2),
            TechnicianId,
            CreatedAtUtc.AddHours(2),
            UserId);

        return workOrder;
    }

    private static WorkOrder CreateDispatchedWorkOrder()
    {
        WorkOrder workOrder =
            CreateScheduledWorkOrder();

        workOrder.Dispatch(
            CreatedAtUtc.AddHours(3),
            UserId);

        return workOrder;
    }

    private static WorkOrder CreateInProgressWorkOrder()
    {
        WorkOrder workOrder =
            CreateDispatchedWorkOrder();

        workOrder.Start(
            CreatedAtUtc.AddHours(4),
            UserId);

        return workOrder;
    }
}
