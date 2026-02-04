using System;
using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraBars;
using DevExpress.XtraDataLayout;

namespace nU3.Core.UI.Controls
{
    // =================================================================================================
    // 1. Additional RepositoryItems (Completing 100% In-place Editor Support)
    // =================================================================================================

    [UserRepositoryItem("RegisternU3ImageComboBox")]
    public class nU3RepositoryItemImageComboBox : RepositoryItemImageComboBox
    {
        static nU3RepositoryItemImageComboBox() { RegisternU3ImageComboBox(); }
        public const string CustomEditName = "nU3ImageComboBox";
        public override string EditorTypeName => CustomEditName;

        public static void RegisternU3ImageComboBox()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(
                CustomEditName, typeof(ImageComboBoxEdit), typeof(nU3RepositoryItemImageComboBox),
                typeof(DevExpress.XtraEditors.ViewInfo.ImageComboBoxEditViewInfo), 
                new DevExpress.XtraEditors.Drawing.ButtonEditPainter(), true));
        }
    }

    [UserRepositoryItem("RegisternU3ProgressBar")]
    public class nU3RepositoryItemProgressBar : RepositoryItemProgressBar
    {
        static nU3RepositoryItemProgressBar() { RegisternU3ProgressBar(); }
        public const string CustomEditName = "nU3ProgressBar";
        public override string EditorTypeName => CustomEditName;

        public static void RegisternU3ProgressBar()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(
                CustomEditName, typeof(ProgressBarControl), typeof(nU3RepositoryItemProgressBar),
                typeof(DevExpress.XtraEditors.ViewInfo.ProgressBarViewInfo), 
                new DevExpress.XtraEditors.Drawing.ProgressBarPainter(), true));
        }
    }

    // =================================================================================================
    // 2. nU3 Bar Items (Ribbon & Toolbar Authorization Support)
    // =================================================================================================

    /// <summary>
    /// nU3 Standard Button Item for Ribbon/Toolbar
    /// </summary>
    public class nU3BarButtonItem : BarButtonItem
    {
        public nU3BarButtonItem() : base() { }
        
        [Category("nU3 Framework")]
        [Description("Authorization ID for this button")]
        public string AuthId { get; set; } = string.Empty;
    }

    /// <summary>
    /// nU3 Standard Check Item for Ribbon/Toolbar
    /// </summary>
    public class nU3BarCheckItem : BarCheckItem
    {
        public nU3BarCheckItem() : base() { }

        [Category("nU3 Framework")]
        public string AuthId { get; set; } = string.Empty;
    }

    /// <summary>
    /// nU3 Standard SubMenu Item for Ribbon/Toolbar
    /// </summary>
    public class nU3BarSubItem : BarSubItem
    {
        public nU3BarSubItem() : base() { }

        [Category("nU3 Framework")]
        public string AuthId { get; set; } = string.Empty;
    }

    /// <summary>
    /// nU3 Standard Edit Item (In-place editor) for Ribbon/Toolbar
    /// </summary>
    public class nU3BarEditItem : BarEditItem
    {
        public nU3BarEditItem() : base() { }

        [Category("nU3 Framework")]
        public string AuthId { get; set; } = string.Empty;
    }


    // =================================================================================================
    // 3. nU3DataLayoutControl (Metadata-Driven Layout)
    // =================================================================================================

    [ToolboxItem(true)]
    public class nU3DataLayoutControl : DataLayoutControl, InU3Control
    {
        public nU3DataLayoutControl() : base() 
        {
            this.AllowGeneratingNestedGroups = DevExpress.Utils.DefaultBoolean.True;
        }

        // Ensuring that dynamic fields also use nU3 standards where possible
        protected override void OnFieldRetrieved(FieldRetrievedEventArgs e)
        {
            base.OnFieldRetrieved(e);
            // Future implementation: Logic to force 'e.Control' to become an nU3 wrapper instance
        }

        #region InU3Control Implementation
        public object? GetValue() => null;
        public void SetValue(object? value) { }
        public void Clear() { }
        public string GetControlId() => this.Name;
        #endregion
    }
}
