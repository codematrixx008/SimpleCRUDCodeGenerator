using CodeGen.Core.Schema;
using CodeGen.Core.Settings;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CodeGen.Infrastructure.Schema;

public sealed class SqlSchemaRepository : ISchemaRepository
{
    private readonly string _connectionString;
    private readonly CodeGenSettings _settings;

    public SqlSchemaRepository(string connectionString, CodeGenSettings settings)
    {
        _connectionString = connectionString;
        _settings = settings;
    }

    public async Task<SchemaResult> GetSchemaAsync(string tableName)
    {
        if (string.IsNullOrWhiteSpace(_settings.SchemaStoredProcedure))
        {
            throw new InvalidOperationException("CodeGen:SchemaStoredProcedure is not configured.");
        }

        await using var conn = new SqlConnection(_connectionString);

        using var multi = await conn.QueryMultipleAsync(
            _settings.SchemaStoredProcedure,
            new { TableName = tableName },
            commandType: CommandType.StoredProcedure);

        var tableCols = (await multi.ReadAsync<DbColumnSchema>()).ToList();
        var searchCols = (await multi.ReadAsync<DbColumnSchema>()).ToList();

        if (searchCols.Count == 0)
        {
            searchCols = tableCols.ToList();
        }

        return new SchemaResult
        {
            TableColumns = tableCols,
            SearchResultColumns = searchCols
        };
    }
}
