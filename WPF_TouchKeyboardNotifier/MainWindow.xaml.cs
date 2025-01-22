using Microsoft.Windows.Input.TouchKeyboard;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.UI.ViewManagement;

namespace WPF_TouchKeyboardNotifier
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
        
        InputPane? m_InputPane;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            var wih = new WindowInteropHelper(window);
            m_InputPane = Windows.UI.ViewManagement.InputPaneInterop.GetForWindow(wih.Handle);
            m_InputPane.Showing += M_InputPane_Showing;
            m_InputPane.Hiding += M_InputPane_Hiding;
        }

        private void M_InputPane_Hiding(Windows.UI.ViewManagement.InputPane sender, Windows.UI.ViewManagement.InputPaneVisibilityEventArgs args)
        {
            System.Diagnostics.Trace.WriteLine($"Hiding {args.OccludedRect} {args.EnsuredFocusedElementInView}");
        }

        private void M_InputPane_Showing(Windows.UI.ViewManagement.InputPane sender, Windows.UI.ViewManagement.InputPaneVisibilityEventArgs args)
        {
            System.Diagnostics.Trace.WriteLine($"Showing {args.OccludedRect} {args.EnsuredFocusedElementInView}");
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var cc = sender as CheckBox;
            m_InputPane?.TryShow();
            //if(cc.IsChecked == true)
            //{
            //    var bb = m_InputPane?.TryShow();
            //}
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var bb = m_InputPane?.TryShow();
        }
    }
}