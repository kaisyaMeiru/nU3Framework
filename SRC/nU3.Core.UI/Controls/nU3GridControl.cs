using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace nU3.Core.UI.Controls
{
    /// <summary>
    /// nU3 Framework 전용 그리드 컬럼
    /// 권한 ID 및 다국어 리소스 키를 지원합니다.
    /// </summary>
    public class nU3GridColumn : GridColumn
    {
        public nU3GridColumn() : base() { }

        [Category("nU3 Framework")]
        [Description("컬럼 권한 제어를 위한 고유 ID")]
        public string AuthId { get; set; } = string.Empty;

        [Category("nU3 Framework")]
        [Description("다국어 처리를 위한 리소스 키")]
        public string ResourceKey { get; set; } = string.Empty;
    }

    /// <summary>
    /// nU3 Framework 전용 그리드 컬럼 컬렉션
    /// </summary>
    public class nU3GridColumnCollection : GridColumnCollection
    {
        public nU3GridColumnCollection(ColumnView view) : base(view) { }

        // [Factory Override] nU3GridColumn을 생성하도록 설정
        protected override GridColumn CreateColumn()
        {
            return new nU3GridColumn();
        }
    }

    /// <summary>
    /// nU3 Framework 전용 그리드 뷰
    /// 기본적으로 그룹 패널을 숨기고 행 선택 모드를 설정합니다.
    /// </summary>
    public class nU3GridView : GridView
    {
        public nU3GridView() : base() { InitializeView(); }
        public nU3GridView(GridControl grid) : base(grid) { InitializeView(); }

        private void InitializeView()
        {
            this.OptionsView.ShowGroupPanel = false;
            this.OptionsSelection.MultiSelect = true;
            this.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;
        }

        // [Factory Override] nU3GridColumnCollection을 생성하도록 설정
        protected override GridColumnCollection CreateColumnCollection()
        {
            return new nU3GridColumnCollection(this);
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
        }
    }

    /// <summary>
    /// nU3GridView를 VS Designer에 등록하기 위한 클래스
    /// </summary>
    public class nU3GridViewInfoRegistrator : GridInfoRegistrator
    {
        public override string ViewName => "nU3GridView";
        public override BaseView CreateView(GridControl grid) => new nU3GridView(grid);
    }

    /// <summary>
    /// nU3 Framework 표준 그리드 컨트롤
    /// nU3GridView를 기본 뷰로 사용하며 InU3Control 인터페이스를 구현합니다.
    /// </summary>
    [ToolboxItem(true)]
    public class nU3GridControl : GridControl, InU3Control
    {
        public nU3GridControl() : base() 
        {
            this.UseEmbeddedNavigator = false;
        }

        // [Factory Override] 기본 뷰를 nU3GridView로 설정
        protected override BaseView CreateDefaultView()
        {
            return new nU3GridView(this);
        }

        // [Factory Override] nU3GridView를 사용 가능한 뷰 목록에 등록
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
}
