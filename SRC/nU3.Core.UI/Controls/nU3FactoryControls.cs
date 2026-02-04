using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraVerticalGrid.Rows;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;

namespace nU3.Core.UI.Controls
{
    // =================================================================================================
    // 1. nU3GridControl (GridControl Chain)
    // =================================================================================================
    
    /// <summary>
    /// nU3 Framework Custom Grid Column
    /// </summary>
    public class nU3GridColumn : GridColumn
    {
        public nU3GridColumn() : base() { }

        [Category("nU3 Framework")]
        [Description("Column Authorization ID")]
        public string AuthId { get; set; } = string.Empty;

        [Category("nU3 Framework")]
        [Description("Multilingual Resource Key")]
        public string ResourceKey { get; set; } = string.Empty;
    }

    /// <summary>
    /// nU3 Framework Custom Grid View
    /// </summary>
    public class nU3GridView : GridView
    {
        public nU3GridView() : base() { }
        public nU3GridView(GridControl grid) : base(grid) { }

        // [Factory Override] Create nU3GridColumn instead of default GridColumn
        protected override GridColumn CreateColumn()
        {
            return new nU3GridColumn();
        }

        // Initialize default settings for the framework
        protected override void OnLoaded()
        {
            base.OnLoaded();
            // Example: Standardize appearance
            this.OptionsView.ShowGroupPanel = false;
            this.OptionsSelection.MultiSelect = true;
        }
    }

    /// <summary>
    /// Registrator to expose nU3GridView to the Designer
    /// </summary>
    public class nU3GridViewInfoRegistrator : GridInfoRegistrator
    {
        public override string ViewName => "nU3GridView";
        public override BaseView CreateView(GridControl grid) => new nU3GridView(grid);
    }

    /// <summary>
    /// nU3 Framework Grid Control with Factory Overrides
    /// Replace the content of nU3GridControl.cs with this (keeping InU3Control interface)
    /// </summary>
    // [ToolboxItem(true)] // Uncomment if replacing the original file
    public class nU3GridControlExtended : GridControl, InU3Control
    {
        public nU3GridControlExtended() : base() 
        {
            this.UseEmbeddedNavigator = false;
        }

        // [Factory Override] Use nU3GridView as the default view
        protected override BaseView CreateDefaultView()
        {
            return new nU3GridView(this);
        }

        // [Factory Override] Register nU3GridView
        protected override void RegisterAvailableViewsCore(InfoCollection collection)
        {
            base.RegisterAvailableViewsCore(collection);
            collection.Add(new nU3GridViewInfoRegistrator());
        }

        #region InU3Control Implementation
        public object? GetValue() => this.DataSource;
        public void SetValue(object? value) => this.DataSource = value;
        public void Clear() => this.DataSource = null;
        public string GetControlId() => this.Name;
        #endregion
    }


    // =================================================================================================
    // 2. nU3TreeList (TreeList Chain)
    // =================================================================================================

    public class nU3TreeListColumn : TreeListColumn
    {
        public nU3TreeListColumn() : base() { }

        [Category("nU3 Framework")]
        public string AuthId { get; set; } = string.Empty;
    }

    // [ToolboxItem(true)]
    public class nU3TreeListExtended : TreeList, InU3Control
    {
        public nU3TreeListExtended() : base() 
        {
            this.OptionsView.ShowColumns = true;
        }

        // [Factory Override] Create nU3TreeListColumn
        protected override TreeListColumn CreateColumn()
        {
            return new nU3TreeListColumn();
        }

        #region InU3Control Implementation
        public object? GetValue() => this.DataSource;
        public void SetValue(object? value) => this.DataSource = value;
        public void Clear() => this.DataSource = null;
        public string GetControlId() => this.Name;
        #endregion
    }


    // =================================================================================================
    // 3. nU3VGridControl (VerticalGrid Chain)
    // =================================================================================================

    // VerticalGrid is harder to subclass rows directly via factory, 
    // mostly relies on RepositoryItems for editing. 
    // However, we can wrap the control itself.

    // [ToolboxItem(true)]
    public class nU3VGridControlExtended : VGridControl, InU3Control
    {
        public nU3VGridControlExtended() : base() { }

        #region InU3Control Implementation
        public object? GetValue() => this.DataSource;
        public void SetValue(object? value) => this.DataSource = value;
        public void Clear() => this.DataSource = null;
        public string GetControlId() => this.Name;
        #endregion
    }


    // =================================================================================================
    // 4. nU3TextEdit (RepositoryItem Chain)
    // =================================================================================================

