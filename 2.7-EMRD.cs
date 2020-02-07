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

        }
    }
}
