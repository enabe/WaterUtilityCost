-- GasBillingsテーブルからDiscountCharge列を削除するスクリプト
-- 実行前にデータベースのバックアップを取ることを推奨します

-- 列が存在する場合のみ削除
IF EXISTS (
    SELECT * 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[GasBillings]') 
    AND name = 'DiscountCharge'
)
BEGIN
    ALTER TABLE [dbo].[GasBillings]
    DROP COLUMN [DiscountCharge];
    
    PRINT 'DiscountCharge列を削除しました。';
END
ELSE
BEGIN
    PRINT 'DiscountCharge列は存在しません。';
END



