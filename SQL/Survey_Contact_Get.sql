USE [TheDatabase];
GO
/****** Object:  StoredProcedure [dbo].[Survey_Contact_Get]    Script Date: 4/4/2017 12:37:39 PM ******/

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Contact_Get')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Contact_Get;
END;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Contact_Get]    Script Date: 4/4/2017 12:37:39 PM ******/

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Contact_Get]    Script Date: 07/15/2015 13:47:11 ******/

CREATE PROCEDURE dbo.Survey_Contact_Get(
       @p_ContactId BIGINT)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [contact].[PK]       [ContactId],
		 [FirstName],
		 [MiddleName],
		 [LastName],
		 [PhoneNumber],
		 [CallerNotes]        [RepNotes],
		 [IsPrimary],
		 [contact].[CalleeID] [AgentId],
		 [agent].[AgencyCode] [AgencyCode],
		 [agent].[AgencyName] [AgencyName],
		 CASE
			WHEN [agent].[agencystatus] = 'Can' THEN 1
			ELSE 0
		 END                  [IsCanceled]
    FROM dbo.tb_Survey_CalleeContact contact
    JOIN tb_Survey_Callee callee
	    ON callee.pk = contact.CalleeId
    LEFT OUTER JOIN Agency agent
	    ON agent.agencycode = callee.agencycode
    WHERE [contact].[pk] = @p_ContactId
		OR @p_ContactId IS NULL;
END;

GO

GRANT EXECUTE ON dbo.Survey_Contact_Get TO [public] AS [dbo];
GO