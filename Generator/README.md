# Simple Employee CRUD Code Generator - Backend + React Frontend

This project follows the same high-level 4-project generator structure as your existing CodeGen solution:

```text
CodeGen.sln
├── CodeGen.Api
├── CodeGen.Core
├── CodeGen.GenerationFiles
└── CodeGen.Infrastructure
```

It generates simple CRUD project files from a SQL Server table schema. Backend generation uses **Dapper**, not Entity Framework. Frontend generation creates **React TypeScript** feature files.

## Core service structure

```text
CodeGen.Core/Services
├── Backend
│   ├── BackendCodeGeneratorService.cs
│   └── BackendTemplateTokenBuilder.cs
│
├── Frontend
│   ├── FrontendCodeGeneratorService.cs
│   └── ReactTemplateTokenBuilder.cs
│
├── Shared
│   ├── EntitySchemaBuilder.cs
│   ├── EntityNamingService.cs
│   └── SimpleTemplateEngine.cs
│
└── FullStackCodeGeneratorService.cs
```

## Generator flow

```text
POST /api/generator/fullstack/generate?tableName=dbo.tblEmployee&solutionName=SimpleEmployeeCRUD&frontendAppName=SimpleEmployeeCRUD.React
        ↓
CodeGen.Api / GeneratorController
        ↓
FullStackCodeGeneratorService
        ↓
BackendCodeGeneratorService + FrontendCodeGeneratorService
        ↓
SqlSchemaRepository
        ↓
dbo.usp_GetObjectSchemas @TableName
        ↓
EntitySchemaBuilder
        ↓
BackendTemplateTokenBuilder / ReactTemplateTokenBuilder
        ↓
Backend .tpl files / React .tpl files
        ↓
CodeGen.GenerationFiles/GeneratedOutput
```

## Template folders

```text
CodeGen.GenerationFiles/Templates
├── Backend
│   ├── ApiController.tpl
│   ├── DomainModel.tpl
│   ├── Repository.tpl
│   └── ...
│
└── Frontend
    └── ReactTs
        ├── Model.tpl
        ├── CreateRequest.tpl
        ├── UpdateRequest.tpl
        ├── ApiClient.tpl
        ├── Service.tpl
        ├── Form.tpl
        ├── ListPage.tpl
        ├── CreatePage.tpl
        ├── EditPage.tpl
        ├── RoutePatch.tpl
        └── PackageNotes.tpl
```

## Schema source

The generator reads table columns from SQL Server. It does not read fields from JSON.

The API receives a table name:

```http
POST /api/generator/backend/generate?tableName=dbo.tblEmployee&solutionName=SimpleEmployeeCRUD
```

Then `SqlSchemaRepository` calls:

```json
"SchemaStoredProcedure": "dbo.usp_GetObjectSchemas"
```

The stored procedure must return two result sets:

```text
1. Table columns
2. Search-result columns
```

For this simple CRUD generator, the second result set can be the same as the first result set.

A starter script is included here:

```text
CodeGen.GenerationFiles/Sql/usp_GetObjectSchemas.sql
```

## Generator database config

Edit:

```text
CodeGen.Api/appsettings.json
```

Example:

```json
{
  "ConnectionStrings": {
    "Default": "Server=.;Database=SimpleEmployeeCRUDDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
  "CodeGen": {
    "TemplatesPath": "../CodeGen.GenerationFiles/Templates",
    "OutputPath": "../CodeGen.GenerationFiles/GeneratedOutput",
    "SchemaStoredProcedure": "dbo.usp_GetObjectSchemas"
  }
}
```

## Employee table for testing

Run this in the database configured above:

```sql
CREATE TABLE dbo.tblEmployee
(
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_tblEmployee PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    DOB DATE NOT NULL,
    Gender NVARCHAR(10) NOT NULL,
    Address NVARCHAR(250) NULL,
    CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_tblEmployee_CreatedDate DEFAULT SYSUTCDATETIME(),
    UpdatedDate DATETIME2(7) NULL,
    IsDeleted BIT NOT NULL CONSTRAINT DF_tblEmployee_IsDeleted DEFAULT 0
);
```

Then test:

```sql
EXEC dbo.usp_GetObjectSchemas @TableName = 'dbo.tblEmployee';
```

## Run generator

```bash
dotnet restore
dotnet run --project CodeGen.Api
```

Swagger UI is enabled:

```text
http://localhost:5144/swagger
```

## Endpoints

Backward-compatible backend-only endpoints:

```http
POST /api/generator/preview?tableName=dbo.tblEmployee&solutionName=SimpleEmployeeCRUD
POST /api/generator/generate?tableName=dbo.tblEmployee&solutionName=SimpleEmployeeCRUD
```

Backend-only endpoints:

```http
POST /api/generator/backend/preview?tableName=dbo.tblEmployee&solutionName=SimpleEmployeeCRUD
POST /api/generator/backend/generate?tableName=dbo.tblEmployee&solutionName=SimpleEmployeeCRUD
```

Frontend-only endpoints:

