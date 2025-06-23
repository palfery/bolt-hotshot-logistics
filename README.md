# Hotshot Logistics

A modern logistics platform for hotshot delivery, built for speed, reliability, and flexibility.  
This monorepo contains the full-stack solution: a React-based Admin Dashboard, a cross-platform Driver Mobile App, and a .NET/Azure-based backend with SQL Server.

---

## 🚚 Project Overview

**Bolt Hotshot Logistics** is an end-to-end system for managing hotshot delivery jobs, drivers, and real-time logistics operations.  
It features:

- **Admin Dashboard:** Manage jobs, drivers, customers, and view analytics (React/Next.js, TailwindCSS)
- **Driver Mobile App:** Accept & manage jobs, track deliveries, navigation (Expo React Native)
- **Backend API:** Scalable Azure Functions (.NET 8), SQL Server, Entity Framework Core, secure config with Key Vault/App Config

---

## 🏗️ Tech Stack & Architecture

- **Frontend (Admin):** Next.js (React 18), TailwindCSS, Recharts, React Table, React Query, Zod, Headless UI, Heroicons
- **Mobile (Driver):** Expo (React Native 0.79+), Lucide React Native, Expo Router, Google Fonts, React Native Maps
- **Backend:** Azure Functions v4 (.NET 8), Entity Framework Core, SQL Server provider, Azure App Configuration, Key Vault
- **Database:** SQL Server
- **Dev Tools:** ESLint, Prettier, TypeScript, xUnit for backend tests

### Clean Architecture

The repository is organized using a classic **Clean Architecture** approach.
Presentation projects (APIs, web dashboard, and mobile app) only depend on the
Application layer, which in turn depends on Domain abstractions. Infrastructure
and persistence concerns live in separate projects so the core business logic
remains framework agnostic.

Folder mapping:

1. **0-Base** – foundational types shared between layers
2. **1-Presentation** – Next.js admin portal, Expo mobile app and Azure
   Functions API
3. **2-Application** – orchestrates use cases and business rules
4. **3-Domain** – entities and contracts that model the logistics domain
5. **4-Persistence** – EF Core implementations and data migrations
6. **5-Test** – unit, integration and architecture tests
7. **7-Deployment** – docker compose files and deployment scripts
## 🆕 Recent Changes
- Upgraded all .NET projects to **.NET 8**.
- Added integration and architecture tests with **xUnit** and GitHub Actions CI.
- Fixed linting/build errors and applied formatting across the repo.
- Added initial Azure App Configuration support.
- Migrated from MySQL to Azure SQL Server for improved performance and Azure integration.


### Clean Architecture

The repository is organized using a classic **Clean Architecture** approach.
Presentation projects (APIs, web dashboard, and mobile app) only depend on the
Application layer, which in turn depends on Domain abstractions. Infrastructure
and persistence concerns live in separate projects so the core business logic
remains framework agnostic.

Folder mapping:

1. **0-Base** – foundational types shared between layers
2. **1-Presentation** – Next.js admin portal, Expo mobile app and Azure
   Functions API
3. **2-Application** – orchestrates use cases and business rules
4. **3-Domain** – entities and contracts that model the logistics domain
5. **4-Persistence** – EF Core implementations and data migrations
6. **5-Test** – unit, integration and architecture tests
7. **7-Deployment** – docker compose files and deployment scripts
## 🆕 Recent Changes
- Upgraded all .NET projects to **.NET 8**.
- Added integration and architecture tests with **xUnit** and GitHub Actions CI.
- Fixed linting/build errors and applied formatting across the repo.
- Added initial Azure App Configuration support.


---

## 📁 Project Structure

```
root/
│
├─ 0-Base/
│   └─ HotshotLogistics.Core/             # base types shared across layers
│
├─ 1-Presentation/
│   ├─ HotshotLogistics.Api/              # .NET Azure Functions API
│   ├─ HotshotLogistics.mobile/           # Expo React Native driver app
│   └─ admin-dashboard/                   # Next.js admin portal
│
├─ 2-Application/
│   └─ HotshotLogistics.Application/      # application services & orchestrators
│
├─ 3-Domain/
│   ├─ HotshotLogistics.Contracts/        # domain interfaces
│   └─ HotshotLogistics.Domain/           # domain models
│
├─ 4-Persistence/
│   ├─ HotshotLogistics.Data/             # EF Core migrations & seed data
│   └─ HotshotLogistics.Infrastructure/   # repository implementations
│
├─ 5-Test/
│   └─ tests/HotshotLogistics.Tests/      # xUnit tests for backend
│
├─ 6-Lib/
│   └─ data/                              # sample data and utilities
│
├─ 7-Deployment/                          # docker compose & deployment scripts
│
├─ admin-dashboard/                       # copy of Next.js portal (shortcut)
├─ components/                            # shared UI components
├─ hooks/                                 # shared hooks
├─ backend/                               # Node.js example API
├─ data/                                  # miscellaneous mock data
│
├─ .gitignore, README.md, etc.
```

---

## 🚀 Getting Started

### Prerequisites

- Node.js (18+)
- .NET 8 SDK
- SQL Server (local or cloud)
- (Optional) Azure account for cloud deployment

---

### 1. Admin Dashboard (React/Next.js)

```bash
cd 1-Presentation/admin-dashboard
npm install
npm run dev
```
- Runs at [http://localhost:3001](http://localhost:3001)

---

### 2. Driver App (Expo React Native)

```bash
cd 1-Presentation/HotshotLogistics.mobile
npm install
npm run dev
```
- Use Expo Go or an emulator (`npx expo start` if you want the CLI menu)

---

### 3. Backend API (Node.js)

```bash
cd backend
npm install
npm run dev
```
- Configure your connection string in `.env` file.
- API runs locally on port 3000 by default.

---

### 4. .NET Backend API (Azure Functions)

```bash
cd 1-Presentation/HotshotLogistics.Api
dotnet build
dotnet run
```
- Configure your connection string in `local.settings.json` or Azure App Configuration.
- API runs locally on port 7060 by default.

---

### 5. Database Setup

- Update `DefaultConnection` in your config to point to your SQL Server instance.
- Apply EF Core migrations (coming soon).
- Example:
  ```json
  {
    "ConnectionStrings": {
      "DefaultConnection": "Server=localhost;Database=hotshotdb;User=root;Password=yourpassword;"
    }
  }
  ```

---

### 6. Running Tests

```bash
cd 5-Test/tests/HotshotLogistics.Tests
dotnet test
```

---

## 👥 Contributing

PRs and issues welcome!  
See future `CONTRIBUTING.md` for guidelines.

---

## 📄 License

[MIT](LICENSE) (or specify another if you wish)

---

## ✨ Credits

- Built with [Next.js](https://nextjs.org/), [Expo](https://expo.dev/), [.NET](https://dotnet.microsoft.com/), [Azure](https://azure.microsoft.com/), [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-2022), and more.
