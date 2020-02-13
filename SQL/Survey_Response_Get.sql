USE [TheDatabase];
GO
/****** Object:  StoredProcedure [dbo].[Survey_Response_Get]    Script Date: 4/4/2017 12:37:39 PM ******/

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Response_Get')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Response_Get;
END;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Response_Get]    Script Date: 4/4/2017 12:37:39 PM ******/

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Response_Get]    Script Date: 07/15/2015 13:47:11 ******/

CREATE PROCEDURE dbo.Survey_Response_Get(
       @p_ResponseId BIGINT)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [resp].[PK]         [ResponseId],
           [resp].[Text]       [ResponseText],
           [resp].[AnswerID]   [AnswerId],
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
           [resp].[CalleeID]   [AgentId],
           [agent].[AgencyCode],
           [info].[AgencyName],
           CASE
               WHEN [info].[agencystatus] = 'Can' THEN 0
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
         ON info.AgencyCode = agent.AgencyCode
    WHERE [ans].[pk] = @p_ResponseId
          OR @p_ResponseId IS NULL 
    ORDER BY [quest].[PeriodId] DESC,
          [quest].[Sort] ASC,
          [ans].[Sort] ASC;
END;

GO

GRANT EXECUTE ON dbo.Survey_Response_Get TO [public] AS [dbo];
GO