using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using SimpleWifi;
using SimpleWifi.Win32;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using System.Linq;
using WifiManager.View;
using GalaSoft.MvvmLight.Messaging;

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

        ObservableCollection<MyAccessPoint> _AccessPointList = new ObservableCollection<MyAccessPoint>();
        public ObservableCollection<MyAccessPoint> AccessPointList
        {
            get
            {
                return _AccessPointList;
            }
            set
            {
                _AccessPointList = value;
                RaisePropertyChanged(()=>AccessPointList);
            }
        }

        bool IsRefresh = false;
        bool IsSettingOK = false;
        MyAccessPoint _SelectedAccessPoint;
        public MyAccessPoint SelectedAccessPoint
        {
            get
            {
                return _SelectedAccessPoint;
            }
            set
            {
                _SelectedAccessPoint = value;
                RaisePropertyChanged(() => SelectedAccessPoint);
            }
        }

        public ICommand ListAllCommand { get; set; }
        public ICommand SelectionChangedCommand { get; set; }

        public bool IsSupportUsername
        {
            get
            {
                if (SelectedAccessPoint == null)
                {
                    return false;
                }
                else
                {
                    AuthRequest authRequest = new AuthRequest(SelectedAccessPoint.AccessPoint);
                    if (authRequest.IsPasswordRequired&&authRequest.IsUsernameRequired)
                    {
                        return true;
                    }
                    return false;
                }
            }
        }
        public bool IsSupportDomain
        {
            get
            {
                if (SelectedAccessPoint == null)
                {
                    return false;
                }
                else
                {
                    AuthRequest authRequest = new AuthRequest(SelectedAccessPoint.AccessPoint);
                    if (authRequest.IsPasswordRequired && authRequest.IsDomainSupported)
                    {
                        return true;
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ListAllCommand = new RelayCommand(ListAll);
            SelectionChangedCommand = new RelayCommand(ConnectOrDis, !IsRefresh);
            ListAll();
            Messenger.Default.Register<string>(this,ViewMessage.OKMsg,(msg)=> 
            {
                IsSettingOK = true;
            });
        }

        void ListAll()
        {
            IsRefresh = true;
            AccessPointList = new ObservableCollection<MyAccessPoint>(
                wifi.GetAccessPoints().OrderByDescending(ap => ap.SignalStrength).Select(
                    t=>new MyAccessPoint() { AccessPoint=t}));
            IsRefresh = false;
        }

        void ConnectOrDis()
        {
            if (SelectedAccessPoint != null)
            {
                if (SelectedAccessPoint.AccessPoint.IsConnected)
                {
                    wifi.Disconnect();
                }
                else
                {
                    Connect(SelectedAccessPoint);
                }
                ListAll();
            }
        }

        void Connect(MyAccessPoint point)
        {
            if (point.AccessPoint.IsConnected)
            {
                return;
            }
            AuthRequest authRequest = new AuthRequest(point.AccessPoint);
            bool overwrite = true;
            if (authRequest.IsPasswordRequired)
            {
                if (point.AccessPoint.HasProfile)
                {
                    overwrite = false;
                }
            }
            if (overwrite)
            {
                WifiSettingView view = new WifiSettingView();
                view.ShowDialog();
                if (IsSettingOK)
                {
                    var viewmodel = (view.DataContext) as WifiSettingViewModel;
                    authRequest.Domain = viewmodel.Domain;
                    authRequest.Password = viewmodel.Password;
                    authRequest.Username = viewmodel.UserName;
                }
            }
            point.AccessPoint.ConnectAsync(authRequest, overwrite);
            IsSettingOK = false;
            ListAll();
        }
    }
}