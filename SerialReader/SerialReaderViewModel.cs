using System;
using System.Collections.Generic;
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
    class SerialReaderViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private SerialReaderModel serialReaderModel;
        DispatcherTimer timer;

        private ICommand ConnectCommand;
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

        public string GyroXVM
        {
            get
            {
                return serialReaderModel.gyro[0].ToString();
            }
        }

        public string GyroYVM
        {
            get
            {
                return serialReaderModel.gyro[1].ToString();
            }
        }

        public string GyroZVM
        {
            get
            {
                return serialReaderModel.gyro[2].ToString();
            }
        }

        public string AccelXVM
        {
            get
            {
                return serialReaderModel.accel[0].ToString();
            }
        }

        public string AccelYVM
        {
            get
            {
                return serialReaderModel.accel[1].ToString();
            }
        }

        public string AccelZVM
        {
            get
            {
                return serialReaderModel.accel[2].ToString();
            }
        }

        public string TimestampVM
        {
            get
            {
                return serialReaderModel.timestamp.ToString();
            }
        }
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

        #endregion
    }
}
