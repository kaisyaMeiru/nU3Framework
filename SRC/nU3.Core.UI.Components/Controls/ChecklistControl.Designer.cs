#pragma warning disable CS8669
#pragma warning disable CS0246
using DevExpress.XtraLayout;

namespace nU3.Core.UI.Components.Controls
{
    partial class ChecklistControl
    {
        private System.ComponentModel.IContainer? _components = null;

        private DevExpress.XtraLayout.LayoutControl? _layoutControl;
        private DevExpress.XtraEditors.SimpleButton? _checkAllButton;
        private DevExpress.XtraEditors.SimpleButton? _uncheckAllButton;
        private DevExpress.XtraEditors.SimpleButton? _clearButton;
        private DevExpress.XtraEditors.PanelControl? _buttonPanel;

        private DevExpress.XtraLayout.LayoutControlGroup? _rootGroup;
        private DevExpress.XtraLayout.LayoutControlGroup? _buttonGroup;
        private DevExpress.XtraLayout.LayoutControlItem? _checkAllButtonItem;
        private DevExpress.XtraLayout.LayoutControlItem? _uncheckAllButtonItem;
        private DevExpress.XtraLayout.LayoutControlItem? _clearButtonItem;

        private void InitializeComponent()
        {
            this._components = new System.ComponentModel.Container();
            this.SuspendLayout();

            this.Name = "ChecklistControl";
            this.Size = new System.Drawing.Size(400, 300);

            InitializeControls();

            this.ResumeLayout(false);
        }

        private void InitializeControls()
        {
            _layoutControl = new DevExpress.XtraLayout.LayoutControl();
            this._components.Add(_layoutControl);
            _layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
            _layoutControl.Parent = this;

            _rootGroup = _layoutControl.Root;

            if (ShowButtons)
            {
                InitializeButtonPanel();
            }
        }

        private void InitializeButtonPanel()
        {
            _buttonPanel = new DevExpress.XtraEditors.PanelControl();
            this._components.Add(_buttonPanel);
            _buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            _buttonPanel.Height = 40;
            _buttonPanel.Parent = this;

            _checkAllButton = new DevExpress.XtraEditors.SimpleButton();
            this._components.Add(_checkAllButton);
            _checkAllButton.Text = "전체선택";
            _checkAllButton.Width = 80;
            _checkAllButton.Location = new System.Drawing.Point(10, 8);
            _checkAllButton.Parent = _buttonPanel;
            _checkAllButton.Click += new System.EventHandler(this.OnCheckAll);

            _uncheckAllButton = new DevExpress.XtraEditors.SimpleButton();
            this._components.Add(_uncheckAllButton);
            _uncheckAllButton.Text = "전체해제";
            _uncheckAllButton.Width = 80;
            _uncheckAllButton.Location = new System.Drawing.Point(100, 8);
            _uncheckAllButton.Parent = _buttonPanel;
            _uncheckAllButton.Click += new System.EventHandler(this.OnUncheckAll);

            _clearButton = new DevExpress.XtraEditors.SimpleButton();
            this._components.Add(_clearButton);
            _clearButton.Text = "초기화";
            _clearButton.Width = 80;
            _clearButton.Location = new System.Drawing.Point(190, 8);
            _clearButton.Parent = _buttonPanel;
            _clearButton.Click += new System.EventHandler(this.OnClear);

            _buttonGroup = _rootGroup.AddGroup();
            _buttonGroup.Text = "동작";

            _checkAllButtonItem = _buttonGroup.AddItem();
            _checkAllButtonItem.Control = _checkAllButton;
            _checkAllButtonItem.TextVisible = false;

            _uncheckAllButtonItem = _buttonGroup.AddItem();
            _uncheckAllButtonItem.Control = _uncheckAllButton;
            _uncheckAllButtonItem.TextVisible = false;

            _clearButtonItem = _buttonGroup.AddItem();
            _clearButtonItem.Control = _clearButton;
            _clearButtonItem.TextVisible = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
            {
                _components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
