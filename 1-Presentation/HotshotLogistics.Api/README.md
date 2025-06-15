# HotshotLogistics.Api

Azure Functions API written in .NET 8 providing endpoints for jobs and drivers.
This project is the **Presentation** layer entry point for the backend and calls
into the Application layer to perform commands and queries.

## Tech Stack
- Azure Functions v4
- Entity Framework Core
- MySQL via Pomelo provider

## Run Locally

```bash
dotnet build
func start
```

The API listens on port 7060 by default.

