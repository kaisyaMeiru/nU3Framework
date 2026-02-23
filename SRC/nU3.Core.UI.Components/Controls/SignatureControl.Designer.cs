using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace nU3.Core.UI.Components.Controls
{
    partial class SignatureControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            if (disposing)
            {
                _signatureGraphics?.Dispose();
                _signatureBitmap?.Dispose();
                _clearButton?.Dispose();
                _saveButton?.Dispose();
                _undoButton?.Dispose();
                _confirmCheckEdit?.Dispose();
                _commentEdit?.Dispose();
                _signaturePanel?.Dispose();
                _buttonPanel?.Dispose();
                _commentPanel?.Dispose();
                _commentLabel?.Dispose();
                _statusLabel?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // _signaturePanel
            // 
            this._signaturePanel = new nU3.Core.UI.Controls.nU3PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this._signaturePanel)).BeginInit();
            this._signaturePanel.SuspendLayout();
            this._signaturePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._signaturePanel.Location = new System.Drawing.Point(0, 0);
            this._signaturePanel.Name = "_signaturePanel";
            this._signaturePanel.Size = new System.Drawing.Size(500, 150);
            this._signaturePanel.TabIndex = 0;
            // 
            // _buttonPanel
            // 
            this._buttonPanel = new nU3.Core.UI.Controls.nU3PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this._buttonPanel)).BeginInit();
            this._buttonPanel.SuspendLayout();
            this._buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._buttonPanel.Location = new System.Drawing.Point(0, 360);
            this._buttonPanel.Name = "_buttonPanel";
            this._buttonPanel.Size = new System.Drawing.Size(500, 40);
            this._buttonPanel.TabIndex = 1;
            // 
            // _clearButton
            // 
            this._clearButton = new nU3.Core.UI.Controls.nU3SimpleButton();
            this._clearButton.Location = new System.Drawing.Point(10, 8);
            this._clearButton.Name = "_clearButton";
            this._clearButton.Size = new System.Drawing.Size(80, 23);
            this._clearButton.TabIndex = 0;
            this._clearButton.Text = "지우기";
            // 
            // _undoButton
            // 
            this._undoButton = new nU3.Core.UI.Controls.nU3SimpleButton();
            this._undoButton.Location = new System.Drawing.Point(100, 8);
            this._undoButton.Name = "_undoButton";
            this._undoButton.Size = new System.Drawing.Size(80, 23);
            this._undoButton.TabIndex = 1;
            this._undoButton.Text = "되돌리기";
            // 
            // _saveButton
            // 
            this._saveButton = new nU3.Core.UI.Controls.nU3SimpleButton();
            this._saveButton.Location = new System.Drawing.Point(390, 8);
            this._saveButton.Name = "_saveButton";
            this._saveButton.Size = new System.Drawing.Size(100, 23);
            this._saveButton.TabIndex = 2;
            this._saveButton.Text = "서명완료";
            // 
            // _commentPanel
            // 
            this._commentPanel = new nU3.Core.UI.Controls.nU3PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this._commentPanel)).BeginInit();
            this._commentPanel.SuspendLayout();
            this._commentPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._commentPanel.Location = new System.Drawing.Point(0, 260);
            this._commentPanel.Name = "_commentPanel";
            this._commentPanel.Size = new System.Drawing.Size(500, 100);
            this._commentPanel.TabIndex = 2;
            // 
            // _commentLabel
            // 
            this._commentLabel = new nU3.Core.UI.Controls.nU3LabelControl();
            this._commentLabel.Location = new System.Drawing.Point(10, 10);
            this._commentLabel.Name = "_commentLabel";
            this._commentLabel.Size = new System.Drawing.Size(56, 14);
            this._commentLabel.TabIndex = 0;
            this._commentLabel.Text = "코멘트";
            // 
            // _commentEdit
            // 
            this._commentEdit = new nU3.Core.UI.Controls.nU3MemoEdit();
            ((System.ComponentModel.ISupportInitialize)(this._commentEdit.Properties)).BeginInit();
            this._commentEdit.Location = new System.Drawing.Point(10, 35);
            this._commentEdit.Name = "_commentEdit";
            this._commentEdit.Properties.NullValuePrompt = "선택사항: 추가 코멘트를 입력하세요...";
            this._commentEdit.Size = new System.Drawing.Size(480, 55);
            this._commentEdit.TabIndex = 1;
            // 
            // _confirmCheckEdit
            // 
            this._confirmCheckEdit = new nU3.Core.UI.Controls.nU3CheckEdit();
            ((System.ComponentModel.ISupportInitialize)(this._confirmCheckEdit.Properties)).BeginInit();
            this._confirmCheckEdit.Location = new System.Drawing.Point(10, 200);
            this._confirmCheckEdit.Name = "_confirmCheckEdit";
            this._confirmCheckEdit.Properties.Caption = "위 내용을 확인하고 서명합니다.";
            this._confirmCheckEdit.Size = new System.Drawing.Size(300, 20);
            this._confirmCheckEdit.TabIndex = 3;
            // 
            // _statusLabel
            // 
            this._statusLabel = new nU3.Core.UI.Controls.nU3LabelControl();
            this._statusLabel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this._statusLabel.Location = new System.Drawing.Point(10, 230);
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new System.Drawing.Size(480, 20);
            this._statusLabel.TabIndex = 4;
            this._statusLabel.Text = "서명 패드에 서명하세요.";
            // 
            // SignatureControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._signaturePanel);
            this.Controls.Add(this._statusLabel);
            this.Controls.Add(this._confirmCheckEdit);
            this.Controls.Add(this._commentPanel);
            this.Controls.Add(this._buttonPanel);
            this.Name = "SignatureControl";
            this.Size = new System.Drawing.Size(500, 400);
            this._buttonPanel.Controls.Add(this._clearButton);
            this._buttonPanel.Controls.Add(this._undoButton);
            this._buttonPanel.Controls.Add(this._saveButton);
            this._commentPanel.Controls.Add(this._commentLabel);
            this._commentPanel.Controls.Add(this._commentEdit);
            ((System.ComponentModel.ISupportInitialize)(this._signaturePanel)).EndInit();
            this._signaturePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._buttonPanel)).EndInit();
            this._buttonPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._commentPanel)).EndInit();
            this._commentPanel.ResumeLayout(false);
            this._commentPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._commentEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._confirmCheckEdit.Properties)).EndInit();
            this.ResumeLayout(false);

            #endregion
        }
    }
}
