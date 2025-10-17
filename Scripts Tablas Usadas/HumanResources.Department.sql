USE [AdventureWorks2014]
GO

/****** Object: Table [HumanResources].[Department] Script Date: 2/10/2025 12:51:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [HumanResources].[Department] (
    [DepartmentID] SMALLINT     IDENTITY (1, 1) NOT NULL,
    [Name]         [dbo].[Name] NOT NULL,
    [GroupName]    [dbo].[Name] NOT NULL,
    [ModifiedDate] DATETIME     NOT NULL
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [AK_Department_Name]
    ON [HumanResources].[Department]([Name] ASC);


GO
ALTER TABLE [HumanResources].[Department]
    ADD CONSTRAINT [PK_Department_DepartmentID] PRIMARY KEY CLUSTERED ([DepartmentID] ASC);


GO
ALTER TABLE [HumanResources].[Department]
    ADD CONSTRAINT [DF_Department_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate];


