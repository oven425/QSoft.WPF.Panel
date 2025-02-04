using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.Foundation.Metadata;
using Windows.Devices.SerialCommunication;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App2
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }
        HttpClient client = new HttpClient();
        private async void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Content = "Clicked";
            try
            {
                var ppp = System.IO.Ports.SerialPort.GetPortNames();
                var resp = await client.GetAsync("https://www.yahoo.com.tw");
                if (resp.IsSuccessStatusCode)
                {
                    var str = await resp.Content.ReadAsStringAsync();

                    ContentDialog content_dialog = new ContentDialog()
                    {
                        Title = "httpclient return",
                        Content = str,
                        PrimaryButtonText = "OK",
                        FullSizeDesired = true,
                    };

                    content_dialog.PrimaryButtonClick += (_s, _e) => { };
                    content_dialog.XamlRoot = this.Content.XamlRoot;
                    await content_dialog.ShowAsync();


                }

            }
            catch (Exception ex)
            {

            }
        }

        private void CommandInvokedHandler(IUICommand command)
        {
            // Display message showing the label of the command that was invoked
            //rootPage.NotifyUser("The '" + command.Label + "' command has been selected.",
            //    NotifyType.StatusMessage);
        }
    }
}
