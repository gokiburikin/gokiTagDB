using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gokiTagDB
{
    public partial class frmEntriesPerPage : Form
    {
        public frmEntriesPerPage()
        {
            InitializeComponent();
            Load += EntriesPerPageForm_Load;
            btnOk.Click += btnOk_Click;
            btnCancel.Click += btnCancel_Click;
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        void btnOk_Click(object sender, EventArgs e)
        {
            frmMainForm.entriesPerPage = (int)numEntries.Value;
            Close();
        }

        void EntriesPerPageForm_Load(object sender, EventArgs e)
        {
            numEntries.Value = (decimal)frmMainForm.entriesPerPage;
        }
    }
}
