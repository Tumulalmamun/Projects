namespace HPS
{
    partial class Report_Viewer
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource2 = new Microsoft.Reporting.WinForms.ReportDataSource();
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.hpsDataSet = new HPS.hpsDataSet();
            this.tblHematologyReportsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tblHematologyReportsTableAdapter = new HPS.hpsDataSetTableAdapters.tblHematologyReportsTableAdapter();
            this.tblPatientInfoBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tblPatientInfoTableAdapter = new HPS.hpsDataSetTableAdapters.tblPatientInfoTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.hpsDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tblHematologyReportsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tblPatientInfoBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // reportViewer1
            // 
            reportDataSource1.Name = "hematologyReport";
            reportDataSource1.Value = this.tblHematologyReportsBindingSource;
            reportDataSource2.Name = "patientInfo";
            reportDataSource2.Value = this.tblPatientInfoBindingSource;
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource2);
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "HPS.HematologyReport.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(51, 22);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.ServerReport.BearerToken = null;
            this.reportViewer1.Size = new System.Drawing.Size(640, 344);
            this.reportViewer1.TabIndex = 0;
            // 
            // hpsDataSet
            // 
            this.hpsDataSet.DataSetName = "hpsDataSet";
            this.hpsDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // tblHematologyReportsBindingSource
            // 
            this.tblHematologyReportsBindingSource.DataMember = "tblHematologyReports";
            this.tblHematologyReportsBindingSource.DataSource = this.hpsDataSet;
            // 
            // tblHematologyReportsTableAdapter
            // 
            this.tblHematologyReportsTableAdapter.ClearBeforeFill = true;
            // 
            // tblPatientInfoBindingSource
            // 
            this.tblPatientInfoBindingSource.DataMember = "tblPatientInfo";
            this.tblPatientInfoBindingSource.DataSource = this.hpsDataSet;
            // 
            // tblPatientInfoTableAdapter
            // 
            this.tblPatientInfoTableAdapter.ClearBeforeFill = true;
            // 
            // Report_Viewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.reportViewer1);
            this.Name = "Report_Viewer";
            this.Text = "Report_Viewer";
            this.Load += new System.EventHandler(this.Report_Viewer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.hpsDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tblHematologyReportsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tblPatientInfoBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.BindingSource tblHematologyReportsBindingSource;
        private hpsDataSet hpsDataSet;
        private System.Windows.Forms.BindingSource tblPatientInfoBindingSource;
        private hpsDataSetTableAdapters.tblHematologyReportsTableAdapter tblHematologyReportsTableAdapter;
        private hpsDataSetTableAdapters.tblPatientInfoTableAdapter tblPatientInfoTableAdapter;
    }
}