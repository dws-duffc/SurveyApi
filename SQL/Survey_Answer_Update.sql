USE [TheDatabase];
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Answer_Update')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Answer_Update;
END;
GO

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE PROCEDURE dbo.Survey_Answer_Update(
       @p_AnswerId   BIGINT,
       @p_QuestionId INT,
       @p_Text       VARCHAR(8000),
       @p_SortOrder  BIGINT,
       @p_RowCount   INT OUTPUT)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.tb_Survey_Answer
      SET [QuestionID] = @p_QuestionId,
          [Text] = @p_Text,
          [Sort] = @p_SortOrder
    WHERE [PK] = @p_AnswerId;

    SELECT @p_RowCount = @@ROWCOUNT;
END;

GO

GRANT EXECUTE ON dbo.Survey_Answer_Update TO [public] AS [dbo];
GO