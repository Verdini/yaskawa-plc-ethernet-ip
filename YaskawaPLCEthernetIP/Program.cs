using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaskawaEthernetIPCommunication
{
    
    public class Program
    {
        /// <summary>
        /// Example program to test the Yaskawa Ethernet/IP communication.
        /// A MP2300 Series controller was used to validate the YaskawaPLC class.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Yaskawa PLC Ethernet/IP communication test...");

            YaskawaPLC mp2300 = new YaskawaPLC();

            // Connect with the controller using default IP and port
            if(!mp2300.Connect("192.168.1.1", 44818))
            {
                Console.WriteLine("Error: Couldn't connect to Yaskawa PLC controller!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Connected to Yaskawa PLC controller!");

            // Get the current state of the variables
            mp2300.GetState();

            // Print the state
            Console.WriteLine("Yaskawa MP2300 Controller Current State: ");
            Console.WriteLine("EIP_IN_VAR0: " + mp2300.EIP_IN_VAR0);
            Console.WriteLine("EIP_IN_VAR1: " + mp2300.EIP_IN_VAR1);
            Console.WriteLine("EIP_IN_VAR2: " + mp2300.EIP_IN_VAR2);
            Console.WriteLine("EIP_IN_VAR3: " + mp2300.EIP_IN_VAR3);
            Console.WriteLine("EIP_IN_VAR4: " + mp2300.EIP_IN_VAR4);
            Console.WriteLine("EIP_IN_VAR5: " + mp2300.EIP_IN_VAR5);
            Console.WriteLine("EIP_IN_VAR6: " + mp2300.EIP_IN_VAR6);
            Console.WriteLine("EIP_IN_VAR7: " + mp2300.EIP_IN_VAR7);
            Console.WriteLine("EIP_IN_VAR8: " + mp2300.EIP_IN_VAR8);
            Console.WriteLine("EIP_IN_VAR9: " + mp2300.EIP_IN_VAR9);

            // Set the output values
            mp2300.EIP_OUT_VAR0 = 33.3333;
            mp2300.EIP_OUT_VAR1 = Math.PI;
            mp2300.EIP_OUT_VAR2 = 16565;
            mp2300.EIP_OUT_VAR3 = 32000;
            mp2300.EIP_OUT_VAR4 = 128;
            mp2300.EIP_OUT_VAR5 = 56;
            mp2300.EIP_OUT_VAR6 = true;
            mp2300.EIP_OUT_VAR7 = false;
            mp2300.EIP_OUT_VAR8 = true;
            mp2300.EIP_OUT_VAR9 = true;

            // Send the updated values to the controller
            mp2300.SetState();

            // The updated values can be checked inside the debug mode of Motion Works
            Console.WriteLine("Yaskawa PLC controller update has been updated.");
            
            // Disconnect the controller
            mp2300.Disconnect();

            Console.WriteLine("Yaskawa PLC Ethernet/IP communication test has ended.");
            Console.ReadKey();
        }

      
    }
}
