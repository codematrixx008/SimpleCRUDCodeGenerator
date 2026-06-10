namespace CodeGen.Core.Models;

public sealed class EntityRelation
{
    public string RelationName { get; set; } = string.Empty;

    public string LocalColumn { get; set; } = string.Empty;
    public string LocalProperty { get; set; } = string.Empty;

    public string LookupSchemaName { get; set; } = "dbo";
    public string LookupTableName { get; set; } = string.Empty;
    public string LookupTableFullName { get; set; } = string.Empty;

    public string LookupEntityName { get; set; } = string.Empty;
    public string LookupEntityPlural { get; set; } = string.Empty;
    public string LookupFeatureFolder { get; set; } = string.Empty;
    public string LookupEntityPluralVariable { get; set; } = string.Empty;

    public string LookupKeyColumn { get; set; } = "Id";
    public string LookupKeyProperty { get; set; } = "Id";

    public string LookupDisplayColumn { get; set; } = string.Empty;
    public string LookupDisplayProperty { get; set; } = string.Empty;
    public string LookupDisplayVariable { get; set; } = string.Empty;

    public string Alias { get; set; } = string.Empty;
    public bool Required { get; set; }
}
