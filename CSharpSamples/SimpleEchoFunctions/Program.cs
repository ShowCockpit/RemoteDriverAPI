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

            // Connect to ShowCockpit
            api.Connect();

            // Keep the process alive
            while(true)
                Console.ReadLine();
        }
    }
}
