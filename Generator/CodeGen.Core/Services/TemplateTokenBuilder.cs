using System.Globalization;
using System.Text;
using CodeGen.Core.Models;

namespace CodeGen.Core.Services;

public sealed class TemplateTokenBuilder
{
    private readonly EntityNamingService _naming;

    public TemplateTokenBuilder(EntityNamingService naming)
    {
        _naming = naming;
    }

    public IReadOnlyDictionary<string, string> Build(EntitySchema schema, string solutionName)
    {
        var entityName = _naming.NormalizeEntityName(schema.EntityName);
        var entityPlural = string.IsNullOrWhiteSpace(schema.EntityPlural)
            ? _naming.ToPlural(entityName)
            : _naming.NormalizeEntityName(schema.EntityPlural!);

        var entityVariable = _naming.ToCamelCase(entityName);
        var key = schema.Fields.FirstOrDefault(x => x.IsKey)
            ?? schema.Fields.FirstOrDefault(x => x.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"No primary key/Id column found for {entityName}.");

        var createFields = schema.Fields.Where(x => !x.IsKey && !x.IsIdentity && x.IncludeInCreate).ToList();
        var updateFields = schema.Fields.Where(x => !x.IsKey && !x.IsIdentity && x.IncludeInUpdate).ToList();

        return new Dictionary<string, string>
        {
            ["SolutionName"] = solutionName,
            ["EntityName"] = entityName,
            ["EntityPlural"] = entityPlural,
            ["EntityVariable"] = entityVariable,
            ["EntityPluralVariable"] = _naming.ToCamelCase(entityPlural),
            ["KeyType"] = ToCSharpType(key),
            ["KeyName"] = key.Name,
            ["KeyVariable"] = _naming.ToCamelCase(key.Name),
            ["ModelProperties"] = BuildProperties(schema.Fields),
            ["DtoProperties"] = BuildProperties(schema.Fields),
            ["CreateDtoProperties"] = BuildProperties(createFields),
            ["UpdateDtoProperties"] = BuildProperties(updateFields),
            ["CreateEntityAssignments"] = BuildCreateAssignments(createFields),
            ["UpdateEntityAssignments"] = BuildUpdateAssignments(updateFields, entityVariable),
            ["DtoAssignments"] = BuildDtoAssignments(schema.Fields, entityVariable),
            ["EfEntityConfiguration"] = BuildEfEntityConfiguration(schema, entityName),
            ["DbSetProperty"] = $"public DbSet<{entityName}> {entityPlural} => Set<{entityName}>();"
        };
    }

    private static string BuildProperties(IEnumerable<EntityField> fields)
    {
        var builder = new StringBuilder();
        foreach (var field in fields)
        {
            builder.Append("    public ");
            builder.Append(ToCSharpType(field));
            builder.Append(' ');
            builder.Append(field.Name);
            builder.Append(" { get; set; }");

            if (RequiresDefaultValue(field))
            {
                builder.Append(" = string.Empty;");
            }

            builder.AppendLine();
        }

        return builder.ToString().TrimEnd();
    }

    private static string BuildCreateAssignments(IEnumerable<EntityField> fields)
    {
        return string.Join(Environment.NewLine, fields.Select(field => $"            {field.Name} = request.{field.Name},"));
    }

    private static string BuildUpdateAssignments(IEnumerable<EntityField> fields, string entityVariable)
    {
        return string.Join(Environment.NewLine, fields.Select(field => $"        {entityVariable}.{field.Name} = request.{field.Name};"));
    }

    private static string BuildDtoAssignments(IEnumerable<EntityField> fields, string entityVariable)
    {
        return string.Join(Environment.NewLine, fields.Select(field => $"            {field.Name} = {entityVariable}.{field.Name},"));
    }

    private static string BuildEfEntityConfiguration(EntitySchema schema, string entityName)
    {
        var list = schema.Fields.ToList();
        var key = list.FirstOrDefault(x => x.IsKey)
            ?? list.FirstOrDefault(x => x.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"No primary key/Id column found for {entityName}.");

        var builder = new StringBuilder();

        builder.AppendLine($"        modelBuilder.Entity<{entityName}>(entity =>");
        builder.AppendLine("        {");

        if (!string.IsNullOrWhiteSpace(schema.DbTableName))
        {
            if (!string.IsNullOrWhiteSpace(schema.DbSchemaName))
            {
                builder.AppendLine($"            entity.ToTable(\"{Escape(schema.DbTableName)}\", \"{Escape(schema.DbSchemaName!)}\");");
            }
            else
            {
                builder.AppendLine($"            entity.ToTable(\"{Escape(schema.DbTableName)}\");");
            }
        }

        builder.AppendLine($"            entity.HasKey(e => e.{key.Name});");

        foreach (var field in list)
        {
            var configParts = new List<string>();

            if (!string.IsNullOrWhiteSpace(field.ColumnName))
            {
                configParts.Add($".HasColumnName(\"{Escape(field.ColumnName)}\")");
            }

            if (field.MaxLength is > 0)
            {
                configParts.Add($".HasMaxLength({field.MaxLength.Value.ToString(CultureInfo.InvariantCulture)})");
            }

            if (IsDecimal(field) && field.Precision is > 0)
            {
                var scale = field.Scale ?? 0;
                configParts.Add($".HasPrecision({field.Precision.Value.ToString(CultureInfo.InvariantCulture)}, {scale.ToString(CultureInfo.InvariantCulture)})");
            }

            if (!field.IsNullable && IsString(field))
            {
                configParts.Add(".IsRequired()");
            }

            if (configParts.Count > 0)
            {
                builder.AppendLine($"            entity.Property(e => e.{field.Name}){string.Concat(configParts)};");
            }
        }

        builder.AppendLine("        });");
        return builder.ToString().TrimEnd();
    }

    private static bool RequiresDefaultValue(EntityField field)
    {
        return IsString(field) && !field.IsNullable;
    }

    private static bool IsString(EntityField field)
    {
        var type = field.Type.Trim().ToLowerInvariant();
        return type is "string" or "nvarchar" or "varchar" or "char" or "nchar" or "text" or "ntext";
    }

    private static bool IsDecimal(EntityField field)
    {
        var type = field.Type.Trim().ToLowerInvariant();
        return type is "decimal" or "numeric" or "money" or "smallmoney";
    }

    private static string ToCSharpType(EntityField field)
    {
        var type = field.Type.Trim();
        var csharpType = type.ToLowerInvariant() switch
        {
            "bigint" or "long" or "int64" => "long",
            "int" or "int32" => "int",
            "smallint" or "short" or "int16" => "short",
            "tinyint" or "byte" => "byte",
            "bit" or "bool" or "boolean" => "bool",
            "decimal" or "numeric" or "money" or "smallmoney" => "decimal",
            "float" or "double" => "double",
            "real" => "float",
            "datetime" or "datetime2" or "date" or "smalldatetime" => "DateTime",
            "time" => "TimeSpan",
            "datetimeoffset" => "DateTimeOffset",
            "uniqueidentifier" or "guid" => "Guid",
            "nvarchar" or "varchar" or "char" or "nchar" or "text" or "ntext" or "string" => "string",
            "varbinary" or "binary" or "image" => "byte[]",
            _ => type
        };

        if (field.IsNullable && csharpType != "string" && csharpType != "byte[]" && !csharpType.EndsWith('?'))
        {
            return csharpType + "?";
        }

        if (field.IsNullable && csharpType == "string")
        {
            return "string?";
        }

        return csharpType;
    }

    private static string Escape(string value)
    {
        return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
