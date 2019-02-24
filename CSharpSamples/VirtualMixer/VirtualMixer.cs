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

namespace VirtualMixer
{
    public partial class VirtualMixer : Form
    {
        /// <summary>
        /// The custom ShowDriver object
        /// </summary>
        VirtualMixerDriver drv = null;

        /// <summary>
        /// API Connector
        /// </summary>
        SCAPIConnector api = null;

        /// <summary>
        /// Used as mutex for simultaneous feedback on trackbars
        /// </summary>
        object lockControls = new object();

        /// <summary>
        /// Dictionary that translates a TrackBar to the respective index
        /// </summary>
        Dictionary<TrackBar, int> trackbar2idx = new Dictionary<TrackBar, int>();

        /// <summary>
        /// Dictionary that translates an index to a TrackBar
        /// </summary>
        Dictionary<int, TrackBar> idx2trackbar = new Dictionary<int, TrackBar>();

        /// <summary>
        /// Dictionary that translates a Button to the respective index
        /// </summary>
        Dictionary<Button, int> button2idx = new Dictionary<Button, int>();

        /// <summary>
        /// Dictionary that translates an index to a Button
        /// </summary>
        Dictionary<int, Button> idx2button = new Dictionary<int, Button>();

        /// <summary>
        /// Constructor
        /// </summary>
        public VirtualMixer()
        {
            InitializeComponent();
            AddControls();
        }

        /// <summary>
        /// Form initialized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VirtualMixer_Load(object sender, EventArgs e)
        {
            // Create a new ShowDriver object
            drv = new VirtualMixerDriver();

            // Listen to the FaderChangedValue event, that is fired when feedback is received from ShowCockpit
            drv.FaderChangedValue += Drv_FaderChangedValue;

            // Create a SC Remote Driver API Connector
            api = new SCAPIConnector(drv);

            // Connect to ShowCockpit
            api.Connect();
        }

        /// <summary>
        /// Generates the controls (trackbars and buttons) and adds them to the UI
        /// </summary>
        private void AddControls()
        {
            for (int i = 0; i < 8; i++)
            {
                // Trackbar

                TrackBar tb = new TrackBar();
                tb.Minimum = 0;
                tb.Maximum = 255;
                tb.Name = "Fader " + (i + 1);
                tb.Orientation = System.Windows.Forms.Orientation.Vertical;
                tb.TickStyle = System.Windows.Forms.TickStyle.Both;
                tb.Scroll += Tb_Scroll;
                tb.Dock = DockStyle.Fill;

                idx2trackbar[i] = tb;
                trackbar2idx[tb] = i;

                this.tableLayoutPanel1.Controls.Add(tb, i, 0);

                // Button
                Button btn = new Button();
                btn.Name = "Button " + (i + 1);
                btn.Click += Btn_Click;
                tb.Dock = DockStyle.Fill;

                idx2button[i] = btn;
                button2idx[btn] = i;

                this.tableLayoutPanel1.Controls.Add(btn, i, 1);
            }
        }

        /// <summary>
        /// The TrackBar scroll handler function
        /// </summary>
        /// <param name="sender">the TrackBar that was clicked</param>
        /// <param name="e"></param>
        private void Tb_Scroll(object sender, EventArgs e)
        {
            // Get TrackBar
            TrackBar tb = sender as TrackBar;

            // Generate the Fader Move event on the driver
            drv?.FaderAction(trackbar2idx[tb], tb.Value * 1.0f / 255.0f);
        }

        /// <summary>
        /// The Button click handler function
        /// </summary>
        /// <param name="sender">the Button that was clicked</param>
        /// <param name="e"></param>
        private void Btn_Click(object sender, EventArgs e)
        {
            // Get button
            Button btn = sender as Button;

            // Generate Button Press event on the driver
            drv?.ButtonAction(button2idx[btn], true);

            // Wait 10 ms
            System.Threading.Thread.Sleep(10);

            // Generate Button Release event on the driver
            drv?.ButtonAction(button2idx[btn], false); // release
        }

        private void Drv_FaderChangedValue(object sender, VirtualMixerDriver.FaderChangedValueEventArgs e)
        {
            // Prevent simultaneous changes
            lock (lockControls)
            {
                // BeginInvoke is required because we are on a multi-thread architecture, 
                // the event fire occurred in a different thread
                
                MethodInvoker m = () =>
                {
                    // This is the code we want to run
                    if (idx2trackbar.ContainsKey(e.FaderIndex))
                    {
                        TrackBar tb = idx2trackbar[e.FaderIndex];
                        tb.Value = (int)(e.Value * 255);
                    }
                };

                // This runs the code above in the right thread
                BeginInvoke(m);
            }
        }
    }
}
