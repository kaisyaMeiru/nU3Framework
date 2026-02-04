using System;
using System.ComponentModel;
using DevExpress.XtraEditors;

namespace nU3.Core.UI.Controls
{
    [ToolboxItem(true)]
    public class nU3SimpleButton : SimpleButton, InU3Control
    {
        public nU3SimpleButton()
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
            // Do nothing for button usually
        }

        public string GetControlId()
        {
            return this.Name;
        }
    }
}
