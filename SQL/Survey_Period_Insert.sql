USE [TheDatabase];
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Period_Insert')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Period_Insert;
END;
GO

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE PROCEDURE dbo.Survey_Period_Insert(
       @p_StartDate DATE,
       @p_EndDate   DATE,
       @p_IsOpen    BIT,
       @p_PeriodId  SMALLINT OUTPUT)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO tb_Survey_Period
           ([StartDate],
            [EndDate],
            [IsOpen])
    VALUES
           (@p_StartDate,
            @p_EndDate,
            @p_IsOpen);

    SELECT @p_PeriodId = @@IDENTITY;
END;


GO

GRANT EXECUTE ON dbo.Survey_Period_Insert TO [public] AS [dbo];
GO