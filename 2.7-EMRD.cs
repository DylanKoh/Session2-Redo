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
    public partial class EMRD : Form
    {
        long _emID = 0;
        public EMRD(long emID)
        {
            InitializeComponent();
            _emID = emID;
        }

        private void EMRD_Load(object sender, EventArgs e)
        {
            dtpCompleted.Text = "";
            dtpStart.Text = "";
            using (var context = new Session2Entities())
            {
                var getParts = (from x in context.Parts
                                select x.Name);
                foreach (var item in getParts)
                {
                    cbParts.Items.Add(item);
                }

                var getAssetInfo = (from x in context.EmergencyMaintenances
                                    where x.ID == _emID
                                    select x).First();
                lblSN.Text = getAssetInfo.Asset.AssetSN;
                lblDepartment.Text = getAssetInfo.Asset.DepartmentLocation.Department.Name;
                lblName.Text = getAssetInfo.Asset.AssetName;
                LoadGrid();
            }
        }
        private void LoadGrid()
        {
            dataGridView1.ColumnCount = 3;
            dataGridView1.Columns[0].Name = "Part Name";
            dataGridView1.Columns[1].Name = "Amount";
            dataGridView1.Columns[2].Name = "Part ID";
            dataGridView1.Columns[2].Visible = false;
            DataGridViewLinkColumn linkCell = new DataGridViewLinkColumn()
            {
                LinkColor = Color.Blue,
                Name = "Action",
                Text = "Remove",
                UseColumnTextForLinkValue = true
            };
            dataGridView1.Columns.Add(linkCell);
            dataGridView1.Rows.Clear();
            using (var context = new Session2Entities())
            {
                var getPartsChanged = (from x in context.ChangedParts
                                       where x.EmergencyMaintenanceID == _emID
                                       select x);
                foreach (var item in getPartsChanged)
                {
                    var rows = new List<string>()
                    {
                        item.Part.Name, item.Amount.ToString(), item.PartID.ToString()
                    };
                    dataGridView1.Rows.Add(rows.ToArray());
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var context = new Session2Entities())
            {
                var partID = (from x in context.Parts
                              where x.Name == cbParts.SelectedItem.ToString()
                              select x.ID).FirstOrDefault();
                var checkPart = (from x in context.ChangedParts
                                 where x.Part.Name == cbParts.SelectedItem.ToString()
                                 orderby x.EmergencyMaintenance.EMEndDate descending
                                 select x.EmergencyMaintenance.EMEndDate).FirstOrDefault();
                TimeSpan check = TimeSpan.Zero;
                if (checkPart != null)
                {
                    check = DateTime.Now - Convert.ToDateTime(checkPart);
                }

                foreach (DataGridViewRow item in dataGridView1.Rows)
                {
                    if (item.Cells[0].Value.ToString() == cbParts.SelectedItem.ToString())
                    {
                        MessageBox.Show("Cannot add duplicate parts! Please remove and add again!", "Duplicate detected",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                if (cbParts.SelectedItem == null)
                {
                    MessageBox.Show("Please select a part!", "No part detected",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                if (nUDAmount.Value == 0)
                {
                    MessageBox.Show("Please choose a value more than 0!", "Invalid amount detected",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (check.TotalDays < context.Parts.Where(x => x.Name == cbParts.SelectedItem.ToString()).Select(x => x.EffectiveLife).FirstOrDefault())
                {
                    var dl = MessageBox.Show("Are you sure you want to add? Effective life is not over", "Add to list",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dl == DialogResult.Yes)
                    {
                        var row = new List<string>()
                        {
                            cbParts.SelectedItem.ToString(), nUDAmount.Value.ToString(), partID.ToString()
                        };
                        dataGridView1.Rows.Add(row.ToArray());

                    }
                }
                else
                {
                    var row = new List<string>()
                    {
                        cbParts.SelectedItem.ToString(), nUDAmount.Value.ToString(), partID.ToString()
                    };
                    dataGridView1.Rows.Add(row.ToArray());
                }
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            using (var context = new Session2Entities())
            {
                if (rtxtTechnicianNote.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter technician notes before submitting", "Empty note",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (dtpCompleted.Value < dtpStart.Value)
                {
                    MessageBox.Show("Completed date cannot be earlier than start date", "Error in date entry",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    foreach (var item in context.ChangedParts.Where(x => x.EmergencyMaintenanceID == _emID).Select(x => x))
                    {
                        context.ChangedParts.Remove(item);
                    }
                    var update = (from x in context.EmergencyMaintenances
                                  where x.ID == _emID
                                  select x).First();
                    update.EMEndDate = dtpCompleted.Value;
                    update.EMStartDate = dtpStart.Value;
                    update.EMTechnicianNote = rtxtTechnicianNote.Text;
                    foreach (DataGridViewRow item in dataGridView1.Rows)
                    {
                        context.ChangedParts.Add(new ChangedPart()
                        {
                            EmergencyMaintenanceID = _emID,
                            PartID = Convert.ToInt64(item.Cells[2].Value),
                            Amount = Convert.ToDecimal(item.Cells[1].Value)
                        });
                    }
                    context.SaveChanges();
                    this.Close();
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                dataGridView1.Rows.RemoveAt(e.RowIndex);
            }
        }
    }
}
