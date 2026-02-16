namespace nU3.Modules.NUR.NR.NURNAMESPACE
{
    partial class NURPROGRAMID
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
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            patientInfoControl1 = new nU3.Core.UI.Components.Controls.PatientInfoControl();
            patientListControl1 = new nU3.Core.UI.Components.Controls.PatientListControl();
            familyHistoryControl1 = new nU3.Core.UI.Components.Controls.FamilyHistoryControl();
            SuspendLayout();
            // 
            // patientInfoControl1
            // 
            patientInfoControl1.Dock = System.Windows.Forms.DockStyle.Top;
            patientInfoControl1.EventBus = null;
            patientInfoControl1.EventBusUse = false;
            patientInfoControl1.EventSource = "BaseWorkComponent";
            patientInfoControl1.Location = new System.Drawing.Point(0, 0);
            patientInfoControl1.Margin = new System.Windows.Forms.Padding(4);
            patientInfoControl1.Name = "patientInfoControl1";
            patientInfoControl1.OwnerEventBus = null;
            patientInfoControl1.OwnerProgramID = null;
            patientInfoControl1.Size = new System.Drawing.Size(1244, 30);
            patientInfoControl1.TabIndex = 1;
            // 
            // patientListControl1
            // 
            patientListControl1.Dock = System.Windows.Forms.DockStyle.Left;
            patientListControl1.EventBus = null;
            patientListControl1.EventBusUse = true;
            patientListControl1.EventSource = "BaseWorkComponent";
            patientListControl1.Location = new System.Drawing.Point(0, 30);
            patientListControl1.Margin = new System.Windows.Forms.Padding(4);
            patientListControl1.Name = "patientListControl1";
            patientListControl1.OwnerEventBus = null;
            patientListControl1.OwnerProgramID = null;
            patientListControl1.Size = new System.Drawing.Size(500, 634);
            patientListControl1.TabIndex = 2;
            patientListControl1.PatientSelected += patientListControl1_PatientSelected;
            // 
            // familyHistoryControl1
            // 
            familyHistoryControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            familyHistoryControl1.EventBus = null;
            familyHistoryControl1.EventBusUse = false;
            familyHistoryControl1.EventSource = "BaseWorkComponent";
            familyHistoryControl1.Location = new System.Drawing.Point(500, 30);
            familyHistoryControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            familyHistoryControl1.Name = "familyHistoryControl1";
            familyHistoryControl1.OwnerEventBus = null;
            familyHistoryControl1.OwnerProgramID = null;
            familyHistoryControl1.Size = new System.Drawing.Size(744, 634);
            familyHistoryControl1.TabIndex = 3;
            // 
            // NURPROGRAMID
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(familyHistoryControl1);
            Controls.Add(patientListControl1);
            Controls.Add(patientInfoControl1);
            Name = "NURPROGRAMID";
            Size = new System.Drawing.Size(1244, 664);
            ResumeLayout(false);
        }

        #endregion
        private Core.UI.Components.Controls.PatientInfoControl patientInfoControl1;
        private Core.UI.Components.Controls.PatientListControl patientListControl1;
        private Core.UI.Components.Controls.FamilyHistoryControl familyHistoryControl1;
    }
}
