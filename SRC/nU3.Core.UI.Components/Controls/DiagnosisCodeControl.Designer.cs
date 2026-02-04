using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using System.ComponentModel;

namespace nU3.Core.UI.Components.Controls
{
    partial class DiagnosisCodeControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (disposing)
            {
                _codeGrid?.Dispose();
                _codeView?.Dispose();
                _selectedCodeGrid?.Dispose();
                _selectedCodeView?.Dispose();
                _searchControl?.Dispose();
                _codeEdit?.Dispose();
                _nameEdit?.Dispose();
                _addButton?.Dispose();
                _removeButton?.Dispose();
                _clearButton?.Dispose();
                _selectionPanel?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // _selectionPanel
            // 
            this._selectionPanel = new PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this._selectionPanel)).BeginInit();
            this._selectionPanel.SuspendLayout();
            this._selectionPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._selectionPanel.Location = new System.Drawing.Point(0, 300);
            this._selectionPanel.Name = "_selectionPanel";
            this._selectionPanel.Size = new System.Drawing.Size(700, 200);
            this._selectionPanel.TabIndex = 0;
            // 
            // _selectedLabel
            // 
            var _selectedLabel = new LabelControl();
            _selectedLabel.Location = new System.Drawing.Point(10, 10);
            _selectedLabel.Name = "_selectedLabel";
            _selectedLabel.Size = new System.Drawing.Size(112, 14);
            _selectedLabel.TabIndex = 0;
            _selectedLabel.Text = "선택된 진단코드:";
            this._selectionPanel.Controls.Add(_selectedLabel);
            // 
            // _codeEdit
            // 
            this._codeEdit = new TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this._codeEdit.Properties)).BeginInit();
            this._codeEdit.Location = new System.Drawing.Point(10, 40);
            this._codeEdit.Name = "_codeEdit";
            this._codeEdit.Properties.NullValuePrompt = "코드";
            this._codeEdit.Size = new System.Drawing.Size(100, 20);
            this._codeEdit.TabIndex = 1;
            ((System.ComponentModel.ISupportInitialize)(this._codeEdit.Properties)).EndInit();
            this._selectionPanel.Controls.Add(this._codeEdit);
            // 
            // _nameEdit
            // 
            this._nameEdit = new TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this._nameEdit.Properties)).BeginInit();
            this._nameEdit.Location = new System.Drawing.Point(120, 40);
            this._nameEdit.Name = "_nameEdit";
            this._nameEdit.Properties.NullValuePrompt = "진단명";
            this._nameEdit.Size = new System.Drawing.Size(400, 20);
            this._nameEdit.TabIndex = 2;
            ((System.ComponentModel.ISupportInitialize)(this._nameEdit.Properties)).EndInit();
            this._selectionPanel.Controls.Add(this._nameEdit);
            // 
            // _addButton
            // 
            this._addButton = new SimpleButton();
            this._addButton.Location = new System.Drawing.Point(530, 35);
            this._addButton.Name = "_addButton";
            this._addButton.Size = new System.Drawing.Size(80, 23);
            this._addButton.TabIndex = 3;
            this._addButton.Text = "추가";
            this._selectionPanel.Controls.Add(this._addButton);
            // 
            // _removeButton
            // 
            this._removeButton = new SimpleButton();
            this._removeButton.Location = new System.Drawing.Point(620, 35);
            this._removeButton.Name = "_removeButton";
            this._removeButton.Size = new System.Drawing.Size(80, 23);
            this._removeButton.TabIndex = 4;
            this._removeButton.Text = "삭제";
            this._selectionPanel.Controls.Add(this._removeButton);
            // 
            // _clearButton
            // 
            this._clearButton = new SimpleButton();
            this._clearButton.Location = new System.Drawing.Point(620, 75);
            this._clearButton.Name = "_clearButton";
            this._clearButton.Size = new System.Drawing.Size(80, 23);
            this._clearButton.TabIndex = 5;
            this._clearButton.Text = "전체삭제";
            this._selectionPanel.Controls.Add(this._clearButton);
            // 
            // _selectedCodeGrid
            // 
            this._selectedCodeGrid = new GridControl();
            ((System.ComponentModel.ISupportInitialize)(this._selectedCodeGrid)).BeginInit();
            this._selectedCodeGrid.Location = new System.Drawing.Point(10, 110);
            this._selectedCodeGrid.Name = "_selectedCodeGrid";
            this._selectedCodeGrid.Size = new System.Drawing.Size(690, 80);
            this._selectedCodeGrid.TabIndex = 6;
            // 
            // _selectedCodeView
            // 
            this._selectedCodeView = new GridView(this._selectedCodeGrid);
            ((System.ComponentModel.ISupportInitialize)(this._selectedCodeView)).BeginInit();
            this._selectedCodeView.Name = "_selectedCodeView";
            this._selectedCodeView.OptionsBehavior.Editable = false;
            this._selectedCodeView.OptionsView.ShowGroupPanel = false;
            this._selectedCodeGrid.MainView = this._selectedCodeView;
            ((System.ComponentModel.ISupportInitialize)(this._selectedCodeView)).EndInit();
            this._selectionPanel.Controls.Add(this._selectedCodeGrid);
            // 
            // _searchControl
            // 
            this._searchControl = new SearchControl();
            ((System.ComponentModel.ISupportInitialize)(this._searchControl.Properties)).BeginInit();
            this._searchControl.Dock = System.Windows.Forms.DockStyle.Top;
            this._searchControl.Location = new System.Drawing.Point(0, 0);
            this._searchControl.Name = "_searchControl";
            this._searchControl.Size = new System.Drawing.Size(700, 40);
            this._searchControl.TabIndex = 1;
            ((System.ComponentModel.ISupportInitialize)(this._searchControl.Properties)).EndInit();
            // 
            // _codeGrid
            // 
            this._codeGrid = new GridControl();
            ((System.ComponentModel.ISupportInitialize)(this._codeGrid)).BeginInit();
            this._codeGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._codeGrid.Location = new System.Drawing.Point(0, 40);
            this._codeGrid.Name = "_codeGrid";
            this._codeGrid.Size = new System.Drawing.Size(700, 260);
            this._codeGrid.TabIndex = 2;
            // 
            // _codeView
            // 
            this._codeView = new GridView(this._codeGrid);
            ((System.ComponentModel.ISupportInitialize)(this._codeView)).BeginInit();
            this._codeView.Name = "_codeView";
            this._codeView.OptionsBehavior.Editable = false;
            this._codeView.OptionsView.ShowGroupPanel = false;
            // 
            // colCode
            // 
            this.colCode = new GridColumn();
            this.colCode.FieldName = "Code";
            this.colCode.Caption = "코드";
            this.colCode.VisibleIndex = 0;
            this.colCode.Width = 100;
            this.colCode.Name = "colCode";
            // 
            // colName
            // 
            this.colName = new GridColumn();
            this.colName.FieldName = "Name";
            this.colName.Caption = "진단명";
            this.colName.VisibleIndex = 1;
            this.colName.Name = "colName";
            // 
            // colCategory
            // 
            this.colCategory = new GridColumn();
            this.colCategory.FieldName = "Category";
            this.colCategory.Caption = "카테고리";
            this.colCategory.VisibleIndex = 2;
            this.colCategory.Width = 150;
            this.colCategory.Name = "colCategory";
            
            this._codeView.Columns.AddRange(new GridColumn[] { this.colCode, this.colName, this.colCategory });
            this._codeGrid.MainView = this._codeView;
            ((System.ComponentModel.ISupportInitialize)(this._codeView)).EndInit();
            // 
            // DiagnosisCodeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._codeGrid);
            this.Controls.Add(this._searchControl);
            this.Controls.Add(this._selectionPanel);
            this.Name = "DiagnosisCodeControl";
            this.Size = new System.Drawing.Size(700, 500);
            ((System.ComponentModel.ISupportInitialize)(this._selectionPanel)).EndInit();
            this._selectionPanel.ResumeLayout(false);
            this._selectionPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._selectedCodeGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._codeGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private GridControl? _codeGrid;
        private GridView? _codeView;
        private GridControl? _selectedCodeGrid;
        private GridView? _selectedCodeView;
        private SearchControl? _searchControl;
        private TextEdit? _codeEdit;
        private TextEdit? _nameEdit;
        private SimpleButton? _addButton;
        private SimpleButton? _removeButton;
        private SimpleButton? _clearButton;
        private PanelControl? _selectionPanel;
        private GridColumn? colCode;
        private GridColumn? colName;
        private GridColumn? colCategory;

        private BindingList<Models.DiagnosisCode>? _availableCodes;
        private BindingList<Models.DiagnosisCode>? _selectedCodes;
    }
}
