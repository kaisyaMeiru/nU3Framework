using nU3.Core.Attributes;
using nU3.Core.UI;
using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace nU3.Modules.ADM.AD.Deployer
{
    [nU3ProgramInfo(typeof(DeployerWorkControl), "ADM AD Deployer", "ADM_AD_00001")]
    public class DeployerWorkControl : BaseWorkControl
    {
        private Label label1;
        
        public DeployerWorkControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            label1 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(100, 23);
            label1.TabIndex = 0;
            // 
            // DeployerWorkControl
            // 
            Controls.Add(label1);
            Name = "DeployerWorkControl";
            Size = new Size(1528, 850);
            ResumeLayout(false);
        }
    }
}
