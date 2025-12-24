# Roadmap: Wealth & Growth Manager

## Giai đoạn 1: Architecture & Foundation (Tuần 1-2)
**Mục tiêu:** Thiết lập khung sườn vững chắc, chuẩn Clean Architecture.

### Tuần 1: Setup & Identity
- [ ] **Setup Solution**
  - [x] Modular Monolith Structure (Modules/Finance created)
  - [x] Config Entity Framework Core, SQL Server, Docker compose
- [ ] **Implement Identity**
  - [ ] Login, Register, Refresh Token
  - [ ] Role-based Authorization (User/Admin)
  - [ ] Custom Middleware (Exception Handling, Logging)
  - [ ] Dependency Injection Setup

### Tuần 2: Frontend Base & Authentication
- [ ] **Setup Angular 17+ (Nx Workspace)**
- [ ] **Authentication Flow** (Interceptors, Guard, Login UI)

## Giai đoạn 2: Core Business & Complex Logic (Tuần 3-5)
**Mục tiêu:** Xử lý nghiệp vụ phức tạp, áp dụng CQRS và tối ưu Database.

### Tuần 3: Finance Module (Backend Heavy)
- [ ] **Domain & DB Design** (Transaction, Portfolio, Wallet)
- [ ] **CQRS Implementation** (MediatR, Commands/Queries)
- [ ] **Complex Financial Logic** (EAR calculation, Decimal handling)
- [ ] **Concurrency Control** (Optimistic Locking)

### Tuần 4: Learning Module & Frontend State
- [ ] **Learning Module Backend** (Flashcards, Spaced Repetition)
- [ ] **Frontend State Management** (Angular Signals)
- [ ] **Complex UI** (Drag & Drop, Dynamic Forms)

### Tuần 5: Performance & Data Access
- [ ] **Optimization** (AsNoTracking, SplitQuery, Indexing)
- [ ] **Caching** (Redis)
- [ ] **Generic Features** (Paging, Sorting, Filtering)

## Giai đoạn 3: Advanced Features & DevOps (Tuần 6-7)
- [ ] **Background Jobs** (Hangfire)
- [ ] **Real-time** (SignalR)
- [ ] **Reporting & Dashboard**
- [ ] **CI/CD & Deployment**
