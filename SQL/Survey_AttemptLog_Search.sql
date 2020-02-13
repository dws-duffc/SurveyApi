USE [TheDatabase];
GO
/****** Object:  StoredProcedure [dbo].[Survey_AttemptLog_Search]    Script Date: 4/15/2017 7:11:01 PM ******/

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_AttemptLog_Search')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_AttemptLog_Search;
END;
GO
/****** Object:  StoredProcedure [dbo].[Survey_AttemptLog_Search]    Script Date: 4/15/2017 7:11:01 PM ******/

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO
/****** Object:  StoredProcedure [dbo].[Survey_AttemptLog_Search]    Script Date: 07/15/2015 13:47:11 ******/

CREATE PROCEDURE dbo.Survey_AttemptLog_Search(
       @p_PeriodId      SMALLINT,
       @p_PeriodIsOpen  BIT,
       @p_AgentId       INT,
       @p_AgencyCode    VARCHAR(10),
       @p_AgencyName    VARCHAR(102),
       @p_IsActiveAgent BIT,
       @p_RepId    INT,
       @p_RepUsername   VARCHAR(202),
       @p_RepFirstName  VARCHAR(12),
       @p_RepLastName   VARCHAR(22),
       @p_RepIsActive   BIT,
       @p_AttemptedDate DATETIME,
       @p_AttemptedBy   VARCHAR(202))
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @SQL_WHERE NVARCHAR(4000);
    DECLARE @SQL_ORDERBY NVARCHAR(4000);
    DECLARE @SQL NVARCHAR(4000);
    DECLARE @SQL_PARAMS NVARCHAR(2000);

    SET @SQL_PARAMS =
    N'@PeriodId SMALLINT,@PeriodIsOpen BIT,@AgentId INT,@AgencyCode VARCHAR(10),@AgencyName VARCHAR(102),
    @RepId INT,@RepUsername VARCHAR(202),@RepFirstName VARCHAR(12),@RepLastName VARCHAR(22),
    @RepIsActive BIT,@AttemptedDate DATETIME,@AttemptedBy VARCHAR(202)';

    SET @SQL =
    'SET NOCOUNT ON;
SELECT [PeriodId],
       [period].[startdate]      [StartDate],
       [period].[enddate]        [EndDate],
       [period].[isopen]         [IsOpen],
       [CalleeID]                [AgentId],
       [agent].[agencycode]      [AgencyCode],
       [agent].[agencyname]      [AgencyName],
       CASE
           WHEN [agent].[agencystatus] = ''Can'' THEN 0
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
     ON usr.usr_userid = Rep.username';

    SET @SQL_WHERE = '
WHERE 1 = 1';

    IF isnull(@p_PeriodId, 0) > 0
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [period].[pk] = @PeriodId';
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

    IF @p_RepLastname IS NOT NULL
    BEGIN
        SET @p_RepLastname = CONCAT('%', @p_RepLastname, '%');
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [usr].[usr_lname] LIKE @RepLastName';
    END;

    IF @p_RepIsActive IS NOT NULL
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [Rep].[isactive] = @RepIsActive';
    END;

    IF @p_AttemptedDate IS NOT NULL
    BEGIN
        SET @p_AttemptedDate = CAST(@p_AttemptedDate AS DATE);
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [attempt].[AttemptedDate] >= @AttemptedDate
      AND [attempt].[AttemptedDate] < DATEADD(day, 1, @AttemptedDate)';
    END;

    IF @p_AttemptedBy IS NOT NULL
    BEGIN
        SET @p_AttemptedBy = CONCAT('%', @p_AttemptedBy, '%');
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [attempt].[attemptedby] LIKE @AttemptedBy';
    END;

    SET @SQL_ORDERBY = '
ORDER BY [attempt].[AttemptedDate] DESC;';

    SET @SQL = @SQL
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
                       @AttemptedDate = @p_AttemptedDate,
                       @AttemptedBy = @p_AttemptedBy;
END;

GO

GRANT EXECUTE ON dbo.Survey_AttemptLog_Search TO [public] AS [dbo];
GO