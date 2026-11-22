# Industry ERP questionnaire

Working responses to the Metal Treating Institute ERP Vendor Comparative Questionnaire. This is a requirements and implementation reference for NullSupport ERP, not a signed vendor submission.

**Company:** NullSupport LLC  
**Product:** NullSupport ERP  
**Version:** Development  
**Contact:** Assigned when a formal response is prepared

## Response notes

The original questionnaire asks vendors to distinguish available features, third party services, customization and roadmap work. These notes describe the intended implementation for each requirement, along with relevant foundation work already in the .NET and PostgreSQL project.

An implementation approach is not a claim that the feature is released. Cost, customer references, service agreements and compliance credentials will be filled in from actual business records when a formal vendor response is prepared. No subscription pricing has been decided, so the questionnaire's cost codes remain open.

## 1. Company & Heat-Treat Experience

**1. How many years has your company worked with heat-treat companies?**
Business detail to confirm: Verify the company's experience through business records before entering a number.

**2. How did your organization develop its internal knowledge of heat-treat operations?**
Approach: Develop industry knowledge through process mapping, interviews with practitioners, domain specifications and validation of sample production records.

**3. How many heat-treat companies currently use your ERP?**
Business detail to confirm: Report a number from verified production deployment records when available.

**4. Approximately how many heat-treat facilities are currently running your system?**
Business detail to confirm: Count active production facilities from deployment records when available.

**5. What percentage of your customers are commercial heat treaters versus captive heat-treat operations?**
Business detail to confirm: Calculate the actual commercial and captive facility mix from customer records when available.

**6. Which heat-treat processes are currently supported by your system?**
Approach: Heat treating process support is a proposed specialized module. Define supported process families with subject matter experts and test each against real process specifications.

**7. Do you have employees dedicated specifically to heat-treat implementation and/or support?**
Business detail to confirm: Define the implementation and support responsibilities, including escalation paths, before offering services.

**8. Can you provide three heat-treat customer references?**
Business detail to confirm: Gather consenting, verifiable customer references when deployments are established.

## 2. Core ERP & Heat-Treat Functionality

**1. Quality management**
Approach: Create inspection plans, quality checks, acceptance rules, dispositions and approval history linked to lots and work orders.

**2. Maintenance / preventive maintenance**
Approach: Track assets, meter or calendar based service intervals, maintenance work orders and downtime records.

**3. Scheduling**
Foundation and approach: Dispatch schedule models, PostgreSQL persistence and a read API exist. Add authenticated create and update operations, capacity rules, job sequencing and a scheduling interface.

**4. Production management**
Approach: Build production orders, routing steps, resource assignments, state transitions and shop activity entries.

**5. Change control / revision control**
Approach: Version specifications, recipes and operating documents with approvals, effective dates and immutable revision history.

**6. Training management**
Approach: Build employee skill records, training assignments, completions and restrictions based on current qualifications.

**7. EHS**
Option to evaluate: Environmental, health and safety incidents, checks and action tracking could be added after industry requirements are validated.

**8. Certifications / certificate management**
Approach: Associate documents and certifications with equipment, operators, jobs and customer requirements; track expiry and approvals.

**9. Accounting**
Approach: Build financial records and reporting as a later module, with accounting synchronization where appropriate. Accounting functions and integrations will follow the core financial records.

**10. Customer service**
Foundation and approach: Customer and contact domain models exist; add customer service case workflows, searchable history and authenticated access.

**11. Shipping / receiving**
Approach: Implement receiving, shipment creation, packing records, carrier events and lot movement history.

**12. Timekeeping / payroll support**
Option to evaluate: Integrate with a payroll provider or add controlled time entry and approval, depending on customer needs.

**13. Customer portal**
Approach: Provide company scoped access for external customers to review authorized jobs, documents and status updates.

**14. APQP**
Option to evaluate: APQP workflows need a specific industry definition before implementation; possible scope includes planning gates and approval artifacts.

