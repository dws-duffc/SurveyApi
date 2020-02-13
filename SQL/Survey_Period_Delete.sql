USE [TheDatabase];
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Period_Delete')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Period_Delete;
END;
GO

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE PROCEDURE dbo.Survey_Period_Delete(
       @p_PeriodId SMALLINT,
       @p_RowCount INT OUTPUT)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM tb_Survey_Period
    WHERE [PK] = @p_PeriodId;

    SELECT @p_RowCount = @@ROWCOUNT;
END;


GO

GRANT EXECUTE ON dbo.Survey_Period_Delete TO [public] AS [dbo];
GO