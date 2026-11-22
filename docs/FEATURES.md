# Feature inventory

Tracking what exists in the repository separately from what is usable. A defined model is not necessarily a completed feature.

## Foundation

**Companies and branches:** Domain models exist. Persistence and secured API operations still needed.

**Employees and memberships:** Domain models exist. Login, account management and company membership enforcement still needed.

**Roles and permissions:** Domain models and permission resolution logic exist. Administrator controls, stored assignments and API enforcement still needed.

**Protected Access:** Planned. Requires additional authentication for selected resources or actions, along with access checks and audit records.

**Audit history:** Some record level fields exist. Complete audit logging for sensitive activity and permission changes is still needed.

## Operations

**Customers, contacts and locations:** Domain models exist. Database operations, API endpoints and interface still needed.

**Equipment:** Domain models exist. Persistence and links to service records still needed.

**Work orders:** Domain models and business rules exist. Persistence, create and update endpoints, workflow transitions and interface still needed.

**Checklists, documents and signatures:** Domain models exist. Storage, access controls and user workflows still needed.

**Dispatch scheduling:** EF Core configuration, an initial migration and read API exist. Authentication, company filtering, create and update operations and scheduling interface are still needed.

## Later modules

**Inventory and purchasing:** Planned.

**Quotes and invoicing:** Planned.

**Reporting and integrations:** Planned beyond the existing basic API and database diagnostics.

**Subscriptions and company onboarding:** Future commercial phase.

## Deployment

Docker and local PostgreSQL configuration are present. Production hosting, automated deployment, monitoring and backup recovery testing have not been verified.

A feature should be marked complete when its actual workflow, persistence, permissions and tests have been checked, not just when the models exist.
