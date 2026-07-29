namespace HvacERP.Domain.Documents;

public enum DocumentStatus
{
    PendingUpload = 1,
    Available = 2,
    UploadFailed = 3,
    Quarantined = 4,
    Deleted = 5
}