**15. Lot and batch traceability**
Approach: Introduce lot identifiers, parent child relationships, movement events and immutable material genealogy from receipt through shipment.

**16. Split and combined lots**
Approach: Represent split and merge operations as explicit traceable transformations with quantity reconciliation.

**17. Furnace / load scheduling**
Approach: Extend scheduling to furnaces, load capacity, work center constraints, cycle time and conflict prevention.

**18. Recipe / process specification control**
Approach: Version recipes and process specifications, tie approved revisions to jobs, require permission to release or change settings.

**19. Pyrometry records**
Approach: Record temperature and instrument measurements with timestamps, source device, calibration context and retained raw attachments.

**20. Furnace and equipment calibration**
Approach: Register furnaces and instruments, calibration schedules, certificates, tolerance checks and out of tolerance exceptions.

**21. Thermocouple tracking**
Approach: Track thermocouple inventory, identification, calibration status, usage history and replacement events.

**22. Quench tracking**
Approach: Capture quench medium, temperature, timing, parameters and traceable links to loads and specifications.

**23. Material certifications**
Approach: Store supplier material certificates and relate them to incoming lots and orders, with controlled access.

**24. Certificate of conformance generation**
Approach: Generate certificates from approved production and inspection records using versioned templates and controlled signoff.

**25. Customer-specific specifications**
Approach: Store customer specific acceptance criteria and revision controlled specifications; attach requirements to each job.

**26. Nonconformance management**
Approach: Log nonconformances against job, lot or equipment; hold affected output until disposition is approved.

**27. Corrective actions**
Approach: Create corrective action tasks with owner, due date, root cause, evidence, verification and closure approval.

**28. Job costing by furnace / load**
Approach: Calculate labor, process time, consumables, energy and overhead per job/load, using recorded rates and machine activity.

**29. Energy usage by job / load**
Approach: Ingest meter readings or configured rate estimates and allocate energy to each load or cycle.

**30. Outside processing**
Approach: Track external processing purchase orders, transfers, vendor receipts, results and returned lot genealogy.

**31. Rework and scrap tracking**
Approach: Record rework routing, scrap reason, quantities, authorized disposition and cost impacts.

**32. Real-time WIP visibility**
Approach: Project live WIP from production events with company scoped dashboards and updated stage history.

**33. Mobile / tablet shop-floor entry**
Approach: Develop responsive shop floor forms for touch input, barcode scans, offline handling if required and controlled synchronization.

**34. Barcode capability**
Approach: Implement label identifiers, scanners, scan validation and event recording for items, lots and jobs.

**35. RFID capability**
Option to evaluate: RFID requires device and reader selection; build an adapter for events and map reads to materials or locations if required.

**36. Automated customer notifications**
Approach: Use event driven messages for schedule changes, completion, holds and document availability, with customer preferences and delivery logs.

**37. Paper traveler and/or paper certificate capability**
Approach: Produce printable travelers and certificate PDFs from versioned templates with controlled numbering and archival links.

## 3. Implementation, Training & Support

**1. What is your typical implementation timeline for a heat-treat customer?**
Business detail to confirm: Define implementation phases around configuration, import, training and acceptance testing. Set delivery estimates after those steps are measured.

**2. What customer resources are typically required during implementation?**
Approach: Proposed customer responsibilities include process owners, a system administrator, data mapping approval, user acceptance testers and integration contacts.

**3. Is implementation performed by your employees, third parties, or both?**
Business detail to confirm: Define accountable implementation roles and any third party responsibilities before offering a delivery package.

**4. What implementation services are included in the quoted price versus charged separately?**
Business detail to confirm: Break implementation into discovery, configuration, migration, integrations, training and support, with included and additional services listed in each proposal.

**5. How is historical data migrated and validated?**
Approach: Build import mappings, preview and validation reports, trial migrations, reconciliation checks and controlled final cutover with rollback.

