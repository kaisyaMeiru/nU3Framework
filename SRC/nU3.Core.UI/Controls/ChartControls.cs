using System;
using System.ComponentModel;
using DevExpress.XtraCharts;

namespace nU3.Core.UI.Controls
{
    /// <summary>
    /// nU3 Framework 표준 ChartControl
    /// </summary>
    [ToolboxItem(true)]
    public class nU3ChartControl : ChartControl, InU3Control
    {
        public nU3ChartControl() : base() 
        {
            this.AutoLayout = true;
        }

        #region InU3Control Implementation
        public object? GetValue() => this.DataSource;
        public void SetValue(object? value) => this.DataSource = value;
        public void Clear() => this.DataSource = null;
        public string GetControlId() => this.Name;
        #endregion
    }
}