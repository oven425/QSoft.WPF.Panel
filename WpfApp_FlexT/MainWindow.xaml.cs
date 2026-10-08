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
        // "全部" (every category) and "其他" (a file whose name has no category part).
        const string AllCategories = "\u5168\u90e8";
        const string OtherCategory = "\u5176\u4ed6";
        // Search keywords are told apart by a space or by the ideographic space the Chinese IME types.
        static readonly char[] KeywordSeparators = [' ', '\u3000'];
        static readonly JsonSerializerOptions WebMessageJsonOptions = new()
        {
            Converters = { new JsonStringEnumConverter() },
            // The web page echoes measurements back with camelCase (JS-natural) property names.
            PropertyNameCaseInsensitive = true,
        };
        // Same as WebMessageJsonOptions but indented, for showing/editing FlexPanelTestData as readable JSON
        // in the content box (parsing a user edit back still uses the compact WebMessageJsonOptions above).
        static readonly JsonSerializerOptions ContentEditorJsonOptions = new(WebMessageJsonOptions) { WriteIndented = true };
        static readonly Brush ContentEditorNormalBackground = Brushes.White;
        // Same pink the comparison ListView uses for a mismatched row (see MainWindow.xaml), reused here so
        // "something is wrong" looks consistent across the window.
        static readonly Brush ContentEditorErrorBackground = CreateFrozenBrush(0xFB, 0xD9, 0xD9);

        static Brush CreateFrozenBrush(byte r, byte g, byte b)
        {
            var brush = new SolidColorBrush(Color.FromRgb(r, g, b));
            brush.Freeze();
            return brush;
        }

        string? webPageDataJson;
        int requestSequence;
        int currentRequestId = -1;
        List<ItemRect>? lastWpfItemRects;
        FlexPanelTestData? currentTestData;
        // Every test case found in the TestCase folder; the ComboBox lists the part of it that matches the filter.
        List<TestCaseItem> allTestCases = [];
        // The words the search box suggests: the parts of the test case descriptions ("Direction=Column", ...).
        List<string> searchWords = [];
        // True while AcceptSuggestion writes a word into the search box, so that this change does not open the list again.
        bool acceptingSuggestion;
        // Opened and closed by hand from MouseEnter/MouseLeave instead of through ToolTipService, which waits out a show
        // delay and keeps the tip open while the mouse crosses the "safe area" between the item and the tip. Driving it
        // directly shows the tip the moment the mouse enters an item and hides it the moment the mouse leaves.
        readonly ToolTip flexItemToolTip = new();
        // The web page keeps the viewport size it was last sent, so it has to be sent the new one when the window is
        // resized. A drag raises SizeChanged for every mouse move; the timer folds them into one update per tick, and
        // moves the update out of the layout pass that raises SizeChanged, where nothing can be measured yet.
        readonly DispatcherTimer resizeTimer = new() { Interval = TimeSpan.FromMilliseconds(50) };
        // True while a test case selection (or the initial load) writes JSON into the content box, so
        // textbox_content_TextChanged does not mistake that programmatic update for a user edit to apply.
        bool settingContentText;
        // Debounces content-box edits the same way resizeTimer debounces SizeChanged: re-parsing and re-rendering on
        // every keystroke would be wasteful and would flash invalid-JSON errors while a bracket/quote is half-typed.
        readonly DispatcherTimer contentEditTimer = new() { Interval = TimeSpan.FromMilliseconds(500) };

        public MainWindow()
        {
            InitializeComponent();
            // The tip's popup looks this key up through its PlacementTarget (a flex item), so setting it on the panel
            // removes the ~150 ms fade in/out (applied when the OS animates tooltips) for the item tips only.
            flexpanel.Resources[SystemParameters.ToolTipPopupAnimationKey] = PopupAnimation.None;
            InitializeWebViewAsync();
            resizeTimer.Tick += ResizeTimer_Tick;
            contentEditTimer.Tick += ContentEditTimer_Tick;
            SizeChanged += MainWindow_SizeChanged;
            // A popup stays where it is on the screen when the window is dragged away from under it.
            LocationChanged += (_, _) => CloseSuggestions();
            Closed += (_, _) => { resizeTimer.Stop(); contentEditTimer.Stop(); };
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

            // Also the name UI Automation reports for the row, which makes every row readable even when the
            // ListView has not created the cells of the rows that are scrolled out of view.
            public override string ToString() => $"#{Index} | WPF({WpfRect}) | Web({WebRect}) | Diff({DeltaRect}) | {StatusText}";
        }

        // One entry per TestCase\*.json file. DisplayName prefers FlexPanelTestData.Description so the
        // picker stays readable even when the file name alone can't express what the case covers.
        // Category is the part of the file name before its first "_" (WrapFlex for WrapFlex_NoWrap_Column_GrowAll.json),
        // which is how the generated cases are grouped by what they exercise.
        sealed record TestCaseItem(string FilePath, string Category, string DisplayName, FlexPanelTestData Data)
        {
            public string FileName { get; } = System.IO.Path.GetFileName(FilePath);
            // What the search keywords are matched against: the file name and the description, so a case is found by either.
            public string SearchText { get; } = $"{System.IO.Path.GetFileName(FilePath)} {DisplayName}";
            // The comma separated parts of the description ("Direction=Column", "AlignItems=Center", ...): the words the search box suggests.
            public HashSet<string> Keywords { get; } = new(DisplayName.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries), StringComparer.OrdinalIgnoreCase);
        }

        // One entry of the category picker; a null Name stands for every category.
        sealed record CategoryItem(string? Name, int Count)
        {
            public string DisplayName => $"{Name ?? AllCategories} ({Count})";

            // Also the name UI Automation reports for the item.
            public override string ToString() => DisplayName;
        }

        // One entry of the list under the search box: a word, and the number of test cases that have it.
        sealed record SuggestionItem(string Text, int Count)
        {
            // Also the name UI Automation reports for the item.
            public override string ToString() => $"{Text} ({Count})";
        }

        static string GetCategory(string filePath)
        {
            var name = System.IO.Path.GetFileNameWithoutExtension(filePath);
            int separator = name.IndexOf('_');
            return separator > 0 ? name[..separator] : OtherCategory;
        }

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
                    items.Add(new TestCaseItem(file, GetCategory(file), displayName, data));
                }
            }

            allTestCases = items;
            // A part found in one test case only (the number of a Fuzz case, for example) is left out of the suggestions: the
            // list of test cases is the better way to pick a single one. A word with a space in it could not be a keyword.
            searchWords = items
                .SelectMany(t => t.Keywords)
                .GroupBy(word => word, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1 && g.Key.IndexOfAny(KeywordSeparators) < 0)
                .Select(g => g.Key)
                .Order(StringComparer.OrdinalIgnoreCase)
                .ToList();
            var categories = new List<CategoryItem> { new(null, items.Count) };
            categories.AddRange(items
                .GroupBy(t => t.Category)
                .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase)
                .Select(g => new CategoryItem(g.Key, g.Count())));
            combobox_category.ItemsSource = categories;
            // Picking "all" narrows nothing down and selects the first test case, as the plain ComboBox used to.
            combobox_category.SelectedIndex = 0;
        }

        private void combobox_category_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilter();

        private void textbox_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
            UpdateSuggestions();
        }

        // The test cases of the category (null for every category) that match every keyword.
        IEnumerable<TestCaseItem> Search(string? category, string[] keywords) =>
            allTestCases.Where(t => (category is null || t.Category == category) && keywords.All(k => Matches(t, k)));

        // A keyword that is one of the suggested words matches that part of the description only ("Direction=Column" does
        // not match "Direction=ColumnReverse"), so the number shown beside a suggestion is the number of cases picking it
        // leaves. Any other keyword is looked for anywhere in the file name and the description.
        bool Matches(TestCaseItem testCase, string keyword) =>
            IsSearchWord(keyword)
                ? testCase.Keywords.Contains(keyword)
                : testCase.SearchText.Contains(keyword, StringComparison.OrdinalIgnoreCase);

        // searchWords is sorted with this comparer.
        bool IsSearchWord(string keyword) => searchWords.BinarySearch(keyword, StringComparer.OrdinalIgnoreCase) >= 0;

        // Lists the test cases of the chosen category that match the search keywords.
        void ApplyFilter()
        {
            var category = (combobox_category.SelectedItem as CategoryItem)?.Name;
            var keywords = textbox_search.Text.Split(KeywordSeparators, StringSplitOptions.RemoveEmptyEntries);
            var matches = Search(category, keywords).ToList();

            var previous = combobox_testcase.SelectedItem as TestCaseItem;
            combobox_testcase.ItemsSource = matches;
            // The case on screen stays selected while it still matches, so narrowing the filter does not render it again.
            combobox_testcase.SelectedItem = previous is not null && matches.Contains(previous) ? previous : matches.FirstOrDefault();
            // A case that stays selected raises no SelectionChanged, yet its place in the shorter or longer list has changed.
            UpdateStepButtons();
            textblock_count.Text = $"\u7b26\u5408 {matches.Count} / {allTestCases.Count} \u7b46"; // 符合 n / total 筆
        }

        private void textbox_search_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) => UpdateSuggestions();

        private void textbox_search_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) => CloseSuggestions();

        // The word at the caret changes when the caret moves. A list that is closed stays closed, so clicking into the
        // text does not open it.
        private void textbox_search_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (popup_suggestions.IsOpen) UpdateSuggestions();
        }

        // The arrow keys pick a suggestion and Enter or Tab takes it. The keyboard focus stays in the box all the time, so
        // the typing can go on.
        private void textbox_search_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool isOpen = popup_suggestions.IsOpen;
            switch (e.Key)
            {
                case Key.Down:
                    if (isOpen) MoveSuggestion(1); else UpdateSuggestions(force: true);
                    e.Handled = true;
                    break;
                case Key.Up when isOpen:
                    MoveSuggestion(-1);
                    e.Handled = true;
                    break;
                case Key.Enter or Key.Tab when isOpen && listbox_suggestions.SelectedItem is SuggestionItem suggestion:
                    AcceptSuggestion(suggestion);
                    e.Handled = true;
                    break;
                case Key.Escape when isOpen:
                    CloseSuggestions();
                    e.Handled = true;
                    break;
            }
        }

        // Taking a suggestion with the mouse. Handled here, before the list reacts: the items cannot take the keyboard
        // focus anyway, but this keeps the box from ever losing it to a click.
        private void listbox_suggestions_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is DependencyObject source
                && ItemsControl.ContainerFromElement(listbox_suggestions, source) is ListBoxItem { DataContext: SuggestionItem suggestion })
            {
                AcceptSuggestion(suggestion);
                e.Handled = true;
            }
        }

        // The word of the search text the caret is in or touches, as the start and the end of its characters.
        (int Start, int End) CurrentWord()
        {
            var text = textbox_search.Text;
            int caret = Math.Clamp(textbox_search.CaretIndex, 0, text.Length);
            int start = caret;
            while (start > 0 && !KeywordSeparators.Contains(text[start - 1])) start--;
            int end = caret;
            while (end < text.Length && !KeywordSeparators.Contains(text[end])) end++;
            return (start, end);
        }

        // Opens the list under the search box with the words that could complete the word the caret is in, or closes it when
        // there are none. The list belongs to the user typing: the text is also changed by code and by UI Automation, which
        // must not pop it up, so it only opens while the box has the keyboard focus. With no word typed yet it lists every
        // word that narrows the search down, but only for an empty box (the hint when the box is entered) or when the Down
        // key asks for it.
        void UpdateSuggestions(bool force = false)
        {
            var text = textbox_search.Text;
            var (start, end) = CurrentWord();
            var word = text[start..end];
            if (acceptingSuggestion || !textbox_search.IsKeyboardFocused || textbox_search.SelectionLength > 0
                || (word.Length == 0 && text.Length > 0 && !force))
            {
                CloseSuggestions();
                return;
            }

            var otherKeywords = (text[..start] + ' ' + text[end..]).Split(KeywordSeparators, StringSplitOptions.RemoveEmptyEntries);
            var suggestions = FindSuggestions(word, otherKeywords);
            if (suggestions.Count == 0)
            {
                CloseSuggestions();
                return;
            }

            listbox_suggestions.ItemsSource = suggestions;
            listbox_suggestions.SelectedIndex = -1;
            popup_suggestions.IsOpen = true;
        }

        // The words that contain the typed word, each with the number of test cases that have it on top of the other keywords.
        List<SuggestionItem> FindSuggestions(string word, string[] otherKeywords)
        {
            var category = (combobox_category.SelectedItem as CategoryItem)?.Name;
            var remaining = Search(category, otherKeywords).ToList();
            var found = new List<(SuggestionItem Suggestion, int Rank)>();
            foreach (var searchWord in searchWords)
            {
                int index = searchWord.IndexOf(word, StringComparison.OrdinalIgnoreCase);
                // Not a completion of the word, the word itself, or a word the search has already.
                if (index < 0
                    || searchWord.Equals(word, StringComparison.OrdinalIgnoreCase)
                    || otherKeywords.Contains(searchWord, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                // A suggested word matches a part of the description exactly, as in Matches.
                int count = remaining.Count(t => t.Keywords.Contains(searchWord));
                // With nothing typed, only a word that narrows the search down is worth listing.
                if (count == 0 || (word.Length == 0 && count == remaining.Count)) continue;

                // Rank 0 where the typed word starts the suggestion or its value ("Row" in "Direction=Row"), 1 anywhere else.
                found.Add((new SuggestionItem(searchWord, count), index == 0 || searchWord[index - 1] == '=' ? 0 : 1));
            }
            // The sort is stable, so each rank stays in the alphabetical order of the vocabulary, which groups the values of a setting.
            return found.OrderBy(f => f.Rank).Select(f => f.Suggestion).ToList();
        }

        // Moves the pick through the list; above the first entry nothing is picked, which is where typing starts.
        void MoveSuggestion(int offset)
        {
            int index = Math.Clamp(listbox_suggestions.SelectedIndex + offset, -1, listbox_suggestions.Items.Count - 1);
            listbox_suggestions.SelectedIndex = index;
            if (index >= 0) listbox_suggestions.ScrollIntoView(listbox_suggestions.SelectedItem);
        }

        // Writes the suggestion in place of the word the caret is in. At the end of the text a space follows it, ready for
        // the next keyword.
        void AcceptSuggestion(SuggestionItem suggestion)
        {
            var text = textbox_search.Text;
            var (start, end) = CurrentWord();
            var insert = end == text.Length ? suggestion.Text + ' ' : suggestion.Text;
            acceptingSuggestion = true;
            try
            {
                textbox_search.Text = text[..start] + insert + text[end..];
                textbox_search.CaretIndex = start + insert.Length;
            }
            finally
            {
                acceptingSuggestion = false;
            }
            CloseSuggestions();
            textbox_search.Focus();
        }

        void CloseSuggestions() => popup_suggestions.IsOpen = false;

        private void combobox_testcase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateStepButtons();

            if (combobox_testcase.SelectedItem is TestCaseItem item)
            {
                SetContentText(JsonSerializer.Serialize(item.Data, ContentEditorJsonOptions));
                textbox_content.ScrollToHome();
                // Filtering selects the case that is already on screen again; that one needs no new render.
                if (!ReferenceEquals(item.Data, currentTestData)) ApplyTestCase(item.Data);
            }
        }

        // Writes into the content box without textbox_content_TextChanged treating it as a user edit to parse/apply.
        void SetContentText(string text)
        {
            settingContentText = true;
            try
            {
                textbox_content.Text = text;
            }
            finally
            {
                settingContentText = false;
            }
            SetContentEditorValid(true);
        }

        // The content box doubles as a live editor: typing valid FlexPanelTestData JSON re-renders both WPF and the
        // web page (see ContentEditTimer_Tick) without ever writing back to the test case file - switching test cases,
        // stepping, or reloading always reverts to what is on disk. Debounced like window resizing (contentEditTimer),
        // so parsing/re-rendering does not happen on every keystroke and does not flash invalid-JSON errors mid-edit.
        void textbox_content_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (settingContentText) return;
            contentEditTimer.Stop();
            contentEditTimer.Start();
        }

        void ContentEditTimer_Tick(object? sender, EventArgs e)
        {
            contentEditTimer.Stop();

            FlexPanelTestData? data;
            try
            {
                data = JsonSerializer.Deserialize<FlexPanelTestData>(textbox_content.Text, WebMessageJsonOptions);
            }
            catch (JsonException ex)
            {
                SetContentEditorValid(false, ex.Message);
                return;
            }
            if (data?.Parent is null || data.Childs is null)
            {
                SetContentEditorValid(false, "JSON 需要是完整的測試案例物件 (Parent/Childs 不可省略或為 null)");
                return;
            }

            SetContentEditorValid(true);
            ApplyTestCase(data);
        }

        // Tints the content box like a mismatched comparison row when its text is not valid FlexPanelTestData JSON,
        // and shows why as a tooltip; the last successfully applied render is left on screen untouched either way.
        void SetContentEditorValid(bool valid, string? error = null)
        {
            textbox_content.Background = valid ? ContentEditorNormalBackground : ContentEditorErrorBackground;
            textbox_content.ToolTip = error;
        }

        void UpdateStepButtons()
        {
            int index = combobox_testcase.SelectedIndex;
            button_prev.IsEnabled = index > 0;
            button_next.IsEnabled = index >= 0 && index < combobox_testcase.Items.Count - 1;
        }

        private void button_prev_Click(object sender, RoutedEventArgs e) => StepTestCase(-1);

        private void button_next_Click(object sender, RoutedEventArgs e) => StepTestCase(1);

        // Moves through the test cases the filter has left, so a whole category can be walked through case by case.
        void StepTestCase(int offset)
        {
            int index = combobox_testcase.SelectedIndex + offset;
            if (index >= 0 && index < combobox_testcase.Items.Count)
            {
                combobox_testcase.SelectedIndex = index;
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
            flexpanel.RowGap = data.RowGap ?? double.NaN;
            flexpanel.ColumnGap = data.ColumnGap ?? double.NaN;
            flexpanel.FlexWrap = data.Wrap;
            flexpanel.JustifyContent = data.JustifyContent;
            flexpanel.FlexDirection = data.Direction;
            flexpanel.AlignItems = data.AlignItems;
            flexpanel.AlignContent = data.AlignContent;

            bool isRow = data.Direction is FlexDirection.Row or FlexDirection.RowReverse;
            flexpanel.Children.Clear();
            for (int i = 0; i < data.Childs.Count; i++)
            {
                var item = CreateFlexChild(data.Childs[i], i, isDark, isRow);
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

        static UIElement CreateFlexChild(FlexPanelTestDataChild child, int index, bool isDark, bool isRow)
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
            FlexPanel.SetAlignSelf(border, child.AlignSelf);
            border.Margin = child.Margin;

            // Width/MinWidth/MaxWidth (and the Height ones) size the border box, like box-sizing: border-box does for
            // the web items. The main-axis size itself comes from Basis, so only its limits are set here.
            if (child.CrossSize is { } crossSize)
            {
                if (isRow) border.Height = crossSize; else border.Width = crossSize;
            }
            if (child.MinMain is { } minMain)
            {
                if (isRow) border.MinWidth = minMain; else border.MinHeight = minMain;
            }
            if (child.MaxMain is { } maxMain)
            {
                if (isRow) border.MaxWidth = maxMain; else border.MaxHeight = maxMain;
            }

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
        // Overrides Gap on one axis only, like CSS's separate row-gap/column-gap: null keeps using Gap on
        // that axis. row-gap is the physical vertical gap and column-gap the physical horizontal one, the
        // same regardless of Direction (see FlexPanel.GetAxisGaps).
        public double? RowGap { get; set; }
        public double? ColumnGap { get; set; }
        public FlexWrap Wrap { get; set; }
        public JustifyContent JustifyContent { get; set; }
        public FlexDirection Direction { get; set; }
        public AlignItems AlignItems { set; get; }
        public AlignContent AlignContent { get; set; } = AlignContent.Stretch;
        public List<FlexPanelTestDataChild> Childs { get; set; } = [];
    }

    public class FlexPanelTestDataChild
    {
        public int Basis { set; get; }
        public int Shrink {  set; get; }
        public int Grow {  set; get; }
        public AlignSelf AlignSelf { get; set; }
        // Size on the cross axis: the Height of a row, the Width of a column. Null leaves it to the content.
        public double? CrossSize { get; set; }
        // Limits of the main-axis size: MinWidth/MaxWidth of a row, MinHeight/MaxHeight of a column.
        public double? MinMain { get; set; }
        public double? MaxMain { get; set; }
        public Thickness Margin { get; set; }
    }
}