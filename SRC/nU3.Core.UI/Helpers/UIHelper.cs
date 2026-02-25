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
