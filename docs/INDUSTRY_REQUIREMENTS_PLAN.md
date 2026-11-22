# Industry requirements plan

The [industry questionnaire](INDUSTRY_ERP_QUESTIONNAIRE.md) adds a useful set of requirements beyond the main ERP workflow. It covers production, quality, material traceability, equipment records, data access, security, training and support.

I want these requirements documented now so the foundation can accommodate them. That does not mean every specialized feature has to be part of the first release.

## How the modules will fit

**Lots and batches:** Assign identifiers when material is received, then record every movement and transformation. Splitting or combining a lot will create linked events with reconciled quantities. The material history can then be traced through processing, rework and shipping.

**Production and scheduling:** Use production orders, routing steps and work centers. Equipment availability, capacity, cycle times and process requirements will determine which loads can be scheduled. This is separate from field dispatch but can share the same company security and audit rules.

**Quality and specifications:** Store controlled specification revisions, inspection plans, results, nonconformances and corrective actions. Approval and revision history will be associated with the correct customer, job, lot or equipment.

**Certificates and process records:** Generate certificates from approved records and versioned templates. Calibration, instrument, temperature and quench data can be captured directly or imported through adapters, with the original readings and source retained.

**Shipping and outside processing:** Receiving, shipment, carrier events and outside vendor work will update lot genealogy and work status. Printed travelers, labels and certificates will use the same records as the application.

**Mobile and automated entry:** Provide responsive forms for shop floor activity. Barcode input will map scans to jobs and lots; RFID and device connectors can use separate adapters where the hardware and data formats justify them.

## Implementation order

1. Finish company authentication, permissions, Protected Access and the core customer, work order and dispatch workflow.
2. Add inventory, purchasing, documents, quotes, invoicing, exports and reporting, using consistent records and history.
3. Introduce lot genealogy and production orders, then equipment capacity scheduling and shop floor activity.
4. Add quality, specifications, calibration and certificates, followed by process specific measurements and integrations.
5. Test traceability end to end, including split and combined lots, rework, external processing and the resulting documents.

## Work beyond the application

Data migration will use field mappings, trial imports, validation and reconciliation. Production operations will need monitored backups, restore tests, patching, incident procedures and a release process. Support and training will need instructions that reflect the actual workflows.

AI and automation can be introduced around these modules after access controls and record history are reliable. A proposed approach is advisory suggestions first, with explicit approval and audit checks before any automated transaction is allowed.

The questionnaire keeps the original vendor questions for reference. Each capability has a proposed implementation direction. Where a question asks for business facts such as customer counts, support agreements or compliance certification, those facts must be verified rather than invented.
