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

        }
    }
}
