using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using SimpleWifi;
using SimpleWifi.Win32;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using System.Linq;

namespace WifiManager.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private object Lock = new object();
        private Wifi _wifi;
        private Wifi wifi
        {
            get
            {
                if (_wifi == null)
                {
                    _wifi = new Wifi();
                }
                return _wifi;
            }
        }

        public ObservableCollection<MyAccessPoint> AccessPointList { get; set; }

        public MyAccessPoint SelectedAccessPoint { get; set; }

        public ICommand ListAllCommand { get; set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ListAllCommand = new RelayCommand(ListAll);
            ListAll();
        }

        void ListAll()
        {
            AccessPointList = new ObservableCollection<MyAccessPoint>(
                wifi.GetAccessPoints().OrderByDescending(ap => ap.SignalStrength).Select(
                    t=>new MyAccessPoint() { AccessPoint=t}));
        }
    }
}