# Million Real Estate API

REST API for real estate property management developed with .NET 8, SQL Server, and Clean Architecture.

## 🏗️ Architecture

The project follows Clean Architecture principles with the following layers:

- **Million.Domain**: Domain entities and business rules
- **Million.Application**: Use cases, DTOs, interfaces, and application logic (CQRS with MediatR)
- **Million.Infrastructure**: Persistence implementations (EF Core), external services
- **Million.Api**: Presentation layer with REST controllers
- **Million.Tests**: Unit tests (NUnit) with coverage for core handlers

## 🚀 Technologies

- **.NET 8**: Main Framework
- **SQL Server 2022**: Relational Database
- **Entity Framework Core**: ORM for SQL Server
- **MediatR**: CQRS pattern and mediator
- **FluentResults**: Result and error handling
- **FluentValidation**: Data validation
- **Mapster**: Object mapping
- **JWT**: Authentication and authorization
- **Swagger/OpenAPI**: API Documentation

## 📋 Prerequisites

Before running the project, make sure you have installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) and Docker Compose
- [Git](https://git-scm.com/)

## 🔧 Installation and Setup

### 1. Clone the repository

```bash
git clone <repository-url>
cd Million.RealEstate
```

### 2. Start SQL Server

First, start the database using Docker Compose:

```bash
cd docker
docker-compose up -d
```

This will start:
- **SQL Server**: Port 1433

Verify the container is running:

```bash
docker-compose ps
```

### 3. Run the API

Navigate to the API project and run it:

```bash
cd ../src/Million.Api
dotnet restore
dotnet run
```

The API will run at: **http://localhost:5233**

## 📊 Service Access

### REST API
- **URL**: http://localhost:5233
- **Swagger UI**: http://localhost:5233/swagger

### SQL Server
- **Host**: localhost
- **Port**: 1433
- **User**: sa
- **Password**: Password123!
- **Database**: MillionRealEstate

## 📖 API Documentation

Once the API is running, access the interactive Swagger documentation at:

**http://localhost:5233/swagger**

From Swagger you can:
- View all available endpoints
- Test API calls
- View data schemas
- Get request/response examples

## 🔐 Authentication

The API uses JWT (JSON Web Tokens) for authentication. Protected endpoints require the header:

```
Authorization: Bearer <your-jwt-token>
```

### Default Credentials (Owner)
The system automatically creates a test owner user if the database is empty:
- **Email**: `owner@test.com`
- **Password**: `TestPassword123!`

Use the `/api/Auth/login` endpoint to obtain the token.

## 🏠 Main Features

### Properties
- **GET /api/Properties**: List properties with pagination and filters
- **GET /api/Properties/{id}**: Get property by ID
- **GET /api/Properties/owner/{ownerId}**: Get properties by owner
- **POST /api/Properties**: Create new property (Protected)
- **POST /api/Properties/{id}/images**: Add image to property (Protected)
- **PATCH /api/Properties/{id}/price**: Change property price (Protected)
- **PUT /api/Properties/{id}**: Update property (Protected)
- **DELETE /api/Properties/{id}**: Delete property (Protected)

### Owners
- **GET /api/Owners**: List owners
- **GET /api/Owners/{id}**: Get owner by ID

### Property Images
- **POST /api/PropertyImages**: Upload image (Protected)
- **GET /api/PropertyImages/property/{propertyId}**: Get images for a property
- **DELETE /api/PropertyImages/{id}**: Delete image

### Property Traces
- **GET /api/PropertyTraces/property/{propertyId}**: Get history of a property
- **POST /api/PropertyTraces**: Create new trace (Protected)

## 🗃️ Data Structure

### Property
```json
{
  "id": "guid",
  "name": "string",
  "description": "string",
  "address": "string",
  "price": "decimal",
  "codeInternal": "string",
  "year": "int",
  "ownerId": "guid",
  "images": [
     {
        "file": "string (URL)",
        "enabled": true
     }
  ],
  "traces": []
}
```

## 🌱 Seed Data

The application includes a seeder that automatically loads test data on startup:
- 3 sample owners
- 6 properties with images
- Trace history for some properties

## 🛠️ Development

### Run in development mode

```bash
cd src/Million.Api
dotnet watch run
```

### Run tests

```bash
dotnet test
```

### Build solution

```bash
dotnet build
```

## 📁 Project Structure

```
Million.RealEstate/
├── docker/
│   └── docker-compose.yml          # Docker services configuration
├── src/
│   ├── Million.Api/                # Presentation layer
│   ├── Million.Application/        # Application logic
│   ├── Million.Domain/             # Domain entities
│   ├── Million.Infrastructure/     # Persistence and services
│   └── Million.Tests/              # Unit tests
├── Million.RealEstate.sln
└── README.md
```