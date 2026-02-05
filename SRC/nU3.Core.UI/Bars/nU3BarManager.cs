using DevExpress.XtraBars;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nU3.Core.UI.Controls.Bars
{
    /// <summary>
    /// nU3 Framework 표준 BarManager 컴포넌트
    /// </summary>
    [ToolboxItem(true)]
    public class nU3BarManager : BarManager, InU3Control
    {
        public nU3BarManager() : base() { }
        public nU3BarManager(IContainer container) : base(container) { }

        public void Clear()
        {
            Bars.Clear();
            Items.Clear();
        }

        public string GetControlId()
        {
            return Site?.Name ?? string.Empty;
        }

        public object? GetValue()
        {
            return null;
        }

        public void SetValue(object? value)
        {
        }
    }
}