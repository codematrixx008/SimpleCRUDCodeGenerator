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
