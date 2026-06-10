using Dapper;
using GeneratedCrud.Domain.Interfaces;
using GeneratedCrud.Domain.Models;
using GeneratedCrud.Infrastructure.Data;

namespace GeneratedCrud.Infrastructure.Repositories;

public sealed class DesignationRepository : IDesignationRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public DesignationRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Designation>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT
    e.[Id] AS [Id],
    e.[DesignationName] AS [DesignationName],
    e.[DesignationCode] AS [DesignationCode],
    e.[Description] AS [Description],
    e.[CreatedDate] AS [CreatedDate],
    e.[UpdatedDate] AS [UpdatedDate],
    e.[IsDeleted] AS [IsDeleted]
FROM [dbo].[tblDesignation] e
WHERE e.[IsDeleted] = CAST(0 AS bit)
ORDER BY e.[Id];
""";

        using var connection = _connectionFactory.CreateConnection();
        var items = await connection.QueryAsync<Designation>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        return items.AsList();
    }

    public async Task<Designation?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT
    e.[Id] AS [Id],
    e.[DesignationName] AS [DesignationName],
    e.[DesignationCode] AS [DesignationCode],
    e.[Description] AS [Description],
    e.[CreatedDate] AS [CreatedDate],
    e.[UpdatedDate] AS [UpdatedDate],
    e.[IsDeleted] AS [IsDeleted]
FROM [dbo].[tblDesignation] e
WHERE e.[Id] = @Id AND e.[IsDeleted] = CAST(0 AS bit);
""";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Designation>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<Designation> CreateAsync(Designation designation, CancellationToken cancellationToken = default)
    {
        const string sql = """
INSERT INTO [dbo].[tblDesignation]
(
    [DesignationName],
    [DesignationCode],
    [Description]
)
OUTPUT
    INSERTED.[Id] AS [Id],
    INSERTED.[DesignationName] AS [DesignationName],
    INSERTED.[DesignationCode] AS [DesignationCode],
    INSERTED.[Description] AS [Description],
    INSERTED.[CreatedDate] AS [CreatedDate],
    INSERTED.[UpdatedDate] AS [UpdatedDate],
    INSERTED.[IsDeleted] AS [IsDeleted]
VALUES
(
    @DesignationName,
    @DesignationCode,
    @Description
);
""";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<Designation>(
            new CommandDefinition(sql, designation, cancellationToken: cancellationToken));
    }

    public async Task<bool> UpdateAsync(Designation designation, CancellationToken cancellationToken = default)
    {
        const string sql = """
UPDATE [dbo].[tblDesignation]
SET
    [DesignationName] = @DesignationName,
    [DesignationCode] = @DesignationCode,
    [Description] = @Description,
    [UpdatedDate] = SYSUTCDATETIME()
WHERE [Id] = @Id AND [IsDeleted] = CAST(0 AS bit);
""";

        using var connection = _connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteAsync(
            new CommandDefinition(sql, designation, cancellationToken: cancellationToken));

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
UPDATE [dbo].[tblDesignation]
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
