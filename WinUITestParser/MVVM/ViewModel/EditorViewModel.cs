using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinUITestParser.MVVM.Model;
using WinUITestParser.Services;
using System;
using Microsoft.UI.Text;

namespace WinUITestParser.MVVM.ViewModel
{
    public class EditorViewModel : ObservableRecipient
    {
        private readonly XmlUtilService _xmlUtilService;

        private RichEditBox _editor1 { get; set; }
        private RichEditBox _editor2 { get; set; }

        private StorageFile OriginFile { get; set; }
        private StorageFile TranslateFile { get; set; }

        private bool _isOriginLoading;
        public bool IsOriginLoading
        {
            get => _isOriginLoading;
            set => SetProperty(ref _isOriginLoading, value);
        }
        public Dictionary<int, int> OriginLinePosDict { get; set; }
        public List<TransUnit> OriginTransUnits { get; set; }
        public ValidationErrorModel OriginValidation { get; set; }

        public RelayCommand OpenOrCmd { get; set; }

        private bool _isTranslateLoading;
        public bool IsTranslateLoading
        {
            get => _isTranslateLoading;
            set => SetProperty(ref _isTranslateLoading, value);
        }
        public Dictionary<int, int> TranslateLinePosDict { get; set; }
        public List<TransUnit> TranslateTransUnits { get; set; }
        public ValidationErrorModel TranslateValidation { get; set; }

        public RelayCommand OpenTrCmd { get; set; }

        public EditorViewModel(XmlUtilService xmlUtilService)
        {
            _xmlUtilService = xmlUtilService;

            OriginValidation = new();
            TranslateValidation = new();

            OpenOrCmd = new RelayCommand(async () => await OpenOr());
            OpenTrCmd = new RelayCommand(async () => await OpenTr());
        }

        public void Initialize(RichEditBox editor1, RichEditBox editor2)
        {
            _editor1 = editor1;
            _editor2 = editor2;
        }

        #region open file

        private async Task OpenOr()
        {
            var picker = GetOpenFilePicker();

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                IsOriginLoading = true;

                OriginFile = file;
                _editor1.Header = file.Name;

                var xml = await FileIO.ReadTextAsync(OriginFile);

                App.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    _editor1.TextDocument.SetText(TextSetOptions.None, xml);
                    OriginLinePosDict = UpdateEditorLineDict(_editor1, xml);
                });

                OriginTransUnits = _xmlUtilService.MapXmlToObject(xml);

                App.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    IsOriginLoading = false;
                });

            }
        }

        private async Task OpenTr()
        {
            var picker = GetOpenFilePicker();

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                IsTranslateLoading = true;

                TranslateFile = file;
                _editor2.Header = file.Name;

                var xml = await FileIO.ReadTextAsync(TranslateFile);

                App.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    _editor2.TextDocument.SetText(TextSetOptions.None, xml);
                    TranslateLinePosDict = UpdateEditorLineDict(_editor2, xml);
                });

                TranslateTransUnits = _xmlUtilService.MapXmlToObject(xml);

                App.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    IsTranslateLoading = false;
                });
            }
        }

        private FileOpenPicker GetOpenFilePicker()
        {
            FileOpenPicker picker = new();

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".xlf");

            return picker;
        }

        #endregion open file

        private Dictionary<int, int> UpdateEditorLineDict(RichEditBox editor, string xml)
        {
            Dictionary<int, int> dict = new();

            int size = 0;
            while (xml.Length > size)
            {
                ITextRange range = editor.Document.GetRange(size, size + 1);
                size += range.Expand(TextRangeUnit.Line);
                var lineIndex = range.GetIndex(TextRangeUnit.Line);
                dict[lineIndex] = range.StartPosition;
                size += 1;
            }

            return dict;
        }
    }
}
