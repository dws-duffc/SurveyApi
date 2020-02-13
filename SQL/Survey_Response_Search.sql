USE [TheDatabase];
GO
/****** Object:  StoredProcedure [dbo].[Survey_Response_Search]    Script Date: 4/15/2017 7:11:01 PM ******/

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Response_Search')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Response_Search;
END;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Response_Search]    Script Date: 4/15/2017 7:11:01 PM ******/

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Response_Search]    Script Date: 07/15/2015 13:47:11 ******/

CREATE PROCEDURE dbo.Survey_Response_Search(
       @p_ResponseId      BIGINT,
       @p_ResponseText    VARCHAR(8000),
       @p_AnswerId        BIGINT,
       @p_AnswerText      VARCHAR(8000),
       @p_AnswerSort      BIGINT,
       @p_QuestionId      INT,
       @p_QuestionType    TINYINT,
       @p_PeriodId        SMALLINT,
       @p_PeriodStartDate DATETIME,
       @p_PeriodEndDate   DATETIME,
       @p_PeriodIsOpen    BIT,
       @p_QuestionText    VARCHAR(8000),
       @p_QuestionSort    INT,
       @p_QuestionDesc    VARCHAR(8000),
       @p_HasAnswers      BIT,
       @p_AgentId         INT,
       @p_AgencyCode      NVARCHAR(10),
       @p_AgencyName      NVARCHAR(102),
       @p_IsActiveAgent   BIT)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @SQL_WHERE NVARCHAR(4000);
    DECLARE @SQL_ORDERBY NVARCHAR(4000);
    DECLARE @SQL NVARCHAR(4000);
    DECLARE @SQL_PARAMS NVARCHAR(2000);

    SET @SQL_PARAMS =
    N'@ResponseId BIGINT,
	  @ResponseText VARCHAR(8000),
	  @AnswerId BIGINT,
       @AnswerText VARCHAR(8000),
       @AnswerSort BIGINT,
       @QuestionId INT,
	  @QuestionType TINYINT,
       @PeriodId SMALLINT,
       @PeriodStartDate DATETIME,
       @PeriodEndDate DATETIME,
	  @PeriodIsOpen BIT,
	  @QuestionText VARCHAR(8000),
	  @QuestionSort INT,
	  @QuestionDesc VARCHAR(8000),
	  @HasAnswers BIT,
	  @AgentId INT,
       @AgencyCode NVARCHAR(10),
       @AgencyName NVARCHAR(102)';

    SET @SQL =
    'SET NOCOUNT ON;
SELECT [resp].[PK]         [ResponseId],
       [resp].[Text]       [ResponseText],
       [resp].[AnswerId]   [AnswerId],
       [ans].[Text]        [AnswerText],
       [ans].[Sort]        [AnswerSort],
       [ans].[QuestionId]  [QuestionId],
       [quest].[Type]      [QuestionTypeId],
       [quest].[PeriodId],
       [period].[StartDate],
       [period].[EndDate],
       [period].[IsOpen],
       [quest].[Text]      [QuestionText],
       [quest].[Sort]      [QuestionSort],
       [qtype].[Description],
       [qtype].[HasAnswers],
       [resp].[CalleeId]   [AgentId],
       [agent].[AgencyCode],
       [info].[AgencyName],
       CASE
           WHEN [info].[agencystatus] = ''Can'' THEN 0
           ELSE 1
       END                 [IsActiveAgent]
FROM dbo.tb_Survey_Response resp
LEFT OUTER JOIN dbo.tb_Survey_Answer ans
     ON resp.AnswerID = ans.PK
LEFT OUTER JOIN dbo.tb_Survey_Question quest
     ON ans.QuestionID = quest.PK
LEFT OUTER JOIN dbo.tb_Survey_QuestionType qtype
     ON quest.[Type] = qtype.PK
LEFT OUTER JOIN dbo.tb_Survey_Period period
     ON quest.PeriodId = period.PK
LEFT OUTER JOIN dbo.tb_Survey_Callee agent
     ON resp.CalleeID = agent.PK
LEFT OUTER JOIN dbo.Agency info
     ON info.AgencyCode = agent.AgencyCode';

    SET @SQL_WHERE = '
WHERE 1 = 1';

    IF isnull(@p_ResponseId, 0) > 0
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [resp].[PK] = @ResponseId';
    END;

    IF @p_ResponseText IS NOT NULL
    BEGIN
        SET @p_ResponseText = CONCAT('%', @p_ResponseText, '%');
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [resp].[Text] LIKE @ResponseText';
    END;

    IF isnull(@p_AnswerId, 0) > 0
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [ans].[PK] = @AnswerId';
    END;

    IF @p_AnswerText IS NOT NULL
    BEGIN
        SET @p_AnswerText = CONCAT('%', @p_AnswerText, '%');
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [ans].[Text] LIKE @AnswerText';
    END;

    IF isnull(@p_AnswerSort, 0) > 0
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [ans].[Sort] = @AnswerSort';
    END;

    IF isnull(@p_QuestionId, 0) > 0
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [ans].[QuestionId] = @QuestionId';
    END;

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

    IF isnull(@p_AgentId, 0) > 0
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [resp].[CalleeID] = @AgentId';
    END;

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
        SET @SQL_WHERE = @SQL_WHERE+CASE
                                        WHEN @p_IsActiveAgent = 1 THEN '
      AND [info].[agencystatus] = ''act'''
                                        ELSE '
      AND [info].[agencystatus] = ''can'''
                                    END;
    END;

    SET @SQL_ORDERBY = '
ORDER BY [quest].[PeriodId] DESC, 
	    [quest].[Sort] ASC, 
	    [ans].[Sort] ASC;';

    SET @SQL = @SQL
               + @SQL_WHERE
               + @SQL_ORDERBY;

    EXEC sp_executesql @SQL,
                       @SQL_PARAMS,
                       @ResponseId = @p_ResponseId,
                       @ResponseText = @p_ResponseText,
                       @AnswerId = @p_AnswerId,
                       @AnswerText = @p_AnswerText,
                       @AnswerSort = @p_AnswerSort,
                       @QuestionId = @p_QuestionId,
                       @QuestionType = @p_QuestionType,
                       @PeriodId = @p_PeriodId,
                       @PeriodStartDate = @p_PeriodStartDate,
                       @PeriodEndDate = @p_PeriodEndDate,
                       @PeriodIsOpen = @p_PeriodIsOpen,
                       @QuestionText = @p_QuestionText,
                       @QuestionSort = @p_QuestionSort,
                       @QuestionDesc = @p_QuestionDesc,
                       @HasAnswers = @p_HasAnswers,
                       @AgentId = @p_AgentId,
                       @AgencyCode = @p_AgencyCode,
                       @AgencyName = @p_AgencyName;
END;

GO

GRANT EXECUTE ON dbo.Survey_Response_Search TO [public] AS [dbo];
GO