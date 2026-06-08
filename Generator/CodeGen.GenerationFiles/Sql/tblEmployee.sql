CREATE TABLE dbo.tblEmployee
(
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_tblEmployee PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    DOB DATE NOT NULL,
    Gender NVARCHAR(10) NOT NULL,
    Address NVARCHAR(250) NULL,
    CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_tblEmployee_CreatedDate DEFAULT SYSUTCDATETIME(),
    UpdatedDate DATETIME2(7) NULL,
    IsDeleted BIT NOT NULL CONSTRAINT DF_tblEmployee_IsDeleted DEFAULT 0
);
GO

INSERT INTO dbo.tblEmployee (FirstName, LastName, DOB, Gender, Address)
VALUES
('Rahul', 'Sharma', '1995-05-15', 'Male', 'Delhi, India'),
('Priya', 'Verma', '1998-09-22', 'Female', 'Mumbai, India');
GO
