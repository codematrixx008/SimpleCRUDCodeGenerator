namespace CodeGen.Core.Abstractions;

public interface ITemplateRepository
{
    Task<string> GetTemplateAsync(string templateName, CancellationToken cancellationToken = default);
}
