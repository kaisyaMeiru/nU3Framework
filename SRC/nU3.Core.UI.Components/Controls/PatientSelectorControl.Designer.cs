using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace nU3.Core.UI.Components.Controls
{
    partial class PatientSelectorControl
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
                _searchEdit?.Dispose();
                _searchButton?.Dispose();
                _clearButton?.Dispose();
                _selectButton?.Dispose();
                _headerPanel?.Dispose();
                _buttonPanel?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            _headerPanel = new nU3.Core.UI.Controls.nU3PanelControl();
            _searchEdit = new nU3.Core.UI.Controls.nU3TextEdit();
            _searchButton = new nU3.Core.UI.Controls.nU3SimpleButton();
            _clearButton = new nU3.Core.UI.Controls.nU3SimpleButton();
            _buttonPanel = new nU3.Core.UI.Controls.nU3PanelControl();
            _selectButton = new nU3.Core.UI.Controls.nU3SimpleButton();
            _gridControl = new nU3.Core.UI.Controls.nU3GridControl();
            _gridView = new nU3.Core.UI.Controls.nU3GridView();
            colPatientId = new nU3.Core.UI.Controls.nU3GridColumn();
            colPatientName = new nU3.Core.UI.Controls.nU3GridColumn();
            colBirthDate = new nU3.Core.UI.Controls.nU3GridColumn();
            colGender = new nU3.Core.UI.Controls.nU3GridColumn();
            colPhoneNumber = new nU3.Core.UI.Controls.nU3GridColumn();
            ((System.ComponentModel.ISupportInitialize)_headerPanel).BeginInit();
            _headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_buttonPanel).BeginInit();
            _buttonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_gridControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_gridView).BeginInit();
            SuspendLayout();
            // 
            // _headerPanel
            // 
            _headerPanel.Controls.Add(_searchEdit);
            _headerPanel.Controls.Add(_searchButton);
            _headerPanel.Controls.Add(_clearButton);
            _headerPanel.Dock = DockStyle.Top;
            _headerPanel.Location = new Point(0, 0);
            _headerPanel.Name = "_headerPanel";
            _headerPanel.Size = new Size(600, 40);
            _headerPanel.TabIndex = 0;
            // 
            // _searchEdit
            // 
            _searchEdit.Dock = DockStyle.Fill;
            _searchEdit.Location = new Point(2, 2);
            _searchEdit.Name = "_searchEdit";
            _searchEdit.Size = new Size(436, 20);
            _searchEdit.TabIndex = 0;
            // 
            // _searchButton
            // 
            _searchButton.Dock = DockStyle.Right;
            _searchButton.Location = new Point(438, 2);
            _searchButton.Name = "_searchButton";
            _searchButton.Size = new Size(80, 36);
            _searchButton.TabIndex = 1;
            _searchButton.Text = "검색";
            // 
            // _clearButton
            // 
            _clearButton.Dock = DockStyle.Right;
            _clearButton.Location = new Point(518, 2);
            _clearButton.Name = "_clearButton";
            _clearButton.Size = new Size(80, 36);
            _clearButton.TabIndex = 2;
            _clearButton.Text = "초기화";
            // 
            // _buttonPanel
            // 
            _buttonPanel.Controls.Add(_selectButton);
            _buttonPanel.Dock = DockStyle.Bottom;
            _buttonPanel.Location = new Point(0, 350);
            _buttonPanel.Name = "_buttonPanel";
            _buttonPanel.Size = new Size(600, 50);
            _buttonPanel.TabIndex = 1;
            // 
            // _selectButton
            // 
            _selectButton.Location = new Point(10, 13);
            _selectButton.Name = "_selectButton";
            _selectButton.Size = new Size(120, 23);
            _selectButton.TabIndex = 0;
            _selectButton.Text = "선택";
            // 
            // _gridControl
            // 
            _gridControl.Dock = DockStyle.Fill;
            _gridControl.Location = new Point(0, 40);
            _gridControl.MainView = _gridView;
            _gridControl.Name = "_gridControl";
            _gridControl.Size = new Size(600, 310);
            _gridControl.TabIndex = 2;
            _gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { _gridView });
            // 
            // _gridView
            // 
            _gridView.Columns.AddRange(new GridColumn[] { colPatientId, colPatientName, colBirthDate, colGender, colPhoneNumber });
            _gridView.GridControl = _gridControl;
            _gridView.Name = "_gridView";
            _gridView.OptionsBehavior.Editable = false;
            _gridView.OptionsView.ShowAutoFilterRow = true;
            _gridView.OptionsView.ShowGroupPanel = false;
            // 
            // colPatientId
            // 
            colPatientId.Caption = "환자ID";
            colPatientId.FieldName = "PatientId";
            colPatientId.Name = "colPatientId";
            colPatientId.Visible = true;
            colPatientId.VisibleIndex = 0;
            colPatientId.Width = 100;
            // 
            // colPatientName
            // 
            colPatientName.Caption = "환자명";
            colPatientName.FieldName = "PatientName";
            colPatientName.Name = "colPatientName";
            colPatientName.Visible = true;
            colPatientName.VisibleIndex = 1;
            colPatientName.Width = 100;
            // 
            // colBirthDate
            // 
            colBirthDate.Caption = "생년월일";
            colBirthDate.DisplayFormat.FormatString = "yyyy-MM-dd";
            colBirthDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            colBirthDate.FieldName = "BirthDate";
            colBirthDate.Name = "colBirthDate";
            colBirthDate.Visible = true;
            colBirthDate.VisibleIndex = 2;
            colBirthDate.Width = 100;
            // 
            // colGender
            // 
            colGender.Caption = "성별";
            colGender.FieldName = "Gender";
            colGender.Name = "colGender";
            colGender.UnboundType = DevExpress.Data.UnboundColumnType.String;
            colGender.Visible = true;
            colGender.VisibleIndex = 3;
            colGender.Width = 60;
            // 
            // colPhoneNumber
            // 
            colPhoneNumber.Caption = "연락처";
            colPhoneNumber.FieldName = "PhoneNumber";
            colPhoneNumber.Name = "colPhoneNumber";
            colPhoneNumber.Visible = true;
            colPhoneNumber.VisibleIndex = 4;
            colPhoneNumber.Width = 120;
            // 
            // PatientSelectorControl
            // 
            AutoScaleDimensions = new SizeF(7F, 14F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(_gridControl);
            Controls.Add(_buttonPanel);
            Controls.Add(_headerPanel);
            Name = "PatientSelectorControl";
            Size = new Size(600, 400);
            ((System.ComponentModel.ISupportInitialize)_headerPanel).EndInit();
            _headerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_buttonPanel).EndInit();
            _buttonPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_gridControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)_gridView).EndInit();
            ResumeLayout(false);

            #endregion
        }
        private nU3.Core.UI.Controls.nU3GridColumn colPatientId;
        private nU3.Core.UI.Controls.nU3GridColumn colPatientName;
        private nU3.Core.UI.Controls.nU3GridColumn colBirthDate;
        private nU3.Core.UI.Controls.nU3GridColumn colGender;
        private nU3.Core.UI.Controls.nU3GridColumn colPhoneNumber;
    }
}
