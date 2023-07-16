using System.Threading.Tasks;

using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using Windows.Storage;

namespace WinUITestParser;

public static class XmlUtils
{
    private static XmlSchemaSet _xmlSchemaSet { get; set; }

    public static async Task InitSchemas()
    {
        var fileXSD1 = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/XmlForms/xliff-core-1.2-strict.xsd"));
        var fileXSD2 = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/XmlForms/xml.xsd"));
        var fileXSD3 = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/XmlForms/xsdschema.xsd"));

        _xmlSchemaSet = new();
        _xmlSchemaSet.Add("urn:oasis:names:tc:xliff:document:1.2", fileXSD1.Path);
        _xmlSchemaSet.Add("http://www.w3.org/XML/1998/namespace", fileXSD2.Path);
        _xmlSchemaSet.Add("http://www.w3.org/2001/XMLSchema", fileXSD3.Path);
    }

    public static ValidationErrorModel ValidateXml(StorageFile file)
    {
        ValidationErrorModel result = new();
        List<ValidationError> validateResult = new();

        try
        {
            XmlReaderSettings settings = new();
            settings.Schemas = _xmlSchemaSet;
            settings.ValidationEventHandler += (object sender, ValidationEventArgs args) =>
            {
                validateResult.Add(new ValidationError(args.Exception.LineNumber, args.Exception.LinePosition, args.Message, args.Severity.ToString()));
            };
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationType = ValidationType.Schema;

            using XmlReader reader = XmlReader.Create(file.Path, settings);
            XDocument doc = XDocument.Load(reader);
        }
        catch (XmlException e)
        {
            validateResult.Add(new ValidationError(e.LineNumber, e.LinePosition, e.Message, ValidationErrorType.Error));
        }
        catch (Exception e)
        {
            validateResult.Add(new ValidationError(0, 0, e.Message, ValidationErrorType.Error));
        }

        return new ValidationErrorModel(validateResult);
    }

    public static ValidationErrorModel ValidateXml(string xml)
    {
        List<ValidationError> validateResult = new();

        try
        {
            var opts = LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo;
            XDocument doc = XDocument.Parse(xml, opts);

            doc.Validate(
                _xmlSchemaSet,
                (object sender, ValidationEventArgs args) =>
                {
                    validateResult.Add(new ValidationError(args.Exception.LineNumber, args.Exception.LinePosition, args.Message, args.Severity.ToString()));
                });
        }
        catch (XmlException e)
        {
            if (e.Message.Contains("start tag on line") && e.Message.Contains("not match the end tag"))
            {
                try
                {
                    var lineStr = e.Message.IndexOf("line");
                    var line = Convert.ToInt32(e.Message.Substring(lineStr + 4, 3).Trim());

                    var positionStr = e.Message.IndexOf("position");
                    var position = Convert.ToInt32(e.Message.Substring(positionStr + 8, 3).Trim());

                    validateResult.Add(new ValidationError(line, position, e.Message, ValidationErrorType.Error));
                }
                catch (Exception) { }
            }

            validateResult.Add(new ValidationError(e.LineNumber, e.LinePosition, e.Message, ValidationErrorType.Error));
        }
        catch (Exception e)
        {
            validateResult.Add(new ValidationError(0, 0, e.Message, ValidationErrorType.Error));
        }

        return new ValidationErrorModel(validateResult);
    }

