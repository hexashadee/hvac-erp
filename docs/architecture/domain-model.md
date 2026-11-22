# Domain model

## Scope

These are the data model notes for NullSupport ERP. They cover the main records, their relationships and the rules the application will enforce as the modules are connected.

The design is intentionally wider than the first database migration. The [status document](../STATUS.md) covers the backend foundation, while this file records the structure the platform is being built around.

NullSupport ERP is intended to support separate companies using the same platform with their own records, permissions and configurable workflows.

Each tenant is represented by a Company. Company data must remain isolated from every other company.

The broader design needs to support:

* Multiple companies
* Multiple branches per company
* Company hierarchy
* Users assigned to one or more branches
* Roles and permissions
* Protected Access for sensitive records and actions
* Customers and service locations
* Equipment and service history
* Work orders and dispatching
* Quotes and invoices
* Inventory and purchasing
* Notifications
* Complete audit history

## Tenant Boundary

### Company

Company is the primary tenant and security boundary.

Almost every business record must include a CompanyId.

Examples:

* Branch
* User membership
* Customer
* Service location
* Equipment
* Work order
* Quote
* Invoice
* Inventory item
* Vendor
* Permission assignment
* Protected access policy

A record belonging to one company must never be accessible by another company.

## Company Structure

### Company

Represents a company using the platform.

Proposed properties:

* Id
* Name
* Slug
* Legal name
* Primary email
* Primary phone
* Time zone
* Default currency
* IsActive
* CreatedAtUtc
* CreatedByUserId
* LastModifiedAtUtc
* LastModifiedByUserId

Responsibilities:

* Maintain company identity
* Control active status
* Own branches
* Own company-level settings
* Provide the tenant boundary

### Branch

Represents a physical office, warehouse, or operational division.

Proposed properties:

* Id
* CompanyId
* Name
* Code
* Email
* Phone
* Time zone
* Address
* IsPrimary
* IsActive
* CreatedAtUtc
* CreatedByUserId
* LastModifiedAtUtc
* LastModifiedByUserId

Planned rules:

* Every branch belongs to one company
* A company must have at least one branch
* A company can have only one primary branch
* Branch codes must be unique inside a company
* Deactivated branches cannot receive new work orders
* Historical records remain attached to deactivated branches

### Department

Represents a functional unit inside a company or branch.

Examples:

* Service
* Installation
* Dispatch
* Sales
* Accounting
* Warehouse
* Administration

A department may belong to the whole company or to a specific branch.

## Users and Access

### User

Represents a person with an account.

Authentication will use an identity provider. The provider has not been finalized.

The ERP stores the business profile and access relationships.

Proposed properties:

* Id
* ExternalIdentityId (proposed)
* FirstName
* LastName
* Email
* Phone
* IsActive
* LastLoginAtUtc

A user account does not directly own company data.

Access is granted through company membership.

### CompanyMembership

Connects a user to a company.

Proposed properties:

* Id
* CompanyId
* UserId
* EmployeeNumber
* JobTitle
* EmploymentStatus
* IsCompanyOwner
* IsActive
* HireDate
* TerminationDate

Planned rules:

* A user may belong to multiple companies
* Membership permissions are isolated by company
* Disabling membership removes company access without deleting the user
* Historical activity remains attributed to the user

### BranchAssignment

Connects a company member to one or more branches.

Proposed properties:

* Id
* CompanyMembershipId
* BranchId
* IsPrimary
* IsActive

Planned rules:

* A member may be assigned to multiple branches
* A member can have only one primary branch per company
* Branch assignments cannot cross company boundaries

### Role

A named collection of permissions.

Examples:

* Company Owner
* Administrator
* Dispatcher
* Service Manager
* Technician
* Installer
* Sales Representative
* Accountant
* Warehouse Manager
* Read Only

Roles may be system-defined or company-created.

### Permission

Represents one specific allowed action.

Permission naming format:

`module.resource.action`

Examples:

* `customers.customer.view`
* `customers.customer.create`
* `workorders.workorder.assign`
* `workorders.workorder.close`
* `billing.invoice.void`
* `security.user.manage`
* `payroll.employee.view`

Permissions should be granular and not tied directly to UI pages.

### RolePermission

Connects a role to a permission.

### MembershipRole

Assigns a role to a company membership.

Role assignments may optionally be restricted to:

