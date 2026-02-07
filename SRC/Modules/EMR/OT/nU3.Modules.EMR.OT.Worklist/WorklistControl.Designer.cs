using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace nU3.Modules.EMR.OT.Worklist
{
    public partial class WorklistControl
    {
        // UI Components
        private GridControl _gridControl;
        private GridView _gridView;
        private Panel _topPanel;
        private Button _btnSearch;
        private Label _lblTitle;

        private void InitializeComponent()
        {
            _topPanel = new Panel();
            _lblTitle = new Label();
            _btnSearch = new Button();
            _gridControl = new GridControl();
            _gridView = new GridView();
            _topPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_gridControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_gridView).BeginInit();
            SuspendLayout();
            // 
            // _topPanel
            // 
            _topPanel.Controls.Add(_lblTitle);
            _topPanel.Controls.Add(_btnSearch);
            _topPanel.Dock = DockStyle.Top;
            _topPanel.Location = new Point(0, 0);
            _topPanel.Name = "_topPanel";
            _topPanel.Size = new Size(2239, 100);
            _topPanel.TabIndex = 1;
            // 
            // _lblTitle
            // 
            _lblTitle.Dock = DockStyle.Left;
            _lblTitle.Location = new Point(0, 0);
            _lblTitle.Name = "_lblTitle";
            _lblTitle.Size = new Size(371, 100);
            _lblTitle.TabIndex = 0;
            _lblTitle.Text = "null";
            // 
            // _btnSearch
            // 
            _btnSearch.Dock = DockStyle.Fill;
            _btnSearch.Location = new Point(0, 0);
            _btnSearch.Name = "_btnSearch";
            _btnSearch.Size = new Size(2239, 100);
            _btnSearch.TabIndex = 1;
            _btnSearch.Text = "Search Patient";
            _btnSearch.Click += _btnSearch_Click1;
            // 
            // _gridControl
            // 
            _gridControl.Dock = DockStyle.Fill;
            _gridControl.Location = new Point(0, 100);
            _gridControl.MainView = _gridView;
            _gridControl.Name = "_gridControl";
            _gridControl.Size = new Size(2239, 1025);
            _gridControl.TabIndex = 0;
            _gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { _gridView });
            // 
            // _gridView
            // 
            _gridView.GridControl = _gridControl;
            _gridView.Name = "_gridView";
            // 
            // WorklistControl
            // 
            Controls.Add(_gridControl);
            Controls.Add(_topPanel);
            Name = "WorklistControl";
            Size = new Size(2239, 1125);
            _topPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_gridControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)_gridView).EndInit();
            ResumeLayout(false);
        }


    }
}
