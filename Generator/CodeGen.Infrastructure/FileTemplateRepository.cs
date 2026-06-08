using CodeGen.Core.Abstractions;
using CodeGen.Core.Settings;

namespace CodeGen.Infrastructure;

public sealed class FileTemplateRepository : ITemplateRepository
{
    private readonly CodeGenSettings _settings;

    public FileTemplateRepository(CodeGenSettings settings)
    {
        _settings = settings;
    }

    public async Task<string> GetTemplateAsync(string templateName, CancellationToken cancellationToken = default)
    {
        var path = Path.Combine(_settings.TemplatesPath, templateName);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Template not found: {path}", path);
        }

        return await File.ReadAllTextAsync(path, cancellationToken);
    }
}
