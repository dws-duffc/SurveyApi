USE [TheDatabase];
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Period_Update')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Period_Update;
END;
GO

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE PROCEDURE dbo.Survey_Period_Update(
       @p_PeriodId  SMALLINT,
       @p_StartDate DATE,
       @p_EndDate   DATE,
       @p_IsOpen    BIT,
       @p_RowCount  INT OUTPUT)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE tb_Survey_Period
      SET [StartDate] = @p_StartDate,
          [EndDate] = @p_EndDate,
          [IsOpen] = @p_IsOpen
    WHERE [PK] = @p_PeriodId;

    SELECT @p_RowCount = @@ROWCOUNT;
END;


GO

GRANT EXECUTE ON dbo.Survey_Period_Update TO [public] AS [dbo];
GO