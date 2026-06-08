namespace CodeGen.Core.Models;

public sealed class GenerationResult
{
    public string SolutionName { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public bool FilesWritten { get; set; }
    public List<GeneratedFileResult> Files { get; set; } = new();
}

public sealed class GeneratedFileResult
{
    public string RelativePath { get; set; } = string.Empty;
    public string? Content { get; set; }
}
