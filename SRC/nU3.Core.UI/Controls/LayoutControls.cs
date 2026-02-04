using System;
using System.ComponentModel;
using DevExpress.XtraLayout;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;

namespace nU3.Core.UI.Controls
{
    [ToolboxItem(true)]
    public class nU3LayoutControl : LayoutControl, InU3Control
    {
        public object? GetValue() => null; // Container
        public void SetValue(object? value) { }
        public void Clear() { }
        public string GetControlId() => this.Name;
    }

    [ToolboxItem(true)]
    public class nU3GroupControl : GroupControl, InU3Control
    {
        public object? GetValue() => this.Text;
        public void SetValue(object? value) => this.Text = value?.ToString();
        public void Clear() { }
        public string GetControlId() => this.Name;
    }

    [ToolboxItem(true)]
    public class nU3PanelControl : PanelControl, InU3Control
    {
        public object? GetValue() => null;
        public void SetValue(object? value) { }
        public void Clear() { }
        public string GetControlId() => this.Name;
    }

    [ToolboxItem(true)]
    public class nU3XtraTabControl : XtraTabControl, InU3Control
    {
        public object? GetValue() => this.SelectedTabPage;
        public void SetValue(object? value) 
        {
            if (value is XtraTabPage page) this.SelectedTabPage = page;
        }
        public void Clear() { }
        public string GetControlId() => this.Name;
    }

    [ToolboxItem(true)]
    public class nU3SplitContainerControl : SplitContainerControl, InU3Control
    {
         public object? GetValue() => null;
         public void SetValue(object? value) { }
         public void Clear() { }
         public string GetControlId() => this.Name;
    }
}
