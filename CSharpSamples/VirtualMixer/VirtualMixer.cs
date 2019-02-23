using ShowCockpit.Shared;
using ShowCockpit.Shared.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class VirtualMixer : Form
    {
        SCAPIConnector api = null;
        VirtualMixerDriver drv = null;

        object lockTrackbars = new object();
        Dictionary<TrackBar, int> trackbar2idx = new Dictionary<TrackBar, int>();
        Dictionary<int, TrackBar> idx2trackbar = new Dictionary<int, TrackBar>();

        public VirtualMixer()
        {
            InitializeComponent();
            AddTrackbars();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            drv = new VirtualMixerDriver();
            drv.FaderChangedValue += Drv_FaderChangedValue;

            api = new SCAPIConnector(drv);
            api.Connect();
            MsgRegister.Reply reply = api.GetRegistrationReply();
        }

        private void AddTrackbars()
        {
            for(int i = 0; i < 8; i++)
            {
                TrackBar tb = new TrackBar();
                tb.Minimum = 0;
                tb.Maximum = 255;
                tb.Name = "Fader " + (i + 1);
                tb.Orientation = System.Windows.Forms.Orientation.Vertical;
                tb.TickStyle = System.Windows.Forms.TickStyle.Both;
                tb.Scroll += Tb_Scroll;
                tb.Dock = DockStyle.Fill;

                this.tableLayoutPanel1.Controls.Add(tb);

                idx2trackbar[i] = tb;
                trackbar2idx[tb] = i;
            }
        }

        private void Tb_Scroll(object sender, EventArgs e)
        {
            TrackBar tb = sender as TrackBar;
            drv?.GenerateFaderEvent(trackbar2idx[tb], tb.Value * 1.0f / 255.0f);
        }

        private void Drv_FaderChangedValue(object sender, VirtualMixerDriver.FaderChangedValueEventArgs e)
        {
            lock (lockTrackbars)
            {
                MethodInvoker m = () =>
                {
                    if(idx2trackbar.ContainsKey(e.FaderIndex))
                    {
                        TrackBar tb = idx2trackbar[e.FaderIndex];
                        tb.Value = (int)(e.Value * 255);
                    }
                };
                BeginInvoke(m);
            }
        }
    }
}
