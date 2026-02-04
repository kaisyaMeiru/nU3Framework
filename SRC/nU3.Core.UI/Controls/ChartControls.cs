using System;
using System.ComponentModel;
using DevExpress.XtraCharts;

namespace nU3.Core.UI.Controls
{
    [ToolboxItem(true)]
    public class nU3ChartControl : ChartControl, InU3Control
    {
        public object? GetValue() => this.DataSource;
        public void SetValue(object? value) => this.DataSource = value;
        public void Clear() => this.DataSource = null;
        public string GetControlId() => this.Name;
    }
}