**6. What training is provided before go-live?**
Approach: Create administrator and operator guides, role based walkthroughs, test environments and acceptance training records before launch.

**7. Is on-site training available?**
Business detail to confirm: Decide on site availability and costs when training and support packages are defined.

**8. Is a dedicated implementation manager assigned?**
Business detail to confirm: Assign an accountable point of contact for each implementation and document escalation and milestones.

**9. How are scope, budget, and timeline overruns handled?**
Approach: Use documented scope changes, issue tracking, change approval, revised estimates and release checkpoints. Terms remain to be written.

**10. Describe your support model: live support, dedicated contact, online knowledge base, AI, after-hours, and any upcharges.**
Approach: Plan issue intake, knowledge base, incident escalation and support hours. Live support, AI support and after hours options are not committed.

**11. What are your support response-time commitments / SLAs?**
Business detail to confirm: Define incident severity levels, coverage hours and measurable response targets after support capacity is established.

**12. What are your published system uptime SLAs?**
Business detail to confirm: Measure uptime and recovery performance before publishing a service commitment.

## 4. Data, Integration & Connectivity

**1. Who owns the customer data?**
Business detail to confirm: Document company rights to operational records and define authenticated export and deletion procedures in customer terms.

**2. How is tenant segregation handled?**
Approach: Use company identifiers, API authorization, scoped queries and tests that explicitly attempt cross company access.

**3. Can data be transferred from an existing ERP? What data can be migrated?**
Approach: Create mapping tools and importer services for customers, locations, assets, work orders, lots and history; validate required fields and relationship integrity.

**4. What is the process for extracting ALL historical data if a customer later changes ERP systems?**
Approach: Design complete exports of tenant scoped entities, attachments, references and audit history with documented formats and integrity checks.

**5. What backend/database technology does the system use (SQL, other)?**
Foundation and approach: ASP.NET Core/.NET 10 backend and PostgreSQL via EF Core/Npgsql are present. Extend the PostgreSQL schema through versioned migrations as new modules are implemented.

**6. Does the system integrate directly with QuickBooks Desktop and QuickBooks Online with real-time synchronization?**
Option to evaluate:  Evaluate approved desktop connector and online API routes, with reconciliation, deduplication and retry handling.

**7. Does the system integrate with electronic furnace data acquisition systems?**
Approach: For the specialized manufacturing module, build connectors to supported acquisition systems using documented device protocols, files or APIs.

**8. Do you provide a documented API?**
Foundation and approach: Generate and publish an OpenAPI specification with versioned resources, documented errors and secured partner access.

**9. Is REST API access available?**
Foundation and approach: Extend the existing HTTP API into authenticated REST resources with tenant scoped create, read, update and search operations.

**10. Is API access included in the base price?**
Business detail to confirm: Decide API access levels and any usage limits when subscription packages are defined.

**11. Can customers directly access their own data through the API?**
Approach: Add tenant scoped service credentials or delegated access, documented read and write permissions, audit events and rate limits.

**12. Which accounting systems have native integrations?**
Business detail to confirm: Select the first accounting integration after its data model and reconciliation requirements are confirmed.

**13. Which payroll systems have native integrations?**
Business detail to confirm: Evaluate payroll APIs and map approved time entries and employee records through permission checked connectors.

**14. Which CRM systems have native integrations?**
Business detail to confirm: Define contact, account and opportunity synchronization through a scoped API connector.

**15. Does the system integrate with Power BI or similar BI tools?**
Option to evaluate: Provide authorized reporting exports or a scoped analytics connector; Power BI support depends on confirmed integration requirements.

**16. Does the system support EDI?**
Option to evaluate: EDI mappings and transport are future possibilities; assess transaction sets and industry trading partners first.

**17. Does the system integrate with laboratory/testing systems?**
Option to evaluate: Introduce adapter interfaces for lab result ingestion with unit validation and attachment to jobs/lots.

