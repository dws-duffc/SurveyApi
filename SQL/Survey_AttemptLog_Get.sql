USE [TheDatabase];
GO
/****** Object:  StoredProcedure [dbo].[Survey_AttemptLog_Get]    Script Date: 4/4/2017 12:37:39 PM ******/

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_AttemptLog_Get')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_AttemptLog_Get;
END;
GO
/****** Object:  StoredProcedure [dbo].[Survey_AttemptLog_Get]    Script Date: 4/4/2017 12:37:39 PM ******/

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO
/****** Object:  StoredProcedure [dbo].[Survey_AttemptLog_Get]    Script Date: 07/15/2015 13:47:11 ******/

CREATE PROCEDURE dbo.Survey_AttemptLog_Get(
       @p_AttemptLogId BIGINT)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [PeriodId],
           [period].[startdate]      [StartDate],
           [period].[enddate]        [EndDate],
           [period].[isopen]         [IsOpen],
           [CalleeID]                [AgentId],
           [agent].[agencycode]      [AgencyCode],
           [agent].[agencyname]      [AgencyName],
           CASE
               WHEN [agent].[agencystatus] = 'Can' THEN 0
               ELSE 1
           END                       [IsActiveAgent],
           [CallerID]                [RepId],
           [Rep].[username]     [Username],
           [usr].[usr_fname]         [FirstName],
           [usr].[usr_lname]         [LastName],
           [Rep].[isactive]     [IsActive],
           [attempt].[attemptedby]   [AttemptedBy],
           [attempt].[attempteddate] [AttemptedDate]
    FROM TheDatabase.dbo.tb_Survey_AttemptLog attempt
    JOIN tb_Survey_Period period
         ON period.pk = attempt.periodid
    JOIN tb_Survey_Callee callee
         ON callee.pk = attempt.calleeid
    JOIN tb_Survey_Caller Rep
         ON Rep.pk = attempt.callerid
    LEFT OUTER JOIN Agency agent
         ON agent.agencycode = callee.agencycode
    LEFT OUTER JOIN ps.dbo.ttsusers usr
         ON usr.usr_userid = Rep.username
    WHERE [attempt].[pk] = @p_AttemptLogId
          OR @p_AttemptLogId IS NULL;
END;

GO

GRANT EXECUTE ON dbo.Survey_AttemptLog_Get TO [public] AS [dbo];
GO