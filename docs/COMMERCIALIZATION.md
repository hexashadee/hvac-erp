# Commercial product strategy

NullSupport ERP is being developed under NullSupport LLC. The longer term plan is one application that can serve different companies without needing a separate codebase for each customer.

The company administration and security model is part of the product itself, not an add on to be worked out after deployment.

## Separate company environments

Each company will have its own records, users, administrators, permissions and configured workflows. The API and database access layer will enforce the company boundary across normal records, reports, attachments, imports, exports and background jobs.

Company admins will be able to manage users and access within their own organization. They can choose available modules, assign permissions and apply Protected Access to sensitive resources without changing the application code.

## Onboarding and data movement

Onboarding will create the company, initial administrator, branch structure, permission defaults and selected workflows. Import tools will map existing records into the ERP and show validation errors before final import.

Exports will use documented formats and include the records and attachments a company is authorized to retrieve. Historical relationships should remain understandable after an export.

## Subscriptions and support

Subscriptions can be managed at the company level, with plan configuration separated from employee permissions. Pricing, usage rules and any additional services will be decided separately from the application architecture.

Operating a hosted service also means documenting updates, support requests, incident handling, recovery procedures and customer notices. Those processes will be tested and established alongside the production deployment.

## Security and customer requirements

Sensitive actions will use normal authorization plus optional Protected Access. Audit history will cover administrative changes, important data operations and integration activity. Backup and restore procedures, encryption and access isolation will be verified before production use.

The [industry questionnaire](INDUSTRY_ERP_QUESTIONNAIRE.md) is being used to plan additional capabilities and to track the operational questions customers may ask. Published compliance claims, service commitments and customer references will be based on verifiable evidence when those are relevant.

## Ownership

NullSupport ERP is being developed under NullSupport LLC. Any terms for commercial distribution or use of externally licensed components will be addressed when the product is prepared for release.
