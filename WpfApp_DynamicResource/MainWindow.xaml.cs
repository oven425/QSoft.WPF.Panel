using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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

namespace WpfApp_DynamicResource
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadLanguageResources("en-US");
        }

        private void LoadLanguageResources(string languageCode)
        {
            // 移除現有的語言資源字典 (如果存在)
            ResourceDictionary existingResource = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Resources."));

            if (existingResource != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(existingResource);
            }

            // 建立新的語言資源字典
            ResourceDictionary newResource = new ResourceDictionary();
            try
            {
                newResource.Source = new Uri($"Languages/Resources.{languageCode}.xaml", UriKind.Relative);
                Application.Current.Resources.MergedDictionaries.Add(newResource);

                // 可選：設定 CurrentCulture 和 CurrentUICulture，以便日期、貨幣等格式也符合所選語言
                CultureInfo culture = new CultureInfo(languageCode);
                System.Threading.Thread.CurrentThread.CurrentCulture = culture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"載入語言資源失敗: {ex.Message}");
                // 可以 fallback 到預設語言或記錄錯誤
            }
        }

        private void ChangeLanguage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string languageCode)
            {
                LoadLanguageResources(languageCode);
            }
        }
    }

}
