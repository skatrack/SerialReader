using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CircularBuffer;
using Microsoft.Win32;

namespace SerialReader
{
    public class ComPort
    {
        #region Fields

        public bool connectionState { get; private set; }
        public CircularBuffer<byte> circularBuffer { get; private set; }

        private SerialPort serialPort;
        private List<string> portList;

        #endregion


        #region Constructor

        public ComPort()
        {
            serialPort = new SerialPort();
            portList = SerialPort.GetPortNames().ToList<string>();
            connectionState = false;

            circularBuffer = new CircularBuffer<byte>(10000000);
        }
        #endregion

        #region Serial port functions

        public List<string> GetComPortNames(string param)
        {
            String pattern = String.Format("^{0}", param);
            Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);
            List<string> comports = new List<string>();

            RegistryKey rk1 = Registry.LocalMachine;
            RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");

            foreach (String s3 in rk2.GetSubKeyNames())
            {
                RegistryKey rk3 = rk2.OpenSubKey(s3);
                foreach (String s in rk3.GetSubKeyNames())
                {
                    if (_rx.Match(s).Success)
                    {
                        RegistryKey rk4 = rk3.OpenSubKey(s);
                        foreach (String s2 in rk4.GetSubKeyNames())
                        {
                            RegistryKey rk5 = rk4.OpenSubKey(s2);
                            string location = (string)rk5.GetValue("LocationInformation");
                            RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                            string portName = (string)rk6.GetValue("PortName");
                            if (!String.IsNullOrEmpty(portName) && SerialPort.GetPortNames().Contains(portName))
                                comports.Add((string)rk6.GetValue("PortName"));
                        }
                    }
                }
            }
            return comports;
        }

        public bool SetConnectionState(string portName, bool state)
        {
            if (serialPort.IsOpen == false && state == true)
            {
                try
                {
                    serialPort.PortName = portName;
                    serialPort.Open();
                    serialPort.DataReceived += DataReceivedHandler;
                    connectionState = true;
                    Console.WriteLine("COM connected at port {0}", portName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: {0}", ex.Message);
                    Console.WriteLine("COM connection error");
                    return false;
                }
            }
            else if (serialPort.IsOpen == true && state == false)
            {
                try
                {
                    serialPort.Close();
                    serialPort.DataReceived -= DataReceivedHandler;
                    connectionState = false;
                    Console.WriteLine("COM disconnected");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: {0}", ex.Message);
                    Console.WriteLine("COM disconnection error");
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public string GetSelectedPort()
        {
            if (serialPort.IsOpen)
                return serialPort.PortName;
            else
                return "";
        }

        void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            int cnt = sp.BytesToRead;
            byte[] tmp_buf = new byte[cnt];
            byte[] tmp2_buf = new byte[cnt];

            if (sp.IsOpen)
            {
                sp.Read(tmp_buf, 0, cnt);
                circularBuffer.Enqueue(tmp_buf, cnt);
            }
            else
            {
                sp.DiscardInBuffer();
            }
        }
        #endregion
    }
}
