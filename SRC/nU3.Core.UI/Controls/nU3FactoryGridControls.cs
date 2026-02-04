using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.IO;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;

namespace nU3.Core.UI.Controls
{
    #region 1. nU Grid Control Chain

    /// <summary>
    /// nU Framework 전용 그리드 컬럼
    /// </summary>
    public class nUGridColumn : GridColumn
    {
        public nUGridColumn() : base() { }

        [Category("nU Framework")]
        [Description("권한 제어를 위한 고유 ID")]
        public string nUAuthID { get; set; } = string.Empty;

        [Category("nU Framework")]
        [Description("다국어 처리를 위한 사전 키")]
        public string nUDictionaryKey { get; set; } = string.Empty;
    }

    /// <summary>
    /// nU Framework 전용 그리드 뷰
    /// </summary>
    public class nUGridView : GridView
    {
        public nUGridView() : base() { }
        public nUGridView(GridControl grid) : base(grid) 
        {
            InitializeView();
        }

        private void InitializeView()
        {
            this.OptionsView.ShowGroupPanel = false;
            this.OptionsSelection.MultiSelect = true;
            this.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;
        }

        // [Factory Method Override] nUGridColumn을 생성하도록 가로채기
        protected override GridColumn CreateColumn()
        {
            return new nUGridColumn();
        }
    }

    /// <summary>
    /// nUGridView를 디자이너에 등록하기 위한 Registrator
    /// </summary>
    public class nUGridViewInfoRegistrator : GridInfoRegistrator
    {
        public override string ViewName => "nUGridView";
        
        public override BaseView CreateView(GridControl grid)
        {
            return new nUGridView(grid);
        }
    }

    /// <summary>
    /// nU Framework 표준 그리드 컨트롤
    /// </summary>
    [ToolboxItem(true)]
    public class nUGridControl : GridControl
    {
        public nUGridControl() : base()
        {
            this.UseEmbeddedNavigator = false;
        }

        // [Factory Method Override] 기본 뷰를 nUGridView로 설정
        protected override BaseView CreateDefaultView()
        {
            return new nUGridView(this);
        }

        // [Factory Method Override] 지원 가능한 뷰 목록에 nUGridView 등록
        protected override void RegisterAvailableViewsCore(InfoCollection collection)
        {
            base.RegisterAvailableViewsCore(collection);
            collection.Add(new nUGridViewInfoRegistrator());
        }
    }

    #endregion

    #region 2. nU Pivot Grid Control Chain

    /// <summary>
    /// nU Framework 전용 피벗 그리드 필드
    /// </summary>
    public class nUPivotGridField : PivotGridField
    {
        public nUPivotGridField() : base() { }
        public nUPivotGridField(string fieldName, PivotArea area) : base(fieldName, area) { }

        [Category("nU Framework")]
        public bool nUIsPersonalData { get; set; } = false;
    }

    /// <summary>
    /// nU Framework 표준 피벗 그리드 컨트롤
    /// </summary>
    [ToolboxItem(true)]
    public class nUPivotGridControl : PivotGridControl
    {
        public nUPivotGridControl() : base() { }

        // [Factory Method Override] nUPivotGridField를 생성하도록 가로채기
        protected override PivotGridField CreateField(string fieldName, PivotArea area)
        {
            return new nUPivotGridField(fieldName, area);
        }

        protected override PivotGridData CreateData()
        {
            return new nUPivotGridData(this);
        }
    }

    public class nUPivotGridData : PivotGridData
    {
        public nUPivotGridData(PivotGridControl control) : base(control) { }

        public override PivotGridField CreateField(string fieldName, PivotArea area)
        {
            return new nUPivotGridField(fieldName, area);
        }
    }

    #endregion

    #region 3. nU TextEdit Control Chain (In-place Editor 지원)

    /// <summary>
    /// nU Framework 전용 RepositoryItem (그리드 내부 편집기 및 컨트롤 속성 정의)
    /// </summary>
    [UserRepositoryItem("RegisternUTextEdit")]
    public class nURepositoryItemTextEdit : RepositoryItemTextEdit
    {
        static nURepositoryItemTextEdit()
        {
            RegisternUTextEdit();
        }

        public const string CustomEditName = "nUTextEdit";
        public override string EditorTypeName => CustomEditName;

        public static void RegisternUTextEdit()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(
                CustomEditName,
                typeof(nUTextEdit),
                typeof(nURepositoryItemTextEdit),
                typeof(DevExpress.XtraEditors.ViewInfo.TextEditViewInfo),
                new DevExpress.XtraEditors.Drawing.TextEditPainter(),
                true));
        }

        [Category("nU Framework")]
        public bool nUIsRequired { get; set; } = false;
    }

    /// <summary>
    /// nU Framework 표준 텍스트 에디터
    /// </summary>
    [ToolboxItem(true)]
    public class nUTextEdit : TextEdit
    {
        static nUTextEdit()
        {
            nURepositoryItemTextEdit.RegisternUTextEdit();
        }

        public nUTextEdit() : base() { }

        public override string EditorTypeName => nURepositoryItemTextEdit.CustomEditName;

        // [Factory Method Override] 전용 RepositoryItem 연결
        protected override RepositoryItem CreateRepositoryItem()
        {
            return new nURepositoryItemTextEdit();
        }
    }

    #endregion
}
