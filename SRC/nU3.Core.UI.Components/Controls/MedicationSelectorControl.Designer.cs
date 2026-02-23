using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace nU3.Core.UI.Components.Controls
{
    partial class MedicationSelectorControl
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
                _medicationGrid?.Dispose();
                _medicationView?.Dispose();
                _selectedMedicationGrid?.Dispose();
                _selectedMedicationView?.Dispose();
                _searchControl?.Dispose();
                _codeEdit?.Dispose();
                _nameEdit?.Dispose();
                _dosageEdit?.Dispose();
                _quantityEdit?.Dispose();
                _addButton?.Dispose();
                _removeButton?.Dispose();
                _clearButton?.Dispose();
                _selectedLabel?.Dispose();
                _selectionPanel?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this._selectionPanel = new nU3.Core.UI.Controls.nU3PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this._selectionPanel)).BeginInit();
            this._selectionPanel.SuspendLayout();
            this._selectionPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._selectionPanel.Location = new System.Drawing.Point(0, 350);
            this._selectionPanel.Name = "_selectionPanel";
            this._selectionPanel.Size = new System.Drawing.Size(840, 250);
            this._selectionPanel.TabIndex = 0;
            this._selectedLabel = new nU3.Core.UI.Controls.nU3LabelControl();
            this._selectedLabel.Location = new System.Drawing.Point(10, 10);
            this._selectedLabel.Name = "_selectedLabel";
            this._selectedLabel.Size = new System.Drawing.Size(84, 14);
            this._selectedLabel.TabIndex = 0;
            this._selectedLabel.Text = "선택한약물:";
            this._codeEdit = new nU3.Core.UI.Controls.nU3TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this._codeEdit.Properties)).BeginInit();
            this._codeEdit.Location = new System.Drawing.Point(10, 40);
            this._codeEdit.Name = "_codeEdit";
            this._codeEdit.Properties.NullValuePrompt = "약물코드";
            this._codeEdit.Size = new System.Drawing.Size(100, 20);
            this._codeEdit.TabIndex = 1;
            this._nameEdit = new nU3.Core.UI.Controls.nU3TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this._nameEdit.Properties)).BeginInit();
            this._nameEdit.Location = new System.Drawing.Point(120, 40);
            this._nameEdit.Name = "_nameEdit";
            this._nameEdit.Properties.NullValuePrompt = "약물명";
            this._nameEdit.Size = new System.Drawing.Size(300, 20);
            this._nameEdit.TabIndex = 2;
            this._dosageEdit = new nU3.Core.UI.Controls.nU3TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this._dosageEdit.Properties)).BeginInit();
            this._dosageEdit.Location = new System.Drawing.Point(430, 40);
            this._dosageEdit.Name = "_dosageEdit";
            this._dosageEdit.Properties.NullValuePrompt = "용량/투여방법";
            this._dosageEdit.Size = new System.Drawing.Size(200, 20);
            this._dosageEdit.TabIndex = 3;
            this._quantityEdit = new nU3.Core.UI.Controls.nU3SpinEdit();
            ((System.ComponentModel.ISupportInitialize)(this._quantityEdit.Properties)).BeginInit();
            this._quantityEdit.Location = new System.Drawing.Point(640, 40);
            this._quantityEdit.Name = "_quantityEdit";
            this._quantityEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this._quantityEdit.Properties.NullValuePrompt = "수량";
            this._quantityEdit.Size = new System.Drawing.Size(100, 20);
            this._quantityEdit.TabIndex = 4;
            this._addButton = new nU3.Core.UI.Controls.nU3SimpleButton();
            this._addButton.Location = new System.Drawing.Point(750, 35);
            this._addButton.Name = "_addButton";
            this._addButton.Size = new System.Drawing.Size(80, 23);
            this._addButton.TabIndex = 5;
            this._addButton.Text = "추가";
            this._removeButton = new nU3.Core.UI.Controls.nU3SimpleButton();
            this._removeButton.Location = new System.Drawing.Point(750, 70);
            this._removeButton.Name = "_removeButton";
            this._removeButton.Size = new System.Drawing.Size(80, 23);
            this._removeButton.TabIndex = 6;
            this._removeButton.Text = "삭제";
            this._clearButton = new nU3.Core.UI.Controls.nU3SimpleButton();
            this._clearButton.Location = new System.Drawing.Point(750, 105);
            this._clearButton.Name = "_clearButton";
            this._clearButton.Size = new System.Drawing.Size(80, 23);
            this._clearButton.TabIndex = 7;
            this._clearButton.Text = "전체삭제";
            this._selectedMedicationGrid = new nU3.Core.UI.Controls.nU3GridControl();
            ((System.ComponentModel.ISupportInitialize)(this._selectedMedicationGrid)).BeginInit();
            this._selectedMedicationGrid.Location = new System.Drawing.Point(10, 140);
            this._selectedMedicationGrid.Name = "_selectedMedicationGrid";
            this._selectedMedicationGrid.Size = new System.Drawing.Size(820, 100);
            this._selectedMedicationGrid.TabIndex = 8;
            this._selectedMedicationView = new nU3.Core.UI.Controls.nU3GridView();
            this._selectedMedicationGrid.MainView = this._selectedMedicationView;
            this._selectedMedicationView.GridControl = this._selectedMedicationGrid;
            this._selectedMedicationView.Name = "_selectedMedicationView";
            this._selectedMedicationView.OptionsBehavior.Editable = false;
            this._selectedMedicationView.OptionsSelection.MultiSelect = false;
            this._selectedMedicationView.OptionsView.ShowGroupPanel = false;
            
            var colSelectedCode = new nU3.Core.UI.Controls.nU3GridColumn();
            colSelectedCode.Caption = "코드";
            colSelectedCode.FieldName = "Medication.Code";
            colSelectedCode.Name = "colSelectedCode";
            colSelectedCode.Visible = true;
            colSelectedCode.VisibleIndex = 0;
            colSelectedCode.Width = 100;
            
            var colSelectedName = new nU3.Core.UI.Controls.nU3GridColumn();
            colSelectedName.Caption = "약물명";
            colSelectedName.FieldName = "Medication.Name";
            colSelectedName.Name = "colSelectedName";
            colSelectedName.Visible = true;
            colSelectedName.VisibleIndex = 1;
            colSelectedName.Width = 200;
            
            var colSelectedCategory = new nU3.Core.UI.Controls.nU3GridColumn();
            colSelectedCategory.Caption = "카테고리";
            colSelectedCategory.FieldName = "Medication.Category";
            colSelectedCategory.Name = "colSelectedCategory";
            colSelectedCategory.Visible = true;
            colSelectedCategory.VisibleIndex = 2;
            colSelectedCategory.Width = 150;
            
            var colDosage = new nU3.Core.UI.Controls.nU3GridColumn();
            colDosage.Caption = "용량";
            colDosage.FieldName = "Dosage";
            colDosage.Name = "colDosage";
            colDosage.Visible = true;
            colDosage.VisibleIndex = 3;
            colDosage.Width = 150;
            
            var colQuantity = new nU3.Core.UI.Controls.nU3GridColumn();
            colQuantity.Caption = "수량";
            colQuantity.FieldName = "Quantity";
            colQuantity.Name = "colQuantity";
            colQuantity.Visible = true;
            colQuantity.VisibleIndex = 4;
            colQuantity.Width = 80;
            
            this._selectedMedicationView.Columns.AddRange(new nU3.Core.UI.Controls.nU3GridColumn[] {
                colSelectedCode,
                colSelectedName,
                colSelectedCategory,
                colDosage,
                colQuantity
            });
            
            this._searchControl = new nU3.Core.UI.Controls.nU3SearchControl();
            ((System.ComponentModel.ISupportInitialize)(this._searchControl.Properties)).BeginInit();
            this._searchControl.Dock = System.Windows.Forms.DockStyle.Top;
            this._searchControl.Location = new System.Drawing.Point(0, 0);
            this._searchControl.Name = "_searchControl";
            this._searchControl.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Repository.ClearButton(),
            new DevExpress.XtraEditors.Repository.SearchButton()});
            this._searchControl.Size = new System.Drawing.Size(840, 20);
            this._searchControl.TabIndex = 1;
            this._medicationGrid = new nU3.Core.UI.Controls.nU3GridControl();
            ((System.ComponentModel.ISupportInitialize)(this._medicationGrid)).BeginInit();
            this._medicationGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._medicationGrid.Location = new System.Drawing.Point(0, 20);
            this._medicationGrid.Name = "_medicationGrid";
            this._medicationGrid.Size = new System.Drawing.Size(840, 330);
            this._medicationGrid.TabIndex = 2;
            this._medicationView = new nU3.Core.UI.Controls.nU3GridView();
            this._medicationGrid.MainView = this._medicationView;
            this._medicationView.GridControl = this._medicationGrid;
            this._medicationView.Name = "_medicationView";
            this._medicationView.OptionsBehavior.Editable = false;
            this._medicationView.OptionsSelection.MultiSelect = false;
            this._medicationView.OptionsView.ShowGroupPanel = false;
            
            var colCode = new nU3.Core.UI.Controls.nU3GridColumn();
            colCode.Caption = "코드";
            colCode.FieldName = "Code";
            colCode.Name = "colCode";
            colCode.Visible = true;
            colCode.VisibleIndex = 0;
            colCode.Width = 100;
            
            var colName = new nU3.Core.UI.Controls.nU3GridColumn();
            colName.Caption = "약물명";
            colName.FieldName = "Name";
            colName.Name = "colName";
            colName.Visible = true;
            colName.VisibleIndex = 1;
            colName.Width = 250;
            
            var colCategory = new nU3.Core.UI.Controls.nU3GridColumn();
            colCategory.Caption = "카테고리";
            colCategory.FieldName = "Category";
            colCategory.Name = "colCategory";
            colCategory.Visible = true;
            colCategory.VisibleIndex = 2;
            colCategory.Width = 150;
            
            this._medicationView.Columns.AddRange(new nU3.Core.UI.Controls.nU3GridColumn[] {
                colCode,
                colName,
                colCategory
            });
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._medicationGrid);
            this.Controls.Add(this._searchControl);
            this.Controls.Add(this._selectionPanel);
            this.Name = "MedicationSelectorControl";
            this.Size = new System.Drawing.Size(840, 600);
            this._selectionPanel.Controls.Add(this._selectedMedicationGrid);
            this._selectionPanel.Controls.Add(this._clearButton);
            this._selectionPanel.Controls.Add(this._removeButton);
            this._selectionPanel.Controls.Add(this._addButton);
            this._selectionPanel.Controls.Add(this._quantityEdit);
            this._selectionPanel.Controls.Add(this._dosageEdit);
            this._selectionPanel.Controls.Add(this._nameEdit);
            this._selectionPanel.Controls.Add(this._codeEdit);
            this._selectionPanel.Controls.Add(this._selectedLabel);
            ((System.ComponentModel.ISupportInitialize)(this._selectionPanel)).EndInit();
            this._selectionPanel.ResumeLayout(false);
            this._selectionPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._codeEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._nameEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._dosageEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._quantityEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._selectedMedicationGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._selectedMedicationView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._searchControl.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._medicationGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._medicationView)).EndInit();
            this.ResumeLayout(false);
            
            this._medicationGrid.DataSource = this._medications;
            this._selectedMedicationGrid.DataSource = this._selectedMedications;

            #endregion
        }
    }
}
