-- ChildMeterReadingsテーブルにType列を追加するスクリプト
-- 実行前にデータベースのバックアップを取ることを推奨します

-- 列が存在しない場合のみ追加
IF NOT EXISTS (
    SELECT * 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[ChildMeterReadings]') 
    AND name = 'Type'
)
BEGIN
    ALTER TABLE [dbo].[ChildMeterReadings]
    ADD [Type] NVARCHAR(50) NULL;
    
    PRINT 'Type列を追加しました。';
END
ELSE
BEGIN
    PRINT 'Type列は既に存在します。';
END


