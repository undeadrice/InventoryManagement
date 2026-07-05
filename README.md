This project is a sample implementation of an inventory and order management system built with ASP.NET Core using Clean Architecture and CQRS principles.

It demonstrates how to structure a modular backend system with clear separation of concerns between domain logic, application workflows, persistence, and API exposure. The system supports product and order management with business rules around stock handling, pricing, and discount application.

# Key Features
- Product management (create and list products)
- Order processing with stock validation and automatic stock updates
- Business rules for pricing and discount calculation
- Customer-aware pricing adjustments based on location
- Rule-based discount engine with priority handling (non-stackable discounts)
- Basic CRUD support for customer data (used for localization context)

# Architecture & Design
The solution follows Clean Architecture with a Domain-Driven Design approach:

- Domain Layer – Core business entities and rules
- Application Layer – CQRS-based use cases implemented with MediatR
- Persistence Layer – EF Core implementation with repository and unit of work patterns
- API Layer – REST endpoints exposing application functionality
- Infrastructure Layer – Reserved for external integrations (not heavily utilized)

Cross-cutting concerns such as validation and transaction handling are implemented via MediatR pipeline behaviors.

# Tech Stack
- ASP.NET Core
- Entity Framework Core (MSSQL)
- MediatR (CQRS)
- FluentValidation
- xUnit
- FluentAssertions
- NSubstitute

# Testing
The project includes unit and integration tests for core business logic and critical flows, with special handling for transactional behavior and deterministic test execution where needed.

# Notes
The system assumes a fixed set of business rules for discounts, holidays, and pricing adjustments to keep the domain logic self-contained and predictable. Some simplifications were made around time-dependent logic and external integrations to maintain test stability and focus on core architecture.
