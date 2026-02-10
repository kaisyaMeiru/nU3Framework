namespace nU3.Modules.OCS.IN.MainEntry.Controls
{
    partial class SendMemoControl
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            grpSendMemo = new nU3.Core.UI.Controls.nU3GroupControl();
            pnlButtons = new nU3.Core.UI.Controls.nU3PanelControl();
            btnMemoResv = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnSave = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnClear = new nU3.Core.UI.Controls.nU3SimpleButton();
            memoEdit = new nU3.Core.UI.Controls.nU3MemoEdit();
            ((System.ComponentModel.ISupportInitialize)grpSendMemo).BeginInit();
            grpSendMemo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlButtons).BeginInit();
            pnlButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)memoEdit.Properties).BeginInit();
            SuspendLayout();
            // 
            // grpSendMemo
            // 
            grpSendMemo.Controls.Add(pnlButtons);
            grpSendMemo.Controls.Add(memoEdit);
            grpSendMemo.Dock = DockStyle.Fill;
            grpSendMemo.Location = new Point(0, 0);
            grpSendMemo.Margin = new Padding(4);
            grpSendMemo.Name = "grpSendMemo";
            grpSendMemo.Size = new Size(1406, 750);
            grpSendMemo.TabIndex = 0;
            grpSendMemo.Text = "전달메모";
            // 
            // pnlButtons
            // 
            pnlButtons.Controls.Add(btnMemoResv);
            pnlButtons.Controls.Add(btnSave);
            pnlButtons.Controls.Add(btnClear);
            pnlButtons.Dock = DockStyle.Top;
            pnlButtons.Location = new Point(2, 23);
            pnlButtons.Margin = new Padding(4);
            pnlButtons.Name = "pnlButtons";
            pnlButtons.Size = new Size(1402, 38);
            pnlButtons.TabIndex = 1;
            // 
            // btnMemoResv
            // 
            btnMemoResv.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMemoResv.AuthId = "";
            btnMemoResv.Location = new Point(1306, 4);
            btnMemoResv.Margin = new Padding(4);
            btnMemoResv.Name = "btnMemoResv";
            btnMemoResv.Size = new Size(88, 29);
            btnMemoResv.TabIndex = 2;
            btnMemoResv.Text = "예약문구";
            btnMemoResv.Click += btnMemoResv_Click;
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSave.AuthId = "";
            btnSave.Location = new Point(1211, 4);
            btnSave.Margin = new Padding(4);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(88, 29);
            btnSave.TabIndex = 1;
            btnSave.Text = "저장";
            btnSave.Click += btnSave_Click;
            // 
            // btnClear
            // 
            btnClear.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClear.AuthId = "";
            btnClear.Location = new Point(1117, 4);
            btnClear.Margin = new Padding(4);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(88, 29);
            btnClear.TabIndex = 0;
            btnClear.Text = "지우기";
            btnClear.Click += btnClear_Click;
            // 
            // memoEdit
            // 
            memoEdit.Dock = DockStyle.Fill;
            memoEdit.IsRequired = false;
            memoEdit.Location = new Point(2, 23);
            memoEdit.Margin = new Padding(4);
            memoEdit.Name = "memoEdit";
            memoEdit.Properties.Appearance.Options.UseFont = true;
            memoEdit.Properties.MaxLength = 2000;
            memoEdit.Size = new Size(1402, 725);
            memoEdit.TabIndex = 0;
            // 
            // SendMemoControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(grpSendMemo);
            Margin = new Padding(4);
            Name = "SendMemoControl";
            Size = new Size(1406, 750);
            ((System.ComponentModel.ISupportInitialize)grpSendMemo).EndInit();
            grpSendMemo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pnlButtons).EndInit();
            pnlButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)memoEdit.Properties).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private nU3.Core.UI.Controls.nU3GroupControl grpSendMemo;
        private nU3.Core.UI.Controls.nU3PanelControl pnlButtons;
        private nU3.Core.UI.Controls.nU3SimpleButton btnMemoResv;
        private nU3.Core.UI.Controls.nU3SimpleButton btnSave;
        private nU3.Core.UI.Controls.nU3SimpleButton btnClear;
        private nU3.Core.UI.Controls.nU3MemoEdit memoEdit;
    }
}