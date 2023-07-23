using Windows.Storage;
using Windows.Storage.Pickers;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using System.ComponentModel;
using System.Runtime.CompilerServices;

using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Text;

using WinUITestParser.MVVM.ViewModel;
using WinUITestParser.MVVM.Model;

namespace WinUITestParser.MVVM.View
{
    public sealed partial class EditorView : Page
    {
        public EditorViewModel ViewModel { get; set; }

        //private StorageFile OriginFile { get; set; }
        //private StorageFile TranslateFile { get; set; }

        //private bool _isOriginLoading;
        //public bool IsOriginLoading
        //{
        //    get => _isOriginLoading;
        //    set
        //    {
        //        _isOriginLoading = value;
        //        //OnPropertyChanged("IsOriginLoading");
        //    }
        //}
        //public Dictionary<int, int> OriginLinePosDict { get; set; }
        //public List<TransUnit> OriginTransUnits { get; set; }
        //public ValidationErrorModel OriginValidation { get; set; }

        //private bool _isTranslateLoading;
        //public bool IsTranslateLoading
        //{
        //    get => _isTranslateLoading;
        //    set
        //    {
        //        _isTranslateLoading = value;
        //        //OnPropertyChanged("IsTranslateLoading");
        //    }
        //}
        //public Dictionary<int, int> TranslateLinePosDict { get; set; }
        //public List<TransUnit> TranslateTransUnits { get; set; }
        //public ValidationErrorModel TranslateValidation { get; set; }

        public EditorView()
        {
            DataContext = ViewModel = App.GetService<EditorViewModel>();

            //OriginValidation = new();
            //TranslateValidation = new();

            InitializeComponent();
        }

        //public void CheckMatchesBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        editor1.TextDocument.GetText(TextGetOptions.None, out var xmlOr);
        //        OriginLinePosDict = UpdateEditorLineDict(editor1, xmlOr);
        //        OriginTransUnits = XmlUtils.MapXmlToObject(xmlOr);

        //        editor2.TextDocument.GetText(TextGetOptions.None, out var xmlTr);
        //        TranslateLinePosDict = UpdateEditorLineDict(editor2, xmlTr);
        //        TranslateTransUnits = XmlUtils.MapXmlToObject(xmlTr);

        //        foreach (var item in OriginTransUnits)
        //        {
        //            var search = TranslateTransUnits.FirstOrDefault(x => x.Id.Equals(item.Id));
        //            if (search == null)
        //            {
        //                item.IsNew = true;
        //                item.IsSomeDifferent = true;
        //            }
        //            else
        //            {
        //                if (item.Notes.Count > 0 && search.Notes.Count == 0)
        //                {
        //                    item.Notes.ForEach(x => x.IsNew = true);
        //                }
        //                else
        //                {
        //                    if (item.Notes.Count == 0 && search.Notes.Count > 0)
        //                        search.Notes.ForEach(x => x.IsNotNow = true);
        //                    else
        //                    {
        //                        for (int i = 0; i < item.Notes.Count; i++)
        //                        {
        //                            var or = item.Notes.ElementAtOrDefault(i);
        //                            var tr = search.Notes.ElementAtOrDefault(i) ?? null;

        //                            if (tr == null)
        //                            {
        //                                or.IsNew = true;
        //                                item.IsSomeDifferent = true;
        //                            }
        //                            else
        //                            {
        //                                if (tr.Priority != or.Priority || tr.From != or.From || tr.Text != or.Text)
        //                                {
        //                                    or.IsNew = true;
        //                                    tr.IsNotNow = true;
        //                                    item.IsSomeDifferent = true;
        //                                    search.IsSomeDifferent = true;
        //                                }
        //                            }
        //                        }
        //                    }
        //                }

        //                if (item.ContextGroups.Count > 0 && search.ContextGroups.Count == 0)
        //                {
        //                    item.ContextGroups.ForEach(x => x.IsNew = true);
        //                }
        //                else
        //                {
        //                    if (item.ContextGroups.Count == 0 && search.ContextGroups.Count > 0)
        //                        search.ContextGroups.ForEach(x => x.IsNotNow = true);
        //                    else
        //                    {
        //                        for (int i = 0; i < item.ContextGroups.Count; i++)
        //                        {
        //                            var or = item.ContextGroups.ElementAtOrDefault(i);
        //                            var tr = search.ContextGroups.ElementAtOrDefault(i) ?? null;

