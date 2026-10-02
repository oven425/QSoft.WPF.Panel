using Microsoft.Web.WebView2.Core;
using Microsoft.Win32;
using QSoft.WPF.Panel;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp_FlexT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string WebPageHostName = "flexapp.example";
        const string WebPageFolder = "webpage";
        const string TestCaseFolder = "TestCase";
        // Rects within this many DIPs/CSS-px of each other count as a match; accounts for the two
        // engines' differing sub-pixel rounding rather than a real FlexPanel layout bug.
        const double CompareTolerance = 1.0;
        static readonly JsonSerializerOptions WebMessageJsonOptions = new()
        {
            Converters = { new JsonStringEnumConverter() },
            // The web page echoes measurements back with camelCase (JS-natural) property names.
            PropertyNameCaseInsensitive = true,
        };

        string? webPageDataJson;
        int requestSequence;
        int currentRequestId = -1;
        List<ItemRect>? lastWpfItemRects;
        FlexPanelTestData? currentTestData;
        // Opened and closed by hand from MouseEnter/MouseLeave instead of through ToolTipService, which waits out a show
        // delay and keeps the tip open while the mouse crosses the "safe area" between the item and the tip. Driving it
        // directly shows the tip the moment the mouse enters an item and hides it the moment the mouse leaves.
        readonly ToolTip flexItemToolTip = new();
        // The web page keeps the viewport size it was last sent, so it has to be sent the new one when the window is
        // resized. A drag raises SizeChanged for every mouse move; the timer folds them into one update per tick, and
        // moves the update out of the layout pass that raises SizeChanged, where nothing can be measured yet.
        readonly DispatcherTimer resizeTimer = new() { Interval = TimeSpan.FromMilliseconds(50) };

        public MainWindow()
        {
            InitializeComponent();
            // The tip's popup looks this key up through its PlacementTarget (a flex item), so setting it on the panel
            // removes the ~150 ms fade in/out (applied when the OS animates tooltips) for the item tips only.
            flexpanel.Resources[SystemParameters.ToolTipPopupAnimationKey] = PopupAnimation.None;
            InitializeWebViewAsync();
            resizeTimer.Tick += ResizeTimer_Tick;
            SizeChanged += MainWindow_SizeChanged;
            Closed += (_, _) => resizeTimer.Stop();
            // Deferred to Loaded: before the window has an HWND/real size, UpdateLayout() inside
            // ApplyTestCase can't measure real WPF item rects (they'd all come back as 0,0,0,0).
            Loaded += (_, _) => LoadTestCases();
        }

        // Position/size of one flex item, in the same coordinate space (relative to the flex
        // container's own top-left, i.e. inside its Padding) for both the WPF and web renderers.
        sealed record ItemRect(double X, double Y, double Width, double Height)
        {
            public override string ToString() => $"{X:F1}, {Y:F1}, {Width:F1}, {Height:F1}";
        }

        // Shape of the JSON the web page posts back via window.chrome.webview.postMessage after it
        // renders and measures #box's children (see webpage/app.js).
        sealed record MeasurementMessage(string? Type, int RequestId, List<ItemRect>? Items);

        // The size WPF's ScrollViewer gave the FlexPanel. The web page lays #box out inside a viewport of exactly
        // this size rather than the WebView2 control's own bounds, which are resized asynchronously (for example when
        // the result list below changes height) and would otherwise race with the web page's measurement.
        sealed record ViewportSize(double Width, double Height);

        // What WPF sends to the web page: the test data and the viewport it was laid out in, plus a RequestId so the
        // measurements the page echoes back can be matched to the WPF-side snapshot taken for that same render.
        sealed record WebPageEnvelope(int RequestId, ViewportSize Viewport, FlexPanelTestData TestData);

        sealed class ComparisonRow
        {
            public int Index { get; init; }
            public string WpfRect { get; init; } = "-";
            public string WebRect { get; init; } = "-";
            public string DeltaRect { get; init; } = "-";
            public bool IsMatch { get; init; }
            public string StatusText => IsMatch ? "\u2713 \u76f8\u7b26" : "\u2717 \u4e0d\u540c";
        }

        // One entry per TestCase\*.json file. DisplayName prefers FlexPanelTestData.Description so the
        // picker stays readable even when the file name alone can't express what the case covers.
        sealed record TestCaseItem(string FilePath, string DisplayName, FlexPanelTestData Data);

        void LoadTestCases()
        {
            // Fully qualified: a plain "using System.IO;" would clash with the Path type already
            // brought in by "using System.Windows.Shapes;" above.
            var folder = System.IO.Path.Combine(AppContext.BaseDirectory, TestCaseFolder);
            var items = new List<TestCaseItem>();
            if (System.IO.Directory.Exists(folder))
            {
                foreach (var file in System.IO.Directory.EnumerateFiles(folder, "*.json").OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
                {
                    FlexPanelTestData? data;
                    try
                    {
                        data = JsonSerializer.Deserialize<FlexPanelTestData>(System.IO.File.ReadAllText(file), WebMessageJsonOptions);
                    }
                    catch (JsonException)
                    {
                        continue; // Skip files that aren't valid FlexPanelTestData JSON.
                    }
                    if (data is null) continue;

                    var displayName = string.IsNullOrWhiteSpace(data.Description)
                        ? System.IO.Path.GetFileNameWithoutExtension(file)
                        : data.Description;
                    items.Add(new TestCaseItem(file, displayName, data));
                }
            }

            combobox_testcase.ItemsSource = items;
            if (items.Count > 0)
            {
                combobox_testcase.SelectedIndex = 0;
            }
        }

        private void combobox_testcase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (combobox_testcase.SelectedItem is TestCaseItem item)
            {
                ApplyTestCase(item.Data);
            }
        }

        private void button_recompare_Click(object sender, RoutedEventArgs e)
        {
            // Renders both sides again from scratch, for example after the OS theme changed. Resizing the window
            // needs no click: MainWindow_SizeChanged brings the web page up to date by itself.
            if (currentTestData is not null)
            {
                ApplyTestCase(currentTestData);
            }
        }

        void ApplyTestCase(FlexPanelTestData data)
        {
            currentTestData = data;

            ShowInFlexPanel(data);
            MeasureAndShowInWebView(data);

            listview_comparison.ItemsSource = null;
            textblock_summary.Text = "\u6bd4\u5c0d\u4e2d... (\u7b49\u5f85\u7db2\u9801\u91cf\u6e2c\u7d50\u679c)";
        }

        // Takes the WPF snapshot of the FlexPanel as it is laid out now and has the web page lay the same test case out in
        // the same viewport. The comparison follows when the page's measurements come back (OnWebMessageReceived).
        void MeasureAndShowInWebView(FlexPanelTestData data)
        {
            currentRequestId = ++requestSequence;
            // Forces a synchronous layout pass so ActualWidth/ActualHeight/TranslatePoint below are
            // accurate immediately, instead of waiting for WPF's next async layout pass.
            flexpanel.UpdateLayout();
            lastWpfItemRects = MeasureFlexPanelItems();

            ShowInWebView(data, currentRequestId);
        }

        void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (currentTestData is not null && !resizeTimer.IsEnabled) resizeTimer.Start();
        }

        // WPF lays the FlexPanel out again for the new size by itself, but the web page only knows the viewport it was
        // sent. The table is not emptied meanwhile (unlike for a new test case) so it does not flicker while dragging.
        void ResizeTimer_Tick(object? sender, EventArgs e)
        {
            resizeTimer.Stop();
            if (currentTestData is not null) MeasureAndShowInWebView(currentTestData);
        }

        List<ItemRect> MeasureFlexPanelItems()
        {
            var rects = new List<ItemRect>();
            foreach (UIElement child in flexpanel.Children)
            {
                if (child is FrameworkElement fe)
                {
                    var origin = fe.TranslatePoint(new Point(0, 0), flexpanel);
                    rects.Add(new ItemRect(origin.X, origin.Y, fe.ActualWidth, fe.ActualHeight));
                }
            }
            return rects;
        }

        void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            MeasurementMessage? message;
            try
            {
                message = JsonSerializer.Deserialize<MeasurementMessage>(e.WebMessageAsJson, WebMessageJsonOptions);
            }
            catch (JsonException)
            {
                return; // Not a measurement message we understand; ignore.
            }
            // Ignore stale replies from a render that's since been replaced by a newer test case/re-compare.
            if (message is not { Type: "measurements" } || message.RequestId != currentRequestId) return;

            CompareAndShow(message.Items ?? []);
        }

        void CompareAndShow(List<ItemRect> webRects)
        {
            var wpfRects = lastWpfItemRects ?? [];
            int count = Math.Max(wpfRects.Count, webRects.Count);
            var rows = new List<ComparisonRow>(count);
            int mismatches = 0;

            for (int i = 0; i < count; i++)
            {
                var w = i < wpfRects.Count ? wpfRects[i] : null;
                var b = i < webRects.Count ? webRects[i] : null;
                bool isMatch = w is not null && b is not null
                    && Math.Abs(w.X - b.X) <= CompareTolerance
                    && Math.Abs(w.Y - b.Y) <= CompareTolerance
                    && Math.Abs(w.Width - b.Width) <= CompareTolerance
                    && Math.Abs(w.Height - b.Height) <= CompareTolerance;
                if (!isMatch) mismatches++;

                rows.Add(new ComparisonRow
                {
                    Index = i,
                    WpfRect = w?.ToString() ?? "-",
                    WebRect = b?.ToString() ?? "-",
                    DeltaRect = w is not null && b is not null
                        ? $"{w.X - b.X:F1}, {w.Y - b.Y:F1}, {w.Width - b.Width:F1}, {w.Height - b.Height:F1}"
                        : "-",
                    IsMatch = isMatch,
                });
            }

            listview_comparison.ItemsSource = rows;
            textblock_summary.Text = mismatches == 0
                ? $"\u2705 \u5168\u90e8 {count} \u500b\u9805\u76ee\u76f8\u7b26 (\u8aa4\u5dee\u5bb9\u8a31 {CompareTolerance:F1}px)"
                : $"\u274c {mismatches}/{count} \u500b\u9805\u76ee\u4e0d\u540c (\u8aa4\u5dee\u5bb9\u8a31 {CompareTolerance:F1}px)";
        }

        async void InitializeWebViewAsync()
        {
            await web.EnsureCoreWebView2Async();
            web.CoreWebView2.SetVirtualHostNameToFolderMapping(
                WebPageHostName,
                System.IO.Path.Combine(AppContext.BaseDirectory, WebPageFolder),
                CoreWebView2HostResourceAccessKind.Deny);
            web.CoreWebView2.WebMessageReceived += OnWebMessageReceived;
            // Messages posted before the page script runs are lost, so send the data again after every load.
            web.CoreWebView2.NavigationCompleted += (_, e) =>
            {
                if (e.IsSuccess) PostWebPageData();
            };
            web.CoreWebView2.Navigate($"https://{WebPageHostName}/index.html");
        }


        public void ShowInWebView(FlexPanelTestData data, int requestId)
        {
            var viewport = new ViewportSize(scrollviewer_flexpanel.ViewportWidth, scrollviewer_flexpanel.ViewportHeight);
            webPageDataJson = JsonSerializer.Serialize(new WebPageEnvelope(requestId, viewport, data), WebMessageJsonOptions);
            PostWebPageData();
        }

        void PostWebPageData()
        {
            if (webPageDataJson is not null && web.CoreWebView2 is not null)
            {
                web.CoreWebView2.PostWebMessageAsJson(webPageDataJson);
            }
        }

        public void ShowInFlexPanel(FlexPanelTestData data)
        {
            scrollviewer_flexpanel.HorizontalScrollBarVisibility = data.Parent.EnableHorizontalScrollbar
                ? ScrollBarVisibility.Auto
                : ScrollBarVisibility.Disabled;
            scrollviewer_flexpanel.VerticalScrollBarVisibility = data.Parent.EnableVerticalScrollbar
                ? ScrollBarVisibility.Auto
                : ScrollBarVisibility.Disabled;

            // Resolved once per Set click so every element below uses the same light/dark colors as the web page.
            bool isDark = FlexWebPalette.IsOsDarkTheme();
            scrollviewer_flexpanel.Background = FlexWebPalette.BoxBackground(isDark);

            flexpanel.Padding = data.Padding;
            flexpanel.Gap = data.Gap;
            flexpanel.JustifyContent = data.JustifyContent;
            flexpanel.FlexDirection = data.Direction;
            flexpanel.AlignItems = data.AlignItems;

            flexpanel.Children.Clear();
            for (int i = 0; i < data.Childs.Count; i++)
            {
                var item = CreateFlexChild(data.Childs[i], i, isDark);
                item.MouseEnter += FlexItem_MouseEnter;
                item.MouseLeave += FlexItem_MouseLeave;
                flexpanel.Children.Add(item);
            }
        }

        void FlexItem_MouseEnter(object sender, MouseEventArgs e)
        {
            var item = (UIElement)sender;
            double basis = FlexPanel.GetBasis(item);
            // FlexPanel only honours a positive Basis; otherwise the item keeps its own size ("auto").
            flexItemToolTip.Content = $"Grow: {FlexPanel.GetGrow(item)}\nShrink: {FlexPanel.GetShrink(item)}\nBasis: {(basis > 0 ? basis.ToString() : "auto")}";
            flexItemToolTip.PlacementTarget = item;
            flexItemToolTip.IsOpen = true;
        }

        void FlexItem_MouseLeave(object sender, MouseEventArgs e) => flexItemToolTip.IsOpen = false;

        static UIElement CreateFlexChild(FlexPanelTestDataChild child, int index, bool isDark)
        {
            var border = new Border
            {
                BorderBrush = FlexWebPalette.ItemBorder(isDark),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                // Matches the web ".item" padding (.125rem = 2px) so both sides box the same content the same way.
                Padding = new Thickness(2),
                Background = FlexWebPalette.ItemBackground(isDark),
                Child = new TextBlock
                {
                    Text = $"index: {index}",
                    // Matches the browser's default font (16px "Segoe UI"); WPF's default control font is smaller,
                    // which otherwise makes the WPF items noticeably narrower than their web counterparts.
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 16,
                    // Matches the web page's "body { line-height: 1.5 }" (1.5 * 16px = 24px). Without this, WPF's
                    // line box uses the font's own (shorter) natural line height, making the item noticeably shorter.
                    LineHeight = 24,
                    LineStackingStrategy = LineStackingStrategy.BlockLineHeight,
                    Foreground = FlexWebPalette.PageForeground(isDark),
                    Padding = new Thickness(12, 4, 12, 4),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                },
            };

            FlexPanel.SetBasis(border, child.Basis);
            FlexPanel.SetShrink(border, child.Shrink);
            FlexPanel.SetGrow(border, child.Grow);

            return border;
        }
    }

    // Colors converted from webpage/style.css's OKLCH palette (Tailwind CSS v4 gray/neutral scale) so the WPF
    // FlexPanel matches the WebView2 rendering in both light and dark mode.
    static class FlexWebPalette
    {
        static readonly SolidColorBrush White = Freeze(Colors.White);
        static readonly SolidColorBrush Gray600 = Freeze(Color.FromRgb(0x4A, 0x55, 0x65));
        static readonly SolidColorBrush Gray800 = Freeze(Color.FromRgb(0x1E, 0x29, 0x39));
        static readonly SolidColorBrush Gray900 = Freeze(Color.FromRgb(0x10, 0x18, 0x28));
        static readonly SolidColorBrush Neutral200 = Freeze(Color.FromRgb(0xE5, 0xE5, 0xE5));
        static readonly SolidColorBrush Neutral400 = Freeze(Color.FromRgb(0xA1, 0xA1, 0xA1));
        static readonly SolidColorBrush Neutral700 = Freeze(Color.FromRgb(0x40, 0x40, 0x40));

        public static Brush PageForeground(bool isDark) => isDark ? White : Neutral700;
        public static Brush BoxBackground(bool isDark) => isDark ? Gray900 : Neutral200;
        public static Brush ItemBackground(bool isDark) => isDark ? Gray800 : White;
        public static Brush ItemBorder(bool isDark) => isDark ? Gray600 : Neutral400;

        static SolidColorBrush Freeze(Color color)
        {
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            return brush;
        }

        public static bool IsOsDarkTheme()
        {
            const string KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(KeyPath);
                // Same registry value Chromium/WebView2 reads to resolve the CSS "prefers-color-scheme" media query.
                return key?.GetValue("AppsUseLightTheme") is int appsUseLightTheme && appsUseLightTheme == 0;
            }
            catch
            {
                return false;
            }
        }
    }

    public class Parent
    {
        public bool EnableHorizontalScrollbar { set; get; } = false;
        public bool EnableVerticalScrollbar { set; get; } = false;
    }

    public class FlexPanelTestData
    {
        // Optional human-readable description, used as the picker's display text when the file name
        // alone can't express what the test case covers.
        public string? Description { get; set; }
        public Parent Parent { get; set; } = new();
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