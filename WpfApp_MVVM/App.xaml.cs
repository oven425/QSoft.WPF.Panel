using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp_MVVM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        IHost? m_Host;
        public App()
        {
            var args = Environment.GetCommandLineArgs();
            var builder = Host.CreateApplicationBuilder(args);
            var bb = builder.Configuration["aa"];
            //builder.Configuration.AddCommandLine(args);
            builder.Services.AddTransient<ParameterVM_New<People>, ParameterVM_New_People>();
            builder.Services.AddTransient<ParameterVM_Modify<People>, ParameterVM_Modify_People>();
            builder.Services.Configure<AppArgs>(config => new AppArgs()
            {
                aa = builder.Configuration["aa"] ?? ""
            });

            m_Host = builder.Build();
            Ioc.Default.ConfigureServices(m_Host.Services);

            var appargs = m_Host.Services.GetService<IOptions<AppArgs>>();
        }
    }

    public class AppArgs
    {
        public string aa { set; get; }
    }


    public abstract class BasePage<TViewModel> : BasePage where TViewModel : ObservableObject
    {
        protected BasePage(TViewModel viewModel) : base(viewModel)
        {
        }

        public new TViewModel DataContext => (TViewModel)base.DataContext;
    }

    public abstract class BasePage : UserControl
    {
        protected BasePage(object? viewModel = null)
        {
            DataContext = viewModel;

        }
    }
}
