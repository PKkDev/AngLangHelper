using Microsoft.UI.Xaml.Controls;
using WinUITestParser.MVVM.ViewModel;

namespace WinUITestParser.MVVM.View
{
    public sealed partial class ShellView : Page
    {
        public ShellViewModel ViewModel { get; set; }

        public ShellView()
        {
            DataContext = ViewModel = App.GetService<ShellViewModel>();

            InitializeComponent();
        }
    }
}
