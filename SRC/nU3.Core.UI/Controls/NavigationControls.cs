using System;
using System.ComponentModel;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Navigation;

namespace nU3.Core.UI.Controls
{
    #region 1. Ribbon Chain

    /// <summary>
    /// nU3 Framework 전용 Ribbon Page
    /// </summary>
    public class nU3RibbonPage : RibbonPage
    {
        public nU3RibbonPage() : base() { }
        public nU3RibbonPage(string text) : base(text) { }

        [Category("nU3 Framework")]
        [Description("페이지 권한 제어를 위한 고유 ID")]
        public string AuthId { get; set; } = string.Empty;
    }

    /// <summary>
    /// nU3 Framework 표준 Ribbon 컨트롤
    /// </summary>
    [ToolboxItem(true)]
    public class nU3RibbonControl : RibbonControl, InU3Control
    {
        public nU3RibbonControl() : base() 
        {
            this.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
        }

        #region InU3Control Implementation
        public object? GetValue() => this.SelectedPage;
        public void SetValue(object? value) 
        {
            if (value is RibbonPage page) this.SelectedPage = page;
        }
        public void Clear() { }
        public string GetControlId() => this.Name;
        #endregion
    }

    #endregion

    #region 2. Accordion Chain

    /// <summary>
    /// nU3 Framework 전용 Accordion Element
    /// </summary>
    public class nU3AccordionControlElement : AccordionControlElement
    {
        public nU3AccordionControlElement() : base() { }
        
        [Category("nU3 Framework")]
        [Description("엘리먼트 권한 제어를 위한 고유 ID")]
        public string AuthId { get; set; } = string.Empty;
    }

    /// <summary>
    /// nU3 Framework 표준 Accordion 컨트롤
    /// </summary>
    [ToolboxItem(true)]
    public class nU3AccordionControl : AccordionControl, InU3Control
    {
        public nU3AccordionControl() : base() { }

        #region InU3Control Implementation
        public object? GetValue() => this.SelectedElement;
        public void SetValue(object? value) 
        {
            if (value is AccordionControlElement el) this.SelectedElement = el;
        }
        public void Clear() { }
        public string GetControlId() => this.Name;
        #endregion
    }

    #endregion

    #region 3. OfficeNavigationBar Chain

    /// <summary>
    /// nU3 Framework 전용 Navigation Bar Item
    /// </summary>
    public class nU3NavigationBarItem : NavigationBarItem
    {
        public nU3NavigationBarItem() : base() { }

        [Category("nU3 Framework")]
        [Description("아이템 권한 제어를 위한 고유 ID")]
        public string AuthId { get; set; } = string.Empty;
    }

    /// <summary>
    /// nU3 Framework 표준 OfficeNavigationBar 컨트롤
    /// </summary>
    [ToolboxItem(true)]
    public class nU3OfficeNavigationBar : OfficeNavigationBar, InU3Control
    {
        public nU3OfficeNavigationBar() : base() { }

        #region InU3Control Implementation
        public object? GetValue() => this.SelectedItem;
        public void SetValue(object? value) 
        {
            if (value is NavigationBarItem item) this.SelectedItem = item;
        }
        public void Clear() => this.SelectedItem = null;
        public string GetControlId() => this.Name;
        #endregion
    }

    #endregion

    #region 4. Bar Items (Authorization Support)

    public class nU3BarButtonItem : BarButtonItem
    {
        public nU3BarButtonItem() : base() { }
        
        [Category("nU3 Framework")]
        public string AuthId { get; set; } = string.Empty;
    }

    public class nU3BarCheckItem : BarCheckItem
    {
        public nU3BarCheckItem() : base() { }

        [Category("nU3 Framework")]
        public string AuthId { get; set; } = string.Empty;
    }

    public class nU3BarSubItem : BarSubItem
    {
        public nU3BarSubItem() : base() { }

        [Category("nU3 Framework")]
        public string AuthId { get; set; } = string.Empty;
    }

    public class nU3BarEditItem : BarEditItem
    {
        public nU3BarEditItem() : base() { }

        [Category("nU3 Framework")]
        public string AuthId { get; set; } = string.Empty;
    }

    #endregion
}