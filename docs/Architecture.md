# System Architecture

## 1. High-Level System Design (HLD)

The E-Commerce Admin Portal follows a decoupled client-server architecture, containerized for consistent deployment.

Refer C:\Users\vinay\Desktop\WORK\Projects\ECommerceAdminPortal\docs\HLD.png

## 2. Clean Architecture (Backend)

The backend strictly adheres to Clean Architecture principles (Onion Architecture) to ensure the separation of concerns, testability, and independence from frameworks.

Refer C:\Users\vinay\Desktop\WORK\Projects\ECommerceAdminPortal\docs\BackendArchitecture.png

- **Domain:** Contains only enterprise logic (Entities: Product, Category, User). No external dependencies.
- **Application:** Contains business use cases (Services). Defines interfaces for repositories (`IProductRepository`).
- **Infrastructure:** Implements the interfaces (Database access, API calls).
- **Api:** The entry point. Handles HTTP requests, routes them to Application services, and returns standardized HTTP responses.

## 3. Architectural Decision Record (ADR) - For Interviews

When asked "Why did you choose X?", use these documented decisions:

1. **Why .NET 10 / Clean Architecture?**
    - *Decision:* To use N-Tier/Clean Architecture instead of a monolithic MVC app.
    - *Why:* It drastically improves **unit testability**. By isolating business logic in the Application layer, we can **test it using Moq without needing a real database**. It also **protects the core business rules if we ever swap out the UI or Database.**
2. **Why Angular?**
    - *Decision:* To use Angular over React for the frontend.
    - *Why:* Angular provides a **highly opinionated, "batteries-included" framework out of the box** (Routing, HttpClient, Dependency Injection, RxJS). This aligns perfectly **with the structured, object-oriented nature of .NET**, making it the **preferred choice for enterprise back-office applications.**
3. **Why PostgreSQL?**
    - *Decision:* To use PostgreSQL instead of SQL Server or NoSQL.
    - *Why:* Our data (**Categories and Products) is inherently relational**, but we needed flexibility for our AI integration.
        - PostgreSQL is a robust relational database that handles **JSON/text manipulation** (`JSONB`) exceptionally well. This allows us to store the flexible, semi-structured metadata returned by the AI APIs directly alongside our rigid relational data without needing a separate NoSQL database.
4. **Why JWT over Session Cookies?**
    - *Decision:* Stateless JWT Authentication.
    - *Why:* Since the Angular frontend and .NET backend will eventually be hosted on different domains/containers, CORS and cross-domain cookie management introduce friction. JWT provides a **stateless, easily transportable authorization mechanism ideal for decoupled SPAs.**

## 4. Anticipated Challenges & Mitigations

- **Issue:** *The N+1 Query Problem in EF Core.*
    - *Mitigation:* When fetching a list of Products, accessing `Product.Category.Name` in a loop will trigger hundreds of database calls. **We will mitigate this using Eager Loading (`.Include(p => p.Category)`) in our Repository layer.**
- **Issue:** *AI API Latency.*
    - *Mitigation:* LLM calls take 3-10 seconds. We will **mitigate UI freezing by handling this asynchronously in .NET and using loading spinners in Angular, preventing the user from clicking "Generate" multiple times.**
- **Issue:** *Over-posting Vulnerabilities.*
    - *Mitigation:* We will **never expose raw Domain Entities to the frontend.** We will strictly **use DTOs (Data Transfer Objects)** to control exactly what data the client can send and receive.