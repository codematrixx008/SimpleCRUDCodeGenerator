CREATE OR ALTER PROCEDURE dbo.usp_GetObjectSchemas
    @TableName nvarchar(256)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @SchemaName sysname = ISNULL(PARSENAME(@TableName, 2), 'dbo');
    DECLARE @ObjectName sysname = PARSENAME(@TableName, 1);

    SELECT
        c.name AS ColumnName,
        t.name AS SqlType,
        CASE
            WHEN c.max_length = -1 THEN NULL
            ELSE c.max_length
        END AS MaxLength,
        c.precision AS Precision,
        c.scale AS Scale,
        CONVERT(bit, c.is_nullable) AS IsNullable,
        CONVERT(bit, c.is_identity) AS IsIdentity,
        c.column_id AS OrdinalPosition
    FROM sys.tables tb
    INNER JOIN sys.schemas s ON s.schema_id = tb.schema_id
    INNER JOIN sys.columns c ON c.object_id = tb.object_id
    INNER JOIN sys.types t ON t.user_type_id = c.user_type_id
    WHERE s.name = @SchemaName
      AND tb.name = @ObjectName
    ORDER BY c.column_id;

    -- Search-result schema. For this simple CRUD generator it is same as table schema.
    SELECT
        c.name AS ColumnName,
        t.name AS SqlType,
        CASE
            WHEN c.max_length = -1 THEN NULL
            ELSE c.max_length
        END AS MaxLength,
        c.precision AS Precision,
        c.scale AS Scale,
        CONVERT(bit, c.is_nullable) AS IsNullable,
        CONVERT(bit, c.is_identity) AS IsIdentity,
        c.column_id AS OrdinalPosition
    FROM sys.tables tb
    INNER JOIN sys.schemas s ON s.schema_id = tb.schema_id
    INNER JOIN sys.columns c ON c.object_id = tb.object_id
    INNER JOIN sys.types t ON t.user_type_id = c.user_type_id
    WHERE s.name = @SchemaName
      AND tb.name = @ObjectName
    ORDER BY c.column_id;
END;
GO
