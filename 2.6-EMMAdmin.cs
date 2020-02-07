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
    public partial class EMMAdmin : Form
    {
        long _id = 0;
        public EMMAdmin(long id)
        {
            InitializeComponent();
            _id = id;
        }

        private void EMMAdmin_Load(object sender, EventArgs e)
        {
            LoadGrid();
        }
        private void LoadGrid()
        {
            dataGridView1.ColumnCount = 6;
            dataGridView1.Columns[0].Name = "Asset SN";
            dataGridView1.Columns[1].Name = "Asset Name";
            dataGridView1.Columns[2].Name = "Request Date";
            dataGridView1.Columns[3].Name = "Employee Full Name";
            dataGridView1.Columns[4].Name = "Department";
            dataGridView1.Columns[5].Name = "EMID";
            dataGridView1.Columns[5].Visible = false;
            dataGridView1.Rows.Clear();
            using (var context = new Session2Entities())
            {
                var getEMs = (from x in context.EmergencyMaintenances
                              where x.EMEndDate == null
                              orderby x.EMReportDate
                              orderby x.PriorityID descending
                              select x);
                foreach (var item in getEMs)
                {
                    var rows = new List<string>()
                    {
                        item.Asset.AssetSN, item.Asset.AssetName, item.EMReportDate.ToString("dd/MM/yyyy"),
                        item.Asset.Employee.FirstName + " " + item.Asset.Employee.LastName,
                        item.Asset.DepartmentLocation.Department.Name, item.ID.ToString()
                    };
                    dataGridView1.Rows.Add(rows.ToArray());
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please select an asset to manage!", "No asset selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                var emID = Convert.ToInt64(dataGridView1.CurrentRow.Cells[5].Value);
                (new EMRD(emID)).Show();
                LoadGrid();
            }
        }
    }
}

