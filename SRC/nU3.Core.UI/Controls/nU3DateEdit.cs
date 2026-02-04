using System;
using System.ComponentModel;
using DevExpress.XtraEditors;

namespace nU3.Core.UI.Controls
{
    [ToolboxItem(true)]
    public class nU3DateEdit : DateEdit, InU3Control
    {
        public nU3DateEdit()
        {
        }

        public object? GetValue()
        {
            return this.EditValue;
        }

        public void SetValue(object? value)
        {
            this.EditValue = value;
        }

        public void Clear()
        {
            this.EditValue = null;
        }

        public string GetControlId()
        {
            return this.Name;
        }
    }
}
