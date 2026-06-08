using CodeGen.Core.Models;

namespace CodeGen.Core.Abstractions;

public interface IOutputWriter
{
    Task WriteAsync(IEnumerable<GeneratedFile> files, CancellationToken cancellationToken = default);
}