        //                            if (tr == null)
        //                            {
        //                                or.IsNew = true;
        //                                item.IsSomeDifferent = true;
        //                            }
        //                            else
        //                            {
        //                                if (tr.Purpose == or.Purpose)
        //                                {
        //                                    if (tr.Contexts.Count != or.Contexts.Count)
        //                                    {
        //                                        or.IsNew = true;
        //                                        tr.IsNotNow = true;
        //                                        item.IsSomeDifferent = true;
        //                                        search.IsSomeDifferent = true;
        //                                    }
        //                                    else
        //                                    {
        //                                        for (int j = 0; j < or.Contexts.Count; j++)
        //                                        {
        //                                            var orC = or.Contexts.ElementAtOrDefault(j);
        //                                            var trC = tr.Contexts.ElementAtOrDefault(j) ?? null;

        //                                            if (trC == null)
        //                                            {
        //                                                or.IsNew = true;
        //                                                tr.IsNotNow = true;
        //                                                item.IsSomeDifferent = true;
        //                                                search.IsSomeDifferent = true;
        //                                            }
        //                                            else
        //                                            {
        //                                                if (orC.ContextType != trC.ContextType || orC.Text != trC.Text)
        //                                                {
        //                                                    or.IsNew = true;
        //                                                    tr.IsNotNow = true;
        //                                                    item.IsSomeDifferent = true;
        //                                                    search.IsSomeDifferent = true;
        //                                                }
        //                                            }
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    or.IsNew = true;
        //                                    tr.IsNotNow = true;
        //                                    item.IsSomeDifferent = true;
        //                                    search.IsSomeDifferent = true;
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        ITextCharacterFormat newFormat = editor1.Document.GetDefaultCharacterFormat();
        //        newFormat.ForegroundColor = Colors.Green;

        //        ITextCharacterFormat notFoundFormat = editor1.Document.GetDefaultCharacterFormat();
        //        notFoundFormat.ForegroundColor = Colors.Red;

        //        foreach (var item in OriginTransUnits)
        //        {
        //            if (item.IsNew)
        //            {
        //                var pos = OriginLinePosDict[item.GlobalLineNumber];
        //                ITextRange? t = editor1.Document.GetRange(pos, pos + 1);
        //                t.Expand(TextRangeUnit.Line);
        //                for (int i = 0; i < item.LineCount; i++)
        //                {
        //                    t.EndPosition += 1;
        //                    t.Expand(TextRangeUnit.Line);
        //                }
        //                t.CharacterFormat = newFormat;
        //            }
        //            else
        //            {
        //                foreach (var itemNote in item.Notes)
        //                {
        //                    if (itemNote.IsNew)
        //                    {
        //                        var pos = OriginLinePosDict[itemNote.GlobalLineNumber];
        //                        ITextRange? t = editor1.Document.GetRange(pos, pos + 1);
        //                        t.Expand(TextRangeUnit.Line);
        //                        t.CharacterFormat = newFormat;
        //                    }
        //                }

        //                foreach (var itemCG in item.ContextGroups)
        //                {
        //                    if (itemCG.IsNew)
        //                    {
        //                        var pos = OriginLinePosDict[itemCG.GlobalLineNumber];
        //                        ITextRange? t = editor1.Document.GetRange(pos, pos + 1);
        //                        t.Expand(TextRangeUnit.Line);
        //                        for (int i = 0; i < itemCG.LineCount; i++)
        //                        {
        //                            t.EndPosition += 1;
        //                            t.Expand(TextRangeUnit.Line);
        //                        }
        //                        t.CharacterFormat = newFormat;
        //                    }
        //                }
        //            }
        //        }

        //        foreach (var item in TranslateTransUnits)
        //        {
        //            if (item.IsNotNow)
        //            {
        //                var pos = TranslateLinePosDict[item.GlobalLineNumber];
        //                ITextRange? t = editor2.Document.GetRange(pos, pos + 1);
        //                t.Expand(TextRangeUnit.Line);
        //                for (int i = 0; i < item.LineCount; i++)
        //                {
        //                    t.EndPosition += 1;
        //                    t.Expand(TextRangeUnit.Line);
        //                }
        //                t.CharacterFormat = notFoundFormat;
        //            }
        //            else
        //            {
        //                foreach (var itemNote in item.Notes)
        //                {
        //                    if (itemNote.IsNotNow)
        //                    {
        //                        var pos = TranslateLinePosDict[itemNote.GlobalLineNumber];
        //                        ITextRange? t = editor2.Document.GetRange(pos, pos + 1);
        //                        t.Expand(TextRangeUnit.Line);
        //                        t.CharacterFormat = notFoundFormat;
        //                    }
        //                }

