using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WifiManager.ViewModel
{
    public class WifiSettingViewModel : ViewModelBase
    {
        public WifiSettingViewModel()
        {
            CancelCommand = new RelayCommand(Cancel);
            OKCommand = new RelayCommand(OK);
        }

        //public WifiSettingViewModel(bool isSupportDomain, bool isSupportUsername):base()
        //{
        //    IsSupportDomain = isSupportDomain;
        //    IsSupportUsername = isSupportUsername;
        //}

        public MyAccessPoint MyPoint { get; set; }


        string _Title;
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        string _Password;
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

        string _UserName;
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

        string _Domain;
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

        bool _IsSupportDomain;
        public bool IsSupportDomain
        {
            get
            {
                return _IsSupportDomain;
            }
            set
            {
                _IsSupportDomain = value;
                RaisePropertyChanged(() => IsSupportDomain);
            }
        }

        bool _IsSupportUsername;
        public bool IsSupportUsername
        {
            get
            {
                return _IsSupportUsername;
            }
            set
            {
                _IsSupportUsername = value;
                RaisePropertyChanged(() => IsSupportUsername);
            }
        }

        public ICommand CancelCommand { get; set; }
        public ICommand OKCommand { get; set; }

        void Cancel()
        {

            Messenger.Default.Send("",ViewMessage.CancelMsg);
            Messenger.Default.Send("",ViewMessage.CloseMsg);

        }

        void OK()
        {
            bool validPassFormat = MyPoint.AccessPoint.IsValidPassword(Password);

            if (validPassFormat)
            {
                Messenger.Default.Send("", ViewMessage.OKMsg);
                Messenger.Default.Send("", ViewMessage.CloseMsg);
            }
            else
            {
                Messenger.Default.Send("密码错了，兄弟", ViewMessage.ShowMessageMsg);
            }
        }

    



    }
}
