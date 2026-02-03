# Data Migration Decision (ETL)

## Scope

During the migration to the new database, the following data is not migrated:
- Historical sales
- Inactive products, sold products.

## Justification

- Inactive products are considered equivalent to deleted records.
- There are no sales in an intermediate state nor processes dependent on historical sales.
- This data has no functional or operational impact on the current system.
- Migrating it would increase the complexity of the migration itself and the risk of errors without adding value.

## Data Backup

- Legacy database preserved in a cloud storage service, in read-only state.

## Post-migration State

- The new database contains only active and relevant data.
- The legacy database is frozen, with no writes after the migration.
- The system’s operational continuity does not depend on the legacy database.