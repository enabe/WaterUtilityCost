IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WaterBillings]') AND type in (N'U'))
BEGIN
    IF NOT EXISTS (
        SELECT * FROM sys.columns
        WHERE object_id = OBJECT_ID(N'[dbo].[WaterBillings]') AND name = 'ParentMeterId')
    BEGIN
        ALTER TABLE [dbo].[WaterBillings] ADD [ParentMeterId] INT NULL;
        PRINT 'WaterBillings.ParentMeterId列を追加しました。';
    END
    ELSE
    BEGIN
        PRINT 'WaterBillings.ParentMeterId列は既に存在します。';
    END
END
