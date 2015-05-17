using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace gokiTagDB
{
    public partial class frmFileFilter : Form
    {
        public frmFileFilter()
        {
            InitializeComponent();
            txtFilter.Text = GokiTagDB.settings.FileFilter;
            btnOk.Click += btnOk_Click;
            btnCancel.Click += btnCancel_Click;
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            GokiTagDB.settings.FileFilter = txtFilter.Text;
            this.Close();
        }
    }
}
