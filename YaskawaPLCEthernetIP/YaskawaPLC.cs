using Sres.Net.EEIP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaskawaEthernetIPCommunication
{
    /// <summary>
    /// Cclass responsible for controlling the Yaskawa PLC communication.
    /// 
    /// This class depends on the EEIP.NET written by Rossmann Engineering. 
    /// You can install it via NuGet using the following command:
    /// PM> Install-Package EEIP
    /// It's also possible to get the source code from it's github repository:
    /// https://github.com/rossmann-engineering/EEIP.NET
    /// </summary>
    public class YaskawaPLC
    {
        /// <summary>
        /// This attribute is part of the EEIP library. It sends and receives the Ethernet/IP messages.
        /// </summary>
        private EEIPClient eeipClient = new EEIPClient();

        /// <summary>
        /// The output instance number defined in the Hardware Configurations Tool from Motion Works.
        /// </summary>
        private int outputInstance = 111;

        /// <summary>
        /// The input instance number defined in the Hardware Configurations Tool from Motion Works.
        /// </summary>
        private int inputInstance = 101;

        /// <summary>
        /// A buffer to write the output memory block before sending it.
        /// </summary>
        protected byte[] outputBytes = new byte[128];



        public YaskawaPLC()
        {
            eeipClient = new EEIPClient();
        }


        #region Interface

        // Example Input variables to read from the PLC
        #region Input Instance Variables
        public double EIP_IN_VAR0 { get; private set;  }
        public double EIP_IN_VAR1 { get; private set; }
        public int EIP_IN_VAR2 { get; private set; }
        public int EIP_IN_VAR3 { get; private set; }
        public short EIP_IN_VAR4 { get; private set; }
        public short EIP_IN_VAR5 { get; private set; }
        public bool EIP_IN_VAR6 { get; private set; }
        public bool EIP_IN_VAR7 { get; private set; }
        public bool EIP_IN_VAR8 { get; private set; }
        public bool EIP_IN_VAR9 { get; private set; }
        #endregion

        // Example Output variables to send to the PLC
        #region Output Instance Variables
        public double EIP_OUT_VAR0 { get; set; }
        public double EIP_OUT_VAR1 { get; set; }
        public int EIP_OUT_VAR2 { get; set; }
        public int EIP_OUT_VAR3 { get; set; }
        public short EIP_OUT_VAR4 { get; set; }
        public short EIP_OUT_VAR5 { get; set; }
        public bool EIP_OUT_VAR6 { get; set; }
        public bool EIP_OUT_VAR7 { get; set; }
        public bool EIP_OUT_VAR8 { get; set; }
        public bool EIP_OUT_VAR9 { get; set; }
        #endregion


        /// <summary>
        /// Connects to a Yaskawa PLC.
        /// </summary>
        /// <param name="IPAddress">Ethernet/IP address to connect</param>
        /// <param name="port">Ethernet/IP port to connect</param>
        /// <returns>True if the connection succeed</returns>
        public bool Connect(string IPAddress, ushort port)
        {
            try
            {
                eeipClient.RegisterSession(IPAddress,port);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Disconnects the PLC.
        /// </summary>
        public void Disconnect()
        {   
            eeipClient.UnRegisterSession();
        }


        /// <summary>
        /// Read the input instance and update the variables.
        /// </summary>
        /// <returns>True if succeed</returns>
        public bool GetState()
        {
            try
            {
                byte[] input = eeipClient.AssemblyObject.getInstance(inputInstance);

                EIP_IN_VAR0 = BitConverter.ToDouble(input, 0);
                EIP_IN_VAR1 = BitConverter.ToDouble(input, 8);
                EIP_IN_VAR2 = BitConverter.ToInt32(input, 16);
                EIP_IN_VAR3 = BitConverter.ToInt32(input, 20);
                EIP_IN_VAR4 = BitConverter.ToInt16(input, 24);
                EIP_IN_VAR5 = BitConverter.ToInt16(input, 26);

                BitArray bits = new BitArray(input);
                EIP_IN_VAR6 = bits[224];
                EIP_IN_VAR7 = bits[225];
                EIP_IN_VAR8 = bits[226];
                EIP_IN_VAR9 = bits[227];

                return true;
            }
            catch
            {
                return false;
            }

        }


        /// <summary>
        /// Updates the output instance of the PLC.
        /// </summary>
        /// <returns></returns>
        public bool SetState()
        {
            try
            {
                setDouble(outputBytes, 0, EIP_OUT_VAR0);
                setDouble(outputBytes, 8, EIP_OUT_VAR1);
                setInt(outputBytes, 16, EIP_OUT_VAR2);
                setInt(outputBytes, 20, EIP_OUT_VAR3);
                setShort(outputBytes, 24, EIP_OUT_VAR4);
                setShort(outputBytes, 26, EIP_OUT_VAR5);
                setBit(outputBytes, 224, EIP_OUT_VAR6);
                setBit(outputBytes, 225, EIP_OUT_VAR7);
                setBit(outputBytes, 226, EIP_OUT_VAR8);
                setBit(outputBytes, 227, EIP_OUT_VAR9);  
                 
                eeipClient.AssemblyObject.setInstance(outputInstance, outputBytes);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion


        #region Auxiliary
        /// <summary>
        /// Set a bit value in a byte array at a specific position.
        /// /// IEC 61131-3 represents a bit as a BOOL
        /// </summary>
        /// <param name="array">Memory block to write</param>
        /// <param name="index">Index position to set the bit</param>
        /// <param name="value">New value for the bit in the specified index position</param>
        private void setBit(byte[] array, int index, bool value)
        {
            int byteIndex = index / 8;
            int bitIndex = index % 8;
            byte mask = (byte)(1 << bitIndex);

            array[byteIndex] = (byte)(value ? (array[byteIndex] | mask) : (array[byteIndex] & ~mask));
        }

        /// <summary>
        /// Set a 16 bits integer in a byte array at a specific position.
        /// IEC 61131-3 represents a 16 bits integer as a INT.
        /// </summary>
        /// <param name="array">Memory block to write</param>
        /// <param name="index">Index position to set the short integer</param>
        /// <param name="value">New value for the short integer in the specified index position</param>
        private void setShort(byte[] array, int index, short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Buffer.BlockCopy(bytes, 0, array, index, 2);
        }

        /// <summary>
        /// Set a 32 bits integer in a byte array at a specific position.
        /// IEC 61131-3 represents a 32 bits integer as a DINT.
        /// </summary>
        /// <param name="array">Memory block to write</param>
        /// <param name="index">Index position to set the integer</param>
        /// <param name="value">New value for the integer in the specified index position</param>
        private void setInt(byte[] array, int index, int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Buffer.BlockCopy(bytes, 0, array, index, 4);
        }

        /// <summary>
        /// Set a 64 bits double in a byte array at a specific position.
        /// IEC 61131-3 represents a 64 bits double as a LREAL.
        /// </summary>
        /// <param name="array">Memory block to write</param>
        /// <param name="index">Index position to set the double</param>
        /// <param name="value">New value for the double in the specified index position</param>
        private void setDouble(byte[] array, int index, double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Buffer.BlockCopy(bytes, 0, array, index, 8);
        }

        /// <summary>
        /// Set a 32 bits integer array (int[] with 32*n bits) at a specific position.
        /// </summary>
        /// <param name="array">Memory block to write</param>
        /// <param name="index">Index position to set the double</param>
        /// <param name="value">New value for the double in the specified index position</param>
        private void setIntArray(byte[] array, int index, int[] value)
        {
            Buffer.BlockCopy(value, 0, array, index, 112);
        }
        #endregion

    }
}