    public static string FormatXml(string xml)
    {
        try
        {
            //XmlWriterSettings settings = new()
            //{
            //    OmitXmlDeclaration = true,

            //    Indent = true,
            //    NewLineOnAttributes = true,
            //    IndentChars = "  ",
            //    ConformanceLevel = ConformanceLevel.Document
            //};

            //using var textReader = new StringReader(xml);
            //using var xmlReader = XmlReader.Create(textReader, new XmlReaderSettings { ConformanceLevel = settings.ConformanceLevel });
            //using var textWriter = new StringWriter();

            //using (var xmlWriter = XmlWriter.Create(textWriter, settings))
            //    xmlWriter.WriteNode(xmlReader, true);

            //string xmlParsed = textWriter.ToString();

            //XmlDocument xmlDoc = new XmlDocument();
            //StringWriter sw = new StringWriter();
            //xmlDoc.LoadXml(xml);
            //xmlDoc.Save(sw);
            //string formattedXml = sw.ToString();



            XDocument doc = XDocument.Parse(xml);
            //string xmlParsed2 = doc.ToString(SaveOptions.None);


            StringWriter sw2 = new StringWriterWithEncoding(Encoding.UTF8);
            doc.Save(sw2);
            string xmlParsed3 = sw2.ToString();

            return xmlParsed3;
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public static List<TransUnit> MapXmlToObject(string xml)
    {
        List<TransUnit> transUnitsObj = new();

        try
        {
            var loadOptions = LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo;

            using var textReader = new StringReader(xml);
            using XmlReader xmlReader = XmlReader.Create(textReader);

            XDocument document = XDocument.Load(xmlReader, loadOptions);
            var declaration = document.Declaration.ToString();
            XmlNameTable table = xmlReader.NameTable;
            XElement root = document.Root;
            var xmlns = root.Attribute("xmlns").Value;
            XmlNamespaceManager xmlnsManager = new(table);
            xmlnsManager.AddNamespace("def", xmlns);
            var transUnits = document.XPathSelectElements("//def:trans-unit", xmlnsManager);

            foreach (XElement item in transUnits)
            {
                TransUnit unit = new();

                var itemInfo = (IXmlLineInfo)item;
                var lineNumber = itemInfo.HasLineInfo() ? itemInfo.LineNumber : -1;

                unit.LocalLineNumber = 1;
                unit.GlobalLineNumber = lineNumber;

                XDocument docI = XDocument.Parse(item.ToString(), loadOptions);

                XElement rootI = docI.Root;

                unit.Xml = rootI.ToString().Replace("xmlns=\"urn:oasis:names:tc:xliff:document:1.2\"", "");
                unit.LineCount = unit.Xml.Split(Environment.NewLine).Length - 1;

                var id = rootI.Attribute("id")?.Value ?? null;
                unit.Id = id;
                var datatype = rootI.Attribute("datatype")?.Value ?? null;
                unit.Datatype = datatype;

                var source = rootI.XPathSelectElement("//def:source", xmlnsManager);
                var sourceTxt = source.Value;
                unit.Source = sourceTxt;

                var target = rootI.XPathSelectElement("//def:target", xmlnsManager);
                var targetTxt = target?.Value ?? null;
                unit.Target = targetTxt;

                var notes = rootI.XPathSelectElements("//def:note", xmlnsManager);
                foreach (XElement note in notes)
                {
                    TransUnitNote noteObj = new();

                    var noteInfo = (IXmlLineInfo)note;
                    var noteLineNumber = noteInfo.HasLineInfo() ? noteInfo.LineNumber : -1;

                    noteObj.LocalLineNumber = noteLineNumber;
                    noteObj.GlobalLineNumber = noteLineNumber + unit.GlobalLineNumber - 1;

                    noteObj.Xml = note.ToString().Replace("xmlns=\"urn:oasis:names:tc:xliff:document:1.2\"", "");

                    var priority = note.Attribute("priority")?.Value ?? null;
                    noteObj.Priority = priority;

                    var from = note.Attribute("from")?.Value ?? null;
                    noteObj.From = from;

                    noteObj.Text = note.Value;

                    unit.Notes.Add(noteObj);
                }

                var contextGroups = rootI.XPathSelectElements("//def:context-group", xmlnsManager);
                foreach (var contextGroup in contextGroups)
                {
                    TransUnitContextGroup contextGroupObj = new();

                    XDocument docCG = XDocument.Parse(contextGroup.ToString(), loadOptions);
                    XElement rootCG = docCG.Root;

                    var contextGroupInfo = (IXmlLineInfo)contextGroup;
                    var contextGroupInfoLineNumber = contextGroupInfo.HasLineInfo() ? contextGroupInfo.LineNumber : -1;

                    contextGroupObj.LocalLineNumber = contextGroupInfoLineNumber;
                    contextGroupObj.GlobalLineNumber = contextGroupInfoLineNumber + unit.GlobalLineNumber - 1;

                    contextGroupObj.Xml = rootCG.ToString().Replace("xmlns=\"urn:oasis:names:tc:xliff:document:1.2\"", "");

                    var countLine = contextGroupObj.Xml.Split(Environment.NewLine).Length - 1;

                    contextGroupObj.LineCount = countLine;

                    var purpose = rootCG.Attribute("purpose")?.Value;
                    contextGroupObj.Purpose = purpose;

                    var contexts = rootCG.XPathSelectElements("//def:context", xmlnsManager);
                    foreach (var context in contexts)
                    {
                        ContextItem ci = new();

                        var contextInfo = (IXmlLineInfo)context;
                        var contextInfoLineNumber = contextInfo.HasLineInfo() ? contextInfo.LineNumber : -1;

                        ci.LocalLineNumber = contextInfoLineNumber + contextGroupObj.LocalLineNumber - 1;
                        ci.GlobalLineNumber = contextInfoLineNumber + contextGroupObj.GlobalLineNumber - 1;

                        ci.Xml = context.ToString().Replace("xmlns=\"urn:oasis:names:tc:xliff:document:1.2\"", "");

                        var contextType = context.Attribute("context-type")?.Value;
                        ci.ContextType = contextType;

                        ci.Text = context.Value;

                        contextGroupObj.Contexts.Add(ci);
                    }

                    unit.ContextGroups.Add(contextGroupObj);
                }

                transUnitsObj.Add(unit);
            }

            return transUnitsObj;

        }
        catch (Exception e)
        {
            return transUnitsObj;
        }
    }

    public static string MapObjectToXml(List<TransUnit> transUnits)
    {
        XNamespace aw = "urn:oasis:names:tc:xliff:document:1.2";

        XElement xliffEl = new(aw + "xliff");
        xliffEl.Add(new XAttribute("version", "1.2"));

        XElement file = new(aw + "file");
        file.Add(new XAttribute("source-language", "en"));
        file.Add(new XAttribute("datatype", "plaintext"));
        file.Add(new XAttribute("original", "ng2.templat"));

        XElement body = new(aw + "body");

        foreach (var transUnit in transUnits)
        {
            XElement transUnitEl = new(aw + "trans-unit");

            transUnitEl.Add(new XAttribute("id", transUnit.Id));
            transUnitEl.Add(new XAttribute("datatype", transUnit.Datatype));

            XElement sourceEl = new(aw + "source");
            sourceEl.Value = transUnit.Source;
            transUnitEl.Add(sourceEl);

            XElement targetEl = new(aw + "target");
            targetEl.Value = transUnit.Target ?? string.Empty;
            transUnitEl.Add(targetEl);

            foreach (var contextGroup in transUnit.ContextGroups)
            {
                XElement contextGroupEl = new(aw + "context-group");
                contextGroupEl.Add(new XAttribute("purpose", contextGroup.Purpose));

                foreach (var context in contextGroup.Contexts)
                {
                    XElement contextEl = new(aw + "context");
                    contextEl.Add(new XAttribute("context-type", context.ContextType));
                    contextEl.Value = context.Text;

                    contextGroupEl.Add(contextEl);
                }

                transUnitEl.Add(contextGroupEl);
            }

            foreach (var note in transUnit.Notes)
            {
                XElement noteEl = new(aw + "note");
                noteEl.Add(new XAttribute("priority", note.Priority));
                noteEl.Add(new XAttribute("from", note.From));
                noteEl.Value = note.Text;
                transUnitEl.Add(noteEl);
            }

            body.Add(transUnitEl);
        }

        file.Add(body);

        xliffEl.Add(file);

        XDocument document = new(xliffEl);

        StringWriter sw = new StringWriterWithEncoding(Encoding.UTF8);
        document.Save(sw);
        string xmlParsed = sw.ToString();

        return xmlParsed;
    }

    public static List<TransUnit> UpdateModel(List<TransUnit> origin, List<TransUnit> translate)
    {
        List<TransUnit> transUnits = new();

        foreach (var unit in origin)
        {
            var resUnit = new TransUnit(unit);

            var tr = translate.FirstOrDefault(x => x.Id == unit.Id);
            if (tr != null)
                resUnit.Target = tr.Target;

            transUnits.Add(resUnit);
        }

        return transUnits;
    }
}