* Entire company
* Specific branch
* Specific department

### PermissionOverride

Allows an administrator to explicitly allow or deny an individual permission.

Explicit deny should override role-based allow.

## Protected Access

### ProtectedAccessPolicy

Adds an additional security wall to sensitive resources.

A policy may protect:

* Module
* Page
* Widget
* Record
* Report
* Field
* Action
* Export
* API operation

Proposed properties:

* Id
* CompanyId
* Name
* ResourceType
* ResourceKey
* ProtectionMethod
* ReauthenticationDurationMinutes
* IsActive

Protection methods may include:

* Additional password
* Reauthentication through the chosen identity provider
* MFA challenge
* Administrator approval

### ProtectedAccessGrant

Records which users or roles may attempt to unlock a protected resource.

### ProtectedAccessSession

Represents a temporary successful unlock.

Proposed properties:

* Id
* CompanyId
* UserId
* ProtectedAccessPolicyId
* GrantedAtUtc
* ExpiresAtUtc
* GrantedByUserId
* RevokedAtUtc

Planned rules:

* Normal permission is still required
* Protected access does not replace authorization
* Access expires automatically
* Administrators can revoke active sessions
* Every attempt must be audited
* Passwords must never be stored as plain text

Authorization flow:

1. Confirm company membership
2. Confirm normal permission
3. Check for a protected access policy
4. Confirm an active protected access session
5. Allow or deny the requested operation
6. Record the result in the audit log

## Customers

### Customer

Represents the person or business receiving services. The current code uses `Customer` and `CustomerLocation`; some names below are conceptual and may differ from the implemented types.

Customer types:

* Residential
* Commercial
* Property Management
* Government
* Internal

Proposed properties:

* Id
* CompanyId
* CustomerNumber
* CustomerType
* DisplayName
* BillingEmail
* BillingPhone
* BillingAddress
* TaxExempt
* IsActive

Planned rules:

* Customer numbers must be unique inside a company
* Deactivated customers retain historical records
* A customer may have multiple contacts
* A customer may have multiple service locations

### CustomerContact

Represents a person associated with a customer.

Examples:

* Homeowner
* Tenant
* Property manager
* Accounts payable contact
* Site manager

### ServiceLocation

Represents a customer location where work occurs.

Proposed properties:

* Id
* CompanyId
* CustomerId
* Name
* Address
* AccessInstructions
* ServiceNotes
* Latitude
* Longitude
* IsActive

Planned rules:

* A service location belongs to one customer
* A customer may have multiple service locations
* Work orders are performed at a service location

## Equipment

### Equipment

Represents equipment associated with a customer location.

Examples:

* Furnace
* Air conditioner
* Boiler
* Heat pump
* Rooftop unit
* Air handler
* Thermostat
* Humidifier

Proposed properties:

* Id
* CompanyId
* CustomerId
* ServiceLocationId
* EquipmentType
* Manufacturer
* ModelNumber
* SerialNumber
* InstallationDate
* WarrantyExpirationDate
* Status
* Notes

### EquipmentServiceRecord

Represents historical service performed on equipment.

It may be generated from a completed work order.

### EquipmentDocument

Stores references to manuals, photos, warranty records and other files in object storage. The storage provider is still to be selected.

## Work Orders

### WorkOrder

Represents requested or scheduled work.

Proposed properties:

* Id
* CompanyId
* BranchId
* CustomerId
* ServiceLocationId
* WorkOrderNumber
* WorkOrderType
* Priority
* Status
* Summary
* Description
* ScheduledStartUtc
* ScheduledEndUtc
* CompletedAtUtc
* CancelledAtUtc

Suggested status lifecycle:

* Draft
* Unscheduled
* Scheduled
* Dispatched
* EnRoute
* InProgress
* OnHold
* Completed
* Cancelled

Planned rules:

* Work order numbers must be unique inside a company
* A work order must belong to a customer and service location
* Completed work orders cannot be silently edited
* Reopening a completed work order requires permission and audit logging
* Cancellation requires a reason
* Required completion information must exist before closing

### WorkOrderAssignment

Assigns technicians or crews to a work order.

### WorkOrderEquipment

Connects equipment to a work order.

### WorkOrderNote

Stores internal or customer-visible notes.

### WorkOrderLabor

Records labor time.

### WorkOrderMaterial

Records materials used.

### WorkOrderPhoto

