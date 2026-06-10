using System.Text;
using CodeGen.Core.Models;
using CodeGen.Core.Services.Shared;

namespace CodeGen.Core.Services.Frontend;

public sealed class ReactTemplateTokenBuilder
{
    private readonly EntityNamingService _naming;

    public ReactTemplateTokenBuilder(EntityNamingService naming)
    {
        _naming = naming;
    }

    public IReadOnlyDictionary<string, string> Build(EntitySchema schema, string frontendAppName)
    {
        var entityName = _naming.NormalizeEntityName(schema.EntityName);
        var entityPlural = string.IsNullOrWhiteSpace(schema.EntityPlural)
            ? _naming.ToPlural(entityName)
            : _naming.NormalizeEntityName(schema.EntityPlural!);

        var entityVariable = _naming.ToCamelCase(entityName);
        var entityPluralVariable = _naming.ToCamelCase(entityPlural);
        var routeSegment = _naming.ToKebabCase(entityPlural);
        var key = schema.Fields.FirstOrDefault(x => x.IsKey)
            ?? schema.Fields.FirstOrDefault(x => x.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"No primary key/Id column found for {entityName}.");

        var tableFields = schema.Fields.ToList();
        var relationDisplayFields = BuildRelationDisplayFields(schema).ToList();
        var readFields = MergeFields(tableFields, relationDisplayFields).ToList();
        var createFields = tableFields.Where(x => !x.IsKey && !x.IsIdentity && x.IncludeInCreate).ToList();
        var updateFields = tableFields.Where(x => !x.IsKey && !x.IsIdentity && x.IncludeInUpdate).ToList();
        var displayFields = MergeFields(tableFields.Where(x => !IsDeletedColumn(x)), relationDisplayFields).ToList();

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
            ["FrontendAppName"] = frontendAppName,
            ["EntityName"] = entityName,
            ["EntityPlural"] = entityPlural,
            ["EntityVariable"] = entityVariable,
            ["EntityPluralVariable"] = entityPluralVariable,
            ["FeatureFolder"] = entityPluralVariable,
            ["RouteSegment"] = routeSegment,
            ["ApiRoute"] = $"/api/{entityPlural}",
            ["KeyName"] = key.Name,
            ["KeyVariable"] = _naming.ToCamelCase(key.Name),
            ["KeyTypeTs"] = ToTypeScriptBaseType(key),
            ["RouteKeyExpression"] = BuildRouteKeyExpression(key),
            ["InvalidRouteKeyCondition"] = BuildInvalidRouteKeyCondition(key),
            ["TsModelProperties"] = BuildTsProperties(readFields),
            ["TsCreateRequestProperties"] = BuildTsProperties(createFields),
            ["TsUpdateRequestProperties"] = BuildTsProperties(updateFields),
            ["InitialFormState"] = BuildInitialFormState(createFields),
            ["EditFormStateAssignments"] = BuildEditFormStateAssignments(updateFields, entityVariable),
            ["ReactFormImport"] = BuildReactFormImport(schema),
            ["LookupImports"] = BuildLookupImports(schema),
            ["LookupStateAndEffects"] = BuildLookupStateAndEffects(schema),
            ["FormInputs"] = BuildFormInputs(createFields, schema.Relations),
            ["TableHeaders"] = BuildTableHeaders(displayFields),
            ["TableCells"] = BuildTableCells(displayFields, entityVariable)
        };
    }

    private static IEnumerable<EntityField> BuildRelationDisplayFields(EntitySchema schema)
    {
        foreach (var relation in schema.Relations)
        {
            yield return new EntityField
            {
                Name = relation.LookupDisplayProperty,
                ColumnName = relation.LookupDisplayColumn,
                Type = "string",
                IsNullable = true,
                IncludeInCreate = false,
                IncludeInUpdate = false
            };
        }
    }

    private static IEnumerable<EntityField> MergeFields(IEnumerable<EntityField> tableFields, IEnumerable<EntityField> relationFields)
    {
        var output = tableFields.ToList();
        foreach (var relationField in relationFields)
        {
            if (!output.Any(x => x.Name.Equals(relationField.Name, StringComparison.OrdinalIgnoreCase)))
            {
                output.Add(relationField);
            }
        }

        return output;
    }

    private string BuildTsProperties(IEnumerable<EntityField> fields)
    {
        return string.Join(Environment.NewLine, fields.Select(field =>
        {
            var propertyName = _naming.ToCamelCase(field.Name);
            var optional = field.IsNullable ? "?" : string.Empty;
            return $"  {propertyName}{optional}: {ToTypeScriptType(field)};";
        }));
    }

    private string BuildInitialFormState(IEnumerable<EntityField> fields)
    {
        return string.Join("," + Environment.NewLine, fields.Select(field =>
            $"  {_naming.ToCamelCase(field.Name)}: {GetInitialValue(field)}"));
    }

    private string BuildEditFormStateAssignments(IEnumerable<EntityField> fields, string entityVariable)
    {
        return string.Join("," + Environment.NewLine, fields.Select(field =>
        {
            var propertyName = _naming.ToCamelCase(field.Name);
            return $"        {propertyName}: {BuildValueFromLoadedEntity(field, entityVariable)}";
        }));
    }

    private static string BuildReactFormImport(EntitySchema schema)
    {
        return schema.Relations.Count == 0
            ? "import type { FormEvent } from \"react\";"
            : "import { type FormEvent, useEffect, useState } from \"react\";";
    }

    private string BuildLookupImports(EntitySchema schema)
    {
        if (schema.Relations.Count == 0)
        {
            return string.Empty;
        }

        return string.Join(Environment.NewLine, schema.Relations.Select(relation =>
            $"import type {{ {relation.LookupEntityName} }} from \"../../{relation.LookupFeatureFolder}/models/{relation.LookupEntityName}\";{Environment.NewLine}import {{ {relation.LookupEntityPluralVariable}Service }} from \"../../{relation.LookupFeatureFolder}/services/{relation.LookupEntityPluralVariable}Service\";"));
    }

    private string BuildLookupStateAndEffects(EntitySchema schema)
    {
        if (schema.Relations.Count == 0)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();
        foreach (var relation in schema.Relations)
        {
            var itemsVariable = relation.LookupEntityPluralVariable;
            var setItems = "set" + relation.LookupEntityPlural;
            var loadingVariable = "isLoading" + relation.LookupEntityPlural;
            var setLoading = "setIsLoading" + relation.LookupEntityPlural;

            builder.AppendLine($"  const [{itemsVariable}, {setItems}] = useState<{relation.LookupEntityName}[]>([]);");
            builder.AppendLine($"  const [{loadingVariable}, {setLoading}] = useState(false);");
            builder.AppendLine();
            builder.AppendLine("  useEffect(() => {");
            builder.AppendLine("    let isMounted = true;");
            builder.AppendLine($"    {setLoading}(true);");
            builder.AppendLine();
            builder.AppendLine($"    {itemsVariable}Service.getAll()");
            builder.AppendLine("      .then((data) => {");
            builder.AppendLine("        if (isMounted) {");
            builder.AppendLine($"          {setItems}(data);");
            builder.AppendLine("        }");
            builder.AppendLine("      })");
            builder.AppendLine("      .catch((exception: unknown) => {");
            builder.AppendLine($"        console.error(\"Failed to load {relation.LookupEntityPlural}.\", exception);");
            builder.AppendLine("      })");
            builder.AppendLine("      .finally(() => {");
            builder.AppendLine("        if (isMounted) {");
            builder.AppendLine($"          {setLoading}(false);");
            builder.AppendLine("        }");
            builder.AppendLine("      });");
            builder.AppendLine();
            builder.AppendLine("    return () => {");
            builder.AppendLine("      isMounted = false;");
            builder.AppendLine("    };");
            builder.AppendLine("  }, []);");
            builder.AppendLine();
        }

        return builder.ToString().TrimEnd();
    }

    private string BuildFormInputs(IEnumerable<EntityField> fields, IReadOnlyList<EntityRelation> relations)
    {
        var builder = new StringBuilder();
        foreach (var field in fields)
        {
            var relation = relations.FirstOrDefault(x => x.LocalProperty.Equals(field.Name, StringComparison.OrdinalIgnoreCase));
            if (relation is not null)
            {
                AppendLookupSelect(builder, field, relation);
                continue;
            }

            var propertyName = _naming.ToCamelCase(field.Name);
            var label = _naming.ToTitleCaseWords(field.Name);
            builder.AppendLine("      <div className=\"form-field\">");
            builder.AppendLine($"        <label htmlFor=\"{propertyName}\">{label}</label>");

            if (IsBoolean(field))
            {
                builder.AppendLine("        <input");
                builder.AppendLine($"          id=\"{propertyName}\"");
                builder.AppendLine($"          name=\"{propertyName}\"");
                builder.AppendLine("          type=\"checkbox\"");
                builder.AppendLine($"          checked={{Boolean(value.{propertyName})}}");
                builder.AppendLine($"          onChange={{(event) => onChange(\"{propertyName}\", event.target.checked)}}");
                builder.AppendLine("        />");
            }
            else if (IsLongText(field))
            {
                builder.AppendLine("        <textarea");
                builder.AppendLine($"          id=\"{propertyName}\"");
                builder.AppendLine($"          name=\"{propertyName}\"");
                builder.AppendLine($"          value={{value.{propertyName} ?? \"\"}}");
                builder.AppendLine($"          onChange={{(event) => onChange(\"{propertyName}\", {BuildStringChangeExpression(field)})}}");
                builder.AppendLine("        />");
            }
            else
            {
                builder.AppendLine("        <input");
                builder.AppendLine($"          id=\"{propertyName}\"");
                builder.AppendLine($"          name=\"{propertyName}\"");
                builder.AppendLine($"          type=\"{GetInputType(field)}\"");
                builder.AppendLine($"          value={{{BuildInputValueExpression(field, propertyName)}}}");
                builder.AppendLine($"          onChange={{(event) => onChange(\"{propertyName}\", {BuildChangeExpression(field)})}}");
                builder.AppendLine("        />");
            }

            builder.AppendLine("      </div>");
        }

        return builder.ToString().TrimEnd();
    }

    private void AppendLookupSelect(StringBuilder builder, EntityField field, EntityRelation relation)
    {
        var propertyName = _naming.ToCamelCase(field.Name);
        var label = _naming.ToTitleCaseWords(relation.RelationName);
        var itemsVariable = relation.LookupEntityPluralVariable;
        var itemVariable = _naming.ToCamelCase(relation.LookupEntityName);
        var keyProperty = _naming.ToCamelCase(relation.LookupKeyProperty);
        var displayProperty = _naming.ToCamelCase(relation.LookupDisplayProperty);
        var loadingVariable = "isLoading" + relation.LookupEntityPlural;

        builder.AppendLine("      <div className=\"form-field\">");
        builder.AppendLine($"        <label htmlFor=\"{propertyName}\">{label}</label>");
        builder.AppendLine("        <select");
        builder.AppendLine($"          id=\"{propertyName}\"");
        builder.AppendLine($"          name=\"{propertyName}\"");
        builder.AppendLine($"          value={{value.{propertyName} ?? \"\"}}");
        builder.AppendLine($"          onChange={{(event) => onChange(\"{propertyName}\", event.target.value === \"\" ? null : Number(event.target.value))}}");
        builder.AppendLine($"          disabled={{isSubmitting || {loadingVariable}}}");
        builder.AppendLine("        >");
        builder.AppendLine($"          <option value=\"\">Select {relation.RelationName}</option>");
        builder.AppendLine($"          {{{itemsVariable}.map(({itemVariable}) => (");
        builder.AppendLine($"            <option key={{{itemVariable}.{keyProperty}}} value={{{itemVariable}.{keyProperty}}}>");
        builder.AppendLine($"              {{{itemVariable}.{displayProperty}}}");
        builder.AppendLine("            </option>");
        builder.AppendLine("          ))}");
        builder.AppendLine("        </select>");
        builder.AppendLine("      </div>");
    }

    private string BuildTableHeaders(IEnumerable<EntityField> fields)
    {
        return string.Join(Environment.NewLine, fields.Select(field =>
            $"            <th>{_naming.ToTitleCaseWords(field.Name)}</th>"));
    }

    private string BuildTableCells(IEnumerable<EntityField> fields, string entityVariable)
    {
        return string.Join(Environment.NewLine, fields.Select(field =>
        {
            var propertyName = _naming.ToCamelCase(field.Name);
            return $"            <td>{{formatDisplayValue({entityVariable}.{propertyName})}}</td>";
        }));
    }

    private string BuildInputValueExpression(EntityField field, string propertyName)
    {
        if (IsDateLike(field))
        {
            return $"(value.{propertyName} ?? \"\").substring(0, 10)";
        }

        return $"value.{propertyName} ?? \"\"";
    }

    private string BuildChangeExpression(EntityField field)
    {
        if (IsNumber(field))
        {
            return field.IsNullable
                ? "event.target.value === \"\" ? null : Number(event.target.value)"
                : "event.target.value === \"\" ? 0 : Number(event.target.value)";
        }

        return BuildStringChangeExpression(field);
    }

    private static string BuildStringChangeExpression(EntityField field)
    {
        return field.IsNullable
            ? "event.target.value === \"\" ? null : event.target.value"
            : "event.target.value";
    }

    private static string GetInputType(EntityField field)
    {
        if (IsDateLike(field))
        {
            return "date";
        }

        if (IsNumber(field))
        {
            return "number";
        }

        return "text";
    }

    private static string GetInitialValue(EntityField field)
    {
        if (IsBoolean(field))
        {
            return "false";
        }

        if (IsNumber(field))
        {
            return field.IsNullable ? "null" : "0";
        }

        if (field.IsNullable)
        {
            return "null";
        }

        return "\"\"";
    }

    private string BuildValueFromLoadedEntity(EntityField field, string entityVariable)
    {
        var propertyName = _naming.ToCamelCase(field.Name);

        if (IsDateLike(field))
        {
            return $"{entityVariable}.{propertyName} ? {entityVariable}.{propertyName}.substring(0, 10) : \"\"";
        }

        if (IsBoolean(field))
        {
            return $"Boolean({entityVariable}.{propertyName})";
        }

        if (IsNumber(field))
        {
            return field.IsNullable
                ? $"{entityVariable}.{propertyName} ?? null"
                : $"{entityVariable}.{propertyName}";
        }

        return field.IsNullable
            ? $"{entityVariable}.{propertyName} ?? null"
            : $"{entityVariable}.{propertyName} ?? \"\"";
    }

    private static string BuildRouteKeyExpression(EntityField key)
    {
        return IsNumber(key) ? "Number(idParam)" : "idParam";
    }

    private static string BuildInvalidRouteKeyCondition(EntityField key)
    {
        return IsNumber(key) ? "!idParam || Number.isNaN(id)" : "!id";
    }

    private static string ToTypeScriptType(EntityField field)
    {
        var baseType = ToTypeScriptBaseType(field);
        return field.IsNullable ? $"{baseType} | null" : baseType;
    }

    private static string ToTypeScriptBaseType(EntityField field)
    {
        var type = field.Type.Trim().ToLowerInvariant();
        return type switch
        {
            "bigint" or "long" or "int64" or "int" or "int32" or "smallint" or "short" or "int16" or "tinyint" or "byte" => "number",
            "decimal" or "numeric" or "money" or "smallmoney" or "float" or "double" or "real" => "number",
            "bit" or "bool" or "boolean" => "boolean",
            "datetime" or "datetime2" or "date" or "smalldatetime" or "time" or "datetimeoffset" => "string",
            "uniqueidentifier" or "guid" => "string",
            "nvarchar" or "varchar" or "char" or "nchar" or "text" or "ntext" or "string" => "string",
            "varbinary" or "binary" or "image" => "string",
            _ => "string"
        };
    }

    private static bool IsNumber(EntityField field)
    {
        var type = field.Type.Trim().ToLowerInvariant();
        return type is "bigint" or "long" or "int64" or "int" or "int32" or "smallint" or "short" or "int16" or "tinyint" or "byte"
            or "decimal" or "numeric" or "money" or "smallmoney" or "float" or "double" or "real";
    }

    private static bool IsBoolean(EntityField field)
    {
        var type = field.Type.Trim().ToLowerInvariant();
        return type is "bit" or "bool" or "boolean";
    }

    private static bool IsDateLike(EntityField field)
    {
        var type = field.Type.Trim().ToLowerInvariant();
        return type is "datetime" or "datetime2" or "date" or "smalldatetime" or "time" or "datetimeoffset";
    }

    private static bool IsLongText(EntityField field)
    {
        var type = field.Type.Trim().ToLowerInvariant();
        return (type is "text" or "ntext") || (field.MaxLength.HasValue && field.MaxLength.Value >= 250) || field.Name.Contains("Address", StringComparison.OrdinalIgnoreCase)
            || field.Name.Contains("Description", StringComparison.OrdinalIgnoreCase)
            || field.Name.Contains("Notes", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsDeletedColumn(EntityField field)
    {
        return field.Name.Equals("IsDeleted", StringComparison.OrdinalIgnoreCase);
    }
}
