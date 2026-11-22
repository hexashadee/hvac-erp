namespace NullSupportERP.Domain.WorkOrders;

public enum WorkOrderType
{
    ServiceCall = 1,
    PreventiveMaintenance = 2,
    Installation = 3,
    Inspection = 4,
    Warranty = 5,
    Estimate = 6,
    Callback = 7,
    Emergency = 8,
    Other = 99
}
