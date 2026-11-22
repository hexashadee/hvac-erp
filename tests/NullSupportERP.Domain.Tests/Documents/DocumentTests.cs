using NullSupportERP.Domain.Documents;

namespace NullSupportERP.Domain.Tests.Documents;

public sealed class DocumentTests
{
    private static readonly Guid DocumentId =
        Guid.Parse(
            "11111111-1111-1111-1111-111111111111");

    private static readonly Guid CompanyId =
        Guid.Parse(
            "22222222-2222-2222-2222-222222222222");

    private static readonly Guid UserId =
        Guid.Parse(
            "33333333-3333-3333-3333-333333333333");

    private static readonly Guid SecondUserId =
        Guid.Parse(
            "44444444-4444-4444-4444-444444444444");

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
    public void Create_WithValidValues_CreatesPendingDocument()
    {
        Document document =
            CreateDocument();

        Assert.Equal(
            DocumentId,
            document.Id);

        Assert.Equal(
            CompanyId,
            document.CompanyId);

        Assert.Equal(
            DocumentCategory.BeforePhoto,
            document.Category);

        Assert.Equal(
            DocumentVisibility.Internal,
            document.Visibility);

        Assert.Equal(
            DocumentStatus.PendingUpload,
            document.Status);

        Assert.Equal(
            "condenser-before.jpg",
            document.FileName);

        Assert.Equal(
            "image/jpeg",
            document.ContentType);

        Assert.Equal(
            2_048_000,
            document.FileSizeBytes);

        Assert.Equal(
            "s3",
            document.StorageProvider);

        Assert.Equal(
            "companies/company-1/documents/document-1",
            document.StorageKey);

        Assert.Equal(
            "ABC123",
            document.ContentHash);

        Assert.Equal(
            "Outdoor condenser before repair.",
            document.Description);

        Assert.False(
            document.IsAvailable);

        Assert.False(
            document.IsDeleted);
    }

    [Fact]
    public void Create_NormalizesTextValues()
    {
        Document document =
            Document.Create(
                DocumentId,
                CompanyId,
                DocumentCategory.Photo,
                DocumentVisibility.CustomerVisible,
                "  system.jpg  ",
                "  image/jpeg  ",
                1_000,
                "  s3  ",
                "  documents/system.jpg  ",
                "  HASH123  ",
                "  Completed repair.  ",
                CreatedAtUtc,
                UserId);

        Assert.Equal(
            "system.jpg",
            document.FileName);

        Assert.Equal(
            "image/jpeg",
            document.ContentType);

        Assert.Equal(
            "s3",
            document.StorageProvider);

        Assert.Equal(
            "documents/system.jpg",
            document.StorageKey);

        Assert.Equal(
            "HASH123",
            document.ContentHash);

        Assert.Equal(
            "Completed repair.",
            document.Description);
    }

    [Fact]
    public void Create_WithBlankOptionalValues_StoresNull()
    {
        Document document =
            Document.Create(
                DocumentId,
                CompanyId,
                DocumentCategory.General,
                DocumentVisibility.Internal,
                "document.pdf",
                "application/pdf",
                1_000,
                "s3",
                "documents/document.pdf",
                " ",
                null,
                CreatedAtUtc,
                UserId);

        Assert.Null(
            document.ContentHash);

        Assert.Null(
            document.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Create_WithBlankFileName_ThrowsArgumentException(
        string fileName)
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => Document.Create(
                    DocumentId,
                    CompanyId,
                    DocumentCategory.General,
                    DocumentVisibility.Internal,
                    fileName,
                    "application/pdf",
                    1_000,
                    "s3",
                    "documents/document.pdf",
                    null,
                    null,
                    CreatedAtUtc,
                    UserId));

        Assert.Equal(
            "fileName",
            exception.ParamName);
    }

