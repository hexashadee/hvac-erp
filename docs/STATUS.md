# Development status

NullSupport ERP is in backend development. This page tracks what the current foundation supports and what each part will be connected to next. It is not a release announcement.

## Backend and database

The solution is built around C# and .NET 10, with separate Domain, Application, Infrastructure and API projects. PostgreSQL is configured through EF Core and Npgsql. Docker and a local PostgreSQL Compose environment are included.

The API has basic application and database health endpoints, plus a dispatch schedule read endpoint. Dispatch schedules and technician assignments have EF Core configuration and an initial migration.

The next step here is to extend the data layer for customers, locations, equipment and work orders. Those records will be connected through authenticated API operations, including creation, changes, status transitions and history.

## Company access

Company, branch, membership, role and permission models are part of the domain. Permission resolution logic is in the application layer.

Authentication will connect signed in users to their company memberships. The API will then verify the company boundary, role permissions and any individual restrictions before allowing access to a resource or action.

Company administrators will manage users and available workflows through their own controls. Protected Access will provide a separate authentication challenge for selected sensitive operations, with an expiry and audit record. These checks will be covered by isolation and permission tests.

## Main ERP workflow

Customer, equipment, document, work order, checklist, signature and dispatch concepts are modeled in the backend. The next implementation pass connects them to persistent records, business operations and the frontend.

The first end to end test will cover creating a customer, opening a work order, assigning work, updating the schedule and closing the job. The record history should still be available afterward, with permissions enforced at every step.

The frontend will be built around those operations first. Admin controls, customer records, work orders and the dispatch view should share the same API and permission behavior.

## Additional modules

Inventory and purchasing will use stock transactions, vendor records, purchase orders and recorded movements. Quotes and invoices will connect to customers and work orders through controlled status changes. Reports will read company scoped records, with separate permissions for sensitive exports.

The industry requirements extend that design into lot and batch traceability, production schedules, quality checks, process specifications and certification records. The implementation approach is in the [industry requirements plan](INDUSTRY_REQUIREMENTS_PLAN.md), with the full responses in the [questionnaire](INDUSTRY_ERP_QUESTIONNAIRE.md).

## Testing and release

The repository includes .NET test projects covering portions of the domain, application and API. The next testing pass will exercise database migrations, authenticated operations, company isolation and the complete operational workflow.

Production preparation will cover encrypted connections and storage, audit logs, monitored backups, restore exercises, deployment checks, logging and a rollback procedure. Import and export workflows will include validation and reconciliation before any data is moved between systems.

No production date has been set. Development is being tracked against working workflows and verification, rather than an arbitrary percentage of completion.

See [features](FEATURES.md) for the module plan and [roadmap](ROADMAP.md) for implementation order.
