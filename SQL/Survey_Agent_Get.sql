USE [TheDatabase];
GO
/****** Object:  StoredProcedure [dbo].[Survey_Agent_Get]    Script Date: 4/4/2017 12:37:39 PM ******/

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Agent_Get')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Agent_Get;
END;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Agent_Get]    Script Date: 4/4/2017 12:37:39 PM ******/

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Agent_Get]    Script Date: 07/15/2015 13:47:11 ******/

CREATE PROCEDURE dbo.Survey_Agent_Get(
       @p_AgentId INT)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [agent].[PK]         [AgentId],
           [agent].[AgencyCode] [AgencyCode],
           [info].[AgencyName]  [AgencyName],
           CASE
               WHEN [info].[agencystatus] = 'Can' THEN 0
               ELSE 1
           END                  [IsActiveAgent]
    FROM dbo.tb_Survey_Callee agent
    LEFT OUTER JOIN dbo.Agency info
         ON info.AgencyCode = agent.AgencyCode
    WHERE [agent].[pk] = @p_AgentId
          OR @p_AgentId IS NULL;
END;

GO

GRANT EXECUTE ON dbo.Survey_Agent_Get TO [public] AS [dbo];
GO