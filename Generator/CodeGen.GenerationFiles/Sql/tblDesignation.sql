/*
    Creates dbo.tblDesignation and inserts dummy designation data.
    Run this in the same database used by SimpleEmployeeCRUD.
*/

IF OBJECT_ID('dbo.tblDesignation', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblDesignation
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_tblDesignation PRIMARY KEY,

        DesignationName NVARCHAR(100) NOT NULL,
        DesignationCode NVARCHAR(20) NOT NULL,
        Description NVARCHAR(500) NULL,

        CreatedDate DATETIME2(7) NOT NULL
            CONSTRAINT DF_tblDesignation_CreatedDate DEFAULT SYSUTCDATETIME(),

        UpdatedDate DATETIME2(7) NULL,

        IsDeleted BIT NOT NULL
            CONSTRAINT DF_tblDesignation_IsDeleted DEFAULT 0
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_tblDesignation_DesignationCode'
      AND object_id = OBJECT_ID('dbo.tblDesignation')
)
BEGIN
    CREATE UNIQUE INDEX UX_tblDesignation_DesignationCode
    ON dbo.tblDesignation(DesignationCode)
    WHERE IsDeleted = 0;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.tblDesignation WHERE DesignationCode = 'SE')
BEGIN
    INSERT INTO dbo.tblDesignation
    (
        DesignationName,
        DesignationCode,
        Description
    )
    VALUES
    ('Software Engineer', 'SE', 'Builds and maintains software applications'),
    ('Senior Software Engineer', 'SSE', 'Handles complex development tasks and mentoring'),
    ('Team Lead', 'TL', 'Leads a small development team'),
    ('Project Manager', 'PM', 'Manages project delivery and coordination'),
    ('HR Executive', 'HRE', 'Handles HR operations and employee support'),
    ('Accountant', 'ACC', 'Handles finance and accounting activities');
END;
GO

SELECT *
FROM dbo.tblDesignation
WHERE IsDeleted = 0
ORDER BY Id;
GO
