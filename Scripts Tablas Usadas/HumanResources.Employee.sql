USE [AdventureWorks2014]
GO

/****** Object: Table [HumanResources].[Employee] Script Date: 2/10/2025 12:52:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [HumanResources].[Employee] (
    [BusinessEntityID]  INT                 NOT NULL,
    [NationalIDNumber]  NVARCHAR (15)       NOT NULL,
    [LoginID]           NVARCHAR (256)      NOT NULL,
    [OrganizationNode]  [sys].[hierarchyid] NULL,
    [OrganizationLevel] AS                  ([OrganizationNode].[GetLevel]()),
    [JobTitle]          NVARCHAR (50)       NOT NULL,
    [BirthDate]         DATE                NOT NULL,
    [MaritalStatus]     NCHAR (1)           NOT NULL,
    [Gender]            NCHAR (1)           NOT NULL,
    [HireDate]          DATE                NOT NULL,
    [SalariedFlag]      [dbo].[Flag]        NOT NULL,
    [VacationHours]     SMALLINT            NOT NULL,
    [SickLeaveHours]    SMALLINT            NOT NULL,
    [CurrentFlag]       [dbo].[Flag]        NOT NULL,
    [rowguid]           UNIQUEIDENTIFIER    ROWGUIDCOL NOT NULL,
    [ModifiedDate]      DATETIME            NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_Employee_OrganizationNode]
    ON [HumanResources].[Employee]([OrganizationNode] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Employee_OrganizationLevel_OrganizationNode]
    ON [HumanResources].[Employee]([OrganizationLevel] ASC, [OrganizationNode] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [AK_Employee_LoginID]
    ON [HumanResources].[Employee]([LoginID] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [AK_Employee_NationalIDNumber]
    ON [HumanResources].[Employee]([NationalIDNumber] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [AK_Employee_rowguid]
    ON [HumanResources].[Employee]([rowguid] ASC);


GO
ALTER TABLE [HumanResources].[Employee]
    ADD CONSTRAINT [PK_Employee_BusinessEntityID] PRIMARY KEY CLUSTERED ([BusinessEntityID] ASC);


GO
ALTER TABLE [HumanResources].[Employee]
    ADD CONSTRAINT [FK_Employee_Person_BusinessEntityID] FOREIGN KEY ([BusinessEntityID]) REFERENCES [Person].[Person] ([BusinessEntityID]);


GO
ALTER TABLE [HumanResources].[Employee]
    ADD CONSTRAINT [DF_Employee_SalariedFlag] DEFAULT ((1)) FOR [SalariedFlag];


GO
ALTER TABLE [HumanResources].[Employee]
    ADD CONSTRAINT [DF_Employee_VacationHours] DEFAULT ((0)) FOR [VacationHours];


GO
ALTER TABLE [HumanResources].[Employee]
    ADD CONSTRAINT [DF_Employee_SickLeaveHours] DEFAULT ((0)) FOR [SickLeaveHours];


GO
ALTER TABLE [HumanResources].[Employee]
    ADD CONSTRAINT [DF_Employee_CurrentFlag] DEFAULT ((1)) FOR [CurrentFlag];


GO
ALTER TABLE [HumanResources].[Employee]
    ADD CONSTRAINT [DF_Employee_rowguid] DEFAULT (newid()) FOR [rowguid];


GO
ALTER TABLE [HumanResources].[Employee]
    ADD CONSTRAINT [DF_Employee_ModifiedDate] DEFAULT (getdate()) FOR [ModifiedDate];


GO
ALTER TABLE [HumanResources].[Employee]
    ADD CONSTRAINT [CK_Employee_BirthDate] CHECK ([BirthDate]>='1930-01-01' AND [BirthDate]<=dateadd(year,(-18),getdate()));


GO
ALTER TABLE [HumanResources].[Employee]
    ADD CONSTRAINT [CK_Employee_MaritalStatus] CHECK (upper([MaritalStatus])='S' OR upper([MaritalStatus])='M');


GO
ALTER TABLE [HumanResources].[Employee]
    ADD CONSTRAINT [CK_Employee_HireDate] CHECK ([HireDate]>='1996-07-01' AND [HireDate]<=dateadd(day,(1),getdate()));


GO
ALTER TABLE [HumanResources].[Employee]
    ADD CONSTRAINT [CK_Employee_Gender] CHECK (upper([Gender])='F' OR upper([Gender])='M');


GO
ALTER TABLE [HumanResources].[Employee]
    ADD CONSTRAINT [CK_Employee_VacationHours] CHECK ([VacationHours]>=(-40) AND [VacationHours]<=(240));


GO
ALTER TABLE [HumanResources].[Employee]
    ADD CONSTRAINT [CK_Employee_SickLeaveHours] CHECK ([SickLeaveHours]>=(0) AND [SickLeaveHours]<=(120));


