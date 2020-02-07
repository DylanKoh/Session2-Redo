using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Session2_Redo
{
    //Initial Commit
    public partial class EMR : Form
    {
        long _assetID = 0;
        public EMR(long assetID)
        {
            InitializeComponent();
            _assetID = assetID;
        }

        private void EMR_Load(object sender, EventArgs e)
        {
            using (var context  = new Session2Entities())
            {
                var getAsset = (from x in context.Assets
                                where x.ID == _assetID
                                select x).First();
                lblSN.Text = getAsset.AssetSN;
                lblName.Text = getAsset.AssetName;
                lblDepartment.Text = getAsset.DepartmentLocation.Department.Name;

                var getPriorities = (from x in context.Priorities
                                     select x.Name);
                cbPriority.Items.Clear();
                foreach (var item in getPriorities)
                {
                    cbPriority.Items.Add(item);
                }
            }
        }

        private void bntCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (rtxtDescription.Text.Trim() == "")
            {
                MessageBox.Show("Please ensure description of emergency maintenance is keyed in!", "Missing details",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (cbPriority.SelectedItem == null)
            {
                MessageBox.Show("Please select priority level!", "Missing details",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                using (var context = new Session2Entities())
                {
                    var priority = cbPriority.SelectedItem.ToString();
                    var getPriorityID = (from x in context.Priorities
                                         where x.Name == priority
                                         select x.ID).First();
                    context.EmergencyMaintenances.Add(new EmergencyMaintenance()
                    {
                        AssetID = _assetID,
                        EMReportDate = DateTime.Now,
                        DescriptionEmergency = rtxtDescription.Text,
                        PriorityID = getPriorityID,
                        OtherConsiderations = rtxtOther.Text
                    });
                    context.SaveChanges();
                }
                Close();
            }
        }
    }
}