        //                foreach (var itemCG in item.ContextGroups)
        //                {
        //                    if (itemCG.IsNotNow)
        //                    {
        //                        var pos = TranslateLinePosDict[itemCG.GlobalLineNumber];
        //                        ITextRange? t = editor2.Document.GetRange(pos, pos + 1);
        //                        t.Expand(TextRangeUnit.Line);
        //                        for (int i = 0; i < itemCG.LineCount; i++)
        //                        {
        //                            t.EndPosition += 1;
        //                            t.Expand(TextRangeUnit.Line);
        //                        }
        //                        t.CharacterFormat = notFoundFormat;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        //public void CreateTranslateBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    var xml = XmlUtils.MapObjectToXml(OriginTransUnits);

        //    editor2.Document.SetText(TextSetOptions.None, xml);
        //    TranslateTransUnits = XmlUtils.MapXmlToObject(xml);
        //}

        //public void UpdateTrBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    var updatedModel = XmlUtils.UpdateModel(OriginTransUnits, TranslateTransUnits);

        //    var xml = XmlUtils.MapObjectToXml(updatedModel);

        //    editor2.Document.SetText(TextSetOptions.None, xml);
        //}

        //#region Validate

        //public void ValidateOrBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    IsOriginLoading = true;
        //    editor1.TextDocument.GetText(TextGetOptions.None, out var xml);
        //    OriginLinePosDict = UpdateEditorLineDict(editor1, xml);
        //    OriginValidation = XmlUtils.ValidateXml(xml);
        //    //OnPropertyChanged("OriginValidation");

        //    if (OriginValidation.Errors.Any())
        //    {
        //        OrSubPanel.Height = 150;
        //        OrSubPanelCloseIcon.Glyph = "\uE972";
        //    }
        //    else
        //    {
        //        OrSubPanel.Height = 30;
        //        OrSubPanelCloseIcon.Glyph = "\uE971";
        //    }

        //    ITextCharacterFormat baseFormat = editor1.Document.GetDefaultCharacterFormat();

        //    if (OriginValidation.Errors.Any())
        //    {
        //        ITextCharacterFormat errFormat = editor1.Document.GetDefaultCharacterFormat();
        //        errFormat.ForegroundColor = Colors.Red;

        //        foreach (var item in OriginValidation.Errors)
        //        {
        //            var line = OriginLinePosDict[item.LineNumber];

        //            var stPos = line + item.LinePosition;
        //            var newRange = editor1.Document.GetRange(stPos, stPos);
        //            newRange.Expand(TextRangeUnit.Line);

        //            newRange.CharacterFormat = errFormat;
        //        }
        //    }
        //    else
        //    {
        //        editor1.Document.GetRange(0, xml.Length).CharacterFormat = baseFormat;
        //    }

        //    IsOriginLoading = false;
        //}

        //public void ValidateTrBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    editor2.TextDocument.GetText(TextGetOptions.None, out var xml);
        //    TranslateLinePosDict = UpdateEditorLineDict(editor2, xml);
        //    TranslateValidation = XmlUtils.ValidateXml(xml);

        //    var noTargetList = TranslateTransUnits
        //        .Where(x => string.IsNullOrEmpty(x.Target))
        //        .Select(x => new ValidationError(x.GlobalLineNumber, 0, "Target not found", ValidationErrorType.Warning));
        //    TranslateValidation.SetNoTarget(noTargetList);

        //    //OnPropertyChanged("TranslateValidation");

        //    if (TranslateValidation.Errors.Any())
        //    {
        //        TrSubPanel.Height = 150;
        //        TrSubPanelCloseIcon.Glyph = "\uE972";
        //    }
        //    else
        //    {
        //        TrSubPanel.Height = 30;
        //        TrSubPanelCloseIcon.Glyph = "\uE971";
        //    }

        //    ITextCharacterFormat baseFormat = editor2.Document.GetDefaultCharacterFormat();

