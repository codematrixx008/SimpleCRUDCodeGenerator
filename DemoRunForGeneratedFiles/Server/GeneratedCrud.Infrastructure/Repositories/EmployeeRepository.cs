using GeneratedCrud.Domain.Interfaces;
using GeneratedCrud.Domain.Models;
using GeneratedCrud.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GeneratedCrud.Infrastructure.Repositories;

public sealed class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }

    public async Task<Employee> CreateAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        await _context.Employees.AddAsync(employee, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return employee;
    }

    public async Task<bool> UpdateAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        var exists = await _context.Employees
            .AnyAsync(x => x.Id.Equals(employee.Id), cancellationToken);

        if (!exists)
        {
            return false;
        }

        _context.Employees.Update(employee);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);

        if (employee is null)
        {
            return false;
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
