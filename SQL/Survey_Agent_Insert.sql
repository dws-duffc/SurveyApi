USE [TheDatabase];
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Agent_Insert')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Agent_Insert;
END;
GO

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE PROCEDURE dbo.Survey_Agent_Insert(
       @p_AgentCode NVARCHAR(8),
       @p_AgentId   INT OUTPUT)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.tb_Survey_Callee([AgencyCode])
    VALUES(@p_AgentCode);

    SELECT @p_AgentId = @@IDENTITY;
END;

GO

GRANT EXECUTE ON dbo.Survey_Agent_Insert TO [public] AS [dbo];
GO