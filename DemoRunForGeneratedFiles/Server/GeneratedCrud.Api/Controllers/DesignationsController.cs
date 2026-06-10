using GeneratedCrud.Domain.DTOs;
using GeneratedCrud.Domain.Interfaces;
using GeneratedCrud.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace GeneratedCrud.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class DesignationsController : ControllerBase
{
    private readonly IDesignationRepository _repository;

    public DesignationsController(IDesignationRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DesignationDto>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return Ok(items.Select(ToDto).ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DesignationDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var designation = await _repository.GetByIdAsync(id, cancellationToken);
        if (designation is null)
        {
            return NotFound();
        }

        return Ok(ToDto(designation));
    }

    [HttpPost]
    public async Task<ActionResult<DesignationDto>> Create(CreateDesignationDto request, CancellationToken cancellationToken)
    {
        var designation = new Designation
        {
            DesignationName = request.DesignationName,
            DesignationCode = request.DesignationCode,
            Description = request.Description,
        };

        var created = await _repository.CreateAsync(designation, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateDesignationDto request, CancellationToken cancellationToken)
    {
        var designation = await _repository.GetByIdAsync(id, cancellationToken);
        if (designation is null)
        {
            return NotFound();
        }

        designation.DesignationName = request.DesignationName;
        designation.DesignationCode = request.DesignationCode;
        designation.Description = request.Description;

        var updated = await _repository.UpdateAsync(designation, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _repository.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    private static DesignationDto ToDto(Designation designation)
    {
        return new DesignationDto
        {
            Id = designation.Id,
            DesignationName = designation.DesignationName,
            DesignationCode = designation.DesignationCode,
            Description = designation.Description,
            CreatedDate = designation.CreatedDate,
            UpdatedDate = designation.UpdatedDate,
            IsDeleted = designation.IsDeleted,
        };
    }
}
