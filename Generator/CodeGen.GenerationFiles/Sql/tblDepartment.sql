USE SimpleEmployeeCRUDDb;
GO

CREATE TABLE dbo.tblDepartment
(
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_tblDepartment PRIMARY KEY,

    DepartmentName NVARCHAR(100) NOT NULL,
    DepartmentCode NVARCHAR(20) NOT NULL,
    Description NVARCHAR(500) NULL,

    CreatedDate DATETIME2(7) NOT NULL
        CONSTRAINT DF_tblDepartment_CreatedDate DEFAULT SYSUTCDATETIME(),

    UpdatedDate DATETIME2(7) NULL,

    IsDeleted BIT NOT NULL
        CONSTRAINT DF_tblDepartment_IsDeleted DEFAULT 0
);
GO

CREATE UNIQUE INDEX UX_tblDepartment_DepartmentCode
ON dbo.tblDepartment(DepartmentCode)
WHERE IsDeleted = 0;
GO

INSERT INTO dbo.tblDepartment
(
    DepartmentName,
    DepartmentCode,
    Description
)
VALUES
('Human Resources', 'HR', 'Handles recruitment, employee records, and HR operations'),
('Information Technology', 'IT', 'Manages software, hardware, infrastructure, and support'),
('Finance', 'FIN', 'Handles accounts, payroll, budgets, and financial reporting'),
('Sales', 'SAL', 'Handles customer acquisition and revenue generation'),
('Administration', 'ADM', 'Handles office administration and facility operations');
GO

SELECT * FROM dbo.tblDepartment;
GO
