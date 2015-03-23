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
    public partial class frmTagEdit : Form
    {
        public frmTagEdit()
        {
            InitializeComponent();
            Load += frmTagEdit_Load;
        }

        void frmTagEdit_Load(object sender, EventArgs e)
        {
            updateTagList();
            updateCategoryList();
            updateAliasList();
            grpAliases.Enabled = false;
        }

        public void updateTagList()
        {
            lstTags.SuspendLayout();
            lstTags.Items.Clear();
            foreach( KeyValuePair<string,string> tag in Tags.tags)
            {
                lstTags.Items.Add(tag.Key);
            }
            lstTags.ResumeLayout();
        }

        public void updateCategoryList()
        {
            lstCategories.SuspendLayout();
            lstCategories.Items.Clear();
            foreach (KeyValuePair<string, Color> tag in Tags.categories)
            {
                lstCategories.Items.Add(tag.Key);
            }
            lstCategories.ResumeLayout();
        }

        public void updateAliasList()
        {
            lstAliases.SuspendLayout();
            lstAliases.Items.Clear();
            foreach ( List<string> tag in Tags.aliasGroups )
            {
                lstAliases.Items.Add(tag[0]);
            }
            lstAliases.ResumeLayout();
        }

        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            if ( txtCategory.Text.Length > 0)
            {
                if ( !Tags.categories.ContainsKey(txtCategory.Text.Trim()))
                {
                    Tags.addCategory(txtCategory.Text.Trim(), Color.AliceBlue);
                    lblCategoriesStatus.Text = "Category \"" + txtCategory.Text.Trim() + "\" added";
                    txtCategory.Text = "";
                }
                else
                {
                    lblCategoriesStatus.Text = txtCategory.Text.Trim() + " already exists";
                }
            }
            else
            {
                lblCategoriesStatus.Text = "Enter a category name";
            }
        }
    }
}
