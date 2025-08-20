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
