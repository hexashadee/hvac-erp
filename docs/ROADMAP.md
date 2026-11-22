# Delivery roadmap

This is an order of work and release gates, not a promised schedule. Duration depends on contributors, priority, requirements, and results of the initial build audit.

## Stage 0: Baseline and safety

* Restore, compile, and execute the full .NET solution and tests.
* Start PostgreSQL and apply the existing migration in a development environment.
* Confirm whether any frontend or deployment components exist outside this repository.
* Establish GitHub branch protection, code review, automated build, tests, scanning, and environment configuration.
* Block access to operational data via unauthenticated and unscoped endpoints.

**Gate:** Reproducible development build, green tests, protected repository, and no exposed real business data.

## Stage 1: Identity and tenant boundaries

* Integrate an identity provider such as the planned Amazon Cognito.
* Persist companies, branches, users, memberships, roles, and permissions.
* Derive company context from a validated identity and membership, not a caller supplied company identifier alone.
* Enforce authorization across all relevant API paths and data queries.
* Write automated tests proving one company cannot access another company's records.

**Gate:** Authenticated requests, role based controls, and demonstrated tenant isolation.

## Stage 2: First operational workflow

* Persist customers, customer locations, equipment, work orders, and schedule assignments.
* Add APIs to create, update, view, and search operational records.
* Implement a usable web UI covering customer lookup, job creation, dispatch, technician assignment, and progress updates.
* Add audit events for important changes and minimum operational reporting.

**Gate:** HYVAC reviewers complete an agreed customer to work order to dispatch scenario without manual database intervention.

## Stage 3: HYVAC pilot readiness

* Confirm HYVAC process requirements and prioritized Blue Streak workflows.
* Prepare data mapping, test imports, reconciliation, user training, and cutover planning.
* Configure staging and production hosting, HTTPS, secrets, monitoring, alerts, backups, tested restoration, and rollback.
* Complete security, performance, and user acceptance testing.
* Run a small pilot, resolve blockers, and expand only when stakeholders approve.

**Gate:** Approved pilot acceptance, security and recovery verification, operational ownership, and documented rollback plan.

## Stage 4: Broader operational modules

Develop inventory, purchasing, quoting, invoicing, accounting integrations, exports, and automation according to measured HYVAC demand. Prioritize business dependencies instead of attempting every feature at once.

## Stage 5: Commercial SaaS readiness

* Confirm IP ownership and commercial licensing rights.
* Finalize tenant provisioning, isolation testing, account lifecycle, feature packaging, and subscription billing.
* Add self service or assisted onboarding, support workflows, and contractual terms.
* Validate demand with other contractors and run an external beta before broad sales.

**Gate:** Isolated external tenants, customer support process, secure billing, agreements, and repeatable onboarding.

## Timeline assumptions to validate

Earlier discussions considered a demonstration within roughly 1 to 2 months, an internal pilot in 4 to 6 months, and a broader rollout in 6 to 9 months **assuming dedicated development resources**. These are preliminary scenarios, not dates or commitments. The actual code audit and stakeholder scope may materially change them.
