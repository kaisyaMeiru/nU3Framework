#pragma warning disable CS8669

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace nU3.Modules.EMR.CL.Component
{
    partial class ClinicStatsControl
    {
        private System.ComponentModel.IContainer _components = null;

        private DevExpress.XtraLayout.LayoutControl _layoutControl = null!;
        private DevExpress.XtraEditors.LabelControl _lblStatus = null!;
        private DevExpress.XtraEditors.MemoEdit _txtEventLog = null!;
        private DevExpress.XtraGrid.GridControl _gridStats = null!;
        private DevExpress.XtraGrid.Views.Grid.GridView _gridViewStats = null!;

        private DevExpress.XtraLayout.LayoutControlGroup _rootGroup = null!;
        private DevExpress.XtraLayout.LayoutControlGroup _statusGroup = null!;
        private DevExpress.XtraLayout.LayoutControlItem _statusItem = null!;
        private DevExpress.XtraLayout.LayoutControlGroup _statsGroup = null!;
        private DevExpress.XtraLayout.LayoutControlItem _statsItem = null!;
        private DevExpress.XtraLayout.LayoutControlGroup _logGroup = null!;
        private DevExpress.XtraLayout.LayoutControlItem _logItem = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
            {
                _components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            _layoutControl = new LayoutControl();
            _lblStatus = new LabelControl();
            _gridStats = new GridControl();
            _gridViewStats = new GridView();
            _txtEventLog = new MemoEdit();
            _rootGroup = new LayoutControlGroup();
            _statusGroup = new LayoutControlGroup();
            _statusItem = new LayoutControlItem();
            _statsGroup = new LayoutControlGroup();
            _statsItem = new LayoutControlItem();
            _logGroup = new LayoutControlGroup();
            _logItem = new LayoutControlItem();
            ((ISupportInitialize)_layoutControl).BeginInit();
            _layoutControl.SuspendLayout();
            ((ISupportInitialize)_gridStats).BeginInit();
            ((ISupportInitialize)_gridViewStats).BeginInit();
            ((ISupportInitialize)_txtEventLog.Properties).BeginInit();
            ((ISupportInitialize)_rootGroup).BeginInit();
            ((ISupportInitialize)_statusGroup).BeginInit();
            ((ISupportInitialize)_statusItem).BeginInit();
            ((ISupportInitialize)_statsGroup).BeginInit();
            ((ISupportInitialize)_statsItem).BeginInit();
            ((ISupportInitialize)_logGroup).BeginInit();
            ((ISupportInitialize)_logItem).BeginInit();
            SuspendLayout();
            // 
            // _layoutControl
            // 
            _layoutControl.Controls.Add(_lblStatus);
            _layoutControl.Controls.Add(_gridStats);
            _layoutControl.Controls.Add(_txtEventLog);
            _layoutControl.Dock = DockStyle.Fill;
            _layoutControl.Location = new Point(0, 0);
            _layoutControl.Name = "_layoutControl";
            _layoutControl.Root = _rootGroup;
            _layoutControl.Size = new Size(1528, 793);
            _layoutControl.TabIndex = 0;
            _layoutControl.Text = "layoutControl1";
            // 
            // _lblStatus
            // 
            _lblStatus.Appearance.FontSizeDelta = 2;
            _lblStatus.Appearance.ForeColor = Color.Gray;
            _lblStatus.Appearance.Options.UseFont = true;
            _lblStatus.Appearance.Options.UseForeColor = true;
            _lblStatus.Location = new Point(24, 43);
            _lblStatus.Name = "_lblStatus";
            _lblStatus.Size = new Size(75, 18);
            _lblStatus.StyleController = _layoutControl;
            _lblStatus.TabIndex = 4;
            _lblStatus.Text = "상태: 대기중";
            // 
            // _gridStats
            // 
            _gridStats.Dock = DockStyle.Fill;
            _gridStats.Location = new Point(24, 108);
            _gridStats.MainView = _gridViewStats;
            _gridStats.Name = "_gridStats";
            _gridStats.Size = new Size(1480, 406);
            _gridStats.TabIndex = 5;
            _gridStats.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { _gridViewStats });
            // 
            // _gridViewStats
            // 
            _gridViewStats.DetailHeight = 327;
            _gridViewStats.GridControl = _gridStats;
            _gridViewStats.Name = "_gridViewStats";
            _gridViewStats.OptionsView.ShowAutoFilterRow = true;
            // 
            // _txtEventLog
            // 
            _txtEventLog.Dock = DockStyle.Fill;
            _txtEventLog.Location = new Point(24, 561);
            _txtEventLog.Name = "_txtEventLog";
            _txtEventLog.Properties.ReadOnly = true;
            _txtEventLog.Size = new Size(1480, 210);
            _txtEventLog.StyleController = _layoutControl;
            _txtEventLog.TabIndex = 6;
            // 
            // _rootGroup
            // 
            _rootGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            _rootGroup.GroupBordersVisible = false;
            _rootGroup.Items.AddRange(new BaseLayoutItem[] { _statusGroup, _statsGroup, _logGroup });
            _rootGroup.Name = "_rootGroup";
            _rootGroup.Size = new Size(1528, 793);
            _rootGroup.TextVisible = false;
            // 
            // _statusGroup
            // 
            _statusGroup.Items.AddRange(new BaseLayoutItem[] { _statusItem });
            _statusGroup.Location = new Point(0, 0);
            _statusGroup.Name = "_statusGroup";
            _statusGroup.Size = new Size(1508, 65);
            _statusGroup.Text = "상태";
            // 
            // _statusItem
            // 
            _statusItem.Control = _lblStatus;
            _statusItem.Location = new Point(0, 0);
            _statusItem.Name = "_statusItem";
            _statusItem.Size = new Size(1484, 22);
            _statusItem.TextSize = new Size(0, 0);
            _statusItem.TextVisible = false;
            // 
            // _statsGroup
            // 
            _statsGroup.Items.AddRange(new BaseLayoutItem[] { _statsItem });
            _statsGroup.Location = new Point(0, 65);
            _statsGroup.Name = "_statsGroup";
            _statsGroup.Size = new Size(1508, 453);
            _statsGroup.Text = "진료 통계";
            // 
            // _statsItem
            // 
            _statsItem.Control = _gridStats;
            _statsItem.Location = new Point(0, 0);
            _statsItem.MinSize = new Size(1, 280);
            _statsItem.Name = "_statsItem";
            _statsItem.Size = new Size(1484, 410);
            _statsItem.SizeConstraintsType = SizeConstraintsType.Custom;
            _statsItem.TextSize = new Size(0, 0);
            _statsItem.TextVisible = false;
            // 
            // _logGroup
            // 
            _logGroup.Items.AddRange(new BaseLayoutItem[] { _logItem });
            _logGroup.Location = new Point(0, 518);
            _logGroup.Name = "_logGroup";
            _logGroup.Size = new Size(1508, 257);
            _logGroup.Text = "이벤트 로그";
            // 
            // _logItem
            // 
            _logItem.Control = _txtEventLog;
            _logItem.Location = new Point(0, 0);
            _logItem.MinSize = new Size(1, 140);
            _logItem.Name = "_logItem";
            _logItem.Size = new Size(1484, 214);
            _logItem.SizeConstraintsType = SizeConstraintsType.Custom;
            _logItem.TextSize = new Size(0, 0);
            _logItem.TextVisible = false;
            // 
            // ClinicStatsControl
            // 
            AutoScaleDimensions = new SizeF(7F, 14F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(_layoutControl);
            Name = "ClinicStatsControl";
            Size = new Size(1528, 793);
            ((ISupportInitialize)_layoutControl).EndInit();
            _layoutControl.ResumeLayout(false);
            ((ISupportInitialize)_gridStats).EndInit();
            ((ISupportInitialize)_gridViewStats).EndInit();
            ((ISupportInitialize)_txtEventLog.Properties).EndInit();
            ((ISupportInitialize)_rootGroup).EndInit();
            ((ISupportInitialize)_statusGroup).EndInit();
            ((ISupportInitialize)_statusItem).EndInit();
            ((ISupportInitialize)_statsGroup).EndInit();
            ((ISupportInitialize)_statsItem).EndInit();
            ((ISupportInitialize)_logGroup).EndInit();
            ((ISupportInitialize)_logItem).EndInit();
            ResumeLayout(false);

        }

        #endregion
    }
}
