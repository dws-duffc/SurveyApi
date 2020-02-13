USE [TheDatabase];
GO
/****** Object:  StoredProcedure [dbo].[Survey_Rep_Search]    Script Date: 4/15/2017 7:11:01 PM ******/

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Rep_Search')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Rep_Search;
END;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Rep_Search]    Script Date: 4/15/2017 7:11:01 PM ******/

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Rep_Search]    Script Date: 07/15/2015 13:47:11 ******/

CREATE PROCEDURE dbo.Survey_Rep_Search(
       @p_Username  VARCHAR(202),
       @p_FirstName VARCHAR(12),
       @p_LastName  VARCHAR(22),
       @p_IsActive  BIT)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @SQL_WHERE NVARCHAR(4000);
    DECLARE @SQL_ORDERBY NVARCHAR(4000);
    DECLARE @SQL NVARCHAR(4000);
    DECLARE @SQL_PARAMS NVARCHAR(2000);

    SET @SQL_PARAMS =
    N'@Username VARCHAR(202),
	   @FirstName VARCHAR(12),
	   @LastName VARCHAR(22),
	   @IsActive BIT';

    SET @SQL =
    'SET NOCOUNT ON;
SELECT [Rep].[PK]       [RepId],
       [Rep].[Username] [Username],
       [names].[usr_fname]   [FirstName],
       [names].[usr_lname]   [LastName],
       [IsActive]
FROM dbo.tb_Survey_Caller Rep
LEFT OUTER JOIN PS.dbo.ttsusers names
     ON names.usr_userid = Rep.username';

    SET @SQL_WHERE = '
WHERE 1 = 1';

    IF @p_Username IS NOT NULL
    BEGIN
        SET @p_Username = CONCAT('%', @p_Username, '%');
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [Rep].[Username] LIKE @Username';
    END;

    IF @p_FirstName IS NOT NULL
    BEGIN
        SET @p_FirstName = CONCAT('%', @p_FirstName, '%');
        SET @SQL_WHERE = @SQL_WHERE+'
      AND names.[usr_fname] LIKE @FirstName';
    END;

    IF @p_LastName IS NOT NULL
    BEGIN
        SET @p_LastName = CONCAT('%', @p_LastName, '%');
        SET @SQL_WHERE = @SQL_WHERE+'
      AND names.[usr_lname] LIKE @LastName';
    END;

    IF @p_IsActive IS NOT NULL
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [Rep].[IsActive] = @IsActive';
    END;

    SET @SQL_ORDERBY = '
ORDER BY names.[usr_lname] DESC, [Rep].[IsActive] DESC;';

    SET @SQL = @SQL
               + @SQL_WHERE
               + @SQL_ORDERBY;

    EXEC sp_executesql @SQL,
                       @SQL_PARAMS,
                       @Username = @p_Username,
                       @FirstName = @p_FirstName,
                       @LastName = @p_LastName,
                       @IsActive = @p_IsActive;
END;

GO

GRANT EXECUTE ON dbo.Survey_Rep_Search TO [public] AS [dbo];
GO