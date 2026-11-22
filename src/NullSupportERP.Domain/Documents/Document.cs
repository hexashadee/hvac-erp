using NullSupportERP.Domain.Common.Base;

namespace NullSupportERP.Domain.Documents;

public sealed class Document : AggregateRoot
{
    public const int MaximumFileNameLength = 255;

    public const int MaximumContentTypeLength = 255;

    public const int MaximumStorageProviderLength = 100;

    public const int MaximumStorageKeyLength = 2_000;

    public const int MaximumHashLength = 128;

    public const int MaximumDescriptionLength = 2_000;

    public const int MaximumDeletionReasonLength = 500;

    public const long MaximumFileSizeBytes =
        5L * 1024L * 1024L * 1024L;

    public Guid CompanyId { get; private set; }

    public DocumentCategory Category { get; private set; }

    public DocumentVisibility Visibility { get; private set; }

    public DocumentStatus Status { get; private set; }

    public string FileName { get; private set; }

    public string ContentType { get; private set; }

    public long FileSizeBytes { get; private set; }

    public string StorageProvider { get; private set; }

    public string StorageKey { get; private set; }

    public string? ContentHash { get; private set; }

    public string? Description { get; private set; }

    public DateTimeOffset? UploadedAtUtc { get; private set; }

    public Guid? UploadedByUserId { get; private set; }

    public DateTimeOffset? DeletedAtUtc { get; private set; }

    public Guid? DeletedByUserId { get; private set; }

    public string? DeletionReason { get; private set; }

    public bool IsDeleted =>
        Status == DocumentStatus.Deleted;

    public bool IsAvailable =>
        Status == DocumentStatus.Available;

    public bool IsCustomerVisible =>
        Visibility == DocumentVisibility.CustomerVisible;

    public bool IsVendorVisible =>
        Visibility == DocumentVisibility.VendorVisible;

