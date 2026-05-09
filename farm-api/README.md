# Farm API Solution

A modular, versioned REST API for farm management, built with ASP.NET Core (.NET 8), Entity Framework Core, and supporting modern authentication, API versioning, and robust error handling.

## Solution Structure

- **Services\Farm.Api**  
  Main ASP.NET Core Web API project. Hosts controllers, middleware, and API configuration.

- **Infrastructure\Farm.Business**  
  Business logic layer. Contains service implementations and business rules.

- **Infrastructure\Farm.Domain**  
  Domain layer. Contains entity models, DbContext, and EF Core configuration.

- **Tests\Farm.TestApi**  
  API integration and controller tests using xUnit and Moq.

- **Tests\Farm.TestBusiness**  
  Business logic unit tests.

- **Tests\TestDomain**  
  Domain model and data access tests, using EF Core InMemory provider.

## Features

- **.NET 8** and ASP.NET Core Web API
- **Entity Framework Core** with SQL Server and InMemory support
- **API Versioning** via URL, header, or media type
- **JWT Authentication** (with support for Azure AD and B2C)
- **Swagger/OpenAPI** (enabled in non-production)
- **Global Exception Handling** middleware
- **CORS** configuration for admin and client apps
- **AutoMapper** for DTO/entity mapping
- **Application Insights** telemetry
- **xUnit** and **Moq** for testing

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Docker (for containerized deployment)
- (Optional) SQL Server (if not using Dockerized SQL Server)

### Configuration

1. **App Settings**  
   Configure `appsettings.json` and environment-specific files in `Services\Farm.Api` for:
   - Connection strings (`farmDb`)
   - JWT/Azure AD settings under `IdentityServerAuthentication`
   - Admin URLs under `Admin`

2. **User Secrets**  
   For local development, use [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) for sensitive data.

### Database

- The API will automatically apply EF Core migrations on startup.
- To add migrations:

```
dotnet ef migrations add <MigrationName> -p Infrastructure/Farm.Domain/Farm.Domain.csproj -s Services/Farm.Api/Farm.Api.csproj
```

---

## Running with Docker

This project includes a `docker-compose.yml` for easy containerized setup of both the API and SQL Server.

### 1. Build and Start the Containers

From the solution root, run:

```
docker-compose up --build
```

This will:
- Build the API Docker image.
- Start both the API (`farmapi`) and SQL Server (`sqlserver`) containers.
- Expose the API on port `8080` and SQL Server on port `1433`.

### 2. Access the API

Once running, the API will be available at:

```
http://localhost:8080
```

Swagger UI (API documentation) will be available at:

```
http://localhost:8080/swagger
```

### 3. Stopping the Containers

To stop and remove the containers, run:

```
docker-compose down
```

### 4. Notes on Connection Strings

When running in Docker, the API uses the following connection string (set in `appsettings.Development.json`):

```
"farmDb": "Server=sqlserver,1433;Database=farm-db;User=sa;Password=dockerABC@123;TrustServerCertificate=True"
```
- `sqlserver` is the Docker Compose service name for SQL Server.
- The database and user credentials are set in `docker-compose.yml`.

---

## Running Locally (Without Docker)

From the solution root:

```
dotnet build
dotnet run --project Services/Farm.Api/Farm.Api.csproj
```

The API will be available at `https://localhost:44333` (or as configured).

---

## Running with IIS Express in Visual Studio

You can run the API using IIS Express directly from Visual Studio for local development and debugging.

### Steps:

1. **Open the Solution in Visual Studio**
2. **Set `Farm.Api` as the Startup Project**
3. **Select IIS Express** in the run/debug dropdown at the top of Visual Studio
4. **Press F5** (or click the green play button) to start debugging

- The API will be hosted at a URL similar to:
  - `https://localhost:44333` (check the launch profile or output window for the exact port)
- Swagger UI will be available at:
  - `https://localhost:44333/swagger`

**Note:**
- Ensure your `appsettings.Development.json` connection string points to a SQL Server instance accessible from your host (e.g., `Server=localhost;Database=farm-db;User=sa;Password=yourpassword;TrustServerCertificate=True`).
- If using SQL Server Express, you may use `Server=.\\SQLEXPRESS;...`.
- If you want to use the Dockerized SQL Server, use `Server=localhost,1433;User=sa;Password=dockerABC@123;...` and ensure the SQL Server container is running.

---

## Testing

Run all tests:

```
dotnet test
```

---

## Project Highlights

- **GlobalExceptionHandler**: Centralized error handling with request context and support-friendly error IDs.
- **ConfigurationController**: Exposes admin and navigation configuration endpoints.
- **MappingProfile**: Centralized AutoMapper profile for DTO/entity mapping.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Open a pull request

## License

This project is licensed under the MIT License.

---