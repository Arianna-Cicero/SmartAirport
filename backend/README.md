# SmartAirport

A microservices-based airport management system built with .NET 8.0.

## Architecture

This project consists of three independent microservices:

### 1. FlightService
Main service for managing flight operations.

**Features:**
- Flight management (CRUD operations)
- JWT Authentication & Authorization
- API Versioning (v1)
- Health checks
- Structured logging with Serilog
- Swagger/OpenAPI documentation
- Entity Framework Core with In-Memory database
- Global exception handling

**Port:** 5001 (default)

### 2. SensorService
Service for managing airport sensors and monitoring data.

**Features:**
- Sensor data management
- Real-time monitoring capabilities

**Port:** 5002 (default)

### 3. ExternalService
Service for external system integrations.

**Features:**
- External API integrations
- Third-party service communication

**Port:** 5003 (default)

## Technologies Used

- **.NET 8.0** - Latest .NET framework
- **ASP.NET Core Web API** - RESTful API framework
- **Entity Framework Core** - ORM with In-Memory database
- **JWT Bearer Authentication** - Security
- **Serilog** - Structured logging
- **Swagger/OpenAPI** - API documentation
- **API Versioning** - Version management
- **Health Checks** - Service monitoring

## Project Structure

```
SmartAirport/
├── FlightService/
│   ├── Controllers/      # API endpoints
│   ├── Data/            # Database context
│   ├── DTOs/            # Data transfer objects
│   ├── Models/          # Domain models
│   ├── Services/        # Business logic
│   ├── Middleware/      # Custom middleware
│   └── Program.cs       # Application entry point
├── SensorService/
│   ├── Controllers/
│   ├── Models/
│   ├── Services/
│   └── Program.cs
└── ExternalService/
    └── Program.cs
```

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Git

### Running the Services

Each service can be run independently:

#### FlightService
```bash
cd FlightService
dotnet run
```

#### SensorService
```bash
cd SensorService
dotnet run
```

#### ExternalService
```bash
cd ExternalService
dotnet run
```

### Building the Services

```bash
# Build FlightService
dotnet build FlightService/FlightService.csproj

# Build SensorService
dotnet build SensorService/SensorService.csproj

# Build ExternalService
dotnet build ExternalService/ExternalService.csproj
```

## API Documentation

When running in Development mode, each service provides Swagger UI for API documentation:

- **FlightService:** `https://localhost:5001/swagger`
- **SensorService:** `https://localhost:5002/swagger`
- **ExternalService:** `https://localhost:5003/swagger`

## Health Checks

Each service exposes a health check endpoint:

- `GET /health` - Returns service health status

## Authentication

FlightService uses JWT Bearer token authentication. Include the token in the Authorization header:

```
Authorization: Bearer <your-jwt-token>
```

## Development

### Configuration

Service configurations can be modified in `appsettings.json` and `appsettings.Development.json` files within each service directory.

### Logging

Logs are written to:
- Console output
- File system (in `logs/` directory within each service)

## License

This project is for educational purposes.

## Contributors

- Arianna Cicero
- Guilherme Souza