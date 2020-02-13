USE [TheDatabase];
GO
/****** Object:  StoredProcedure [dbo].[Survey_Assignment_Get]    Script Date: 4/4/2017 12:37:39 PM ******/

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Assignment_Get')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Assignment_Get;
END;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Assignment_Get]    Script Date: 4/4/2017 12:37:39 PM ******/

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Assignment_Get]    Script Date: 07/15/2015 13:47:11 ******/

CREATE PROCEDURE dbo.Survey_Assignment_Get(
       @p_AgentId    INT,
       @p_RepId INT,
       @p_PeriodId   INT)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @SQL_SELECT NVARCHAR(4000);
    DECLARE @SQL_FROM NVARCHAR(4000);
    DECLARE @SQL_WHERE NVARCHAR(4000);
    DECLARE @SQL_ORDERBY NVARCHAR(4000);
    DECLARE @SQL NVARCHAR(4000);
    DECLARE @SQL_PARAMS NVARCHAR(2000);

    SET @SQL_PARAMS = N'@AgentId int, @RepId int, @PeriodId int';

    SET @SQL_SELECT =
    'SET NOCOUNT ON;
SELECT [PeriodId],
       [period].[startdate]             [StartDate],
       [period].[enddate]               [EndDate],
       [period].[isopen]                [IsOpen],
       [CalleeID]                       [AgentId],
       [agent].[agencycode]             [AgencyCode],
       [agent].[agencyname]             [AgencyName],
       CASE
           WHEN [agent].[agencystatus] = ''Can'' THEN 0
           ELSE 1
       END                              [IsActiveAgent],
       [CallerID]                       [RepId],
       [Rep].[username]            [Username],
       [usr].[usr_fname]                [FirstName],
       [usr].[usr_lname]                [LastName],
       [Rep].[isactive]            [IsActive],
       [assignment].[status]            [Status],
       [assignment].[attemptcount]      [AttemptCount],
       [assignment].[lastattemptedby]   [LastAttemptedBy],
       [assignment].[lastattempteddate] [LastAttemptedDate],
       [assignment].[notes]             [Notes]';

    SET @SQL_FROM =
    '
FROM tb_Survey_Assignment assignment
JOIN tb_Survey_Period period
     ON period.pk = assignment.periodid
JOIN tb_Survey_Callee callee
     ON callee.pk = assignment.calleeid
JOIN tb_Survey_Caller Rep
     ON Rep.pk = assignment.callerid
LEFT OUTER JOIN tb_Survey_AssignmentStatus asgnstatus
     ON asgnstatus.status = assignment.status
LEFT OUTER JOIN Agency agent
     ON agent.agencycode = callee.agencycode
LEFT OUTER JOIN ps.dbo.ttsusers usr
     ON usr.usr_userid = Rep.username';

    SET @SQL_WHERE = '
WHERE 1 = 1';

    IF @p_AgentId IS NOT NULL
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [callee].[pk] = @AgentId';
    END;
    IF @p_RepId IS NOT NULL
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [Rep].[pk] = @RepId';
    END;

    IF @p_PeriodId IS NOT NULL
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [period].[pk] = @PeriodId';
    END;

    SET @SQL_ORDERBY = '
ORDER BY [Rep].[username] ASC;';

    SET @SQL = @SQL_SELECT
               + @SQL_FROM
               + @SQL_WHERE
               + @SQL_ORDERBY;

    EXEC sp_executesql @SQL,
                       @SQL_PARAMS,
                       @AgentId = @p_AgentId,
                       @RepId = @p_RepId,
                       @PeriodId = @p_PeriodId;
END;


GO

GRANT EXECUTE ON dbo.Survey_Assignment_Get TO [public] AS [dbo];
GO