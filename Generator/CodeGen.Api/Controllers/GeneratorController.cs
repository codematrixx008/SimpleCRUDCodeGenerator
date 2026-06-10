using CodeGen.Core.Models;
using CodeGen.Core.Services;
using CodeGen.Core.Services.Backend;
using CodeGen.Core.Services.Frontend;
using Microsoft.AspNetCore.Mvc;

namespace CodeGen.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class GeneratorController : ControllerBase
{
    private readonly BackendCodeGeneratorService _backendGenerator;
    private readonly FrontendCodeGeneratorService _frontendGenerator;
    private readonly FullStackCodeGeneratorService _fullStackGenerator;

    public GeneratorController(
        BackendCodeGeneratorService backendGenerator,
        FrontendCodeGeneratorService frontendGenerator,
        FullStackCodeGeneratorService fullStackGenerator)
    {
        _backendGenerator = backendGenerator;
        _frontendGenerator = frontendGenerator;
        _fullStackGenerator = fullStackGenerator;
    }

    // Backward-compatible backend endpoints. These generate no relation-aware code.
    [HttpPost("preview")]
    public Task<ActionResult<GenerationResult>> Preview(
        [FromQuery] string tableName = "dbo.tblEmployee",
        [FromQuery] string solutionName = "GeneratedCrud",
        CancellationToken cancellationToken = default)
    {
        return BackendPreview(tableName, solutionName, cancellationToken);
    }

    [HttpPost("generate")]
    public Task<ActionResult<GenerationResult>> Generate(
        [FromQuery] string tableName = "dbo.tblEmployee",
        [FromQuery] string solutionName = "GeneratedCrud",
        CancellationToken cancellationToken = default)
    {
        return BackendGenerate(tableName, solutionName, cancellationToken);
    }

    [HttpPost("backend/preview")]
    public async Task<ActionResult<GenerationResult>> BackendPreview(
        [FromQuery] string tableName = "dbo.tblEmployee",
        [FromQuery] string solutionName = "GeneratedCrud",
        CancellationToken cancellationToken = default)
    {
        var result = await _backendGenerator.PreviewAsync(tableName, solutionName, cancellationToken: cancellationToken);
        return Ok(result);
    }

    [HttpPost("backend/generate")]
    public async Task<ActionResult<GenerationResult>> BackendGenerate(
        [FromQuery] string tableName = "dbo.tblEmployee",
        [FromQuery] string solutionName = "GeneratedCrud",
        CancellationToken cancellationToken = default)
    {
        var result = await _backendGenerator.GenerateAsync(tableName, solutionName, cancellationToken: cancellationToken);
        return Ok(result);
    }

    [HttpPost("backend/preview-with-relations")]
    public async Task<ActionResult<GenerationResult>> BackendPreviewWithRelations(
        [FromBody] BackendGenerationRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _backendGenerator.PreviewAsync(request.TableName, request.SolutionName, request.Relations, cancellationToken);
        return Ok(result);
    }

    [HttpPost("backend/generate-with-relations")]
    public async Task<ActionResult<GenerationResult>> BackendGenerateWithRelations(
        [FromBody] BackendGenerationRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _backendGenerator.GenerateAsync(request.TableName, request.SolutionName, request.Relations, cancellationToken);
        return Ok(result);
    }

    [HttpPost("frontend/preview")]
    public async Task<ActionResult<GenerationResult>> FrontendPreview(
        [FromQuery] string tableName = "dbo.tblEmployee",
        [FromQuery] string frontendAppName = "GeneratedCrud.React",
        CancellationToken cancellationToken = default)
    {
        var result = await _frontendGenerator.PreviewAsync(tableName, frontendAppName, cancellationToken: cancellationToken);
        return Ok(result);
    }

    [HttpPost("frontend/generate")]
    public async Task<ActionResult<GenerationResult>> FrontendGenerate(
        [FromQuery] string tableName = "dbo.tblEmployee",
        [FromQuery] string frontendAppName = "GeneratedCrud.React",
        CancellationToken cancellationToken = default)
    {
        var result = await _frontendGenerator.GenerateAsync(tableName, frontendAppName, cancellationToken: cancellationToken);
        return Ok(result);
    }

    [HttpPost("frontend/preview-with-relations")]
    public async Task<ActionResult<GenerationResult>> FrontendPreviewWithRelations(
        [FromBody] FrontendGenerationRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _frontendGenerator.PreviewAsync(request.TableName, request.FrontendAppName, request.Relations, cancellationToken);
        return Ok(result);
    }

    [HttpPost("frontend/generate-with-relations")]
    public async Task<ActionResult<GenerationResult>> FrontendGenerateWithRelations(
        [FromBody] FrontendGenerationRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _frontendGenerator.GenerateAsync(request.TableName, request.FrontendAppName, request.Relations, cancellationToken);
        return Ok(result);
    }

    [HttpPost("fullstack/preview")]
    public async Task<ActionResult<GenerationResult>> FullStackPreview(
        [FromQuery] string tableName = "dbo.tblEmployee",
        [FromQuery] string solutionName = "GeneratedCrud",
        [FromQuery] string frontendAppName = "GeneratedCrud.React",
        CancellationToken cancellationToken = default)
    {
        var result = await _fullStackGenerator.PreviewAsync(tableName, solutionName, frontendAppName, cancellationToken: cancellationToken);
        return Ok(result);
    }

    [HttpPost("fullstack/generate")]
    public async Task<ActionResult<GenerationResult>> FullStackGenerate(
        [FromQuery] string tableName = "dbo.tblEmployee",
        [FromQuery] string solutionName = "GeneratedCrud",
        [FromQuery] string frontendAppName = "GeneratedCrud.React",
        CancellationToken cancellationToken = default)
    {
        var result = await _fullStackGenerator.GenerateAsync(tableName, solutionName, frontendAppName, cancellationToken: cancellationToken);
        return Ok(result);
    }

    [HttpPost("fullstack/preview-with-relations")]
    public async Task<ActionResult<GenerationResult>> FullStackPreviewWithRelations(
        [FromBody] FullStackGenerationRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _fullStackGenerator.PreviewAsync(
            request.TableName,
            request.SolutionName,
            request.FrontendAppName,
            request.Relations,
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("fullstack/generate-with-relations")]
    public async Task<ActionResult<GenerationResult>> FullStackGenerateWithRelations(
        [FromBody] FullStackGenerationRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _fullStackGenerator.GenerateAsync(
            request.TableName,
            request.SolutionName,
            request.FrontendAppName,
            request.Relations,
            cancellationToken);

        return Ok(result);
    }
}
