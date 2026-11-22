# NullSupport ERP

Modular ERP platform being developed by NullSupport LLC.

The goal is to consolidate operations into one system, including companies and branches, employees, permissions, customers, locations, equipment, work orders and dispatch scheduling. Purchasing, inventory, quotes, invoicing, reporting and integrations will be added as development progresses.

The platform is being structured to support multiple companies independently, each with its own records, permissions and configurable workflows.

Some functionality will follow established industry workflows, using existing systems as a reference. The implementation, design and code will be original or appropriately licensed.

Each company will have its own administrative controls. Designated administrators will be able to manage employee accounts, assign roles, adjust individual permissions and configure which modules and workflows are available. Access can be controlled at the company, branch, department and individual function level, including who can view, create, modify or approve certain records and actions.

Security is being designed in layers, rather than relying on a single login or permission level. Authentication establishes user identity, followed by company membership verification, role based access controls and permissions for specific resources or actions. Protected Access adds another layer for sensitive areas, allowing additional authentication to be required before certain information or functions can be accessed.

These controls will be enforced through the API, not just the interface. Company data will remain isolated, administrative access will be limited to the appropriate organization, and sensitive actions and permission changes will be recorded through audit logging.

The goal is to give each company control over its users, permissions and workflows without compromising security or requiring changes to the underlying application.

## What is in this repository today

* A .NET 10 ASP.NET Core API with basic information, health, database connectivity, and read only dispatch schedule endpoints.
* Domain model code for organizations, memberships, roles, customers, equipment, documents, work orders, checklists, signatures, and dispatch scheduling.
* Application permission resolution abstractions and logic.
* EF Core PostgreSQL persistence configuration and an initial migration for dispatch schedules and assignments.
* Docker configuration, local PostgreSQL Compose file, architecture notes, and automated tests.

**Not established by this snapshot:** a user facing web application, a working login flow, API enforcement of tenant permissions, complete customer and work order persistence and CRUD endpoints, protected access challenges, inventory or finance modules, live production hosting, or completed HYVAC migration.

## Tech
API: ASP.NET Core / C# / .NET 10
Domain and application: Separate C# projects
Data access: EF Core / Npgsql / PostgreSQL
Deployment: Dockerfile and PostgreSQL Compose
AWS: Optional RDS IAM authentication code
Testing: .NET domain, application and API tests
Frontend: Not implemented yet

Earlier research included JavaScript backend options. The current codebase is .NET, so that's what I'm continuing with unless there's a reason to change it.

Earlier planning materials mention alternative JavaScript backends; the **actual committed code is .NET**, which should guide future implementation unless a formal architecture decision changes it.

## Documentation

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

The Compose password is for **local development only**. Never reuse it in a public environment. 

## Release policy
No production launch date has been committed. A HYVAC pilot needs verified core business workflows, secured endpoints, data isolation tests, operational backups, observability, user acceptance testing, and a controlled migration and rollback plan. See the [roadmap](docs/ROADMAP.md) for gates, not promises.

## Ownership

NullSupport ERP is being developed under NullSupport LLC
