using HvacERP.Domain.WorkOrders;

namespace HvacERP.Domain.Tests.WorkOrders;

public sealed class WorkOrderAttachmentTests
{
    private static readonly Guid AttachmentId =
        Guid.Parse(
            "11111111-1111-1111-1111-111111111111");

    private static readonly Guid WorkOrderId =
        Guid.Parse(
            "22222222-2222-2222-2222-222222222222");

    private static readonly Guid DocumentId =
        Guid.Parse(
            "33333333-3333-3333-3333-333333333333");

    private static readonly Guid UserId =
        Guid.Parse(
            "44444444-4444-4444-4444-444444444444");

    private static readonly Guid SecondUserId =
        Guid.Parse(
            "55555555-5555-5555-5555-555555555555");

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
    public void Create_WithValidValues_CreatesAttachment()
    {
        WorkOrderAttachment attachment =
            CreateAttachment();

        Assert.Equal(
            AttachmentId,
            attachment.Id);

        Assert.Equal(
            WorkOrderId,
            attachment.WorkOrderId);

        Assert.Equal(
            DocumentId,
            attachment.DocumentId);

        Assert.Equal(
            WorkOrderAttachmentType.BeforePhoto,
            attachment.AttachmentType);

        Assert.Equal(
            "Outdoor condenser before repair.",
            attachment.Caption);

        Assert.Equal(
            1,
            attachment.DisplayOrder);

        Assert.False(
            attachment.IsPrimary);

        Assert.False(
            attachment.IsDeleted);
    }

    [Fact]
    public void Create_WithPrimaryValue_CreatesPrimaryAttachment()
    {
        WorkOrderAttachment attachment =
            WorkOrderAttachment.Create(
                AttachmentId,
                WorkOrderId,
                DocumentId,
                WorkOrderAttachmentType.EquipmentPhoto,
                null,
                0,
                true,
                CreatedAtUtc,
                UserId);

        Assert.True(
            attachment.IsPrimary);
    }

    [Fact]
    public void Create_NormalizesCaption()
    {
        WorkOrderAttachment attachment =
            WorkOrderAttachment.Create(
                AttachmentId,
                WorkOrderId,
                DocumentId,
                WorkOrderAttachmentType.AfterPhoto,
                "  Repair completed.  ",
                2,
                false,
                CreatedAtUtc,
                UserId);

        Assert.Equal(
            "Repair completed.",
            attachment.Caption);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithBlankCaption_StoresNull(
        string? caption)
    {
        WorkOrderAttachment attachment =
            WorkOrderAttachment.Create(
                AttachmentId,
                WorkOrderId,
                DocumentId,
                WorkOrderAttachmentType.General,
                caption,
                0,
                false,
                CreatedAtUtc,
                UserId);

        Assert.Null(
            attachment.Caption);
    }

    [Fact]
    public void Create_WithEmptyAttachmentId_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => WorkOrderAttachment.Create(
                    Guid.Empty,
                    WorkOrderId,
                    DocumentId,
                    WorkOrderAttachmentType.General,
                    null,
                    0,
                    false,
                    CreatedAtUtc,
                    UserId));

        Assert.Equal(
            "id",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithEmptyWorkOrderId_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => WorkOrderAttachment.Create(
                    AttachmentId,
                    Guid.Empty,
                    DocumentId,
                    WorkOrderAttachmentType.General,
                    null,
                    0,
                    false,
                    CreatedAtUtc,
                    UserId));

        Assert.Equal(
            "workOrderId",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithEmptyDocumentId_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => WorkOrderAttachment.Create(
                    AttachmentId,
                    WorkOrderId,
                    Guid.Empty,
                    WorkOrderAttachmentType.General,
                    null,
                    0,
                    false,
                    CreatedAtUtc,
                    UserId));

        Assert.Equal(
            "documentId",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithEmptyCreatedByUserId_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => WorkOrderAttachment.Create(
                    AttachmentId,
                    WorkOrderId,
                    DocumentId,
                    WorkOrderAttachmentType.General,
                    null,
                    0,
                    false,
                    CreatedAtUtc,
                    Guid.Empty));

