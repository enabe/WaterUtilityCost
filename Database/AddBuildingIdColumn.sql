-- BuildingsテーブルにBuildingId列を追加するスクリプト
-- 実行前にデータベースのバックアップを取ることを推奨します

-- 列が存在しない場合のみ追加
IF NOT EXISTS (
    SELECT * 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[Buildings]') 
    AND name = 'BuildingId'
)
BEGIN
    ALTER TABLE [dbo].[Buildings]
    ADD [BuildingId] NVARCHAR(50) NULL;
    
    PRINT 'BuildingId列を追加しました。';
END
ELSE
BEGIN
    PRINT 'BuildingId列は既に存在します。';
END


