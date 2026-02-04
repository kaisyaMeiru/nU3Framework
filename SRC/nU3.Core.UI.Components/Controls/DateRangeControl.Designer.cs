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
            this.SuspendLayout();
            // 
            // _mainPanel
            // 
            this._mainPanel = new PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this._mainPanel)).BeginInit();
            this._mainPanel.SuspendLayout();
            this._mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._mainPanel.Location = new System.Drawing.Point(0, 0);
            this._mainPanel.Name = "_mainPanel";
            this._mainPanel.Size = new System.Drawing.Size(400, 80);
            this._mainPanel.TabIndex = 0;
            // 
            // _periodLabel
            // 
            var _periodLabel = new LabelControl();
            _periodLabel.Location = new System.Drawing.Point(10, 12);
            _periodLabel.Name = "_periodLabel";
            _periodLabel.Size = new System.Drawing.Size(50, 14);
            _periodLabel.TabIndex = 0;
            _periodLabel.Text = "기간:";
            this._mainPanel.Controls.Add(_periodLabel);
            // 
            // _startDateEdit
            // 
            this._startDateEdit = new DateEdit();
            ((System.ComponentModel.ISupportInitialize)(this._startDateEdit.Properties)).BeginInit();
            this._startDateEdit.Location = new System.Drawing.Point(70, 10);
            this._startDateEdit.Name = "_startDateEdit";
            this._startDateEdit.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this._startDateEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this._startDateEdit.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this._startDateEdit.Size = new System.Drawing.Size(120, 20);
            this._startDateEdit.TabIndex = 1;
            ((System.ComponentModel.ISupportInitialize)(this._startDateEdit.Properties)).EndInit();
            this._mainPanel.Controls.Add(this._startDateEdit);
            // 
            // _toLabel
            // 
            var _toLabel = new LabelControl();
            _toLabel.Location = new System.Drawing.Point(200, 12);
            _toLabel.Name = "_toLabel";
            _toLabel.Size = new System.Drawing.Size(20, 14);
            _toLabel.TabIndex = 2;
            _toLabel.Text = "~";
            this._mainPanel.Controls.Add(_toLabel);
            // 
            // _endDateEdit
            // 
            this._endDateEdit = new DateEdit();
            ((System.ComponentModel.ISupportInitialize)(this._endDateEdit.Properties)).BeginInit();
            this._endDateEdit.Location = new System.Drawing.Point(230, 10);
            this._endDateEdit.Name = "_endDateEdit";
            this._endDateEdit.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this._endDateEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this._endDateEdit.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this._endDateEdit.Size = new System.Drawing.Size(120, 20);
            this._endDateEdit.TabIndex = 3;
            ((System.ComponentModel.ISupportInitialize)(this._endDateEdit.Properties)).EndInit();
            this._mainPanel.Controls.Add(this._endDateEdit);
            // 
            // _todayButton
            // 
            this._todayButton = new SimpleButton();
            this._todayButton.Location = new System.Drawing.Point(10, 45);
            this._todayButton.Name = "_todayButton";
            this._todayButton.Size = new System.Drawing.Size(60, 23);
            this._todayButton.TabIndex = 4;
            this._todayButton.Text = "오늘";
            this._mainPanel.Controls.Add(this._todayButton);
            // 
            // _thisWeekButton
            // 
            this._thisWeekButton = new SimpleButton();
            this._thisWeekButton.Location = new System.Drawing.Point(80, 45);
            this._thisWeekButton.Name = "_thisWeekButton";
            this._thisWeekButton.Size = new System.Drawing.Size(60, 23);
            this._thisWeekButton.TabIndex = 5;
            this._thisWeekButton.Text = "이번주";
            this._mainPanel.Controls.Add(this._thisWeekButton);
            // 
            // _thisMonthButton
            // 
            this._thisMonthButton = new SimpleButton();
            this._thisMonthButton.Location = new System.Drawing.Point(150, 45);
            this._thisMonthButton.Name = "_thisMonthButton";
            this._thisMonthButton.Size = new System.Drawing.Size(60, 23);
            this._thisMonthButton.TabIndex = 6;
            this._thisMonthButton.Text = "이번달";
            this._mainPanel.Controls.Add(this._thisMonthButton);
            // 
            // _clearButton
            // 
            this._clearButton = new SimpleButton();
            this._clearButton.Location = new System.Drawing.Point(220, 45);
            this._clearButton.Name = "_clearButton";
            this._clearButton.Size = new System.Drawing.Size(60, 23);
            this._clearButton.TabIndex = 7;
            this._clearButton.Text = "초기화";
            this._mainPanel.Controls.Add(this._clearButton);
            // 
            // DateRangeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._mainPanel);
            this.Name = "DateRangeControl";
            this.Size = new System.Drawing.Size(400, 80);
            ((System.ComponentModel.ISupportInitialize)(this._mainPanel)).EndInit();
            this._mainPanel.ResumeLayout(false);
            this._mainPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private PanelControl? _mainPanel;
        private DateEdit? _startDateEdit;
        private DateEdit? _endDateEdit;
        private SimpleButton? _todayButton;
        private SimpleButton? _thisWeekButton;
        private SimpleButton? _thisMonthButton;
        private SimpleButton? _clearButton;
    }
}
