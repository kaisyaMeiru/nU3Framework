using System;
using System.Windows.Forms;
using nU3.Core.Attributes;
using nU3.Core.UI;

namespace nU3.Modules.EMR.IN.Worklist
{
    [nU3ProgramInfo(typeof(DoctorScheduleForm), "¿«ªÁΩ∫ƒ…¡Ï", "EMR_IN_00001", AuthLevel = 1)]
    public class DoctorScheduleForm : BaseWorkControl
    {
        private Label label1;
        
        public DoctorScheduleForm()
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
            // DoctorScheduleForm
            // 
            Controls.Add(label1);
            Name = "DoctorScheduleForm";
            Size = new Size(1305, 852);
            ResumeLayout(false);
        }
    }
}
