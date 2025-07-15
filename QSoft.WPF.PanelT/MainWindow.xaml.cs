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
    }

    public class MainUI
    {
        public ObservableCollection<QSoft.WPF.Panel.JustifyContent> JustifyContents { get; set; } =
        [
            QSoft.WPF.Panel.JustifyContent.Left,
            QSoft.WPF.Panel.JustifyContent.Right,
            QSoft.WPF.Panel.JustifyContent.Center,
            QSoft.WPF.Panel.JustifyContent.SpaceAround,
            QSoft.WPF.Panel.JustifyContent.SpaceBetween
        ];

        public ObservableCollection<QSoft.WPF.Panel.AlignItems> AlignItems { get; set; } =
        [
            QSoft.WPF.Panel.AlignItems.Top,
            QSoft.WPF.Panel.AlignItems.Bottom,
            QSoft.WPF.Panel.AlignItems.Center,
            QSoft.WPF.Panel.AlignItems.Stretch,
            //QSoft.WPF.Panel.AlignItems.BaeseLine
        ];
        public QSoft.WPF.Panel.JustifyContent JustifyContent { set; get; } = QSoft.WPF.Panel.JustifyContent.Left;
        public QSoft.WPF.Panel.AlignItems AlignItem { set; get; } = QSoft.WPF.Panel.AlignItems.Top;
    }

    public class DpiDecorator : Decorator
    {
        public DpiDecorator()
        {
            this.Loaded += (s, e) =>
            {
                Matrix m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
                ScaleTransform dpiTransform = new ScaleTransform(1 / m.M11, 1 / m.M22);
                if (dpiTransform.CanFreeze)
                    dpiTransform.Freeze();
                this.LayoutTransform = dpiTransform;
            };
        }
    }
}