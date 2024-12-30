using Microsoft.Web.WebView2.Core;
using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_pdf_Click(object sender, RoutedEventArgs e)
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var pdfs = System.IO.Directory.GetFiles(folder, "*.pdf");

            webview2.CoreWebView2.Navigate(pdfs.FirstOrDefault());
        }

        private void button_url_Click(object sender, RoutedEventArgs e)
        {
            webview2.CoreWebView2.Navigate("https://www.yahoo.com.tw");
        }

        private void button_googlemap_Click(object sender, RoutedEventArgs e)
        {
            webview2.CoreWebView2.Navigate("https://www.google.com/maps");
        }
        //https://www.cnblogs.com/xslx/p/17244811.html
        //https://www.cnblogs.com/zhaotianff/p/18256433
        async private void button_reactjs_Click(object sender, RoutedEventArgs e)
        {
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

        private void webview2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {

        }
    }
}