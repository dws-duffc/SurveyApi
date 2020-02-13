USE [TheDatabase];
GO
/****** Object:  StoredProcedure [dbo].[Survey_Contact_Search]    Script Date: 4/15/2017 7:11:01 PM ******/

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Contact_Search')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Contact_Search;
END;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Contact_Search]    Script Date: 4/15/2017 7:11:01 PM ******/

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Contact_Search]    Script Date: 07/15/2015 13:47:11 ******/

CREATE PROCEDURE dbo.Survey_Contact_Search(
       @p_FirstName     NVARCHAR(202),
       @p_MiddleName    NVARCHAR(202),
       @p_LastName      NVARCHAR(202),
       @p_PhoneNumber   NVARCHAR(202),
       @p_RepNotes NVARCHAR(2002),
       @p_IsPrimary     BIT,
       @p_AgentId       INT,
       @p_AgencyCode    VARCHAR(10),
       @p_AgencyName    VARCHAR(102),
       @p_IsActiveAgent BIT)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @SQL_WHERE NVARCHAR(4000);
    DECLARE @SQL_ORDERBY NVARCHAR(4000);
    DECLARE @SQL NVARCHAR(4000);
    DECLARE @SQL_PARAMS NVARCHAR(2000);

    SET @SQL_PARAMS =
    N'@FirstName NVARCHAR(202),
	   @MiddleName NVARCHAR(202),
	   @LastName NVARCHAR(202),
	   @PhoneNumber NVARCHAR(202),
	   @RepNotes NVARCHAR(2002),
	   @IsPrimary BIT,
	   @AgentId INT,
	   @AgencyCode VARCHAR(10),
	   @AgencyName VARCHAR(102)';

    SET @SQL =
    'SET NOCOUNT ON;
SELECT [contact].[pk]          [ContactId],
       [contact].[FirstName]   [FirstName],
       [contact].[MiddleName]  [MiddleName],
       [contact].[LastName]    [LastName],
       [contact].[PhoneNumber] [PhoneNumber],
       [contact].[CallerNotes] [RepNotes],
       [contact].[IsPrimary]   [IsPrimary],
       [contact].[CalleeID]    [AgentId],
       [agent].[AgencyCode]    [AgencyCode],
       [agent].[AgencyName]    [AgencyName],
       CASE
           WHEN [agent].[agencystatus] = ''Can'' THEN 0
           ELSE 1
       END                     [IsActiveAgent]
FROM tb_Survey_CalleeContact contact
JOIN tb_Survey_Callee callee
     ON callee.pk = contact.CalleeId
LEFT OUTER JOIN Agency agent
     ON agent.agencycode = callee.agencycode';

    SET @SQL_WHERE = '
WHERE 1 = 1';

    IF @p_FirstName IS NOT NULL
    BEGIN
        SET @p_FirstName = CONCAT('%', @p_FirstName, '%');
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [contact].[FirstName] LIKE @FirstName';
    END;

    IF @p_MiddleName IS NOT NULL
    BEGIN
        SET @p_MiddleName = CONCAT('%', @p_MiddleName, '%');
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [contact].[MiddleName] LIKE @MiddleName';
    END;

    IF @p_LastName IS NOT NULL
    BEGIN
        SET @p_LastName = CONCAT('%', @p_LastName, '%');
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [contact].[LastName] LIKE @LastName';
    END;

    IF @p_PhoneNumber IS NOT NULL
    BEGIN
        SET @p_PhoneNumber = CONCAT('%', @p_PhoneNumber, '%');
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [contact].[PhoneNumber] LIKE @PhoneNumber';
    END;

    IF @p_RepNotes IS NOT NULL
    BEGIN
        SET @p_RepNotes = CONCAT('%', @p_RepNotes, '%');
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [contact].[CallerNotes] LIKE @RepNotes';
    END;

    IF @p_IsPrimary IS NOT NULL
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [contact].[IsPrimary] = @IsPrimary';
    END;

    IF isnull(@p_AgentId, 0) > 0
    BEGIN
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [contact].[CalleeId] = @AgentId';
    END;

    IF @p_AgencyCode IS NOT NULL
    BEGIN
        SET @p_AgencyCode = CONCAT('%', @p_AgencyCode, '%');
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [agent].[AgencyCode] LIKE @AgencyCode';
    END;

    IF @p_AgencyName IS NOT NULL
    BEGIN
        SET @p_AgencyName = CONCAT('%', @p_AgencyName, '%');
        SET @SQL_WHERE = @SQL_WHERE+'
      AND [agent].[AgencyName] LIKE @AgencyName';
    END;

    IF @p_IsActiveAgent IS NOT NULL
    BEGIN
	   SET @SQL_WHERE = @SQL_WHERE+CASE WHEN @p_IsActiveAgent = 1 THEN '
      AND [agent].[agencystatus] = ''act'''
								ELSE '
      AND [agent].[agencystatus] = ''can'''
							 END;
    END;

    SET @SQL_ORDERBY = '
ORDER BY [contact].[LastName] DESC, [contact].[IsPrimary] DESC;';

    SET @SQL = @SQL
               + @SQL_WHERE
               + @SQL_ORDERBY;

    EXEC sp_executesql @SQL,
                       @SQL_PARAMS,
                       @FirstName = @p_FirstName,
                       @MiddleName = @p_MiddleName,
                       @LastName = @p_LastName,
                       @PhoneNumber = @p_PhoneNumber,
                       @RepNotes = @p_RepNotes,
                       @IsPrimary = @p_IsPrimary,
                       @AgentId = @p_AgentId,
                       @AgencyCode = @p_AgencyCode,
                       @AgencyName = @p_AgencyName;
END;

GO

GRANT EXECUTE ON dbo.Survey_Contact_Search TO [public] AS [dbo];
GO