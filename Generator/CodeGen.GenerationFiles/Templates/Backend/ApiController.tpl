using {{SolutionName}}.Domain.DTOs;
using {{SolutionName}}.Domain.Interfaces;
using {{SolutionName}}.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace {{SolutionName}}.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class {{EntityPlural}}Controller : ControllerBase
{
    private readonly I{{EntityName}}Repository _repository;

    public {{EntityPlural}}Controller(I{{EntityName}}Repository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<{{EntityName}}Dto>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return Ok(items.Select(ToDto).ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<{{EntityName}}Dto>> GetById({{KeyType}} id, CancellationToken cancellationToken)
    {
        var {{EntityVariable}} = await _repository.GetByIdAsync(id, cancellationToken);
        if ({{EntityVariable}} is null)
        {
            return NotFound();
        }

        return Ok(ToDto({{EntityVariable}}));
    }

    [HttpPost]
    public async Task<ActionResult<{{EntityName}}Dto>> Create(Create{{EntityName}}Dto request, CancellationToken cancellationToken)
    {
        var {{EntityVariable}} = new {{EntityName}}
        {
{{CreateEntityAssignments}}
        };

        var created = await _repository.CreateAsync({{EntityVariable}}, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.{{KeyName}} }, ToDto(created));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update({{KeyType}} id, Update{{EntityName}}Dto request, CancellationToken cancellationToken)
    {
        var {{EntityVariable}} = await _repository.GetByIdAsync(id, cancellationToken);
        if ({{EntityVariable}} is null)
        {
            return NotFound();
        }

{{UpdateEntityAssignments}}

        var updated = await _repository.UpdateAsync({{EntityVariable}}, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete({{KeyType}} id, CancellationToken cancellationToken)
    {
        var deleted = await _repository.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    private static {{EntityName}}Dto ToDto({{EntityName}} {{EntityVariable}})
    {
        return new {{EntityName}}Dto
        {
{{DtoAssignments}}
        };
    }
}
