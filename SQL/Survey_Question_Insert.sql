USE [TheDatabase];
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Question_Insert')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Question_Insert;
END;
GO

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE PROCEDURE dbo.Survey_Question_Insert(
       @p_QuestionTypeId TINYINT,
       @p_PeriodId       SMALLINT,
       @p_Text           VARCHAR(8000),
       @p_SortOrder      INT,
       @p_QuestionId     INT OUTPUT)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO tb_Survey_Question
           ([Type],
            [periodID],
            [Text],
            [Sort])
    VALUES
           (@p_QuestionTypeId,
            @p_PeriodId,
            @p_Text,
            @p_SortOrder);

    SELECT @p_QuestionId = @@IDENTITY;
END;

GO

GRANT EXECUTE ON dbo.Survey_Question_Insert TO [public] AS [dbo];
GO