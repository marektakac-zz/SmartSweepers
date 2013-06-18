using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartSweepers2
{
    public partial class MainForm : Form
    {
        delegate void UpdatePictureBoxCallback();

        BackgroundWorker worker;
        Controller controller;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                controller = new Controller();

                worker = new BackgroundWorker();
                worker.DoWork += worker_DoWork;
                worker.RunWorkerAsync();
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (!controller.Update())
                {
                    break;
                }

                UpdatePictureBox();

                //Thread.Sleep(100);
            }
        }

        private void UpdatePictureBox()
        {
            if (pictureBox.InvokeRequired)
            {
                try
                {
                    this.Invoke(new UpdatePictureBoxCallback(UpdatePictureBox));
                }
                catch (Exception) { }
            }
            else
            {
                controller.Render(pictureBox.CreateGraphics());
            }
        }
    }
}