**18. Does the system integrate with UPS, FedEx, or other shipping systems?**
Option to evaluate: Add shipping provider connectors for label creation, tracking and delivery status based on demand.

**19. Can approved third-party applications write data back into the ERP?**
Approach: Design a scoped integration API with explicit write permissions, validation, idempotency, audit logs and tenant isolation.

## 5. Cybersecurity, Compliance & Hosting

**1. Is the solution cloud-hosted, on-premise, or available in both models?**
Business detail to confirm: Use the containerized application as the deployment base. Evaluate managed cloud hosting and whether a supported private deployment option is needed.

**2. Where is customer data physically hosted?**
Business detail to confirm: Select a hosting region and document data residency before onboarding companies.

**3. Who is the cloud hosting provider?**
Business detail to confirm: Evaluate managed services for application hosting, PostgreSQL, object storage and identity. Confirm a provider before production.

**4. Is any support, data hosting, or system operation located outside the United States?**
Business detail to confirm: Document the physical data location and support access arrangements once vendors and operating roles are selected.

**5. Is data encrypted at rest and in transit?**
Approach: Require HTTPS/TLS, encrypted managed storage and backups, key management and verification of configuration before production.

**6. Is multi-factor authentication supported and/or required?**
Approach: Use managed identity, MFA, session handling and a fresh authentication challenge for Protected Access.

**7. Does the system support role-based permissions?**
Foundation and approach: Connect the existing role and permission models to stored assignments and enforce company scoped authorization in every API operation.

**8. Are user and system audit logs maintained?**
Approach: Add tamper resistant application audit events for identity, permissions, data changes and critical actions, alongside infrastructure logs.

**9. How frequently are backups performed?**
Business detail to confirm: Configure scheduled encrypted backups, retention and backup monitoring. Test restores before committing to a recovery target.

**10. Describe the disaster recovery and business continuity process.**
Approach: Write and test restore, incident, continuity and rollback procedures; verify recovery with a nonproduction restore exercise.

**11. What are your Recovery Time Objective (RTO) and Recovery Point Objective (RPO)?**
Business detail to confirm: Measure the recovery time and recovery point during restore testing, then set targets consistent with the hosting design.

**12. Has your organization experienced a material cybersecurity breach in the past five years? If yes, explain at a high level.**
Business detail to confirm: Confirm the actual incident history from authorized business and security records before submitting a response.

**13. Do you undergo independent penetration testing?**
Business detail to confirm: Schedule independent penetration testing before commercial release and track remediation through closure.

**14. What is your SOC 2 Type II status?**
Business detail to confirm: If customers require SOC 2 Type II, scope controls, implement evidence collection, complete readiness work and arrange an independent audit.

**15. What is your NIST 800-53 compliance status?**
Business detail to confirm: Map controls and collect evidence against NIST 800 53 if the target customers require that framework.

**16. Are you FedRAMP authorized/accredited, or are you pursuing it?**
Business detail to confirm: Evaluate the business case and eligibility before pursuing a FedRAMP authorization program.

**17. Are you ITAR registered or willing to register?**
Business detail to confirm: Verify applicable registration and export control requirements before committing to a regulated deployment.

**18. Do you offer hosting/configuration options appropriate for customers with CMMC requirements?**
Option to evaluate: Document customer security requirements first. Assess tenant isolation, logging, access controls, hosting boundaries and applicable CMMC obligations for each use case.

**19. How are security patches and vulnerability updates deployed?**
Approach: Establish dependency scanning, patch review, automated tests, deployment approvals, maintenance windows and rollback procedures.

**20. How can customers obtain your current compliance documentation?**
Business detail to confirm: Prepare a security documentation package and make current evidence available through a controlled request process.

## 6. Artificial Intelligence & Automation

**1. Which AI capabilities are available in the product today?**
Current scope and approach: Start with permission checked document search and operational summaries if AI enters the roadmap. Any current availability claim must be verified against a release.

