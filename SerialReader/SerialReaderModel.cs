using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SerialReader
{
    public class SerialReaderModel
    {
        #region Fields

        public float[] gyro { get; private set; }
        public float[] accel { get; private set; }
        public float timestamp { get; private set; }

        public ComPort comPort { get; private set; }
        public string btName { get; private set; }

        #endregion

        #region Constructor

        public SerialReaderModel()
        {
            gyro = new float[3];
            accel = new float[3];
            timestamp = 0;

            btName = "{00001101-0000-1000-8000-00805F9B34FB}";

            comPort = new ComPort();

            Task.Run(() =>
            {
                while(true)
                {
                    DataParser();
                    Thread.Sleep(20);
                }
            });
        }

        void DataParser ()
        {
            byte[] buffer;

            if (comPort.connectionState && comPort.circularBuffer.Length > 36)
            {
                buffer = comPort.circularBuffer.Dequeue(comPort.circularBuffer.Length);

                for (int i=1; i<buffer.Length; i++)
                {
                    if (buffer[i] == 0x55 && buffer[i-1] == 0x55 && i >= 33)
                    {
                        gyro[0] = BitConverter.ToSingle(buffer, i - 29); 
                        gyro[1] = BitConverter.ToSingle(buffer, i - 25);
                        gyro[2] = BitConverter.ToSingle(buffer, i - 21);

                        accel[0] = BitConverter.ToSingle(buffer, i - 17);
                        accel[1] = BitConverter.ToSingle(buffer, i - 13);
                        accel[2] = BitConverter.ToSingle(buffer, i - 9);

                        timestamp = BitConverter.ToSingle(buffer, i - 5);
                    }
                }
            }
        }

        #endregion

    }
}
