using CodeGen.Core.Abstractions;
using CodeGen.Core.Models;
using CodeGen.Core.Schema;
using CodeGen.Core.Services.Shared;

namespace CodeGen.Core.Services;

public sealed class CodeGeneratorService
{
    private static readonly IReadOnlyList<TemplateDefinition> Templates = new List<TemplateDefinition>
    {
        new("ApiController.tpl", "{{SolutionName}}.Api/Controllers/{{EntityPlural}}Controller.cs"),
        new("DomainModel.tpl", "{{SolutionName}}.Domain/Models/{{EntityName}}.cs"),
        new("Dto.tpl", "{{SolutionName}}.Domain/DTOs/{{EntityName}}Dto.cs"),
        new("CreateDto.tpl", "{{SolutionName}}.Domain/DTOs/Create{{EntityName}}Dto.cs"),
        new("UpdateDto.tpl", "{{SolutionName}}.Domain/DTOs/Update{{EntityName}}Dto.cs"),
        new("RepositoryInterface.tpl", "{{SolutionName}}.Domain/Interfaces/I{{EntityName}}Repository.cs"),
        new("SqlConnectionFactory.tpl", "{{SolutionName}}.Infrastructure/Data/SqlConnectionFactory.cs"),
        new("Repository.tpl", "{{SolutionName}}.Infrastructure/Repositories/{{EntityName}}Repository.cs"),
        new("DependencyInjection.tpl", "{{SolutionName}}.Infrastructure/DependencyInjection.cs"),
        new("ProgramPatch.tpl", "_patches/{{SolutionName}}.Api.Program.cs.patch.txt"),
        new("AppSettingsPatch.tpl", "_patches/{{SolutionName}}.Api.appsettings.json.patch.txt")
    };

    private readonly ISchemaRepository _schemaRepository;
    private readonly ITemplateRepository _templateRepository;
    private readonly IOutputWriter _outputWriter;
    private readonly TemplateTokenBuilder _tokenBuilder;
    private readonly SimpleTemplateEngine _templateEngine;
    private readonly EntityNamingService _naming;

    public CodeGeneratorService(
        ISchemaRepository schemaRepository,
        ITemplateRepository templateRepository,
        IOutputWriter outputWriter,
        TemplateTokenBuilder tokenBuilder,
        SimpleTemplateEngine templateEngine,
        EntityNamingService naming)
    {
        _schemaRepository = schemaRepository;
        _templateRepository = templateRepository;
        _outputWriter = outputWriter;
        _tokenBuilder = tokenBuilder;
        _templateEngine = templateEngine;
        _naming = naming;
    }

    public async Task<GenerationResult> PreviewAsync(string tableName, string solutionName, CancellationToken cancellationToken = default)
    {
        var (schema, files) = await BuildFilesAsync(tableName, solutionName, cancellationToken);
        return ToResult(files, solutionName, tableName, schema.EntityName, filesWritten: false, includeContent: true);
    }

    public async Task<GenerationResult> GenerateAsync(string tableName, string solutionName, CancellationToken cancellationToken = default)
    {
        var (schema, files) = await BuildFilesAsync(tableName, solutionName, cancellationToken);
        await _outputWriter.WriteAsync(files, cancellationToken);
        return ToResult(files, solutionName, tableName, schema.EntityName, filesWritten: true, includeContent: false);
    }

    private async Task<(EntitySchema Schema, IReadOnlyList<GeneratedFile> Files)> BuildFilesAsync(
        string tableName,
        string solutionName,
        CancellationToken cancellationToken)
    {
        _naming.ValidateTableName(tableName);
        _naming.ValidateSolutionName(solutionName);

        var schemaResult = await _schemaRepository.GetSchemaAsync(tableName);
        var schema = BuildEntitySchema(tableName, schemaResult);
        var tokens = _tokenBuilder.Build(schema, solutionName);
        var files = new List<GeneratedFile>();

        foreach (var templateDefinition in Templates)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var template = await _templateRepository.GetTemplateAsync(templateDefinition.TemplateName, cancellationToken);
            var content = _templateEngine.Render(template, tokens);
            var relativePath = _templateEngine.Render(templateDefinition.OutputPathTemplate, tokens).Replace('\\', '/');

            files.Add(new GeneratedFile
            {
                RelativePath = relativePath,
                Content = content
            });
        }

