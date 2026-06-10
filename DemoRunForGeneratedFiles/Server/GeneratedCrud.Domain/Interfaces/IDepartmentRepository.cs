using GeneratedCrud.Domain.Models;

namespace GeneratedCrud.Domain.Interfaces;

public interface IDepartmentRepository
{
    Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Department> CreateAsync(Department department, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Department department, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
