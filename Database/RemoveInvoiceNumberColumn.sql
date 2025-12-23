-- ElectricBillingsテーブルからInvoiceNumber列を削除するスクリプト
-- 実行前にデータベースのバックアップを取ることを推奨します

-- 列が存在する場合のみ削除
IF EXISTS (
    SELECT * 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[ElectricBillings]') 
    AND name = 'InvoiceNumber'
)
BEGIN
    ALTER TABLE [dbo].[ElectricBillings]
    DROP COLUMN [InvoiceNumber];
    
    PRINT 'InvoiceNumber列を削除しました。';
END
ELSE
BEGIN
    PRINT 'InvoiceNumber列は存在しません。';
END



