using ShowCockpit.Shared;
using ShowCockpit.Shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEchoFunctions
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create the driver object
            SimpleEchoFunctionsDriver drv = new SimpleEchoFunctionsDriver();

            // Create API connector object
            SCAPIConnector api = new SCAPIConnector(drv);

            // Events Handling
            api.Connected += Api_Connected;
            api.Disconnected += Api_Disconnected;

            // Connect to ShowCockpit
            api.Connect();

            // Keep the process alive
            while(true)
                Console.ReadLine();
        }

        private static void Api_Disconnected(object sender, EventArgs e)
        {
            Console.WriteLine("Disconnected from SC");
        }

        private static void Api_Connected(object sender, EventArgs e)
        {
            Console.WriteLine("Connected to SC");
        }
    }
}
