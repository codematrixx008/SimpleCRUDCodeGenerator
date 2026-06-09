using CodeGen.Core.Abstractions;
using CodeGen.Core.Schema;
using CodeGen.Core.Services;
using CodeGen.Core.Services.Backend;
using CodeGen.Core.Services.Frontend;
using CodeGen.Core.Services.Shared;
using CodeGen.Core.Settings;
using CodeGen.Infrastructure;
using CodeGen.Infrastructure.Schema;

var builder = WebApplication.CreateBuilder(args);

var codeGenSettings = builder.Configuration.GetSection("CodeGen").Get<CodeGenSettings>() ?? new CodeGenSettings();
codeGenSettings.ResolveRelativePaths(builder.Environment.ContentRootPath);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("ConnectionStrings:Default is not configured.");

builder.Services.AddSingleton(codeGenSettings);
builder.Services.AddSingleton<ISchemaRepository>(_ => new SqlSchemaRepository(connectionString, codeGenSettings));
builder.Services.AddSingleton<ITemplateRepository, FileTemplateRepository>();
builder.Services.AddSingleton<IOutputWriter, FileSystemOutputWriter>();

builder.Services.AddSingleton<EntityNamingService>();
builder.Services.AddSingleton<EntitySchemaBuilder>();
builder.Services.AddSingleton<SimpleTemplateEngine>();

builder.Services.AddSingleton<BackendTemplateTokenBuilder>();
builder.Services.AddSingleton<BackendCodeGeneratorService>();

builder.Services.AddSingleton<ReactTemplateTokenBuilder>();
builder.Services.AddSingleton<FrontendCodeGeneratorService>();

builder.Services.AddSingleton<FullStackCodeGeneratorService>();

builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger in all environments because this is a local/dev code generator.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Simple Employee CRUD Code Generator API v1");
    options.RoutePrefix = "swagger";
});

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapGet("/info", () => Results.Ok(new
{
    name = "Simple Employee CRUD Code Generator",
    schemaSource = "SQL Server stored procedure",
    swagger = "/swagger",
    services = new[]
    {
        "BackendCodeGeneratorService",
        "FrontendCodeGeneratorService",
        "FullStackCodeGeneratorService"
    },
    endpoints = new[]
    {
        "POST /api/generator/backend/preview?tableName=dbo.tblEmployee&solutionName=SimpleEmployeeCRUD",
        "POST /api/generator/backend/generate?tableName=dbo.tblEmployee&solutionName=SimpleEmployeeCRUD",
        "POST /api/generator/frontend/preview?tableName=dbo.tblEmployee&frontendAppName=SimpleEmployeeCRUD.React",
        "POST /api/generator/frontend/generate?tableName=dbo.tblEmployee&frontendAppName=SimpleEmployeeCRUD.React",
        "POST /api/generator/fullstack/preview?tableName=dbo.tblEmployee&solutionName=SimpleEmployeeCRUD&frontendAppName=SimpleEmployeeCRUD.React",
        "POST /api/generator/fullstack/generate?tableName=dbo.tblEmployee&solutionName=SimpleEmployeeCRUD&frontendAppName=SimpleEmployeeCRUD.React"
    }
}));

app.MapControllers();
app.Run();
