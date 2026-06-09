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

    public EntitySchema Build(string tableName, SchemaResult schemaResult)
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

        return new EntitySchema
        {
            EntityName = entityName,
            EntityPlural = _naming.ToPlural(entityName),
            DbSchemaName = schemaName,
            DbTableName = tableOnlyName,
            Fields = fields
        };
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

        var type = column.SqlType.ToLowerInvariant();
        if (type is "nvarchar" or "nchar")
        {
            return column.MaxLength.Value / 2;
        }

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
}
