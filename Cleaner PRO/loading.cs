using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Cleaner_PRO
{
    public partial class loading : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]

        private static extern IntPtr CreateRoundRectRgn
            (
            int nLeftRecr,
            int nTopRecr,
            int RightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
            );
        public loading()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
            Progressbar1.Value = 0;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void loading_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Progressbar1.Value += 2;
            Progressbar1.Text = Progressbar1.Value.ToString() + "%";

            if (Progressbar1.Value == 100)
            {
                timer1.Enabled = false;
                Form1 se_form = new Form1();
                se_form.Show();
                this.Hide();
            }
        }

        private void Progressbar1_Click(object sender, EventArgs e)
        {

        }
    }
}
