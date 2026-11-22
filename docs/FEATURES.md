# Feature inventory

The labels below distinguish source code implementation from functional completion. **Domain model exists** means types and domain behavior exist, not a usable end to end feature.

| Capability | Current evidence | Next requirement | Priority |
| --- | --- | --- | --- |
| Company and branch entities | Domain model exists | Persist, validate and expose through secured APIs | Critical |
| Users and company membership | Domain model exists | Connect identity provider and enforce membership | Critical |
| Roles and permissions | Domain types and resolver exist | Persist assignments and enforce on every endpoint | Critical |
| Protected Access | Architectural specification | Implement policy store, challenge, expiry, audits and tests | High |
| Customers and contacts | Domain model exists | Persistence, API operations, UI, tenant checks | Critical |
| Service locations | Domain model exists | Persistence, API operations, UI | Critical |
| Equipment | Domain model exists | Persistence, linked service history | High |
| Work orders | Extensive domain model exists | Persistence, CRUD, transitions, UI and audit | Critical |
| Checklists, attachments, signatures | Domain model exists | Storage, APIs, authorizations and UI | High |
| Dispatch schedules | EF mapping, migration, read API and query exist | Authentication, tenant filter, writes and calendar UI | Critical |
| Audit history | Some domain audit fields | Durable application audit events and reporting | Critical |
| Reporting | Not established | Minimum operational views and exports | Medium |
| Inventory and purchasing | Architectural intent | Business requirements and implementation | Later |
| Quotes and invoices | Architectural intent | Business rules, persistence, integrations | Later |
| Frontend application | Not found | Implement authenticated workflow UI | Critical |
| SaaS subscriptions | Not found | Billing, provisioning, account lifecycle | Commercial phase |
| CI and deployment | Docker build configuration exists | Continuous validation, staging, release process | Critical |

## Completion rules

A feature is **completed** only when its backend, persistent data where appropriate, authorization, user experience, automated tests, and stakeholder acceptance have been verified. Source files alone do not satisfy this threshold.
