using System;
using System.ComponentModel;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraLayout;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using DevExpress.XtraGauges.Win;
using DevExpress.XtraMap;
using DevExpress.XtraPdfViewer;
using DevExpress.XtraTreeMap;
using DevExpress.XtraCharts;
using DevExpress.XtraWizard;
using DevExpress.XtraSpellChecker;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Docking2010;

namespace nU3.Core.UI.Controls
{
    // ... (Previous implementations) ...

    // =================================================================================================
    // 8. nU3GanttControl (Gantt Control Chain)
    // =================================================================================================
    
    // GanttControl is part of DevExpress.XtraGantt namespace, ensure reference is added.
    // Assuming reference is available or will be added.
    
    /* 
    public class nU3GanttControlExtended : DevExpress.XtraGantt.GanttControl, InU3Control
    {
        public nU3GanttControlExtended() : base() { }

        #region InU3Control Implementation
        public object? GetValue() => this.DataSource;
        public void SetValue(object? value) => this.DataSource = value;
        public void Clear() => this.DataSource = null;
        public string GetControlId() => this.Name;
        #endregion
    }
    */

    // =================================================================================================
    // 9. nU3DiagramControl (Diagram Control Chain)
    // =================================================================================================

    // DiagramControl is part of DevExpress.XtraDiagram namespace.
    
    /*
    public class nU3DiagramControlExtended : DevExpress.XtraDiagram.DiagramControl, InU3Control
    {
        public nU3DiagramControlExtended() : base() { }

        #region InU3Control Implementation
        public object? GetValue() => null; // Complex state
        public void SetValue(object? value) { }
        public void Clear() => this.Items.Clear();
        public string GetControlId() => this.Name;
        #endregion
    }
    */

    // =================================================================================================
    // 10. nU3GaugeControl (Gauge Control Chain)
    // =================================================================================================

    // [ToolboxItem(true)]
    public class nU3GaugeControlExtended : GaugeControl, InU3Control
    {
        public nU3GaugeControlExtended() : base() { }

        #region InU3Control Implementation
        public object? GetValue() => null; // Gauges hold values individually
        public void SetValue(object? value) { }
        public void Clear() { }
        public string GetControlId() => this.Name;
        #endregion
    }

    // =================================================================================================
    // 11. nU3MapControl (Map Control Chain)
    // =================================================================================================

    // [ToolboxItem(true)]
    public class nU3MapControlExtended : MapControl, InU3Control
    {
        public nU3MapControlExtended() : base() { }

        #region InU3Control Implementation
        public object? GetValue() => null; 
        public void SetValue(object? value) { }
        public void Clear() { } 
        public string GetControlId() => this.Name;
        #endregion
    }

    // =================================================================================================
    // 12. nU3PdfViewer (PDF Viewer Chain)
    // =================================================================================================

    // [ToolboxItem(true)]
    public class nU3PdfViewerExtended : PdfViewer, InU3Control
    {
        public nU3PdfViewerExtended() : base() 
        {
            this.DetachStreamAfterLoadComplete = true;
        }

        #region InU3Control Implementation
        public object? GetValue() => this.DocumentFilePath;
        public void SetValue(object? value) 
        {
            if (value is string path) this.LoadDocument(path);
            else if (value is System.IO.Stream stream) this.LoadDocument(stream);
        }
        public void Clear() => this.CloseDocument();
        public string GetControlId() => this.Name;
        #endregion
    }

    // =================================================================================================
    // 13. nU3TreeMapControl (TreeMap Chain)
    // =================================================================================================

    // [ToolboxItem(true)]
    public class nU3TreeMapControlExtended : TreeMapControl, InU3Control
    {
        public nU3TreeMapControlExtended() : base() { }

        #region InU3Control Implementation
        public object? GetValue() => this.DataSource;
        public void SetValue(object? value) => this.DataSource = value;
        public void Clear() => this.DataSource = null;
        public string GetControlId() => this.Name;
        #endregion
    }

    // =================================================================================================
    // 14. nU3WizardControl (Wizard Chain)
    // =================================================================================================

    // [ToolboxItem(true)]
    public class nU3WizardControlExtended : WizardControl, InU3Control
    {
        public nU3WizardControlExtended() : base() { }

        #region InU3Control Implementation
        public object? GetValue() => this.SelectedPage;
        public void SetValue(object? value) 
        {
            if (value is BaseWizardPage page) this.SelectedPage = page;
        }
        public void Clear() { }
        public string GetControlId() => this.Name;
        #endregion
    }

    // =================================================================================================
    // 15. nU3DockManager (Docking Library Chain)
    // =================================================================================================

    // DockManager is a Component, not a Control, but often managed centrally.
    public class nU3DockManagerExtended : DockManager
    {
        public nU3DockManagerExtended() : base() { }
        public nU3DockManagerExtended(IContainer container) : base(container) { }
    }

    // =================================================================================================
    // 16. nU3DocumentManager (Document Manager Chain)
    // =================================================================================================

    public class nU3DocumentManagerExtended : DocumentManager
    {
        public nU3DocumentManagerExtended() : base() { }
        public nU3DocumentManagerExtended(IContainer container) : base(container) { }
    }

}
