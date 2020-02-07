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
        long _userID = 0;
        long _assetID = 0;
        public EMR(long assetID, long userID)
        {
            InitializeComponent();
            _userID = userID;
            _assetID = assetID;
        }

        private void EMR_Load(object sender, EventArgs e)
        {

        }
    }
}
