USE [TheDatabase];
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Response_Update')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Response_Update;
END;
GO

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE PROCEDURE dbo.Survey_Response_Update(
       @p_ResponseId BIGINT,
       @p_AgentId    INT,
       @p_AnswerId   BIGINT,
       @p_Text       VARCHAR(8000),
       @p_RowCount   INT OUTPUT)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.tb_Survey_Response
      SET [CalleeId] = @p_AgentId,
          [AnswerId] = @p_AgentId,
          [Text] = @p_Text
    WHERE [PK] = @p_ResponseId;

    SELECT @p_RowCount = @@ROWCOUNT;
END;

GO

GRANT EXECUTE ON dbo.Survey_Response_Update TO [public] AS [dbo];
GO