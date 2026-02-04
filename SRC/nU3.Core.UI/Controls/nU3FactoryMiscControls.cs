using System;
using System.ComponentModel;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraRichEdit;
using DevExpress.XtraScheduler;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;

namespace nU3.Core.UI.Controls
{
    // =================================================================================================
    // 1. nU3SpreadsheetControl
    // =================================================================================================

    // [ToolboxItem(true)]
    public class nU3SpreadsheetControlExtended : SpreadsheetControl, InU3Control
    {
        public nU3SpreadsheetControlExtended() : base() 
        {
            // Standard: Disable formula bar by default for clean UI
            this.Options.Behavior.ShowPopupMenu = DevExpress.XtraSpreadsheet.DocumentCapability.Disabled;
        }

        #region InU3Control Implementation
        public object? GetValue() => this.Document;
        public void SetValue(object? value) { }
        public void Clear() => this.CreateNewDocument();
        public string GetControlId() => this.Name;
        #endregion
    }


    // =================================================================================================
    // 2. nU3RichEditControl
    // =================================================================================================

    // [ToolboxItem(true)]
    public class nU3RichEditControlExtended : RichEditControl, InU3Control
    {
        public nU3RichEditControlExtended() : base() 
        {
            // Standard: ReadOnly by default unless specified?
            // this.ReadOnly = true; 
        }

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


    // =================================================================================================
    // 3. nU3SchedulerControl
    // =================================================================================================

    // [ToolboxItem(true)]
    public class nU3SchedulerControlExtended : SchedulerControl, InU3Control
    {
        public nU3SchedulerControlExtended() : base() 
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


    // =================================================================================================
    // 4. nU3ChartControl
    // =================================================================================================

    // [ToolboxItem(true)]
    public class nU3ChartControlExtended : ChartControl, InU3Control
    {
        public nU3ChartControlExtended() : base() 
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


    // =================================================================================================
    // 5. nU3ButtonEdit (RepositoryItem Chain)
    // =================================================================================================

    [UserRepositoryItem("RegisternU3ButtonEdit")]
    public class nU3RepositoryItemButtonEdit : RepositoryItemButtonEdit
    {
        static nU3RepositoryItemButtonEdit() { RegisternU3ButtonEdit(); }
        public const string CustomEditName = "nU3ButtonEdit";
        public override string EditorTypeName => CustomEditName;

        public static void RegisternU3ButtonEdit()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(
                CustomEditName, typeof(nU3ButtonEditExtended), typeof(nU3RepositoryItemButtonEdit),
                typeof(DevExpress.XtraEditors.ViewInfo.ButtonEditViewInfo), new DevExpress.XtraEditors.Drawing.ButtonEditPainter(), true));
        }
    }

    // [ToolboxItem(true)]
    public class nU3ButtonEditExtended : ButtonEdit, InU3Control
    {
        static nU3ButtonEditExtended() { nU3RepositoryItemButtonEdit.RegisternU3ButtonEdit(); }
        public override string EditorTypeName => nU3RepositoryItemButtonEdit.CustomEditName;

        protected override RepositoryItem CreateRepositoryItem() => new nU3RepositoryItemButtonEdit();

        #region InU3Control Implementation
        public object? GetValue() => this.EditValue;
        public void SetValue(object? value) => this.EditValue = value;
        public void Clear() => this.EditValue = null;
        public string GetControlId() => this.Name;
        #endregion
    }


    // =================================================================================================
    // 6. nU3SimpleButton
    // =================================================================================================

    // [ToolboxItem(true)]
    public class nU3SimpleButtonExtended : SimpleButton, InU3Control
    {
        public nU3SimpleButtonExtended() : base() { }

        [Category("nU3 Framework")]
        public string AuthId { get; set; } = string.Empty;

        #region InU3Control Implementation
        public object? GetValue() => this.Text;
        public void SetValue(object? value) => this.Text = value?.ToString();
        public void Clear() { }
        public string GetControlId() => this.Name;
        #endregion
    }


    // =================================================================================================
    // 7. nU3LabelControl
    // =================================================================================================

    // [ToolboxItem(true)]
    public class nU3LabelControlExtended : LabelControl, InU3Control
    {
        public nU3LabelControlExtended() : base() { }

        [Category("nU3 Framework")]
        public bool IsRequiredMarker { get; set; } = false;

        #region InU3Control Implementation
        public object? GetValue() => this.Text;
        public void SetValue(object? value) => this.Text = value?.ToString();
        public void Clear() { }
        public string GetControlId() => this.Name;
        #endregion
    }
}
