namespace CodeGen.Core.Settings;

public sealed class CodeGenSettings
{
    public string TemplatesPath { get; set; } = @"..\CodeGen.GenerationFiles\Templates";
    public string OutputPath { get; set; } = @"..\CodeGen.GenerationFiles\GeneratedOutput";
    public string SchemaStoredProcedure { get; set; } = "dbo.usp_GetObjectSchemas";

    public void ResolveRelativePaths(string contentRootPath)
    {
        TemplatesPath = ResolvePath(contentRootPath, TemplatesPath);
        OutputPath = ResolvePath(contentRootPath, OutputPath);
    }

    private static string ResolvePath(string basePath, string path)
    {
        var normalizedPath = path
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar);

        if (Path.IsPathRooted(normalizedPath))
        {
            return Path.GetFullPath(normalizedPath);
        }

        return Path.GetFullPath(Path.Combine(basePath, normalizedPath));
    }
}
