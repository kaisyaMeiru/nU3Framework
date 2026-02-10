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
            _pnlTitleBar = new Panel();
            _btnClose = new Button();
            _lblTitleText = new Label();
            _pnlProgress = new Panel();
            _lblDetail = new Label();
            _progressBar = new ProgressBar();
            _lblStatus = new Label();
            _pnlList = new Panel();
            _listView = new ListView();
            _colStatus = new ColumnHeader();
            _colComponent = new ColumnHeader();
            _colVersion = new ColumnHeader();
            _colSize = new ColumnHeader();
            _colType = new ColumnHeader();
            _pnlBottom = new Panel();
            _btnCancel = new Button();
            _pnlTitleBar.SuspendLayout();
            _pnlProgress.SuspendLayout();
            _pnlList.SuspendLayout();
            _pnlBottom.SuspendLayout();
            SuspendLayout();
            // 
            // _pnlTitleBar
            // 
            _pnlTitleBar.BackColor = Color.FromArgb(51, 51, 51);
            _pnlTitleBar.Controls.Add(_btnClose);
            _pnlTitleBar.Controls.Add(_lblTitleText);
            _pnlTitleBar.Dock = DockStyle.Top;
            _pnlTitleBar.Location = new Point(0, 0);
            _pnlTitleBar.Name = "_pnlTitleBar";
            _pnlTitleBar.Size = new Size(600, 40);
            _pnlTitleBar.TabIndex = 3;
            _pnlTitleBar.MouseDown += PnlTitleBar_MouseDown;
            // 
            // _btnClose
            // 
            _btnClose.Dock = DockStyle.Right;
            _btnClose.FlatAppearance.BorderSize = 0;
            _btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 17, 35);
            _btnClose.FlatStyle = FlatStyle.Flat;
            _btnClose.Font = new Font("Segoe UI", 9F);
            _btnClose.ForeColor = Color.White;
            _btnClose.Location = new Point(560, 0);
            _btnClose.Name = "_btnClose";
            _btnClose.Size = new Size(40, 40);
            _btnClose.TabIndex = 0;
            _btnClose.Text = "✕";
            _btnClose.Click += BtnClose_Click;
            // 
            // _lblTitleText
            // 
            _lblTitleText.AutoSize = true;
            _lblTitleText.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            _lblTitleText.ForeColor = Color.White;
            _lblTitleText.Location = new Point(12, 10);
            _lblTitleText.Name = "_lblTitleText";
            _lblTitleText.Size = new Size(182, 20);
            _lblTitleText.TabIndex = 1;
            _lblTitleText.Text = "nU3 Framework Updater";
            _lblTitleText.MouseDown += LblTitleText_MouseDown;
            // 
            // _pnlProgress
            // 
            _pnlProgress.BackColor = Color.FromArgb(248, 248, 248);
            _pnlProgress.Controls.Add(_lblDetail);
            _pnlProgress.Controls.Add(_progressBar);
            _pnlProgress.Controls.Add(_lblStatus);
            _pnlProgress.Dock = DockStyle.Top;
            _pnlProgress.Location = new Point(0, 40);
            _pnlProgress.Name = "_pnlProgress";
            _pnlProgress.Padding = new Padding(20);
            _pnlProgress.Size = new Size(600, 110);
            _pnlProgress.TabIndex = 2;
            // 
            // _lblDetail
            // 
            _lblDetail.AutoEllipsis = true;
            _lblDetail.Font = new Font("Segoe UI", 9F);
            _lblDetail.ForeColor = Color.Gray;
            _lblDetail.Location = new Point(20, 75);
            _lblDetail.Name = "_lblDetail";
            _lblDetail.Size = new Size(560, 20);
            _lblDetail.TabIndex = 0;
            _lblDetail.Text = "Preparing...";
            // 
            // _progressBar
            // 
            _progressBar.Location = new Point(20, 60);
            _progressBar.Name = "_progressBar";
            _progressBar.Size = new Size(560, 4);
            _progressBar.Style = ProgressBarStyle.Continuous;
            _progressBar.TabIndex = 1;
            // 
            // _lblStatus
            // 
            _lblStatus.AutoEllipsis = true;
            _lblStatus.Font = new Font("Segoe UI", 14F);
            _lblStatus.ForeColor = Color.FromArgb(0, 120, 215);
            _lblStatus.Location = new Point(20, 20);
            _lblStatus.Name = "_lblStatus";
            _lblStatus.Size = new Size(560, 30);
            _lblStatus.TabIndex = 2;
            _lblStatus.Text = "Checking for updates...";
            // 
            // _pnlList
            // 
            _pnlList.Controls.Add(_listView);
            _pnlList.Dock = DockStyle.Fill;
            _pnlList.Location = new Point(0, 150);
            _pnlList.Name = "_pnlList";
            _pnlList.Padding = new Padding(20, 10, 20, 0);
            _pnlList.Size = new Size(600, 290);
            _pnlList.TabIndex = 0;
            // 
            // _listView
            // 
            _listView.BorderStyle = BorderStyle.None;
            _listView.Columns.AddRange(new ColumnHeader[] { _colStatus, _colComponent, _colVersion, _colSize, _colType });
            _listView.Dock = DockStyle.Fill;
            _listView.Font = new Font("Segoe UI", 9F);
            _listView.FullRowSelect = true;
            _listView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            _listView.Location = new Point(20, 10);
            _listView.Name = "_listView";
            _listView.OwnerDraw = true;
            _listView.Size = new Size(560, 280);
            _listView.TabIndex = 0;
            _listView.UseCompatibleStateImageBehavior = false;
            _listView.View = View.Details;
            _listView.DrawColumnHeader += ListView_DrawColumnHeader;
            _listView.DrawSubItem += ListView_DrawSubItem;
            // 
            // _colStatus
            // 
            _colStatus.Text = "상태";
            _colStatus.Width = 80;
            // 
            // _colComponent
            // 
            _colComponent.Text = "컴포넌트";
            _colComponent.Width = 250;
            // 
            // _colVersion
            // 
            _colVersion.Text = "버전";
            _colVersion.Width = 100;
            // 
            // _colSize
            // 
            _colSize.Text = "크기";
            _colSize.Width = 80;
            // 
            // _colType
            // 
            _colType.Text = "유형";
            // 
            // _pnlBottom
            // 
            _pnlBottom.Controls.Add(_btnCancel);
            _pnlBottom.Dock = DockStyle.Bottom;
            _pnlBottom.Location = new Point(0, 440);
            _pnlBottom.Name = "_pnlBottom";
            _pnlBottom.Padding = new Padding(0, 0, 20, 0);
            _pnlBottom.Size = new Size(600, 60);
            _pnlBottom.TabIndex = 1;
            // 
            // _btnCancel
            // 
            _btnCancel.BackColor = Color.FromArgb(225, 225, 225);
            _btnCancel.FlatAppearance.BorderSize = 0;
            _btnCancel.FlatStyle = FlatStyle.Flat;
            _btnCancel.Font = new Font("Segoe UI", 9F);
            _btnCancel.Location = new Point(480, 12);
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Size = new Size(100, 36);
            _btnCancel.TabIndex = 0;
            _btnCancel.Text = "취소";
            _btnCancel.UseVisualStyleBackColor = false;
            _btnCancel.Click += BtnCancel_Click;
            // 
            // UpdateProgressForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.White;
            ClientSize = new Size(600, 500);
            Controls.Add(_pnlList);
            Controls.Add(_pnlBottom);
            Controls.Add(_pnlProgress);
            Controls.Add(_pnlTitleBar);
            FormBorderStyle = FormBorderStyle.None;
            Name = "UpdateProgressForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "nU3 Framework Updater";
            _pnlTitleBar.ResumeLayout(false);
            _pnlTitleBar.PerformLayout();
            _pnlProgress.ResumeLayout(false);
            _pnlList.ResumeLayout(false);
            _pnlBottom.ResumeLayout(false);
            ResumeLayout(false);
        }

        // Window Dragging Support
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern bool ReleaseCapture();
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private void PnlTitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void LblTitleText_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void ListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }
    }
}