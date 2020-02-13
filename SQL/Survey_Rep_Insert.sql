USE [TheDatabase];
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'dbo.Survey_Rep_Insert')
                AND [type] IN(N'P', N'PC'))
BEGIN
    DROP PROCEDURE dbo.Survey_Rep_Insert;
END;
GO

SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE PROCEDURE dbo.Survey_Rep_Insert(
       @p_Username   VARCHAR(200),
       @p_IsActive   BIT,
       @p_RepId INT OUTPUT)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.tb_Survey_Caller
           ([UserName],
            [IsActive])
    VALUES
           (@p_Username,
            @p_IsActive);

    SELECT @p_RepId = @@IDENTITY;
END;

GO

GRANT EXECUTE ON dbo.Survey_Rep_Insert TO [public] AS [dbo];
GO