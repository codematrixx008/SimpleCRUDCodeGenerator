namespace CodeGen.Core.Models;

public sealed class RelationDefinition
{
    public string LocalColumn { get; set; } = string.Empty;
    public string LookupTableName { get; set; } = string.Empty;
    public string LookupKeyColumn { get; set; } = "Id";
    public string LookupDisplayColumn { get; set; } = string.Empty;
    public string? RelationName { get; set; }
    public bool Required { get; set; }
}
