-- BuildingsテーブルからBuiltDate, Area, Owner, Contact列を削除するスクリプト
-- 実行前にデータベースのバックアップを取ることを推奨します

-- BuiltDate列が存在する場合のみ削除
IF EXISTS (
    SELECT * 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[Buildings]') 
    AND name = 'BuiltDate'
)
BEGIN
    ALTER TABLE [dbo].[Buildings]
    DROP COLUMN [BuiltDate];
    
    PRINT 'BuiltDate列を削除しました。';
END
ELSE
BEGIN
    PRINT 'BuiltDate列は存在しません。';
END

-- Area列が存在する場合のみ削除
IF EXISTS (
    SELECT * 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[Buildings]') 
    AND name = 'Area'
)
BEGIN
    ALTER TABLE [dbo].[Buildings]
    DROP COLUMN [Area];
    
    PRINT 'Area列を削除しました。';
END
ELSE
BEGIN
    PRINT 'Area列は存在しません。';
END

-- Owner列が存在する場合のみ削除
IF EXISTS (
    SELECT * 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[Buildings]') 
    AND name = 'Owner'
)
BEGIN
    ALTER TABLE [dbo].[Buildings]
    DROP COLUMN [Owner];
    
    PRINT 'Owner列を削除しました。';
END
ELSE
BEGIN
    PRINT 'Owner列は存在しません。';
END

-- Contact列が存在する場合のみ削除
IF EXISTS (
    SELECT * 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[Buildings]') 
    AND name = 'Contact'
)
BEGIN
    ALTER TABLE [dbo].[Buildings]
    DROP COLUMN [Contact];
    
    PRINT 'Contact列を削除しました。';
END
ELSE
BEGIN
    PRINT 'Contact列は存在しません。';
END








