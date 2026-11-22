# Development roadmap

The order here is based on dependencies. There is not much value in building ten separate modules before the first customer record can move through a complete work order and dispatch process.

## 1. Finish the platform foundation

Use the existing .NET solution and PostgreSQL setup as the base. Run the tests, apply the current migrations and verify the dispatch API with the local database. Then add consistent validation, error handling and repeatable setup for future modules.

At the same time, connect authentication to company memberships. Roles and individual permissions will be checked by the API on every protected action. A user will only work within the company and branch access assigned to them.

Company admins will have controls for user accounts, permissions, modules and workflows. Protected Access will add a fresh authentication challenge where an admin marks a resource as sensitive. Challenge results will expire and be audited.

**First checkpoint:** Sign in, administer company access, and prove through tests that another company's records and restricted actions cannot be accessed.

## 2. Connect the core ERP operations

Complete the persistence and API operations for customers, contacts, locations, equipment and work orders. Extend dispatch beyond schedule retrieval to creation, technician assignments, changes and status tracking.

Build the frontend around the same process. The first useful flow is customer creation through work order completion, including notes, attachments and a readable history of changes.

**Next checkpoint:** Complete a work order from start to finish in the interface, with saved records and permission checks throughout.

## 3. Add business modules

Inventory will track items, stock locations and transactions. Purchasing will use vendors and purchase orders. Quotes will have approval states and be able to feed work orders. Invoicing will use explicit financial states and retain correction history. Reports will pull from company scoped data and enforce export permissions.

Notifications and integrations will use background jobs, retry handling and logging. Imports will validate records before writing to the main database. Exports will allow a company to retrieve its own records in a documented format.

## 4. Build out industry modules

The [industry questionnaire](INDUSTRY_ERP_QUESTIONNAIRE.md) introduces quality management, production control and specialized equipment workflows.

Start with lot identifiers and movement history, production orders and routing, equipment capacity scheduling, quality inspections and specification revisions. Certificates, calibration records and more specialized process measurements will attach to those same records.

The exact fields and process rules will be checked against real operating examples before implementation. See [industry requirements plan](INDUSTRY_REQUIREMENTS_PLAN.md).

## 5. Production and commercial preparation

Set up production hosting, encryption, monitoring, audit storage, backups and tested restores. Test failure behavior, deployment rollback and access revocation. Then run acceptance testing against representative records and workflows.

For multiple customer companies, add self contained onboarding, administrator setup, configuration, data import and export, subscription management and support procedures.

**Release checkpoint:** A release is ready when the workflows, security checks, backups and recovery procedures work in the intended environment. Dates will follow the actual verification results.
