USE [TheDatabase];
GO
/****** Object:  StoredProcedure [dbo].[Survey_Period_Get]    Script Date: 4/4/2017 12:37:39 PM ******/

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Period_Get')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Period_Get;
END;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Period_Get]    Script Date: 4/4/2017 12:37:39 PM ******/

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO
/****** Object:  StoredProcedure [dbo].[Survey_Period_Get]    Script Date: 07/15/2015 13:47:11 ******/

CREATE PROCEDURE dbo.Survey_Period_Get(
       @p_PeriodId SMALLINT)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [PK] [PeriodId],
           [StartDate],
           [EndDate],
           [IsOpen]
    FROM tb_Survey_Period
    WHERE [pk] = @p_PeriodId
          OR @p_PeriodId IS NULL;
END;


GO

GRANT EXECUTE ON dbo.Survey_Period_Get TO [public] AS [dbo];
GO