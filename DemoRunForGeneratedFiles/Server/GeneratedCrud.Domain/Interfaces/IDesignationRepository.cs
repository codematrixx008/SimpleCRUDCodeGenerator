using GeneratedCrud.Domain.Models;

namespace GeneratedCrud.Domain.Interfaces;

public interface IDesignationRepository
{
    Task<IReadOnlyList<Designation>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Designation?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Designation> CreateAsync(Designation designation, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Designation designation, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
