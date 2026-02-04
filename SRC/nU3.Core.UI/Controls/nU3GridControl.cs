using System;
using System.ComponentModel;
using DevExpress.XtraGrid;

namespace nU3.Core.UI.Controls
{
    [ToolboxItem(true)]
    public class nU3GridControl : GridControl, InU3Control
    {
        public nU3GridControl()
        {
        }

        public object? GetValue()
        {
            return this.DataSource;
        }

        public void SetValue(object? value)
        {
            this.DataSource = value;
        }

        public void Clear()
        {
            this.DataSource = null;
        }

        public string GetControlId()
        {
            return this.Name;
        }
    }
}
