USE [TheDatabase];
GO
/****** Object:  StoredProcedure [dbo].[Survey_Answer_Get]    Script Date: 4/4/2017 12:37:39 PM ******/

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Answer_Get')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Answer_Get;
END;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Answer_Get]    Script Date: 4/4/2017 12:37:39 PM ******/

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Answer_Get]    Script Date: 07/15/2015 13:47:11 ******/

CREATE PROCEDURE dbo.Survey_Answer_Get(
       @p_AnswerId BIGINT)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [ans].[PK]         [AnswerId],
           [ans].[Text]       [AnswerText],
           [ans].[Sort]       [AnswerSort],
           [ans].[QuestionId] [QuestionId],
           [quest].[Type]     [QuestionTypeId],
           [quest].[PeriodId],
           [period].[StartDate],
           [period].[EndDate],
           [period].[IsOpen],
           [quest].[Text]     [QuestionText],
           [quest].[Sort]     [QuestionSort],
           [qtype].[Description],
           [qtype].[HasAnswers]
    FROM dbo.tb_Survey_Answer ans
    LEFT OUTER JOIN dbo.tb_Survey_Question quest
         ON ans.QuestionID = quest.PK
    LEFT OUTER JOIN dbo.tb_Survey_QuestionType qtype
         ON quest.[Type] = qtype.PK
    LEFT OUTER JOIN dbo.tb_Survey_Period period
         ON quest.PeriodId = period.PK
    WHERE [ans].[pk] = @p_AnswerId
          OR @p_AnswerId IS NULL
    ORDER BY [quest].[PeriodId] DESC, 
		   [quest].[Sort] ASC, 
		   [ans].[Sort] ASC;
END;

GO

GRANT EXECUTE ON dbo.Survey_Answer_Get TO [public] AS [dbo];
GO