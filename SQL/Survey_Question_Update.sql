USE [TheDatabase];
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Question_Update')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Question_Update;
END;
GO

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE PROCEDURE dbo.Survey_Question_Update(
       @p_QuestionId     INT,
       @p_QuestionTypeId TINYINT,
       @p_PeriodId       SMALLINT,
       @p_Text           VARCHAR(8000),
       @p_SortOrder      INT,
       @p_RowCount       INT OUTPUT)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.tb_Survey_Question
      SET [Type] = @p_QuestionTypeId,
          [periodID] = @p_PeriodId,
          [Text] = @p_Text,
          [Sort] = @p_SortOrder
    WHERE [PK] = @p_QuestionId;

    SELECT @p_RowCount = @@ROWCOUNT;
END;

GO

GRANT EXECUTE ON dbo.Survey_Question_Update TO [public] AS [dbo];
GO