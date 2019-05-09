using SachoWifiManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SachoWifiManager.ViewModel
{
    public class WifiMsgViewModel : ModalWindowBase
    {
        string _Content = "";
        public string Content
        {
            get
            {
                return _Content;
            }
            set
            {
                Set(ref _Content, value);
            }
        }

        string _ButtonContent = "";
        public string ButtonContent
        {
            get
            {
                return _ButtonContent;
            }
            set
            {
                Set(ref _ButtonContent, value);
            }
        }

        public WifiMsgViewModel(string content, string buttonContent)
        {
            Content = content;
            ButtonContent = buttonContent;
        }



    }
}
