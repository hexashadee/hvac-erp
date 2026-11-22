# Feature inventory

This tracks the modules and how they fit together. The [status page](STATUS.md) covers the current code; this page is mainly for how the functionality will work as the platform is built out.

## Companies, users and security

**Companies and branches:** Each company will have its own settings, branches, records and administrators. PostgreSQL records will carry the correct company identifier. API queries and background processes will apply that boundary consistently.

**Employees and memberships:** Authentication will establish identity, then the application will load the user's company membership and applicable branch access. Company administrators will manage accounts and role assignments.

**Roles and permissions:** The existing permission model will be connected to stored assignments and API authorization. Access can be scoped by company, branch, department, user and action. Administrators can configure which modules and workflows are available to their company.

**Protected Access:** A company administrator can require additional authentication for selected resources and sensitive actions. Normal permissions are checked first, followed by the Protected Access challenge. Successful access has a limited duration and all attempts are logged.

**Audit history:** Permission changes, protected actions, exports and material record edits will create audit events with the actor, time, resource and result.

## Core operations

**Customers, contacts and locations:** Store customer profiles and multiple contacts or service locations. Add searchable history and controls for record creation, updates and archived records.

**Equipment:** Link equipment to a location, customer and service records. Keep identifiers, documents, maintenance history and job associations together.

**Work orders:** Add creation, assignment, status transitions, labor, materials, notes, documents, checklists and completion records. Preserve history rather than silently replacing completed work.

**Dispatch:** Extend the existing schedule records and API into secured create, change and assignment operations. The interface will show technician availability, scheduled work and changes to assignments.

**Documents and signatures:** Store files separately from business records with permission checked links. Associate attachments, captured signatures and completion evidence with the correct work order or equipment.

## Business modules

**Inventory and purchasing:** Track stock locations, item movements, suppliers, purchase orders, receipts and adjustments. Quantities will come from recorded transactions so changes can be traced.

**Quotes and invoices:** Connect estimates, approvals, work orders and invoices through explicit states. Financial corrections will use recorded adjustments instead of deleting historical transactions.

**Reports and exports:** Use company scoped queries and permission checked exports. Add configurable reports after the underlying records are consistent.

**Notifications and integrations:** Business events will trigger messages and connector jobs. Each integration will need its own credentials, allowed operations, retry handling and audit history.

## Manufacturing and quality

The [industry questionnaire](INDUSTRY_ERP_QUESTIONNAIRE.md) adds the requirements for more specialized operations.

**Lots and batches:** Create lot identifiers, movements and parent child relationships for split or combined lots. Retain the complete material history from receipt to shipment.

**Production:** Add production orders, routing, work centers and capacity based schedules. Shop activity entries will update the current work in progress state.

**Quality and certificates:** Link inspection results, nonconformances, corrective actions, revisions and certificates to jobs, lots and equipment.

**Equipment and process records:** Add maintenance and calibration schedules, instrument history and process measurements through separate records attached to the relevant jobs and equipment.

The [industry requirements plan](INDUSTRY_REQUIREMENTS_PLAN.md) has the proposed build order. Specific process rules will be validated before the related implementation is finalized.

## Production use

The application will be deployed with encrypted connections and storage, monitored backups, tested recovery procedures, access logs, release checks and rollback instructions. Company onboarding and data portability will use the same tenant security rules as the rest of the ERP.
