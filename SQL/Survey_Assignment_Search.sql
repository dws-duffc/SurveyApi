USE [TheDatabase];
GO
/****** Object:  StoredProcedure [dbo].[Survey_Assignment_Search]    Script Date: 4/4/2017 12:37:39 PM ******/

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Assignment_Search')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Assignment_Search;
END;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Assignment_Search]    Script Date: 4/4/2017 12:37:39 PM ******/

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Assignment_Search]    Script Date: 07/15/2015 13:47:11 ******/

CREATE PROCEDURE dbo.Survey_Assignment_Search(
       @p_PeriodId               SMALLINT,
       @p_PeriodStartDate	   DATETIME,
       @p_PeriodEndDate		   DATETIME,
       @p_PeriodIsOpen           BIT,
       @p_AgentId                INT,
       @p_AgencyCode             VARCHAR(10),
       @p_AgencyName             VARCHAR(102),
       @p_IsActiveAgent          BIT,
       @p_RepId             INT,
       @p_RepUsername            VARCHAR(202),
       @p_RepFirstName           VARCHAR(12),
       @p_RepLastName            VARCHAR(22),
       @p_RepIsActive            BIT,
       @p_AssignmentStatus       VARCHAR(202),
       @p_LastAttemptedBy        VARCHAR(202),
       @p_LastAttemptedDate      DATETIME,
       @p_Notes                  VARCHAR(8000))
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @SQL_SELECT NVARCHAR(4000);
    DECLARE @SQL_FROM NVARCHAR(4000);
    DECLARE @SQL_WHERE NVARCHAR(4000);
    DECLARE @SQL_ORDERBY NVARCHAR(4000);
    DECLARE @SQL NVARCHAR(4000);
    DECLARE @SQL_PARAMS NVARCHAR(2000);

    SET @SQL_PARAMS =
    N'@PeriodId SMALLINT,@PeriodStartDate DATETIME,@PeriodEndDate DATETIME,
    @PeriodIsOpen BIT,@AgentId INT,@AgencyCode VARCHAR(10),
    @AgencyName VARCHAR(102),@RepId INT,@RepUsername VARCHAR(202),
    @RepFirstName VARCHAR(12),@RepLastName VARCHAR(22),@RepIsActive BIT,
    @AssignmentStatus VARCHAR(202),@LastAttemptedBy VARCHAR(202),
    @LastAttemptedDate DATETIME,@Notes VARCHAR(8000)';

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

    SET @SQL_FROM ='
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

    IF isnull(@p_PeriodId, 0) > 0
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [period].[pk] = @PeriodId';
    END;

    IF @p_PeriodStartDate IS NOT NULL
    BEGIN
        SET @p_PeriodStartDate = CAST(@p_PeriodStartDate AS DATE);
        SET @SQL_WHERE = @SQL_WHERE+'
      AND CAST([period].[StartDate] AS DATE) BETWEEN @PeriodStartDate and DATEADD(day, 1, @PeriodStartDate)';
    END;
    
    IF @p_PeriodEndDate IS NOT NULL
    BEGIN
        SET @p_PeriodEndDate = CAST(@p_PeriodEndDate AS DATE);
        SET @SQL_WHERE = @SQL_WHERE+'
      AND CAST([period].[EndDate] AS DATE) BETWEEN @PeriodEndDate and DATEADD(day, 1, @PeriodEndDate)';
    END;

    IF @p_PeriodIsOpen IS NOT NULL
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [period].[isopen] = @PeriodIsOpen';
    END;

    IF isnull(@p_AgentId, 0) > 0
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [callee].[pk] = @AgentId';
    END;

    IF @p_AgencyCode IS NOT NULL
    BEGIN
        SET @p_AgencyCode = CONCAT('%', @p_AgencyCode, '%');
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [agent].[agencycode] LIKE @AgencyCode';
    END;

    IF @p_AgencyName IS NOT NULL
    BEGIN
        SET @p_AgencyName = CONCAT('%', @p_AgencyName, '%');
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [agent].[agencyname] LIKE @AgencyName';
    END;

    IF @p_IsActiveAgent IS NOT NULL
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+CASE
                                        WHEN @p_IsActiveAgent = 1 THEN '
      AND [agent].[agencystatus] = ''act'''
                                        ELSE '
      AND [agent].[agencystatus] = ''can'''
                                    END;
    END;

    IF isnull(@p_RepId, 0) > 0
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [Rep].[pk] = @RepId';
    END;

    IF @p_RepUsername IS NOT NULL
    BEGIN
        SET @p_RepUsername = CONCAT('%', @p_RepUsername, '%');
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [Rep].[username] LIKE @RepUsername';
    END;

    IF @p_RepFirstName IS NOT NULL
    BEGIN
        SET @p_RepFirstName = CONCAT('%', @p_RepFirstName, '%');
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [usr].[usr_fname] LIKE @RepFirstName';
    END;

    IF @p_RepLastName IS NOT NULL
    BEGIN
        SET @p_RepLastName = CONCAT('%', @p_RepLastName, '%');
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [usr].[usr_lname] LIKE @RepLastName';
    END;

    IF @p_RepIsActive IS NOT NULL
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [Rep].[isactive] = @RepIsActive';
    END;

    IF @p_AssignmentStatus IS NOT NULL
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [assignment].[status] = @AssignmentStatus';
    END;

    IF @p_LastAttemptedBy IS NOT NULL
    BEGIN
        SET @p_LastAttemptedBy = CONCAT('%', @p_LastAttemptedBy, '%');
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [assignment].[lastattemptedby] LIKE @LastAttemptedBy';
    END;

    IF @p_LastAttemptedDate IS NOT NULL
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [assignment].[lastattempteddate] >= @LastAttemptedStartDate';
    END;

    IF @p_LastAttemptedDate IS NOT NULL
    BEGIN
        SET @p_LastAttemptedDate = CAST(@p_LastAttemptedDate AS DATE);
        SET @SQL_WHERE = @SQL_WHERE+
        '
      AND CAST([assignment].[lastattempteddate] AS DATE) BETWEEN @LastAttemptedDate and DATEADD(day, 1, @LastAttemptedDate)';
    END;

    IF @p_Notes IS NOT NULL
    BEGIN
        SET @p_Notes = CONCAT('%', @p_Notes, '%');
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [assignment].[notes] LIKE @Notes';
    END;

    SET @SQL_ORDERBY = '
ORDER BY [Rep].[username] ASC;';

    SET @SQL = @SQL_SELECT
               + @SQL_FROM
               + @SQL_WHERE
               + @SQL_ORDERBY;

    EXEC sp_executesql @SQL,
                       @SQL_PARAMS,
                       @PeriodId = @p_PeriodId,
                       @PeriodIsOpen = @p_PeriodIsOpen,
                       @AgentId = @p_AgentId,
                       @AgencyCode = @p_AgencyCode,
                       @AgencyName = @p_AgencyName,
                       @RepId = @p_RepId,
                       @RepUsername = @p_RepUsername,
                       @RepFirstName = @p_RepFirstName,
                       @RepLastName = @p_RepLastName,
                       @RepIsActive = @p_RepIsActive,
                       @AssignmentStatus = @p_AssignmentStatus,
                       @LastAttemptedBy = @p_LastAttemptedBy,
                       @LastAttemptedDate = @p_LastAttemptedDate,
                       @Notes = @p_Notes;
END;


GO

GRANT EXECUTE ON dbo.Survey_Assignment_Search TO [public] AS [dbo];
GO