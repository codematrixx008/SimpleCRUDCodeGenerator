using GeneratedCrud.Domain.DTOs;
using GeneratedCrud.Domain.Interfaces;
using GeneratedCrud.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace GeneratedCrud.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _repository;

    public EmployeesController(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EmployeeDto>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return Ok(items.Select(ToDto).ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var employee = await _repository.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        return Ok(ToDto(employee));
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> Create(CreateEmployeeDto request, CancellationToken cancellationToken)
    {
        var employee = new Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            DOB = request.DOB,
            Gender = request.Gender,
            Address = request.Address,
            DepartmentId = request.DepartmentId,
        };

        var created = await _repository.CreateAsync(employee, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateEmployeeDto request, CancellationToken cancellationToken)
    {
        var employee = await _repository.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        employee.FirstName = request.FirstName;
        employee.LastName = request.LastName;
        employee.DOB = request.DOB;
        employee.Gender = request.Gender;
        employee.Address = request.Address;
        employee.DepartmentId = request.DepartmentId;

        var updated = await _repository.UpdateAsync(employee, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _repository.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    private static EmployeeDto ToDto(Employee employee)
    {
        return new EmployeeDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            DOB = employee.DOB,
            Gender = employee.Gender,
            Address = employee.Address,
            CreatedDate = employee.CreatedDate,
            UpdatedDate = employee.UpdatedDate,
            IsDeleted = employee.IsDeleted,
            DepartmentId = employee.DepartmentId,
            DepartmentName = employee.DepartmentName,
        };
    }
}
