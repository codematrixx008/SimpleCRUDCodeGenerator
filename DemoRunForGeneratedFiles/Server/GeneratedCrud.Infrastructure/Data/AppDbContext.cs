using GeneratedCrud.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GeneratedCrud.Infrastructure.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("tblEmployee", "dbo");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("Id").HasMaxLength(4);
            entity.Property(e => e.FirstName).HasColumnName("FirstName").HasMaxLength(50).IsRequired();
            entity.Property(e => e.LastName).HasColumnName("LastName").HasMaxLength(50).IsRequired();
            entity.Property(e => e.DOB).HasColumnName("DOB").HasMaxLength(3);
            entity.Property(e => e.Gender).HasColumnName("Gender").HasMaxLength(10).IsRequired();
            entity.Property(e => e.Address).HasColumnName("Address").HasMaxLength(250);
            entity.Property(e => e.CreatedDate).HasColumnName("CreatedDate").HasMaxLength(8);
            entity.Property(e => e.UpdatedDate).HasColumnName("UpdatedDate").HasMaxLength(8);
            entity.Property(e => e.IsDeleted).HasColumnName("IsDeleted").HasMaxLength(1);
        });
    }
}
