using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace nU3.Core.UI.Components.Controls
{
    partial class SearchBarControl
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
                _autoSearchTimer?.Stop();
                _autoSearchTimer?.Dispose();
                _searchEdit?.Dispose();                
                _searchButton?.Dispose();
                _clearButton?.Dispose();
                _searchTypeCombo?.Dispose();
                _mainPanel?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this._mainPanel = new nU3.Core.UI.Controls.nU3PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this._mainPanel)).BeginInit();
            this._mainPanel.SuspendLayout();
            this._mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._mainPanel.Location = new System.Drawing.Point(0, 0);
            this._mainPanel.Name = "_mainPanel";
            this._mainPanel.Size = new System.Drawing.Size(610, 50);
            this._mainPanel.TabIndex = 0;
            this._searchTypeCombo = new nU3.Core.UI.Controls.nU3ComboBoxEdit();
            ((System.ComponentModel.ISupportInitialize)(this._searchTypeCombo.Properties)).BeginInit();
            this._searchTypeCombo.Location = new System.Drawing.Point(10, 13);
            this._searchTypeCombo.Name = "_searchTypeCombo";
            this._searchTypeCombo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this._searchTypeCombo.Properties.Items.AddRange(new object[] {
            "전체",
            "환자ID",
            "환자명",
            "주민번호"});
            this._searchTypeCombo.Size = new System.Drawing.Size(100, 20);
            this._searchTypeCombo.TabIndex = 0;
            this._searchEdit = new nU3.Core.UI.Controls.nU3TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this._searchEdit.Properties)).BeginInit();
            this._searchEdit.Location = new System.Drawing.Point(120, 13);
            this._searchEdit.Name = "_searchEdit";
            this._searchEdit.Properties.NullValuePrompt = "검색어를 입력하세요...";
            this._searchEdit.Size = new System.Drawing.Size(300, 20);
            this._searchEdit.TabIndex = 1;
            this._searchButton = new nU3.Core.UI.Controls.nU3SimpleButton();
            this._searchButton.Location = new System.Drawing.Point(430, 11);
            this._searchButton.Name = "_searchButton";
            this._searchButton.Size = new System.Drawing.Size(80, 23);
            this._searchButton.TabIndex = 2;
            this._searchButton.Text = "검색";
            this._clearButton = new nU3.Core.UI.Controls.nU3SimpleButton();
            this._clearButton.Location = new System.Drawing.Point(520, 11);
            this._clearButton.Name = "_clearButton";
            this._clearButton.Size = new System.Drawing.Size(80, 23);
            this._clearButton.TabIndex = 3;
            this._clearButton.Text = "초기화";
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._mainPanel);
            this.Name = "SearchBarControl";
            this.Size = new System.Drawing.Size(610, 50);
            this._mainPanel.Controls.Add(this._searchTypeCombo);
            this._mainPanel.Controls.Add(this._searchEdit);
            this._mainPanel.Controls.Add(this._searchButton);
            this._mainPanel.Controls.Add(this._clearButton);
            ((System.ComponentModel.ISupportInitialize)(this._mainPanel)).EndInit();
            this._mainPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._searchTypeCombo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._searchEdit.Properties)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private nU3.Core.UI.Controls.nU3PanelControl? _mainPanel;
        private nU3.Core.UI.Controls.nU3ComboBoxEdit? _searchTypeCombo;
        private nU3.Core.UI.Controls.nU3TextEdit? _searchEdit;
        private nU3.Core.UI.Controls.nU3SimpleButton? _searchButton;
        private nU3.Core.UI.Controls.nU3SimpleButton? _clearButton;
    }
}
