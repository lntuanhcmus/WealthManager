# Identity Module Implementation Plan

## Goal
Implement a self-contained **Identity Module** to handle User Management, Authentication (JWT), and Authorization, replacing the default monolithic Identity setup.

## Proposed Changes

### 1. Module Structure
Create `src/Modules/Identity` with standard 4-project structure:
- `WealthManager.Modules.Identity.Domain`
- `WealthManager.Modules.Identity.Application`
- `WealthManager.Modules.Identity.Infrastructure`
- `WealthManager.Modules.Identity.API`

### 2. Domain Layer
#### [NEW] [ApplicationUser.cs](file:///src/Modules/Identity/WealthManager.Modules.Identity.Domain/Entities/ApplicationUser.cs)
- Inherit from `IdentityUser` (Microsoft.AspNetCore.Identity)
- Add custom properties (e.g., `FullName`).

### 3. Infrastructure Layer
#### [NEW] [IdentityDbContext.cs](file:///src/Modules/Identity/WealthManager.Modules.Identity.Infrastructure/Database/IdentityDbContext.cs)
- Inherit from `IdentityDbContext<ApplicationUser>`
- Configure Schema: "identity" (isolate tables from "finance").

#### Dependencies
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.SqlServer`

### 4. Integration
#### [MODIFY] [Program.cs](file:///d:/Study/WealthManager/src/WealthManager.API/Program.cs)
- Register Identity Module services.
- Add Authentication/Authorization Middleware.

## Verification Plan
- **Build**: Ensure no conflicts with Finance DbContext.
- **Migration**: Run `dotnet ef migrations add InitialIdentity` to create Identity tables (`AspNetUsers`, `AspNetRoles` etc.) in the database.
- **Test**: Verify tables are created in schema `identity` via SQL query.
