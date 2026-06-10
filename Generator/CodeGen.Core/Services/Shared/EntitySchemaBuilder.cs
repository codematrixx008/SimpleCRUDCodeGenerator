using CodeGen.Core.Models;
using CodeGen.Core.Schema;

namespace CodeGen.Core.Services.Shared;

public sealed class EntitySchemaBuilder
{
    private readonly EntityNamingService _naming;

    public EntitySchemaBuilder(EntityNamingService naming)
    {
        _naming = naming;
    }

    public EntitySchema Build(
        string tableName,
        SchemaResult schemaResult,
        IReadOnlyList<RelationDefinition>? relations = null)
    {
        if (schemaResult.TableColumns.Count == 0)
        {
            throw new InvalidOperationException($"No columns were returned for table '{tableName}'. Check the configured schema stored procedure.");
        }

        var (schemaName, tableOnlyName) = _naming.SplitSchemaAndTableName(tableName);
        var entityName = _naming.ToEntityNameFromTableName(tableName);
        var fields = schemaResult.TableColumns
            .OrderBy(x => x.OrdinalPosition)
            .Select(MapColumnToField)
            .ToList();

        var key = fields.FirstOrDefault(x => x.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
        if (key is not null)
        {
            key.IsKey = true;
        }

        if (!fields.Any(x => x.IsKey))
        {
            throw new InvalidOperationException($"Table '{tableName}' must have an Id column for this simple CRUD generator.");
        }

        var schema = new EntitySchema
        {
            EntityName = entityName,
            EntityPlural = _naming.ToPlural(entityName),
            DbSchemaName = schemaName,
            DbTableName = tableOnlyName,
            Fields = fields
        };

        schema.Relations = BuildRelations(schema, relations ?? Array.Empty<RelationDefinition>());
        return schema;
    }

    private List<EntityRelation> BuildRelations(EntitySchema schema, IReadOnlyList<RelationDefinition> relationDefinitions)
    {
        var relations = new List<EntityRelation>();

        for (var index = 0; index < relationDefinitions.Count; index++)
        {
            var relationDefinition = relationDefinitions[index];

            if (string.IsNullOrWhiteSpace(relationDefinition.LocalColumn))
            {
                throw new ArgumentException("Relation LocalColumn is required.", nameof(relationDefinitions));
            }

            if (string.IsNullOrWhiteSpace(relationDefinition.LookupTableName))
            {
                throw new ArgumentException("Relation LookupTableName is required.", nameof(relationDefinitions));
            }

            if (string.IsNullOrWhiteSpace(relationDefinition.LookupDisplayColumn))
            {
                throw new ArgumentException("Relation LookupDisplayColumn is required.", nameof(relationDefinitions));
            }

            _naming.ValidateTableName(relationDefinition.LookupTableName);

            var localProperty = _naming.NormalizeEntityName(relationDefinition.LocalColumn);
            var localField = schema.Fields.FirstOrDefault(x =>
                x.ColumnName.Equals(relationDefinition.LocalColumn, StringComparison.OrdinalIgnoreCase)
                || x.Name.Equals(localProperty, StringComparison.OrdinalIgnoreCase));

            if (localField is null)
            {
                throw new InvalidOperationException($"Relation local column '{relationDefinition.LocalColumn}' was not found in entity '{schema.EntityName}'. Add the column to the database table first.");
            }

            var (lookupSchemaName, lookupTableName) = _naming.SplitSchemaAndTableName(relationDefinition.LookupTableName);
            lookupSchemaName ??= "dbo";

            var lookupEntityName = _naming.ToEntityNameFromTableName(relationDefinition.LookupTableName);
            var lookupEntityPlural = _naming.ToPlural(lookupEntityName);
            var lookupKeyColumn = string.IsNullOrWhiteSpace(relationDefinition.LookupKeyColumn)
                ? "Id"
                : relationDefinition.LookupKeyColumn.Trim();

            var lookupDisplayColumn = relationDefinition.LookupDisplayColumn.Trim();
            var lookupDisplayProperty = _naming.NormalizeEntityName(lookupDisplayColumn);
            var relationName = string.IsNullOrWhiteSpace(relationDefinition.RelationName)
                ? lookupEntityName
                : _naming.NormalizeEntityName(relationDefinition.RelationName!);

            relations.Add(new EntityRelation
            {
                RelationName = relationName,
                LocalColumn = localField.ColumnName,
                LocalProperty = localField.Name,
                LookupSchemaName = lookupSchemaName,
                LookupTableName = lookupTableName,
                LookupTableFullName = $"{QuoteIdentifier(lookupSchemaName)}.{QuoteIdentifier(lookupTableName)}",
                LookupEntityName = lookupEntityName,
                LookupEntityPlural = lookupEntityPlural,
                LookupFeatureFolder = _naming.ToCamelCase(lookupEntityPlural),
                LookupEntityPluralVariable = _naming.ToCamelCase(lookupEntityPlural),
                LookupKeyColumn = lookupKeyColumn,
                LookupKeyProperty = _naming.NormalizeEntityName(lookupKeyColumn),
                LookupDisplayColumn = lookupDisplayColumn,
                LookupDisplayProperty = lookupDisplayProperty,
                LookupDisplayVariable = _naming.ToCamelCase(lookupDisplayProperty),
                Alias = $"l{index + 1}",
                Required = relationDefinition.Required
            });
        }

        return relations;
    }

    private EntityField MapColumnToField(DbColumnSchema column)
    {
        var propertyName = _naming.NormalizeEntityName(column.ColumnName);
        var isId = propertyName.Equals("Id", StringComparison.OrdinalIgnoreCase);

        return new EntityField
        {
            Name = propertyName,
            ColumnName = column.ColumnName,
            Type = column.SqlType,
            IsKey = isId,
            IsIdentity = column.IsIdentity,
            IsNullable = column.IsNullable,
            MaxLength = NormalizeMaxLength(column),
            Precision = column.Precision,
            Scale = column.Scale,
            IncludeInCreate = !isId && !column.IsIdentity && !IsAuditColumn(propertyName),
            IncludeInUpdate = !isId && !column.IsIdentity && !IsAuditColumn(propertyName)
        };
    }

    private static int? NormalizeMaxLength(DbColumnSchema column)
    {
        if (column.MaxLength is null or <= 0)
        {
            return null;
        }

        // The included dbo.usp_GetObjectSchemas script already returns character length for nvarchar/nchar.
        return column.MaxLength;
    }

    private static bool IsAuditColumn(string propertyName)
    {
        return propertyName.Equals("CreatedDate", StringComparison.OrdinalIgnoreCase)
            || propertyName.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase)
            || propertyName.Equals("CreatedBy", StringComparison.OrdinalIgnoreCase)
            || propertyName.Equals("UpdatedDate", StringComparison.OrdinalIgnoreCase)
            || propertyName.Equals("UpdatedAt", StringComparison.OrdinalIgnoreCase)
            || propertyName.Equals("ModifiedDate", StringComparison.OrdinalIgnoreCase)
            || propertyName.Equals("ModifiedAt", StringComparison.OrdinalIgnoreCase)
            || propertyName.Equals("UpdatedBy", StringComparison.OrdinalIgnoreCase)
            || propertyName.Equals("ModifiedBy", StringComparison.OrdinalIgnoreCase)
            || propertyName.Equals("IsDeleted", StringComparison.OrdinalIgnoreCase);
    }

    private static string QuoteIdentifier(string value)
    {
        return $"[{value.Replace("]", "]]", StringComparison.Ordinal)}]";
    }
}
