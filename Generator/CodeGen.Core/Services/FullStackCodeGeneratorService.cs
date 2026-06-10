using CodeGen.Core.Models;
using CodeGen.Core.Services.Backend;
using CodeGen.Core.Services.Frontend;

namespace CodeGen.Core.Services;

public sealed class FullStackCodeGeneratorService
{
    private readonly BackendCodeGeneratorService _backendGenerator;
    private readonly FrontendCodeGeneratorService _frontendGenerator;

    public FullStackCodeGeneratorService(
        BackendCodeGeneratorService backendGenerator,
        FrontendCodeGeneratorService frontendGenerator)
    {
        _backendGenerator = backendGenerator;
        _frontendGenerator = frontendGenerator;
    }

    public async Task<GenerationResult> PreviewAsync(
        string tableName,
        string solutionName,
        string frontendAppName,
        IReadOnlyList<RelationDefinition>? relations = null,
        CancellationToken cancellationToken = default)
    {
        var backendResult = await _backendGenerator.PreviewAsync(tableName, solutionName, relations, cancellationToken);
        var frontendResult = await _frontendGenerator.PreviewAsync(tableName, frontendAppName, relations, cancellationToken);

        return MergeResults(backendResult, frontendResult, filesWritten: false);
    }

    public async Task<GenerationResult> GenerateAsync(
        string tableName,
        string solutionName,
        string frontendAppName,
        IReadOnlyList<RelationDefinition>? relations = null,
        CancellationToken cancellationToken = default)
    {
        var backendResult = await _backendGenerator.GenerateAsync(tableName, solutionName, relations, cancellationToken);
        var frontendResult = await _frontendGenerator.GenerateAsync(tableName, frontendAppName, relations, cancellationToken);

        return MergeResults(backendResult, frontendResult, filesWritten: backendResult.FilesWritten && frontendResult.FilesWritten);
    }

    private static GenerationResult MergeResults(GenerationResult backendResult, GenerationResult frontendResult, bool filesWritten)
    {
        return new GenerationResult
        {
            SolutionName = backendResult.SolutionName,
            TableName = backendResult.TableName,
            EntityName = backendResult.EntityName,
            FilesWritten = filesWritten,
            Files = backendResult.Files.Concat(frontendResult.Files).ToList()
        };
    }
}
