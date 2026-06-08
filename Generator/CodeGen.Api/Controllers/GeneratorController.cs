using CodeGen.Core.Models;
using CodeGen.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeGen.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class GeneratorController : ControllerBase
{
    private readonly CodeGeneratorService _generator;

    public GeneratorController(CodeGeneratorService generator)
    {
        _generator = generator;
    }

    [HttpPost("preview")]
    public async Task<ActionResult<GenerationResult>> Preview(
        [FromQuery] string tableName = "dbo.tblEmployee",
        [FromQuery] string solutionName = "SimpleEmployeeCRUD",
        CancellationToken cancellationToken = default)
    {
        var result = await _generator.PreviewAsync(tableName, solutionName, cancellationToken);
        return Ok(result);
    }

    [HttpPost("generate")]
    public async Task<ActionResult<GenerationResult>> Generate(
        [FromQuery] string tableName = "dbo.tblEmployee",
        [FromQuery] string solutionName = "SimpleEmployeeCRUD",
        CancellationToken cancellationToken = default)
    {
        var result = await _generator.GenerateAsync(tableName, solutionName, cancellationToken);
        return Ok(result);
    }
}
