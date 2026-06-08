using Dapper;
using GeneratedCrud.Domain.Interfaces;
using GeneratedCrud.Domain.Models;
using GeneratedCrud.Infrastructure.Data;

namespace GeneratedCrud.Infrastructure.Repositories;

public sealed class EmployeeRepository : IEmployeeRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public EmployeeRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT
    [Id] AS [Id],
    [FirstName] AS [FirstName],
    [LastName] AS [LastName],
    [DOB] AS [DOB],
    [Gender] AS [Gender],
    [Address] AS [Address],
    [CreatedDate] AS [CreatedDate],
    [UpdatedDate] AS [UpdatedDate],
    [IsDeleted] AS [IsDeleted]
FROM [dbo].[tblEmployee]
WHERE [IsDeleted] = CAST(0 AS bit)
ORDER BY [Id];
""";

        using var connection = _connectionFactory.CreateConnection();
        var items = await connection.QueryAsync<Employee>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        return items.AsList();
    }

    public async Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT
    [Id] AS [Id],
    [FirstName] AS [FirstName],
    [LastName] AS [LastName],
    [DOB] AS [DOB],
    [Gender] AS [Gender],
    [Address] AS [Address],
    [CreatedDate] AS [CreatedDate],
    [UpdatedDate] AS [UpdatedDate],
    [IsDeleted] AS [IsDeleted]
FROM [dbo].[tblEmployee]
WHERE [Id] = @Id AND [IsDeleted] = CAST(0 AS bit);
""";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Employee>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<Employee> CreateAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        const string sql = """
INSERT INTO [dbo].[tblEmployee]
(
    [FirstName],
    [LastName],
    [DOB],
    [Gender],
    [Address]
)
OUTPUT
    INSERTED.[Id] AS [Id],
    INSERTED.[FirstName] AS [FirstName],
    INSERTED.[LastName] AS [LastName],
    INSERTED.[DOB] AS [DOB],
    INSERTED.[Gender] AS [Gender],
    INSERTED.[Address] AS [Address],
    INSERTED.[CreatedDate] AS [CreatedDate],
    INSERTED.[UpdatedDate] AS [UpdatedDate],
    INSERTED.[IsDeleted] AS [IsDeleted]
VALUES
(
    @FirstName,
    @LastName,
    @DOB,
    @Gender,
    @Address
);
""";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<Employee>(
            new CommandDefinition(sql, employee, cancellationToken: cancellationToken));
    }

    public async Task<bool> UpdateAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        const string sql = """
UPDATE [dbo].[tblEmployee]
SET
    [FirstName] = @FirstName,
    [LastName] = @LastName,
    [DOB] = @DOB,
    [Gender] = @Gender,
    [Address] = @Address,
    [UpdatedDate] = SYSUTCDATETIME()
WHERE [Id] = @Id AND [IsDeleted] = CAST(0 AS bit);
""";

        using var connection = _connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteAsync(
            new CommandDefinition(sql, employee, cancellationToken: cancellationToken));

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
UPDATE [dbo].[tblEmployee]
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
