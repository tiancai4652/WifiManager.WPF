using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;
using SachoWifiManager.Model;
using SimpleWifi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SachoWifiManager.ViewModel
{
    public class WifiSettingViewModel : ModalWindowBase
    {
        public WifiSettingViewModel(MyAccessPoint selectedAccessPoint)
        {
            SelectedAccessPoint = selectedAccessPoint;
            PwdCanSeeChangedCommand = new RelayCommand(PwdCanSeeChanged);
        }

        #region Binding Property

        ImageSource _ImageSource = CantSeeImage;
        public ImageSource ImageSource
        {
            get
            {
                return _ImageSource;
            }
            set
            {
                _ImageSource = value;
                RaisePropertyChanged(() => ImageSource);
            }
        }

        bool _IsPwdCanSee = false;
        /// <summary>
        /// 密码是否明文显示
        /// </summary>
        public bool IsPwdCanSee
        {
            get
            {
                return _IsPwdCanSee;
            }
            set
            {
                _IsPwdCanSee = value;
                RaisePropertyChanged(() => IsPwdCanSee);
                OnIsCanSeePwdChanged();
            }
        }

        string _PromptMessage;
        /// <summary>
        /// 提示信息
        /// </summary>
        public string PromptMessage
        {
            get
            {
                return _PromptMessage;
            }
            set
            {
                _PromptMessage = value;
                RaisePropertyChanged(() => PromptMessage);
            }
        }

        string _Domain;
        /// <summary>
        /// 域信息
        /// </summary>
        public string Domain
        {
            get
            {
                return _Domain;
            }
            set
            {
                _Domain = value;
                RaisePropertyChanged(() => Domain);
            }
        }

        string _UserName;
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                _UserName = value;
                RaisePropertyChanged(() => UserName);
            }
        }

        string _Password;
        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                _Password = value;
                RaisePropertyChanged(() => Password);
            }
        }

        /// <summary>
        /// 支持填写用户名
        /// </summary>
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
                    if (authRequest == null)
                    {
                        authRequest = new AuthRequest(SelectedAccessPoint.AccessPoint);
                    }
                    if (authRequest.IsPasswordRequired && authRequest.IsUsernameRequired)
                    {
                        return true;
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// 支持填写域
        /// </summary>
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
                    if (authRequest == null)
                    {
                        authRequest = new AuthRequest(SelectedAccessPoint.AccessPoint);
                    }
                    if (authRequest.IsPasswordRequired && authRequest.IsDomainSupported)
                    {
                        return true;
                    }
                    return false;
                }
            }
        }

        #endregion

        #region Private Property

        AuthRequest authRequest = null;

        /// <summary>
        /// 选择的Wifi
        /// </summary>
        public MyAccessPoint SelectedAccessPoint { get; set; }

        static ImageSource _CanSeeImage;
        static ImageSource CanSeeImage
        {
            get
            {
                if (_CanSeeImage == null)
                {
                    _CanSeeImage = new BitmapImage(new Uri("pack://application:,,,/SachoWifiManager;component/Resource/CanSee.png", UriKind.Absolute));
                }
                return _CanSeeImage;
            }
        }

        static ImageSource _CantSeeImage;
        static ImageSource CantSeeImage
        {
            get
            {
                if (_CantSeeImage == null)
                {
                    _CantSeeImage = new BitmapImage(new Uri("pack://application:,,,/SachoWifiManager;component/Resource/CantSee.png", UriKind.Absolute));
                }
                return _CantSeeImage;
            }
        }


        #endregion

        #region Command&&Command Method

        /// <summary>
        /// 密码是否可见状态改变命令
        /// </summary>
        public ICommand PwdCanSeeChangedCommand { get; set; }

        /// <summary>
        /// 密码是否可见状态改变
        /// </summary>
        void PwdCanSeeChanged()
        {
            IsPwdCanSee = !IsPwdCanSee;
        }
        #endregion

        #region Other Method

        /// <summary>
        /// 检查输入内容
        /// </summary>
        /// <returns></returns>
        bool CheckInput()
        {
            bool isOK = Check(IsSupportUsername, UserName) && Check(IsSupportDomain, Domain) && Check(true, Password);
            if (!isOK)
            {
                PromptMessage = "请补全信息!";
            }
            else
            {
                if (!SelectedAccessPoint.AccessPoint.IsValidPassword(Password))
                {
                    PromptMessage = "密码格式错误!";
                }
                PromptMessage = "";
            }
            return isOK;
        }

        bool Check(bool IsEnabled, string content)
        {
            if (!IsEnabled)
            {
                return true;
            }
            else
            {
                if (!string.IsNullOrEmpty(content))
                {
                    return true;
                }
            }
            return false;
        }

        void OnIsCanSeePwdChanged()
        {
            if (IsPwdCanSee)
            {
                if (ImageSource != CantSeeImage)
                {
                    ImageSource = CantSeeImage;
                }
            }
            else
            {
                if (ImageSource != CanSeeImage)
                {
                    ImageSource = CanSeeImage;
                }
            }
        }
        #endregion

        public override void OnClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, "1")) return;

            if (!CheckInput())
            {
                eventArgs.Cancel();
            }
        }
    }
}
