namespace CodeGen.Core.Services;

public sealed class SimpleTemplateEngine
{
    public string Render(string template, IReadOnlyDictionary<string, string> tokens)
    {
        var output = template;
        foreach (var token in tokens)
        {
            output = output.Replace("{{" + token.Key + "}}", token.Value, StringComparison.Ordinal);
        }

        return output;
    }
}
