USE [TheDatabase];
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Answer_Insert')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Answer_Insert;
END;
GO

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE PROCEDURE dbo.Survey_Answer_Insert(
       @p_QuestionId INT,
       @p_Text       VARCHAR(8000),
       @p_SortOrder  BIGINT,
       @p_AnswerId   BIGINT OUTPUT)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO tb_Survey_Answer
           ([QuestionID],
            [Text],
            [Sort])
    VALUES
           (@p_QuestionId,
            @p_Text,
            @p_SortOrder);

    SELECT @p_AnswerId = @@IDENTITY;
END;

GO

GRANT EXECUTE ON dbo.Survey_Answer_Insert TO [public] AS [dbo];
GO