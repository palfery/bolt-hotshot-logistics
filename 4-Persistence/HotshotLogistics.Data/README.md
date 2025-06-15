# HotshotLogistics.Data

Entity Framework Core data access layer and migrations.
This project implements the **Persistence**/Infrastructure concerns of the Clean
Architecture, providing repositories that the Application layer depends upon.

## Tech Stack
- EF Core with Pomelo MySQL

## Build

```bash
dotnet build
```

## Tests

```bash
dotnet test ../../5-Test/tests/HotshotLogistics.Tests/HotshotLogistics.Tests.csproj
```

