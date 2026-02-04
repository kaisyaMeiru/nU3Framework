using System;
using System.ComponentModel;
using DevExpress.XtraEditors;

namespace nU3.Core.UI.Controls
{
    [ToolboxItem(true)]
    public class nU3CheckEdit : CheckEdit, InU3Control
    {
        public object? GetValue() => this.Checked;
        public void SetValue(object? value) 
        {
            if (value is bool b) this.Checked = b;
            else if (value != null) this.Checked = Convert.ToBoolean(value);
            else this.Checked = false;
        }
        public void Clear() => this.Checked = false;
        public string GetControlId() => this.Name;
    }

    [ToolboxItem(true)]
    public class nU3MemoEdit : MemoEdit, InU3Control
    {
        public object? GetValue() => this.EditValue;
        public void SetValue(object? value) => this.EditValue = value;
        public void Clear() => this.EditValue = null;
        public string GetControlId() => this.Name;
    }

    [ToolboxItem(true)]
    public class nU3ComboBoxEdit : ComboBoxEdit, InU3Control
    {
        public object? GetValue() => this.EditValue;
        public void SetValue(object? value) => this.EditValue = value;
        public void Clear() => this.EditValue = null;
        public string GetControlId() => this.Name;
    }

    [ToolboxItem(true)]
    public class nU3SpinEdit : SpinEdit, InU3Control
    {
        public object? GetValue() => this.Value;
        public void SetValue(object? value) => this.EditValue = value;
        public void Clear() => this.Value = 0;
        public string GetControlId() => this.Name;
    }

    [ToolboxItem(true)]
    public class nU3RadioGroup : RadioGroup, InU3Control
    {
        public object? GetValue() => this.EditValue;
        public void SetValue(object? value) => this.EditValue = value;
        public void Clear() => this.EditValue = null;
        public string GetControlId() => this.Name;
    }

    [ToolboxItem(true)]
    public class nU3ToggleSwitch : ToggleSwitch, InU3Control
    {
        public object? GetValue() => this.IsOn;
        public void SetValue(object? value) 
        {
             if (value is bool b) this.IsOn = b;
             else if (value != null) this.IsOn = Convert.ToBoolean(value);
             else this.IsOn = false;
        }
        public void Clear() => this.IsOn = false;
        public string GetControlId() => this.Name;
    }
}
