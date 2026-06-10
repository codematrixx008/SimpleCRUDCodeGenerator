using Dapper;
using GeneratedCrud.Domain.Interfaces;
using GeneratedCrud.Domain.Models;
using GeneratedCrud.Infrastructure.Data;

namespace GeneratedCrud.Infrastructure.Repositories;

public sealed class DepartmentRepository : IDepartmentRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public DepartmentRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT
    [Id] AS [Id],
    [DepartmentName] AS [DepartmentName],
    [DepartmentCode] AS [DepartmentCode],
    [Description] AS [Description],
    [CreatedDate] AS [CreatedDate],
    [UpdatedDate] AS [UpdatedDate],
    [IsDeleted] AS [IsDeleted]
FROM [tblDepartment]
WHERE [IsDeleted] = CAST(0 AS bit)
ORDER BY [Id];
""";

        using var connection = _connectionFactory.CreateConnection();
        var items = await connection.QueryAsync<Department>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        return items.AsList();
    }

    public async Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT
    [Id] AS [Id],
    [DepartmentName] AS [DepartmentName],
    [DepartmentCode] AS [DepartmentCode],
    [Description] AS [Description],
    [CreatedDate] AS [CreatedDate],
    [UpdatedDate] AS [UpdatedDate],
    [IsDeleted] AS [IsDeleted]
FROM [tblDepartment]
WHERE [Id] = @Id AND [IsDeleted] = CAST(0 AS bit);
""";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Department>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<Department> CreateAsync(Department department, CancellationToken cancellationToken = default)
    {
        const string sql = """
INSERT INTO [tblDepartment]
(
    [DepartmentName],
    [DepartmentCode],
    [Description]
)
OUTPUT
    INSERTED.[Id] AS [Id],
    INSERTED.[DepartmentName] AS [DepartmentName],
    INSERTED.[DepartmentCode] AS [DepartmentCode],
    INSERTED.[Description] AS [Description],
    INSERTED.[CreatedDate] AS [CreatedDate],
    INSERTED.[UpdatedDate] AS [UpdatedDate],
    INSERTED.[IsDeleted] AS [IsDeleted]
VALUES
(
    @DepartmentName,
    @DepartmentCode,
    @Description
);
""";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<Department>(
            new CommandDefinition(sql, department, cancellationToken: cancellationToken));
    }

    public async Task<bool> UpdateAsync(Department department, CancellationToken cancellationToken = default)
    {
        const string sql = """
UPDATE [tblDepartment]
SET
    [DepartmentName] = @DepartmentName,
    [DepartmentCode] = @DepartmentCode,
    [Description] = @Description,
    [UpdatedDate] = SYSUTCDATETIME()
WHERE [Id] = @Id AND [IsDeleted] = CAST(0 AS bit);
""";

        using var connection = _connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteAsync(
            new CommandDefinition(sql, department, cancellationToken: cancellationToken));

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
UPDATE [tblDepartment]
SET
    [IsDeleted] = CAST(1 AS bit),
    [UpdatedDate] = SYSUTCDATETIME()
WHERE [Id] = @Id AND [IsDeleted] = CAST(0 AS bit);
""";

        using var connection = _connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        return affectedRows > 0;
    }
}
