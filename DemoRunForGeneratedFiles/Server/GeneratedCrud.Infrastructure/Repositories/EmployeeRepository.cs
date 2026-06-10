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
    e.[Id] AS [Id],
    e.[FirstName] AS [FirstName],
    e.[LastName] AS [LastName],
    e.[DOB] AS [DOB],
    e.[Gender] AS [Gender],
    e.[Address] AS [Address],
    e.[CreatedDate] AS [CreatedDate],
    e.[UpdatedDate] AS [UpdatedDate],
    e.[IsDeleted] AS [IsDeleted],
    e.[DepartmentId] AS [DepartmentId],
    l1.[DepartmentName] AS [DepartmentName]
FROM [dbo].[tblEmployee] e
LEFT JOIN [dbo].[tblDepartment] l1 ON l1.[Id] = e.[DepartmentId]
   AND l1.[IsDeleted] = CAST(0 AS bit)
WHERE e.[IsDeleted] = CAST(0 AS bit)
ORDER BY e.[Id];
""";

        using var connection = _connectionFactory.CreateConnection();
        var items = await connection.QueryAsync<Employee>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        return items.AsList();
    }

    public async Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT
    e.[Id] AS [Id],
    e.[FirstName] AS [FirstName],
    e.[LastName] AS [LastName],
    e.[DOB] AS [DOB],
    e.[Gender] AS [Gender],
    e.[Address] AS [Address],
    e.[CreatedDate] AS [CreatedDate],
    e.[UpdatedDate] AS [UpdatedDate],
    e.[IsDeleted] AS [IsDeleted],
    e.[DepartmentId] AS [DepartmentId],
    l1.[DepartmentName] AS [DepartmentName]
FROM [dbo].[tblEmployee] e
LEFT JOIN [dbo].[tblDepartment] l1 ON l1.[Id] = e.[DepartmentId]
   AND l1.[IsDeleted] = CAST(0 AS bit)
WHERE e.[Id] = @Id AND e.[IsDeleted] = CAST(0 AS bit);
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
    [Address],
    [DepartmentId]
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
    INSERTED.[IsDeleted] AS [IsDeleted],
    INSERTED.[DepartmentId] AS [DepartmentId]
VALUES
(
    @FirstName,
    @LastName,
    @DOB,
    @Gender,
    @Address,
    @DepartmentId
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
    [DepartmentId] = @DepartmentId,
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
