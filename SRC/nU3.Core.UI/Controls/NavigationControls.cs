using System;
using System.ComponentModel;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Navigation;

namespace nU3.Core.UI.Controls
{
    [ToolboxItem(true)]
    public class nU3RibbonControl : RibbonControl, InU3Control
    {
        public object? GetValue() => this.SelectedPage;
        public void SetValue(object? value) 
        {
            if (value is RibbonPage page) this.SelectedPage = page;
        }
        public void Clear() { }
        public string GetControlId() => this.Name;
    }

    [ToolboxItem(true)]
    public class nU3AccordionControl : AccordionControl, InU3Control
    {
        public object? GetValue() => this.SelectedElement;
        public void SetValue(object? value) 
        {
            if (value is AccordionControlElement el) this.SelectedElement = el;
        }
        public void Clear() { }
        public string GetControlId() => this.Name;
    }
    
    [ToolboxItem(true)]
    public class nU3OfficeNavigationBar : OfficeNavigationBar, InU3Control
    {
         public object? GetValue() => this.SelectedItem;
         public void SetValue(object? value) 
         {
             if (value is NavigationBarItem item) this.SelectedItem = item;
         }
         public void Clear() => this.SelectedItem = null;
         public string GetControlId() => this.Name;
    }
}
