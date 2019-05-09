using SachoWifiManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SachoWifiManager.ViewModel
{
    public class WifiStateViewModel : ModalWindowBase
    {
        MyAccessPoint _AccessPoint;
        /// <summary>
        /// 当前选择的Wifi
        /// </summary>
        public MyAccessPoint AccessPoint
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

        public WifiStateViewModel(MyAccessPoint accessPoint)
        {
            AccessPoint = accessPoint;
        }

    }
}
