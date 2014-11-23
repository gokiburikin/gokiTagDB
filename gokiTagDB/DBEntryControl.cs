using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GokiLibrary;

namespace gokiTagDB
{
    public partial class DBEntryControl : UserControl
    {
        private DBEntry entry;

        public DBEntry Entry
        {
            get { return entry; }
            set { entry = value; }
        }
        private bool hover = false;

        public bool Hover
        {
            get { return hover; }
            set { hover = value; }
        }
        private bool selected = false;

        public bool Selected
        {
            get { return selected; }
            set {
                selected = value;
                Invalidate();
            }
        }
        public DBEntryControl()
        {
            InitializeComponent();
            picImage.MouseEnter += mouseEnter;
            picImage.MouseLeave += mouseLeave;
            picImage.Click += picImage_Click;
            picImage.DoubleClick += picImage_DoubleClick;
            picImage.MouseWheel += picImage_MouseWheel;
            Paint += DBEntryControl_Paint;
            Click += click;
            DoubleClick += doubleClick;
        }

        void picImage_MouseWheel(object sender, MouseEventArgs e)
        {
            OnMouseWheel(e);
        }



        void doubleClick(object sender, EventArgs e)
        {
            Invalidate();
            System.Diagnostics.Process.Start(Entry.Location);
        }

        void picImage_DoubleClick(object sender, EventArgs e)
        {
            OnDoubleClick(e);
        }

        void click(object sender, EventArgs e)
        {
            Invalidate();
        }

        void picImage_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }


        void DBEntryControl_Paint(object sender, PaintEventArgs e)
        {
            txtLabel.BackColor = this.BackColor;
            if ( selected )
            {
                BackColor = Color.LightSalmon;
            }
            else
            {
                BackColor = Color.White;
            }
            if (hover)
            {
                using (Pen pen = new Pen(Color.Black))
                {
                    e.Graphics.DrawRectangle(pen, GokiUtility.getRectangleContracted(e.ClipRectangle, 0, 0, 1, 1));
                }
            }
        }

        void mouseLeave(object sender, EventArgs e)
        {
            hover = false;
            Invalidate();
        }

        void mouseEnter(object sender, EventArgs e)
        {
            hover = true;
            Invalidate();
        }
        public void setEntry( DBEntry entry )
        {
            Entry = entry;
            if (entry != null)
            {
                txtLabel.Text = entry.Name;
                if ( entry.Thumbnail != null )
                {
                    picImage.Image = entry.Thumbnail;
                }
            }
            else
            {
                txtLabel.Text = "<missing>";
            }
        }
    }
}
