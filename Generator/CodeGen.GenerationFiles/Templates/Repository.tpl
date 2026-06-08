using {{SolutionName}}.Domain.Interfaces;
using {{SolutionName}}.Domain.Models;
using {{SolutionName}}.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace {{SolutionName}}.Infrastructure.Repositories;

public sealed class {{EntityName}}Repository : I{{EntityName}}Repository
{
    private readonly AppDbContext _context;

    public {{EntityName}}Repository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<{{EntityName}}>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.{{EntityPlural}}
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<{{EntityName}}?> GetByIdAsync({{KeyType}} id, CancellationToken cancellationToken = default)
    {
        return await _context.{{EntityPlural}}
            .FirstOrDefaultAsync(x => x.{{KeyName}}.Equals(id), cancellationToken);
    }

    public async Task<{{EntityName}}> CreateAsync({{EntityName}} {{EntityVariable}}, CancellationToken cancellationToken = default)
    {
        await _context.{{EntityPlural}}.AddAsync({{EntityVariable}}, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return {{EntityVariable}};
    }

    public async Task<bool> UpdateAsync({{EntityName}} {{EntityVariable}}, CancellationToken cancellationToken = default)
    {
        var exists = await _context.{{EntityPlural}}
            .AnyAsync(x => x.{{KeyName}}.Equals({{EntityVariable}}.{{KeyName}}), cancellationToken);

        if (!exists)
        {
            return false;
        }

        _context.{{EntityPlural}}.Update({{EntityVariable}});
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync({{KeyType}} id, CancellationToken cancellationToken = default)
    {
        var {{EntityVariable}} = await _context.{{EntityPlural}}
            .FirstOrDefaultAsync(x => x.{{KeyName}}.Equals(id), cancellationToken);

        if ({{EntityVariable}} is null)
        {
            return false;
        }

        _context.{{EntityPlural}}.Remove({{EntityVariable}});
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
