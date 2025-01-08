using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wpf_WebView2
{
    /// <summary>
    /// WebViewReactjs.xaml 的互動邏輯
    /// </summary>
    public partial class WebViewReactjs : UserControl
    {
        public WebViewReactjs()
        {
            InitializeComponent();
        }
        WebViewReactjsVM m_MainUI;
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if(this.m_MainUI == null)
            {
                this.DataContext =this.m_MainUI = new WebViewReactjsVM();
                var GetDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var root = System.IO.Path.GetDirectoryName(GetDirectory);
                root = System.IO.Path.GetDirectoryName(root);
                root = System.IO.Path.GetDirectoryName(root);
                root = @$"{root}\reactjst\dist";

                var options = new CoreWebView2EnvironmentOptions("--allow-file-access-from-files");
                var environment = await CoreWebView2Environment.CreateAsync(null, null, options);
                await webview2.EnsureCoreWebView2Async(environment);
                var reactjs = @$"{root}\index.html";
                webview2.CoreWebView2.Navigate(reactjs);
            }
        }

        private void webview2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            System.Diagnostics.Trace.WriteLine(e.WebMessageAsJson);
        }

        private async void button_alert_Click(object sender, RoutedEventArgs e)
        {
            await webview2.CoreWebView2.ExecuteScriptAsync($"alert({DateTime.Now:T})");
        }
    }

    public class WebViewReactjsVM : INotifyPropertyChanged
    {
        public string ExcutSnn { set; get; }
        public event PropertyChangedEventHandler? PropertyChanged;
        void Update(string name)=>this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