    private Document(
        Guid id,
        Guid companyId,
        DocumentCategory category,
        DocumentVisibility visibility,
        string fileName,
        string contentType,
        long fileSizeBytes,
        string storageProvider,
        string storageKey,
        string? contentHash,
        string? description,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
        : base(
            id,
            createdAtUtc,
            createdByUserId)
    {
        CompanyId = companyId;
        Category = category;
        Visibility = visibility;
        Status = DocumentStatus.PendingUpload;
        FileName = fileName;
        ContentType = contentType;
        FileSizeBytes = fileSizeBytes;
        StorageProvider = storageProvider;
        StorageKey = storageKey;
        ContentHash = contentHash;
        Description = description;
    }

    public static Document Create(
        Guid id,
        Guid companyId,
        DocumentCategory category,
        DocumentVisibility visibility,
        string fileName,
        string contentType,
        long fileSizeBytes,
        string storageProvider,
        string storageKey,
        string? contentHash,
        string? description,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
    {
        ValidateId(
            id,
            nameof(id),
            "Document ID");

        ValidateId(
            companyId,
            nameof(companyId),
            "Company ID");

        ValidateCategory(category);

        ValidateVisibility(visibility);

        string normalizedFileName =
            NormalizeRequired(
                fileName,
                MaximumFileNameLength,
                nameof(fileName),
                "File name");

        ValidateFileName(normalizedFileName);

        string normalizedContentType =
            NormalizeRequired(
                contentType,
                MaximumContentTypeLength,
                nameof(contentType),
                "Content type");

        ValidateFileSize(fileSizeBytes);

        string normalizedStorageProvider =
            NormalizeRequired(
                storageProvider,
                MaximumStorageProviderLength,
                nameof(storageProvider),
                "Storage provider");

        string normalizedStorageKey =
            NormalizeRequired(
                storageKey,
                MaximumStorageKeyLength,
                nameof(storageKey),
                "Storage key");

        string? normalizedContentHash =
            NormalizeOptional(
                contentHash,
                MaximumHashLength,
                nameof(contentHash),
                "Content hash");

        string? normalizedDescription =
            NormalizeOptional(
                description,
                MaximumDescriptionLength,
                nameof(description),
                "Description");

        ValidateId(
            createdByUserId,
            nameof(createdByUserId),
            "Created by user ID");

        return new Document(
            id,
            companyId,
            category,
            visibility,
            normalizedFileName,
            normalizedContentType,
            fileSizeBytes,
            normalizedStorageProvider,
            normalizedStorageKey,
            normalizedContentHash,
            normalizedDescription,
            createdAtUtc,
            createdByUserId);
    }

    public void MarkAvailable(
        DateTimeOffset uploadedAtUtc,
        Guid uploadedByUserId)
    {
        EnsureNotDeleted();

        if (Status == DocumentStatus.Available)
        {
            throw new InvalidOperationException(
                "The document is already available.");
        }

        ValidateId(
            uploadedByUserId,
            nameof(uploadedByUserId),
            "Uploaded by user ID");

        EnsureActionNotBeforeCreation(
            uploadedAtUtc,
            nameof(uploadedAtUtc));

        Status = DocumentStatus.Available;
        UploadedAtUtc = uploadedAtUtc;
        UploadedByUserId = uploadedByUserId;

        MarkModified(
            uploadedAtUtc,
            uploadedByUserId);
    }

    public void MarkUploadFailed(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotDeleted();

        ValidateId(
            modifiedByUserId,
            nameof(modifiedByUserId),
            "Modified by user ID");

        EnsureActionNotBeforeCreation(
            modifiedAtUtc,
            nameof(modifiedAtUtc));

        Status = DocumentStatus.UploadFailed;
        UploadedAtUtc = null;
        UploadedByUserId = null;

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void MarkQuarantined(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotDeleted();

        ValidateId(
            modifiedByUserId,
            nameof(modifiedByUserId),
            "Modified by user ID");

        EnsureActionNotBeforeCreation(
            modifiedAtUtc,
            nameof(modifiedAtUtc));

        Status = DocumentStatus.Quarantined;

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void RetryUpload(
        string storageProvider,
        string storageKey,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotDeleted();

        if (Status != DocumentStatus.UploadFailed &&
            Status != DocumentStatus.Quarantined)
        {
            throw new InvalidOperationException(
                "Only a failed or quarantined document can retry uploading.");
        }

        string normalizedStorageProvider =
            NormalizeRequired(
                storageProvider,
                MaximumStorageProviderLength,
                nameof(storageProvider),
                "Storage provider");

        string normalizedStorageKey =
            NormalizeRequired(
                storageKey,
                MaximumStorageKeyLength,
                nameof(storageKey),
                "Storage key");

        ValidateId(
            modifiedByUserId,
            nameof(modifiedByUserId),
            "Modified by user ID");

        EnsureActionNotBeforeCreation(
            modifiedAtUtc,
            nameof(modifiedAtUtc));

        StorageProvider = normalizedStorageProvider;
        StorageKey = normalizedStorageKey;
        Status = DocumentStatus.PendingUpload;
        UploadedAtUtc = null;
        UploadedByUserId = null;

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void UpdateMetadata(
        DocumentCategory category,
        DocumentVisibility visibility,
        string fileName,
        string? description,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotDeleted();

        ValidateCategory(category);

        ValidateVisibility(visibility);

        string normalizedFileName =
            NormalizeRequired(
                fileName,
                MaximumFileNameLength,
                nameof(fileName),
                "File name");

        ValidateFileName(normalizedFileName);

        string? normalizedDescription =
            NormalizeOptional(
                description,
                MaximumDescriptionLength,
                nameof(description),
                "Description");

        ValidateId(
            modifiedByUserId,
            nameof(modifiedByUserId),
            "Modified by user ID");

        EnsureActionNotBeforeCreation(
            modifiedAtUtc,
            nameof(modifiedAtUtc));

        Category = category;
        Visibility = visibility;
        FileName = normalizedFileName;
        Description = normalizedDescription;

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void ReplaceFile(
        string fileName,
        string contentType,
        long fileSizeBytes,
        string storageProvider,
        string storageKey,
        string? contentHash,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotDeleted();

        string normalizedFileName =
            NormalizeRequired(
                fileName,
                MaximumFileNameLength,
                nameof(fileName),
                "File name");

        ValidateFileName(normalizedFileName);

        string normalizedContentType =
            NormalizeRequired(
                contentType,
                MaximumContentTypeLength,
                nameof(contentType),
                "Content type");

        ValidateFileSize(fileSizeBytes);

        string normalizedStorageProvider =
            NormalizeRequired(
                storageProvider,
                MaximumStorageProviderLength,
                nameof(storageProvider),
                "Storage provider");

        string normalizedStorageKey =
            NormalizeRequired(
                storageKey,
                MaximumStorageKeyLength,
                nameof(storageKey),
                "Storage key");

        string? normalizedContentHash =
            NormalizeOptional(
                contentHash,
                MaximumHashLength,
                nameof(contentHash),
                "Content hash");

        ValidateId(
            modifiedByUserId,
            nameof(modifiedByUserId),
            "Modified by user ID");

        EnsureActionNotBeforeCreation(
            modifiedAtUtc,
            nameof(modifiedAtUtc));

        FileName = normalizedFileName;
        ContentType = normalizedContentType;
        FileSizeBytes = fileSizeBytes;
        StorageProvider = normalizedStorageProvider;
        StorageKey = normalizedStorageKey;
        ContentHash = normalizedContentHash;
        Status = DocumentStatus.PendingUpload;
        UploadedAtUtc = null;
        UploadedByUserId = null;

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void Delete(
        string deletionReason,
        DateTimeOffset deletedAtUtc,
        Guid deletedByUserId)
    {
        EnsureNotDeleted();

        string normalizedDeletionReason =
            NormalizeRequired(
                deletionReason,
                MaximumDeletionReasonLength,
                nameof(deletionReason),
                "Deletion reason");

        ValidateId(
            deletedByUserId,
            nameof(deletedByUserId),
            "Deleted by user ID");

        EnsureActionNotBeforeCreation(
            deletedAtUtc,
            nameof(deletedAtUtc));

        Status = DocumentStatus.Deleted;
        DeletionReason = normalizedDeletionReason;
        DeletedAtUtc = deletedAtUtc;
        DeletedByUserId = deletedByUserId;

        MarkModified(
            deletedAtUtc,
            deletedByUserId);
    }

    private void EnsureNotDeleted()
    {
        if (IsDeleted)
        {
            throw new InvalidOperationException(
                "A deleted document cannot be changed.");
        }
    }

    private void EnsureActionNotBeforeCreation(
        DateTimeOffset actionAtUtc,
        string parameterName)
    {
        if (actionAtUtc < CreatedAtUtc)
        {
            throw new ArgumentException(
                "Document activity cannot occur before the document was created.",
                parameterName);
        }
    }

    private static void ValidateId(
        Guid value,
        string parameterName,
        string displayName)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(
                $"{displayName} is required.",
                parameterName);
        }
    }

    private static void ValidateCategory(
        DocumentCategory category)
    {
        if (!Enum.IsDefined(category))
        {
            throw new ArgumentOutOfRangeException(
                nameof(category),
                category,
                "Document category is invalid.");
        }
    }

    private static void ValidateVisibility(
        DocumentVisibility visibility)
    {
        if (!Enum.IsDefined(visibility))
        {
            throw new ArgumentOutOfRangeException(
                nameof(visibility),
                visibility,
                "Document visibility is invalid.");
        }
    }

    private static void ValidateFileSize(
        long fileSizeBytes)
    {
        if (fileSizeBytes <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(fileSizeBytes),
                fileSizeBytes,
                "File size must be greater than zero.");
        }

        if (fileSizeBytes > MaximumFileSizeBytes)
        {
            throw new ArgumentOutOfRangeException(
                nameof(fileSizeBytes),
                fileSizeBytes,
                "File size cannot exceed 5 GB.");
        }
    }

    private static void ValidateFileName(
        string fileName)
    {
        if (fileName is "." or "..")
        {
            throw new ArgumentException(
                "File name is invalid.",
                nameof(fileName));
        }

        if (fileName.IndexOfAny(
                [
                    '/',
                    '\\',
                    '\0'
                ]) >= 0)
        {
            throw new ArgumentException(
                "File name cannot contain a path.",
                nameof(fileName));
        }
    }

    private static string NormalizeRequired(
        string value,
        int maximumLength,
        string parameterName,
        string displayName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                $"{displayName} is required.",
                parameterName);
        }

        string normalizedValue =
            value.Trim();

        if (normalizedValue.Length > maximumLength)
        {
            throw new ArgumentException(
                $"{displayName} cannot exceed {maximumLength} characters.",
                parameterName);
        }

        return normalizedValue;
    }

    private static string? NormalizeOptional(
        string? value,
        int maximumLength,
        string parameterName,
        string displayName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        string normalizedValue =
            value.Trim();

        if (normalizedValue.Length > maximumLength)
        {
            throw new ArgumentException(
                $"{displayName} cannot exceed {maximumLength} characters.",
                parameterName);
        }

        return normalizedValue;
    }
}
