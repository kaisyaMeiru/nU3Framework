using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace nU3.Core.UI.Helpers
{
    /// <summary>
    /// UI 컨트롤 및 스레드 처리를 위한 공통 헬퍼 클래스입니다.
    /// </summary>
    public static class UIHelper
    {
        public static readonly System.Drawing.Font StandardFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
        public static readonly System.Drawing.Font HeaderFont = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);

        public static void ApplyStandardGridSettings(DataGridView grid)
        {
            grid.BackgroundColor = System.Drawing.Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        public static void ApplyTheme(Control control)
        {
            control.Font = StandardFont;
            foreach (Control child in control.Controls)
            {
                ApplyTheme(child);
            }
        }

        public static void SafeInvoke(this Control control, Action action)
        {
            if (control == null || control.IsDisposed || control.Disposing) return;
            if (control.InvokeRequired)
            {
                try { control.BeginInvoke(action); } catch { }
            }
            else action();
        }

        public static List<T> FindAllControls<T>(this Control parent) where T : Control
        {
            var result = new List<T>();
            if (parent == null) return result;
            foreach (Control control in parent.Controls)
            {
                if (control is T target) result.Add(target);
                if (control.HasChildren) result.AddRange(FindAllControls<T>(control));
            }
            return result;
        }
    }
}