        //    if (TranslateValidation.Errors.Any())
        //    {
        //        ITextCharacterFormat errFormat = editor2.Document.GetDefaultCharacterFormat();
        //        errFormat.ForegroundColor = Colors.Red;

        //        foreach (var item in TranslateValidation.Errors)
        //        {
        //            var line = TranslateLinePosDict[item.LineNumber];

        //            var stPos = line + item.LinePosition;
        //            var newRange = editor2.Document.GetRange(stPos, stPos);
        //            newRange.Expand(TextRangeUnit.Line);

        //            newRange.CharacterFormat = errFormat;
        //        }
        //    }
        //    else
        //    {
        //        editor2.Document.GetRange(0, xml.Length).CharacterFormat = baseFormat;
        //    }
        //}

        //public void OrSubPanelClose_Click(object sender, RoutedEventArgs e)
        //{
        //    OrSubPanel.Height = OrSubPanel.Height == 30 ? 150 : 30;
        //    OrSubPanelCloseIcon.Glyph = OrSubPanel.Height == 30 ? "\uE971" : "\uE972";
        //}

        //public void ViewOrErrorItem_Click(object sender, ItemClickEventArgs e)
        //{
        //    if (e.ClickedItem is ValidationError error)
        //    {
        //        var line = OriginLinePosDict[error.LineNumber];
        //        var newRange = editor1.Document.GetRange(line, line + 1);
        //        newRange.Expand(TextRangeUnit.Line);

        //        newRange.ScrollIntoView(PointOptions.Start);
        //    }
        //}

        //public void TrSubPanelClose_Click(object sender, RoutedEventArgs e)
        //{
        //    TrSubPanel.Height = TrSubPanel.Height == 30 ? 150 : 30;
        //    TrSubPanelCloseIcon.Glyph = TrSubPanel.Height == 30 ? "\uE971" : "\uE972";
        //}

        //public void ViewTrErrorItem_Click(object sender, ItemClickEventArgs e)
        //{
        //    if (e.ClickedItem is ValidationError error)
        //    {
        //        var line = TranslateLinePosDict[error.LineNumber];
        //        var newRange = editor2.Document.GetRange(line, line + 1);
        //        newRange.Expand(TextRangeUnit.Line);

        //        newRange.ScrollIntoView(PointOptions.Start);
        //    }
        //}

        //#endregion Validate

        //#region Analyz

        //public void AnalyzOrBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    editor1.TextDocument.GetText(TextGetOptions.None, out var xml);
        //    OriginLinePosDict = UpdateEditorLineDict(editor1, xml);
        //    OriginTransUnits = XmlUtils.MapXmlToObject(xml);
        //}

        //public void AnalyzTrBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    editor2.TextDocument.GetText(TextGetOptions.None, out var xml);
        //    TranslateLinePosDict = UpdateEditorLineDict(editor2, xml);
        //    TranslateTransUnits = XmlUtils.MapXmlToObject(xml);
        //}

        //#endregion Analyz

        //#region format text

        //public void FormatOrBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    editor1.TextDocument.GetText(TextGetOptions.None, out var xml);
        //    var formatedXml = XmlUtils.FormatXml(xml);
        //    editor1.TextDocument.SetText(TextSetOptions.None, formatedXml);
        //}

        //public void FormatTrBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    editor2.TextDocument.GetText(TextGetOptions.None, out var xml);
        //    var formatedXml = XmlUtils.FormatXml(xml);
        //    editor2.TextDocument.SetText(TextSetOptions.None, formatedXml);
        //}

        //#endregion format text

        //#region open file

        //public async void OpenOrBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    var picker = GetOpenFilePicker();

        //    StorageFile file = await picker.PickSingleFileAsync();
        //    if (file != null)
        //    {
        //        IsOriginLoading = true;

        //        OriginFile = file;
        //        editor1.Header = file.Name;

        //        var task = Task.Run(async () =>
        //        {
        //            var xml = await FileIO.ReadTextAsync(OriginFile);

        //            App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        //            {
        //                editor1.TextDocument.SetText(TextSetOptions.None, xml);
        //                OriginLinePosDict = UpdateEditorLineDict(editor1, xml);
        //            });

        //            OriginTransUnits = XmlUtils.MapXmlToObject(xml);

        //            App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        //            {
        //                IsOriginLoading = false;
        //            });
        //        });
        //    }
        //}

        //public async void OpenTrBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    var picker = GetOpenFilePicker();

