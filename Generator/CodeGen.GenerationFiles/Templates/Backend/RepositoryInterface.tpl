using {{SolutionName}}.Domain.Models;

namespace {{SolutionName}}.Domain.Interfaces;

public interface I{{EntityName}}Repository
{
    Task<IReadOnlyList<{{EntityName}}>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<{{EntityName}}?> GetByIdAsync({{KeyType}} id, CancellationToken cancellationToken = default);
    Task<{{EntityName}}> CreateAsync({{EntityName}} {{EntityVariable}}, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync({{EntityName}} {{EntityVariable}}, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync({{KeyType}} id, CancellationToken cancellationToken = default);
}
