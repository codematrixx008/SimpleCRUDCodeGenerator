using Dapper;
using {{SolutionName}}.Domain.Interfaces;
using {{SolutionName}}.Domain.Models;
using {{SolutionName}}.Infrastructure.Data;

namespace {{SolutionName}}.Infrastructure.Repositories;

public sealed class {{EntityName}}Repository : I{{EntityName}}Repository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public {{EntityName}}Repository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<{{EntityName}}>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT
{{SelectColumnList}}
FROM {{FromAndJoinSql}}
{{SelectWhereNotDeleted}}
ORDER BY e.[{{KeyColumnName}}];
""";

        using var connection = _connectionFactory.CreateConnection();
        var items = await connection.QueryAsync<{{EntityName}}>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        return items.AsList();
    }

    public async Task<{{EntityName}}?> GetByIdAsync({{KeyType}} id, CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT
{{SelectColumnList}}
FROM {{FromAndJoinSql}}
WHERE e.[{{KeyColumnName}}] = @Id{{SelectAndNotDeleted}};
""";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<{{EntityName}}>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<{{EntityName}}> CreateAsync({{EntityName}} {{EntityVariable}}, CancellationToken cancellationToken = default)
    {
        const string sql = """
INSERT INTO {{TableFullName}}
(
{{InsertColumnList}}
)
OUTPUT
{{OutputInsertedColumnList}}
VALUES
(
{{InsertValueList}}
);
""";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<{{EntityName}}>(
            new CommandDefinition(sql, {{EntityVariable}}, cancellationToken: cancellationToken));
    }

    public async Task<bool> UpdateAsync({{EntityName}} {{EntityVariable}}, CancellationToken cancellationToken = default)
    {
        const string sql = """
UPDATE {{TableFullName}}
SET
{{UpdateSetList}}
WHERE [{{KeyColumnName}}] = @{{KeyName}}{{MutationAndNotDeleted}};
""";

        using var connection = _connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteAsync(
            new CommandDefinition(sql, {{EntityVariable}}, cancellationToken: cancellationToken));

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync({{KeyType}} id, CancellationToken cancellationToken = default)
    {
        const string sql = """
{{DeleteSql}}
""";

        using var connection = _connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        return affectedRows > 0;
    }
}
