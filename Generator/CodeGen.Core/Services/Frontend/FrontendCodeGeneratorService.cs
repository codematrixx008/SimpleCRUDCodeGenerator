using CodeGen.Core.Abstractions;
using CodeGen.Core.Models;
using CodeGen.Core.Schema;
using CodeGen.Core.Services.Shared;

namespace CodeGen.Core.Services.Frontend;

public sealed class FrontendCodeGeneratorService
{
    private static readonly IReadOnlyList<TemplateDefinition> Templates = new List<TemplateDefinition>
    {
        new("Frontend/ReactTs/Model.tpl", "{{FrontendAppName}}/src/features/{{FeatureFolder}}/models/{{EntityName}}.ts"),
        new("Frontend/ReactTs/CreateRequest.tpl", "{{FrontendAppName}}/src/features/{{FeatureFolder}}/models/Create{{EntityName}}Request.ts"),
        new("Frontend/ReactTs/UpdateRequest.tpl", "{{FrontendAppName}}/src/features/{{FeatureFolder}}/models/Update{{EntityName}}Request.ts"),
        new("Frontend/ReactTs/ApiClient.tpl", "{{FrontendAppName}}/src/features/{{FeatureFolder}}/api/{{EntityPluralVariable}}Api.ts"),
        new("Frontend/ReactTs/Service.tpl", "{{FrontendAppName}}/src/features/{{FeatureFolder}}/services/{{EntityPluralVariable}}Service.ts"),
        new("Frontend/ReactTs/Form.tpl", "{{FrontendAppName}}/src/features/{{FeatureFolder}}/components/{{EntityName}}Form.tsx"),
        new("Frontend/ReactTs/ListPage.tpl", "{{FrontendAppName}}/src/features/{{FeatureFolder}}/pages/{{EntityPlural}}ListPage.tsx"),
        new("Frontend/ReactTs/CreatePage.tpl", "{{FrontendAppName}}/src/features/{{FeatureFolder}}/pages/Create{{EntityName}}Page.tsx"),
        new("Frontend/ReactTs/EditPage.tpl", "{{FrontendAppName}}/src/features/{{FeatureFolder}}/pages/Edit{{EntityName}}Page.tsx"),
        new("Frontend/ReactTs/RoutePatch.tpl", "_patches/{{FrontendAppName}}.routes.patch.txt"),
        new("Frontend/ReactTs/PackageNotes.tpl", "_patches/{{FrontendAppName}}.package-notes.txt")
    };

    private readonly ISchemaRepository _schemaRepository;
    private readonly ITemplateRepository _templateRepository;
    private readonly IOutputWriter _outputWriter;
    private readonly ReactTemplateTokenBuilder _tokenBuilder;
    private readonly SimpleTemplateEngine _templateEngine;
    private readonly EntitySchemaBuilder _schemaBuilder;
    private readonly EntityNamingService _naming;

    public FrontendCodeGeneratorService(
        ISchemaRepository schemaRepository,
        ITemplateRepository templateRepository,
        IOutputWriter outputWriter,
        ReactTemplateTokenBuilder tokenBuilder,
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

    public async Task<GenerationResult> PreviewAsync(string tableName, string frontendAppName, CancellationToken cancellationToken = default)
    {
        var (schema, files) = await BuildFilesAsync(tableName, frontendAppName, cancellationToken);
        return ToResult(files, frontendAppName, tableName, schema.EntityName, filesWritten: false, includeContent: true);
    }

    public async Task<GenerationResult> GenerateAsync(string tableName, string frontendAppName, CancellationToken cancellationToken = default)
    {
        var (schema, files) = await BuildFilesAsync(tableName, frontendAppName, cancellationToken);
        await _outputWriter.WriteAsync(files, cancellationToken);
        return ToResult(files, frontendAppName, tableName, schema.EntityName, filesWritten: true, includeContent: false);
    }

    private async Task<(EntitySchema Schema, IReadOnlyList<GeneratedFile> Files)> BuildFilesAsync(
        string tableName,
        string frontendAppName,
        CancellationToken cancellationToken)
    {
        _naming.ValidateTableName(tableName);
        _naming.ValidateSolutionName(frontendAppName);

        var schemaResult = await _schemaRepository.GetSchemaAsync(tableName);
        var schema = _schemaBuilder.Build(tableName, schemaResult);
        var tokens = _tokenBuilder.Build(schema, frontendAppName);
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
        string frontendAppName,
        string tableName,
        string entityName,
        bool filesWritten,
        bool includeContent)
    {
        return new GenerationResult
        {
            SolutionName = frontendAppName,
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
