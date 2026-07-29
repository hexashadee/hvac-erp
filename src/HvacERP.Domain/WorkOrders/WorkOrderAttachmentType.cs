namespace HvacERP.Domain.WorkOrders;

public enum WorkOrderAttachmentType
{
    General = 1,
    BeforePhoto = 2,
    AfterPhoto = 3,
    DiagnosticPhoto = 4,
    EquipmentPhoto = 5,
    CustomerProvided = 6,
    TechnicianProvided = 7,
    Receipt = 8,
    InspectionReport = 9,
    Manual = 10,
    Signature = 11,
    Other = 99
}
