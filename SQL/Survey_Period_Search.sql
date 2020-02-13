USE [TheDatabase];
GO
/****** Object:  StoredProcedure [dbo].[Survey_Period_Search]    Script Date: 4/15/2017 7:11:01 PM ******/

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Period_Search')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Period_Search;
END;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Period_Search]    Script Date: 4/15/2017 7:11:01 PM ******/

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Period_Search]    Script Date: 07/15/2015 13:47:11 ******/

CREATE PROCEDURE dbo.Survey_Period_Search(
       @p_PeriodId   SMALLINT,
       @p_PeriodDate DATE,
       @p_IsOpen     BIT)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @SQL_WHERE NVARCHAR(4000);
    DECLARE @SQL_ORDERBY NVARCHAR(4000);
    DECLARE @SQL NVARCHAR(4000);
    DECLARE @SQL_PARAMS NVARCHAR(2000);

    SET @SQL_PARAMS = N'@PeriodId SMALLINT,@PeriodDate DATE,@IsOpen BIT';

    SET @SQL =
    'SET NOCOUNT ON;
SELECT [pk]                 [PeriodId],
       [period].[startdate] [StartDate],
       [period].[enddate]   [EndDate],
       [period].[isopen]    [IsOpen]
FROM tb_Survey_Period period';

    SET @SQL_WHERE = '
WHERE 1 = 1';

    IF isnull(@p_PeriodId, 0) > 0
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [period].[pk] = @PeriodId';
    END;

    IF @p_PeriodDate IS NOT NULL
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+'
      AND @PeriodDate BETWEEN [period].[startDate] AND [period].[endDate]';
    END;

    IF @p_IsOpen IS NOT NULL
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [period].[isopen] = @IsOpen';
    END;

    SET @SQL_ORDERBY = '
ORDER BY [period].[pk] DESC;';

    SET @SQL = @SQL
               + @SQL_WHERE
               + @SQL_ORDERBY;

    EXEC sp_executesql @SQL,
                       @SQL_PARAMS,
                       @PeriodId = @p_PeriodId,
                       @PeriodDate = @p_PeriodDate,
                       @IsOpen = @p_IsOpen;
END;

GO

GRANT EXECUTE ON dbo.Survey_Period_Search TO [public] AS [dbo];
GO