        //    StorageFile file = await picker.PickSingleFileAsync();
        //    if (file != null)
        //    {
        //        IsTranslateLoading = true;

        //        TranslateFile = file;
        //        editor2.Header = file.Name;

        //        var task = Task.Run(async () =>
        //        {
        //            var xml = await FileIO.ReadTextAsync(TranslateFile);

        //            App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        //            {
        //                editor2.TextDocument.SetText(TextSetOptions.None, xml);
        //                TranslateLinePosDict = UpdateEditorLineDict(editor2, xml);
        //            });

        //            TranslateTransUnits = XmlUtils.MapXmlToObject(xml);

        //            App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        //            {
        //                IsTranslateLoading = false;
        //            });
        //        });
        //    }
        //}

        //private FileOpenPicker GetOpenFilePicker()
        //{
        //    FileOpenPicker picker = new();

        //    var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
        //    WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        //    picker.ViewMode = PickerViewMode.Thumbnail;
        //    picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        //    picker.FileTypeFilter.Add(".xlf");

        //    return picker;
        //}

        //#endregion open file

        //#region save file

        //public async void SaveOrBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    IsOriginLoading = true;

        //    editor1.TextDocument.GetText(TextGetOptions.None, out var xml);
        //    await FileIO.WriteTextAsync(OriginFile, xml);

        //    OriginTransUnits = XmlUtils.MapXmlToObject(xml);

        //    IsOriginLoading = false;
        //}

        //public async void SaveTrBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    if (TranslateFile == null)
        //    {
        //        FileSavePicker picker = new();

        //        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
        //        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        //        picker.FileTypeChoices.Add("Localixation File", new List<string>() { ".xlf" });
        //        picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        //        picker.SuggestedFileName = $"{OriginFile.DisplayName}.lang";

        //        StorageFile file = await picker.PickSaveFileAsync();
        //        if (file != null)
        //        {
        //            TranslateFile = file;
        //            editor2.Header = file.Name;

        //            await SaveTranslateFile();
        //        }
        //    }
        //    else
        //    {
        //        await SaveTranslateFile();
        //    }
        //}

        //private async Task SaveTranslateFile()
        //{
        //    IsTranslateLoading = true;

        //    editor2.TextDocument.GetText(TextGetOptions.None, out var xml);
        //    await FileIO.WriteTextAsync(TranslateFile, xml);

        //    TranslateTransUnits = XmlUtils.MapXmlToObject(xml);

        //    IsTranslateLoading = false;
        //}

        //#endregion save file

        //private Dictionary<int, int> UpdateEditorLineDict(RichEditBox editor)
        //{
        //    Dictionary<int, int> dict = new();

        //    editor.TextDocument.GetText(TextGetOptions.None, out var xml);

        //    int size = 0;
        //    while (xml.Length > size)
        //    {
        //        ITextRange range = editor.Document.GetRange(size, size + 1);
        //        size += range.Expand(TextRangeUnit.Line);
        //        var lineIndex = range.GetIndex(TextRangeUnit.Line);
        //        dict[lineIndex] = range.StartPosition;
        //        size += 1;
        //    }

        //    return dict;
        //}
        //private Dictionary<int, int> UpdateEditorLineDict(RichEditBox editor, string xml)
        //{
        //    Dictionary<int, int> dict = new();

        //    int size = 0;
        //    while (xml.Length > size)
        //    {
        //        ITextRange range = editor.Document.GetRange(size, size + 1);
        //        size += range.Expand(TextRangeUnit.Line);
        //        var lineIndex = range.GetIndex(TextRangeUnit.Line);
        //        dict[lineIndex] = range.StartPosition;
        //        size += 1;
        //    }

        //    return dict;
        //}

        //private void ViewTransUnits(List<TransUnit> transUnit, StackPanel panel)
        //{
        //    panel.Children.Clear();
        //    foreach (var item in transUnit)
        //    {
        //        RichEditBox reb = new();
        //        reb.TextWrapping = TextWrapping.NoWrap;
        //        reb.CanBeScrollAnchor = true;
        //        reb.IsSpellCheckEnabled = false;
        //        reb.Margin = new Thickness(0, 0, 0, 10);
        //        reb.Document.SetText(TextSetOptions.None, item.Xml);
        //        panel.Children.Add(reb);
        //    }
        //}
    }
}
