using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HPS
{
    public partial class Report_Viewer : Form
    {
        public Report_Viewer()
        {
            InitializeComponent();
        }

        public void Report_Viewer_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'hpsDataSet.tblHematologyReports' table. You can move, or remove it, as needed.
            this.tblHematologyReportsTableAdapter.Fill(this.hpsDataSet.tblHematologyReports);
            // TODO: This line of code loads data into the 'hpsDataSet.tblPatientInfo' table. You can move, or remove it, as needed.
            this.tblPatientInfoTableAdapter.Fill(this.hpsDataSet.tblPatientInfo);

            this.reportViewer1.RefreshReport();
        }

        public void reportViewerLoad(int patientId,int reportId)
        {
            // TODO: This line of code loads data into the 'hpsDataSet.tblHematologyReports' table. You can move, or remove it, as needed.
            this.tblHematologyReportsTableAdapter.FillBy(this.hpsDataSet.tblHematologyReports, reportId);
            // TODO: This line of code loads data into the 'hpsDataSet.tblPatientInfo' table. You can move, or remove it, as needed.
            this.tblPatientInfoTableAdapter.Fill(this.hpsDataSet.tblPatientInfo);

            this.reportViewer1.RefreshReport();
        }
    }
}
