using {{SolutionName}}.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace {{SolutionName}}.Infrastructure.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    {{DbSetProperty}}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

{{EfEntityConfiguration}}
    }
}
