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
using System.Linq;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.VisualBasic;

namespace WinUITestParser.MVVM.ViewModel
{
    public class EditorViewModel : ObservableRecipient
    {
        private readonly XmlUtilService _xmlUtilService;

        private RichEditBox _editor1 { get; set; }
        private RichEditBox _editor2 { get; set; }

        private ITextCharacterFormat baseFormat { get; set; }
        private ITextCharacterFormat errFormat { get; set; }
        private ITextCharacterFormat newFormat { get; set; }
        private ITextCharacterFormat notFoundFormat { get; set; }

        private StorageFile OriginFile { get; set; }
        private StorageFile TranslateFile { get; set; }

        private bool _isOriginLoading;
        public bool IsOriginLoading { get => _isOriginLoading; set => SetProperty(ref _isOriginLoading, value); }
        public Dictionary<int, int> OriginLinePosDict { get; set; }
        public List<TransUnit> OriginTransUnits { get; set; }
        private ValidationErrorModel _originValidation;
        public ValidationErrorModel OriginValidation { get => _originValidation; set => SetProperty(ref _originValidation, value); }
        public SubPanel OriginSubPanel { get; set; }

        public AsyncRelayCommand OpenOrCmd { get; set; }
        public AsyncRelayCommand SaveOrCmd { get; set; }
        public RelayCommand FormatOrCmd { get; set; }
        public RelayCommand ValidateOrCmd { get; set; }
        public RelayCommand AnalyzOrCmd { get; set; }
        public RelayCommand CheckMatchesCmd { get; set; }
        public RelayCommand CreateTranslateCmd { get; set; }
        public RelayCommand SubPanelOrCloseCmd { get; set; }
        public RelayCommand<ValidationError> ViewErrorOrCmd { get; set; }

        private bool _isTranslateLoading;
        public bool IsTranslateLoading { get => _isTranslateLoading; set => SetProperty(ref _isTranslateLoading, value); }
        public Dictionary<int, int> TranslateLinePosDict { get; set; }
        public List<TransUnit> TranslateTransUnits { get; set; }
        private ValidationErrorModel _translateValidation;
        public ValidationErrorModel TranslateValidation { get => _translateValidation; set => SetProperty(ref _translateValidation, value); }
        public SubPanel TranslateSubPanel { get; set; }

        public AsyncRelayCommand OpenTrCmd { get; set; }
        public AsyncRelayCommand SaveTrCmd { get; set; }
        public RelayCommand FormatTrCmd { get; set; }
        public RelayCommand ValidateTrCmd { get; set; }
        public RelayCommand AnalyzTrCmd { get; set; }
        public RelayCommand UpdateTrCmd { get; set; }
        public RelayCommand SubPanelTrCloseCmd { get; set; }
        public RelayCommand<ValidationError> ViewErrorTrCmd { get; set; }

        public EditorViewModel(XmlUtilService xmlUtilService)
        {
            _xmlUtilService = xmlUtilService;

            OriginSubPanel = new();
            TranslateSubPanel = new();

            OriginValidation = new();
            TranslateValidation = new();

            OpenOrCmd = new AsyncRelayCommand(OpenOr);
            SaveOrCmd = new AsyncRelayCommand(SaveOr);
            FormatOrCmd = new RelayCommand(async () => await FormatOr());
            ValidateOrCmd = new RelayCommand(async () => await ValidateOr());
            AnalyzOrCmd = new RelayCommand(async () => await AnalyzOr());
            CheckMatchesCmd = new RelayCommand(async () => await CheckMatches());
            CreateTranslateCmd = new RelayCommand(async () => await CreateTranslate());
            SubPanelOrCloseCmd = new RelayCommand(() => OriginSubPanel.ChangeState());
            ViewErrorOrCmd = new RelayCommand<ValidationError>((ValidationError error) => ViewErrorOr(error));

            OpenTrCmd = new AsyncRelayCommand(OpenTr);
            SaveTrCmd = new AsyncRelayCommand(SaveTr);
            FormatTrCmd = new RelayCommand(async () => await FormatTr());
            ValidateTrCmd = new RelayCommand(async () => await ValidateTr());
            AnalyzTrCmd = new RelayCommand(async () => await AnalyzTr());
            UpdateTrCmd = new RelayCommand(async () => await UpdateTr());
            SubPanelTrCloseCmd = new RelayCommand(() => TranslateSubPanel.ChangeState());
            ViewErrorTrCmd = new RelayCommand<ValidationError>((ValidationError error) => ViewErrorTr(error));
        }

