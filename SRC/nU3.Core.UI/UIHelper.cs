using System;
using System.Drawing;
using System.Windows.Forms;

namespace nU3.Core.UI
{
    /// <summary>
    /// 애플리케이션 전반에 걸쳐 표준 UI 스타일을 적용하기 위한 유틸리티입니다.
    /// </summary>
    public static class UIHelper
    {
        public static readonly Font StandardFont = new Font("Segoe UI", 9F, FontStyle.Regular);
        public static readonly Font HeaderFont = new Font("Segoe UI", 11F, FontStyle.Bold);

        public static void ApplyStandardGridSettings(DataGridView grid)
        {
            // TODO: DevExpress GridView 설정으로 대체하세요
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        public static void ApplyTheme(Control control)
        {
            // 재귀적으로 테마를 적용합니다 (DevExpress DefaultLookAndFeel 자리표시자)
            control.Font = StandardFont;
            foreach (Control child in control.Controls)
            {
                ApplyTheme(child);
            }
        }
    }
}
