# Project goals

NullSupport ERP is a modular ERP platform being developed by NullSupport LLC.

The idea is to bring company operations into one system without forcing every business to work the same way. Companies and branches, employees, customer records, locations, equipment, work orders and scheduling are the starting point. Purchasing, inventory, quotes, invoices, reporting and integrations will follow.

Each company will have its own records, administrators, permissions and workflows. The system should be configurable without needing a separate version of the application for every company.

## Company administration

A designated administrator should be able to manage employees, assign roles, change individual permissions and choose which modules and workflows the company uses.

Permissions will apply to the company, branch, department and individual function. The API will check those permissions whenever a user tries to view, create, modify or approve something. Hiding a button in the frontend is not a security control.

There will also be a Protected Access layer. An administrator can add an additional authentication requirement to a sensitive page, record or action. This is checked after normal permissions, not instead of them. Successful challenges expire, and the access attempts are logged.

## Operational workflow

The first complete process will connect customers, locations, equipment, work orders and dispatch.

1. Create a customer and location.
2. Associate equipment and open a work order.
3. Assign technicians, set a schedule and record changes.
4. Update the work order through completion, keeping notes, documents and status history together.
5. Retrieve the records with the appropriate company and employee permissions.

The same records can then support quotes, purchasing, invoices and reporting without copying information into unrelated systems.

## Data and security

Company separation is part of the data model and API design. Every company-owned record will have a company boundary, and access checks will apply to queries, exports, attachments, automated jobs and integrations.

Activity history will cover permission changes, Protected Access events and sensitive record changes. Backups, restore testing, monitoring and rollback procedures are part of the production requirements, not features to add later if something fails.

## Expansion

The platform will use separate modules with shared authorization and data rules. Inventory, purchasing, financial workflows and reporting can be added without restructuring the main customer and work order process.

The [industry questionnaire](INDUSTRY_ERP_QUESTIONNAIRE.md) also covers manufacturing and quality requirements. Lot traceability, production routing, equipment calibration, certificates and similar processes can be implemented as specialized modules using the same company boundaries and permission system.

The longer term direction is commercial use by multiple independent companies. The [roadmap](ROADMAP.md) covers development order, and [commercialization](COMMERCIALIZATION.md) covers what operating the platform for outside customers would involve.
