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
    public partial class PATIENT_INFO : Form
    {
        public PATIENT_INFO()
        {
            InitializeComponent();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                // Ask for confirmation before clearing
                DialogResult result = MessageBox.Show("Are you sure you want to clear all form data?",
                                                    "Confirm Clear",
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    ClearAllFields();
                    MessageBox.Show("Form cleared successfully.", "Clear Complete",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error clearing form: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ClearAllFields()
        {
            // Clear Patient Information
            dtpRunDate.Value = DateTime.Now;
            txtPatientId.Clear();
            txtSID.Clear();
            txtName.Clear();
            numAge.Value = 0;
            cmbSex.SelectedIndex = -1;
            txtRefBy.Clear();

            // Clear Hematology Left Panel
            txtHemoglobin.Clear();
            txtESR.Clear();
            txtWBC.Clear();
            txtPlateletCount.Clear();
            txtCirculatingEosinophil.Clear();
            txtHematocrit.Clear();
            txtMCH.Clear();
            txtRBC.Clear();
            txtRDW_CV.Clear();
            txtRDW_SD.Clear();
            txtBloodFilmEntry.Clear();
             
            // Clear Hematology Right Panel
            txtNeutrophils.Clear();
            txtLymphocytes.Clear();
            txtMonocytes.Clear();
            txtEosinophils.Clear();
            txtBasophils.Clear();
            txtMCV.Clear();
            txtMCHC.Clear();
            txtPDW_CV.Clear();
            txtMPV.Clear();
            txtPCT.Clear();

            // Clear Additional Controls
            txtNEU.Clear();
            txtLBAB.Clear();
            chkCheckOnComments.Checked = false;
            txtNucleatedParticles.Clear();
            txtBleedingTime.Clear();
            txtClottingTime.Clear();
            txtPCV.Clear();
            txtPCCE.Clear();

            // Reset Auto Print checkbox
            chkAutoPrint.Checked = false;

            // Set focus to the first field
            txtPatientId.Focus();
        }
    }
}
