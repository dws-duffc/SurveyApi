USE [TheDatabase];
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Rep_Update')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Rep_Update;
END;
GO

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE PROCEDURE dbo.Survey_Rep_Update(
       @p_RepId INT,
       @p_Username   VARCHAR(200),
       @p_IsActive   BIT,
       @p_RowCount   INT OUTPUT)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.tb_Survey_Caller
      SET [Username] = @p_Username,
          [IsActive] = @p_IsActive
    WHERE [PK] = @p_RepId;

    SELECT @p_RowCount = @@ROWCOUNT;
END;

GO

GRANT EXECUTE ON dbo.Survey_Rep_Update TO [public] AS [dbo];
GO