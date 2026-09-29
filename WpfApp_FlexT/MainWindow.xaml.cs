using Microsoft.Web.WebView2.Core;
using QSoft.WPF.Panel;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp_FlexT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string WebPageHostName = "flexapp.example";
        const string WebPageFolder = "webpage";
        static readonly JsonSerializerOptions WebMessageJsonOptions = new()
        {
            Converters = { new JsonStringEnumConverter() },
        };

        string? webPageDataJson;

        public MainWindow()
        {
            InitializeComponent();
            InitializeWebViewAsync();
        }

        async void InitializeWebViewAsync()
        {
            await web.EnsureCoreWebView2Async();
            web.CoreWebView2.SetVirtualHostNameToFolderMapping(
                WebPageHostName,
                System.IO.Path.Combine(AppContext.BaseDirectory, WebPageFolder),
                CoreWebView2HostResourceAccessKind.Deny);
            // Messages posted before the page script runs are lost, so send the data again after every load.
            web.CoreWebView2.NavigationCompleted += (_, e) =>
            {
                if (e.IsSuccess) PostWebPageData();
            };
            web.CoreWebView2.Navigate($"https://{WebPageHostName}/index.html");
        }


        public void ShowInWebView(FlexPanelTestData data)
        {
            webPageDataJson = JsonSerializer.Serialize(data, WebMessageJsonOptions);
            PostWebPageData();
        }

        void PostWebPageData()
        {
            if (webPageDataJson is not null && web.CoreWebView2 is not null)
            {
                web.CoreWebView2.PostWebMessageAsJson(webPageDataJson);
            }
        }

        private void button_set_Click(object sender, RoutedEventArgs e)
        {
            ShowInWebView(new FlexPanelTestData()
            {
                Childs = 
                [
                    new (),
                    new (),
                    new (),
                    new (),
                    new (),
                    new (),
                    new (),
                    new (),
                    new ()
                    ]
            });
        }
    }

    public class FlexPanelTestData
    {
        public Thickness Padding { set; get; }
        public double Gap { set; get; }
        public JustifyContent JustifyContent { get; set; }
        public FlexDirection Direction { get; set; }
        public AlignItems AlignItems { set; get; }
        public List<FlexPanelTestDataChild> Childs { get; set; } = [];
    }

    public class FlexPanelTestDataChild
    {
        public int Basis { set; get; }
        public int Shrink {  set; get; }
        public int Grow {  set; get; }
    }
}