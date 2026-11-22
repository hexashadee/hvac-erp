# Implementation status

**Review basis:** Source snapshot from `hvac-erp-main.zip`, inspected November 22, 2026. This is a static source review, not a verified build, running environment, or production audit. No percentage complete is defensible without agreed feature acceptance criteria.

## Confirmed in source

| Area | Evidence | Interpretation |
| --- | --- | --- |
| Solution structure | `HvacERP.slnx` and Domain, Application, Infrastructure, API projects | Real modular C# foundation |
| API | `src/HvacERP.Api/Program.cs` | Root information endpoint, health endpoint, database connectivity endpoint, read only dispatch schedules endpoint |
| Domain | `src/HvacERP.Domain/` | Rich entity and business rule implementation for companies, branches, users, permissions, customers, equipment, documents, work orders, and dispatch |
| Authorization logic | `EffectivePermissionResolver.cs` and authorization tests | Permission resolver exists; authorization middleware and real authenticated API enforcement not demonstrated |
| PostgreSQL | `HvacErpDbContext.cs`, dispatch entity configurations and migration | Initial persisted dispatch schedules and assignments, not comprehensive ERP persistence |
| Dispatch query | `DispatchScheduleQueries.cs` | Read only schedule retrieval from the configured database |
| Testing | `tests/` | Automated test source exists for domain, permission resolver and API smoke scenarios; tests were not executed as part of this review |
| Packaging | `Dockerfile`, `compose.database.yml` | Container build specification and local PostgreSQL definition; deployment unverified |
| Architecture docs | `docs/architecture/domain-model.md` | Product direction and intended tenant boundaries documented, including entities not yet persisted or exposed by API |

## Not verified or not found in snapshot

* Web frontend and user facing workflows.
* Authentication and Cognito integration in the live API.
* Tenant identity derived from verified authentication context.
* Server side authorization and tenant scoping on current endpoints.
* API endpoints for creating or updating customers, work orders, or assignments.
* EF persistence for most domain entities.
* Protected Access challenge execution and full audit event persistence.
* Production cloud infrastructure, CI pipelines, monitoring and backup restore testing.
* HYVAC user acceptance testing, operational migration, or production cutover.

## Important risks and immediate fixes

1. Current `/api/dispatch/schedules` endpoint has no visible authentication or tenant filter in `Program.cs` or its query. **Do not expose it with real business data until authorization and isolation are enforced.**
2. `/api/system/database` exposes connection status. Determine whether operational diagnostic endpoints should be internal or authenticated in deployed environments.
3. The EF Core context currently defines dispatch schedule and schedule technician sets only. Business entities in the domain model are not equivalent to deployed persistent modules.
4. Planning documents previously disagreed on backend choice. The current implementation is .NET 10 and should be considered the baseline unless explicitly changed.
5. Local Compose credentials are development placeholders, never production credentials.
6. Verify actual build, test, migration, container build, and database behavior in a configured development environment before calling any module complete.

## Near term definition of progress

A credible first vertical slice is an authenticated user from a configured company who can create a customer, create a work order, schedule and assign it, view it from a web interface, and update its status with documented authorization and audit evidence. All tenant boundary tests must pass.
