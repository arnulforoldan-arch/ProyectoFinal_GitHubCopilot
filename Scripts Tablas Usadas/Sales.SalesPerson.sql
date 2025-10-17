USE [AdventureWorks2014]
GO

/****** Object: Table [Sales].[SalesPerson] Script Date: 2/10/2025 12:54:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Sales].[SalesPerson] (
    [BusinessEntityID] INT              NOT NULL,
    [TerritoryID]      INT              NULL,
    [SalesQuota]       MONEY            NULL,
    [Bonus]            MONEY            NOT NULL,
    [CommissionPct]    SMALLMONEY       NOT NULL,
    [SalesYTD]         MONEY            NOT NULL,
    [SalesLastYear]    MONEY            NOT NULL,
    [rowguid]          UNIQUEIDENTIFIER ROWGUIDCOL NOT NULL,
    [ModifiedDate]     DATETIME         NOT NULL
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [AK_SalesPerson_rowguid]
    ON [Sales].[SalesPerson]([rowguid] ASC);


GO
ALTER TABLE [Sales].[SalesPerson]
    ADD CONSTRAINT [PK_SalesPerson_BusinessEntityID] PRIMARY KEY CLUSTERED ([BusinessEntityID] ASC);


GO
ALTER TABLE [Sales].[SalesPerson]
    ADD CONSTRAINT [FK_SalesPerson_Employee_BusinessEntityID] FOREIGN KEY ([BusinessEntityID]) REFERENCES [HumanResources].[Employee] ([BusinessEntityID]);


GO
ALTER TABLE [Sales].[SalesPerson]
    ADD CONSTRAINT [FK_SalesPerson_SalesTerritory_TerritoryID] FOREIGN KEY ([TerritoryID]) REFERENCES [Sales].[SalesTerritory] ([TerritoryID]);


GO
ALTER TABLE [Sales].[SalesPerson]
    ADD CONSTRAINT [DF_SalesPerson_Bonus] DEFAULT ((0.00)) FOR [Bonus];


GO
ALTER TABLE [Sales].[SalesPerson]
    ADD CONSTRAINT [DF_SalesPerson_CommissionPct] DEFAULT ((0.00)) FOR [CommissionPct];


GO
ALTER TABLE [Sales].[SalesPerson]
    ADD CONSTRAINT [DF_SalesPerson_SalesYTD] DEFAULT ((0.00)) FOR [SalesYTD];


GO
ALTER TABLE [Sales].[SalesPerson]
    ADD CONSTRAINT [DF_SalesPerson_SalesLastYear] DEFAULT ((0.00)) FOR [SalesLastYear];


GO
ALTER TABLE [Sales].[SalesPerson]
    ADD CONSTRAINT [DF_SalesPerson_rowguid] DEFAULT (newid()) FOR [rowguid];


GO
ALTER TABLE [Sales].[SalesPerson]
    ADD CONSTRAINT [DF_SalesPerson_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate];


GO
ALTER TABLE [Sales].[SalesPerson]
    ADD CONSTRAINT [CK_SalesPerson_SalesQuota] CHECK ([SalesQuota]>(0.00));


GO
ALTER TABLE [Sales].[SalesPerson]
    ADD CONSTRAINT [CK_SalesPerson_Bonus] CHECK ([Bonus]>=(0.00));


GO
ALTER TABLE [Sales].[SalesPerson]
    ADD CONSTRAINT [CK_SalesPerson_CommissionPct] CHECK ([CommissionPct]>=(0.00));


GO
ALTER TABLE [Sales].[SalesPerson]
    ADD CONSTRAINT [CK_SalesPerson_SalesYTD] CHECK ([SalesYTD]>=(0.00));


GO
ALTER TABLE [Sales].[SalesPerson]
    ADD CONSTRAINT [CK_SalesPerson_SalesLastYear] CHECK ([SalesLastYear]>=(0.00));


