using Microsoft.Win32;
using System.Configuration;
using System.Data;
using System.Runtime;
using System.Windows;
using Windows.UI.ViewManagement;

namespace WpfApp_FlexPanelT
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private UISettings _uiSettings = new UISettings();
        protected override void OnStartup(StartupEventArgs e)
        {
            var clr = _uiSettings.GetColorValue(UIColorType.Background);
            SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
            base.OnStartup(e); 
        }

        private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {

        }
    }

}
