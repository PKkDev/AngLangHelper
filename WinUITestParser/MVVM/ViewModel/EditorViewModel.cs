using CommunityToolkit.Mvvm.ComponentModel;
using WinUITestParser.Services;

namespace WinUITestParser.MVVM.ViewModel
{
    public class EditorViewModel : ObservableRecipient
    {
        private readonly XmlUtilService _xmlUtilService;

        public EditorViewModel(XmlUtilService xmlUtilService)
        {
            _xmlUtilService = xmlUtilService;
        }
    }
}
