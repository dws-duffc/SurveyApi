USE [TheDatabase];
GO
/****** Object:  StoredProcedure [dbo].[Survey_Rep_Get]    Script Date: 4/4/2017 12:37:39 PM ******/

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Rep_Get')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Rep_Get;
END;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Rep_Get]    Script Date: 4/4/2017 12:37:39 PM ******/

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Rep_Get]    Script Date: 07/15/2015 13:47:11 ******/

CREATE PROCEDURE dbo.Survey_Rep_Get(
       @p_RepId INT)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Rep].[PK]       [RepId],
           [Rep].[Username] [Username],
           [names].[usr_fname]   [FirstName],
           [names].[usr_lname]   [LastName],
           [IsActive]
    FROM dbo.tb_Survey_Caller Rep
    LEFT OUTER JOIN PS.dbo.ttsusers names
         ON names.usr_userid = Rep.username
    WHERE [Rep].[pk] = @p_RepId
          OR @p_RepId IS NULL
    ORDER BY [names].[usr_lname];
END;

GO

GRANT EXECUTE ON dbo.Survey_Rep_Get TO [public] AS [dbo];
GO