    [Theory]
    [InlineData("../document.pdf")]
    [InlineData("folder/document.pdf")]
    [InlineData("folder\\document.pdf")]
    [InlineData(".")]
    [InlineData("..")]
    public void Create_WithInvalidFileName_ThrowsArgumentException(
        string fileName)
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => Document.Create(
                    DocumentId,
                    CompanyId,
                    DocumentCategory.General,
                    DocumentVisibility.Internal,
                    fileName,
                    "application/pdf",
                    1_000,
                    "s3",
                    "documents/document.pdf",
                    null,
                    null,
                    CreatedAtUtc,
                    UserId));

        Assert.Equal(
            "fileName",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithEmptyDocumentId_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => Document.Create(
                    Guid.Empty,
                    CompanyId,
                    DocumentCategory.General,
                    DocumentVisibility.Internal,
                    "document.pdf",
                    "application/pdf",
                    1_000,
                    "s3",
                    "documents/document.pdf",
                    null,
                    null,
                    CreatedAtUtc,
                    UserId));

        Assert.Equal(
            "id",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithEmptyCompanyId_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => Document.Create(
                    DocumentId,
                    Guid.Empty,
                    DocumentCategory.General,
                    DocumentVisibility.Internal,
                    "document.pdf",
                    "application/pdf",
                    1_000,
                    "s3",
                    "documents/document.pdf",
                    null,
                    null,
                    CreatedAtUtc,
                    UserId));

        Assert.Equal(
            "companyId",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithInvalidCategory_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Document.Create(
                DocumentId,
                CompanyId,
                (DocumentCategory)500,
                DocumentVisibility.Internal,
                "document.pdf",
                "application/pdf",
                1_000,
                "s3",
                "documents/document.pdf",
                null,
                null,
                CreatedAtUtc,
                UserId));
    }

    [Fact]
    public void Create_WithInvalidVisibility_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Document.Create(
                DocumentId,
                CompanyId,
                DocumentCategory.General,
                (DocumentVisibility)500,
                "document.pdf",
                "application/pdf",
                1_000,
                "s3",
                "documents/document.pdf",
                null,
                null,
                CreatedAtUtc,
                UserId));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithNonPositiveFileSize_ThrowsArgumentOutOfRangeException(
        long fileSizeBytes)
    {
        ArgumentOutOfRangeException exception =
            Assert.Throws<ArgumentOutOfRangeException>(
                () => Document.Create(
                    DocumentId,
                    CompanyId,
                    DocumentCategory.General,
                    DocumentVisibility.Internal,
                    "document.pdf",
                    "application/pdf",
                    fileSizeBytes,
                    "s3",
                    "documents/document.pdf",
                    null,
                    null,
                    CreatedAtUtc,
                    UserId));

        Assert.Equal(
            "fileSizeBytes",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithFileLargerThanMaximum_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Document.Create(
                DocumentId,
                CompanyId,
                DocumentCategory.Video,
                DocumentVisibility.Internal,
                "video.mp4",
                "video/mp4",
                Document.MaximumFileSizeBytes + 1,
                "s3",
                "documents/video.mp4",
                null,
                null,
                CreatedAtUtc,
                UserId));
    }

    [Fact]
    public void MarkAvailable_FromPendingUpload_MarksDocumentAvailable()
    {
        Document document =
            CreateDocument();

        DateTimeOffset uploadedAtUtc =
            CreatedAtUtc.AddMinutes(5);

        document.MarkAvailable(
            uploadedAtUtc,
            SecondUserId);

        Assert.Equal(
            DocumentStatus.Available,
            document.Status);

        Assert.True(
            document.IsAvailable);

        Assert.Equal(
            uploadedAtUtc,
            document.UploadedAtUtc);

        Assert.Equal(
            SecondUserId,
            document.UploadedByUserId);

        Assert.Equal(
            uploadedAtUtc,
            document.LastModifiedAtUtc);

        Assert.Equal(
            SecondUserId,
            document.LastModifiedByUserId);
    }

    [Fact]
    public void MarkAvailable_WhenAlreadyAvailable_ThrowsInvalidOperationException()
    {
        Document document =
            CreateAvailableDocument();

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => document.MarkAvailable(
                    CreatedAtUtc.AddMinutes(10),
                    UserId));

        Assert.Equal(
            "The document is already available.",
            exception.Message);
    }

    [Fact]
    public void MarkUploadFailed_ClearsUploadInformation()
    {
        Document document =
            CreateAvailableDocument();

        document.MarkUploadFailed(
            CreatedAtUtc.AddMinutes(10),
            SecondUserId);

        Assert.Equal(
            DocumentStatus.UploadFailed,
            document.Status);

        Assert.Null(
            document.UploadedAtUtc);

        Assert.Null(
            document.UploadedByUserId);
    }

    [Fact]
    public void MarkQuarantined_ChangesStatus()
    {
        Document document =
            CreateDocument();

        document.MarkQuarantined(
            CreatedAtUtc.AddMinutes(5),
            SecondUserId);

        Assert.Equal(
            DocumentStatus.Quarantined,
            document.Status);
    }

    [Fact]
    public void RetryUpload_FromFailedStatus_ResetsDocument()
    {
        Document document =
            CreateDocument();

        document.MarkUploadFailed(
            CreatedAtUtc.AddMinutes(5),
            UserId);

        document.RetryUpload(
            "azure-blob",
            "replacement/document-1",
            CreatedAtUtc.AddMinutes(10),
            SecondUserId);

        Assert.Equal(
            DocumentStatus.PendingUpload,
            document.Status);

        Assert.Equal(
            "azure-blob",
            document.StorageProvider);

        Assert.Equal(
            "replacement/document-1",
            document.StorageKey);

        Assert.Null(
            document.UploadedAtUtc);

        Assert.Null(
            document.UploadedByUserId);
    }

    [Fact]
    public void RetryUpload_FromAvailableStatus_ThrowsInvalidOperationException()
    {
        Document document =
            CreateAvailableDocument();

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => document.RetryUpload(
                    "s3",
                    "new-key",
                    CreatedAtUtc.AddMinutes(10),
                    UserId));

        Assert.Equal(
            "Only a failed or quarantined document can retry uploading.",
            exception.Message);
    }

    [Fact]
    public void UpdateMetadata_WithValidValues_UpdatesMetadata()
    {
        Document document =
            CreateDocument();

        document.UpdateMetadata(
            DocumentCategory.AfterPhoto,
            DocumentVisibility.CustomerVisible,
            "condenser-after.jpg",
            "  Repair completed.  ",
            CreatedAtUtc.AddMinutes(5),
            SecondUserId);

        Assert.Equal(
            DocumentCategory.AfterPhoto,
            document.Category);

        Assert.Equal(
            DocumentVisibility.CustomerVisible,
            document.Visibility);

        Assert.True(
            document.IsCustomerVisible);

        Assert.Equal(
            "condenser-after.jpg",
            document.FileName);

        Assert.Equal(
            "Repair completed.",
            document.Description);
    }

    [Fact]
    public void ReplaceFile_WithValidValues_ReplacesFileAndRequiresUpload()
    {
        Document document =
            CreateAvailableDocument();

        document.ReplaceFile(
            "replacement.pdf",
            "application/pdf",
            5_000,
            "s3",
            "replacement/document.pdf",
            "REPLACEMENT-HASH",
            CreatedAtUtc.AddMinutes(10),
            SecondUserId);

        Assert.Equal(
            "replacement.pdf",
            document.FileName);

        Assert.Equal(
            "application/pdf",
            document.ContentType);

        Assert.Equal(
            5_000,
            document.FileSizeBytes);

        Assert.Equal(
            "replacement/document.pdf",
            document.StorageKey);

        Assert.Equal(
            "REPLACEMENT-HASH",
            document.ContentHash);

        Assert.Equal(
            DocumentStatus.PendingUpload,
            document.Status);

        Assert.Null(
            document.UploadedAtUtc);

        Assert.Null(
            document.UploadedByUserId);
    }

    [Fact]
    public void Delete_WithValidValues_SoftDeletesDocument()
    {
        Document document =
            CreateAvailableDocument();

        DateTimeOffset deletedAtUtc =
            CreatedAtUtc.AddMinutes(10);

        document.Delete(
            "  Uploaded to the wrong company.  ",
            deletedAtUtc,
            SecondUserId);

        Assert.True(
            document.IsDeleted);

        Assert.Equal(
            DocumentStatus.Deleted,
            document.Status);

        Assert.Equal(
            "Uploaded to the wrong company.",
            document.DeletionReason);

        Assert.Equal(
            deletedAtUtc,
            document.DeletedAtUtc);

        Assert.Equal(
            SecondUserId,
            document.DeletedByUserId);
    }

    [Fact]
    public void Delete_WhenAlreadyDeleted_ThrowsInvalidOperationException()
    {
        Document document =
            CreateDocument();

        document.Delete(
            "Duplicate.",
            CreatedAtUtc.AddMinutes(5),
            UserId);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => document.Delete(
                    "Delete again.",
                    CreatedAtUtc.AddMinutes(10),
                    UserId));

        Assert.Equal(
            "A deleted document cannot be changed.",
            exception.Message);
    }

    [Fact]
    public void UpdateMetadata_WhenDeleted_ThrowsInvalidOperationException()
    {
        Document document =
            CreateDocument();

        document.Delete(
            "Duplicate.",
            CreatedAtUtc.AddMinutes(5),
            UserId);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => document.UpdateMetadata(
                    DocumentCategory.General,
                    DocumentVisibility.Internal,
                    "new-name.pdf",
                    null,
                    CreatedAtUtc.AddMinutes(10),
                    UserId));

        Assert.Equal(
            "A deleted document cannot be changed.",
            exception.Message);
    }

    [Fact]
    public void MarkAvailable_BeforeCreationTime_ThrowsArgumentException()
    {
        Document document =
            CreateDocument();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => document.MarkAvailable(
                    CreatedAtUtc.AddMinutes(-1),
                    UserId));

        Assert.Equal(
            "uploadedAtUtc",
            exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Delete_WithBlankReason_ThrowsArgumentException(
        string deletionReason)
    {
        Document document =
            CreateDocument();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => document.Delete(
                    deletionReason,
                    CreatedAtUtc.AddMinutes(5),
                    UserId));

        Assert.Equal(
            "deletionReason",
            exception.ParamName);
    }

    private static Document CreateDocument()
    {
        return Document.Create(
            DocumentId,
            CompanyId,
            DocumentCategory.BeforePhoto,
            DocumentVisibility.Internal,
            "condenser-before.jpg",
            "image/jpeg",
            2_048_000,
            "s3",
            "companies/company-1/documents/document-1",
            "ABC123",
            "Outdoor condenser before repair.",
            CreatedAtUtc,
            UserId);
    }

    private static Document CreateAvailableDocument()
    {
        Document document =
            CreateDocument();

        document.MarkAvailable(
            CreatedAtUtc.AddMinutes(5),
            UserId);

        return document;
    }
}