    [UserRepositoryItem("RegisternU3TextEdit")]
    public class nU3RepositoryItemTextEdit : RepositoryItemTextEdit
    {
        static nU3RepositoryItemTextEdit() { RegisternU3TextEdit(); }
        public const string CustomEditName = "nU3TextEdit";
        public override string EditorTypeName => CustomEditName;

        public static void RegisternU3TextEdit()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(
                CustomEditName, typeof(nU3TextEditExtended), typeof(nU3RepositoryItemTextEdit),
                typeof(DevExpress.XtraEditors.ViewInfo.TextEditViewInfo), new DevExpress.XtraEditors.Drawing.TextEditPainter(), true));
        }

        [Category("nU3 Framework")]
        public bool IsRequired { get; set; } = false;
    }

    // [ToolboxItem(true)]
    public class nU3TextEditExtended : TextEdit, InU3Control
    {
        static nU3TextEditExtended() { nU3RepositoryItemTextEdit.RegisternU3TextEdit(); }
        public override string EditorTypeName => nU3RepositoryItemTextEdit.CustomEditName;

        protected override RepositoryItem CreateRepositoryItem()
        {
            return new nU3RepositoryItemTextEdit();
        }

        #region InU3Control Implementation
        public object? GetValue() => this.EditValue;
        public void SetValue(object? value) => this.EditValue = value;
        public void Clear() => this.EditValue = null;
        public string GetControlId() => this.Name;
        #endregion
    }


    // =================================================================================================
    // 5. nU3DateEdit (RepositoryItem Chain)
    // =================================================================================================

    [UserRepositoryItem("RegisternU3DateEdit")]
    public class nU3RepositoryItemDateEdit : RepositoryItemDateEdit
    {
        static nU3RepositoryItemDateEdit() { RegisternU3DateEdit(); }
        public const string CustomEditName = "nU3DateEdit";
        public override string EditorTypeName => CustomEditName;

        public static void RegisternU3DateEdit()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(
                CustomEditName, typeof(nU3DateEditExtended), typeof(nU3RepositoryItemDateEdit),
                typeof(DevExpress.XtraEditors.ViewInfo.DateEditViewInfo), new DevExpress.XtraEditors.Drawing.DateEditPainter(), true));
        }
    }

    // [ToolboxItem(true)]
    public class nU3DateEditExtended : DateEdit, InU3Control
    {
        static nU3DateEditExtended() { nU3RepositoryItemDateEdit.RegisternU3DateEdit(); }
        public override string EditorTypeName => nU3RepositoryItemDateEdit.CustomEditName;

        protected override RepositoryItem CreateRepositoryItem()
        {
            return new nU3RepositoryItemDateEdit();
        }

        #region InU3Control Implementation
        public object? GetValue() => this.EditValue;
        public void SetValue(object? value) => this.EditValue = value;
        public void Clear() => this.EditValue = null;
        public string GetControlId() => this.Name;
        #endregion
    }


    // =================================================================================================
    // 6. nU3LookUpEdit (RepositoryItem Chain)
    // =================================================================================================

    [UserRepositoryItem("RegisternU3LookUpEdit")]
    public class nU3RepositoryItemLookUpEdit : RepositoryItemLookUpEdit
    {
        static nU3RepositoryItemLookUpEdit() { RegisternU3LookUpEdit(); }
        public const string CustomEditName = "nU3LookUpEdit";
        public override string EditorTypeName => CustomEditName;

        public static void RegisternU3LookUpEdit()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(
                CustomEditName, typeof(nU3LookUpEditExtended), typeof(nU3RepositoryItemLookUpEdit),
                typeof(DevExpress.XtraEditors.ViewInfo.LookUpEditViewInfo), new DevExpress.XtraEditors.Drawing.LookUpEditPainter(), true));
        }
    }

    // [ToolboxItem(true)]
    public class nU3LookUpEditExtended : LookUpEdit, InU3Control
    {
        static nU3LookUpEditExtended() { nU3RepositoryItemLookUpEdit.RegisternU3LookUpEdit(); }
        public override string EditorTypeName => nU3RepositoryItemLookUpEdit.CustomEditName;

        protected override RepositoryItem CreateRepositoryItem()
        {
            return new nU3RepositoryItemLookUpEdit();
        }

        #region InU3Control Implementation
        public object? GetValue() => this.EditValue;
        public void SetValue(object? value) => this.EditValue = value;
        public void Clear() => this.EditValue = null;
        public string GetControlId() => this.Name;
        #endregion
    }
}
