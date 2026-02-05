using System;
using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraGauges.Win;
using DevExpress.XtraMap;
using DevExpress.XtraPdfViewer;
using DevExpress.XtraTreeMap;
using DevExpress.XtraWizard;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Docking2010;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Base;

namespace nU3.Core.UI.Controls
{
    #region 1. SearchLookUpEdit

    [UserRepositoryItem("RegisternU3SearchLookUpEdit")]
    public class nU3RepositoryItemSearchLookUpEdit : RepositoryItemSearchLookUpEdit
    {
        static nU3RepositoryItemSearchLookUpEdit() { RegisternU3SearchLookUpEdit(); }
        public const string CustomEditName = "nU3SearchLookUpEdit";
        public override string EditorTypeName => CustomEditName;

        public static void RegisternU3SearchLookUpEdit()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(
                CustomEditName, typeof(nU3SearchLookUpEdit), typeof(nU3RepositoryItemSearchLookUpEdit),
                typeof(DevExpress.XtraEditors.ViewInfo.SearchLookUpEditBaseViewInfo), 
                new DevExpress.XtraEditors.Drawing.ButtonEditPainter(), true));
        }

        // [Factory Override] Use nU3GridView for the popup grid
        protected override ColumnView CreateViewInstance() => new nU3GridView(); 
    }

    [ToolboxItem(true)]
    public class nU3SearchLookUpEdit : SearchLookUpEdit, InU3Control
    {
        static nU3SearchLookUpEdit() { nU3RepositoryItemSearchLookUpEdit.RegisternU3SearchLookUpEdit(); }
        public nU3SearchLookUpEdit() : base() { }
        public override string EditorTypeName => nU3RepositoryItemSearchLookUpEdit.CustomEditName;

        public object? GetValue() => this.EditValue;
        public void SetValue(object? value) => this.EditValue = value;
        public new void Clear() => this.EditValue = null;
        public string GetControlId() => this.Name;
    }

    #endregion

    #region 2. GridLookUpEdit

    [UserRepositoryItem("RegisternU3GridLookUpEdit")]
    public class nU3RepositoryItemGridLookUpEdit : RepositoryItemGridLookUpEdit
    {
        static nU3RepositoryItemGridLookUpEdit() { RegisternU3GridLookUpEdit(); }
        public const string CustomEditName = "nU3GridLookUpEdit";
        public override string EditorTypeName => CustomEditName;

        public static void RegisternU3GridLookUpEdit()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(
                CustomEditName, typeof(nU3GridLookUpEdit), typeof(nU3RepositoryItemGridLookUpEdit),
                typeof(DevExpress.XtraEditors.ViewInfo.GridLookUpEditBaseViewInfo), 
                new DevExpress.XtraEditors.Drawing.ButtonEditPainter(), true));
        }

        // [Factory Override] Use nU3GridView for the popup grid
        protected override ColumnView CreateViewInstance() => new nU3GridView();
    }

    [ToolboxItem(true)]
    public class nU3GridLookUpEdit : GridLookUpEdit, InU3Control
    {
        static nU3GridLookUpEdit() { nU3RepositoryItemGridLookUpEdit.RegisternU3GridLookUpEdit(); }
        public nU3GridLookUpEdit() : base() { }
        public override string EditorTypeName => nU3RepositoryItemGridLookUpEdit.CustomEditName;

        public object? GetValue() => this.EditValue;
        public void SetValue(object? value) => this.EditValue = value;
        public new void Clear() => this.EditValue = null;
        public string GetControlId() => this.Name;
    }

    #endregion

    #region 3. FilterControl

    [ToolboxItem(true)]
    public class nU3FilterControl : FilterControl, InU3Control
    {
        public nU3FilterControl() : base() 
        {
            this.ShowGroupCommandsIcon = true;
            this.AllowAggregateEditing = FilterControlAllowAggregateEditing.AggregateWithCondition;
        }

        public object? GetValue() => this.FilterString;
        public void SetValue(object? value) => this.FilterString = value?.ToString();
        public void Clear() => this.FilterString = string.Empty;
        public string GetControlId() => this.Name;
    }

    #endregion

    #region 4. PDF Viewer

    [ToolboxItem(true)]
    public class nU3PdfViewer : PdfViewer, InU3Control
    {
        public nU3PdfViewer() : base() 
        {
            this.DetachStreamAfterLoadComplete = true;
        }

        public object? GetValue() => this.DocumentFilePath;
        public void SetValue(object? value) 
        {
            if (value is string path) this.LoadDocument(path);
            else if (value is System.IO.Stream stream) this.LoadDocument(stream);
        }
        public void Clear() => this.CloseDocument();
        public string GetControlId() => this.Name;
    }

    #endregion

    #region 5. TreeMapControl

    [ToolboxItem(true)]
    public class nU3TreeMapControl : TreeMapControl, InU3Control
    {
        public nU3TreeMapControl() : base() { }

        public object? GetValue() => null;
        public void SetValue(object? value) { }
        public void Clear() { }
        public string GetControlId() => this.Name;
    }

    #endregion

    #region 6. GaugeControl

    [ToolboxItem(true)]
    public class nU3GaugeControl : GaugeControl, InU3Control
    {
        public nU3GaugeControl() : base() { }

        public object? GetValue() => null; 
        public void SetValue(object? value) { }
        public void Clear() { }
        public string GetControlId() => this.Name;
    }

    #endregion

    #region 7. MapControl

    [ToolboxItem(true)]
    public class nU3MapControl : MapControl, InU3Control
    {
        public nU3MapControl() : base() { }

        public object? GetValue() => null; 
        public void SetValue(object? value) { }
        public void Clear() { } 
        public string GetControlId() => this.Name;
    }

    #endregion

    #region 8. WizardControl

    [ToolboxItem(true)]
    public class nU3WizardControl : WizardControl, InU3Control
    {
        public nU3WizardControl() : base() { }

        public object? GetValue() => this.SelectedPage;
        public void SetValue(object? value) 
        {
            if (value is BaseWizardPage page) this.SelectedPage = page;
        }
        public void Clear() { }
        public string GetControlId() => this.Name;
    }

    #endregion

    #region 9. Managers (Components)

    public class nU3DockManager : DockManager
    {
        public nU3DockManager() : base() { }
        public nU3DockManager(IContainer container) : base(container) { }
    }

    public class nU3DocumentManager : DocumentManager
    {
        public nU3DocumentManager() : base() { }
        public nU3DocumentManager(IContainer container) : base(container) { }
    }

    public class nU3WorkspaceManager : WorkspaceManager
    {
        public nU3WorkspaceManager() : base() 
        {
        }
    }

    public class nU3SplashScreenManager : DevExpress.XtraSplashScreen.SplashScreenManager
    {
        public nU3SplashScreenManager() : base() { }
        public nU3SplashScreenManager(System.Windows.Forms.Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut) 
            : base(parentForm, splashFormType, useFadeIn, useFadeOut) { }
    }

    #endregion
}