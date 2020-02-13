USE [TheDatabase];
GO
/****** Object:  Table [dbo].[Agency]    Script Date: 4/9/2017 11:41:43 AM ******/

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

SET ANSI_PADDING ON;
GO

CREATE TABLE dbo.Agency(
             [AgencyCode] NVARCHAR(8) NOT NULL,
             [AgencyName] NVARCHAR(100) NULL,
             CONSTRAINT PK_Agency PRIMARY KEY CLUSTERED([AgencyCode] ASC)
             WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]);

GO

SET ANSI_PADDING OFF;
GO

GRANT SELECT ON dbo.Agency TO public AS [dbo];
GO

GRANT DELETE ON dbo.Agency TO public AS [dbo];
GO

GRANT INSERT ON dbo.Agency TO public AS [dbo];
GO

GRANT UPDATE ON dbo.Agency TO public AS [dbo];
GO


INSERT INTO dbo.Agency
       ([AgencyCode],
        [AgencyName])
VALUES
       ('0000',
        'Loop Users'),
       ('0004',
        'ABC Agency'),
       ('0008',
        'Best Darn Agency');