using System;
using System.ComponentModel;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;

namespace nU3.Core.UI.Controls
{
    #region 1. TreeList Chain

    /// <summary>
    /// nU3 Framework 전용 TreeList 컬럼
    /// </summary>
    public class nU3TreeListColumn : TreeListColumn
    {
        public nU3TreeListColumn() : base() { }

        [Category("nU3 Framework")]
        [Description("컬럼 권한 제어를 위한 고유 ID")]
        public string AuthId { get; set; } = string.Empty;
    }

    /// <summary>
    /// nU3 Framework 전용 TreeList 컬럼 컬렉션
    /// </summary>
    public class nU3TreeListColumnCollection : TreeListColumnCollection
    {
        public nU3TreeListColumnCollection(TreeList treeList) : base(treeList) { }

        // [Factory Override] nU3TreeListColumn 생성
        protected override TreeListColumn CreateColumn()
        {
            return new nU3TreeListColumn();
        }
    }

    /// <summary>
    /// nU3 Framework 표준 TreeList 컨트롤
    /// </summary>
    [ToolboxItem(true)]
    public class nU3TreeList : TreeList, InU3Control
    {
        public nU3TreeList() : base() 
        {
            this.OptionsView.ShowColumns = true;
        }

        // [Factory Override] nU3TreeListColumnCollection 생성
        protected override TreeListColumnCollection CreateColumns()
        {
            return new nU3TreeListColumnCollection(this);
        }

        #region InU3Control Implementation
        public object? GetValue() => this.DataSource;
        public void SetValue(object? value) => this.DataSource = value;
        public void Clear() => this.DataSource = null;
        public string GetControlId() => this.Name;
        #endregion
    }

    #endregion

    #region 2. VerticalGrid Chain

    /// <summary>
    /// nU3 Framework 표준 VerticalGrid 컨트롤
    /// </summary>
    [ToolboxItem(true)]
    public class nU3VGridControl : VGridControl, InU3Control
    {
        public nU3VGridControl() : base() { }

        #region InU3Control Implementation
        public object? GetValue() => this.DataSource;
        public void SetValue(object? value) => this.DataSource = value;
        public void Clear() => this.DataSource = null;
        public string GetControlId() => this.Name;
        #endregion
    }

    /// <summary>
    /// nU3 Framework 표준 PropertyGrid 컨트롤
    /// </summary>
    [ToolboxItem(true)]
    public class nU3PropertyGridControl : PropertyGridControl, InU3Control
    {
        public nU3PropertyGridControl() : base() { }

        #region InU3Control Implementation
        public object? GetValue() => this.SelectedObject;
        public void SetValue(object? value) => this.SelectedObject = value;
        public void Clear() => this.SelectedObject = null;
        public string GetControlId() => this.Name;
        #endregion
    }

    #endregion

    #region 3. PivotGrid Chain

    /// <summary>
    /// nU3 Framework 전용 피벗 그리드 필드
    /// </summary>
    public class nU3PivotGridField : PivotGridField
    {
        public nU3PivotGridField() : base() { }
        public nU3PivotGridField(string fieldName, PivotArea area) : base(fieldName, area) { }

        [Category("nU3 Framework")]
        [Description("개인정보 포함 여부")]
        public bool IsPersonalData { get; set; } = false;
    }

    /// <summary>
    /// nU3 Framework 표준 피벗 그리드 컨트롤
    /// </summary>
    [ToolboxItem(true)]
    public class nU3PivotGridControl : PivotGridControl, InU3Control
    {
        public nU3PivotGridControl() : base() { }

        protected override PivotGridViewInfoData CreateData()
        {
            return new nU3PivotGridData(this);
        }

        #region InU3Control Implementation
        public object? GetValue() => this.DataSource;
        public void SetValue(object? value) => this.DataSource = value;
        public void Clear() => this.DataSource = null;
        public string GetControlId() => this.Name;
        #endregion
    }

    /// <summary>
    /// PivotGrid 내부 데이터 처리 클래스 (Field 생성 지원)
    /// </summary>
    public class nU3PivotGridData : PivotGridViewInfoData
    {
        public nU3PivotGridData(PivotGridControl control) : base(control) { }
    }

    #endregion
}