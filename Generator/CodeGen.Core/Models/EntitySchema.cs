namespace CodeGen.Core.Models;

public sealed class EntitySchema
{
    public string EntityName { get; set; } = "Employee";
    public string? EntityPlural { get; set; }
    public string? DbSchemaName { get; set; }
    public string DbTableName { get; set; } = string.Empty;
    public List<EntityField> Fields { get; set; } = new();
}
