# Project goals

## Purpose

Create an original HVAC ERP system that initially supports HYVAC Technologies and can later evolve into a configurable commercial product for other HVAC contractors.

## Primary outcome

HYVAC staff should be able to manage a job from customer and location through work order creation, scheduling, technician assignment, activity history, and job completion without maintaining disconnected records.

## Product principles

1. **Operational usefulness first.** Build and verify one complete workflow before expanding to every module.
2. **Company isolation.** Each customer business owns and can access only its own data.
3. **Server enforced permissions.** UI visibility alone is not security. Enforce permissions and tenant context in APIs and persistence.
4. **Sensitive access requires stronger proof.** Planned Protected Access should add a time limited authentication challenge and audit records where appropriate.
5. **Traceable changes.** Changes to jobs, privileges, and significant records should be auditable.
6. **Modularity.** Extend the product with new modules without creating distinct codebases for each customer.
7. **Practical delivery.** Pilot HYVAC before promising a complete Blue Streak replacement.

## Initial HYVAC pilot scope

* Secure employee login and company membership.
* Company, branch, and role management sufficient for actual HYVAC staff.
* Customers, contacts, service locations, and equipment.
* Work order lifecycle, notes, assignments, and basic checklists.
* Dispatch schedule and technician assignment experience.
* Minimal dashboard or reports needed to operate and review work.
* Audit events, backup and restore proof, and error monitoring.

Actual HYVAC processes and data sources must be confirmed with stakeholders before these requirements become acceptance criteria.

## Later operational scope

Quoting, purchasing, inventory, invoices, accounting connections, field mobile usability, notifications, advanced reporting, and other modules are candidates for later releases. Prioritization depends on HYVAC's real workflows and integration dependencies.

## Long term commercial goal

Support subscription based access for independent HVAC companies. Each company should have isolated data, its own users, branch structure, roles, branding options, and configurable features. Commercial release also requires onboarding, billing, legal agreements, support, security review, and validation of actual customer willingness to pay.

## What success looks like

* Staff can complete agreed pilot workflows on a test environment and then in a carefully controlled production pilot.
* A user from one company cannot read or modify another company's data, demonstrated by automated integration tests.
* The service can be deployed, monitored, backed up, restored, and rolled back by documented procedures.
* The owner of the code and the rights to commercialize it are confirmed in writing.
* Potential external customers confirm the product solves a problem they would pay to address.
