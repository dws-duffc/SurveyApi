USE [TheDatabase];
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Assignment_Update')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Assignment_Update;
END;
GO

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE PROCEDURE dbo.Survey_Assignment_Update(
       @p_PeriodId          SMALLINT,
       @p_AgentId           INT,
       @p_RepId        INT,
       @p_Status            VARCHAR(200),
       @p_Notes             VARCHAR(8000),
       @p_LastAttemptedBy   VARCHAR(200),
       @p_LastAttemptedDate DATETIME,
       @p_AttemptCount      TINYINT,
       @p_RowCount          INT OUTPUT)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE tb_Survey_Assignment
      SET [Status] = @p_Status,
          [Notes] = @p_Notes,
          [LastAttemptedBy] = @p_LastAttemptedBy,
          [LastAttemptedDate] = @p_LastAttemptedDate,
          [AttemptCount] = @p_AttemptCount
    WHERE [PeriodID] = @p_PeriodId
          AND [CalleeID] = @p_AgentId
          AND [CallerID] = @p_RepId;

    SELECT @p_RowCount = @@ROWCOUNT;
END;


GO

GRANT EXECUTE ON dbo.Survey_Assignment_Update TO [public] AS [dbo];
GO