**2. Which AI capabilities are on the product roadmap?**
Option to evaluate: Potential future features include document search, anomaly flags, production summaries, suggested schedule changes and natural language reporting. None is committed.

**3. What AI model/provider(s) are used?**
Business detail to confirm: Select the provider only after reviewing data handling, deployment options, cost and access controls.

**4. Is customer data ever used to train your AI models?**
Business detail to confirm: Write the customer data handling and model training policy, and review provider contracts before AI receives customer data.

**5. Can customers opt out of AI processing?**
Approach: If AI is introduced, provide an organization level enable/disable policy and optional per feature controls.

**6. Does customer information leave the ERP environment when AI is used?**
Business detail to confirm: This depends on selected infrastructure/provider. Design explicit data routing disclosures and a reviewed data processing agreement.

**7. Is AI access limited by the same user permissions and role controls as the ERP?**
Approach: Any AI service must recheck user, role, company and resource permissions on source retrieval and before writing results.

**8. Does the system maintain an audit trail of AI-generated recommendations/actions?**
Approach: Log prompts or safe metadata as appropriate, source references, generated recommendations and approved actions while avoiding sensitive data leakage.

**9. Can AI execute transactions or workflow actions, or does it only recommend actions?**
Option to evaluate: Begin with advisory only capabilities. Any later transaction execution would need explicit approval policies, least privilege and audit records.

**10. Can AI perform data logging and automated workflows?**
Option to evaluate: Add constrained workflow automations only after event rules, human review and error handling are established.

**11. Can approved AI plug-ins or connectivity tools work with data across ERP modules?**
Option to evaluate: Design permission scoped integration connectors with per company configuration and event logging if needed.

**12. If AI uses an LLM, is it isolated/closed to the customer environment or able to learn from other systems?**
Business detail to confirm: Evaluate private inference or a contracted provider with explicit data boundaries. Do not assume cross company data or model training is permitted.

## 7. Customer References, Terms & Product Roadmap

**1. Provide three current heat-treat customer references, including company, contact, and implementation year.**
Business detail to confirm: Provide references only with customer consent and verified implementation details.

**2. Provide a copy of the current customer terms and conditions / user agreement.**
Business detail to confirm: Prepare customer terms that cover access, data rights, support, termination and security responsibilities before selling subscriptions.

**3. How often are major product releases issued?**
Business detail to confirm: Set a release cadence after deployment automation and rollback are in place, and maintain a versioned change history.

**4. How are customers informed of product changes and deprecations?**
Approach: Publish change logs and admin notices; establish versioning and deprecation periods before external use.

**5. Describe the top heat-treat-related capabilities planned for the next 12-24 months.**
Option to evaluate: Proposed specialized priorities: lot genealogy, furnace load scheduling, process specifications, quality checks, calibration and certificates. Scope and sequence require field validation.

**6. What percentage of annual revenue is invested in product development / R&D?**
Business detail to confirm: Calculate the actual percentage from financial records when a reporting period is available.

**7. Describe your process for customer enhancement requests.**
Approach: Track requests in an issue backlog, assess operational impact and security, prioritize approved changes and publish release status.

**8. Identify any major modules or functions that rely on third-party products or services.**
Foundation and approach: The platform is based on .NET, EF Core, Npgsql and PostgreSQL, with container based development. Document selected identity, hosting and integration providers when contracted.

## Where these requirements fit

The [development roadmap](ROADMAP.md) starts with authentication, company administration and the core work order workflow. Inventory, finance and document management follow. The [industry requirements plan](INDUSTRY_REQUIREMENTS_PLAN.md) explains how lot genealogy, production, quality and process records can be built as additional modules.

The questionnaire is written as a development reference. If it is used as a formal vendor response later, each answer will be checked against a deployed release and supporting evidence, including costs, commitments, compliance status and customer references.
