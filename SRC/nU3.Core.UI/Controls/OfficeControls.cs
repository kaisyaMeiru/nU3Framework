using System;
using System.ComponentModel;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraRichEdit;
using DevExpress.XtraScheduler;

namespace nU3.Core.UI.Controls
{
    /// <summary>
    /// nU3 Framework 표준 SpreadsheetControl
    /// </summary>
    [ToolboxItem(true)]
    public class nU3SpreadsheetControl : SpreadsheetControl, InU3Control
    {
        public nU3SpreadsheetControl() : base() 
        {
            this.Options.Behavior.ShowPopupMenu = DevExpress.XtraSpreadsheet.DocumentCapability.Disabled;
        }

        #region InU3Control Implementation
        public object? GetValue() => this.Document;
        public void SetValue(object? value) { /* 복합 로직 필요 시 구현 */ }
        public void Clear() => this.CreateNewDocument();
        public string GetControlId() => this.Name;
        #endregion
    }

    /// <summary>
    /// nU3 Framework 표준 RichEditControl
    /// </summary>
    [ToolboxItem(true)]
    public class nU3RichEditControl : RichEditControl, InU3Control
    {
        public nU3RichEditControl() : base() { }

        #region InU3Control Implementation
        public object? GetValue() => this.HtmlText;
        public void SetValue(object? value) 
        {
             if (value is string s) this.HtmlText = s;
        }
        public void Clear() => this.CreateNewDocument();
        public string GetControlId() => this.Name;
        #endregion
    }

    /// <summary>
    /// nU3 Framework 표준 SchedulerControl
    /// </summary>
    [ToolboxItem(true)]
    public class nU3SchedulerControl : SchedulerControl, InU3Control
    {
        public nU3SchedulerControl() : base() 
        {
            this.ActiveViewType = SchedulerViewType.Month;
        }

        #region InU3Control Implementation
        public object? GetValue() => this.DataStorage;
        public void SetValue(object? value) 
        {
            if (value is ISchedulerStorage storage) this.DataStorage = storage;
        }
        public void Clear() => this.DataStorage?.Appointments.Clear();
        public string GetControlId() => this.Name;
        #endregion
    }
}