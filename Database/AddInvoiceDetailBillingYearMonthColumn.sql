-- InvoiceDetailsテーブルに請求年月（受領請求年月）列を追加するスクリプト
-- 実行前にデータベースのバックアップを取ることを推奨します

IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[InvoiceDetails]')
    AND name = 'BillingYearMonth'
)
BEGIN
    ALTER TABLE [dbo].[InvoiceDetails]
    ADD [BillingYearMonth] NVARCHAR(7) NULL;

    PRINT 'BillingYearMonth列を追加しました。';
END
ELSE
BEGIN
    PRINT 'BillingYearMonth列は既に存在します。';
END
