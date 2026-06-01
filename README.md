# InventoryManagement
Since it was more clear for me, I've decided to put in CRUD for Customers so i can easily fetch the customer localization. (This is the only flow thats not covered by tests)

Tech stack:
- EntityFramework with MSSQL
- MediatR to implement CQRS
- XUnit
- FluentAssertions
- NSubstitute

Architecture used:
Clean architecture with DDD approach

- Domain layer contains all business entities with logic / validation.
- Application layer is the coordinator of domain. This is where our use cases (commands/queries and handlers) are.
- Persistence layer is basic EF stuff and UoW / Repositories implementation
- Infrastructure layer is not used, but could be used for eg. dynamicaly fetching holidays i quess
- UI layer lets clients talk with our application

# Assumptions
- For discount calculation I've assumed that the discount is applied after the localization multiplier is applied

# Trade-offs
- Because I've went for transaction approach the UnitOfWork has an integration test dedicated case (that's because integration tests use in memory database which doesnt support transactions)