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

        var allFields = schema.Fields.ToList();
        var createFields = allFields.Where(x => !x.IsKey && !x.IsIdentity && x.IncludeInCreate).ToList();
        var updateFields = allFields.Where(x => !x.IsKey && !x.IsIdentity && x.IncludeInUpdate).ToList();

        if (createFields.Count == 0)
        {
            throw new InvalidOperationException($"No insertable columns found for {entityName}.");
        }

        if (updateFields.Count == 0)
        {
            throw new InvalidOperationException($"No updateable columns found for {entityName}.");
        }

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
            ["KeyColumnName"] = GetColumnName(key),
            ["ModelProperties"] = BuildProperties(allFields),
            ["DtoProperties"] = BuildProperties(allFields),
            ["CreateDtoProperties"] = BuildProperties(createFields),
            ["UpdateDtoProperties"] = BuildProperties(updateFields),
            ["CreateEntityAssignments"] = BuildCreateAssignments(createFields),
            ["UpdateEntityAssignments"] = BuildUpdateAssignments(updateFields, entityVariable),
            ["DtoAssignments"] = BuildDtoAssignments(allFields, entityVariable),
            ["TableFullName"] = BuildTableFullName(schema),
            ["SelectColumnList"] = BuildSelectColumnList(allFields),
            ["InsertColumnList"] = BuildInsertColumnList(createFields),
            ["InsertValueList"] = BuildInsertValueList(createFields),
            ["OutputInsertedColumnList"] = BuildOutputInsertedColumnList(allFields),
            ["UpdateSetList"] = BuildUpdateSetList(updateFields, allFields),
            ["WhereNotDeleted"] = BuildWhereNotDeleted(allFields),
            ["AndNotDeleted"] = BuildAndNotDeleted(allFields),
            ["DeleteSql"] = BuildDeleteSql(schema, key, allFields)
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

    private static string BuildTableFullName(EntitySchema schema)
    {
        var tableName = string.IsNullOrWhiteSpace(schema.DbTableName) ? schema.EntityName : schema.DbTableName;
        if (string.IsNullOrWhiteSpace(schema.DbSchemaName))
        {
            return QuoteIdentifier(tableName);
        }

        return $"{QuoteIdentifier(schema.DbSchemaName!)}.{QuoteIdentifier(tableName)}";
    }

    private static string BuildSelectColumnList(IEnumerable<EntityField> fields)
    {
        return string.Join("," + Environment.NewLine, fields.Select(field =>
            $"    {QuoteIdentifier(GetColumnName(field))} AS {QuoteIdentifier(field.Name)}"));
    }

    private static string BuildInsertColumnList(IEnumerable<EntityField> fields)
    {
        return string.Join("," + Environment.NewLine, fields.Select(field =>
            $"    {QuoteIdentifier(GetColumnName(field))}"));
    }

    private static string BuildInsertValueList(IEnumerable<EntityField> fields)
    {
        return string.Join("," + Environment.NewLine, fields.Select(field =>
            $"    @{field.Name}"));
    }

    private static string BuildOutputInsertedColumnList(IEnumerable<EntityField> fields)
    {
        return string.Join("," + Environment.NewLine, fields.Select(field =>
            $"    INSERTED.{QuoteIdentifier(GetColumnName(field))} AS {QuoteIdentifier(field.Name)}"));
    }

    private static string BuildUpdateSetList(IEnumerable<EntityField> updateFields, IEnumerable<EntityField> allFields)
    {
        var setParts = updateFields
            .Select(field => $"    {QuoteIdentifier(GetColumnName(field))} = @{field.Name}")
            .ToList();

        var updatedDateField = allFields.FirstOrDefault(IsUpdatedDateColumn);
        if (updatedDateField is not null)
        {
            setParts.Add($"    {QuoteIdentifier(GetColumnName(updatedDateField))} = SYSUTCDATETIME()");
        }

        return string.Join("," + Environment.NewLine, setParts);
    }

    private static string BuildWhereNotDeleted(IEnumerable<EntityField> fields)
    {
        var isDeleted = fields.FirstOrDefault(IsDeletedColumn);
        if (isDeleted is null)
        {
            return string.Empty;
        }

        return $"WHERE {QuoteIdentifier(GetColumnName(isDeleted))} = CAST(0 AS bit)";
    }

    private static string BuildAndNotDeleted(IEnumerable<EntityField> fields)
    {
        var isDeleted = fields.FirstOrDefault(IsDeletedColumn);
        if (isDeleted is null)
        {
            return string.Empty;
        }

        return $" AND {QuoteIdentifier(GetColumnName(isDeleted))} = CAST(0 AS bit)";
    }

    private static string BuildDeleteSql(EntitySchema schema, EntityField key, IEnumerable<EntityField> fields)
    {
        var allFields = fields.ToList();
        var tableFullName = BuildTableFullName(schema);
        var keyColumnName = QuoteIdentifier(GetColumnName(key));
        var isDeleted = allFields.FirstOrDefault(IsDeletedColumn);

        if (isDeleted is null)
        {
            return $"DELETE FROM {tableFullName}{Environment.NewLine}WHERE {keyColumnName} = @Id;";
        }

        var setParts = new List<string>
        {
            $"    {QuoteIdentifier(GetColumnName(isDeleted))} = CAST(1 AS bit)"
        };

        var updatedDate = allFields.FirstOrDefault(IsUpdatedDateColumn);
        if (updatedDate is not null)
        {
            setParts.Add($"    {QuoteIdentifier(GetColumnName(updatedDate))} = SYSUTCDATETIME()");
        }

        return $"UPDATE {tableFullName}{Environment.NewLine}SET{Environment.NewLine}{string.Join("," + Environment.NewLine, setParts)}{Environment.NewLine}WHERE {keyColumnName} = @Id AND {QuoteIdentifier(GetColumnName(isDeleted))} = CAST(0 AS bit);";
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

    private static bool IsUpdatedDateColumn(EntityField field)
    {
        return field.Name.Equals("UpdatedDate", StringComparison.OrdinalIgnoreCase)
            || field.Name.Equals("UpdatedAt", StringComparison.OrdinalIgnoreCase)
            || field.Name.Equals("ModifiedDate", StringComparison.OrdinalIgnoreCase)
            || field.Name.Equals("ModifiedAt", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsDeletedColumn(EntityField field)
    {
        return field.Name.Equals("IsDeleted", StringComparison.OrdinalIgnoreCase);
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

    private static string GetColumnName(EntityField field)
    {
        return string.IsNullOrWhiteSpace(field.ColumnName) ? field.Name : field.ColumnName;
    }

    private static string QuoteIdentifier(string value)
    {
        return $"[{value.Replace("]", "]]", StringComparison.Ordinal)}]";
    }
}
