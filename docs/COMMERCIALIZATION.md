# Future commercialization strategy

## Proposed direction

HYVAC is the intended initial deployment and potential design partner. If the platform proves operationally useful, the same product could be offered to other HVAC businesses as subscription software.

This is an **aspiration**, not an existing product launch, customer contract, or revenue forecast.

## Architecture required for SaaS

* Company based tenant boundaries in the database, application services, APIs, searches and exports.
* Authentication and membership enforcement at every entry point.
* Tenant specific users, roles, branch structure, settings and operational data.
* Provisioning, suspension, deletion and export processes.
* Centralized observability that does not leak one customer's sensitive data to another.
* Configurable workflows without maintaining a separate fork for each company.
* Audited administrator access, support permissions, backups and restoration.

## Product validation

Interview HVAC contractors other than HYVAC to understand business problems, software alternatives, required integrations, data migration hurdles and purchasing willingness. Pilot the smallest valuable module set before expanding into a full ERP replacement.

## Revenue strategy

A monthly subscription by company size, user count, enabled modules, or usage is a possible model. Prior illustrative price points of $199, $499, and $999 per month are **hypotheses only**, not validated pricing or expected revenue. Include implementation, data migration, support, cloud costs, sales and payment fees in pricing analysis.

## Legal and ownership decisions

Resolve in writing who owns existing and future source code, whether HYVAC has an exclusive or nonexclusive license, what rights attach to work performed with HYVAC time or resources, and who can commercialize or resell the product. Review use of confidential Blue Streak information, software design, proprietary documentation, trade secrets and third party components. Do not use production HYVAC customer records for external demonstrations without authorization and proper sanitization.

## External launch requirements

Independent security assessment, tenant isolation tests, operating policies, support terms, privacy disclosures, backup and disaster recovery planning, onboarding, contracts and subscription billing all need approval before paid customers depend on the service.
