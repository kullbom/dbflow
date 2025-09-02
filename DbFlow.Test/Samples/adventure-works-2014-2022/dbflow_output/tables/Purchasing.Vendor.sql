CREATE TABLE [Purchasing].[Vendor] (
   [BusinessEntityID] [INT] NOT NULL,
   [AccountNumber] [ACCOUNTNUMBER] NOT NULL,
   [Name] [NAME] NOT NULL,
   [CreditRating] [TINYINT] NOT NULL,
   [PreferredVendorStatus] [FLAG] NOT NULL,
   [ActiveFlag] [FLAG] NOT NULL,
   [PurchasingWebServiceURL] [NVARCHAR](1024) NULL,
   [ModifiedDate] [DATETIME] NOT NULL

   ,CONSTRAINT [PK_Vendor_BusinessEntityID] PRIMARY KEY CLUSTERED ([BusinessEntityID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_Vendor_AccountNumber] ON [Purchasing].[Vendor] ([AccountNumber])

GO

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Companies from whom Adventure Works Cycles purchases parts or other goods.', N'SCHEMA', [Purchasing], N'TABLE', [Vendor];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Primary key for Vendor records.  Foreign key to BusinessEntity.BusinessEntityID', N'SCHEMA', [Purchasing], N'TABLE', [Vendor], N'COLUMN', [BusinessEntityID];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Vendor account (identification) number.', N'SCHEMA', [Purchasing], N'TABLE', [Vendor], N'COLUMN', [AccountNumber];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'Company name.', N'SCHEMA', [Purchasing], N'TABLE', [Vendor], N'COLUMN', [Name];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'1 = Superior, 2 = Excellent, 3 = Above average, 4 = Average, 5 = Below average', N'SCHEMA', [Purchasing], N'TABLE', [Vendor], N'COLUMN', [CreditRating];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'0 = Do not use if another vendor is available. 1 = Preferred over other vendors supplying the same product.', N'SCHEMA', [Purchasing], N'TABLE', [Vendor], N'COLUMN', [PreferredVendorStatus];

EXECUTE [sys].[sp_addextendedproperty] N'MS_Description', N'0 = Vendor no longer used. 1 = Vendor is actively used.', N'SCHEMA', [Purchasing], N'TABLE', [Vendor], N'COLUMN', [ActiveFlag];
