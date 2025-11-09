using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HPS.hpsDataSetTableAdapters;

namespace HPS
{
    public partial class PATIENT_INFO : Form
    {
        public PATIENT_INFO()
        {
            InitializeComponent();
            SetNextPatientID();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                // Ask for confirmation before clearing
                DialogResult result = MessageBox.Show("Are you sure you want to clear all form data?",
                                                    "Confirm Clear!!!",
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    ClearAllFields();
                    SetNextPatientID();
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
        private void btnSave_Click(object sender, EventArgs e)
        {
            // Validate Name
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter the patient's name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }
            if (cmbSex.SelectedIndex < 0)  // no selection
            {
                MessageBox.Show("Please select the patient's sex.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbSex.Focus();
                return;
            }


            // Validate Age
            if (numAge.Value <= 0)  // assuming age must be > 0
            {
                MessageBox.Show("Please enter a valid age.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numAge.Focus();
                return;
            }

            // If validation passes, proceed to save
            SaveData();

        }
        private void SaveData()
        {
            string connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=F:\Projects\hps.accdb;";
            using (var conn = new OleDbConnection(connStr))
            {
                try
                {
                    conn.Open();

                    // Start a transaction to ensure data consistency
                    using (OleDbTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string patientCode = txtPatientId.Text.Trim();
                            if (string.IsNullOrWhiteSpace(patientCode))
                            {
                                MessageBox.Show("Patient ID is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            long patientID = GetPatientIDByCode(conn, patientCode, transaction); // Pass transaction here

                            // Insert or Update Patient Info
                            if (patientID == -1)
                            {
                                string insertPatientSql = @"
                                    INSERT INTO tblPatientInfo 
                                    (PatientCode, FullName, Gender, Age, RegistrationDate, ContactNumber, IsActive)
                                    VALUES (?, ?, ?, ?, ?, ?, ?)";

                                using (var cmd = new OleDbCommand(insertPatientSql, conn))
                                {
                                    cmd.Transaction = transaction;
                                    cmd.Parameters.AddWithValue("@PatientCode", patientCode);
                                    cmd.Parameters.AddWithValue("@FullName", txtName.Text.Trim());
                                    cmd.Parameters.AddWithValue("@Gender", cmbSex.SelectedItem?.ToString() ?? "");
                                    cmd.Parameters.AddWithValue("@Age", Convert.ToInt32(numAge.Value));
                                    cmd.Parameters.Add("@RegistrationDate", OleDbType.Date).Value = DateTime.Now;
                                    cmd.Parameters.AddWithValue("@ContactNumber", txtContactNumber.Text.Trim());
                                    cmd.Parameters.AddWithValue("@IsActive", true);

                                    cmd.ExecuteNonQuery();
                                }

                                // Retrieve the newly inserted PatientID inside the same transaction
                                using (var idCmd = new OleDbCommand("SELECT @@IDENTITY", conn))
                                {
                                    idCmd.Transaction = transaction;
                                    object o = idCmd.ExecuteScalar();
                                    if (o != null && long.TryParse(o.ToString(), out long newId))
                                        patientID = newId;
                                    else
                                        throw new Exception("Failed to retrieve new PatientID.");
                                }
                            }

                            if (patientID == -1)
                                throw new Exception("Failed to determine PatientID.");

                            // Insert Hematology Report
                            string insertReportSql = @"
                                INSERT INTO tblHematologyReports
                                (
                                    PatientID, RunDate, AgeAtTest, SID, RefBy,
                                    Hemoglobin, Hematocrit, RBC, WBC, PlateletCount, ESR,
                                    MCH, MCV, MCHC, RDW_CV, RDW_SD, PDW_CV, MPV, PCT,
                                    Neutrophils, Lymphocytes, Monocytes, Eosinophils, Basophils,
                                    CirculatingEosinophil, NEU, LBAB, PCV, PCCE, NucleatedParticles,
                                    BleedingTime, ClottingTime, BloodFilmEntry,
                                    CheckOnComments, AutoPrint, Printed,
                                    CreatedBy, CreatedDate, IsFinal
                                )
                                VALUES
                                (
                                    ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?
                                )";
                            long reportID = -1;
                            using (var cmd = new OleDbCommand(insertReportSql, conn))
                            {
                                // assign transaction
                                cmd.Transaction = transaction;

                                //  ---- strictly match order above ----
                                cmd.Parameters.Add("PatientID", OleDbType.Integer).Value = patientID; // example FK
                                cmd.Parameters.Add("RunDate", OleDbType.Date).Value = dtpRunDate.Value;
                                cmd.Parameters.Add("AgeAtTest", OleDbType.Integer).Value = (int)numAge.Value;
                                cmd.Parameters.Add("SID", OleDbType.VarChar).Value = txtSID.Text.Trim();
                                cmd.Parameters.Add("RefBy", OleDbType.VarChar).Value = txtRefBy.Text.Trim();

                                cmd.Parameters.Add("Hemoglobin", OleDbType.Double).Value = ParseDoubleOrNull(txtHemoglobin.Text);
                                cmd.Parameters.Add("Hematocrit", OleDbType.Double).Value = ParseDoubleOrNull(txtHematocrit.Text);
                                cmd.Parameters.Add("RBC", OleDbType.Double).Value = ParseDoubleOrNull(txtRBC.Text);
                                cmd.Parameters.Add("WBC", OleDbType.Double).Value = ParseDoubleOrNull(txtWBC.Text);
                                cmd.Parameters.Add("PlateletCount", OleDbType.Double).Value = ParseDoubleOrNull(txtPlateletCount.Text);
                                cmd.Parameters.Add("ESR", OleDbType.Double).Value = ParseDoubleOrNull(txtESR.Text);
                                cmd.Parameters.Add("MCH", OleDbType.Double).Value = ParseDoubleOrNull(txtMCH.Text);
                                cmd.Parameters.Add("MCV", OleDbType.Double).Value = ParseDoubleOrNull(txtMCV.Text);
                                cmd.Parameters.Add("MCHC", OleDbType.Double).Value = ParseDoubleOrNull(txtMCHC.Text);
                                cmd.Parameters.Add("RDW_CV", OleDbType.Double).Value = ParseDoubleOrNull(txtRDW_CV.Text);
                                cmd.Parameters.Add("RDW_SD", OleDbType.Double).Value = ParseDoubleOrNull(txtRDW_SD.Text);
                                cmd.Parameters.Add("PDW_CV", OleDbType.Double).Value = ParseDoubleOrNull(txtPDW_CV.Text);
                                cmd.Parameters.Add("MPV", OleDbType.Double).Value = ParseDoubleOrNull(txtMPV.Text);
                                cmd.Parameters.Add("PCT", OleDbType.Double).Value = ParseDoubleOrNull(txtPCT.Text);
                                cmd.Parameters.Add("Neutrophils", OleDbType.Double).Value = ParseDoubleOrNull(txtNeutrophils.Text);
                                cmd.Parameters.Add("Lymphocytes", OleDbType.Double).Value = ParseDoubleOrNull(txtLymphocytes.Text);
                                cmd.Parameters.Add("Monocytes", OleDbType.Double).Value = ParseDoubleOrNull(txtMonocytes.Text);
                                cmd.Parameters.Add("Eosinophils", OleDbType.Double).Value = ParseDoubleOrNull(txtEosinophils.Text);
                                cmd.Parameters.Add("Basophils", OleDbType.Double).Value = ParseDoubleOrNull(txtBasophils.Text);
                                cmd.Parameters.Add("CirculatingEosinophil", OleDbType.Double).Value = ParseDoubleOrNull(txtCirculatingEosinophil.Text);
                                cmd.Parameters.Add("NEU", OleDbType.Double).Value = ParseDoubleOrNull(txtNEU.Text);
                                cmd.Parameters.Add("LBAB", OleDbType.Double).Value = ParseDoubleOrNull(txtLBAB.Text);
                                cmd.Parameters.Add("PCV", OleDbType.Double).Value = ParseDoubleOrNull(txtPCV.Text);
                                cmd.Parameters.Add("PCCE", OleDbType.Double).Value = ParseDoubleOrNull(txtPCCE.Text);
                                cmd.Parameters.Add("NucleatedParticles", OleDbType.Double).Value = ParseDoubleOrNull(txtNucleatedParticles.Text);
                                cmd.Parameters.Add("BleedingTime", OleDbType.Double).Value = ParseDoubleOrNull(txtBleedingTime.Text);
                                cmd.Parameters.Add("ClottingTime", OleDbType.Double).Value = ParseDoubleOrNull(txtClottingTime.Text);
                                cmd.Parameters.Add("BloodFilmEntry", OleDbType.VarChar).Value = txtBloodFilmEntry.Text.Trim();
                                cmd.Parameters.Add("CheckOnComments", OleDbType.Boolean).Value = chkCheckOnComments.Checked;
                                cmd.Parameters.Add("AutoPrint", OleDbType.Boolean).Value = chkAutoPrint.Checked;
                                cmd.Parameters.Add("Printed", OleDbType.Boolean).Value = false;
                                // Added missing CreatedBy (must match SQL order)
                                cmd.Parameters.Add("CreatedBy", OleDbType.VarChar).Value = Environment.UserName;
                                cmd.Parameters.Add("CreatedDate", OleDbType.Date).Value = DateTime.Now;
                                cmd.Parameters.Add("IsFinal", OleDbType.Boolean).Value = true;

                                cmd.ExecuteNonQuery();
                                // retrieve report id (AUTONUMBER) inside same transaction
                                using (var idCmd = new OleDbCommand("SELECT @@IDENTITY", conn))
                                {
                                    idCmd.Transaction = transaction;
                                    object o2 = idCmd.ExecuteScalar();
                                    if (o2 != null && long.TryParse(o2.ToString(), out long newReportId))
                                    {
                                        reportID = newReportId;
                                        txtReportId.Text = reportID.ToString();
                                    }
                                }
                            }
                            // Commit the transaction
                            transaction.Commit();

                            MessageBox.Show("✅ Patient and Hematology Report saved successfully!", 
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            

                            // After successful save, clear the form and set new ID
                            ClearAllFields();
                            SetNextPatientID();
                        }
                        catch (Exception ex)
                        {
                            // Rollback the transaction if there's an error
                            try { transaction.Rollback(); } catch { /* ignore */ }
                            throw ex;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving data:\n{ex.Message}", 
                        "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void xxSaveData()
        {
            string connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=F:\Projects\hps.accdb;";
            using (var conn = new OleDbConnection(connStr))
            {
                try
                {
                    conn.Open();

                    using (OleDbTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string patientCode = txtPatientId.Text.Trim();
                            if (string.IsNullOrWhiteSpace(patientCode))
                            {
                                MessageBox.Show("Patient ID is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            // Try to get existing patient id within transaction
                            long patientID = GetPatientIDByCode(conn, patientCode, transaction);

                            // If patient doesn't exist, insert and retrieve identity (inside same transaction)
                            if (patientID == -1)
                            {
                                string insertPatientSql = @"
                            INSERT INTO tblPatientInfo 
                            (PatientCode, FullName, Gender, Age, RegistrationDate, ContactNumber, IsActive)
                            VALUES (?, ?, ?, ?, ?, ?, ?)";

                                using (var cmd = new OleDbCommand(insertPatientSql, conn))
                                {
                                    cmd.Transaction = transaction;

                                    cmd.Parameters.AddWithValue("@PatientCode", patientCode);
                                    cmd.Parameters.AddWithValue("@FullName", txtName.Text.Trim());
                                    cmd.Parameters.AddWithValue("@Gender", cmbSex.SelectedItem?.ToString() ?? "");
                                    cmd.Parameters.AddWithValue("@Age", Convert.ToInt32(numAge.Value));
                                    cmd.Parameters.Add("@RegistrationDate", OleDbType.Date).Value = DateTime.Now;
                                    cmd.Parameters.AddWithValue("@ContactNumber", txtContactNumber.Text.Trim());
                                    cmd.Parameters.AddWithValue("@IsActive", true);

                                    cmd.ExecuteNonQuery();
                                }

                                // Get last inserted AUTONUMBER (must run inside the same transaction)
                                using (var idCmd = new OleDbCommand("SELECT @@IDENTITY", conn))
                                {
                                    idCmd.Transaction = transaction;
                                    object o = idCmd.ExecuteScalar();
                                    if (o != null && long.TryParse(o.ToString(), out long newId))
                                        patientID = newId;
                                    else
                                        throw new Exception("Failed to retrieve new PatientID.");
                                }
                            }

                            if (patientID == -1)
                                throw new Exception("Patient ID not found or created.");

                            // ============================
                            // Insert Hematology Report
                            // ============================
                            string insertReportSql = @"
                        INSERT INTO tblHematologyReports
                        (
                            PatientID, RunDate, AgeAtTest, SID, RefBy,
                            Hemoglobin, Hematocrit, RBC, WBC, PlateletCount, ESR,
                            MCH, MCV, MCHC, RDW_CV, RDW_SD, PDW_CV, MPV, PCT,
                            Neutrophils, Lymphocytes, Monocytes, Eosinophils, Basophils,
                            CirculatingEosinophil, NEU, LBAB, PCV, PCCE,
                            NucleatedParticles, BleedingTime, ClottingTime, BloodFilmEntry,
                            CheckOnComments, AutoPrint, Printed, CreatedBy, CreatedDate, IsFinal
                        )
                        VALUES
                        (
                            ?, ?, ?, ?, ?,
                            ?, ?, ?, ?, ?, ?,
                            ?, ?, ?, ?, ?, ?, ?, ?, ?,
                            ?, ?, ?, ?, ?,
                            ?, ?, ?, ?, ?,
                            ?, ?, ?, ?, ?, ?, ?, ?, ?, ?
                        )";

                            using (var cmd = new OleDbCommand(insertReportSql, conn))
                            {
                                // IMPORTANT: assign the same transaction to the command
                                cmd.Transaction = transaction;

                                // Parameters MUST be added in exact order as VALUES placeholders
                                cmd.Parameters.Add("PatientID", OleDbType.Integer).Value = patientID;
                                cmd.Parameters.Add("RunDate", OleDbType.Date).Value = dtpRunDate.Value;
                                cmd.Parameters.Add("AgeAtTest", OleDbType.Integer).Value = (int)numAge.Value;
                                cmd.Parameters.Add("SID", OleDbType.VarChar).Value = txtSID.Text.Trim();
                                cmd.Parameters.Add("RefBy", OleDbType.VarChar).Value = txtRefBy.Text.Trim();

                                cmd.Parameters.Add("Hemoglobin", OleDbType.Double).Value = ParseDoubleOrDBNull(txtHemoglobin.Text);
                                cmd.Parameters.Add("Hematocrit", OleDbType.Double).Value = ParseDoubleOrDBNull(txtHematocrit.Text);
                                cmd.Parameters.Add("RBC", OleDbType.Double).Value = ParseDoubleOrDBNull(txtRBC.Text);
                                cmd.Parameters.Add("WBC", OleDbType.Double).Value = ParseDoubleOrDBNull(txtWBC.Text);
                                cmd.Parameters.Add("PlateletCount", OleDbType.Double).Value = ParseDoubleOrDBNull(txtPlateletCount.Text);
                                cmd.Parameters.Add("ESR", OleDbType.Double).Value = ParseDoubleOrDBNull(txtESR.Text);

                                cmd.Parameters.Add("MCH", OleDbType.Double).Value = ParseDoubleOrDBNull(txtMCH.Text);
                                cmd.Parameters.Add("MCV", OleDbType.Double).Value = ParseDoubleOrDBNull(txtMCV.Text);
                                cmd.Parameters.Add("MCHC", OleDbType.Double).Value = ParseDoubleOrDBNull(txtMCHC.Text);
                                cmd.Parameters.Add("RDW_CV", OleDbType.Double).Value = ParseDoubleOrDBNull(txtRDW_CV.Text);
                                cmd.Parameters.Add("RDW_SD", OleDbType.Double).Value = ParseDoubleOrDBNull(txtRDW_SD.Text);
                                cmd.Parameters.Add("PDW_CV", OleDbType.Double).Value = ParseDoubleOrDBNull(txtPDW_CV.Text);
                                cmd.Parameters.Add("MPV", OleDbType.Double).Value = ParseDoubleOrDBNull(txtMPV.Text);
                                cmd.Parameters.Add("PCT", OleDbType.Double).Value = ParseDoubleOrDBNull(txtPCT.Text);

                                cmd.Parameters.Add("Neutrophils", OleDbType.Double).Value = ParseDoubleOrDBNull(txtNeutrophils.Text);
                                cmd.Parameters.Add("Lymphocytes", OleDbType.Double).Value = ParseDoubleOrDBNull(txtLymphocytes.Text);
                                cmd.Parameters.Add("Monocytes", OleDbType.Double).Value = ParseDoubleOrDBNull(txtMonocytes.Text);
                                cmd.Parameters.Add("Eosinophils", OleDbType.Double).Value = ParseDoubleOrDBNull(txtEosinophils.Text);
                                cmd.Parameters.Add("Basophils", OleDbType.Double).Value = ParseDoubleOrDBNull(txtBasophils.Text);

                                cmd.Parameters.Add("CirculatingEosinophil", OleDbType.Double).Value = ParseDoubleOrDBNull(txtCirculatingEosinophil.Text);
                                cmd.Parameters.Add("NEU", OleDbType.Double).Value = ParseDoubleOrDBNull(txtNEU.Text);
                                cmd.Parameters.Add("LBAB", OleDbType.Double).Value = ParseDoubleOrDBNull(txtLBAB.Text);
                                cmd.Parameters.Add("PCV", OleDbType.Double).Value = ParseDoubleOrDBNull(txtPCV.Text);
                                cmd.Parameters.Add("PCCE", OleDbType.Double).Value = ParseDoubleOrDBNull(txtPCCE.Text);

                                cmd.Parameters.Add("NucleatedParticles", OleDbType.Double).Value = ParseDoubleOrDBNull(txtNucleatedParticles.Text);
                                cmd.Parameters.Add("BleedingTime", OleDbType.Double).Value = ParseDoubleOrDBNull(txtBleedingTime.Text);
                                cmd.Parameters.Add("ClottingTime", OleDbType.Double).Value = ParseDoubleOrDBNull(txtClottingTime.Text);
                                cmd.Parameters.Add("BloodFilmEntry", OleDbType.VarChar).Value = txtBloodFilmEntry.Text.Trim();

                                cmd.Parameters.Add("CheckOnComments", OleDbType.Boolean).Value = chkCheckOnComments.Checked;
                                cmd.Parameters.Add("AutoPrint", OleDbType.Boolean).Value = chkAutoPrint.Checked;
                                cmd.Parameters.Add("Printed", OleDbType.Boolean).Value = false;

                                cmd.Parameters.Add("CreatedBy", OleDbType.VarChar).Value = Environment.UserName;
                                cmd.Parameters.Add("CreatedDate", OleDbType.Date).Value = DateTime.Now;
                                cmd.Parameters.Add("IsFinal", OleDbType.Boolean).Value = true;

                                cmd.ExecuteNonQuery();
                            }

                            // All good — commit
                            transaction.Commit();

                            MessageBox.Show("✅ Patient and Hematology Report saved successfully!",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Optionally clear & prepare form
                            ClearAllFields();
                            SetNextPatientID();
                        }
                        catch
                        {
                            // If anything fails, rollback so DB is consistent
                            try { transaction.Rollback(); } catch { /* ignore rollback errors */ }
                            throw; // rethrow to outer catch for message
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving data:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ✅ Helper: Parse Double or Return DBNull
        private object ParseDoubleOrNull(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return DBNull.Value;
            if (double.TryParse(input, out double val))
                return val;
            return DBNull.Value;
        }

        // ✅ Helper: Get Patient ID by PatientCode
        private long GetPatientIDByCode(OleDbConnection conn, string patientCode, OleDbTransaction transaction = null)
        {
            string sql = "SELECT PatientID FROM tblPatientInfo WHERE PatientCode = ?";
            using (var cmd = new OleDbCommand(sql, conn))
            {
                if (transaction != null)
                    cmd.Transaction = transaction;
                cmd.Parameters.AddWithValue("@PatientCode", patientCode);
                var result = cmd.ExecuteScalar();
                return (result != null && long.TryParse(result.ToString(), out long id)) ? id : -1;
            }
        }
        private void SetNextPatientID()
        {
            using (var conn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=F:\Projects\hps.accdb;"))
            {
                conn.Open();
                long nextID = GetNextPatientID(conn);
                this.txtPatientId.Text = nextID.ToString();
            }
        }

        // Method to get the next PatientID from the database
        private long GetNextPatientID(OleDbConnection conn)
        {
            string sql = "SELECT MAX(PatientID) FROM tblPatientInfo";
            using (var cmd = new OleDbCommand(sql, conn))
            {
                var result = cmd.ExecuteScalar();
                if (result != null && long.TryParse(result.ToString(), out long maxID))
                {
                    return maxID + 1; // next available ID
                }
                else
                {
                    return 1; // if table is empty, start from 1
                }
            }
        }

       

        private void SearchPatientByContact(string contactNumber)
        {
            string connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=F:\Projects\hps.accdb;";
            using (var conn = new OleDbConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string sql = @"SELECT PatientID, PatientCode, FullName, Gender, Age, ContactNumber 
                                 FROM tblPatientInfo 
                                 WHERE ContactNumber = ?";

                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ContactNumber", contactNumber);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Found existing patient - populate fields
                                txtPatientId.Text = reader["PatientCode"].ToString();
                                txtName.Text = reader["FullName"].ToString();
                                cmbSex.SelectedItem = reader["Gender"].ToString();
                                numAge.Value = Convert.ToDecimal(reader["Age"]);
                            }
                            else
                            {
                                // No existing patient found - clear fields for new entry
                                ClearAllFields();
                                txtContactNumber.Text = contactNumber;
                                SetNextPatientID();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error searching patient: {ex.Message}", 
                        "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private long xxGetPatientIDByCode(OleDbConnection conn, string patientCode, OleDbTransaction transaction = null)
        {
            string sql = "SELECT PatientID FROM tblPatientInfo WHERE PatientCode = ?";
            using (var cmd = new OleDbCommand(sql, conn))
            {
                if (transaction != null) cmd.Transaction = transaction;
                cmd.Parameters.AddWithValue("@PatientCode", patientCode);
                object result = cmd.ExecuteScalar();
                return (result != null && long.TryParse(result.ToString(), out long id)) ? id : -1;
            }
        }

        // Helper: return double or DBNull.Value
        private object ParseDoubleOrDBNull(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return DBNull.Value;
            if (double.TryParse(input, out double val)) return val;
            return DBNull.Value;
        }

        private void txtContactNumber_TextChanged(object sender, EventArgs e)
        {
           if (string.IsNullOrWhiteSpace(txtContactNumber.Text) || txtContactNumber.Text.Length < 11)
                return;

                SearchPatientByContact(txtContactNumber.Text.Trim());
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Report_Viewer rv = new Report_Viewer();
            rv.reportViewerLoad(Convert.ToInt32(txtPatientId.Text), Convert.ToInt32(txtReportId.Text));
        }
    }
}

