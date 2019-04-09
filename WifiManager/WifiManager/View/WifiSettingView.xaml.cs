using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WifiManager.View
{
    /// <summary>
    /// WifiSettingView.xaml 的交互逻辑
    /// </summary>
    public partial class WifiSettingView : Window
    {
        public WifiSettingView()
        {
            InitializeComponent();
            this.Unloaded += WifiSettingView_Unloaded;
            Messenger.Default.Register<string>(this,ViewMessage.CloseMsg, Close);
            Messenger.Default.Register<string>(this, ViewMessage.ShowMessageMsg, ShowDialog);
        }

        private void WifiSettingView_Unloaded(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Unregister(this);
        }

        void Close(string str)
        {
            this.Close();
        }

        public async void ShowDialog(string xx)
        {
            var view = new DialogView(xx);
            await DialogHost.Show(view);
        }
    }
}
