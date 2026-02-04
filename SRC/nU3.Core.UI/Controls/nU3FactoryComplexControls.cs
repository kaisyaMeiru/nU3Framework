using System;
using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.Utils;
using DevExpress.XtraSplashScreen;

namespace nU3.Core.UI.Controls
{
    // =================================================================================================
    // 1. nU3SearchLookUpEdit (LookUp with internal GridView chain)
    // =================================================================================================

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
        protected override GridView CreateView()
        {
            return new nUGridView(); 
        }
    }

    [ToolboxItem(true)]
    public class nU3SearchLookUpEdit : SearchLookUpEdit, InU3Control
    {
        static nU3SearchLookUpEdit() { nU3RepositoryItemSearchLookUpEdit.RegisternU3SearchLookUpEdit(); }
        public override string EditorTypeName => nU3RepositoryItemSearchLookUpEdit.CustomEditName;

        protected override RepositoryItem CreateRepositoryItem() => new nU3RepositoryItemSearchLookUpEdit();

        public object? GetValue() => this.EditValue;
        public void SetValue(object? value) => this.EditValue = value;
        public void Clear() => this.EditValue = null;
        public string GetControlId() => this.Name;
    }


    // =================================================================================================
    // 2. nU3GridLookUpEdit (Standard Grid LookUp chain)
    // =================================================================================================

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
        protected override GridView CreateView()
        {
            return new nUGridView();
        }
    }

    [ToolboxItem(true)]
    public class nU3GridLookUpEdit : GridLookUpEdit, InU3Control
    {
        static nU3GridLookUpEdit() { nU3RepositoryItemGridLookUpEdit.RegisternU3GridLookUpEdit(); }
        public override string EditorTypeName => nU3RepositoryItemGridLookUpEdit.CustomEditName;

        protected override RepositoryItem CreateRepositoryItem() => new nU3RepositoryItemGridLookUpEdit();

        public object? GetValue() => this.EditValue;
        public void SetValue(object? value) => this.EditValue = value;
        public void Clear() => this.EditValue = null;
        public string GetControlId() => this.Name;
    }


    // =================================================================================================
    // 3. nU3LayoutControlItem (Style & Padding Standardization)
    // =================================================================================================

    public class nU3LayoutControlItem : LayoutControlItem
    {
        public nU3LayoutControlItem() : base() { }
        public nU3LayoutControlItem(LayoutControlGroup parent, System.Windows.Forms.Control control) : base(parent, control) 
        {
            InitializeItem();
        }

        private void InitializeItem()
        {
            // SI Standard Padding: 2px all around
            this.Padding = new DevExpress.XtraLayout.Utils.Padding(2);
            this.TextToControlDistance = 5;
        }
    }


    // =================================================================================================
    // 4. nU3FilterControl (Advanced Query UI)
    // =================================================================================================

    [ToolboxItem(true)]
    public class nU3FilterControl : FilterControl, InU3Control
    {
        public nU3FilterControl() : base() 
        {
            this.ShowGroupCommandsIcon = true;
            this.AllowAggregateEditing = AggregateEditing.AggregateWithCondition;
        }

        public object? GetValue() => this.FilterString;
        public void SetValue(object? value) => this.FilterString = value?.ToString();
        public void Clear() => this.FilterString = string.Empty;
        public string GetControlId() => this.Name;
    }


    // =================================================================================================
    // 5. nU3WorkspaceManager (Personalization Component)
    // =================================================================================================

    public class nU3WorkspaceManagerExtended : WorkspaceManager
    {
        public nU3WorkspaceManagerExtended() : base() 
        {
            this.TransitionType = Transitions.Push;
        }
    }


    // =================================================================================================
    // 6. nU3SplashScreenManager (Global Progress/Loading)
    // =================================================================================================

    public class nU3SplashScreenManagerExtended : SplashScreenManager
    {
        public nU3SplashScreenManagerExtended() : base() { }
        public nU3SplashScreenManagerExtended(System.Windows.Forms.Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut) 
            : base(parentForm, splashFormType, useFadeIn, useFadeOut) { }
    }
}
