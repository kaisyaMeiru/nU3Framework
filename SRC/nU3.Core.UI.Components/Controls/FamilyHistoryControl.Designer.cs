namespace nU3.Core.UI.Components.Controls
{
    partial class FamilyHistoryControl
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
            gridControl1 = new nU3.Core.UI.Controls.nU3GridControl();
            gridView1 = new nU3.Core.UI.Controls.nU3GridView();
            colRelation = new nU3.Core.UI.Controls.nU3GridColumn();
            colDiseaseName = new nU3.Core.UI.Controls.nU3GridColumn();
            colDiagnosisDate = new nU3.Core.UI.Controls.nU3GridColumn();
            colStatus = new nU3.Core.UI.Controls.nU3GridColumn();
            colNote = new nU3.Core.UI.Controls.nU3GridColumn();
            ((System.ComponentModel.ISupportInitialize)gridControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridView1).BeginInit();
            SuspendLayout();
            // 
            // gridControl1
            // 
            gridControl1.Dock = DockStyle.Fill;
            gridControl1.Location = new Point(0, 0);
            gridControl1.MainView = gridView1;
            gridControl1.Margin = new Padding(3, 4, 3, 4);
            gridControl1.Name = "gridControl1";
            gridControl1.Size = new Size(1463, 931);
            gridControl1.TabIndex = 0;
            gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gridView1 });
            // 
            // gridView1
            // 
            gridView1.Columns.AddRange(new nU3.Core.UI.Controls.nU3GridColumn[] { colRelation, colDiseaseName, colDiagnosisDate, colStatus, colNote });
            gridView1.DetailHeight = 437;
            gridView1.GridControl = gridControl1;
            gridView1.Name = "gridView1";
            gridView1.OptionsBehavior.Editable = false;
            gridView1.OptionsView.ShowGroupPanel = false;
            gridView1.OptionsView.ShowIndicator = false;
            // 
            // colRelation
            // 
            colRelation.Caption = "관계";
            colRelation.FieldName = "Relation";
            colRelation.Name = "colRelation";
            colRelation.Visible = true;
            colRelation.VisibleIndex = 0;
            colRelation.Width = 100;
            // 
            // colDiseaseName
            // 
            colDiseaseName.Caption = "진단명";
            colDiseaseName.FieldName = "DiseaseName";
            colDiseaseName.Name = "colDiseaseName";
            colDiseaseName.Visible = true;
            colDiseaseName.VisibleIndex = 1;
            colDiseaseName.Width = 200;
            // 
            // colDiagnosisDate
            // 
            colDiagnosisDate.Caption = "진단일자";
            colDiagnosisDate.FieldName = "DiagnosisDate";
            colDiagnosisDate.Name = "colDiagnosisDate";
            colDiagnosisDate.Visible = true;
            colDiagnosisDate.VisibleIndex = 2;
            colDiagnosisDate.Width = 120;
            // 
            // colStatus
            // 
            colStatus.Caption = "상태";
            colStatus.FieldName = "Status";
            colStatus.Name = "colStatus";
            colStatus.Visible = true;
            colStatus.VisibleIndex = 3;
            colStatus.Width = 100;
            // 
            // colNote
            // 
            colNote.Caption = "비고";
            colNote.FieldName = "Note";
            colNote.Name = "colNote";
            colNote.Visible = true;
            colNote.VisibleIndex = 4;
            colNote.Width = 200;
            // 
            // FamilyHistoryControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(gridControl1);
            Margin = new Padding(3, 4, 3, 4);
            Name = "FamilyHistoryControl";
            Size = new Size(1463, 931);
            ((System.ComponentModel.ISupportInitialize)gridControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridView1).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private nU3.Core.UI.Controls.nU3GridControl gridControl1;
        private nU3.Core.UI.Controls.nU3GridView gridView1;
        private nU3.Core.UI.Controls.nU3GridColumn colRelation;
        private nU3.Core.UI.Controls.nU3GridColumn colDiseaseName;
        private nU3.Core.UI.Controls.nU3GridColumn colDiagnosisDate;
        private nU3.Core.UI.Controls.nU3GridColumn colStatus;
        private nU3.Core.UI.Controls.nU3GridColumn colNote;
    }
}
