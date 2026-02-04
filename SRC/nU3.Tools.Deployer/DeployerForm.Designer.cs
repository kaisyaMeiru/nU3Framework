namespace nU3.Tools.Deployer
{
    partial class DeployerForm
    {
        private System.ComponentModel.IContainer components = null;

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
            barStatusBar = new DevExpress.XtraBars.Bar();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            toolStripButtonTestServer = new System.Windows.Forms.ToolStripButton();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // barStatusBar
            // 
            barStatusBar.BarName = "Status Bar";
            barStatusBar.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            barStatusBar.DockCol = 0;
            barStatusBar.DockRow = 0;
            barStatusBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            barStatusBar.OptionsBar.AllowQuickCustomization = false;
            barStatusBar.OptionsBar.DrawDragBorder = false;
            barStatusBar.OptionsBar.UseWholeRow = true;
            barStatusBar.Text = "Status Bar";
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabel1, toolStripButtonTestServer });
            statusStrip1.Location = new System.Drawing.Point(0, 946);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new System.Drawing.Size(917, 22);
            statusStrip1.TabIndex = 0;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new System.Drawing.Size(102, 17);
            toolStripStatusLabel1.Text = "서버 상태: 대기중";
            // 
            // toolStripButtonTestServer
            // 
            toolStripButtonTestServer.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            toolStripButtonTestServer.Name = "toolStripButtonTestServer";
            toolStripButtonTestServer.Size = new System.Drawing.Size(103, 20);
            toolStripButtonTestServer.Text = "서버 연결 테스트";
            // 
            // DeployerForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(917, 968);
            Controls.Add(statusStrip1);
            MinimumSize = new System.Drawing.Size(800, 1000);
            Name = "DeployerForm";
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
        private DevExpress.XtraBars.Bar barStatusBar;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripButton toolStripButtonTestServer;
    }
}
