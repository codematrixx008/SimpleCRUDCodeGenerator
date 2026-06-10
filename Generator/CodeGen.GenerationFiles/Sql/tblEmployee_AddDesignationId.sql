/*
    Adds DesignationId to dbo.tblEmployee without creating a hard foreign key.
    This keeps the relation soft / convention-based for the code generator.
*/

IF COL_LENGTH('dbo.tblEmployee', 'DesignationId') IS NULL
BEGIN
    ALTER TABLE dbo.tblEmployee
    ADD DesignationId INT NULL;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_tblEmployee_DesignationId'
      AND object_id = OBJECT_ID('dbo.tblEmployee')
)
BEGIN
    CREATE INDEX IX_tblEmployee_DesignationId
    ON dbo.tblEmployee(DesignationId);
END;
GO

/*
    Optional dummy assignment:
    Assigns a random active designation to existing employees where DesignationId is null.
*/
UPDATE e
SET DesignationId = d.Id
FROM dbo.tblEmployee e
CROSS APPLY
(
    SELECT TOP 1 Id
    FROM dbo.tblDesignation
    WHERE IsDeleted = 0
    ORDER BY NEWID()
) d
WHERE e.DesignationId IS NULL;
GO

SELECT
    e.Id,
    e.FirstName,
    e.LastName,
    e.DepartmentId,
    e.DesignationId,
    d.DesignationName
FROM dbo.tblEmployee e
LEFT JOIN dbo.tblDesignation d
    ON d.Id = e.DesignationId
   AND d.IsDeleted = 0
ORDER BY e.Id;
GO
