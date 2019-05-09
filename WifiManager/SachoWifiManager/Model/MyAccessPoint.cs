using GalaSoft.MvvmLight;
using SimpleWifi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SachoWifiManager.Model
{
    public class MyAccessPoint: ViewModelBase
    {
        AccessPoint _AccessPoint = null;
        public AccessPoint AccessPoint
        {
            get
            {
                return _AccessPoint;
            }
            set
            {
                Set(ref _AccessPoint, value);
            }
        }

        string _PromptMsg = "";
        public string PromptMsg
        {
            get
            {
                return _PromptMsg;
            }
            set
            {
                Set(ref _PromptMsg, value);
            }
        }
    }
}
