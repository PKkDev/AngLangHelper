using Microsoft.UI.Xaml.Controls;
using WinUITestParser.MVVM.ViewModel;

namespace WinUITestParser.MVVM.View
{
    public sealed partial class SettingsView : Page
    {
        public SettingsViewModel ViewModel { get; set; }

        public SettingsView()
        {
            InitializeComponent();
            DataContext = ViewModel = App.GetService<SettingsViewModel>();
        }
    }
}
