using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;



namespace SerialReader
{

    using OxyPlot;
    using OxyPlot.Series;

    class SerialReaderViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private SerialReaderModel serialReaderModel;
        DispatcherTimer timer;

        private ICommand ConnectCommand;
        private ICommand ClearCommand;
        private string selectedPort;

        #region Commands

        public ICommand ConnectButton_Click
        {
            get
            {
                if (ConnectCommand == null)
                {
                    ConnectCommand = new UiCommand((obj) => this.ConnectRequest(obj));
                }
                return ConnectCommand;
            }

        }

        public ICommand ClearButton_Click
        {
            get
            {
                if (ClearCommand == null)
                {
                    ClearCommand = new UiCommand((obj) => this.ClearRequest(obj));
                }
                return ClearCommand;
            }

        }

        #endregion

        #region VM Fields

        public string ActivityLogVM { get; private set; }

        public string ConnectionStateVM
        {
            get
            {   
                return serialReaderModel.comPort.connectionState ? "Disconnect" : "Connect";
            }
        }

        public List<string> PortListVM
        {
            get
            {
                return serialReaderModel.comPort.GetComPortNames( serialReaderModel.btName);
            }
        }
        public string SelectedPortVM
        {
            set
            {
                if (value != null)
                {
                    selectedPort = value;
                }
            }
            get
            {
                if (PortListVM.Count > 0)
                {
                    selectedPort = PortListVM[0];
                    return PortListVM[0];
                }
                return null;
            }
        }

        public ObservableCollection<KeyValuePair<float,float>> GyroXVM
        {
            get
            {
                return serialReaderModel.gyroCollection[0];
            }
            set
            {
                serialReaderModel.gyroCollection[0] = value;
            }
        }

        public ObservableCollection<KeyValuePair<float, float>> GyroYVM
        {
            get
            {
                return serialReaderModel.gyroCollection[1];
            }
            set
            {
                serialReaderModel.gyroCollection[1] = value;
            }
        }

        public ObservableCollection<KeyValuePair<float, float>> GyroZVM
        {
            get
            {
                return serialReaderModel.gyroCollection[2];
            }
            set
            {
                serialReaderModel.gyroCollection[2] = value;
            }
        }

        public ObservableCollection<KeyValuePair<float, float>> AccelXVM
        {
            get
            {
                return serialReaderModel.accelCollection[0];
            }
            set
            {
                serialReaderModel.accelCollection[0] = value;
            }
        }

        public ObservableCollection<KeyValuePair<float, float>> AccelYVM
        {
            get
            {
                return serialReaderModel.accelCollection[1];
            }
            set
            {
                serialReaderModel.accelCollection[1] = value;
            }
        }

        public ObservableCollection<KeyValuePair<float, float>> AccelZVM
        {
            get
            {
                return serialReaderModel.accelCollection[2];
            }
            set
            {
                serialReaderModel.accelCollection[2] = value;
            }
        }

        //public float TimestampVM
        //{
        //    get
        //    {
        //        return serialReaderModel.timestamp;
        //    }
        //}
        #endregion

        #region Contructor

        public SerialReaderViewModel()
        {
            serialReaderModel = new SerialReaderModel();

            WriteLog("Program started", true);

            //Configure and start timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += TimerTickHandler;
            timer.Start();
        }

        #endregion

        #region Local functions

        // Add a line to the activity log text box
        private void WriteLog(string message, bool clear)
        {
            // Replace content
            if (clear)
            {
                ActivityLogVM = string.Format("{0}: {1}", DateTime.Now.ToString("HH:mm:ss"), message);
            }
            // Add new line
            else
            {
                ActivityLogVM += Environment.NewLine + string.Format("{0}: {1}", DateTime.Now.ToString("HH:mm:ss"), message);
            }
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("ActivityLogVM"));
            }
        }

        // Update window with received data
        private void TimerTickHandler(object sender, EventArgs e)
        {
            // update View
            PropertyChanged(this, new PropertyChangedEventArgs("ConnectionStateVM"));
            PropertyChanged(this, new PropertyChangedEventArgs("PortListVM"));
            PropertyChanged(this, new PropertyChangedEventArgs("SelectedPortVM"));
            PropertyChanged(this, new PropertyChangedEventArgs("GyroXVM"));
            PropertyChanged(this, new PropertyChangedEventArgs("GyroYVM"));
            PropertyChanged(this, new PropertyChangedEventArgs("GyroZVM"));
            PropertyChanged(this, new PropertyChangedEventArgs("AccelXVM"));
            PropertyChanged(this, new PropertyChangedEventArgs("AccelYVM"));
            PropertyChanged(this, new PropertyChangedEventArgs("AccelZVM"));
            PropertyChanged(this, new PropertyChangedEventArgs("TimestampVM"));

        }
        #endregion

        #region Model requests

        public void ConnectRequest(object parameter)
        {
            serialReaderModel.comPort.SetConnectionState(selectedPort, !serialReaderModel.comPort.connectionState);
            PropertyChanged(this, new PropertyChangedEventArgs("ConnectionStateVM"));
        }

        public void ClearRequest(object parameter)
        {
            for (int i=0; i<serialReaderModel.gyroCollection.Length;i++)
                serialReaderModel.gyroCollection[i].Clear();

            for (int i = 0; i < serialReaderModel.accelCollection.Length; i++)
                serialReaderModel.accelCollection[i].Clear(); 
        }

        #endregion
    }
}
