/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:WifiManager"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;


namespace WifiManager.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<WifiSettingViewModel>();
            //SimpleIoc.Default.Register<DialogViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        //public DialogViewModel Dialog
        //{
        //    get
        //    {
        //        return ServiceLocator.Current.GetInstance<DialogViewModel>();
        //    }
        //}

        public WifiSettingViewModel Setter
        {
            get
            {
                var result= ServiceLocator.Current.GetInstance<WifiSettingViewModel>();
                if (Main != null)
                {
                    result.IsSupportDomain = Main.IsSupportDomain;
                    result.IsSupportUsername = Main.IsSupportUsername;
                    if (Main.SelectedAccessPoint != null)
                    {
                        result.Title = Main.SelectedAccessPoint.AccessPoint.Name;
                        result.MyPoint = Main.SelectedAccessPoint;
                    }
                }
                return result;
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}