USE [TheDatabase];
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Response_Insert')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Response_Insert;
END;
GO

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE PROCEDURE dbo.Survey_Response_Insert(
       @p_AgentId    INT,
       @p_AnswerId   BIGINT,
       @p_Text       VARCHAR(8000),
       @p_ResponseId BIGINT OUTPUT)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO tb_Survey_Response
           ([CalleeID],
            [AnswerID],
            [Text])
    VALUES
           (@p_AgentId,
            @p_AnswerId,
            @p_Text);

    SELECT @p_ResponseId = @@IDENTITY;
END;

GO

GRANT EXECUTE ON dbo.Survey_Response_Insert TO [public] AS [dbo];
GO