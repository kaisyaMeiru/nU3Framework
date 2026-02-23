using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System.ComponentModel;

namespace nU3.Core.UI.Components.Controls
{
    partial class DateRangeControl
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
                _startDateEdit?.Dispose();
                _endDateEdit?.Dispose();
                _todayButton?.Dispose();
                _thisWeekButton?.Dispose();
                _thisMonthButton?.Dispose();
                _clearButton?.Dispose();
                _mainPanel?.Dispose();
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
            _mainPanel = new nU3.Core.UI.Controls.nU3PanelControl();
            _periodLabel = new nU3.Core.UI.Controls.nU3LabelControl();
            _startDateEdit = new nU3.Core.UI.Controls.nU3DateEdit();
            _toLabel = new nU3.Core.UI.Controls.nU3LabelControl();
            _endDateEdit = new nU3.Core.UI.Controls.nU3DateEdit();
            _todayButton = new nU3.Core.UI.Controls.nU3SimpleButton();
            _thisWeekButton = new nU3.Core.UI.Controls.nU3SimpleButton();
            _thisMonthButton = new nU3.Core.UI.Controls.nU3SimpleButton();
            _clearButton = new nU3.Core.UI.Controls.nU3SimpleButton();
            ((ISupportInitialize)_mainPanel).BeginInit();
            _mainPanel.SuspendLayout();
            SuspendLayout();
            // 
            // _mainPanel
            // 
            _mainPanel.Controls.Add(_periodLabel);
            _mainPanel.Controls.Add(_startDateEdit);
            _mainPanel.Controls.Add(_toLabel);
            _mainPanel.Controls.Add(_endDateEdit);
            _mainPanel.Controls.Add(_todayButton);
            _mainPanel.Controls.Add(_thisWeekButton);
            _mainPanel.Controls.Add(_thisMonthButton);
            _mainPanel.Controls.Add(_clearButton);
            _mainPanel.Dock = DockStyle.Fill;
            _mainPanel.Location = new Point(0, 0);
            _mainPanel.Margin = new Padding(4, 3, 4, 3);
            _mainPanel.Name = "_mainPanel";
            _mainPanel.Size = new Size(1463, 931);
            _mainPanel.TabIndex = 0;
            // 
            // _periodLabel
            // 
            _periodLabel.IsRequiredMarker = false;
            _periodLabel.Location = new Point(12, 14);
            _periodLabel.Margin = new Padding(4, 3, 4, 3);
            _periodLabel.Name = "_periodLabel";
            _periodLabel.Size = new Size(24, 14);
            _periodLabel.TabIndex = 0;
            _periodLabel.Text = "기간:";
            // 
            // _startDateEdit
            // 
            _startDateEdit.EditValue = new DateTime(2026, 2, 23, 0, 0, 0, 0);
            _startDateEdit.Location = new Point(82, 12);
            _startDateEdit.Margin = new Padding(4, 3, 4, 3);
            _startDateEdit.Name = "_startDateEdit";
            _startDateEdit.Size = new Size(140, 20);
            _startDateEdit.TabIndex = 1;
            // 
            // _toLabel
            // 
            _toLabel.IsRequiredMarker = false;
            _toLabel.Location = new Point(233, 14);
            _toLabel.Margin = new Padding(4, 3, 4, 3);
            _toLabel.Name = "_toLabel";
            _toLabel.Size = new Size(9, 14);
            _toLabel.TabIndex = 2;
            _toLabel.Text = "~";
            // 
            // _endDateEdit
            // 
            _endDateEdit.EditValue = new DateTime(2026, 2, 23, 0, 0, 0, 0);
            _endDateEdit.Location = new Point(268, 12);
            _endDateEdit.Margin = new Padding(4, 3, 4, 3);
            _endDateEdit.Name = "_endDateEdit";
            _endDateEdit.Size = new Size(140, 20);
            _endDateEdit.TabIndex = 3;
            // 
            // _todayButton
            // 
            _todayButton.AuthId = "";
            _todayButton.Location = new Point(12, 52);
            _todayButton.Margin = new Padding(4, 3, 4, 3);
            _todayButton.Name = "_todayButton";
            _todayButton.Size = new Size(70, 27);
            _todayButton.TabIndex = 4;
            _todayButton.Text = "오늘";
            // 
            // _thisWeekButton
            // 
            _thisWeekButton.AuthId = "";
            _thisWeekButton.Location = new Point(93, 52);
            _thisWeekButton.Margin = new Padding(4, 3, 4, 3);
            _thisWeekButton.Name = "_thisWeekButton";
            _thisWeekButton.Size = new Size(70, 27);
            _thisWeekButton.TabIndex = 5;
            _thisWeekButton.Text = "이번주";
            // 
            // _thisMonthButton
            // 
            _thisMonthButton.AuthId = "";
            _thisMonthButton.Location = new Point(175, 52);
            _thisMonthButton.Margin = new Padding(4, 3, 4, 3);
            _thisMonthButton.Name = "_thisMonthButton";
            _thisMonthButton.Size = new Size(70, 27);
            _thisMonthButton.TabIndex = 6;
            _thisMonthButton.Text = "이번달";
            // 
            // _clearButton
            // 
            _clearButton.AuthId = "";
            _clearButton.Location = new Point(257, 52);
            _clearButton.Margin = new Padding(4, 3, 4, 3);
            _clearButton.Name = "_clearButton";
            _clearButton.Size = new Size(70, 27);
            _clearButton.TabIndex = 7;
            _clearButton.Text = "초기화";
            // 
            // DateRangeControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(_mainPanel);
            Margin = new Padding(4, 3, 4, 3);
            Name = "DateRangeControl";
            Size = new Size(1463, 931);
            ((ISupportInitialize)_mainPanel).EndInit();
            _mainPanel.ResumeLayout(false);
            _mainPanel.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private nU3.Core.UI.Controls.nU3PanelControl? _mainPanel;
        private nU3.Core.UI.Controls.nU3DateEdit? _startDateEdit;
        private nU3.Core.UI.Controls.nU3DateEdit? _endDateEdit;
        private nU3.Core.UI.Controls.nU3SimpleButton? _todayButton;
        private nU3.Core.UI.Controls.nU3SimpleButton? _thisWeekButton;
        private nU3.Core.UI.Controls.nU3SimpleButton? _thisMonthButton;
        private nU3.Core.UI.Controls.nU3SimpleButton? _clearButton;
        private UI.Controls.nU3LabelControl _periodLabel;
        private UI.Controls.nU3LabelControl _toLabel;
    }
}
