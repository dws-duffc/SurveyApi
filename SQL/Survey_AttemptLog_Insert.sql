USE [TheDatabase];
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_AttemptLog_Insert')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_AttemptLog_Insert;
END;
GO

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE PROCEDURE dbo.Survey_AttemptLog_Insert(
       @p_PeriodId      SMALLINT,
       @p_AgentId       INT,
       @p_RepId    INT,
       @p_AttemptedDate DATETIME,
       @p_AttemptedBy   VARCHAR(200),
       @p_AttemptLogId  INT OUTPUT)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.tb_Survey_AttemptLog
           ([PeriodID],
            [CalleeID],
            [CallerID],
            [AttemptedDate],
            [AttemptedBy])
    VALUES
           (@p_PeriodId,
            @p_AgentId,
            @p_RepId,
            @p_AttemptedDate,
            @p_AttemptedBy);

    SELECT @p_AttemptLogId = @@IDENTITY;
END;

GO

GRANT EXECUTE ON dbo.Survey_AttemptLog_Insert TO [public] AS [dbo];
GO