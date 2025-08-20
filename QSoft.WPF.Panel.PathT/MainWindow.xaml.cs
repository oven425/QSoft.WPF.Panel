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

namespace QSoft.WPF.Panel.PathT
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
        //https://learn.microsoft.com/zh-tw/dotnet/api/system.windows.media.pathgeometry.getpointatfractionlength?view=windowsdesktop-9.0
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //if(this.path.Data is EllipseGeometry ellipse)
            //{
            //    var pp = GetPointsFromEllipse(ellipse);
            //    //ellipse.GetPointAtFractionLength
            //}
        }

        public List<Point> GetPointsFromEllipse(EllipseGeometry ellipse)
        {
            var points = new List<Point>();

            // 1. 將 EllipseGeometry 轉換為只包含直線段的 PathGeometry
            // 第二個參數是 tolerance，值越小，點越多，圖形越精確
            PathGeometry flattenedPath = ellipse.GetFlattenedPathGeometry(0.5, ToleranceType.Absolute);

            // 2. 遍歷 PathGeometry 中的所有 Figure
            foreach (var figure in flattenedPath.Figures)
            {
                // 將起點加入列表
                points.Add(figure.StartPoint);

                // 3. 遍歷 Figure 中的所有 Segment
                foreach (var segment in figure.Segments)
                {
                    // 因為已經扁平化，所以所有的 Segment 都會是 PolyLineSegment 或 LineSegment
                    if (segment is PolyLineSegment polyLineSegment)
                    {
                        foreach (var point in polyLineSegment.Points)
                        {
                            points.Add(point);
                        }
                    }
                    else if (segment is LineSegment lineSegment)
                    {
                        points.Add(lineSegment.Point);
                    }
                }
            }

            return points;
        }


        System.Windows.Threading.DispatcherTimer m_Timer;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            m_Timer = new();
            m_Timer.Interval = TimeSpan.FromMilliseconds(1000);
            m_Timer.Tick += M_Timer_Tick;
            m_Timer.Start();
        }

        int m_Angle = 0;
        private void M_Timer_Tick(object? sender, EventArgs e)
        {
            var raduis = Math.PI / 180* (m_Angle-90);
            var y = Math.Sin(raduis) *100;
            var x = Math.Cos(raduis) *100;
            line.X2 = x+100;
            line.Y2 = y + 100;
            m_Angle++;
        }
    }
}