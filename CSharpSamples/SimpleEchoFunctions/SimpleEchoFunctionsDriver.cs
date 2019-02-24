using System;
using ShowCockpit.Shared;
using ShowCockpit.Shared.Messages;
using ShowCockpit.Shared.Parameters;
using System.Diagnostics;

namespace SimpleEchoFunctions
{
    [ShowElementDriver("Simple Echo Functions")]
    [ShowElementDriverAuthor("Your Name", "your@email.com")]
    [ShowElementDriverDescription("Prints the events to a console")]
    public class SimpleEchoFunctionsDriver : ShowDriver
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SimpleEchoFunctionsDriver() : base()
        {
            // Populate list of functions
            RegisterFunction(new Function("Echo Fader", "Prints the fader value to the console", ControlType.Fader, new ParameterDescriptor()
            {
                // Function parameters
                {"Input Number", ParameterDefinition.CreateInt("The magic number that identifies this fader", 1, 1, 10) }
            }), FuncEchoFader);

            // Populate list of functions
            RegisterFunction(new Function("Echo Button", "Prints the button events to the console", ControlType.Button, new ParameterDescriptor()
            {
                // Function parameters
                {"Input Number", ParameterDefinition.CreateInt("The magic number that identifies this fader", 1, 1, 10) }
            }), FuncEchoButton);
        }

        /// <summary>
        /// Declare a function to perform the echo for a fader value
        /// </summary>
        /// <param name="obj"></param>
        private void FuncEchoFader(MsgFunctionTrigger obj)
        {
            // Encapsulate in a try-catch to catch any exception
            try
            {
                // Validate the control event type
                if (obj.ControlEvent.Type != ControlEventType.FaderMoved)
                    return;

                // Validate any parameters
                if (!obj.ParametersList.ContainsKey("Input Number"))
                    return;

                // Get the parameter value
                int inputNumber = obj.ParametersList["Input Number"].ToInt();

                // Print the fader value to the console
                Console.WriteLine("Fader " + inputNumber + " value: " + obj.ControlEvent.ValueDouble.ToString("F2"));
            }
            catch (Exception ex)
            {
                // When an exception is thrown, print log to the ShowCockpit console
                Log(LogLevel.Error, "Error executing \"" + obj.FunctionID + "\": " + ex.Message);
            }
        }

        /// <summary>
        /// Declare a function to perform the echo for a button event
        /// </summary>
        /// <param name="obj"></param>
        private void FuncEchoButton(MsgFunctionTrigger obj)
        {
            // Encapsulate in a try-catch to catch any exception
            try
            {
                // Validate the control event type
                if (obj.ControlEvent.Type != ControlEventType.ButtonPressed && obj.ControlEvent.Type != ControlEventType.ButtonReleased)
                    return;

                // Validate any parameters
                if (!obj.ParametersList.ContainsKey("Input Number"))
                    return;

                // Get the parameter value
                int inputNumber = obj.ParametersList["Input Number"].ToInt();

                // Print the button event to the console
                Console.WriteLine("Button " + inputNumber + " " + (obj.ControlEvent.Type == ControlEventType.ButtonPressed ? "pressed" : "released"));
            }
            catch (Exception ex)
            {
                // When an exception is thrown, print log to the ShowCockpit console
                Log(LogLevel.Error, "Error executing \"" + obj.FunctionID + "\": " + ex.Message);
            }
        }
    }
}

