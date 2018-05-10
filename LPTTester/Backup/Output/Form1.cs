using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using LPTTester;

namespace Output
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void WaitThread(object tumbler)
        {
            try
            {
                while (true)
                {
                    Port.WaitForTumblerSwitch((int)tumbler);
                    Process();
                }
            }
            catch (ThreadAbortException)
            {
            }
        }

        private void Process()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ThreadStart(Process));
            }
            else
            {
                int dot = 0;
                if (int.TryParse(Dot.Text, out dot))
                {
                    if (dot > 0)
                    {
                        string tx = text.Text;
                        Morze m = new Morze(dot);
                        List<Command> cl = m.Encode(tx);
                        ExecuteCommands(cl);
                        MessageBox.Show("Передача завершена", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Некорректная длительность точки", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Некорректная длительность точки", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ExecuteCommands(List<Command> c)
        {
            Port p = Port.CreateLPT(Port.LPTRegister.control);
            p.ChangeDiodState(true, 0);
            for (int i = 0; i < c.Count;i++ )
            {
                p.ChangeDiodState(c[i].enable, c[i].latency);
            }
            p.ChangeDiodState(true, 0);
        }

        private Thread t = null;

        private void ChangeTumbler()
        {
            int pos = comboBox1.SelectedIndex;
            if (pos == -1)
            {
                pos = 4;
            }
            else
            {
                pos++;
            }
            if (t != null)
            {
                t.Abort();
            }
            t = new Thread(new ParameterizedThreadStart(WaitThread));
            t.IsBackground = true;
            t.Start(pos);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeTumbler();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ChangeTumbler();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Port.ShutdownWinIo();
        }
    }
}
