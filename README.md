# HVAC ERP

A modular enterprise resource planning platform being developed initially for HYVAC Technologies, with the longer term goal of serving other HVAC businesses as a commercial software product.

> **Status: backend foundation in development. Not production ready.** This summary describes what was observed in the repository snapshot reviewed November 22, 2026. It is not a verified deployment status. See [Project status](docs/STATUS.md).

## Why this exists

The objective is to give an HVAC business one place to manage companies and branches, employees and permissions, customers and locations, equipment, service work orders, dispatch schedules, and eventually purchasing, inventory, quotes, invoices, reporting, and integrations. HYVAC is the initial use case and prospective pilot customer. The longer term product goal is to support many independent companies with securely isolated records and configurable workflows.

The project may reproduce common *business workflows* found in existing HVAC ERP software, including Blue Streak, but the implementation, branding, design, and code must be original or properly licensed.

## Goals

1. Deliver a reliable internal pilot for HYVAC with a useful customer to work order to dispatch workflow.
2. Build granular, enforceable company and branch aware permissions and additional protection for sensitive actions.
3. Preserve operational history and provide dependable audit trails, backups, and recovery.
4. Keep the product modular so future inventory, purchasing, billing, reporting, and integrations fit without a rewrite.
5. Design tenant isolation early so an eventual subscription business is possible, without treating commercialization as already approved or launched.

## What is in this repository today

* A .NET 10 ASP.NET Core API with basic information, health, database connectivity, and read only dispatch schedule endpoints.
* Domain model code for organizations, memberships, roles, customers, equipment, documents, work orders, checklists, signatures, and dispatch scheduling.
* Application permission resolution abstractions and logic.
* EF Core PostgreSQL persistence configuration and an initial migration for dispatch schedules and assignments.
* Docker configuration, local PostgreSQL Compose file, architecture notes, and automated tests.

**Not established by this snapshot:** a user facing web application, a working login flow, API enforcement of tenant permissions, complete customer and work order persistence and CRUD endpoints, protected access challenges, inventory or finance modules, live production hosting, or completed HYVAC migration.

## Technology in code

| Layer | Current repository |
| --- | --- |
| API | ASP.NET Core and C# on .NET 10 |
| Domain and application | Separate C# projects |
| Data access | EF Core with Npgsql and PostgreSQL |
| Deployment foundation | Dockerfile and PostgreSQL Compose |
| Database cloud preparation | Optional AWS RDS IAM authentication code |
| Tests | .NET test projects for domain, application, and API |
| Frontend | Not present in this snapshot |

Earlier planning materials mention alternative JavaScript backends; the **actual committed code is .NET**, which should guide future implementation unless a formal architecture decision changes it.

## Key documentation

* [Project goals and boundaries](docs/PROJECT_GOALS.md)
* [Current implementation status](docs/STATUS.md)
* [Development roadmap](docs/ROADMAP.md)
* [Feature inventory](docs/FEATURES.md)
* [Commercial product strategy](docs/COMMERCIALIZATION.md)
* [Domain model](docs/architecture/domain-model.md)

## Developer starting point

Requires .NET 10 SDK and Docker with Compose for local PostgreSQL.

```powershell
# From the repository root
docker compose -f compose.database.yml up -d
dotnet restore HvacERP.slnx
dotnet test HvacERP.slnx
$env:HVAC_ERP_CONNECTION_STRING = 'Host=localhost;Port=5432;Database=hvac_erp;Username=hvac_erp;Password=hvac_erp_dev_password'
dotnet run --project src/HvacERP.Api
```

The Compose password is for **local development only**. Never reuse it in a public environment. Database migrations and API runtime verification still need to be performed in the intended environment. Never commit a real `.env` file or secrets.

## Release policy

No production launch date has been committed. A HYVAC pilot needs verified core business workflows, secured endpoints, data isolation tests, operational backups, observability, user acceptance testing, and a controlled migration and rollback plan. See the [roadmap](docs/ROADMAP.md) for gates, not promises.

## Ownership

Commercial licensing, source code ownership, use of HYVAC resources and data, and any rights required to integrate with existing software must be agreed in writing before marketing or selling this product.
