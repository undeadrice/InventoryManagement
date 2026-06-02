# Notes
Since it was more clear for me, I've decided to put in CRUD for Customers so i can easily fetch the customer localization. (This is the only flow thats not covered by tests).

All endpoints have /api prefix in route eg /api/orders

Tech stack:
- EntityFramework with MSSQL
- MediatR to implement CQRS
- FluentValidation
- XUnit
- FluentAssertions
- NSubstitute

You can find database connection string setting in UI projects appsettings

Architecture used:
Clean architecture with DDD approach

- Domain layer contains all business entities with logic / validation.
- Application layer is the coordinator of domain. This is where our use cases (commands/queries and handlers) are.
- Persistence layer is basic EF stuff and UoW / Repositories implementation.
- Infrastructure layer is not used, but could be used for eg. dynamicaly fetching holidays i quess.
- UI layer lets clients talk with our application.

# Assumptions
- The validation and transactions starting/commiting takes place in MediatR pipeline.
- For discount calculation I've assumed that the discount is applied after the localization multiplier is applied.

# Trade-offs
- Because I've went for transaction approach the UnitOfWork has an integration test dedicated flow (that's because integration tests use in memory database which doesnt support transactions).
- When launching integration tests, the application will assume today is the date that the users operating system is set to. We could mock the the actual date by implementing IDateTimeProvider and setting it up in InventoryWebApplicationFactory however i decided to skip this for simplicity. Current solution limits what we can assert on because some tests would be flaky and possibly return invalid result because of holidays.