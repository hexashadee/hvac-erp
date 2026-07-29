namespace HvacERP.Domain.WorkOrders;

public sealed class WorkOrderLineItem
{
    public Guid Id { get; private set; }

    public WorkOrderLineItemType Type { get; private set; }

    public string? ItemNumber { get; private set; }

    public string Description { get; private set; }

    public decimal Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public bool IsTaxable { get; private set; }

    public decimal LineTotal =>
        decimal.Round(
            Quantity * UnitPrice,
            2,
            MidpointRounding.AwayFromZero);

    internal WorkOrderLineItem(
        Guid id,
        WorkOrderLineItemType type,
        string? itemNumber,
        string description,
        decimal quantity,
        decimal unitPrice,
        bool isTaxable)
    {
        Id = id;
        Type = type;
        ItemNumber = itemNumber;
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
        IsTaxable = isTaxable;
    }

    internal void Update(
        WorkOrderLineItemType type,
        string? itemNumber,
        string description,
        decimal quantity,
        decimal unitPrice,
        bool isTaxable)
    {
        Type = type;
        ItemNumber = itemNumber;
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
        IsTaxable = isTaxable;
    }
}
