USE [TheDatabase];
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Contact_Update')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Contact_Update;
END;
GO

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE PROCEDURE dbo.Survey_Contact_Update(
       @p_ContactId     BIGINT,
       @p_AgentId       INT,
       @p_FirstName     NVARCHAR(200),
       @p_MiddleName    NVARCHAR(200),
       @p_LastName      NVARCHAR(200),
       @p_PhoneNumber   NVARCHAR(200),
       @p_RepNotes NVARCHAR(2000),
       @p_IsPrimary     BIT,
       @p_RowCount      INT OUTPUT)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.tb_Survey_CalleeContact
      SET [CalleeID] = @p_AgentId,
          [FirstName] = @p_FirstName,
          [MiddleName] = @p_MiddleName,
          [LastName] = @p_LastName,
          [PhoneNumber] = @p_PhoneNumber,
          [CallerNotes] = @p_RepNotes,
          [IsPrimary] = @p_IsPrimary
    WHERE [PK] = @p_ContactId;

    SELECT @p_RowCount = @@ROWCOUNT;
END;

GO

GRANT EXECUTE ON dbo.Survey_Contact_Update TO [public] AS [dbo];
GO