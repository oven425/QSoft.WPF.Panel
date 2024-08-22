using System;
using System.Collections.Generic;
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

namespace QSoft.WPF.TreeListView
{
    /// <summary>
    /// 依照步驟 1a 或 1b 執行，然後執行步驟 2，以便在 XAML 檔中使用此自訂控制項。
    ///
    /// 步驟 1a) 於存在目前專案的 XAML 檔中使用此自訂控制項。
    /// 加入此 XmlNamespace 屬性至標記檔案的根項目為 
    ///要使用的: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:QSoft.WPF.TreeListView"
    ///
    ///
    /// 步驟 1b) 於存在其他專案的 XAML 檔中使用此自訂控制項。
    /// 加入此 XmlNamespace 屬性至標記檔案的根項目為 
    ///要使用的: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:QSoft.WPF.TreeListView;assembly=QSoft.WPF.TreeListView"
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
    ///     <MyNamespace:TreeListView/>
    ///
    /// </summary>
    public class TreeListView : TreeView
    {
        public static readonly DependencyProperty ViewProperty;
        static TreeListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListView), new FrameworkPropertyMetadata(typeof(TreeListView)));
            ViewProperty = DependencyProperty.Register("View", typeof(ViewBase), typeof(TreeListView));
        }

        public ViewBase View
        {
            set {  SetValue(ViewProperty, value); }
            get {  return (ViewBase)GetValue(ViewProperty);}
        }

        public override void OnApplyTemplate()
        {
            var hp = this.GetTemplateChild("header") as GridViewHeaderRowPresenter;
            hp.Columns = new GridViewColumnCollection();
            var gridview = this.View as GridView;
            hp.Columns = gridview.Columns;

            hp.Columns[0].CellTemplate = FindResource("CellTemplate_Name") as DataTemplate;
            base.OnApplyTemplate();
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var bb = base.GetContainerForItemOverride();
            return new TreeListViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            var aa = base.IsItemItsOwnContainerOverride(item);
            bool _isTreeLVI = item is TreeListViewItem;
            return _isTreeLVI;
        }
    }
}
