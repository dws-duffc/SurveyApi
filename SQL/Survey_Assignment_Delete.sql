USE [TheDatabase];
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Assignment_Delete')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Assignment_Delete;
END;
GO

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE PROCEDURE dbo.Survey_Assignment_Delete(
       @p_PeriodId   SMALLINT,
       @p_AgentId    INT,
       @p_RepId INT,
       @p_RowCount   INT OUTPUT)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS(SELECT 1
              FROM tb_Survey_Assignment assignment
              WHERE [assignment].[PeriodID] = @p_PeriodId
                    AND [assignment].[CalleeID] = @p_AgentId
                    AND [assignment].[CallerID] = @p_RepId)
    BEGIN
        DELETE FROM tb_Survey_Assignment
        WHERE [PeriodID] = @p_PeriodId
              AND [CalleeID] = @p_AgentId
              AND [CallerID] = @p_RepId;
    END;

    SELECT @p_RowCount = @@ROWCOUNT;
END;


GO

GRANT EXECUTE ON dbo.Survey_Assignment_Delete TO [public] AS [dbo];
GO