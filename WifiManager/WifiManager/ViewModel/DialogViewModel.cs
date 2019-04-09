using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WifiManager.ViewModel
{
    public class DialogViewModel: ViewModelBase
    {
    
        public DialogViewModel()
        {
            //Messenger.Default.Register<string>(this, ViewMessage.ShowMessageMsg, (msg) => { Message = msg; });
        }

        private string _Message;

        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                _Message = value;
                RaisePropertyChanged(() => Message);
            }
        }

    }
}
