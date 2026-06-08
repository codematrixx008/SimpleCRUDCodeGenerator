namespace CodeGen.Core.Schema;

public interface ISchemaRepository
{
    Task<SchemaResult> GetSchemaAsync(string tableName);
}