        return (schema, files);
    }

    private EntitySchema BuildEntitySchema(string tableName, SchemaResult schemaResult)
    {
        if (schemaResult.TableColumns.Count == 0)
        {
            throw new InvalidOperationException($"No columns were returned for table '{tableName}'. Check the configured schema stored procedure.");
        }

        var (schemaName, tableOnlyName) = _naming.SplitSchemaAndTableName(tableName);
        var entityName = _naming.ToEntityNameFromTableName(tableName);
        var fields = schemaResult.TableColumns
            .OrderBy(x => x.OrdinalPosition)
            .Select(MapColumnToField)
            .ToList();

        var key = fields.FirstOrDefault(x => x.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
        if (key is not null)
        {
            key.IsKey = true;
        }

        if (!fields.Any(x => x.IsKey))
        {
            throw new InvalidOperationException($"Table '{tableName}' must have an Id column for this simple CRUD generator.");
        }

        return new EntitySchema
        {
            EntityName = entityName,
            EntityPlural = _naming.ToPlural(entityName),
            DbSchemaName = schemaName,
            DbTableName = tableOnlyName,
            Fields = fields
        };
    }

    private EntityField MapColumnToField(DbColumnSchema column)
    {
        var propertyName = _naming.NormalizeEntityName(column.ColumnName);
        var isId = propertyName.Equals("Id", StringComparison.OrdinalIgnoreCase);

        var newEntityField = new EntityField
        {
            Name = propertyName,
            ColumnName = column.ColumnName,
            Type = column.SqlType,
            IsKey = isId,
            IsIdentity = column.IsIdentity,
            IsNullable = column.IsNullable,
            MaxLength = NormalizeMaxLength(column),
            Precision = column.Precision,
            Scale = column.Scale,
            IncludeInCreate = !isId && !column.IsIdentity && !IsAuditColumn(propertyName),
            IncludeInUpdate = !isId && !column.IsIdentity && !IsAuditColumn(propertyName)
        };
        return newEntityField;
    }

    private static int? NormalizeMaxLength(DbColumnSchema column)
    {
        if (column.MaxLength is null or <= 0)
        {
            return null;
        }

        var type = column.SqlType.ToLowerInvariant();
        if (type is "nvarchar" or "nchar")
        {
            return column.MaxLength.Value / 2;
        }

        return column.MaxLength;
    }

    private static bool IsAuditColumn(string propertyName)
    {
        return propertyName.Equals("CreatedDate", StringComparison.OrdinalIgnoreCase)
            || propertyName.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase)
            || propertyName.Equals("CreatedBy", StringComparison.OrdinalIgnoreCase)
            || propertyName.Equals("UpdatedDate", StringComparison.OrdinalIgnoreCase)
            || propertyName.Equals("UpdatedAt", StringComparison.OrdinalIgnoreCase)
            || propertyName.Equals("ModifiedDate", StringComparison.OrdinalIgnoreCase)
            || propertyName.Equals("ModifiedAt", StringComparison.OrdinalIgnoreCase)
            || propertyName.Equals("UpdatedBy", StringComparison.OrdinalIgnoreCase)
            || propertyName.Equals("ModifiedBy", StringComparison.OrdinalIgnoreCase)
            || propertyName.Equals("IsDeleted", StringComparison.OrdinalIgnoreCase);
    }

    private static GenerationResult ToResult(
        IEnumerable<GeneratedFile> files,
        string solutionName,
        string tableName,
        string entityName,
        bool filesWritten,
        bool includeContent)
    {
        return new GenerationResult
        {
            SolutionName = solutionName,
            TableName = tableName,
            EntityName = entityName,
            FilesWritten = filesWritten,
            Files = files.Select(file => new GeneratedFileResult
            {
                RelativePath = file.RelativePath,
                Content = includeContent ? file.Content : null
            }).ToList()
        };
    }
}
