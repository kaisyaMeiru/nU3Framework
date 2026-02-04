using System;
using System.Drawing;
using System.Windows.Forms;

namespace nU3.Bootstrapper
{
    public partial class UpdateProgressForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel _pnlTitleBar;
        private Label _lblTitleText;
        private Button _btnClose;

        private Panel _pnlProgress;
        private Label _lblStatus;
        private Label _lblDetail;
        private ProgressBar _progressBar;

        private Panel _pnlList;
        private ListView _listView;
        private ColumnHeader _colStatus;
        private ColumnHeader _colComponent;
        private ColumnHeader _colVersion;
        private ColumnHeader _colSize;
        private ColumnHeader _colType;

        private Panel _pnlBottom;
        private Button _btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this._pnlTitleBar = new System.Windows.Forms.Panel();
            this._lblTitleText = new System.Windows.Forms.Label();
            this._btnClose = new System.Windows.Forms.Button();
            this._pnlProgress = new System.Windows.Forms.Panel();
            this._lblStatus = new System.Windows.Forms.Label();
            this._lblDetail = new System.Windows.Forms.Label();
            this._progressBar = new System.Windows.Forms.ProgressBar();
            this._pnlList = new System.Windows.Forms.Panel();
            this._listView = new System.Windows.Forms.ListView();
            this._colStatus = new System.Windows.Forms.ColumnHeader();
            this._colComponent = new System.Windows.Forms.ColumnHeader();
            this._colVersion = new System.Windows.Forms.ColumnHeader();
            this._colSize = new System.Windows.Forms.ColumnHeader();
            this._colType = new System.Windows.Forms.ColumnHeader();
            this._pnlBottom = new System.Windows.Forms.Panel();
            this._btnCancel = new System.Windows.Forms.Button();
            
            this._pnlTitleBar.SuspendLayout();
            this._pnlProgress.SuspendLayout();
            this._pnlList.SuspendLayout();
            this._pnlBottom.SuspendLayout();
            this.SuspendLayout();

            // 
            // UpdateProgressForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(600, 500);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "UpdateProgressForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "nU3 Framework Updater";

            // 
            // _pnlTitleBar
            // 
            this._pnlTitleBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this._pnlTitleBar.Controls.Add(this._btnClose);
            this._pnlTitleBar.Controls.Add(this._lblTitleText);
            this._pnlTitleBar.Dock = System.Windows.Forms.DockStyle.Top;
            this._pnlTitleBar.Height = 40;
            this._pnlTitleBar.MouseDown += (s, e) => { if (e.Button == MouseButtons.Left) { ReleaseCapture(); SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0); } };

            // 
            // _lblTitleText
            // 
            this._lblTitleText.AutoSize = true;
            this._lblTitleText.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._lblTitleText.ForeColor = System.Drawing.Color.White;
            this._lblTitleText.Location = new System.Drawing.Point(12, 10);
            this._lblTitleText.Text = "nU3 Framework Updater";
            this._lblTitleText.MouseDown += (s, e) => { if (e.Button == MouseButtons.Left) { ReleaseCapture(); SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0); } };

            // 
            // _btnClose
            // 
            this._btnClose.Dock = System.Windows.Forms.DockStyle.Right;
            this._btnClose.FlatAppearance.BorderSize = 0;
            this._btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(17)))), ((int)(((byte)(35)))));
            this._btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._btnClose.ForeColor = System.Drawing.Color.White;
            this._btnClose.Location = new System.Drawing.Point(560, 0);
            this._btnClose.Size = new System.Drawing.Size(40, 40);
            this._btnClose.Text = "✕";
            this._btnClose.Click += (s, e) => this.Close();

            // 
            // _pnlProgress
            // 
            this._pnlProgress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this._pnlProgress.Controls.Add(this._lblDetail);
            this._pnlProgress.Controls.Add(this._progressBar);
            this._pnlProgress.Controls.Add(this._lblStatus);
            this._pnlProgress.Dock = System.Windows.Forms.DockStyle.Top;
            this._pnlProgress.Height = 110;
            this._pnlProgress.Padding = new System.Windows.Forms.Padding(20);

            // 
            // _lblStatus
            // 
            this._lblStatus.AutoEllipsis = true;
            this._lblStatus.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this._lblStatus.Location = new System.Drawing.Point(20, 20);
            this._lblStatus.Size = new System.Drawing.Size(560, 30);
            this._lblStatus.Text = "Checking for updates...";

            // 
            // _progressBar
            // 
            this._progressBar.Location = new System.Drawing.Point(20, 60);
            this._progressBar.Size = new System.Drawing.Size(560, 4);
            this._progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            
            // 
            // _lblDetail
            // 
            this._lblDetail.AutoEllipsis = true;
            this._lblDetail.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._lblDetail.ForeColor = System.Drawing.Color.Gray;
            this._lblDetail.Location = new System.Drawing.Point(20, 75);
            this._lblDetail.Size = new System.Drawing.Size(560, 20);
            this._lblDetail.Text = "Preparing...";

            // 
            // _pnlList
            // 
            this._pnlList.Controls.Add(this._listView);
            this._pnlList.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pnlList.Padding = new System.Windows.Forms.Padding(20, 10, 20, 0);

            // 
            // _listView
            // 
            this._listView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._colStatus,
            this._colComponent,
            this._colVersion,
            this._colSize,
            this._colType});
            this._listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._listView.FullRowSelect = true;
            this._listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this._listView.View = System.Windows.Forms.View.Details;
            this._listView.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._listView.OwnerDraw = true; 
            this._listView.DrawColumnHeader += (s, e) => e.DrawDefault = true;
            this._listView.DrawSubItem += (s, e) => {
                // Custom draw for better spacing if needed, or just default
                e.DrawDefault = true;
            };

            // Columns
            this._colStatus.Text = "상태"; this._colStatus.Width = 80;
            this._colComponent.Text = "컴포넌트"; this._colComponent.Width = 250;
            this._colVersion.Text = "버전"; this._colVersion.Width = 100;
            this._colSize.Text = "크기"; this._colSize.Width = 80;
            this._colType.Text = "유형"; this._colType.Width = 60;

            // 
            // _pnlBottom
            // 
            this._pnlBottom.Controls.Add(this._btnCancel);
            this._pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._pnlBottom.Height = 60;
            this._pnlBottom.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);

            // 
            // _btnCancel
            // 
            this._btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this._btnCancel.FlatAppearance.BorderSize = 0;
            this._btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._btnCancel.Location = new System.Drawing.Point(480, 12);
            this._btnCancel.Size = new System.Drawing.Size(100, 36);
            this._btnCancel.Text = "취소";
            this._btnCancel.Click += BtnCancel_Click;

            // Add Controls
            this.Controls.Add(this._pnlList);
            this.Controls.Add(this._pnlBottom);
            this.Controls.Add(this._pnlProgress);
            this.Controls.Add(this._pnlTitleBar);

            this._pnlTitleBar.ResumeLayout(false);
            this._pnlTitleBar.PerformLayout();
            this._pnlProgress.ResumeLayout(false);
            this._pnlList.ResumeLayout(false);
            this._pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        // Window Dragging Support
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern bool ReleaseCapture();
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    }
}