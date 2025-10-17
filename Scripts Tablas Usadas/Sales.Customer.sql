USE [AdventureWorks2014]
GO

/****** Object: Table [Sales].[Customer] Script Date: 2/10/2025 12:54:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Sales].[Customer] (
    [CustomerID]    INT              IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [PersonID]      INT              NULL,
    [StoreID]       INT              NULL,
    [TerritoryID]   INT              NULL,
    [AccountNumber] AS               (isnull('AW'+[dbo].[ufnLeadingZeros]([CustomerID]),'')),
    [rowguid]       UNIQUEIDENTIFIER ROWGUIDCOL NOT NULL,
    [ModifiedDate]  DATETIME         NOT NULL
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [AK_Customer_rowguid]
    ON [Sales].[Customer]([rowguid] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [AK_Customer_AccountNumber]
    ON [Sales].[Customer]([AccountNumber] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Customer_TerritoryID]
    ON [Sales].[Customer]([TerritoryID] ASC);


GO
ALTER TABLE [Sales].[Customer]
    ADD CONSTRAINT [PK_Customer_CustomerID] PRIMARY KEY CLUSTERED ([CustomerID] ASC);


GO
ALTER TABLE [Sales].[Customer]
    ADD CONSTRAINT [FK_Customer_Person_PersonID] FOREIGN KEY ([PersonID]) REFERENCES [Person].[Person] ([BusinessEntityID]);


GO
ALTER TABLE [Sales].[Customer]
    ADD CONSTRAINT [FK_Customer_Store_StoreID] FOREIGN KEY ([StoreID]) REFERENCES [Sales].[Store] ([BusinessEntityID]);


GO
ALTER TABLE [Sales].[Customer]
    ADD CONSTRAINT [FK_Customer_SalesTerritory_TerritoryID] FOREIGN KEY ([TerritoryID]) REFERENCES [Sales].[SalesTerritory] ([TerritoryID]);


GO
ALTER TABLE [Sales].[Customer]
    ADD CONSTRAINT [DF_Customer_rowguid] DEFAULT (newid()) FOR [rowguid];


GO
ALTER TABLE [Sales].[Customer]
    ADD CONSTRAINT [DF_Customer_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate];


