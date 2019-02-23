using System;
using ShowCockpit.Shared;
using ShowCockpit.Shared.Messages;
using ShowCockpit.Shared.Parameters;
using System.Diagnostics;

namespace WindowsFormsApp1
{
    [ShowElementDriver("Virtual Faders")]
    [ShowElementDriverAuthor("Your Name", "your@email.com")]
    [ShowElementDriverDescription("A window with virtual faders")]
    public class VirtualMixerDriver : ShowDriver
    {
        public override event MsgControlEventHandler ControlEvent;

        public event FaderChangedValueHandler FaderChangedValue;
        public delegate void FaderChangedValueHandler(object sender, FaderChangedValueEventArgs e);

        public VirtualMixerDriver() : base()
        {
            for (uint i = 1; i <= 8; i++)
                RegisterControl("Fader " + i, ControlType.Fader);
        }

        public void GenerateFaderEvent(int faderIndex, float valNorm)
        {
            ControlEvent?.Invoke(this, new MsgControlEvent()
            {
                ControlID = "Fader " + (faderIndex + 1),
                ElementID = this.DriverName,
                Type = ControlEventType.FaderMoved,
                value = valNorm
            });
            Log(LogLevel.Information, "Moved " + "Fader " + (faderIndex + 1));
        }

        public override void HandleFeedback(string toControlID, MsgFunctionFeedback feedback)
        {
            base.HandleFeedback(toControlID, feedback);

            try
            {
                if (toControlID.StartsWith("Fader "))
                {
                    int faderIndex = int.Parse(toControlID.Replace("Fader ", "")) - 1;
                    FaderChangedValue?.Invoke(this, new FaderChangedValueEventArgs()
                    {
                        FaderIndex = faderIndex,
                        Value = feedback.ValueDouble
                    });
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        public class FaderChangedValueEventArgs : EventArgs
        {
            public int FaderIndex { get; set; }
            public double Value { get; set; }
        }
    }
}

