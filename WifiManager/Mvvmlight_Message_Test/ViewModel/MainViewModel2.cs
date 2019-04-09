using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mvvmlight_Message_Test.ViewModel
{
    public class MainViewModel2 : ViewModelBase
    {
        private string _msg;


        public string Msg
        {
            get
            {
                return _msg;
            }
            set
            {
                _msg = value;
                RaisePropertyChanged(() => Msg);
            }
        }

        public RelayCommand SendCommand
        {
            get; set;
        }

        public MainViewModel2()
        {
            SendCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send<string>(Msg, "Send");
            });
        }
    }
}
