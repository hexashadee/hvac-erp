using NullSupportERP.Domain.WorkOrders;

namespace NullSupportERP.Domain.Tests.WorkOrders;

public sealed class WorkOrderLineItemTests
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

    private static readonly Guid LineItemId =
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
    public void AddLineItem_WithValidValues_AddsLineItem()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        WorkOrderLineItem lineItem =
            workOrder.AddLineItem(
                LineItemId,
                WorkOrderLineItemType.Part,
                "  CAP-45-5  ",
                "  Dual run capacitor  ",
                2,
                35.50m,
                true,
                CreatedAtUtc.AddHours(1),
                UserId);

        Assert.Single(
            workOrder.LineItems);

        Assert.Equal(
            LineItemId,
            lineItem.Id);

        Assert.Equal(
            WorkOrderLineItemType.Part,
            lineItem.Type);

        Assert.Equal(
            "CAP-45-5",
            lineItem.ItemNumber);

        Assert.Equal(
            "Dual run capacitor",
            lineItem.Description);

        Assert.Equal(
            2,
            lineItem.Quantity);

        Assert.Equal(
            35.50m,
            lineItem.UnitPrice);

        Assert.True(
            lineItem.IsTaxable);

        Assert.Equal(
            71.00m,
            lineItem.LineTotal);
    }

    [Fact]
    public void AddLineItem_WithBlankItemNumber_StoresNull()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        WorkOrderLineItem lineItem =
            workOrder.AddLineItem(
                LineItemId,
                WorkOrderLineItemType.Service,
                " ",
                "Diagnostic service",
                1,
                99m,
                false,
                CreatedAtUtc.AddHours(1),
                UserId);

        Assert.Null(
            lineItem.ItemNumber);
    }

    [Fact]
    public void AddLineItem_WithFractionalQuantity_CalculatesRoundedTotal()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        WorkOrderLineItem lineItem =
            workOrder.AddLineItem(
                LineItemId,
                WorkOrderLineItemType.Material,
                "REF-410A",
                "R-410A refrigerant",
                1.25m,
                18.999m,
                true,
                CreatedAtUtc.AddHours(1),
                UserId);

        Assert.Equal(
            23.75m,
            lineItem.LineTotal);
    }

    [Fact]
    public void AddLineItem_WithDuplicateId_ThrowsInvalidOperationException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        AddStandardLineItem(
            workOrder);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => AddStandardLineItem(
                    workOrder));

        Assert.Equal(
            "A work order line item with the same ID already exists.",
            exception.Message);
    }

    [Fact]
    public void AddLineItem_WithEmptyId_ThrowsArgumentException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => workOrder.AddLineItem(
                    Guid.Empty,
                    WorkOrderLineItemType.Part,
                    "CAP-45-5",
                    "Dual run capacitor",
                    1,
                    35.50m,
                    true,
                    CreatedAtUtc.AddHours(1),
                    UserId));

        Assert.Equal(
            "lineItemId",
            exception.ParamName);
    }

    [Fact]
    public void AddLineItem_WithInvalidType_ThrowsArgumentOutOfRangeException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => workOrder.AddLineItem(
                LineItemId,
                (WorkOrderLineItemType)500,
                null,
                "Invalid item",
                1,
                10m,
                false,
                CreatedAtUtc.AddHours(1),
                UserId));
    }

    [Fact]
    public void AddLineItem_WithBlankDescription_ThrowsArgumentException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => workOrder.AddLineItem(
                    LineItemId,
                    WorkOrderLineItemType.Part,
                    "CAP-45-5",
                    " ",
                    1,
                    35.50m,
                    true,
                    CreatedAtUtc.AddHours(1),
                    UserId));

        Assert.Equal(
            "description",
            exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddLineItem_WithNonPositiveQuantity_ThrowsArgumentOutOfRangeException(
        decimal quantity)
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        ArgumentOutOfRangeException exception =
            Assert.Throws<ArgumentOutOfRangeException>(
                () => workOrder.AddLineItem(
                    LineItemId,
                    WorkOrderLineItemType.Part,
                    "CAP-45-5",
                    "Dual run capacitor",
                    quantity,
                    35.50m,
                    true,
                    CreatedAtUtc.AddHours(1),
                    UserId));

        Assert.Equal(
            "quantity",
            exception.ParamName);
    }

    [Fact]
    public void AddLineItem_WithNegativePartPrice_ThrowsArgumentOutOfRangeException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        ArgumentOutOfRangeException exception =
            Assert.Throws<ArgumentOutOfRangeException>(
                () => workOrder.AddLineItem(
                    LineItemId,
                    WorkOrderLineItemType.Part,
                    "CAP-45-5",
                    "Dual run capacitor",
                    1,
                    -35.50m,
                    true,
                    CreatedAtUtc.AddHours(1),
                    UserId));

        Assert.Equal(
            "unitPrice",
            exception.ParamName);
    }

    [Fact]
    public void AddLineItem_WithPositiveDiscountPrice_ThrowsArgumentOutOfRangeException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        ArgumentOutOfRangeException exception =
            Assert.Throws<ArgumentOutOfRangeException>(
                () => workOrder.AddLineItem(
                    LineItemId,
                    WorkOrderLineItemType.Discount,
                    null,
                    "Customer loyalty discount",
                    1,
                    25m,
                    false,
                    CreatedAtUtc.AddHours(1),
                    UserId));

        Assert.Equal(
            "unitPrice",
            exception.ParamName);
    }

    [Fact]
    public void AddLineItem_WithNegativeDiscountPrice_AddsDiscount()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        WorkOrderLineItem lineItem =
            workOrder.AddLineItem(
                LineItemId,
                WorkOrderLineItemType.Discount,
                null,
                "Customer loyalty discount",
                1,
                -25m,
                false,
                CreatedAtUtc.AddHours(1),
                UserId);

        Assert.Equal(
            -25m,
            lineItem.LineTotal);
    }

    [Fact]
    public void UpdateLineItem_WithValidValues_UpdatesLineItem()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        AddStandardLineItem(
            workOrder);

        workOrder.UpdateLineItem(
            LineItemId,
            WorkOrderLineItemType.Material,
            "  WIRE-18-5  ",
            "  Thermostat wire  ",
            25,
            0.75m,
            false,
            CreatedAtUtc.AddHours(2),
            UserId);

        WorkOrderLineItem lineItem =
            Assert.Single(
                workOrder.LineItems);

        Assert.Equal(
            WorkOrderLineItemType.Material,
            lineItem.Type);

        Assert.Equal(
            "WIRE-18-5",
            lineItem.ItemNumber);

        Assert.Equal(
            "Thermostat wire",
            lineItem.Description);

        Assert.Equal(
            25,
            lineItem.Quantity);

        Assert.Equal(
            0.75m,
            lineItem.UnitPrice);

        Assert.False(
            lineItem.IsTaxable);

        Assert.Equal(
            18.75m,
            lineItem.LineTotal);
    }

    [Fact]
    public void UpdateLineItem_WhenLineItemDoesNotExist_ThrowsInvalidOperationException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => workOrder.UpdateLineItem(
                    LineItemId,
                    WorkOrderLineItemType.Part,
                    null,
                    "Dual run capacitor",
                    1,
                    35.50m,
                    true,
                    CreatedAtUtc.AddHours(1),
                    UserId));

        Assert.Equal(
            "The work order line item does not exist.",
            exception.Message);
    }

    [Fact]
    public void RemoveLineItem_WhenLineItemExists_RemovesLineItem()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        AddStandardLineItem(
            workOrder);

        workOrder.RemoveLineItem(
            LineItemId,
            CreatedAtUtc.AddHours(2),
            UserId);

        Assert.Empty(
            workOrder.LineItems);
    }

    [Fact]
    public void RemoveLineItem_WhenLineItemDoesNotExist_ThrowsInvalidOperationException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => workOrder.RemoveLineItem(
                    LineItemId,
                    CreatedAtUtc.AddHours(1),
                    UserId));

        Assert.Equal(
            "The work order line item does not exist.",
            exception.Message);
    }

    [Fact]
    public void LineItemSubtotal_WithMultipleItems_ReturnsCombinedTotal()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        workOrder.AddLineItem(
            Guid.Parse(
                "88888888-8888-8888-8888-888888888881"),
            WorkOrderLineItemType.Part,
            "CAP-45-5",
            "Dual run capacitor",
            2,
            35.50m,
            true,
            CreatedAtUtc.AddHours(1),
            UserId);

        workOrder.AddLineItem(
            Guid.Parse(
                "88888888-8888-8888-8888-888888888882"),
            WorkOrderLineItemType.Service,
            null,
            "Diagnostic service",
            1,
            99m,
            false,
            CreatedAtUtc.AddHours(2),
            UserId);

        workOrder.AddLineItem(
            Guid.Parse(
                "88888888-8888-8888-8888-888888888883"),
            WorkOrderLineItemType.Discount,
            null,
            "Customer discount",
            1,
            -20m,
            false,
            CreatedAtUtc.AddHours(3),
            UserId);

        Assert.Equal(
            150m,
            workOrder.LineItemSubtotal);
    }

    [Fact]
    public void TaxableLineItemSubtotal_ReturnsOnlyTaxableItems()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        workOrder.AddLineItem(
            Guid.Parse(
                "88888888-8888-8888-8888-888888888881"),
            WorkOrderLineItemType.Part,
            "CAP-45-5",
            "Dual run capacitor",
            2,
            35.50m,
            true,
            CreatedAtUtc.AddHours(1),
            UserId);

        workOrder.AddLineItem(
            Guid.Parse(
                "88888888-8888-8888-8888-888888888882"),
            WorkOrderLineItemType.Service,
            null,
            "Diagnostic service",
            1,
            99m,
            false,
            CreatedAtUtc.AddHours(2),
            UserId);

        Assert.Equal(
            71m,
            workOrder.TaxableLineItemSubtotal);
    }

    [Fact]
    public void AddLineItem_WhenWorkOrderCompleted_ThrowsInvalidOperationException()
    {
        WorkOrder workOrder =
            CreateCompletedWorkOrder();

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => AddStandardLineItem(
                    workOrder));

        Assert.Equal(
            "A completed or cancelled work order cannot be changed.",
            exception.Message);
    }

    [Fact]
    public void UpdateLineItem_WhenWorkOrderCancelled_ThrowsInvalidOperationException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        AddStandardLineItem(
            workOrder);

        workOrder.Cancel(
            "Customer cancelled.",
            CreatedAtUtc.AddHours(2),
            UserId);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => workOrder.UpdateLineItem(
                    LineItemId,
                    WorkOrderLineItemType.Part,
                    null,
                    "Updated item",
                    1,
                    10m,
                    true,
                    CreatedAtUtc.AddHours(3),
                    UserId));

        Assert.Equal(
            "A completed or cancelled work order cannot be changed.",
            exception.Message);
    }

    [Fact]
    public void RemoveLineItem_WhenWorkOrderCompleted_ThrowsInvalidOperationException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        AddStandardLineItem(
            workOrder);

        CompleteWorkOrder(
            workOrder);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => workOrder.RemoveLineItem(
                    LineItemId,
                    CreatedAtUtc.AddHours(7),
                    UserId));

        Assert.Equal(
            "A completed or cancelled work order cannot be changed.",
            exception.Message);
    }

    private static void AddStandardLineItem(
        WorkOrder workOrder)
    {
        workOrder.AddLineItem(
            LineItemId,
            WorkOrderLineItemType.Part,
            "CAP-45-5",
            "Dual run capacitor",
            1,
            35.50m,
            true,
            CreatedAtUtc.AddHours(1),
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
