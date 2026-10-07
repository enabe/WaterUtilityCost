using System;
using System.Windows.Forms;

namespace WaterUtilityCost.Forms
{
    /// <summary>
    /// 検針データ削除用の対象年月選択ダイアログ
    /// </summary>
    public partial class ReadingYearMonthSelectDialog : Form
    {
        public int SelectedYear => _dtpYearMonth.Value.Year;
        public int SelectedMonth => _dtpYearMonth.Value.Month;

        public ReadingYearMonthSelectDialog()
            : this("削除する検針年月:")
        {
        }

        public ReadingYearMonthSelectDialog(string yearMonthLabel)
        {
            InitializeComponent();
            _lblYearMonth.Text = yearMonthLabel;
            var today = DateTime.Today;
            _dtpYearMonth.Value = new DateTime(today.Year, today.Month, 1);
        }
    }
}
