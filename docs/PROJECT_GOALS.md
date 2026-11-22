# Project goals

NullSupport ERP is a modular operations platform being developed under NullSupport LLC. The goal is to bring customer records, equipment, work orders, dispatch and the surrounding business functions into one system.

The platform is being structured for multiple companies, each with separate records, users, permissions and configurable workflows.

## Goals

1. Complete a customer to work order to dispatch workflow, including scheduling, technician assignment, status changes and activity history.
2. Give each company its own administration controls for employee accounts, roles, individual permissions and available modules or workflows.
3. Enforce access through the API. Authentication, company membership and role permissions should all be checked before granting access to records or actions.
4. Support Protected Access for sensitive areas, with additional authentication where required.
5. Keep operational history and audit logs, with backups and recovery tested before production use.
6. Keep the platform modular so inventory, purchasing, quotes, invoicing, reporting and integrations can be added as development progresses.

## Initial scope

The first usable workflow needs customer and location records, work order creation, dispatch scheduling, technician assignment and the ability to retrieve and update the records.

Employee login, company and branch permissions, administrative controls and audit logging are part of that initial scope. A user interface is also required before the platform can be tested as an operational system.

## Later scope

Inventory, purchasing, quotes, invoices, accounting integrations, reporting and automation will be developed around the core workflows.

The longer term goal is to support other businesses without maintaining separate versions of the application for each company. Commercial availability is a future direction, not a current release.
