namespace HvacERP.Domain.WorkOrders;

public enum WorkOrderStatus
{
    Draft = 1,
    Open = 2,
    Scheduled = 3,
    Dispatched = 4,
    InProgress = 5,
    OnHold = 6,
    Completed = 7,
    Cancelled = 8
}
