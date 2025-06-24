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

namespace QSoft.WPF.ButtonGroup
{
    /// <summary>
    /// 依照步驟 1a 或 1b 執行，然後執行步驟 2，以便在 XAML 檔中使用此自訂控制項。
    ///
    /// 步驟 1a) 於存在目前專案的 XAML 檔中使用此自訂控制項。
    /// 加入此 XmlNamespace 屬性至標記檔案的根項目為 
    ///要使用的: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:QSoft.WPF.ButtonGroup"
    ///
    ///
    /// 步驟 1b) 於存在其他專案的 XAML 檔中使用此自訂控制項。
    /// 加入此 XmlNamespace 屬性至標記檔案的根項目為 
    ///要使用的: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:QSoft.WPF.ButtonGroup;assembly=QSoft.WPF.ButtonGroup"
    ///
    /// 您還必須將 XAML 檔所在專案的專案參考加入
    /// 此專案並重建，以免發生編譯錯誤: 
    ///
    ///     在 [方案總管] 中以滑鼠右鍵按一下目標專案，並按一下
    ///     [加入參考]->[專案]->[瀏覽並選取此專案]
    ///
    ///
    /// 步驟 2)
    /// 開始使用 XAML 檔案中的控制項。
    ///
    ///     <MyNamespace:ButtonGroup/>
    ///
    /// </summary>
    public class ButtonGroup : ItemsControl
    {
        readonly public static DependencyProperty FirstButtonStyleProperty =
            DependencyProperty.Register("FistButtonBaseStyle", typeof(Style), typeof(ButtonGroup), new PropertyMetadata(null));
        readonly public static DependencyProperty LastButtonStyleProperty =
            DependencyProperty.Register("LastButtonBaseStyle", typeof(Style), typeof(ButtonGroup), new PropertyMetadata(null));
        readonly public static DependencyProperty OtherButtonStyleProperty =
            DependencyProperty.Register("OtherButtonBaseStyle", typeof(Style), typeof(ButtonGroup), new PropertyMetadata(null));
        readonly public static DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ButtonGroup));
        [Category("ButtonGroup")]
        public Style FirstButtonBaseStyle
        {
            get { return (Style)GetValue(FirstButtonStyleProperty); }
            set { SetValue(FirstButtonStyleProperty, value); }
        }
        [Category("ButtonGroup")]
        public Style LastButtonBaseStyle
        {
            get { return (Style)GetValue(LastButtonStyleProperty); }
            set { SetValue(LastButtonStyleProperty, value); }
        }
        [Category("ButtonGroup")]
        public Style OtherButtonBaseStyle
        {
            get { return (Style)GetValue(OtherButtonStyleProperty); }
            set { SetValue(OtherButtonStyleProperty, value); }
        }
        [Category("ButtonGroup")]
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
        static ButtonGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonGroup), new FrameworkPropertyMetadata(typeof(ButtonGroup)));
        }

        public override void OnApplyTemplate()
        {
            
            //this.Items.CurrentChanged += Items_CurrentChanged;
            //if(this.Items[0] is Button button)
            //{
            //    button.Style = this.FirstButtonBaseStyle;
            //}
            //if (this.Items[this.Items.Count - 1] is Button lastButton)
            //{
            //    lastButton.Style = this.LastButtonBaseStyle;
            //}
            //for (int i = 1; i < this.Items.Count - 1; i++)
            //{
            //    if(i==0)
            //    {

            //    }
            //    else
            //    {
            //        //otherButton.Style = this.OtherButtonBaseStyle;
            //    }
                
            //}
            base.OnApplyTemplate();
        }

        private void Items_CurrentChanged(object? sender, EventArgs e)
        {

        }
    }

}
