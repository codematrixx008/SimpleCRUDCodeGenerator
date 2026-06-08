namespace CodeGen.Core.Models;

public sealed class EntityField
{
    public string Name { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;
    public string Type { get; set; } = "string";
    public bool IsKey { get; set; }
    public bool IsIdentity { get; set; }
    public bool IsNullable { get; set; }
    public bool IncludeInCreate { get; set; } = true;
    public bool IncludeInUpdate { get; set; } = true;
    public int? MaxLength { get; set; }
    public int? Precision { get; set; }
    public int? Scale { get; set; }
}
