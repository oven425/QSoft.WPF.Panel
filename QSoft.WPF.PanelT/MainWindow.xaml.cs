using System.Collections.ObjectModel;
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

namespace QSoft.WPF.PanelT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainUI();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //var button = sender as Button;
            //Point relativePoint = button.TranslatePoint(new Point(0, 0), flexpanel);
            //System.Diagnostics.Trace.WriteLine($"{relativePoint} {button.ActualWidth} {button.ActualHeight}");
        }
    }

    public class MainUI
    {
        public ObservableCollection<QSoft.WPF.Panel.JustifyContent> JustifyContents { get; set; } =
        [
            QSoft.WPF.Panel.JustifyContent.Start,
            QSoft.WPF.Panel.JustifyContent.End,
            QSoft.WPF.Panel.JustifyContent.Center,
            QSoft.WPF.Panel.JustifyContent.SpaceAround,
            QSoft.WPF.Panel.JustifyContent.SpaceBetween
        ];

        public ObservableCollection<QSoft.WPF.Panel.AlignItems> AlignItems { get; set; } =
        [
            QSoft.WPF.Panel.AlignItems.Start,
            QSoft.WPF.Panel.AlignItems.End,
            QSoft.WPF.Panel.AlignItems.Center,
            QSoft.WPF.Panel.AlignItems.Stretch,
            //QSoft.WPF.Panel.AlignItems.BaeseLine
        ];
        public ObservableCollection<QSoft.WPF.Panel.FlexDirection> FlexDirections { get; set; } =
        [
            QSoft.WPF.Panel.FlexDirection.Row,
            QSoft.WPF.Panel.FlexDirection.Column,
        ];
    }

    public class DpiDecorator : Decorator
    {
        public DpiDecorator()
        {
            this.Loaded += (s, e) =>
            {
                var v = PresentationSource.FromVisual(this);
                if(v is not null)
                {
                    Matrix m = v.CompositionTarget.TransformToDevice;
                    ScaleTransform dpiTransform = new ScaleTransform(1 / m.M11, 1 / m.M22);
                    if (dpiTransform.CanFreeze)
                        dpiTransform.Freeze();
                    this.LayoutTransform = dpiTransform;
                }
                
            };
        }
    }
}