using System;
using System.ComponentModel;
using DevExpress.XtraLayout;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using DevExpress.XtraDataLayout;

namespace nU3.Core.UI.Controls
{
    #region 1. LayoutControl Chain

    /// <summary>
    /// nU3 Framework 전용 Layout Group
    /// </summary>
    public class nU3LayoutControlGroup : LayoutControlGroup
    {
        public nU3LayoutControlGroup() : base() { }
        
        [Category("nU3 Framework")]
        public bool IsCollapsibleDefault { get; set; } = true;
    }

    /// <summary>
    /// nU3 Framework 전용 Layout Item (Padding 표준화)
    /// </summary>
    public class nU3LayoutControlItem : LayoutControlItem
    {
        public nU3LayoutControlItem() : base() { }
        public nU3LayoutControlItem(LayoutControl owner, System.Windows.Forms.Control control) : base(owner, control) 
        {
            InitializeItem();
        }

        private void InitializeItem()
        {
            // SI 표준 여백: 사방 2px
            this.Padding = new DevExpress.XtraLayout.Utils.Padding(2);
            this.TextToControlDistance = 5;
        }
    }

    /// <summary>
    /// nU3 Framework 표준 LayoutControl
    /// </summary>
    [ToolboxItem(true)]
    public class nU3LayoutControl : LayoutControl, InU3Control
    {
        public nU3LayoutControl() : base() { }

        // [Factory Override] AddGroup() 호출 시 nU3LayoutControlGroup 반환
        // ※ Root 초기화는 DevExpress 내부 경로(Implementor)이므로 Root 자체는 교체 불가
        public override LayoutGroup CreateLayoutGroup(LayoutGroup parent)
        {
            return new nU3LayoutControlGroup();
        }

        // [Factory Override] AddItem() 호출 시 nU3LayoutControlItem 반환
        public override BaseLayoutItem CreateLayoutItem(LayoutGroup parent)
        {
            return new nU3LayoutControlItem();
        }

        #region InU3Control Implementation
        public object? GetValue() => null;
        public void SetValue(object? value) { }
        public new void Clear() { }
        public string GetControlId() => this.Name;
        #endregion
    }

    /// <summary>
    /// nU3 Framework 표준 DataLayoutControl (메타데이터 기반 레이아웃)
    /// </summary>
    [ToolboxItem(true)]
    public class nU3DataLayoutControl : DataLayoutControl, InU3Control
    {
        public nU3DataLayoutControl() : base() 
        {
            this.AllowGeneratingNestedGroups = DevExpress.Utils.DefaultBoolean.True;
        }

        #region InU3Control Implementation
        public object? GetValue() => null;
        public void SetValue(object? value) { }
        public void Clear() { }
        public string GetControlId() => this.Name;
        #endregion
    }

    #endregion

    #region 2. Group & Panel

    /// <summary>
    /// nU3 Framework 표준 GroupControl
    /// </summary>
    [ToolboxItem(true)]
    public class nU3GroupControl : GroupControl, InU3Control
    {
        public nU3GroupControl() : base() 
        {
            this.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
        }

        #region InU3Control Implementation
        public object? GetValue() => this.Text;
        public void SetValue(object? value) => this.Text = value?.ToString();
        public void Clear() { }
        public string GetControlId() => this.Name;
        #endregion
    }

    /// <summary>
    /// nU3 Framework 표준 PanelControl
    /// </summary>
    [ToolboxItem(true)]
    public class nU3PanelControl : PanelControl, InU3Control
    {
        public nU3PanelControl() : base() { }

        #region InU3Control Implementation
        public object? GetValue() => null;
        public void SetValue(object? value) { }
        public void Clear() { }
        public string GetControlId() => this.Name;
        #endregion
    }

    #endregion

    #region 3. Tab & Splitter

    /// <summary>
    /// nU3 Framework 전용 XtraTabPage
    /// </summary>
    public class nU3XtraTabPage : XtraTabPage
    {
        public nU3XtraTabPage() : base() { }

        [Category("nU3 Framework")]
        public string AuthId { get; set; } = string.Empty;
    }

    /// <summary>
    /// nU3 Framework 전용 XtraTabPage 컬렉션
    /// </summary>
    public class nU3XtraTabPageCollection : XtraTabPageCollection
    {
        public nU3XtraTabPageCollection(XtraTabControl tabControl) : base(tabControl) { }

        protected override XtraTabPage CreatePage()
        {
            return new nU3XtraTabPage();
        }
    }

    /// <summary>
    /// nU3 Framework 표준 XtraTabControl
    /// </summary>
    [ToolboxItem(true)]
    public class nU3XtraTabControl : XtraTabControl, InU3Control
    {
        public nU3XtraTabControl() : base() { }

        // [Factory Override] 탭 컬렉션 생성 시 nU3XtraTabPageCollection 사용
        protected override XtraTabPageCollection CreateTabCollection()
        {
            return new nU3XtraTabPageCollection(this);
        }

        #region InU3Control Implementation
        public object? GetValue() => this.SelectedTabPage;
        public void SetValue(object? value) 
        {
            if (value is XtraTabPage page) this.SelectedTabPage = page;
        }
        public void Clear() { }
        public string GetControlId() => this.Name;
        #endregion
    }

    /// <summary>
    /// nU3 Framework 표준 SplitContainerControl
    /// </summary>
    [ToolboxItem(true)]
    public class nU3SplitContainerControl : SplitContainerControl, InU3Control
    {
         public nU3SplitContainerControl() : base() 
         {
             this.FixedPanel = SplitFixedPanel.Panel1;
         }

         #region InU3Control Implementation
         public object? GetValue() => null;
         public void SetValue(object? value) { }
         public void Clear() { }
         public string GetControlId() => this.Name;
         #endregion
    }

    #endregion
}