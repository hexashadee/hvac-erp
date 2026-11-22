# Current implementation status

This is based on the source code snapshot reviewed November 22, 2026. It describes what is present in the repository, not a completed build or deployed environment.

## Repository today

The backend has separate domain, application, infrastructure and API projects using C# and .NET 10.

The API currently includes these endpoints:

`GET /` returns basic application information and version.

`GET /health` includes a PostgreSQL health check.

`GET /api/system/database` checks database connectivity.

`GET /api/dispatch/schedules` retrieves dispatch schedules and assigned technicians.

Dispatch is the first area with an EF Core persistence configuration, an initial migration and a read endpoint. The schedule query includes scheduling times, arrival windows, time zones, status, notes and technician assignments.

Domain models are present for companies, branches, users, memberships, roles, permissions, customers, locations, equipment, documents, work orders, checklists, signatures and dispatch scheduling. There is also permission resolution logic in the application layer.

Docker configuration, a local PostgreSQL Compose environment and automated test projects are included. The test code covers parts of the domain, application logic and API. Tests and deployment were not executed as part of this review.

## Still needed

There is no user facing frontend or working login flow in this snapshot.

The existing permission resolver is not yet enforced across the API. Company membership checks, tenant isolation and Protected Access challenges still need implementation and testing.

Customer and work order domain models exist, but complete persistence and API operations for creating, updating and retrieving these records are not established. The dispatch endpoint is currently read only.

Inventory, purchasing, finance, reporting, production hosting, monitoring, tested backups and migration are also outstanding.

## Security notes

The dispatch schedule endpoint has no visible authentication or company filtering in the reviewed code. It should not expose real business records until those checks are implemented.

The database connectivity endpoint also needs its intended production access reviewed.

Having a domain model does not mean the feature is fully available. Database operations, authorization, API behavior, interface and tests need to be verified separately.

## Next

Complete authentication and company isolation, then connect customer, work order and scheduling operations into one usable workflow.

Run the full test suite, verify database migrations and complete the workflow through a user interface before treating it as ready for an internal pilot.
