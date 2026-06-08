namespace CodeGen.Core.Schema;

public sealed class SchemaResult
{
    public List<DbColumnSchema> TableColumns { get; set; } = new();
    public List<DbColumnSchema> SearchResultColumns { get; set; } = new();
}
