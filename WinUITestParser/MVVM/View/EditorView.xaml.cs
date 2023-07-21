using Microsoft.UI.Xaml.Controls;
using WinUITestParser.MVVM.ViewModel;

namespace WinUITestParser.MVVM.View
{
    public sealed partial class EditorView : Page
    {
        public EditorViewModel ViewModel { get; set; }

        public EditorView()
        {
            DataContext = ViewModel = App.GetService<EditorViewModel>();

            InitializeComponent();
        }
    }
}
