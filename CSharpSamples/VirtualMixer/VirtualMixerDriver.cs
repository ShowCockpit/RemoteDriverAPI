using System;
using ShowCockpit.Shared;
using ShowCockpit.Shared.Messages;
using ShowCockpit.Shared.Parameters;
using System.Diagnostics;

namespace VirtualMixer
{
    [ShowElementDriver("Virtual Mixer")]
    [ShowElementDriverAuthor("Your Name", "your@email.com")]
    [ShowElementDriverDescription("A window with a virtual mixer")]
    public class VirtualMixerDriver : ShowDriver
    {
        // Override the ShowDriver.ControlEvent event to generate events in this class
        public override event MsgControlEventHandler ControlEvent;

        // Event to pass information from this driver to the VirtualMixer GUI class
        internal event FaderChangedValueHandler FaderChangedValue;
        internal delegate void FaderChangedValueHandler(object sender, FaderChangedValueEventArgs e);

        /// <summary>
        /// Constructor
        /// </summary>
        public VirtualMixerDriver() : base()
        {
            // Populate list of fader controls
            for (uint i = 1; i <= 8; i++)
                RegisterControl("Fader " + i, ControlType.Fader);

            // Populate list of button controls
            for (uint i = 1; i <= 8; i++)
                RegisterControl("Button " + i, ControlType.Button);
        }

        /// <summary>
        /// Fires a ControlEvent for a fader
        /// </summary>
        /// <param name="faderIndex">The index of the fader</param>
        /// <param name="valNorm">Value of the fader, between 0 and 1</param>
        public void FaderAction(int faderIndex, float valNorm)
        {
            // Generate and invoke the ControlEvent
            ControlEvent?.Invoke(this, new MsgControlEvent()
            {
                ControlID = "Fader " + (faderIndex + 1),
                ElementID = this.DriverName,
                Type = ControlEventType.FaderMoved,
                value = valNorm
            });

            // Generate an information log
            Log(LogLevel.Information, "Moved " + "Fader " + (faderIndex + 1));
        }

        /// <summary>
        /// Fires a ControlEvent for a button
        /// </summary>
        /// <param name="buttonIndex">The index of the button</param>
        /// <param name="press">true for button press, false for button release</param>
        public void ButtonAction(int buttonIndex, bool press)
        {
            // Generate and invoke the ControlEvent
            ControlEvent?.Invoke(this, new MsgControlEvent()
            {
                ControlID = "Fader " + (buttonIndex + 1),
                ElementID = this.DriverName,
                Type = press ? ControlEventType.ButtonPressed : ControlEventType.ButtonReleased,
                value = press ? 1.0 : 0.0
            });

            // Generate an information log
            Log(LogLevel.Information, (press ? "Pressed" : "Released") + " " + "Button " + (buttonIndex + 1));
        }

        /// <summary>
        /// This function is called whenever a feedback is received from ShowCockpit for a control of this driver
        /// </summary>
        /// <param name="toControlID">The name of the control to receive feedback</param>
        /// <param name="feedback">The feedback data received</param>
        public override void HandleFeedback(string toControlID, MsgFunctionFeedback feedback)
        {
            base.HandleFeedback(toControlID, feedback);

            try
            {
                // If the name of the control starts with fader
                if (toControlID.StartsWith("Fader "))
                {
                    // Get fader index
                    int faderIndex = int.Parse(toControlID.Replace("Fader ", "")) - 1;

                    // Generate event to be catched by the VirtualMixer GUI
                    FaderChangedValue?.Invoke(this, new FaderChangedValueEventArgs()
                    {
                        FaderIndex = faderIndex,
                        Value = feedback.ValueDouble
                    });
                }
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, "Error handling feedback: " + ex.Message);
            }
        }

        /// <summary>
        /// Event args for the FaderChangedValue event
        /// </summary>
        public class FaderChangedValueEventArgs : EventArgs
        {
            /// <summary>
            /// Index of the fader
            /// </summary>
            public int FaderIndex { get; set; }

            /// <summary>
            /// New fader value (between 0 and 1)
            /// </summary>
            public double Value { get; set; }
        }
    }
}

