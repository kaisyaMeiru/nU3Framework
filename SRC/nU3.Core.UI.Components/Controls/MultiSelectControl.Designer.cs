using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace nU3.Core.UI.Components.Controls
{
    partial class MultiSelectControl
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
                _gridControl?.Dispose();
                _gridView?.Dispose();
                _searchControl?.Dispose();
                _selectedPanel?.Dispose();
                _selectedLabel?.Dispose();
                _headerPanel?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this._headerPanel = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this._headerPanel)).BeginInit();
            this._headerPanel.SuspendLayout();
            this._headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this._headerPanel.Location = new System.Drawing.Point(0, 0);
            this._headerPanel.Name = "_headerPanel";
            this._headerPanel.Size = new System.Drawing.Size(500, 120);
            this._headerPanel.TabIndex = 0;
            this._selectedLabel = new DevExpress.XtraEditors.LabelControl();
            this._selectedLabel.Location = new System.Drawing.Point(10, 10);
            this._selectedLabel.Name = "_selectedLabel";
            this._selectedLabel.Size = new System.Drawing.Size(84, 14);
            this._selectedLabel.TabIndex = 0;
            this._selectedLabel.Text = "선택된 항목:";
            this._selectedPanel = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this._selectedPanel)).BeginInit();
            this._selectedPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this._selectedPanel.Location = new System.Drawing.Point(10, 35);
            this._selectedPanel.Name = "_selectedPanel";
            this._selectedPanel.Size = new System.Drawing.Size(480, 75);
            this._selectedPanel.TabIndex = 1;
            this._searchControl = new DevExpress.XtraEditors.SearchControl();
            ((System.ComponentModel.ISupportInitialize)(this._searchControl.Properties)).BeginInit();
            this._searchControl.Dock = System.Windows.Forms.DockStyle.Top;
            this._searchControl.Location = new System.Drawing.Point(0, 120);
            this._searchControl.Name = "_searchControl";
            this._searchControl.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Repository.ClearButton(),
            new DevExpress.XtraEditors.Repository.SearchButton()});
            this._searchControl.Size = new System.Drawing.Size(500, 20);
            this._searchControl.TabIndex = 1;
            this._gridControl = new DevExpress.XtraGrid.GridControl();
            ((System.ComponentModel.ISupportInitialize)(this._gridControl)).BeginInit();
            this._gridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gridControl.Location = new System.Drawing.Point(0, 140);
            this._gridControl.Name = "_gridControl";
            this._gridControl.Size = new System.Drawing.Size(500, 260);
            this._gridControl.TabIndex = 2;
            this._gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this._gridControl.MainView = this._gridView;
            this._gridView.GridControl = this._gridControl;
            this._gridView.Name = "_gridView";
            this._gridView.OptionsBehavior.Editable = true;
            this._gridView.OptionsSelection.MultiSelect = true;
            this._gridView.OptionsSelection.CheckBoxSelectorColumnWidth = 40;
            this._gridView.OptionsView.ShowGroupPanel = false;
            
            var colSelected = new DevExpress.XtraGrid.Columns.GridColumn();
            colSelected.Caption = "선택";
            colSelected.FieldName = "IsSelected";
            colSelected.Name = "colIsSelected";
            colSelected.Visible = true;
            colSelected.VisibleIndex = 0;
            colSelected.Width = 60;
            var repositoryItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            ((System.ComponentModel.ISupportInitialize)(repositoryItemCheckEdit)).BeginInit();
            repositoryItemCheckEdit.Name = "repositoryItemCheckEdit";
            colSelected.ColumnEdit = repositoryItemCheckEdit;
            
            var colDisplayText = new DevExpress.XtraGrid.Columns.GridColumn();
            colDisplayText.Caption = "항목";
            colDisplayText.FieldName = "DisplayText";
            colDisplayText.Name = "colDisplayText";
            colDisplayText.Visible = true;
            colDisplayText.VisibleIndex = 1;
            colDisplayText.Width = 200;
            
            var colValue = new DevExpress.XtraGrid.Columns.GridColumn();
            colValue.Caption = "값";
            colValue.FieldName = "Value";
            colValue.Name = "colValue";
            colValue.Visible = true;
            colValue.VisibleIndex = 2;
            colValue.Width = 200;
            
            this._gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
                colSelected,
                colDisplayText,
                colValue
            });
            
            this._gridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
                repositoryItemCheckEdit
            });
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._gridControl);
            this.Controls.Add(this._searchControl);
            this.Controls.Add(this._headerPanel);
            this.Name = "MultiSelectControl";
            this.Size = new System.Drawing.Size(500, 400);
            this._headerPanel.Controls.Add(this._selectedPanel);
            this._headerPanel.Controls.Add(this._selectedLabel);
            ((System.ComponentModel.ISupportInitialize)(this._headerPanel)).EndInit();
            this._headerPanel.ResumeLayout(false);
            this._headerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._selectedPanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._searchControl.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._gridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._gridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(repositoryItemCheckEdit)).EndInit();
            this.ResumeLayout(false);
            
            this._gridControl.DataSource = this._allItems;

            #endregion
        }
    }
}
