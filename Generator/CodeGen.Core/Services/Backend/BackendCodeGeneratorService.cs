using CodeGen.Core.Abstractions;
using CodeGen.Core.Models;
using CodeGen.Core.Schema;
using CodeGen.Core.Services.Shared;

namespace CodeGen.Core.Services.Backend;

public sealed class BackendCodeGeneratorService
{
    private static readonly IReadOnlyList<TemplateDefinition> Templates = new List<TemplateDefinition>
    {
        new("Backend/ApiController.tpl", "{{SolutionName}}.Api/Controllers/{{EntityPlural}}Controller.cs"),
        new("Backend/DomainModel.tpl", "{{SolutionName}}.Domain/Models/{{EntityName}}.cs"),
        new("Backend/Dto.tpl", "{{SolutionName}}.Domain/DTOs/{{EntityName}}Dto.cs"),
        new("Backend/CreateDto.tpl", "{{SolutionName}}.Domain/DTOs/Create{{EntityName}}Dto.cs"),
        new("Backend/UpdateDto.tpl", "{{SolutionName}}.Domain/DTOs/Update{{EntityName}}Dto.cs"),
        new("Backend/RepositoryInterface.tpl", "{{SolutionName}}.Domain/Interfaces/I{{EntityName}}Repository.cs"),
        new("Backend/SqlConnectionFactory.tpl", "{{SolutionName}}.Infrastructure/Data/SqlConnectionFactory.cs"),
        new("Backend/Repository.tpl", "{{SolutionName}}.Infrastructure/Repositories/{{EntityName}}Repository.cs"),
        new("Backend/DependencyInjection.tpl", "{{SolutionName}}.Infrastructure/DependencyInjection.cs"),
        new("Backend/ProgramPatch.tpl", "_patches/{{SolutionName}}.Api.Program.cs.patch.txt"),
        new("Backend/AppSettingsPatch.tpl", "_patches/{{SolutionName}}.Api.appsettings.json.patch.txt")
    };

    private readonly ISchemaRepository _schemaRepository;
    private readonly ITemplateRepository _templateRepository;
    private readonly IOutputWriter _outputWriter;
    private readonly BackendTemplateTokenBuilder _tokenBuilder;
    private readonly SimpleTemplateEngine _templateEngine;
    private readonly EntitySchemaBuilder _schemaBuilder;
    private readonly EntityNamingService _naming;

    public BackendCodeGeneratorService(
        ISchemaRepository schemaRepository,
        ITemplateRepository templateRepository,
        IOutputWriter outputWriter,
        BackendTemplateTokenBuilder tokenBuilder,
        SimpleTemplateEngine templateEngine,
        EntitySchemaBuilder schemaBuilder,
        EntityNamingService naming)
    {
        _schemaRepository = schemaRepository;
        _templateRepository = templateRepository;
        _outputWriter = outputWriter;
        _tokenBuilder = tokenBuilder;
        _templateEngine = templateEngine;
        _schemaBuilder = schemaBuilder;
        _naming = naming;
    }

    public async Task<GenerationResult> PreviewAsync(
        string tableName,
        string solutionName,
        IReadOnlyList<RelationDefinition>? relations = null,
        CancellationToken cancellationToken = default)
    {
        var (schema, files) = await BuildFilesAsync(tableName, solutionName, relations, cancellationToken);
        return ToResult(files, solutionName, tableName, schema.EntityName, filesWritten: false, includeContent: true);
    }

    public async Task<GenerationResult> GenerateAsync(
        string tableName,
        string solutionName,
        IReadOnlyList<RelationDefinition>? relations = null,
        CancellationToken cancellationToken = default)
    {
        var (schema, files) = await BuildFilesAsync(tableName, solutionName, relations, cancellationToken);
        await _outputWriter.WriteAsync(files, cancellationToken);
        return ToResult(files, solutionName, tableName, schema.EntityName, filesWritten: true, includeContent: false);
    }

    private async Task<(EntitySchema Schema, IReadOnlyList<GeneratedFile> Files)> BuildFilesAsync(
        string tableName,
        string solutionName,
        IReadOnlyList<RelationDefinition>? relations,
        CancellationToken cancellationToken)
    {
        _naming.ValidateTableName(tableName);
        _naming.ValidateSolutionName(solutionName);

        var schemaResult = await _schemaRepository.GetSchemaAsync(tableName);
        var schema = _schemaBuilder.Build(tableName, schemaResult, relations);
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
