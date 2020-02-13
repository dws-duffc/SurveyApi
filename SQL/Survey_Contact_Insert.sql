USE [TheDatabase];
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Contact_Insert')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Contact_Insert;
END;
GO

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE PROCEDURE dbo.Survey_Contact_Insert(
       @p_AgentId       INT,
       @p_FirstName     NVARCHAR(200),
       @p_MiddleName    NVARCHAR(200),
       @p_LastName      NVARCHAR(200),
       @p_PhoneNumber   NVARCHAR(200),
       @p_RepNotes NVARCHAR(200),
       @p_IsPrimary     BIT,
       @p_ContactId     BIGINT OUTPUT)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.tb_Survey_CalleeContact
           ([CalleeID],
            [FirstName],
            [MiddleName],
            [LastName],
            [PhoneNumber],
            [CallerNotes],
            [IsPrimary])
    VALUES
           (@p_AgentId,
            @p_FirstName,
            @p_MiddleName,
            @p_LastName,
            @p_PhoneNumber,
            @p_RepNotes,
            @p_IsPrimary);

    SELECT @p_ContactId = @@IDENTITY;
END;


GO

GRANT EXECUTE ON dbo.Survey_Contact_Insert TO [public] AS [dbo];
GO