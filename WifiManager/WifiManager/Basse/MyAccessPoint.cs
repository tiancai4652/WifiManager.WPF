using GalaSoft.MvvmLight;
using SimpleWifi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WifiManager
{
    public class MyAccessPoint: ObservableObject
    {
        AccessPoint _AccessPoint=null;
        public AccessPoint AccessPoint
        {
            get
            {
                return _AccessPoint;
            }
            set
            {
                _AccessPoint = value;
                RaisePropertyChanged(() => AccessPoint);
            }
        }
    }
}
