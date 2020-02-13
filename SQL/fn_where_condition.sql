USE [TheDatabase];
GO

IF EXISTS(SELECT *
          FROM sys.objects
          WHERE object_id = OBJECT_ID(N'[dbo].[fn_where_condition]')
                AND [type] IN(N'FN', N'IF', N'TF'))
BEGIN
    DROP FUNCTION dbo.fn_where_condition;
END;
GO

CREATE FUNCTION dbo.fn_where_condition(
                @operator   VARCHAR(200),
                @column     VARCHAR(200),
                @comparison VARCHAR(200),
                @value      VARCHAR(200))
RETURNS VARCHAR(1000)
AS
BEGIN
    DECLARE @Result VARCHAR(1000);

    SELECT @Result = (SELECT ' '+@operator+' '+@column+' '+@comparison+CASE
                                                                           WHEN @comparison = 'Like' THEN '''%'+@value+
                                                                           '%'''
                                                                           WHEN @comparison = 'In' THEN '('+@value+')'
                                                                           ELSE ''''+@value+''''
                                                                       END);

    RETURN @Result;
END;

GRANT EXECUTE ON fn_where_condition TO public;
GO