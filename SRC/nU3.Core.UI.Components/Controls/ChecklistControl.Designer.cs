#pragma warning disable CS8669
#pragma warning disable CS0246
using DevExpress.XtraLayout;
using nU3.Core.UI.Controls;

namespace nU3.Core.UI.Components.Controls
{
    partial class ChecklistControl
    {
        private System.ComponentModel.IContainer? _components = null;

        private nU3.Core.UI.Controls.nU3LayoutControl? _layoutControl;
        private nU3.Core.UI.Controls.nU3SimpleButton? _checkAllButton;
        private nU3.Core.UI.Controls.nU3SimpleButton? _uncheckAllButton;
        private nU3.Core.UI.Controls.nU3SimpleButton? _clearButton;
        private nU3.Core.UI.Controls.nU3PanelControl? _buttonPanel;

        private LayoutControlGroup? _rootGroup;
        private LayoutControlGroup? _buttonGroup;
        private LayoutControlItem? _checkAllButtonItem;
        private LayoutControlItem? _uncheckAllButtonItem;
        private LayoutControlItem? _clearButtonItem;

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // ChecklistControl
            // 
            Name = "ChecklistControl";
            Size = new Size(1463, 931);
            ResumeLayout(false);
        }

        private void InitializeControls()
        {
            _layoutControl = new nU3.Core.UI.Controls.nU3LayoutControl();
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
            _buttonPanel = new nU3.Core.UI.Controls.nU3PanelControl();
            this._components.Add(_buttonPanel);
            _buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            _buttonPanel.Height = 40;
            _buttonPanel.Parent = this;

            _checkAllButton = new nU3.Core.UI.Controls.nU3SimpleButton();
            this._components.Add(_checkAllButton);
            _checkAllButton.Text = "전체선택";
            _checkAllButton.Width = 80;
            _checkAllButton.Location = new System.Drawing.Point(10, 8);
            _checkAllButton.Parent = _buttonPanel;
            _checkAllButton.Click += new System.EventHandler(this.OnCheckAll);

            _uncheckAllButton = new nU3.Core.UI.Controls.nU3SimpleButton();
            this._components.Add(_uncheckAllButton);
            _uncheckAllButton.Text = "전체해제";
            _uncheckAllButton.Width = 80;
            _uncheckAllButton.Location = new System.Drawing.Point(100, 8);
            _uncheckAllButton.Parent = _buttonPanel;
            _uncheckAllButton.Click += new System.EventHandler(this.OnUncheckAll);

            _clearButton = new nU3.Core.UI.Controls.nU3SimpleButton();
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
