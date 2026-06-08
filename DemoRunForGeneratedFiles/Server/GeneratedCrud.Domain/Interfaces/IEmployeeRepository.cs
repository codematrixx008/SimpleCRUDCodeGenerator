using GeneratedCrud.Domain.Models;

namespace GeneratedCrud.Domain.Interfaces;

public interface IEmployeeRepository
{
    Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Employee> CreateAsync(Employee employee, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Employee employee, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
