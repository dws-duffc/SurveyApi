USE [TheDatabase];
GO

IF EXISTS(SELECT *
          FROM sys.types
          WHERE [is_table_type] = 1
                AND [name] = 'WhereCondition')
BEGIN
    DROP TYPE WhereCondition;
END;
GO

CREATE TYPE WhereCondition AS TABLE(
                     [Operator]   VARCHAR(200) NOT NULL,
                     [Column]     VARCHAR(200) NOT NULL,
                     [Comparison] VARCHAR(200) NOT NULL,
                     [value]      VARCHAR(200) NOT NULL);

GRANT EXECUTE ON TYPE ::WhereCondition TO public;
GO