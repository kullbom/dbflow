

CREATE TABLE TestTable01 (
	Id INT NOT NULL IDENTITY (1,1) PRIMARY KEY, -- This is a test comment

	ColWithDefault INT NOT NULL DEFAULT ((1)),
	ColWithDefault2 INT NULL DEFAULT NULL,
	ColWithNamedDefault INT NOT NULL
)

ALTER TABLE [dbo].[TestTable01]  WITH NOCHECK ADD  CONSTRAINT [CK_TestTable01_PreventBad] CHECK  ([ColWithDefault2] <> 'Foobar')
GO

ALTER TABLE [dbo].[TestTable01] CHECK CONSTRAINT [CK_TestTable01_PreventBad]
GO

ALTER TABLE TestTable01 ADD CONSTRAINT [DF_TestTable01_ColWithNamedDefault] DEFAULT (((-1))) FOR [ColWithNamedDefault]


CREATE TABLE TestTable02 (
	RefId INT NOT NULL PRIMARY KEY,
	Id INT NOT NULL IDENTITY (1,1) UNIQUE,

	FirstName VARCHAR(100) MASKED WITH (FUNCTION = 'partial(1, "xxxxx", 1)') NULL,
    
)

SET ANSI_PADDING OFF

CREATE TABLE TestTablePadding (
	Id CHAR(7) NOT NULL UNIQUE,
    Content NVARCHAR(16)
)

SET ANSI_PADDING ON

CREATE INDEX IX_TestTablePadding_Content1 ON TestTablePadding (Content)
CREATE INDEX IX_TestTablePadding_Content2 ON TestTablePadding (Content)
ALTER INDEX IX_TestTablePadding_Content2 ON TestTablePadding DISABLE

-- TRIGGERS --------------------------------------------------------------------
GO

CREATE TRIGGER TR_TestTable02_Enabled ON TestTable02
    AFTER UPDATE AS
BEGIN
    DECLARE @Count int = (SELECT COUNT(1) FROM Inserted);
END

GO

CREATE TRIGGER TR_TestTable02_Disabled ON TestTable02
    AFTER UPDATE AS
BEGIN
    DECLARE @Count int = (SELECT COUNT(1) FROM Inserted);
END

GO

DISABLE TRIGGER TR_TestTable02_Disabled ON TestTable02

GO
-- INDEXES --------------------------------------------------------------------

CREATE NONCLUSTERED INDEX IX_TestTable02_RefId ON TestTable02 (RefId) INCLUDE (FirstName)

CREATE NONCLUSTERED INDEX IX_TestTable02_FirstName_56 ON TestTable02 (FirstName) WHERE FirstName = '56'

-- TYPES ----------------------------------------------------------------------

CREATE TYPE [dbo].[DateTime2Utc0] FROM datetime2(0) NULL
CREATE TYPE [dbo].[DateTime2Utc1] FROM datetime2(3) NULL


CREATE TYPE [dbo].[IntList] AS TABLE (
    Id INT NOT NULL IDENTITY (1,1) PRIMARY KEY,
    [Value] [int] NOT NULL DEFAULT ((1)) CHECK ([Value] > 0), -- The check constraint here disapears...

    CplxComputed AS [Value] * 5,

    ColWithNamedDefault INT NOT NULL DEFAULT (((-1)))
)


-- SYNONYMS -------------------------------------------------------------------

CREATE SYNONYM dbo.MyPrecious
    FOR TestTable02.FirstName;


-- SEQUENCES ------------------------------------------------------------------

CREATE SEQUENCE Seq01 AS INT
CREATE SEQUENCE Seq02 AS INT START WITH 4

CREATE SEQUENCE Seq03 AS SMALLINT START WITH 4 INCREMENT BY 2 

CREATE SEQUENCE Seq04 AS TINYINT START WITH 1 INCREMENT BY 1 CYCLE 
CREATE SEQUENCE Seq05 AS TINYINT START WITH 1 INCREMENT BY 1 NO CYCLE 

CREATE SEQUENCE Seq06 AS INT CACHE
CREATE SEQUENCE Seq07 AS NUMERIC(19,0) CACHE 6
CREATE SEQUENCE Seq08 AS INT NO CACHE

CREATE SEQUENCE Seq09 AS INT START WITH 4 MINVALUE 3
CREATE SEQUENCE Seq10 AS INT START WITH 4 NO MINVALUE
CREATE SEQUENCE Seq11 AS INT START WITH 9 MINVALUE 8 MAXVALUE 19
CREATE SEQUENCE Seq12 AS INT START WITH 4 NO MINVALUE MAXVALUE 700


-- XML SCHEMA COLLECTION ------------------------------------------------------

CREATE XML SCHEMA COLLECTION [dbo].[ProductDescriptionSchemaCollection] AS 
'<xsd:schema targetNamespace="http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ProductModelWarrAndMain"
    xmlns="http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ProductModelWarrAndMain" 
    elementFormDefault="qualified" 
    xmlns:xsd="http://www.w3.org/2001/XMLSchema" >
  
    <xsd:element name="Warranty"  >
        <xsd:complexType>
            <xsd:sequence>
                <xsd:element name="WarrantyPeriod" type="xsd:string"  />
                <xsd:element name="Description" type="xsd:string"  />
            </xsd:sequence>
        </xsd:complexType>
    </xsd:element>

    <xsd:element name="Maintenance"  >
        <xsd:complexType>
            <xsd:sequence>
                <xsd:element name="NoOfYears" type="xsd:string"  />
                <xsd:element name="Description" type="xsd:string"  />
            </xsd:sequence>
        </xsd:complexType>
    </xsd:element>
</xsd:schema>';


