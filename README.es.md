# Million Real Estate API

API REST para gestión de propiedades inmobiliarias desarrollada con .NET 8, SQL Server y Clean Architecture.

## 🏗️ Arquitectura

El proyecto sigue los principios de Clean Architecture con las siguientes capas:

- **Million.Domain**: Entidades de dominio y reglas de negocio
- **Million.Application**: Casos de uso, DTOs, interfaces y lógica de aplicación (CQRS con MediatR)
- **Million.Infrastructure**: Implementaciones de persistencia (EF Core), servicios externos
- **Million.Api**: Capa de presentación con controladores REST
- **Million.Tests**: Pruebas unitarias (NUnit) con cobertura para handlers principales

## 🚀 Tecnologías

- **.NET 8**: Framework principal
- **SQL Server 2022**: Base de datos Relacional
- **Entity Framework Core**: ORM para SQL Server
- **MediatR**: Patrón CQRS y mediador
- **FluentResults**: Manejo de resultados y errores
- **FluentValidation**: Validación de datos
- **Mapster**: Mapeo de objetos
- **JWT**: Autenticación y autorización
- **Swagger/OpenAPI**: Documentación de la API

## 📋 Prerrequisitos

Antes de ejecutar el proyecto, asegúrate de tener instalado:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) y Docker Compose
- [Git](https://git-scm.com/)

## 🔧 Instalación y Configuración

### 1. Clonar el repositorio

```bash
git clone <url-del-repositorio>
cd Million.RealEstate
```

### 2. Iniciar SQL Server

Primero inicia la base de datos usando Docker Compose:

```bash
cd docker
docker-compose up -d
```

Esto iniciará:
- **SQL Server**: Puerto 1433

Verifica que el contenedor esté ejecutándose:

```bash
docker-compose ps
```

### 3. Ejecutar la API

Navega al proyecto de la API y ejecútala:

```bash
cd ../src/Million.Api
dotnet restore
dotnet run
```

La API se ejecutará en: **http://localhost:5233**

## 📊 Acceso a los servicios

### API REST
- **URL**: http://localhost:5233
- **Swagger UI**: http://localhost:5233/swagger

### SQL Server
- **Host**: localhost
- **Puerto**: 1433
- **Usuario**: sa
- **Contraseña**: Password123!
- **Base de datos**: MillionRealEstate

## 📖 Documentación de la API

Una vez que la API esté ejecutándose, puedes acceder a la documentación interactiva de Swagger en:

**http://localhost:5233/swagger**

Desde Swagger podrás:
- Ver todos los endpoints disponibles
- Probar las llamadas a la API
- Ver los esquemas de datos
- Obtener ejemplos de request/response

## 🔐 Autenticación

La API utiliza JWT (JSON Web Tokens) para autenticación. Los endpoints protegidos requieren el header:

```
Authorization: Bearer <tu-jwt-token>
```

### Credenciales por Defecto (Owner)
El sistema crea automáticamente un usuario propietario de prueba si la base de datos está vacía:
- **Email**: `owner@test.com`
- **Password**: `TestPassword123!`

Usa el endpoint `/api/Auth/login` para obtener el token.

## 🏠 Funcionalidades principales

### Propiedades
- **GET /api/Properties**: Listar propiedades con paginación y filtros
- **GET /api/Properties/{id}**: Obtener propiedad por ID
- **GET /api/Properties/owner/{ownerId}**: Obtener propiedades de un propietario
- **POST /api/Properties**: Crear nueva propiedad (Protegido)
- **POST /api/Properties/{id}/images**: Agregar imagen a propiedad (Protegido)
- **PATCH /api/Properties/{id}/price**: Cambiar precio de propiedad (Protegido)
- **PUT /api/Properties/{id}**: Actualizar propiedad (Protegido)
- **DELETE /api/Properties/{id}**: Eliminar propiedad (Protegido)

### Propietarios (Owners)
- **GET /api/Owners**: Listar propietarios
- **GET /api/Owners/{id}**: Obtener propietario por ID

### Imágenes de Propiedades
- **POST /api/PropertyImages**: Subir imagen (Protegido)
- **GET /api/PropertyImages/property/{propertyId}**: Obtener imágenes de una propiedad
- **DELETE /api/PropertyImages/{id}**: Eliminar imagen

### Trazas de Propiedades
- **GET /api/PropertyTraces/property/{propertyId}**: Obtener historial de una propiedad
- **POST /api/PropertyTraces**: Crear nueva traza (Protegido)

## 🗃️ Estructura de datos

### Property (Propiedad)
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

## 🌱 Datos de prueba

La aplicación incluye un seeder que carga datos de prueba automáticamente al iniciar:
- 3 propietarios de ejemplo
- 6 propiedades con imágenes
- Historial de trazas para algunas propiedades

## 🛠️ Desarrollo

### Ejecutar en modo desarrollo

```bash
cd src/Million.Api
dotnet watch run
```

### Ejecutar tests

```bash
dotnet test
```

### Compilar solución completa

```bash
dotnet build
```

## 📁 Estructura del proyecto

```
Million.RealEstate/
├── docker/
│   └── docker-compose.yml          # Configuración de servicios Docker
├── src/
│   ├── Million.Api/                # Capa de presentación
│   ├── Million.Application/        # Lógica de aplicación
│   ├── Million.Domain/             # Entidades de dominio
│   ├── Million.Infrastructure/     # Persistencia y servicios
│   └── Million.Tests/              # Pruebas unitarias
├── Million.RealEstate.sln
└── README.md
```
