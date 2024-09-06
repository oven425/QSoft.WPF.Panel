using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
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
        public static readonly DependencyProperty ExpenderStyleProperty;
        public static readonly DependencyProperty ViewProperty;
        static TreeListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListView), new FrameworkPropertyMetadata(typeof(TreeListView)));
            ViewProperty = DependencyProperty.Register("View", typeof(ViewBase), typeof(TreeListView));
            ExpenderStyleProperty = DependencyProperty.Register("ExpenderStyle", typeof(Style), typeof(TreeListView));
            
        }

        public ViewBase View
        {
            set {  SetValue(ViewProperty, value); }
            get {  return (ViewBase)GetValue(ViewProperty);}
        }

        public Style ExpenderStyle
        {
            set { SetValue(ExpenderStyleProperty, value); }
            get { return (Style)GetValue(ExpenderStyleProperty); }
        }

        public override void OnApplyTemplate()
        {
            var str = @"<ResourceDictionary xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    <Style x:Key=""ExpandCollapseToggleStyle"" TargetType=""ToggleButton"">
        <Setter Property=""Focusable""  Value=""False""/>
        <Setter Property=""Width""      Value=""19""/>
        <Setter Property=""Height""     Value=""13""/>
        <Setter Property=""Template"">
            <Setter.Value>
                <ControlTemplate TargetType=""{x:Type ToggleButton}"">
                    <Border Background=""Transparent"" Height=""16"" Padding=""5,5,5,5"" Width=""16"">
                        <Path x:Name=""ExpandPath""  Fill=""#FFFFFFFF"" Stroke=""#FF818181"">
                            <Path.Data>
                                <PathGeometry Figures=""M0,0 L0,6 L6,0 z""/>
                            </Path.Data>
                            <Path.RenderTransform>
                                <RotateTransform Angle=""135"" CenterX=""3"" CenterY=""3""/>
                            </Path.RenderTransform>
                        </Path>
                    </Border>
                    <ControlTemplate.Triggers>
                        <Trigger Property=""IsChecked"" Value=""True"">
                            <Setter Property=""RenderTransform"" TargetName=""ExpandPath"">
                                <Setter.Value>
                                    <RotateTransform Angle=""180"" CenterX=""3"" CenterY=""3""/>
                                </Setter.Value>
                            </Setter>
                            <Setter Property=""Fill"" TargetName=""ExpandPath"" Value=""#FF595959""/>
                            <Setter Property=""Stroke"" TargetName=""ExpandPath"" Value=""#FF262626""/>
                        </Trigger>
                        <Trigger Property=""IsMouseOver"" Value=""True"">
                            <Setter Property=""Stroke"" TargetName=""ExpandPath"" Value=""#FF27C7F7""/>
                            <Setter Property=""Fill"" TargetName=""ExpandPath"" Value=""#FFCCEEFB""/>
                        </Trigger>
                        <MultiTrigger>
                            <MultiTrigger.Conditions>
                                <Condition Property=""IsMouseOver"" Value=""True""/>
                                <Condition Property=""IsChecked"" Value=""True""/>
                            </MultiTrigger.Conditions>
                            <Setter Property=""Stroke"" TargetName=""ExpandPath"" Value=""#FF1CC4F7""/>
                            <Setter Property=""Fill"" TargetName=""ExpandPath"" Value=""#FF82DFFB""/>
                        </MultiTrigger>
                    </ControlTemplate.Triggers>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>

</ResourceDictionary>";
            var obj = XamlReader.Parse(str) as ResourceDictionary;
            var aaaa = obj["ExpandCollapseToggleStyle"] as Style;

            if(this.View is GridView gridview && 
                this.GetTemplateChild("header") is GridViewHeaderRowPresenter hp)
            {
                DataTemplate template = new DataTemplate();
                
                FrameworkElementFactory factory = new FrameworkElementFactory(typeof(DockPanel));
                template.VisualTree = factory;
                FrameworkElementFactory togglebuttonFactory = new FrameworkElementFactory(typeof(ToggleButton));
                togglebuttonFactory.SetValue(ToggleButton.StyleProperty, aaaa);
                factory.AppendChild(togglebuttonFactory);

                togglebuttonFactory.Name = "Expander";
                template.RegisterName(togglebuttonFactory.Name, togglebuttonFactory);

                togglebuttonFactory.SetBinding(ToggleButton.IsCheckedProperty, new Binding()
                {
                    Path = new PropertyPath("IsExpanded",null),
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(TreeListViewItem), 1)
                });
                togglebuttonFactory.SetBinding(ToggleButton.MarginProperty, new Binding()
                {
                    Path = new PropertyPath("Level",null),
                    Converter = new LevelToIndentConverter(),
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(TreeListViewItem), 1)
                });


                var contentcontrolFactory = new FrameworkElementFactory(typeof(ContentControl));
                if(this.ItemTemplate is HierarchicalDataTemplate hd && hd.ItemTemplateSelector !=null)
                {
                    contentcontrolFactory.SetBinding(ContentControl.ContentProperty, new Binding());
                    contentcontrolFactory.SetValue(ContentControl.ContentTemplateSelectorProperty, hd.ItemTemplateSelector);

                }
                else if (gridview.Columns[0].DisplayMemberBinding != null)
                {
                    contentcontrolFactory.SetBinding(ContentControl.ContentProperty, gridview.Columns[0].DisplayMemberBinding);
                }
                else if (gridview.Columns[0].CellTemplate != null)
                {
                    contentcontrolFactory.SetBinding(ContentControl.ContentProperty, new Binding());
                    contentcontrolFactory.SetValue(ContentControl.ContentTemplateProperty, gridview.Columns[0].CellTemplate);
                }
                else if (gridview.Columns[0].CellTemplateSelector != null)
                {
                    contentcontrolFactory.SetBinding(ContentControl.ContentProperty, new Binding());
                    contentcontrolFactory.SetValue(ContentControl.ContentTemplateSelectorProperty, gridview.Columns[0].CellTemplateSelector);

                }
                factory.AppendChild(contentcontrolFactory);


                var datatrigger = new DataTrigger()
                {
                    Binding = new Binding()
                    {
                        Path = new PropertyPath("HasItems", null),
                        RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(TreeListViewItem), 1)
                    },
                    Value = false,
                    Setters =
                    {
                        new Setter()
                        {
                            TargetName = "Expander",
                            Property = ToggleButton.VisibilityProperty,
                            Value = Visibility.Hidden
                        }
                    }
                };

                template.Triggers.Add(datatrigger);
                var xw = XamlWriter.Save(template);
                //xw = xw.Replace(" Name=", " x:Name=");
                var rr = (DataTemplate)XamlReader.Parse(xw);
                hp.Columns = gridview.Columns;
                hp.Columns[0].DisplayMemberBinding = null;
                //hp.Columns[0].CellTemplate = FindResource("CellTemplate_Name11") as DataTemplate;
                hp.Columns[0].CellTemplate = template;
            }
            base.OnApplyTemplate();
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var item = new TreeListViewItem();
            if (this.View is GridView gridview)
            {
                item.ColumnCollection = gridview.Columns;
            }
            return item;
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            var aa = base.IsItemItsOwnContainerOverride(item);
            bool _isTreeLVI = item is TreeListViewItem;
            return _isTreeLVI;
        }
    }

}
