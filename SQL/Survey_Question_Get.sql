USE [TheDatabase];
GO
/****** Object:  StoredProcedure [dbo].[Survey_Question_Get]    Script Date: 4/4/2017 12:37:39 PM ******/

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Question_Get')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Question_Get;
END;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Question_Get]    Script Date: 4/4/2017 12:37:39 PM ******/

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Question_Get]    Script Date: 07/15/2015 13:47:11 ******/

CREATE PROCEDURE dbo.Survey_Question_Get(
       @p_QuestionId INT)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [quest].[PK]   [QuestionId],
           [quest].[Type] [QuestionTypeId],
           [quest].[PeriodId],
           [period].[StartDate],
           [period].[EndDate],
           [period].[IsOpen],
           [quest].[Text] [QuestionText],
           [quest].[Sort] [QuestionSort],
           [qtype].[Description],
           [qtype].[HasAnswers]
    FROM dbo.tb_Survey_Question quest
    LEFT OUTER JOIN dbo.tb_Survey_QuestionType qtype
         ON quest.[Type] = qtype.PK
    LEFT OUTER JOIN dbo.tb_Survey_Period period
         ON quest.PeriodId = period.PK
    WHERE [quest].[pk] = @p_QuestionId
          OR @p_QuestionId IS NULL
    ORDER BY [quest].[PeriodId] DESC, 
		   [quest].[Sort] ASC;
END;

GO

GRANT EXECUTE ON dbo.Survey_Question_Get TO [public] AS [dbo];
GO