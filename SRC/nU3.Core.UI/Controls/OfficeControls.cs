using System;
using System.ComponentModel;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraRichEdit;
using DevExpress.XtraScheduler;

namespace nU3.Core.UI.Controls
{
    [ToolboxItem(true)]
    public class nU3SpreadsheetControl : SpreadsheetControl, InU3Control
    {
        public object? GetValue() => this.Document;
        public void SetValue(object? value) { /* Complex loading logic usually needed */ }
        public void Clear() => this.CreateNewDocument();
        public string GetControlId() => this.Name;
    }

    [ToolboxItem(true)]
    public class nU3RichEditControl : RichEditControl, InU3Control
    {
        public object? GetValue() => this.HtmlText;
        public void SetValue(object? value) 
        {
             if (value is string s) this.HtmlText = s;
        }
        public void Clear() => this.CreateNewDocument();
        public string GetControlId() => this.Name;
    }

    [ToolboxItem(true)]
    public class nU3SchedulerControl : SchedulerControl, InU3Control
    {
        public object? GetValue() => this.DataStorage;
        public void SetValue(object? value) 
        {
            if (value is ISchedulerStorage storage) this.DataStorage = storage;
        }
        public void Clear() => this.DataStorage.Appointments.Clear();
        public string GetControlId() => this.Name;
    }
}
