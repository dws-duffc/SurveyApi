USE [TheDatabase];
GO
/****** Object:  StoredProcedure [dbo].[Survey_Question_Search]    Script Date: 4/15/2017 7:11:01 PM ******/

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Question_Search')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Question_Search;
END;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Question_Search]    Script Date: 4/15/2017 7:11:01 PM ******/

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Question_Search]    Script Date: 07/15/2015 13:47:11 ******/

CREATE PROCEDURE dbo.Survey_Question_Search(
       @p_QuestionType    TINYINT,
       @p_PeriodId        SMALLINT,
       @p_PeriodStartDate DATETIME,
       @p_PeriodEndDate   DATETIME,
       @p_PeriodIsOpen    BIT,
       @p_QuestionText    VARCHAR(8000),
	  @p_QuestionSort	 INT,
       @p_QuestionDesc    VARCHAR(8000),
       @p_HasAnswers      BIT)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @SQL_WHERE NVARCHAR(4000);
    DECLARE @SQL_ORDERBY NVARCHAR(4000);
    DECLARE @SQL NVARCHAR(4000);
    DECLARE @SQL_PARAMS NVARCHAR(2000);

    SET @SQL_PARAMS =
    N'@QuestionType TINYINT,
       @PeriodId SMALLINT,
       @PeriodStartDate DATETIME,
       @PeriodEndDate DATETIME,
	  @PeriodIsOpen BIT,
	  @QuestionText VARCHAR(8000),
	  @QuestionSort INT,
	  @QuestionDesc VARCHAR(8000),
	  @HasAnswers BIT';

    SET @SQL = 'SET NOCOUNT ON;
SELECT [quest].[PK]   [QuestionId],
       [quest].[Type] [QuestionTypeId],
       [quest].[PeriodId],
       [period].[StartDate],
       [period].[EndDate],
       [period].[IsOpen],
       [quest].[Text] [QuestionText],
       [quest].[Sort] [QuestionSort],
       [qtype].[Description],
       [HasAnswers]
FROM dbo.tb_Survey_Question quest
LEFT OUTER JOIN dbo.tb_Survey_QuestionType qtype
     ON quest.[Type] = qtype.PK
LEFT OUTER JOIN dbo.tb_Survey_Period period
     ON quest.PeriodId = period.PK';

    SET @SQL_WHERE = '
WHERE 1 = 1';

    IF isnull(@p_QuestionType, 0) > 0
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [quest].[Type] = @QuestionType';
    END;

    IF isnull(@p_PeriodId, 0) > 0
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+'
      AND period.PK = @PeriodId';
    END;

    IF @p_PeriodStartDate IS NOT NULL
    BEGIN
        SET @p_PeriodStartDate = CAST(@p_PeriodStartDate AS DATE);
        SET @SQL_WHERE = @SQL_WHERE+'
      AND CAST([period].[StartDate] AS DATE) BETWEEN @PeriodStartDate and DATEADD(day, 1, @PeriodStartDate)';
    END;
    
    IF @p_PeriodEndDate IS NOT NULL
    BEGIN
        SET @p_PeriodEndDate = CAST(@p_PeriodEndDate AS DATE);
        SET @SQL_WHERE = @SQL_WHERE+'
      AND CAST([period].[EndDate] AS DATE) BETWEEN @PeriodEndDate and DATEADD(day, 1, @PeriodEndDate)';
    END;

    IF @p_PeriodIsOpen IS NOT NULL
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [period].[IsOpen] = @PeriodIsOpen';
    END;

    IF @p_QuestionText IS NOT NULL
    BEGIN
        SET @p_QuestionText = CONCAT('%', @p_QuestionText, '%');
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [quest].[Text] LIKE @QuestionText';
    END;

    IF isnull(@p_QuestionSort, 0) > 0
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [quest].[Sort] = @QuestionSort';
    END;

    IF @p_QuestionDesc IS NOT NULL
    BEGIN
        SET @p_QuestionDesc = CONCAT('%', @p_QuestionDesc, '%');
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [qtype].[Description] LIKE @QuestionDesc';
    END;

    IF @p_HasAnswers IS NOT NULL
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [qtype].[HasAnswers] = @HasAnswers';
    END;

    SET @SQL_ORDERBY = '
ORDER BY [quest].[PeriodId] DESC, [quest].[Sort] ASC;';

    SET @SQL = @SQL
               + @SQL_WHERE
               + @SQL_ORDERBY;

    EXEC sp_executesql @SQL,
                       @SQL_PARAMS,
                       @QuestionType = @p_QuestionType,
                       @PeriodId = @p_PeriodId,
                       @PeriodStartDate = @p_PeriodStartDate,
                       @PeriodEndDate = @p_PeriodEndDate,
                       @PeriodIsOpen = @p_PeriodIsOpen,
                       @QuestionText = @p_QuestionText,
				   @QuestionSort = @p_QuestionSort,
                       @QuestionDesc = @p_QuestionDesc,
                       @HasAnswers = @p_HasAnswers;
END;

GO

GRANT EXECUTE ON dbo.Survey_Question_Search TO [public] AS [dbo];
GO