using GeneratedCrud.Domain.DTOs;
using GeneratedCrud.Domain.Interfaces;
using GeneratedCrud.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace GeneratedCrud.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class DepartmentsController : ControllerBase
{
    private readonly IDepartmentRepository _repository;

    public DepartmentsController(IDepartmentRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DepartmentDto>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return Ok(items.Select(ToDto).ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DepartmentDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var department = await _repository.GetByIdAsync(id, cancellationToken);
        if (department is null)
        {
            return NotFound();
        }

        return Ok(ToDto(department));
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentDto>> Create(CreateDepartmentDto request, CancellationToken cancellationToken)
    {
        var department = new Department
        {
            DepartmentName = request.DepartmentName,
            DepartmentCode = request.DepartmentCode,
            Description = request.Description,
        };

        var created = await _repository.CreateAsync(department, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateDepartmentDto request, CancellationToken cancellationToken)
    {
        var department = await _repository.GetByIdAsync(id, cancellationToken);
        if (department is null)
        {
            return NotFound();
        }

        department.DepartmentName = request.DepartmentName;
        department.DepartmentCode = request.DepartmentCode;
        department.Description = request.Description;

        var updated = await _repository.UpdateAsync(department, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _repository.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    private static DepartmentDto ToDto(Department department)
    {
        return new DepartmentDto
        {
            Id = department.Id,
            DepartmentName = department.DepartmentName,
            DepartmentCode = department.DepartmentCode,
            Description = department.Description,
            CreatedDate = department.CreatedDate,
            UpdatedDate = department.UpdatedDate,
            IsDeleted = department.IsDeleted,
        };
    }
}
