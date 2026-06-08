# Simple Employee CRUD Code Generator - 4 Project Dapper Version

This project follows the same high-level generator structure as your existing CodeGen solution:

```text
CodeGen.sln
├── CodeGen.Api
├── CodeGen.Core
├── CodeGen.GenerationFiles
└── CodeGen.Infrastructure
```

This version is intentionally simple. It generates a basic Employee-style CRUD module for a target solution such as `SimpleEmployeeCRUD`.

The generated CRUD project uses **Dapper**, not Entity Framework.

## Generator flow

```text
POST /api/generator/generate?tableName=dbo.tblEmployee&solutionName=SimpleEmployeeCRUD
        ↓
CodeGen.Api / GeneratorController
        ↓
CodeGen.Core / CodeGeneratorService
        ↓
CodeGen.Infrastructure / SqlSchemaRepository
        ↓
dbo.usp_GetObjectSchemas @TableName
        ↓
CodeGen.Core / TemplateTokenBuilder
        ↓
CodeGen.GenerationFiles/Templates/*.tpl
        ↓
CodeGen.GenerationFiles/GeneratedOutput
```

## Schema source

The generator reads table columns from SQL Server. It does not read fields from JSON.

The API receives a table name:

```http
POST /api/generator/generate?tableName=dbo.tblEmployee&solutionName=SimpleEmployeeCRUD
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

Preview:

```http
POST http://localhost:5144/api/generator/preview?tableName=dbo.tblEmployee&solutionName=SimpleEmployeeCRUD
```

Generate:

```http
POST http://localhost:5144/api/generator/generate?tableName=dbo.tblEmployee&solutionName=SimpleEmployeeCRUD
```

Generated files are written to:

```text
CodeGen.GenerationFiles/GeneratedOutput
```

## Generated output structure

For `tableName=dbo.tblEmployee` and `solutionName=SimpleEmployeeCRUD`, output is:

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

_patches
├── SimpleEmployeeCRUD.Api.Program.cs.patch.txt
└── SimpleEmployeeCRUD.Api.appsettings.json.patch.txt
```

## Runtime flow of generated CRUD project

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

## How to copy generated files into your Visual Studio solution

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

## Required NuGet packages for generated target project

Install these in `SimpleEmployeeCRUD.Infrastructure`:

```bash
dotnet add SimpleEmployeeCRUD.Infrastructure package Dapper
dotnet add SimpleEmployeeCRUD.Infrastructure package Microsoft.Data.SqlClient
dotnet add SimpleEmployeeCRUD.Infrastructure package Microsoft.Extensions.Configuration.Abstractions
```

No Entity Framework package is required.

## Update generated target API Program.cs

In `SimpleEmployeeCRUD.Api/Program.cs`, add:

```csharp
using SimpleEmployeeCRUD.Infrastructure;
```

Then add this before `var app = builder.Build();`:

```csharp
builder.Services.AddInfrastructure(builder.Configuration);
```

## Update generated target API appsettings.json

Add:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=SimpleEmployeeCRUDDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

The generated `SqlConnectionFactory` expects the name `DefaultConnection`.
