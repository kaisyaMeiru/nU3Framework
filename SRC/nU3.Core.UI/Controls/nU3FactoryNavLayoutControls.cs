using System;
using System.ComponentModel;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraLayout;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;

namespace nU3.Core.UI.Controls
{
    // =================================================================================================
    // 1. nU3RibbonControl (Ribbon Chain)
    // =================================================================================================

    public class nU3RibbonPage : RibbonPage
    {
        public nU3RibbonPage() : base() { }
        public nU3RibbonPage(string text) : base(text) { }

        [Category("nU3 Framework")]
        public string AuthId { get; set; } = string.Empty;
    }

    // [ToolboxItem(true)]
    public class nU3RibbonControlExtended : RibbonControl, InU3Control
    {
        public nU3RibbonControlExtended() : base() 
        {
            // Default nU3 settings
            this.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
        }

        // [Factory Override] Create nU3RibbonPage
        protected override RibbonPage CreatePage(string text)
        {
            return new nU3RibbonPage(text);
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


    // =================================================================================================
    // 2. nU3AccordionControl (Accordion Chain)
    // =================================================================================================

    public class nU3AccordionControlElement : AccordionControlElement
    {
        public nU3AccordionControlElement() : base() { }
        
        [Category("nU3 Framework")]
        public string AuthId { get; set; } = string.Empty;
    }

    // [ToolboxItem(true)]
    public class nU3AccordionControlExtended : AccordionControl, InU3Control
    {
        public nU3AccordionControlExtended() : base() { }

        // [Factory Override] Create nU3AccordionControlElement
        protected override AccordionControlElement CreateElement(AccordionControlElementStyle style)
        {
            var element = new nU3AccordionControlElement();
            element.Style = style;
            return element;
        }

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


    // =================================================================================================
    // 3. nU3OfficeNavigationBar (Navigation Bar Chain)
    // =================================================================================================

    public class nU3NavigationBarItem : NavigationBarItem
    {
        public nU3NavigationBarItem() : base() { }

        [Category("nU3 Framework")]
        public string AuthId { get; set; } = string.Empty;
    }

    // [ToolboxItem(true)]
    public class nU3OfficeNavigationBarExtended : OfficeNavigationBar, InU3Control
    {
        public nU3OfficeNavigationBarExtended() : base() { }

        // [Factory Override] Create nU3NavigationBarItem
        protected override NavigationBarItem CreateItem()
        {
            return new nU3NavigationBarItem();
        }

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


    // =================================================================================================
    // 4. nU3LayoutControl (LayoutControl Chain)
    // =================================================================================================

    public class nU3LayoutControlGroup : LayoutControlGroup
    {
        public nU3LayoutControlGroup() : base() { }
        
        [Category("nU3 Framework")]
        public bool IsCollapsibleDefault { get; set; } = true;
    }

    // [ToolboxItem(true)]
    public class nU3LayoutControlExtended : LayoutControl, InU3Control
    {
        public nU3LayoutControlExtended() : base() { }

        // [Factory Override] Create nU3LayoutControlGroup for the root
        protected override LayoutControlGroup CreateRootGroup()
        {
            return new nU3LayoutControlGroup();
        }

        // Note: For child groups, LayoutControl architecture is more complex to override completely via factory methods 
        // without custom Implementor, but overriding Root is the most critical start.

        #region InU3Control Implementation
        public object? GetValue() => null;
        public void SetValue(object? value) { }
        public void Clear() { }
        public string GetControlId() => this.Name;
        #endregion
    }


    // =================================================================================================
    // 5. nU3GroupControl (GroupControl Chain)
    // =================================================================================================

    // [ToolboxItem(true)]
    public class nU3GroupControlExtended : GroupControl, InU3Control
    {
        public nU3GroupControlExtended() : base() 
        {
            // nU3 Standard: Bold text for groups
            this.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
        }

        #region InU3Control Implementation
        public object? GetValue() => this.Text;
        public void SetValue(object? value) => this.Text = value?.ToString();
        public void Clear() { }
        public string GetControlId() => this.Name;
        #endregion
    }


    // =================================================================================================
    // 6. nU3XtraTabControl (TabControl Chain)
    // =================================================================================================

    public class nU3XtraTabPage : XtraTabPage
    {
        public nU3XtraTabPage() : base() { }

        [Category("nU3 Framework")]
        public string AuthId { get; set; } = string.Empty;
    }

    // [ToolboxItem(true)]
    public class nU3XtraTabControlExtended : XtraTabControl, InU3Control
    {
        public nU3XtraTabControlExtended() : base() { }

        // [Factory Override] Create nU3XtraTabPage on "Add Tab"
        protected override XtraTabPage CreateTabPage()
        {
            return new nU3XtraTabPage();
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


    // =================================================================================================
    // 7. nU3SplitContainerControl
    // =================================================================================================

    // [ToolboxItem(true)]
    public class nU3SplitContainerControlExtended : SplitContainerControl, InU3Control
    {
        public nU3SplitContainerControlExtended() : base() 
        {
            this.FixedPanel = SplitFixedPanel.Panel1; // Default Standard
        }

        #region InU3Control Implementation
        public object? GetValue() => null;
        public void SetValue(object? value) { }
        public void Clear() { }
        public string GetControlId() => this.Name;
        #endregion
    }
}
