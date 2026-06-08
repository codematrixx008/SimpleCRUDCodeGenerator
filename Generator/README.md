# Simple Employee CRUD Code Generator - 4 Project Version

This sample follows the same high-level structure as your existing generator:

```text
CodeGen.sln
├── CodeGen.Api
├── CodeGen.Core
├── CodeGen.GenerationFiles
└── CodeGen.Infrastructure
```

The main difference is that this version is intentionally simple. It generates only a basic CRUD module for a target solution such as `SimpleEmployeeCRUD`.

## What changed from the earlier JSON version

The generator **does not read entity fields from JSON**.

It reads the schema from SQL Server through:

```csharp
CodeGen.Core.Schema.ISchemaRepository
CodeGen.Infrastructure.Schema.SqlSchemaRepository
```

The API receives a table name:

```http
POST /api/generator/generate?tableName=dbo.tblEmployee&solutionName=SimpleEmployeeCRUD
```

Then `SqlSchemaRepository` calls the configured stored procedure:

```json
"SchemaStoredProcedure": "dbo.usp_GetObjectSchemas"
```

The stored procedure must return two result sets:

```text
1. Table columns
2. Search-result columns
```

For this simple CRUD generator, the second result set can be the same as the first one.

## Required SQL stored procedure

A starter script is included here:

```text
CodeGen.GenerationFiles/Sql/usp_GetObjectSchemas.sql
```

Run it in your SQL Server database before calling the generator.

The result shape must match:

```csharp
public sealed class DbColumnSchema
{
    public string ColumnName { get; set; }
    public string SqlType { get; set; }
    public int? MaxLength { get; set; }
    public int? Precision { get; set; }
    public int? Scale { get; set; }
    public bool IsNullable { get; set; }
    public bool IsIdentity { get; set; }
    public int OrdinalPosition { get; set; }
}
```

## Configure database connection

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

## Run generator

```bash
dotnet restore
dotnet run --project CodeGen.Api
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

## Table-name to entity-name convention

Examples:

```text
dbo.tblEmployee -> Employee -> EmployeesController
dbo.Employee    -> Employee -> EmployeesController
tblDepartment   -> Department -> DepartmentsController
```

This simple generator expects the table to have an `Id` column. That column is used as the primary key in the generated repository and controller.

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
│   └── AppDbContext.cs
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
AppDbContext
    ↓
SQL Server
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

In `SimpleEmployeeCRUD.Infrastructure`:

```bash
dotnet add SimpleEmployeeCRUD.Infrastructure package Microsoft.EntityFrameworkCore.SqlServer
dotnet add SimpleEmployeeCRUD.Infrastructure package Microsoft.EntityFrameworkCore.Design
```

In `SimpleEmployeeCRUD.Api`:

```bash
dotnet add SimpleEmployeeCRUD.Api package Microsoft.EntityFrameworkCore.Design
```

## Update generated target API Program.cs

In `SimpleEmployeeCRUD.Api/Program.cs`, add:

```csharp
using SimpleEmployeeCRUD.Infrastructure;
```

Before:

```csharp
var app = builder.Build();
```

add:

```csharp
builder.Services.AddInfrastructure(builder.Configuration);
```

## Update generated target appsettings.json

In `SimpleEmployeeCRUD.Api/appsettings.json`, add:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=SimpleEmployeeCRUDDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

## Create database tables with migrations

If your generated target project should create its own database:

```bash
dotnet ef migrations add InitialCreate --project SimpleEmployeeCRUD.Infrastructure --startup-project SimpleEmployeeCRUD.Api

dotnet ef database update --project SimpleEmployeeCRUD.Infrastructure --startup-project SimpleEmployeeCRUD.Api
```

If you are pointing the generated target project to an existing database/table, review the generated `AppDbContext` mapping. The template includes `ToTable(...)` and `HasColumnName(...)` mapping based on the SQL table schema.