```http
POST /api/generator/frontend/preview?tableName=dbo.tblEmployee&frontendAppName=SimpleEmployeeCRUD.React
POST /api/generator/frontend/generate?tableName=dbo.tblEmployee&frontendAppName=SimpleEmployeeCRUD.React
```

Full-stack endpoints:

```http
POST /api/generator/fullstack/preview?tableName=dbo.tblEmployee&solutionName=SimpleEmployeeCRUD&frontendAppName=SimpleEmployeeCRUD.React
POST /api/generator/fullstack/generate?tableName=dbo.tblEmployee&solutionName=SimpleEmployeeCRUD&frontendAppName=SimpleEmployeeCRUD.React
```

Generated files are written to:

```text
CodeGen.GenerationFiles/GeneratedOutput
```

## Generated backend output

For `tableName=dbo.tblEmployee` and `solutionName=SimpleEmployeeCRUD`, backend output is:

```text
SimpleEmployeeCRUD.Api
└── Controllers
    └── EmployeesController.cs

SimpleEmployeeCRUD.Domain
├── Models
│   └── Employee.cs
├── DTOs
│   ├── EmployeeDto.cs
│   ├── CreateEmployeeDto.cs
│   └── UpdateEmployeeDto.cs
└── Interfaces
    └── IEmployeeRepository.cs

SimpleEmployeeCRUD.Infrastructure
├── Data
│   └── SqlConnectionFactory.cs
├── Repositories
│   └── EmployeeRepository.cs
└── DependencyInjection.cs
```

Generated backend runtime flow:

```text
HTTP request
    ↓
EmployeesController
    ↓
IEmployeeRepository
    ↓
EmployeeRepository
    ↓
Dapper
    ↓
SqlConnectionFactory
    ↓
SQL Server dbo.tblEmployee
```

## Generated frontend output

For `frontendAppName=SimpleEmployeeCRUD.React`, frontend output is:

```text
SimpleEmployeeCRUD.React/src/features/employees
├── models
│   ├── Employee.ts
│   ├── CreateEmployeeRequest.ts
│   └── UpdateEmployeeRequest.ts
│
├── api
│   └── employeesApi.ts
│
├── services
│   └── employeesService.ts
│
├── components
│   └── EmployeeForm.tsx
│
└── pages
    ├── EmployeesListPage.tsx
    ├── CreateEmployeePage.tsx
    └── EditEmployeePage.tsx
```

Generated frontend runtime flow:

```text
EmployeesListPage / CreateEmployeePage / EditEmployeePage
    ↓
EmployeeForm
    ↓
employeesService
    ↓
employeesApi
    ↓
fetch()
    ↓
/api/Employees
```

## How to copy generated backend files into your Visual Studio solution

Create a new Visual Studio solution named:

```text
SimpleEmployeeCRUD
```

Add these projects:

```text
SimpleEmployeeCRUD.Api             ASP.NET Core Web API
SimpleEmployeeCRUD.Domain          Class Library
SimpleEmployeeCRUD.Infrastructure  Class Library
```

Add project references:

```text
SimpleEmployeeCRUD.Api -> SimpleEmployeeCRUD.Domain
SimpleEmployeeCRUD.Api -> SimpleEmployeeCRUD.Infrastructure
SimpleEmployeeCRUD.Infrastructure -> SimpleEmployeeCRUD.Domain
```

Copy files from:

```text
CodeGen.GenerationFiles/GeneratedOutput/SimpleEmployeeCRUD.Api
CodeGen.GenerationFiles/GeneratedOutput/SimpleEmployeeCRUD.Domain
CodeGen.GenerationFiles/GeneratedOutput/SimpleEmployeeCRUD.Infrastructure
```

into the matching Visual Studio projects.

## Required NuGet packages for generated backend target project

Install these in `SimpleEmployeeCRUD.Infrastructure`:

```bash
dotnet add SimpleEmployeeCRUD.Infrastructure package Dapper
dotnet add SimpleEmployeeCRUD.Infrastructure package Microsoft.Data.SqlClient
dotnet add SimpleEmployeeCRUD.Infrastructure package Microsoft.Extensions.Configuration.Abstractions
```

No Entity Framework package is required.

## How to copy generated frontend files into your React app

Create or open a React TypeScript app. A Vite app is a good simple target:

```bash
npm create vite@latest SimpleEmployeeCRUD.React -- --template react-ts
cd SimpleEmployeeCRUD.React
npm install
npm install react-router-dom
```

Copy generated files from:

```text
CodeGen.GenerationFiles/GeneratedOutput/SimpleEmployeeCRUD.React/src/features/employees
```

to:

```text
SimpleEmployeeCRUD.React/src/features/employees
```

Then apply the route patch from:

```text
CodeGen.GenerationFiles/GeneratedOutput/_patches/SimpleEmployeeCRUD.React.routes.patch.txt
```

If the backend API is running on a different origin, add `.env` in the React app:

```text
VITE_API_BASE_URL=http://localhost:5144
```
