using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SachoWifiManager.Helper;
using SachoWifiManager.Model;
using SimpleWifi;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SachoWifiManager.ViewModel
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

        public MainViewModel()
        {
            ListAllCommand = new RelayCommand(ListAll);
            OnSelectedItemChangedCommand = new RelayCommand(OnSelectedItemChangedAsync, () => !IsRunning && !IsConnecting);
            EnabledOrNotWifiCommand = new RelayCommand(EnabledOrNotWifi, () => !IsRunning && !IsConnecting);
            GetCurrentEnabledAdapter();
            IsEnabledWifi = CurrentWifiAdapter != null;
            CheckWifiIsEnabled(IsEnabledWifi);
        }

        #region Binding Property

        bool _IsEnabledWifi = false;
        public bool IsEnabledWifi
        {
            get
            {
                return _IsEnabledWifi;
            }
            set
            {
                Set(ref _IsEnabledWifi, value);
            }
        }

        /// <summary>
        /// Wifi�б�
        /// </summary>
        ObservableCollection<MyAccessPoint> _AccessPointList = new ObservableCollection<MyAccessPoint>();
        public ObservableCollection<MyAccessPoint> AccessPointList
        {
            get
            {
                return _AccessPointList;
            }
            set
            {
                Set(ref _AccessPointList, value);
            }
        }

        /// <summary>
        /// ѡ���Wifi
        /// </summary>
        MyAccessPoint _SelectedAccessPoint;
        public MyAccessPoint SelectedAccessPoint
        {
            get
            {
                return _SelectedAccessPoint;
            }
            set
            {
                Set(ref _SelectedAccessPoint, value);
            }
        }

        #endregion

        #region Private Property

        /// <summary>
        /// ��ǰ��������
        /// </summary>
        ManagementObject CurrentWifiAdapter = null;

        bool _IsRunning = false;
        /// <summary>
        /// �Ƿ���������ִ��
        /// </summary>
        bool IsRunning
        {
            get
            {
                return _IsRunning;
            }
            set
            {
                Set(ref _IsRunning, value);
            }
        }

        bool _IsConnecting = false;
        /// <summary>
        /// �Ƿ�wifi����������
        /// </summary>
        bool IsConnecting
        {
            get
            {
                return _IsConnecting;
            }
            set
            {
                Set(ref _IsConnecting, value);
            }
        }

        public static object Lock = new object();

        private Wifi _wifi;
        /// <summary>
        /// wifi������
        /// </summary>
        private Wifi wifi
        {
            get
            {
                lock (Lock)
                {
                    if (_wifi == null)
                    {
                        _wifi = new Wifi();
                    }
                    return _wifi;
                }
            }
        }
        #endregion

        #region Command&&Command Method

        /// <summary>
        /// �г�����Wifi����
        /// </summary>
        public ICommand ListAllCommand { get; set; }

        /// <summary>
        /// Wifiѡ����ı�����
        /// </summary>
        public ICommand OnSelectedItemChangedCommand { get; set; }

        /// <summary>
        /// ���ý�ֹWifi����
        /// </summary>
        public ICommand EnabledOrNotWifiCommand { get; set; }

        /// <summary>
        /// ���û��ֹwifi
        /// </summary>
        void EnabledOrNotWifi()
        {
            IsRunning = true;
            SetNetWorkAdapterEnabeldAsync();
        }

        /// <summary>
        /// �г�����Wifi
        /// </summary>
        void ListAll()
        {
            AccessPointList = new ObservableCollection<MyAccessPoint>(
                wifi.GetAccessPoints().OrderByDescending(s => s.IsConnected).OrderByDescending(ap => ap.SignalStrength).Select(
                    t => new MyAccessPoint() { AccessPoint = t }));
        }

        /// <summary>
        /// Wifiѡ����ı�
        /// </summary>
        async void OnSelectedItemChangedAsync()
        {
            IsRunning = true;
            await OnSelectedItemChangedBodyAsync();
            IsRunning = false;
        }

        #endregion

        #region Other Method

        async void SetNetWorkAdapterEnabeldAsync()
        {
            await Task.Run(new Action(() =>
            {
                try
                {
                    if (!IsEnabledWifi)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            if (AccessPointList.Count > 0)
                            {
                                AccessPointList.Clear();
                            }
                        }));
                    }
                    GetpotentialAdapter();
                    NetWorkAdapter.SetNetWorkAdapterEnabeld(CurrentWifiAdapter, IsEnabledWifi);
                    CheckWifiIsEnabled(700);
                }
                catch (Exception ex)
                { }
                finally
                {
                    IsRunning = false;
                }
            }));
        }

        /// <summary>
        /// Wifiѡ����ı�
        /// </summary>
        async Task OnSelectedItemChangedBodyAsync()
        {
            if (SelectedAccessPoint.AccessPoint.IsConnected)
            {
                var result = await RunWifiStateDialogAsync(SelectedAccessPoint);
                if (result.Equals("1"))
                {
                    SelectedAccessPoint.AccessPoint.DeleteProfile();
                }
            }
            else
            {
                AuthRequest authRequest = new AuthRequest(SelectedAccessPoint.AccessPoint);
                bool overwrite = true;
                if (authRequest.IsPasswordRequired)
                {
                    if (SelectedAccessPoint.AccessPoint.HasProfile)
                    {
                        var result = await RunWifiStateDialogAsync(SelectedAccessPoint);
                        if (result.Equals("1"))
                        {
                            SelectedAccessPoint.AccessPoint.DeleteProfile();
                        }
                        if (result.Equals("2"))
                        {
                            overwrite = false;
                            Connect(SelectedAccessPoint, overwrite, authRequest);
                        }
                    }
                    else
                    {
                        var result = await RunWifiCfgSettingDialogAsync(SelectedAccessPoint);
                        if (result.Item1)
                        {
                            authRequest.Domain = result.Item2;
                            authRequest.Username = result.Item3;
                            authRequest.Password = result.Item4;
                            Connect(SelectedAccessPoint, overwrite, authRequest);
                        }
                    }
                }
                else
                {
                    Connect(SelectedAccessPoint, overwrite, authRequest);
                }
            }
        }

        /// <summary>
        /// ��ȡ�����õ���������
        /// </summary>
        void GetCurrentEnabledAdapter()
        {
            if (CurrentWifiAdapter == null)
            {
                CurrentWifiAdapter = NetWorkAdapter.GetFirstEnabledWirelessAdapter();
            }
        }

        /// <summary>
        /// ��ȡǱ�ڵ���������
        /// </summary>
        void GetpotentialAdapter()
        {
            if (CurrentWifiAdapter == null)
            {
                CurrentWifiAdapter = NetWorkAdapter.GetFirstWirelessAdapter();
            }
        }

        /// <summary>
        /// ���wifi����״̬�����ӳ�ʱ���ټ��
        /// </summary>
        void CheckWifiIsEnabled(int delayMs = 0)
        {
            Task.Run(new Action(() =>
            {
                CheckWifiIsEnabled(IsEnabledWifi);
            }));
            if (delayMs > 0)
            {
                Task.Run(new Action(() =>
                {
                    if (IsEnabledWifi)
                    {
                        Thread.Sleep(delayMs);
                    }
                    CheckWifiIsEnabled(IsEnabledWifi);
                }));
            }
        }

        /// <summary>
        /// ���wifi����״̬
        /// </summary>
        /// <param name="isEnabledWifi"></param>
        void CheckWifiIsEnabled(bool isEnabledWifi)
        {
            if (isEnabledWifi)
            {
                ListAll();
            }
            else
            {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (AccessPointList.Count > 0)
                    {
                        AccessPointList.Clear();
                    }
                }));
            }
        }

        /// <summary>
        /// ����wifi
        /// </summary>
        /// <param name="point"></param>
        /// <param name="overwrite"></param>
        /// <param name="authRequest"></param>
        void Connect(MyAccessPoint point, bool overwrite, AuthRequest authRequest)
        {
            point.PromptMsg = "������....";
            IsConnecting = true;
            point.AccessPoint.ConnectAsync(authRequest, overwrite, GiveMsgIfConnectFailed);
        }

        /// <summary>
        /// ����Wifi��������ģ̬����
        /// </summary>
        /// <param name="authRequest"></param>
        /// <returns>�Ƿ����óɹ�/Domain/Username/Password</returns>
        static async Task<Tuple<bool, string, string, string>> RunWifiCfgSettingDialogAsync(MyAccessPoint point)
        {
            WifiSettingViewModel viewmodel = new WifiSettingViewModel(point);
            var result = await ACalDialog.Show(viewmodel);
            if (result.Equals("0"))
            {
                return new Tuple<bool, string, string, string>(false, "", "", "");
            }
            else
            {
                return new Tuple<bool, string, string, string>(true, viewmodel.Domain, viewmodel.UserName, viewmodel.Password);
            }
        }

        /// <summary>
        /// ����Wifi״̬ģ̬����
        /// </summary>
        /// <param name="point"></param>
        /// <returns>1��ɾ�������ļ� 2������</returns>
        static async Task<object> RunWifiStateDialogAsync(MyAccessPoint point)
        {
            var viewmodel = new WifiStateViewModel(point);
            return await ACalDialog.Show(viewmodel);
        }

        /// <summary>
        /// ������Ϣ״̬ģ̬����
        /// </summary>
        /// <param name="content"></param>
        /// <param name="buttonContent"></param>
        /// <returns></returns>
        static async Task<object> RunMsgViewAsync(string content, string buttonContent)
        {
            var viewmodel = new WifiMsgViewModel(content, buttonContent);
            return await ACalDialog.Show(viewmodel);
        }

        /// <summary>
        /// Wifi�����Ƿ�ɹ��ص�����
        /// </summary>
        /// <param name="success"></param>
        void GiveMsgIfConnectFailed(bool success)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(async () =>
            {
                if (!success)
                {
                    await RunMsgViewAsync("����ʧ��" /*+ "(" + SelectedAccessPoint.AccessPoint.WlanNotConnectableReason + ")"*/, "֪����");
                }
            }));
            SelectedAccessPoint.PromptMsg = "";
            ListAll();
            IsConnecting = false;
        }


        #endregion
    }
}