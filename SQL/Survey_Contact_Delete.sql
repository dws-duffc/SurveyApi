USE [TheDatabase];
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Contact_Delete')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Contact_Delete;
END;
GO

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE PROCEDURE dbo.Survey_Contact_Delete(
       @p_ContactId BIGINT,
       @p_RowCount INT OUTPUT)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [dbo].[tb_Survey_CalleeContact]
    WHERE [PK] = @p_ContactId;

    SELECT @p_RowCount = @@ROWCOUNT;
END;


GO

GRANT EXECUTE ON dbo.Survey_Contact_Delete TO [public] AS [dbo];
GO