References before, during and after photos stored in object storage.

### WorkOrderStatusHistory

Records every status transition.

### CustomerSignature

Stores customer approval or completion acknowledgement.

## Quotes and Invoices

### Quote

Represents proposed work and pricing.

Suggested statuses:

* Draft
* Sent
* Viewed
* Approved
* Rejected
* Expired
* Converted

### QuoteLine

Represents labor, materials, equipment, discounts, or fees.

### Invoice

Represents a financial charge to a customer.

Suggested statuses:

* Draft
* Issued
* PartiallyPaid
* Paid
* PastDue
* Void

### InvoiceLine

Represents an individual charge.

### Payment

Represents a payment applied to an invoice.

Financial records should never be physically deleted after issuance.

Corrections should use:

* Void
* Credit
* Adjustment
* Reversal

## Inventory and Purchasing

### InventoryItem

Represents a tracked material, part, or product.

### InventoryLocation

Examples:

* Branch warehouse
* Technician vehicle
* Storage room
* Job site

### InventoryTransaction

Records every increase, decrease, transfer, reservation, or adjustment.

### Vendor

Represents a supplier.

### PurchaseOrder

Represents an order placed with a vendor.

### PurchaseOrderLine

Represents an item ordered from a vendor.

Inventory quantities should be calculated from transactions rather than overwritten without history.

## Notifications

### Notification

Represents a message sent to a user.

Notification channels may include:

* In-app
* Email
* SMS
* Push notification

### NotificationPreference

Stores a user’s preferences for each event and channel.

### NotificationTemplate

Stores company-customizable message templates.

## Audit Logging

### AuditLog

Records meaningful security and business events.

Proposed properties:

* Id
* CompanyId
* UserId
* Action
* ResourceType
* ResourceId
* BeforeData
* AfterData
* IpAddress
* UserAgent
* OccurredAtUtc
* Result

Events to audit include:

* Login attempts
* Permission changes
* Role assignments
* Protected access attempts
* Customer changes
* Work-order status changes
* Invoice changes
* Data exports
* Record deletion or archival
* Administrator actions

Audit records should be append-only.

## Initial Aggregate Roots

These are the aggregate roots proposed for the wider platform:

* Company
* Branch
* Customer
* ServiceLocation
* Equipment
* WorkOrder
* Quote
* Invoice
* InventoryItem
* Vendor
* PurchaseOrder
* ProtectedAccessPolicy

Not every entity should be an aggregate root.

Examples of child entities:

* CustomerContact
* QuoteLine
* InvoiceLine
* WorkOrderNote
* WorkOrderLabor
* WorkOrderMaterial
* PurchaseOrderLine

## Initial Implementation Order

1. Company
2. Branch
3. CompanyMembership
4. BranchAssignment
5. Role
6. Permission
7. ProtectedAccessPolicy
8. Customer
9. CustomerContact
10. ServiceLocation
11. Equipment
12. WorkOrder
13. WorkOrderAssignment
14. WorkOrderStatusHistory
15. Quote
16. Invoice
17. Inventory
18. Vendors and purchasing
19. Notifications
20. Audit logging

## Cross-Cutting Rules

* Every tenant-owned record must contain CompanyId
* IDs use Guid values
* Dates are stored in UTC
* User-facing dates use the company or user time zone
* Business objects cannot be created in invalid states
* Sensitive operations require explicit permissions
* Protected Access supplements normal permissions
* Financial and audit records are not physically deleted
* Historical relationships must remain traceable
* All data access must enforce tenant isolation

## Specialized operations

The industry questionnaire includes requirements beyond the current customer and work order models. They would be separate modules, rather than fields added to every existing record.

**Lots and batches:** A lot identifier, parent and child relationships, quantity changes and movement events. This would allow traceability through receiving, processing, rework and shipment.

**Production:** Production orders, routing steps, work centers and equipment or load schedules. Job progress should be recorded as events, with capacity and process restrictions checked when work is assigned.

**Quality and certificates:** Inspection results, specification revisions, nonconformance records, corrective actions and certificates tied back to the appropriate job, lot or equipment.

The detailed questions and suggested implementation approach are in the [industry questionnaire](../INDUSTRY_ERP_QUESTIONNAIRE.md). The implementation approach is described in the [industry requirements plan](../INDUSTRY_REQUIREMENTS_PLAN.md).
