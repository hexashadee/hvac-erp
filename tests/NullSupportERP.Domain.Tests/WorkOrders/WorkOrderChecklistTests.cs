using NullSupportERP.Domain.WorkOrders;

namespace NullSupportERP.Domain.Tests.WorkOrders;

public sealed class WorkOrderChecklistTests
{
    private static readonly Guid ChecklistId =
        Guid.Parse(
            "11111111-1111-1111-1111-111111111111");

    private static readonly Guid WorkOrderId =
        Guid.Parse(
            "22222222-2222-2222-2222-222222222222");

    private static readonly Guid UserId =
        Guid.Parse(
            "33333333-3333-3333-3333-333333333333");

    private static readonly Guid SecondUserId =
        Guid.Parse(
            "44444444-4444-4444-4444-444444444444");

    private static readonly Guid ItemId =
        Guid.Parse(
            "55555555-5555-5555-5555-555555555555");

    private static readonly Guid SecondItemId =
        Guid.Parse(
            "66666666-6666-6666-6666-666666666666");

    private static readonly Guid PhotoDocumentId =
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
    public void Create_WithValidValues_CreatesChecklist()
    {
        WorkOrderChecklist checklist =
            CreateChecklist();

        Assert.Equal(
            ChecklistId,
            checklist.Id);

        Assert.Equal(
            WorkOrderId,
            checklist.WorkOrderId);

        Assert.Equal(
            "Preventive Maintenance",
            checklist.Name);

        Assert.Equal(
            "Seasonal cooling inspection.",
            checklist.Description);

        Assert.Equal(
            WorkOrderChecklistStatus.InProgress,
            checklist.Status);

        Assert.Empty(
            checklist.Items);

        Assert.False(
            checklist.IsComplete);

        Assert.False(
            checklist.IsDeleted);
    }

    [Fact]
    public void Create_NormalizesText()
    {
        WorkOrderChecklist checklist =
            WorkOrderChecklist.Create(
                ChecklistId,
                WorkOrderId,
                "  Installation Startup  ",
                "  New equipment startup checklist.  ",
                CreatedAtUtc,
                UserId);

        Assert.Equal(
            "Installation Startup",
            checklist.Name);

        Assert.Equal(
            "New equipment startup checklist.",
            checklist.Description);
    }