        Assert.Equal(
            "createdByUserId",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithInvalidAttachmentType_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => WorkOrderAttachment.Create(
                AttachmentId,
                WorkOrderId,
                DocumentId,
                (WorkOrderAttachmentType)500,
                null,
                0,
                false,
                CreatedAtUtc,
                UserId));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(10001)]
    public void Create_WithInvalidDisplayOrder_ThrowsArgumentOutOfRangeException(
        int displayOrder)
    {
        ArgumentOutOfRangeException exception =
            Assert.Throws<ArgumentOutOfRangeException>(
                () => WorkOrderAttachment.Create(
                    AttachmentId,
                    WorkOrderId,
                    DocumentId,
                    WorkOrderAttachmentType.General,
                    null,
                    displayOrder,
                    false,
                    CreatedAtUtc,
                    UserId));

        Assert.Equal(
            "displayOrder",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithCaptionLargerThanMaximum_ThrowsArgumentException()
    {
        string caption =
            new(
                'A',
                WorkOrderAttachment.MaximumCaptionLength + 1);

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => WorkOrderAttachment.Create(
                    AttachmentId,
                    WorkOrderId,
                    DocumentId,
                    WorkOrderAttachmentType.General,
                    caption,
                    0,
                    false,
                    CreatedAtUtc,
                    UserId));

        Assert.Equal(
            "caption",
            exception.ParamName);
    }

    [Fact]
    public void UpdateDetails_WithValidValues_UpdatesAttachment()
    {
        WorkOrderAttachment attachment =
            CreateAttachment();

        DateTimeOffset modifiedAtUtc =
            CreatedAtUtc.AddMinutes(5);

        attachment.UpdateDetails(
            WorkOrderAttachmentType.AfterPhoto,
            "  Outdoor condenser after repair.  ",
            3,
            modifiedAtUtc,
            SecondUserId);

        Assert.Equal(
            WorkOrderAttachmentType.AfterPhoto,
            attachment.AttachmentType);

        Assert.Equal(
            "Outdoor condenser after repair.",
            attachment.Caption);

        Assert.Equal(
            3,
            attachment.DisplayOrder);

        Assert.Equal(
            modifiedAtUtc,
            attachment.LastModifiedAtUtc);

        Assert.Equal(
            SecondUserId,
            attachment.LastModifiedByUserId);
    }

    [Fact]
    public void ChangeDisplayOrder_WithValidValue_ChangesOrder()
    {
        WorkOrderAttachment attachment =
            CreateAttachment();

        attachment.ChangeDisplayOrder(
            25,
            CreatedAtUtc.AddMinutes(5),
            SecondUserId);

        Assert.Equal(
            25,
            attachment.DisplayOrder);
    }

    [Fact]
    public void MarkAsPrimary_WhenNotPrimary_MarksAttachmentPrimary()
    {
        WorkOrderAttachment attachment =
            CreateAttachment();

        attachment.MarkAsPrimary(
            CreatedAtUtc.AddMinutes(5),
            SecondUserId);

        Assert.True(
            attachment.IsPrimary);
    }

    [Fact]
    public void MarkAsPrimary_WhenAlreadyPrimary_ThrowsInvalidOperationException()
    {
        WorkOrderAttachment attachment =
            CreatePrimaryAttachment();

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => attachment.MarkAsPrimary(
                    CreatedAtUtc.AddMinutes(5),
                    SecondUserId));

        Assert.Equal(
            "The attachment is already primary.",
            exception.Message);
    }

    [Fact]
    public void RemovePrimaryStatus_WhenPrimary_RemovesPrimaryStatus()
    {
        WorkOrderAttachment attachment =
            CreatePrimaryAttachment();

        attachment.RemovePrimaryStatus(
            CreatedAtUtc.AddMinutes(5),
            SecondUserId);

        Assert.False(
            attachment.IsPrimary);
    }

    [Fact]
    public void RemovePrimaryStatus_WhenNotPrimary_ThrowsInvalidOperationException()
    {
        WorkOrderAttachment attachment =
            CreateAttachment();

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => attachment.RemovePrimaryStatus(
                    CreatedAtUtc.AddMinutes(5),
                    SecondUserId));

        Assert.Equal(
            "The attachment is not primary.",
            exception.Message);
    }

    [Fact]
    public void Delete_WithValidValues_SoftDeletesAttachment()
    {
        WorkOrderAttachment attachment =
            CreatePrimaryAttachment();

        DateTimeOffset deletedAtUtc =
            CreatedAtUtc.AddMinutes(10);

        attachment.Delete(
            "  Duplicate attachment.  ",
            deletedAtUtc,
            SecondUserId);

        Assert.True(
            attachment.IsDeleted);

        Assert.False(
            attachment.IsPrimary);

        Assert.Equal(
            "Duplicate attachment.",
            attachment.DeletionReason);

        Assert.Equal(
            deletedAtUtc,
            attachment.DeletedAtUtc);

        Assert.Equal(
            SecondUserId,
            attachment.DeletedByUserId);

        Assert.Equal(
            deletedAtUtc,
            attachment.LastModifiedAtUtc);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Delete_WithBlankReason_ThrowsArgumentException(
        string deletionReason)
    {
        WorkOrderAttachment attachment =
            CreateAttachment();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => attachment.Delete(
                    deletionReason,
                    CreatedAtUtc.AddMinutes(5),
                    SecondUserId));

        Assert.Equal(
            "deletionReason",
            exception.ParamName);
    }

    [Fact]
    public void Delete_WhenAlreadyDeleted_ThrowsInvalidOperationException()
    {
        WorkOrderAttachment attachment =
            CreateAttachment();

        attachment.Delete(
            "Duplicate.",
            CreatedAtUtc.AddMinutes(5),
            SecondUserId);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => attachment.Delete(
                    "Delete again.",
                    CreatedAtUtc.AddMinutes(10),
                    SecondUserId));

        Assert.Equal(
            "A deleted work order attachment cannot be changed.",
            exception.Message);
    }

    [Fact]
    public void UpdateDetails_WhenDeleted_ThrowsInvalidOperationException()
    {
        WorkOrderAttachment attachment =
            CreateAttachment();

        attachment.Delete(
            "Duplicate.",
            CreatedAtUtc.AddMinutes(5),
            SecondUserId);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => attachment.UpdateDetails(
                    WorkOrderAttachmentType.AfterPhoto,
                    "Updated caption.",
                    2,
                    CreatedAtUtc.AddMinutes(10),
                    SecondUserId));

        Assert.Equal(
            "A deleted work order attachment cannot be changed.",
            exception.Message);
    }

    [Fact]
    public void ChangeDisplayOrder_WhenDeleted_ThrowsInvalidOperationException()
    {
        WorkOrderAttachment attachment =
            CreateAttachment();

        attachment.Delete(
            "Duplicate.",
            CreatedAtUtc.AddMinutes(5),
            SecondUserId);

        Assert.Throws<InvalidOperationException>(
            () => attachment.ChangeDisplayOrder(
                10,
                CreatedAtUtc.AddMinutes(10),
                SecondUserId));
    }

    [Fact]
    public void MarkAsPrimary_WhenDeleted_ThrowsInvalidOperationException()
    {
        WorkOrderAttachment attachment =
            CreateAttachment();

        attachment.Delete(
            "Duplicate.",
            CreatedAtUtc.AddMinutes(5),
            SecondUserId);

        Assert.Throws<InvalidOperationException>(
            () => attachment.MarkAsPrimary(
                CreatedAtUtc.AddMinutes(10),
                SecondUserId));
    }

    [Fact]
    public void UpdateDetails_BeforeCreationTime_ThrowsArgumentException()
    {
        WorkOrderAttachment attachment =
            CreateAttachment();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => attachment.UpdateDetails(
                    WorkOrderAttachmentType.AfterPhoto,
                    null,
                    2,
                    CreatedAtUtc.AddMinutes(-1),
                    SecondUserId));

        Assert.Equal(
            "modifiedAtUtc",
            exception.ParamName);
    }

    [Fact]
    public void UpdateDetails_WithEmptyModifiedByUserId_ThrowsArgumentException()
    {
        WorkOrderAttachment attachment =
            CreateAttachment();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => attachment.UpdateDetails(
                    WorkOrderAttachmentType.AfterPhoto,
                    null,
                    2,
                    CreatedAtUtc.AddMinutes(5),
                    Guid.Empty));

        Assert.Equal(
            "modifiedByUserId",
            exception.ParamName);
    }

    [Fact]
    public void Delete_WithReasonLargerThanMaximum_ThrowsArgumentException()
    {
        WorkOrderAttachment attachment =
            CreateAttachment();

        string deletionReason =
            new(
                'A',
                WorkOrderAttachment.MaximumDeletionReasonLength + 1);

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => attachment.Delete(
                    deletionReason,
                    CreatedAtUtc.AddMinutes(5),
                    SecondUserId));

        Assert.Equal(
            "deletionReason",
            exception.ParamName);
    }

    private static WorkOrderAttachment CreateAttachment()
    {
        return WorkOrderAttachment.Create(
            AttachmentId,
            WorkOrderId,
            DocumentId,
            WorkOrderAttachmentType.BeforePhoto,
            "Outdoor condenser before repair.",
            1,
            false,
            CreatedAtUtc,
            UserId);
    }

    private static WorkOrderAttachment CreatePrimaryAttachment()
    {
        return WorkOrderAttachment.Create(
            AttachmentId,
            WorkOrderId,
            DocumentId,
            WorkOrderAttachmentType.EquipmentPhoto,
            "Primary equipment photo.",
            0,
            true,
            CreatedAtUtc,
            UserId);
    }
}
