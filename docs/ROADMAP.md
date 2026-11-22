# Development roadmap

This tracks the order of work rather than promising a launch date. Current priorities are based on the code that exists in the repository.

## 1. Verify the current foundation

Restore and build the .NET solution, run the tests, start PostgreSQL and verify the existing migration and dispatch endpoint.

Confirm the current development and deployment setup before adding more modules.

## 2. Authentication and company permissions

Implement login and company membership validation. Connect the existing permission logic to the API so access is enforced for actual records and actions.

Each company needs its own administrators with controls for users, roles, permissions and available workflows.

Add tests to confirm company data cannot be accessed across tenants. Protected Access will provide additional authentication for selected sensitive areas.

## 3. Core operational workflow

Connect customer records, locations, equipment, work orders and dispatch scheduling to PostgreSQL.

Add the API operations needed to create, retrieve and update records, then build the frontend around those workflows.

The first complete workflow should cover customer creation, work order creation, scheduling, technician assignment, status changes and activity history.

## 4. Internal testing and deployment

Test the workflow with representative operational data. Verify authentication, permissions, audit logs and company isolation.

Set up production hosting, monitoring, backups and restore testing. Data imports and the rollback process need to be tested before any live migration.

No production date set yet.

## 5. Additional modules

Expand into purchasing, inventory, quotes, invoicing, reporting and integrations as the core system becomes usable.

Keep the modules configurable without maintaining separate application versions for each company.

## 6. Commercial release

The longer term goal is to allow additional companies to use NullSupport ERP independently, with separate records, administrative controls, configurable workflows and account onboarding.

Subscription billing, support, security review and commercial terms are future requirements. They are not part of the current release.
