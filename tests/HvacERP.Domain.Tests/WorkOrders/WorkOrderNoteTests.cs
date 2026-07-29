using HvacERP.Domain.WorkOrders;

namespace HvacERP.Domain.Tests.WorkOrders;

public sealed class WorkOrderNoteTests
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

    private static readonly Guid SecondUserId =
        Guid.Parse(
            "66666666-6666-6666-6666-666666666667");

    private static readonly Guid TechnicianId =
        Guid.Parse(
            "77777777-7777-7777-7777-777777777777");

    private static readonly Guid NoteId =
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
    public void AddNote_WithValidValues_AddsNote()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        WorkOrderNote note =
            workOrder.AddNote(
                NoteId,
                WorkOrderNoteType.Technician,
                "  Replaced failed capacitor.  ",
                true,
                CreatedAtUtc.AddHours(1),
                UserId);

        Assert.Single(
            workOrder.Notes);

        Assert.Single(
            workOrder.ActiveNotes);

        Assert.Equal(
            NoteId,
            note.Id);

        Assert.Equal(
            WorkOrderNoteType.Technician,
            note.Type);

        Assert.Equal(
            "Replaced failed capacitor.",
            note.Content);

        Assert.True(
            note.IsPinned);

        Assert.Equal(
            UserId,
            note.CreatedByUserId);

        Assert.Equal(
            CreatedAtUtc.AddHours(1),
            note.CreatedAtUtc);

        Assert.False(
            note.IsDeleted);

        Assert.False(
            note.IsCustomerVisible);

        Assert.Null(
            note.LastModifiedByUserId);

        Assert.Null(
            note.LastModifiedAtUtc);
    }

    [Fact]
    public void AddNote_WithCustomerVisibleType_IsCustomerVisible()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        WorkOrderNote note =
            workOrder.AddNote(
                NoteId,
                WorkOrderNoteType.CustomerVisible,
                "System is operating normally.",
                false,
                CreatedAtUtc.AddHours(1),
                UserId);

        Assert.True(
            note.IsCustomerVisible);

        Assert.Single(
            workOrder.CustomerVisibleNotes);
    }

    [Fact]
    public void AddNote_WithInternalType_IsNotCustomerVisible()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        workOrder.AddNote(
            NoteId,
            WorkOrderNoteType.Internal,
            "Review account before billing.",
            false,
            CreatedAtUtc.AddHours(1),
            UserId);

        Assert.Empty(
            workOrder.CustomerVisibleNotes);
    }

    [Fact]
    public void AddNote_WithDuplicateId_ThrowsInvalidOperationException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        AddStandardNote(
            workOrder);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => AddStandardNote(
                    workOrder));

        Assert.Equal(
            "A work order note with the same ID already exists.",
            exception.Message);
    }

    [Fact]
    public void AddNote_WithEmptyId_ThrowsArgumentException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => workOrder.AddNote(
                    Guid.Empty,
                    WorkOrderNoteType.Internal,
                    "Valid content.",
                    false,
                    CreatedAtUtc.AddHours(1),
                    UserId));

        Assert.Equal(
            "noteId",
            exception.ParamName);
    }

    [Fact]
    public void AddNote_WithEmptyAuthorId_ThrowsArgumentException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => workOrder.AddNote(
                    NoteId,
                    WorkOrderNoteType.Internal,
                    "Valid content.",
                    false,
                    CreatedAtUtc.AddHours(1),
                    Guid.Empty));

        Assert.Equal(
            "createdByUserId",
            exception.ParamName);
    }

    [Fact]
    public void AddNote_WithInvalidType_ThrowsArgumentOutOfRangeException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => workOrder.AddNote(
                NoteId,
                (WorkOrderNoteType)500,
                "Valid content.",
                false,
                CreatedAtUtc.AddHours(1),
                UserId));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void AddNote_WithBlankContent_ThrowsArgumentException(
        string content)
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => workOrder.AddNote(
                    NoteId,
                    WorkOrderNoteType.Internal,
                    content,
                    false,
                    CreatedAtUtc.AddHours(1),
                    UserId));

        Assert.Equal(
            "content",
            exception.ParamName);
    }

    [Fact]
    public void AddNote_WithContentOverMaximumLength_ThrowsArgumentException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => workOrder.AddNote(
                    NoteId,
                    WorkOrderNoteType.Internal,
                    new string('A', 10_001),
                    false,
                    CreatedAtUtc.AddHours(1),
                    UserId));

        Assert.Equal(
            "content",
            exception.ParamName);
    }

    [Fact]
    public void UpdateNote_WithValidValues_UpdatesEditableFields()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        AddStandardNote(
            workOrder);

        DateTimeOffset modifiedAtUtc =
            CreatedAtUtc.AddHours(2);

        workOrder.UpdateNote(
            NoteId,
            WorkOrderNoteType.CustomerVisible,
            "  Repairs completed successfully.  ",
            true,
            modifiedAtUtc,
            SecondUserId);

        WorkOrderNote note =
            Assert.Single(
                workOrder.Notes);

        Assert.Equal(
            WorkOrderNoteType.CustomerVisible,
            note.Type);

        Assert.Equal(
            "Repairs completed successfully.",
            note.Content);

        Assert.True(
            note.IsPinned);

        Assert.True(
            note.IsCustomerVisible);

        Assert.Equal(
            SecondUserId,
            note.LastModifiedByUserId);

        Assert.Equal(
            modifiedAtUtc,
            note.LastModifiedAtUtc);
    }

    [Fact]
    public void UpdateNote_PreservesOriginalCreationInformation()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        AddStandardNote(
            workOrder);

        workOrder.UpdateNote(
            NoteId,
            WorkOrderNoteType.Billing,
            "Updated note.",
            false,
            CreatedAtUtc.AddHours(2),
            SecondUserId);

        WorkOrderNote note =
            Assert.Single(
                workOrder.Notes);

        Assert.Equal(
            UserId,
            note.CreatedByUserId);

        Assert.Equal(
            CreatedAtUtc.AddHours(1),
            note.CreatedAtUtc);
    }

    [Fact]
    public void UpdateNote_WhenNoteDoesNotExist_ThrowsInvalidOperationException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => workOrder.UpdateNote(
                    NoteId,
                    WorkOrderNoteType.Internal,
                    "Updated content.",
                    false,
                    CreatedAtUtc.AddHours(2),
                    UserId));

        Assert.Equal(
            "The work order note does not exist.",
            exception.Message);
    }

    [Fact]
    public void UpdateNote_BeforeCreationTime_ThrowsArgumentException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        AddStandardNote(
            workOrder);

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => workOrder.UpdateNote(
                    NoteId,
                    WorkOrderNoteType.Internal,
                    "Updated content.",
                    false,
                    CreatedAtUtc,
                    UserId));

        Assert.Equal(
            "modifiedAtUtc",
            exception.ParamName);
    }

    [Fact]
    public void DeleteNote_WithValidValues_SoftDeletesNote()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        AddStandardNote(
            workOrder);

        DateTimeOffset deletedAtUtc =
            CreatedAtUtc.AddHours(2);

        workOrder.DeleteNote(
            NoteId,
            "  Entered on the wrong work order.  ",
            deletedAtUtc,
            SecondUserId);

        WorkOrderNote note =
            Assert.Single(
                workOrder.Notes);

        Assert.True(
            note.IsDeleted);

        Assert.False(
            note.IsPinned);

        Assert.Equal(
            SecondUserId,
            note.DeletedByUserId);

        Assert.Equal(
            deletedAtUtc,
            note.DeletedAtUtc);

        Assert.Equal(
            "Entered on the wrong work order.",
            note.DeletionReason);

        Assert.Empty(
            workOrder.ActiveNotes);
    }

    [Fact]
    public void DeleteNote_PreservesNoteForAuditHistory()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        AddStandardNote(
            workOrder);

        workOrder.DeleteNote(
            NoteId,
            "Duplicate note.",
            CreatedAtUtc.AddHours(2),
            UserId);

        Assert.Single(
            workOrder.Notes);

        Assert.Empty(
            workOrder.ActiveNotes);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void DeleteNote_WithBlankReason_ThrowsArgumentException(
        string deletionReason)
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        AddStandardNote(
            workOrder);

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => workOrder.DeleteNote(
                    NoteId,
                    deletionReason,
                    CreatedAtUtc.AddHours(2),
                    UserId));

        Assert.Equal(
            "deletionReason",
            exception.ParamName);
    }

    [Fact]
    public void DeleteNote_WithReasonOverMaximumLength_ThrowsArgumentException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        AddStandardNote(
            workOrder);

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => workOrder.DeleteNote(
                    NoteId,
                    new string('A', 501),
                    CreatedAtUtc.AddHours(2),
                    UserId));

        Assert.Equal(
            "deletionReason",
            exception.ParamName);
    }

    [Fact]
    public void DeleteNote_WhenAlreadyDeleted_ThrowsInvalidOperationException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        AddStandardNote(
            workOrder);

        workOrder.DeleteNote(
            NoteId,
            "Duplicate note.",
            CreatedAtUtc.AddHours(2),
            UserId);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => workOrder.DeleteNote(
                    NoteId,
                    "Delete again.",
                    CreatedAtUtc.AddHours(3),
                    UserId));

        Assert.Equal(
            "A deleted work order note cannot be changed.",
            exception.Message);
    }

    [Fact]
    public void UpdateNote_WhenDeleted_ThrowsInvalidOperationException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        AddStandardNote(
            workOrder);

        workOrder.DeleteNote(
            NoteId,
            "Duplicate note.",
            CreatedAtUtc.AddHours(2),
            UserId);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => workOrder.UpdateNote(
                    NoteId,
                    WorkOrderNoteType.Internal,
                    "Attempted update.",
                    false,
                    CreatedAtUtc.AddHours(3),
                    UserId));

        Assert.Equal(
            "A deleted work order note cannot be changed.",
            exception.Message);
    }

    [Fact]
    public void CustomerVisibleNotes_ExcludesDeletedNotes()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        workOrder.AddNote(
            NoteId,
            WorkOrderNoteType.CustomerVisible,
            "Repair completed.",
            false,
            CreatedAtUtc.AddHours(1),
            UserId);

        workOrder.DeleteNote(
            NoteId,
            "Incorrect note.",
            CreatedAtUtc.AddHours(2),
            UserId);

        Assert.Empty(
            workOrder.CustomerVisibleNotes);
    }

    [Fact]
    public void ActiveNotes_OrdersPinnedNotesFirst()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        Guid olderPinnedNoteId =
            Guid.Parse(
                "88888888-8888-8888-8888-888888888881");

        Guid newerUnpinnedNoteId =
            Guid.Parse(
                "88888888-8888-8888-8888-888888888882");

        workOrder.AddNote(
            olderPinnedNoteId,
            WorkOrderNoteType.Internal,
            "Pinned note.",
            true,
            CreatedAtUtc.AddHours(1),
            UserId);

        workOrder.AddNote(
            newerUnpinnedNoteId,
            WorkOrderNoteType.Internal,
            "Newer note.",
            false,
            CreatedAtUtc.AddHours(2),
            UserId);

        Assert.Equal(
            olderPinnedNoteId,
            workOrder.ActiveNotes.First().Id);
    }

    [Fact]
    public void AddNote_UpdatesWorkOrderAuditInformation()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        DateTimeOffset noteCreatedAtUtc =
            CreatedAtUtc.AddHours(1);

        workOrder.AddNote(
            NoteId,
            WorkOrderNoteType.Internal,
            "Audit test.",
            false,
            noteCreatedAtUtc,
            SecondUserId);

        Assert.Equal(
            noteCreatedAtUtc,
            workOrder.LastModifiedAtUtc);

        Assert.Equal(
            SecondUserId,
            workOrder.LastModifiedByUserId);
    }

    [Fact]
    public void UpdateNote_UpdatesWorkOrderAuditInformation()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        AddStandardNote(
            workOrder);

        DateTimeOffset modifiedAtUtc =
            CreatedAtUtc.AddHours(2);

        workOrder.UpdateNote(
            NoteId,
            WorkOrderNoteType.Internal,
            "Updated note.",
            false,
            modifiedAtUtc,
            SecondUserId);

        Assert.Equal(
            modifiedAtUtc,
            workOrder.LastModifiedAtUtc);

        Assert.Equal(
            SecondUserId,
            workOrder.LastModifiedByUserId);
    }

    [Fact]
    public void DeleteNote_UpdatesWorkOrderAuditInformation()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        AddStandardNote(
            workOrder);

        DateTimeOffset deletedAtUtc =
            CreatedAtUtc.AddHours(2);

        workOrder.DeleteNote(
            NoteId,
            "Duplicate.",
            deletedAtUtc,
            SecondUserId);

        Assert.Equal(
            deletedAtUtc,
            workOrder.LastModifiedAtUtc);

        Assert.Equal(
            SecondUserId,
            workOrder.LastModifiedByUserId);
    }

    [Fact]
    public void AddNote_WhenWorkOrderCompleted_ThrowsInvalidOperationException()
    {
        WorkOrder workOrder =
            CreateCompletedWorkOrder();

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => AddStandardNote(
                    workOrder));

        Assert.Equal(
            "A completed or cancelled work order cannot be changed.",
            exception.Message);
    }

    [Fact]
    public void UpdateNote_WhenWorkOrderCancelled_ThrowsInvalidOperationException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        AddStandardNote(
            workOrder);

        workOrder.Cancel(
            "Customer cancelled.",
            CreatedAtUtc.AddHours(2),
            UserId);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => workOrder.UpdateNote(
                    NoteId,
                    WorkOrderNoteType.Internal,
                    "Updated note.",
                    false,
                    CreatedAtUtc.AddHours(3),
                    UserId));

        Assert.Equal(
            "A completed or cancelled work order cannot be changed.",
            exception.Message);
    }

    [Fact]
    public void DeleteNote_WhenWorkOrderCompleted_ThrowsInvalidOperationException()
    {
        WorkOrder workOrder =
            CreateWorkOrder();

        AddStandardNote(
            workOrder);

        CompleteWorkOrder(
            workOrder);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => workOrder.DeleteNote(
                    NoteId,
                    "Attempted deletion.",
                    CreatedAtUtc.AddHours(8),
                    UserId));

        Assert.Equal(
            "A completed or cancelled work order cannot be changed.",
            exception.Message);
    }

    private static void AddStandardNote(
        WorkOrder workOrder)
    {
        workOrder.AddNote(
            NoteId,
            WorkOrderNoteType.Internal,
            "Office staff should review this work order.",
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
