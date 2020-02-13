USE [TheDatabase];
GO
/****** Object:  StoredProcedure [dbo].[Survey_Agent_Search]    Script Date: 4/15/2017 7:11:01 PM ******/

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Agent_Search')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Agent_Search;
END;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Agent_Search]    Script Date: 4/15/2017 7:11:01 PM ******/

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Agent_Search]    Script Date: 07/15/2015 13:47:11 ******/

CREATE PROCEDURE dbo.Survey_Agent_Search(
       @p_AgencyCode    NVARCHAR(10),
       @p_AgencyName    NVARCHAR(102),
       @p_IsActiveAgent BIT)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @SQL_WHERE NVARCHAR(4000);
    DECLARE @SQL_ORDERBY NVARCHAR(4000);
    DECLARE @SQL NVARCHAR(4000);
    DECLARE @SQL_PARAMS NVARCHAR(2000);

    SET @SQL_PARAMS = N'@AgencyCode VARCHAR(10),
	   @AgencyName VARCHAR(102)';

    SET @SQL =
    'SET NOCOUNT ON;
SELECT [agent].[PK]         [AgentId],
       [agent].[AgencyCode] [AgencyCode],
       [info].[AgencyName]  [AgencyName],
       CASE
           WHEN [info].[agencystatus] = ''Can'' THEN 0
           ELSE 1
       END                  [IsActiveAgent]
FROM dbo.tb_Survey_Callee agent
LEFT OUTER JOIN dbo.Agency info
     ON info.AgencyCode = agent.AgencyCode';

    SET @SQL_WHERE = '
WHERE 1 = 1';

    IF @p_AgencyCode IS NOT NULL
    BEGIN
        SET @p_AgencyCode = CONCAT('%', @p_AgencyCode, '%');
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [agent].[AgencyCode] LIKE @AgencyCode';
    END;

    IF @p_AgencyName IS NOT NULL
    BEGIN
        SET @p_AgencyName = CONCAT('%', @p_AgencyName, '%');
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [info].[AgencyName] LIKE @AgencyName';
    END;

    IF @p_IsActiveAgent IS NOT NULL
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+CASE WHEN @p_IsActiveAgent = 1 THEN '
      AND [info].[agencystatus] = ''act'''
                                        ELSE '
      AND [info].[agencystatus] = ''can'''
                                    END;
    END;

    SET @SQL_ORDERBY = '
ORDER BY agent.[AgencyCode] ASC;';

    SET @SQL = @SQL
               + @SQL_WHERE
               + @SQL_ORDERBY;

    EXEC sp_executesql @SQL,
                       @SQL_PARAMS,
                       @AgencyCode = @p_AgencyCode,
                       @AgencyName = @p_AgencyName;
END;

GO

GRANT EXECUTE ON dbo.Survey_Agent_Search TO [public] AS [dbo];
GO