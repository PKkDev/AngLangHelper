using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUITestParser.MVVM.View;
using WinUITestParser.MVVM.ViewModel;
using WinUITestParser.Services;

namespace WinUITestParser
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static Window MainWindow;
        public IHost Host { get; }
        private UIElement? _shell { get; set; }

        public static T GetService<T>() where T : class
        {
            if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
                throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");

            return service;
        }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();

            Host = Microsoft.Extensions.Hosting.Host.
                CreateDefaultBuilder().
                UseContentRoot(AppContext.BaseDirectory).
                ConfigureServices((context, services) =>
                {
                    services.AddSingleton<XmlUtilService>();
                    services.AddSingleton<NavigationHelperService>();

                    services.AddTransient<EditorView>();
                    services.AddTransient<EditorViewModel>();

                    services.AddTransient<SettingsView>();
                    services.AddTransient<SettingsViewModel>();

                    services.AddTransient<ShellView>();
                    services.AddTransient<ShellViewModel>();

                    //services.AddSingleton<NavigationHelperService>();
                })
                .Build();

            UnhandledException += App_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private async void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            ContentDialog dialog = new()
            {
                Title = "Ошибка",
                Content = e.Message,
                CloseButtonText = "Ok",
                XamlRoot = App.MainWindow.Content.XamlRoot,
            };
            await dialog.ShowAsync();
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            MainWindow = new MainWindow();


            _shell = App.GetService<ShellView>();
            MainWindow.Content = _shell ?? new Frame();

            MainWindow.Activate();
        }
    }
}
