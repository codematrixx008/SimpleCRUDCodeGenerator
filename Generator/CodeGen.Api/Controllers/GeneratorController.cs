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

    // Backward-compatible backend endpoints.
    [HttpPost("preview")]
    public Task<ActionResult<GenerationResult>> Preview(
        [FromQuery] string tableName = "dbo.tblEmployee",
        [FromQuery] string solutionName = "SimpleEmployeeCRUD",
        CancellationToken cancellationToken = default)
    {
        return BackendPreview(tableName, solutionName, cancellationToken);
    }

    [HttpPost("generate")]
    public Task<ActionResult<GenerationResult>> Generate(
        [FromQuery] string tableName = "dbo.tblEmployee",
        [FromQuery] string solutionName = "SimpleEmployeeCRUD",
        CancellationToken cancellationToken = default)
    {
        return BackendGenerate(tableName, solutionName, cancellationToken);
    }

    [HttpPost("backend/preview")]
    public async Task<ActionResult<GenerationResult>> BackendPreview(
        [FromQuery] string tableName = "dbo.tblEmployee",
        [FromQuery] string solutionName = "SimpleEmployeeCRUD",
        CancellationToken cancellationToken = default)
    {
        var result = await _backendGenerator.PreviewAsync(tableName, solutionName, cancellationToken);
        return Ok(result);
    }

    [HttpPost("backend/generate")]
    public async Task<ActionResult<GenerationResult>> BackendGenerate(
        [FromQuery] string tableName = "dbo.tblEmployee",
        [FromQuery] string solutionName = "SimpleEmployeeCRUD",
        CancellationToken cancellationToken = default)
    {
        var result = await _backendGenerator.GenerateAsync(tableName, solutionName, cancellationToken);
        return Ok(result);
    }

    [HttpPost("frontend/preview")]
    public async Task<ActionResult<GenerationResult>> FrontendPreview(
        [FromQuery] string tableName = "dbo.tblEmployee",
        [FromQuery] string frontendAppName = "SimpleEmployeeCRUD.React",
        CancellationToken cancellationToken = default)
    {
        var result = await _frontendGenerator.PreviewAsync(tableName, frontendAppName, cancellationToken);
        return Ok(result);
    }

    [HttpPost("frontend/generate")]
    public async Task<ActionResult<GenerationResult>> FrontendGenerate(
        [FromQuery] string tableName = "dbo.tblEmployee",
        [FromQuery] string frontendAppName = "SimpleEmployeeCRUD.React",
        CancellationToken cancellationToken = default)
    {
        var result = await _frontendGenerator.GenerateAsync(tableName, frontendAppName, cancellationToken);
        return Ok(result);
    }

    [HttpPost("fullstack/preview")]
    public async Task<ActionResult<GenerationResult>> FullStackPreview(
        [FromQuery] string tableName = "dbo.tblEmployee",
        [FromQuery] string solutionName = "SimpleEmployeeCRUD",
        [FromQuery] string frontendAppName = "SimpleEmployeeCRUD.React",
        CancellationToken cancellationToken = default)
    {
        var result = await _fullStackGenerator.PreviewAsync(tableName, solutionName, frontendAppName, cancellationToken);
        return Ok(result);
    }

    [HttpPost("fullstack/generate")]
    public async Task<ActionResult<GenerationResult>> FullStackGenerate(
        [FromQuery] string tableName = "dbo.tblEmployee",
        [FromQuery] string solutionName = "SimpleEmployeeCRUD",
        [FromQuery] string frontendAppName = "SimpleEmployeeCRUD.React",
        CancellationToken cancellationToken = default)
    {
        var result = await _fullStackGenerator.GenerateAsync(tableName, solutionName, frontendAppName, cancellationToken);
        return Ok(result);
    }
}
