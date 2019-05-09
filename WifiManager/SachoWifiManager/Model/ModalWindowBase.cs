using GalaSoft.MvvmLight;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SachoWifiManager.Model
{
    public class ModalWindowBase: ViewModelBase
    {
        /// <summary>
        /// 打开时调用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public virtual void OnOpenning(object sender, DialogOpenedEventArgs eventArgs)
        {

        }

        /// <summary>
        /// 关闭时调用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public virtual void OnClosing(object sender, DialogClosingEventArgs eventArgs)
        {

        }
    }
}
