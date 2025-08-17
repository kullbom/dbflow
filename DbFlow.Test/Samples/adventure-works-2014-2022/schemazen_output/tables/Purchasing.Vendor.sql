CREATE TABLE [Purchasing].[Vendor] (
   [BusinessEntityID] [int] NOT NULL,
   [AccountNumber] [nvarchar](15) NOT NULL,
   [Name] [nvarchar](50) NOT NULL,
   [CreditRating] [tinyint] NOT NULL,
   [PreferredVendorStatus] [bit] NOT NULL,
   [ActiveFlag] [bit] NOT NULL,
   [PurchasingWebServiceURL] [nvarchar](1024) NULL,
   [ModifiedDate] [datetime] NOT NULL

   ,CONSTRAINT [PK_Vendor_BusinessEntityID] PRIMARY KEY CLUSTERED ([BusinessEntityID])
)

CREATE UNIQUE NONCLUSTERED INDEX [AK_Vendor_AccountNumber] ON [Purchasing].[Vendor] ([AccountNumber])

GO
