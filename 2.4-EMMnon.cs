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
    public partial class EMMnon : Form
    {
        long _id = 0;
        public EMMnon(long id)
        {
            InitializeComponent();
            _id = id;
        }

        private void EMMnon_Load(object sender, EventArgs e)
        {
            using (var context = new Session2Entities())
            {
                var getAssetsRelated = (from x in context.Assets
                                        where x.EmployeeID == _id
                                        select x);
                LoadGrid();
                foreach (var item in getAssetsRelated)
                {
                    var rows = new List<string>()
                    {
                        item.AssetSN, item.AssetName
                    };
                    var getEMCount = (from x in context.EmergencyMaintenances
                                      where x.AssetID == item.ID && x.EMEndDate != null
                                      select x).Count();

                    var getEMLast = (from x in context.EmergencyMaintenances
                                     where x.AssetID == item.ID && x.EMEndDate != null
                                     orderby x.EMEndDate descending
                                     select x.EMEndDate).FirstOrDefault();
                    rows.Add(Convert.ToDateTime(getEMLast).ToString("dd/MM/yyyy"));
                    rows.Add(getEMCount.ToString());
                    rows.Add(item.ID.ToString());
                    dataGridView1.Rows.Add(rows.ToArray());
                }

                foreach (DataGridViewRow item in dataGridView1.Rows)
                {
                    var assetID = Convert.ToInt64(item.Cells[4].Value);
                    var checkCurrent = (from x in context.EmergencyMaintenances
                                        where x.AssetID == assetID && x.EMEndDate == null
                                        select x).FirstOrDefault();
                    if (checkCurrent != null)
                    {
                        item.DefaultCellStyle.BackColor = Color.Red;
                    }
                }
            }
        }
        private void LoadGrid()
        {
            dataGridView1.ColumnCount = 5;
            dataGridView1.Columns[0].Name = "Asset SN";
            dataGridView1.Columns[1].Name = "Asset Name";
            dataGridView1.Columns[2].Name = "Last Closed EM";
            dataGridView1.Columns[3].Name = "Number of EMs";
            dataGridView1.Columns[4].Name = "Asset ID";
            dataGridView1.Columns[4].Visible = false;
            dataGridView1.Rows.Clear();
        }

        private void btnSendEMM_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please select an asset to create a request!", "No asset selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (dataGridView1.CurrentRow.DefaultCellStyle.BackColor == Color.Red)
            {
                MessageBox.Show("Current selected EM has an ongoing request! Unable to send another request",
                    "Ongoing request",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                var getAssetID = Convert.ToInt64(dataGridView1.CurrentRow.Cells[4].Value);
                (new EMR(getAssetID)).ShowDialog();
                EMMnon_Load(null, null);
            }
        }
    }
}
