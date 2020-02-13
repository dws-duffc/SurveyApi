USE [TheDatabase];
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Assignment_Insert')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Assignment_Insert;
END;
GO

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE PROCEDURE dbo.Survey_Assignment_Insert(
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

    IF NOT EXISTS(SELECT 1
                  FROM tb_Survey_Assignment assignment
                  WHERE [assignment].[PeriodID] = @p_PeriodId
                        AND [assignment].[CalleeID] = @p_AgentId
                        AND [assignment].[CallerID] = @p_RepId)
    BEGIN
        INSERT INTO tb_Survey_Assignment
               ([PeriodID],
                [CalleeID],
                [CallerID],
                [Status],
                [Notes],
                [LastAttemptedBy],
                [LastAttemptedDate],
                [AttemptCount])
        VALUES
               (@p_PeriodId,
                @p_AgentId,
                @p_RepId,
                @p_Status,
                @p_Notes,
                @p_LastAttemptedBy,
                @p_LastAttemptedDate,
                @p_AttemptCount);
    END;

    SELECT @p_RowCount = @@ROWCOUNT;
END;


GO

GRANT EXECUTE ON dbo.Survey_Assignment_Insert TO [public] AS [dbo];
GO