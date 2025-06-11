# Bolt Hotshot Logistics

A modern logistics platform for hotshot delivery, built for speed, reliability, and flexibility.  
This monorepo contains the full-stack solution: a React-based Admin Dashboard, a cross-platform Driver Mobile App, and a .NET/Azure-based backend with MySQL.

---

## 🚚 Project Overview

**Bolt Hotshot Logistics** is an end-to-end system for managing hotshot delivery jobs, drivers, and real-time logistics operations.  
It features:

- **Admin Dashboard:** Manage jobs, drivers, customers, and view analytics (React/Next.js, TailwindCSS)
- **Driver Mobile App:** Accept & manage jobs, track deliveries, navigation (Expo React Native)
- **Backend API:** Scalable Azure Functions (.NET 8), MySQL, Entity Framework Core, secure config with Key Vault/App Config

---

## 🏗️ Tech Stack & Architecture

- **Frontend (Admin):** Next.js (React 18), TailwindCSS, Recharts, React Table, React Query, Zod, Headless UI, Heroicons
- **Mobile (Driver):** Expo (React Native 0.79+), Lucide React Native, Expo Router, Google Fonts, React Native Maps
- **Backend:** Azure Functions v4 (.NET 8), Entity Framework Core, Pomelo MySQL provider, Azure App Configuration, Key Vault
- **Database:** MySQL
- **Dev Tools:** ESLint, Prettier, TypeScript, xUnit for backend tests

---

## 📁 Project Structure

```
root/
│
├─ 1-Presentation/
│   ├─ admin-dashboard/        # React/Next.js Admin Dashboard
│   ├─ HotshotLogistics.mobile/# Expo React Native Driver App
│   ├─ components/             # Shared React Native components
│   ├─ hooks/                  # Shared hooks
│   ├─ types/                  # Shared TS types for jobs/drivers
│   ├─ app.json, package.json, tsconfig.json, etc.
│
├─ 2-Application/
│   ├─ HotshotLogistics.Core/  # Domain models, business logic
│   └─ Models/                 # Entity definitions (e.g. Driver)
│
├─ 4-Persistence/
│   ├─ HotshotLogistics.Infrastructure/  # EF Core DbContext, migrations, repo impl.
│   └─ Data/
│
├─ 5-Test/
│   └─ tests/HotshotLogistics.Tests/     # xUnit tests for backend
│
├─ backend/                    # Node.js Backend API
│
├─ .gitignore, README.md, etc.
```

---

## 🚀 Getting Started

### Prerequisites

- Node.js (18+)
- .NET 8 SDK
- MySQL (local or cloud)
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

- Update `DefaultConnection` in your config to point to your MySQL instance.
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

- Built with [Next.js](https://nextjs.org/), [Expo](https://expo.dev/), [.NET](https://dotnet.microsoft.com/), [Azure](https://azure.microsoft.com/), [MySQL](https://www.mysql.com/), and more.