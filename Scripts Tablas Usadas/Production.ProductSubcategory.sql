USE [AdventureWorks2014]
GO

/****** Object: Table [Production].[ProductSubcategory] Script Date: 2/10/2025 12:53:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Production].[ProductSubcategory] (
    [ProductSubcategoryID] INT              IDENTITY (1, 1) NOT NULL,
    [ProductCategoryID]    INT              NOT NULL,
    [Name]                 [dbo].[Name]     NOT NULL,
    [rowguid]              UNIQUEIDENTIFIER ROWGUIDCOL NOT NULL,
    [ModifiedDate]         DATETIME         NOT NULL
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [AK_ProductSubcategory_Name]
    ON [Production].[ProductSubcategory]([Name] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [AK_ProductSubcategory_rowguid]
    ON [Production].[ProductSubcategory]([rowguid] ASC);


GO
ALTER TABLE [Production].[ProductSubcategory]
    ADD CONSTRAINT [PK_ProductSubcategory_ProductSubcategoryID] PRIMARY KEY CLUSTERED ([ProductSubcategoryID] ASC);


GO
ALTER TABLE [Production].[ProductSubcategory]
    ADD CONSTRAINT [FK_ProductSubcategory_ProductCategory_ProductCategoryID] FOREIGN KEY ([ProductCategoryID]) REFERENCES [Production].[ProductCategory] ([ProductCategoryID]);


GO
ALTER TABLE [Production].[ProductSubcategory]
    ADD CONSTRAINT [DF_ProductSubcategory_rowguid] DEFAULT (newid()) FOR [rowguid];


GO
ALTER TABLE [Production].[ProductSubcategory]
    ADD CONSTRAINT [DF_ProductSubcategory_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate];


