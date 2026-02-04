using System;
using System.ComponentModel;
using DevExpress.XtraTreeList;
using DevExpress.XtraVerticalGrid;

namespace nU3.Core.UI.Controls
{
    [ToolboxItem(true)]
    public class nU3TreeList : TreeList, InU3Control
    {
        public object? GetValue() => this.DataSource;
        public void SetValue(object? value) => this.DataSource = value;
        public void Clear() => this.DataSource = null;
        public string GetControlId() => this.Name;
    }

    [ToolboxItem(true)]
    public class nU3VGridControl : VGridControl, InU3Control
    {
        public object? GetValue() => this.DataSource;
        public void SetValue(object? value) => this.DataSource = value;
        public void Clear() => this.DataSource = null;
        public string GetControlId() => this.Name;
    }

    [ToolboxItem(true)]
    public class nU3PropertyGridControl : PropertyGridControl, InU3Control
    {
        public object? GetValue() => this.SelectedObject;
        public void SetValue(object? value) => this.SelectedObject = value;
        public void Clear() => this.SelectedObject = null;
        public string GetControlId() => this.Name;
    }
}