using System;
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
        IniFile iniFile = new IniFile("config.ini");
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void loading_Load(object sender, EventArgs e)
        {

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            Progressbar1.Value += 5;
            Progressbar1.Text = Progressbar1.Value.ToString() + "%";

            if (Progressbar1.Value == 100)
            {
                timer1.Enabled = false;
                string s = iniFile.ReadString("theme", "config");
                if (s == "dark")
                {
                    Form1 se_form = new Form1();
                    se_form.Show();
                    this.Hide();
                }
                else
                {
                    White1 se_form = new White1();
                    se_form.Show();
                    this.Hide();
                }
            }
        }
        private void Progressbar1_Click(object sender, EventArgs e)
        {

        }
    }
}
