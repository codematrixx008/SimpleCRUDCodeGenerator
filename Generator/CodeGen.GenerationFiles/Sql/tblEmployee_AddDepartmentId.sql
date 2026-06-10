USE SimpleEmployeeCRUDDb;
GO

IF COL_LENGTH('dbo.tblEmployee', 'DepartmentId') IS NULL
BEGIN
    ALTER TABLE dbo.tblEmployee
    ADD DepartmentId INT NULL;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_tblEmployee_DepartmentId'
      AND object_id = OBJECT_ID('dbo.tblEmployee')
)
BEGIN
    CREATE INDEX IX_tblEmployee_DepartmentId
    ON dbo.tblEmployee(DepartmentId);
END;
GO

-- Optional: assign a department to existing employees for testing.
UPDATE e
SET DepartmentId = d.Id
FROM dbo.tblEmployee e
CROSS APPLY
(
    SELECT TOP 1 Id
    FROM dbo.tblDepartment
    WHERE IsDeleted = 0
    ORDER BY Id
) d
WHERE e.DepartmentId IS NULL;
GO

SELECT e.Id, e.FirstName, e.LastName, e.DepartmentId, d.DepartmentName
FROM dbo.tblEmployee e
LEFT JOIN dbo.tblDepartment d
    ON d.Id = e.DepartmentId
   AND d.IsDeleted = 0;
GO
