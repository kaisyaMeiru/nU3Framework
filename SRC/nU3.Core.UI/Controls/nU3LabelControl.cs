using System;
using System.ComponentModel;
using DevExpress.XtraEditors;

namespace nU3.Core.UI.Controls
{
    [ToolboxItem(true)]
    public class nU3LabelControl : LabelControl, InU3Control
    {
        public nU3LabelControl()
        {
        }

        public object? GetValue()
        {
            return this.Text;
        }

        public void SetValue(object? value)
        {
            this.Text = value?.ToString();
        }

        public void Clear()
        {
            this.Text = string.Empty;
        }

        public string GetControlId()
        {
            return this.Name;
        }
    }
}