        public void Initialize(RichEditBox editor1, RichEditBox editor2)
        {
            _editor1 = editor1;
            _editor2 = editor2;

            var editorFormat = _editor1.Document.GetDefaultCharacterFormat();

            baseFormat = editorFormat.GetClone();

            errFormat = editorFormat.GetClone();
            errFormat.BackgroundColor = ColorHelper.ToColor("#ff4d4f");
            errFormat.ForegroundColor = ColorHelper.ToColor("#ffffff");

            newFormat = editorFormat.GetClone();
            newFormat.ForegroundColor = ColorHelper.ToColor("#52c41a");

            notFoundFormat = editorFormat.GetClone();
            notFoundFormat.ForegroundColor = ColorHelper.ToColor("#ff4d4f");
        }

        public async Task CheckMatches()
        {
            try
            {
                _editor1.TextDocument.GetText(TextGetOptions.None, out var xmlOr);
                OriginLinePosDict = UpdateEditorLineDict(_editor1, xmlOr);
                OriginTransUnits = _xmlUtilService.MapXmlToObject(xmlOr);

                _editor2.TextDocument.GetText(TextGetOptions.None, out var xmlTr);
                TranslateLinePosDict = UpdateEditorLineDict(_editor2, xmlTr);
                TranslateTransUnits = _xmlUtilService.MapXmlToObject(xmlTr);

                foreach (var item in OriginTransUnits)
                {
                    var search = TranslateTransUnits.FirstOrDefault(x => x.Id.Equals(item.Id));
                    if (search == null)
                    {
                        item.IsNew = true;
                        item.IsSomeDifferent = true;
                    }
                    else
                    {
                        if (item.Notes.Count > 0 && search.Notes.Count == 0)
                        {
                            item.Notes.ForEach(x => x.IsNew = true);
                        }
                        else
                        {
                            if (item.Notes.Count == 0 && search.Notes.Count > 0)
                                search.Notes.ForEach(x => x.IsNotNow = true);
                            else
                            {
                                for (int i = 0; i < item.Notes.Count; i++)
                                {
                                    var or = item.Notes.ElementAtOrDefault(i);
                                    var tr = search.Notes.ElementAtOrDefault(i) ?? null;

                                    if (tr == null)
                                    {
                                        or.IsNew = true;
                                        item.IsSomeDifferent = true;
                                    }
                                    else
                                    {
                                        if (tr.Priority != or.Priority || tr.From != or.From || tr.Text != or.Text)
                                        {
                                            or.IsNew = true;
                                            tr.IsNotNow = true;
                                            item.IsSomeDifferent = true;
                                            search.IsSomeDifferent = true;
                                        }
                                    }
                                }
                            }
                        }

                        if (item.ContextGroups.Count > 0 && search.ContextGroups.Count == 0)
                        {
                            item.ContextGroups.ForEach(x => x.IsNew = true);
                        }
                        else
                        {
                            if (item.ContextGroups.Count == 0 && search.ContextGroups.Count > 0)
                                search.ContextGroups.ForEach(x => x.IsNotNow = true);
                            else
                            {
                                for (int i = 0; i < item.ContextGroups.Count; i++)
                                {
                                    var or = item.ContextGroups.ElementAtOrDefault(i);
                                    var tr = search.ContextGroups.ElementAtOrDefault(i) ?? null;

                                    if (tr == null)
                                    {
                                        or.IsNew = true;
                                        item.IsSomeDifferent = true;
                                    }
                                    else
                                    {
                                        if (tr.Purpose == or.Purpose)
                                        {
                                            if (tr.Contexts.Count != or.Contexts.Count)
                                            {
                                                or.IsNew = true;
                                                tr.IsNotNow = true;
                                                item.IsSomeDifferent = true;
                                                search.IsSomeDifferent = true;
                                            }
                                            else
                                            {
                                                for (int j = 0; j < or.Contexts.Count; j++)
                                                {
                                                    var orC = or.Contexts.ElementAtOrDefault(j);
                                                    var trC = tr.Contexts.ElementAtOrDefault(j) ?? null;

                                                    if (trC == null)
                                                    {
                                                        or.IsNew = true;
                                                        tr.IsNotNow = true;
                                                        item.IsSomeDifferent = true;
                                                        search.IsSomeDifferent = true;
                                                    }
                                                    else
                                                    {
                                                        if (orC.ContextType != trC.ContextType || orC.Text != trC.Text)
                                                        {
                                                            or.IsNew = true;
                                                            tr.IsNotNow = true;
                                                            item.IsSomeDifferent = true;
                                                            search.IsSomeDifferent = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            or.IsNew = true;
                                            tr.IsNotNow = true;
                                            item.IsSomeDifferent = true;
                                            search.IsSomeDifferent = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (var item in OriginTransUnits)
                {
                    if (item.IsNew)
                    {
                        var pos = OriginLinePosDict[item.GlobalLineNumber];
                        ITextRange? t = _editor1.Document.GetRange(pos, pos + 1);
                        t.Expand(TextRangeUnit.Line);
                        for (int i = 0; i < item.LineCount; i++)
                        {
                            t.EndPosition += 1;
                            t.Expand(TextRangeUnit.Line);
                        }
                        t.CharacterFormat = newFormat;
                    }
                    else
                    {
                        foreach (var itemNote in item.Notes)
                        {
                            if (itemNote.IsNew)
                            {
                                var pos = OriginLinePosDict[itemNote.GlobalLineNumber];
                                ITextRange? t = _editor1.Document.GetRange(pos, pos + 1);
                                t.Expand(TextRangeUnit.Line);
                                t.CharacterFormat = newFormat;
                            }
                        }

                        foreach (var itemCG in item.ContextGroups)
                        {
                            if (itemCG.IsNew)
                            {
                                var pos = OriginLinePosDict[itemCG.GlobalLineNumber];
                                ITextRange? t = _editor1.Document.GetRange(pos, pos + 1);
                                t.Expand(TextRangeUnit.Line);
                                for (int i = 0; i < itemCG.LineCount; i++)
                                {
                                    t.EndPosition += 1;
                                    t.Expand(TextRangeUnit.Line);
                                }
                                t.CharacterFormat = newFormat;
                            }
                        }
                    }
                }

                foreach (var item in TranslateTransUnits)
                {
                    if (item.IsNotNow)
                    {
                        var pos = TranslateLinePosDict[item.GlobalLineNumber];
                        ITextRange? t = _editor2.Document.GetRange(pos, pos + 1);
                        t.Expand(TextRangeUnit.Line);
                        for (int i = 0; i < item.LineCount; i++)
                        {
                            t.EndPosition += 1;
                            t.Expand(TextRangeUnit.Line);
                        }
                        t.CharacterFormat = notFoundFormat;
                    }
                    else
                    {
                        foreach (var itemNote in item.Notes)
                        {
                            if (itemNote.IsNotNow)
                            {
                                var pos = TranslateLinePosDict[itemNote.GlobalLineNumber];
                                ITextRange? t = _editor2.Document.GetRange(pos, pos + 1);
                                t.Expand(TextRangeUnit.Line);
                                t.CharacterFormat = notFoundFormat;
                            }
                        }

                        foreach (var itemCG in item.ContextGroups)
                        {
                            if (itemCG.IsNotNow)
                            {
                                var pos = TranslateLinePosDict[itemCG.GlobalLineNumber];
                                ITextRange? t = _editor2.Document.GetRange(pos, pos + 1);
                                t.Expand(TextRangeUnit.Line);
                                for (int i = 0; i < itemCG.LineCount; i++)
                                {
                                    t.EndPosition += 1;
                                    t.Expand(TextRangeUnit.Line);
                                }
                                t.CharacterFormat = notFoundFormat;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public async Task CreateTranslate()
        {
            var xml = _xmlUtilService.MapObjectToXml(OriginTransUnits);

            _editor2.Document.SetText(TextSetOptions.None, xml);
            TranslateTransUnits = _xmlUtilService.MapXmlToObject(xml);
        }

        public async Task UpdateTr()
        {
            var updatedModel = _xmlUtilService.UpdateModel(OriginTransUnits, TranslateTransUnits);

            var xml = _xmlUtilService.MapObjectToXml(updatedModel);

            _editor2.Document.SetText(TextSetOptions.None, xml);
        }

        #region Validate

        private async Task ValidateOr()
        {
            IsOriginLoading = true;

            _editor1.TextDocument.GetText(TextGetOptions.None, out var xml);
            OriginLinePosDict = UpdateEditorLineDict(_editor1, xml);
            OriginValidation = _xmlUtilService.ValidateXml(xml);

            if (OriginValidation.Errors.Any())
            {
                OriginSubPanel.Open();

                foreach (var item in OriginValidation.Errors)
                {
                    var line = OriginLinePosDict[item.LineNumber];

                    var stPos = line + item.LinePosition;
                    var newRange = _editor1.Document.GetRange(stPos, stPos);
                    newRange.Expand(TextRangeUnit.Line);

                    newRange.CharacterFormat = errFormat;
                }
            }
            else
            {
                OriginSubPanel.Close();

                _editor1.Document.GetRange(0, xml.Length).CharacterFormat = baseFormat;
            }

            IsOriginLoading = false;
        }

        private async Task ValidateTr()
        {
            _editor2.TextDocument.GetText(TextGetOptions.None, out var xml);
            TranslateLinePosDict = UpdateEditorLineDict(_editor2, xml);
            TranslateValidation = _xmlUtilService.ValidateXml(xml);

            var noTargetList = TranslateTransUnits
                .Where(x => string.IsNullOrEmpty(x.Target))
                .Select(x => new ValidationError(x.GlobalLineNumber, 0, "Target is empty", ValidationErrorType.Warning));
            TranslateValidation.SetNoTarget(noTargetList);

            if (TranslateValidation.Errors.Any())
            {
                TranslateSubPanel.Open();

                foreach (var item in TranslateValidation.Errors)
                {
                    var line = TranslateLinePosDict[item.LineNumber];

                    var stPos = line + item.LinePosition;
                    var newRange = _editor2.Document.GetRange(stPos, stPos);
                    newRange.Expand(TextRangeUnit.Line);

                    newRange.CharacterFormat = errFormat;
                }
            }
            else
            {
                TranslateSubPanel.Close();

                _editor2.Document.GetRange(0, xml.Length).CharacterFormat = baseFormat;
            }
        }

        private void ViewErrorOr(ValidationError error)
        {
            var line = OriginLinePosDict[error.LineNumber];
            var newRange = _editor1.Document.GetRange(line, line + 1);
            newRange.Expand(TextRangeUnit.Line);
            newRange.ScrollIntoView(PointOptions.Start);
        }

        private void ViewErrorTr(ValidationError error)
        {
            var line = TranslateLinePosDict[error.LineNumber];
            var newRange = _editor2.Document.GetRange(line, line + 1);
            newRange.Expand(TextRangeUnit.Line);
            newRange.ScrollIntoView(PointOptions.Start);
        }

        #endregion Validate

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

                IsOriginLoading = false;
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

                IsTranslateLoading = false;
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

        #region save file

        private async Task SaveOr()
        {
            IsOriginLoading = true;

            _editor1.TextDocument.GetText(TextGetOptions.None, out var xml);
            await FileIO.WriteTextAsync(OriginFile, xml);

            OriginTransUnits = _xmlUtilService.MapXmlToObject(xml);

            IsOriginLoading = false;
        }

        private async Task SaveTr()
        {
            if (TranslateFile == null)
            {
                FileSavePicker picker = new();

                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

                picker.FileTypeChoices.Add("Localixation File", new List<string>() { ".xlf" });
                picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                picker.SuggestedFileName = $"{OriginFile.DisplayName}.lang";

                StorageFile file = await picker.PickSaveFileAsync();
                if (file != null)
                {
                    TranslateFile = file;
                    _editor2.Header = file.Name;

                    await SaveTranslateFile();
                }
            }
            else
            {
                await SaveTranslateFile();
            }
        }

        private async Task SaveTranslateFile()
        {
            IsTranslateLoading = true;

            _editor2.TextDocument.GetText(TextGetOptions.None, out var xml);
            await FileIO.WriteTextAsync(TranslateFile, xml);

            TranslateTransUnits = _xmlUtilService.MapXmlToObject(xml);

            IsTranslateLoading = false;
        }

        #endregion save file

        #region format text

        private async Task FormatOr()
        {
            _editor1.TextDocument.GetText(TextGetOptions.None, out var xml);
            var formatedXml = _xmlUtilService.FormatXml(xml);
            _editor1.TextDocument.SetText(TextSetOptions.None, formatedXml);
        }

        private async Task FormatTr()
        {
            _editor2.TextDocument.GetText(TextGetOptions.None, out var xml);
            var formatedXml = _xmlUtilService.FormatXml(xml);
            _editor2.TextDocument.SetText(TextSetOptions.None, formatedXml);
        }

        #endregion format text

        #region Analyz

        private async Task AnalyzOr()
        {
            _editor1.TextDocument.GetText(TextGetOptions.None, out var xml);
            OriginLinePosDict = UpdateEditorLineDict(_editor1, xml);
            OriginTransUnits = _xmlUtilService.MapXmlToObject(xml);
        }

        private async Task AnalyzTr()
        {
            _editor2.TextDocument.GetText(TextGetOptions.None, out var xml);
            TranslateLinePosDict = UpdateEditorLineDict(_editor2, xml);
            TranslateTransUnits = _xmlUtilService.MapXmlToObject(xml);
        }

        #endregion Analyz

        private Dictionary<int, int> UpdateEditorLineDict(RichEditBox editor)
        {
            Dictionary<int, int> dict = new();

            editor.TextDocument.GetText(TextGetOptions.None, out var xml);

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
