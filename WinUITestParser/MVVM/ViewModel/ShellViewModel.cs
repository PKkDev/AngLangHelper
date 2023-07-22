using CommunityToolkit.Mvvm.ComponentModel;
using WinUITestParser.Services;

namespace WinUITestParser.MVVM.ViewModel
{
    public class ShellViewModel : ObservableRecipient
    {
        public NavigationHelperService NavigationHelperService { get; init; }

        public ShellViewModel(NavigationHelperService navigationHelperService)
        {
            NavigationHelperService = navigationHelperService;
        }
    }
}
