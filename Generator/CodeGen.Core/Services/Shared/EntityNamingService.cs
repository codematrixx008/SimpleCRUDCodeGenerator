using System.Text.RegularExpressions;

namespace CodeGen.Core.Services.Shared;

public sealed class EntityNamingService
{
    private static readonly Regex ValidNameRegex = new("^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.Compiled);
    private static readonly Regex ValidTableNameRegex = new("^([A-Za-z_][A-Za-z0-9_]*\\.)?[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.Compiled);
    private static readonly Regex ValidSolutionNameRegex = new("^[A-Za-z_][A-Za-z0-9_.]*$", RegexOptions.Compiled);

    public void ValidateTableName(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new ArgumentException("Table name is required.", nameof(tableName));
        }

        if (!ValidTableNameRegex.IsMatch(tableName.Trim()))
        {
            throw new ArgumentException("Table name must be in TableName or schema.TableName format.", nameof(tableName));
        }
    }

    public void ValidateSolutionName(string solutionName)
    {
        if (string.IsNullOrWhiteSpace(solutionName))
        {
            throw new ArgumentException("Solution/frontend app name is required.", nameof(solutionName));
        }

        if (!ValidSolutionNameRegex.IsMatch(solutionName.Trim()))
        {
            throw new ArgumentException("Solution/frontend app name can contain only letters, numbers, underscore, and dot, and cannot start with a number.", nameof(solutionName));
        }
    }

    public (string? SchemaName, string TableName) SplitSchemaAndTableName(string tableName)
    {
        ValidateTableName(tableName);

        var parts = tableName.Trim().Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 1)
        {
            return (null, parts[0]);
        }

        return (parts[0], parts[1]);
    }

    public string ToEntityNameFromTableName(string tableName)
    {
        var (_, tableOnlyName) = SplitSchemaAndTableName(tableName);
        return NormalizeEntityName(tableOnlyName);
    }

    public string NormalizeEntityName(string rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName))
        {
            throw new ArgumentException("Name is required.", nameof(rawName));
        }

        var name = rawName.Trim();
        if (name.StartsWith("tbl", StringComparison.OrdinalIgnoreCase) && name.Length > 3)
        {
            name = name[3..];
        }

        name = ToPascalCase(name);

        if (!ValidNameRegex.IsMatch(name))
        {
            throw new ArgumentException("Name can contain only letters, numbers, and underscore, and cannot start with a number.", nameof(rawName));
        }

        return name;
    }

    public string ToPlural(string entityName)
    {
        if (entityName.EndsWith("y", StringComparison.OrdinalIgnoreCase) && entityName.Length > 1)
        {
            return entityName[..^1] + "ies";
        }

        if (entityName.EndsWith("s", StringComparison.OrdinalIgnoreCase))
        {
            return entityName + "es";
        }

        return entityName + "s";
    }

    public string ToCamelCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        return char.ToLowerInvariant(value[0]) + value[1..];
    }

    public string ToKebabCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var normalized = NormalizeEntityName(value);
        return Regex.Replace(normalized, "(?<!^)([A-Z])", "-$1").ToLowerInvariant();
    }

    public string ToTitleCaseWords(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var normalized = NormalizeEntityName(value);
        return Regex.Replace(normalized, "(?<!^)([A-Z])", " $1");
    }

    private static string ToPascalCase(string value)
    {
        var parts = Regex.Split(value, "[^A-Za-z0-9]+").Where(x => !string.IsNullOrWhiteSpace(x));
        return string.Concat(parts.Select(part => char.ToUpperInvariant(part[0]) + part[1..]));
    }
}
