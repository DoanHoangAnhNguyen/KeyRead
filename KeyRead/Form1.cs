using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Threading;


namespace KeyRead
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Minimized;
        }

        private KeyMonitoring monitor;
        private void Form1_Load(object sender, EventArgs e)
        {
            Form1_Resize(sender, e);
            monitor = new KeyMonitoring();
            monitor.Run();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {            
            monitor.Stop();            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(buttonSwitchLanguage.Text == "GE")
            {
                buttonSwitchLanguage.Text = "EN";
                monitor.Stop();
                return;
            }
            buttonSwitchLanguage.Text = "GE";
            monitor.Resume();
            //monitor.Run();
        }


        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;            
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.ShowBalloonTip(500);
            }
        }

        private void notifyIcon1_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (buttonSwitchLanguage.Text == "GE")
            {
                buttonSwitchLanguage.Text = "EN";
                monitor.Stop();
                return;
            }
            buttonSwitchLanguage.Text = "GE";
            monitor.Resume();
        }
    }
}
