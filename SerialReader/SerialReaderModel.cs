using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SerialReader
{
    public class SerialReaderModel
    {
        #region Fields

        private Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
        DispatcherTimer timer;
        private byte[] serialBuffer;

        public float[] gyro { get; private set; }
        public float[] accel { get; private set; }
        public float timestamp { get; private set; }

        public ObservableCollection<KeyValuePair<float, float>>[] gyroCollection { get; set; }
        public ObservableCollection<KeyValuePair<float, float>>[] accelCollection { get; set; }

        public ComPort comPort { get; private set; }
        public string btName { get; private set; }

        #endregion

        #region Constructor

        public SerialReaderModel()
        {
            gyro = new float[3];
            accel = new float[3];

            gyroCollection = new ObservableCollection<KeyValuePair<float, float>>[3];
            accelCollection = new ObservableCollection<KeyValuePair<float, float>>[3];

            for (int i=0; i<gyroCollection.Length; i++)
            {
                gyroCollection[i] = new ObservableCollection<KeyValuePair<float, float>>();
            }

            for (int i = 0; i < accelCollection.Length; i++)
            {
                accelCollection[i] = new ObservableCollection<KeyValuePair<float, float>>();
            }

            timestamp = 0;

            btName = "{00001101-0000-1000-8000-00805F9B34FB}";

            comPort = new ComPort();

            //Configure and start timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += DataParser;
            timer.Start();

            //Task.Run(() =>
            //{
            //    while(true)
            //    {
            //        DataParser(null, null);
            //    }
            //});
        }

        void DataParser(object sender, EventArgs e)
        {

            if (comPort.connectionState && comPort.circularBuffer.Length > 36)
            {
                serialBuffer = comPort.circularBuffer.Dequeue(comPort.circularBuffer.Length);

                for (int i=1; i< serialBuffer.Length; i++)
                {
                    if (serialBuffer[i] == 0x55 && serialBuffer[i-1] == 0x55 && i >= 33)
                    {
                        gyro[0] = BitConverter.ToSingle(serialBuffer, i - 29);
                        gyro[1] = BitConverter.ToSingle(serialBuffer, i - 25);
                        gyro[2] = BitConverter.ToSingle(serialBuffer, i - 21);

                        accel[0] = BitConverter.ToSingle(serialBuffer, i - 17);
                        accel[1] = BitConverter.ToSingle(serialBuffer, i - 13);
                        accel[2] = BitConverter.ToSingle(serialBuffer, i - 9);

                        timestamp = BitConverter.ToSingle(serialBuffer, i - 5);

                        //dispatcher.Invoke(() => gyroCollection[0].Add(new KeyValuePair<float, float>(timestamp, gyro[0])));
                        //dispatcher.Invoke(() => gyroCollection[1].Add(new KeyValuePair<float, float>(timestamp, gyro[1])));
                        //dispatcher.Invoke(() => gyroCollection[2].Add(new KeyValuePair<float, float>(timestamp, gyro[2])));

                        //dispatcher.Invoke(() => accelCollection[0].Add(new KeyValuePair<float, float>(timestamp, accel[0])));
                        //dispatcher.Invoke(() => accelCollection[1].Add(new KeyValuePair<float, float>(timestamp, accel[1])));
                        //dispatcher.Invoke(() => accelCollection[2].Add(new KeyValuePair<float, float>(timestamp, accel[2])));


                        gyroCollection[0].Add(new KeyValuePair<float, float>(timestamp, gyro[0]));
                        gyroCollection[1].Add(new KeyValuePair<float, float>(timestamp, gyro[1]));
                        gyroCollection[2].Add(new KeyValuePair<float, float>(timestamp, gyro[2]));

                        accelCollection[0].Add(new KeyValuePair<float, float>(timestamp, accel[0]));
                        accelCollection[1].Add(new KeyValuePair<float, float>(timestamp, accel[1]));
                        accelCollection[2].Add(new KeyValuePair<float, float>(timestamp, accel[2]));
                    }
                }
            }
        }

        #endregion

    }
}
