using CodeGen.Core.Abstractions;
using CodeGen.Core.Models;
using CodeGen.Core.Settings;

namespace CodeGen.Infrastructure;

public sealed class FileSystemOutputWriter : IOutputWriter
{
    private readonly CodeGenSettings _settings;

    public FileSystemOutputWriter(CodeGenSettings settings)
    {
        _settings = settings;
    }

    public async Task WriteAsync(IEnumerable<GeneratedFile> files, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_settings.OutputPath);

        foreach (var file in files)
        {
            var safeRelativePath = file.RelativePath.Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.GetFullPath(Path.Combine(_settings.OutputPath, safeRelativePath));

            if (!fullPath.StartsWith(_settings.OutputPath, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Blocked unsafe generated path: {file.RelativePath}");
            }

            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(fullPath, file.Content, cancellationToken);
        }
    }
}
