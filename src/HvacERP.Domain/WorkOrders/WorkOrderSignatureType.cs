namespace HvacERP.Domain.WorkOrders;

public enum WorkOrderSignatureType
{
    CustomerApproval = 1,
    WorkAuthorization = 2,
    WorkCompletion = 3,
    EstimateApproval = 4,
    PaymentAuthorization = 5,
    TechnicianCertification = 6,
    InspectorApproval = 7,
    Other = 99
}
