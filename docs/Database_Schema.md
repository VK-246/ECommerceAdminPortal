Refer C:\Users\vinay\Desktop\WORK\Projects\ECommerceAdminPortal\docs\ERDiagram.png

The database uses a relational structure managed by Entity Framework Core Code-First Migrations targeting PostgreSQL.


## Configuration Notes

- **JSONB Column (`Products.AiMetadata`):** Leveraging PostgreSQL's `JSONB` to store the raw, flexible output from the AI (like SEO scores, generated keyword arrays, or alternative titles) without requiring constant schema migrations when the AI prompt changes.
- **Audit Trail (`Products.CreatedById`):** In an enterprise back-office portal, accountability is critical. The `CreatedById` foreign key ensures we always know which internal Admin/Editor created a specific product.
- **Soft Deletes:** (Optional) Consider adding an `IsDeleted` boolean to avoid physically deleting records, maintaining audit history.
- **Indexes:** Indexes will be placed on `Products.CategoryId` to optimize filtering products by category, and `Products.CreatedById` for auditing queries.