    [Fact]
    public void Create_WithEmptyId_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => WorkOrderChecklist.Create(
                    Guid.Empty,
                    WorkOrderId,
                    "Checklist",
                    null,
                    CreatedAtUtc,
                    UserId));

        Assert.Equal(
            "id",
            exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithBlankName_ThrowsArgumentException(
        string name)
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => WorkOrderChecklist.Create(
                    ChecklistId,
                    WorkOrderId,
                    name,
                    null,
                    CreatedAtUtc,
                    UserId));

        Assert.Equal(
            "name",
            exception.ParamName);
    }

    [Fact]
    public void AddItem_WithValidValues_AddsItem()
    {
        WorkOrderChecklist checklist =
            CreateChecklist();

        WorkOrderChecklistItem item =
            checklist.AddItem(
                ItemId,
                "Electrical",
                "Measure run capacitor.",
                ChecklistItemResponseType.NumericReading,
                true,
                false,
                1,
                "µF",
                CreatedAtUtc.AddMinutes(1),
                UserId);

        Assert.Single(
            checklist.Items);

        Assert.Equal(
            ItemId,
            item.Id);

        Assert.Equal(
            ChecklistId,
            item.WorkOrderChecklistId);

        Assert.Equal(
            "Electrical",
            item.Category);

        Assert.Equal(
            ChecklistItemResult.NotCompleted,
            item.Result);

        Assert.Equal(
            "µF",
            item.Unit);

        Assert.Equal(
            1,
            checklist.TotalItemCount);
    }

    [Fact]
    public void AddItem_WithDuplicateId_ThrowsInvalidOperationException()
    {
        WorkOrderChecklist checklist =
            CreateChecklist();

        AddPassFailItem(
            checklist,
            ItemId,
            true,
            false,
            1);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => AddPassFailItem(
                    checklist,
                    ItemId,
                    false,
                    false,
                    2));

        Assert.Equal(
            "A checklist item with this ID already exists.",
            exception.Message);
    }

    [Fact]
    public void AddItem_WithUnitOnNonNumericItem_ThrowsArgumentException()
    {
        WorkOrderChecklist checklist =
            CreateChecklist();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => checklist.AddItem(
                    ItemId,
                    "Safety",
                    "Disconnect installed.",
                    ChecklistItemResponseType.Confirmation,
                    true,
                    false,
                    1,
                    "volts",
                    CreatedAtUtc.AddMinutes(1),
                    UserId));

        Assert.Equal(
            "unit",
            exception.ParamName);
    }

    [Fact]
    public void Items_AreOrderedByDisplayOrder()
    {
        WorkOrderChecklist checklist =
            CreateChecklist();

        AddPassFailItem(
            checklist,
            ItemId,
            true,
            false,
            20);

        AddPassFailItem(
            checklist,
            SecondItemId,
            true,
            false,
            10);

        Guid[] itemIds =
            checklist.Items
                .Select(item => item.Id)
                .ToArray();

        Assert.Equal(
            [
                SecondItemId,
                ItemId
            ],
            itemIds);
    }

    [Fact]
    public void RecordPassFail_WithPassedResult_CompletesItem()
    {
        WorkOrderChecklist checklist =
            CreateChecklist();

        AddPassFailItem(
            checklist,
            ItemId,
            true,
            false,
            1);

        DateTimeOffset completedAtUtc =
            CreatedAtUtc.AddMinutes(5);

        checklist.RecordPassFail(
            ItemId,
            ChecklistItemResult.Passed,
            "Operating normally.",
            null,
            completedAtUtc,
            SecondUserId);

        WorkOrderChecklistItem item =
            Assert.Single(
                checklist.Items);

        Assert.True(
            item.IsCompleted);

        Assert.Equal(
            ChecklistItemResult.Passed,
            item.Result);

        Assert.Equal(
            "Operating normally.",
            item.Notes);

        Assert.Equal(
            completedAtUtc,
            item.CompletedAtUtc);

        Assert.Equal(
            SecondUserId,
            item.CompletedByUserId);
    }

    [Fact]
    public void RecordPassFail_WithPhotoRequiredAndNoPhoto_ThrowsInvalidOperationException()
    {
        WorkOrderChecklist checklist =
            CreateChecklist();

        AddPassFailItem(
            checklist,
            ItemId,
            true,
            true,
            1);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => checklist.RecordPassFail(
                    ItemId,
                    ChecklistItemResult.Passed,
                    null,
                    null,
                    CreatedAtUtc.AddMinutes(5),
                    UserId));

        Assert.Equal(
            "This checklist item requires a photo.",
            exception.Message);
    }

    [Fact]
    public void RecordPassFail_WithRequiredPhoto_CompletesItem()
    {
        WorkOrderChecklist checklist =
            CreateChecklist();

        AddPassFailItem(
            checklist,
            ItemId,
            true,
            true,
            1);

        checklist.RecordPassFail(
            ItemId,
            ChecklistItemResult.Passed,
            null,
            PhotoDocumentId,
            CreatedAtUtc.AddMinutes(5),
            UserId);

        WorkOrderChecklistItem item =
            Assert.Single(
                checklist.Items);

        Assert.Equal(
            PhotoDocumentId,
            item.PhotoDocumentId);
    }

    [Fact]
    public void RecordNumericReading_StoresReadingAndUnit()
    {
        WorkOrderChecklist checklist =
            CreateChecklist();

        checklist.AddItem(
            ItemId,
            "Refrigeration",
            "Record suction pressure.",
            ChecklistItemResponseType.NumericReading,
            true,
            false,
            1,
            "PSI",
            CreatedAtUtc.AddMinutes(1),
            UserId);

        checklist.RecordNumericReading(
            ItemId,
            118.5m,
            ChecklistItemResult.Passed,
            "Within manufacturer range.",
            null,
            CreatedAtUtc.AddMinutes(5),
            UserId);

        WorkOrderChecklistItem item =
            Assert.Single(
                checklist.Items);

        Assert.Equal(
            118.5m,
            item.NumericReading);

        Assert.Equal(
            "PSI",
            item.Unit);

        Assert.Equal(
            ChecklistItemResult.Passed,
            item.Result);
    }

    [Fact]
    public void RecordText_StoresTextResponse()
    {
        WorkOrderChecklist checklist =
            CreateChecklist();

        checklist.AddItem(
            ItemId,
            "Diagnostics",
            "Describe the root cause.",
            ChecklistItemResponseType.Text,
            true,
            false,
            1,
            null,
            CreatedAtUtc.AddMinutes(1),
            UserId);

        checklist.RecordText(
            ItemId,
            "  Failed dual-run capacitor.  ",
            null,
            null,
            CreatedAtUtc.AddMinutes(5),
            UserId);

        WorkOrderChecklistItem item =
            Assert.Single(
                checklist.Items);

        Assert.Equal(
            "Failed dual-run capacitor.",
            item.ResponseText);

        Assert.Equal(
            ChecklistItemResult.Passed,
            item.Result);
    }

    [Fact]
    public void RecordConfirmation_WithFalseValue_MarksItemFailed()
    {
        WorkOrderChecklist checklist =
            CreateChecklist();

        checklist.AddItem(
            ItemId,
            "Safety",
            "Verify disconnect operation.",
            ChecklistItemResponseType.Confirmation,
            true,
            false,
            1,
            null,
            CreatedAtUtc.AddMinutes(1),
            UserId);

        checklist.RecordConfirmation(
            ItemId,
            false,
            "Disconnect would not open.",
            null,
            CreatedAtUtc.AddMinutes(5),
            UserId);

        WorkOrderChecklistItem item =
            Assert.Single(
                checklist.Items);

        Assert.Equal(
            ChecklistItemResult.Failed,
            item.Result);

        Assert.True(
            checklist.HasFailures);

        Assert.Equal(
            1,
            checklist.FailedItemCount);
    }

    [Fact]
    public void RecordWrongResponseType_ThrowsInvalidOperationException()
    {
        WorkOrderChecklist checklist =
            CreateChecklist();

        AddPassFailItem(
            checklist,
            ItemId,
            true,
            false,
            1);

        Assert.Throws<InvalidOperationException>(
            () => checklist.RecordText(
                ItemId,
                "Wrong response.",
                null,
                null,
                CreatedAtUtc.AddMinutes(5),
                UserId));
    }

    [Fact]
    public void ResetItem_ClearsResponse()
    {
        WorkOrderChecklist checklist =
            CreateChecklist();

        AddPassFailItem(
            checklist,
            ItemId,
            true,
            false,
            1);

        checklist.RecordPassFail(
            ItemId,
            ChecklistItemResult.Failed,
            "Failed.",
            null,
            CreatedAtUtc.AddMinutes(5),
            UserId);

        checklist.ResetItem(
            ItemId,
            CreatedAtUtc.AddMinutes(10),
            SecondUserId);

        WorkOrderChecklistItem item =
            Assert.Single(
                checklist.Items);

        Assert.Equal(
            ChecklistItemResult.NotCompleted,
            item.Result);

        Assert.False(
            item.IsCompleted);

        Assert.Null(
            item.Notes);

        Assert.Null(
            item.CompletedAtUtc);
    }

    [Fact]
    public void Complete_WithNoItems_ThrowsInvalidOperationException()
    {
        WorkOrderChecklist checklist =
            CreateChecklist();

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => checklist.Complete(
                    CreatedAtUtc.AddMinutes(5),
                    UserId));

        Assert.Equal(
            "A checklist must contain at least one item before it can be completed.",
            exception.Message);
    }

    [Fact]
    public void Complete_WithIncompleteRequiredItem_ThrowsInvalidOperationException()
    {
        WorkOrderChecklist checklist =
            CreateChecklist();

        AddPassFailItem(
            checklist,
            ItemId,
            true,
            false,
            1);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => checklist.Complete(
                    CreatedAtUtc.AddMinutes(5),
                    UserId));

        Assert.Equal(
            "All required checklist items must be completed.",
            exception.Message);
    }

    [Fact]
    public void Complete_WithCompletedRequiredItems_CompletesChecklist()
    {
        WorkOrderChecklist checklist =
            CreateChecklist();

        AddPassFailItem(
            checklist,
            ItemId,
            true,
            false,
            1);

        AddPassFailItem(
            checklist,
            SecondItemId,
            false,
            false,
            2);

        checklist.RecordPassFail(
            ItemId,
            ChecklistItemResult.Passed,
            null,
            null,
            CreatedAtUtc.AddMinutes(5),
            UserId);

        DateTimeOffset completedAtUtc =
            CreatedAtUtc.AddMinutes(10);

        checklist.Complete(
            completedAtUtc,
            SecondUserId);

        Assert.True(
            checklist.IsComplete);

        Assert.Equal(
            WorkOrderChecklistStatus.Completed,
            checklist.Status);

        Assert.Equal(
            completedAtUtc,
            checklist.CompletedAtUtc);

        Assert.Equal(
            SecondUserId,
            checklist.CompletedByUserId);
    }

    [Fact]
    public void Complete_AllowsFailedRequiredItem()
    {
        WorkOrderChecklist checklist =
            CreateChecklist();

        AddPassFailItem(
            checklist,
            ItemId,
            true,
            false,
            1);

        checklist.RecordPassFail(
            ItemId,
            ChecklistItemResult.Failed,
            "Repair recommended.",
            null,
            CreatedAtUtc.AddMinutes(5),
            UserId);

        checklist.Complete(
            CreatedAtUtc.AddMinutes(10),
            UserId);

        Assert.True(
            checklist.IsComplete);

        Assert.True(
            checklist.HasFailures);
    }

    [Fact]
    public void CompletedChecklist_CannotBeChanged()
    {
        WorkOrderChecklist checklist =
            CreateCompletedChecklist();

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => checklist.ResetItem(
                    ItemId,
                    CreatedAtUtc.AddMinutes(15),
                    UserId));

        Assert.Equal(
            "A completed checklist cannot be changed.",
            exception.Message);
    }

    [Fact]
    public void Reopen_CompletedChecklist_ReturnsToInProgress()
    {
        WorkOrderChecklist checklist =
            CreateCompletedChecklist();

        checklist.Reopen(
            CreatedAtUtc.AddMinutes(15),
            SecondUserId);

        Assert.Equal(
            WorkOrderChecklistStatus.InProgress,
            checklist.Status);

        Assert.False(
            checklist.IsComplete);

        Assert.Null(
            checklist.CompletedAtUtc);

        Assert.Null(
            checklist.CompletedByUserId);
    }

    [Fact]
    public void Reopen_InProgressChecklist_ThrowsInvalidOperationException()
    {
        WorkOrderChecklist checklist =
            CreateChecklist();

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => checklist.Reopen(
                    CreatedAtUtc.AddMinutes(5),
                    UserId));

        Assert.Equal(
            "Only a completed checklist can be reopened.",
            exception.Message);
    }

    [Fact]
    public void RemoveItem_RemovesItem()
    {
        WorkOrderChecklist checklist =
            CreateChecklist();

        AddPassFailItem(
            checklist,
            ItemId,
            true,
            false,
            1);

        checklist.RemoveItem(
            ItemId,
            CreatedAtUtc.AddMinutes(5),
            UserId);

        Assert.Empty(
            checklist.Items);
    }

    [Fact]
    public void Delete_SoftDeletesChecklist()
    {
        WorkOrderChecklist checklist =
            CreateChecklist();

        DateTimeOffset deletedAtUtc =
            CreatedAtUtc.AddMinutes(5);

        checklist.Delete(
            "  Created for the wrong work order.  ",
            deletedAtUtc,
            SecondUserId);

        Assert.True(
            checklist.IsDeleted);

        Assert.Equal(
            WorkOrderChecklistStatus.Deleted,
            checklist.Status);

        Assert.Equal(
            "Created for the wrong work order.",
            checklist.DeletionReason);

        Assert.Equal(
            deletedAtUtc,
            checklist.DeletedAtUtc);

        Assert.Equal(
            SecondUserId,
            checklist.DeletedByUserId);
    }

    [Fact]
    public void DeletedChecklist_CannotBeChanged()
    {
        WorkOrderChecklist checklist =
            CreateChecklist();

        checklist.Delete(
            "Duplicate.",
            CreatedAtUtc.AddMinutes(5),
            UserId);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => AddPassFailItem(
                    checklist,
                    ItemId,
                    true,
                    false,
                    1));

        Assert.Equal(
            "A deleted checklist cannot be changed.",
            exception.Message);
    }

    [Fact]
    public void GetMissingItem_ThrowsInvalidOperationException()
    {
        WorkOrderChecklist checklist =
            CreateChecklist();

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => checklist.ResetItem(
                    ItemId,
                    CreatedAtUtc.AddMinutes(5),
                    UserId));

        Assert.Equal(
            "The checklist item was not found.",
            exception.Message);
    }

    private static WorkOrderChecklist CreateChecklist()
    {
        return WorkOrderChecklist.Create(
            ChecklistId,
            WorkOrderId,
            "Preventive Maintenance",
            "Seasonal cooling inspection.",
            CreatedAtUtc,
            UserId);
    }

    private static WorkOrderChecklist CreateCompletedChecklist()
    {
        WorkOrderChecklist checklist =
            CreateChecklist();

        AddPassFailItem(
            checklist,
            ItemId,
            true,
            false,
            1);

        checklist.RecordPassFail(
            ItemId,
            ChecklistItemResult.Passed,
            null,
            null,
            CreatedAtUtc.AddMinutes(5),
            UserId);

        checklist.Complete(
            CreatedAtUtc.AddMinutes(10),
            UserId);

        return checklist;
    }

    private static WorkOrderChecklistItem AddPassFailItem(
        WorkOrderChecklist checklist,
        Guid itemId,
        bool isRequired,
        bool requiresPhoto,
        int displayOrder)
    {
        return checklist.AddItem(
            itemId,
            "General",
            "Inspect component.",
            ChecklistItemResponseType.PassFail,
            isRequired,
            requiresPhoto,
            displayOrder,
            null,
            CreatedAtUtc.AddMinutes(1),
            UserId);
    }
}
