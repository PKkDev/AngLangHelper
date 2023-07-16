using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

//var file = Path.Combine(AppContext.BaseDirectory, "messages.fr.xlf");
//FileInfo fi = new(file);
//var xml = File.ReadAllText(file);

//var fileXSD1 = Path.Combine(AppContext.BaseDirectory, "xliff-core-1.2-strict.xsd");
//FileInfo fi1 = new(fileXSD1);

//var fileXSD2 = Path.Combine(AppContext.BaseDirectory, "xml.xsd");
//FileInfo fi2 = new(fileXSD2);

//var fileXSD3 = Path.Combine(AppContext.BaseDirectory, "xsdschema.xsd");
//FileInfo fi3 = new(fileXSD3);

//XmlSchemaSet xmlSchemaSet = new();
//xmlSchemaSet.Add("urn:oasis:names:tc:xliff:document:1.2", fileXSD1);
//xmlSchemaSet.Add("http://www.w3.org/XML/1998/namespace", fileXSD2);
//xmlSchemaSet.Add("http://www.w3.org/2001/XMLSchema", fileXSD3);

//try
//{
//    ValidateXml(xmlSchemaSet, file);

//    var xmlForm1 = FormatXml(xml);

//    File.WriteAllText(file, xmlForm1);

//    //sw.Write(xmlForm1);

//    ValidateXml(xmlSchemaSet, file);

//    var x = File.ReadAllText(file);

//    // XmlReaderSettings settings = new();
//    //settings.Schemas = xmlSchemaSet;
//    //settings.ValidationEventHandler += new ValidationEventHandler(ValidationEventHandler);
//    //settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
//    //settings.ValidationType = ValidationType.Schema;

//    //XmlReader reader = XmlReader.Create(file, settings);
//    ////XmlReader reader = new XmlTextReader(xml);


//    //// XDocument doc = XDocument.Parse(xml);
//    //XDocument doc = XDocument.Load(reader);
//    //string xmlParsed = doc.ToString();

//    ////XmlReader reader = doc.CreateReader();
//    //XmlNameTable table = reader.NameTable;

//    //doc.Validate(xmlSchemaSet, ValidationEventHandler);

//    //XElement root = doc.Root;

//    //var xmlns = root.Attribute("xmlns").Value;
//    //XmlNamespaceManager xmlnsManager = new XmlNamespaceManager(table);
//    //xmlnsManager.AddNamespace("def", xmlns);

//    //var transUnits1 = doc.XPathSelectElements("//def:file/def:body/def:trans-unit", xmlnsManager);
//    //var transUnits2 = doc.XPathSelectElements("//def:trans-unit", xmlnsManager);
//}
//catch (XmlException e)
//{
//    Console.WriteLine($"Error Line: {e.LineNumber} Position: {e.LinePosition} {e.Message}");
//}
//catch (Exception e)
//{
//    Console.WriteLine($"Parse error - {e.Message}");
//}



//try
//{


//    XmlReaderSettings settings = new();
//    settings.Schemas = xmlSchemaSet;
//    settings.ValidationEventHandler += new ValidationEventHandler(ValidationEventHandler);
//    settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
//    settings.ValidationType = ValidationType.Schema;

//    XmlReader reader = XmlReader.Create(file, settings);

//    XmlDocument doc = new();

//    doc.Load(reader);
//    XmlElement root = doc.DocumentElement;

//    var xmlns = root.Attributes.GetNamedItem("xmlns").Value;
//    XmlNamespaceManager xmlnsManager = new XmlNamespaceManager(doc.NameTable);
//    xmlnsManager.AddNamespace("def", xmlns);

//    var transUnits1 = doc.SelectNodes("//def:file/def:body/def:trans-unit", xmlnsManager);
//    var transUnits2 = doc.SelectNodes("//def:trans-unit", xmlnsManager);
//}
//catch (XmlException e)
//{
//    Console.WriteLine($"Error Line: {e.LineNumber} Position: {e.LinePosition} {e.Message}");
//}
//catch (Exception e)
//{
//    Console.WriteLine($"Parse error - {e.Message}");
//}


//var text = Console.ReadLine();

//var res = DoAlg(text);
//Console.WriteLine(res);

//Console.ReadKey();

//static string DoAlg(string text)
//{
//    if(text.Length > 1)
//    {
//        var arr = text.Select(x => Convert.ToInt32(x.ToString())).ToList();
//        var summ = arr.Sum(x => x);
//        return DoAlg(summ.ToString());
//    }
//    else
//    {
//        return text;
//    }
//}


var text = File.ReadAllText("input.txt");
var lines = text.Split(Environment.NewLine);
var distint = lines.Distinct().ToList();
File.WriteAllText("output.txt", string.Join(Environment.NewLine, distint));

Console.ReadKey();


//static string FormatXml(string xml)
//{
//    XDocument doc = XDocument.Parse(xml, LoadOptions.SetLineInfo);
//    string xmlParsed = doc.ToString();
//    return xmlParsed;
//}

//static void ValidateXml(XmlSchemaSet xmlSchemaSet, string file)
//{
//    var x = File.ReadAllText(file);

//    XmlReaderSettings settings = new();
//    settings.Schemas = xmlSchemaSet;
//    settings.ValidationEventHandler += new ValidationEventHandler(ValidationEventHandler);
//    settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
//    settings.ValidationType = ValidationType.Schema;

//    XmlReader reader = XmlReader.Create(file, settings);
//    var doc = XDocument.Load(reader);

//    reader.Dispose();
//}

//static void ValidationEventHandler(object sender, ValidationEventArgs args)
//{
//    Console.WriteLine($"{args.Severity} Line: {args.Exception.LineNumber} Position: {args.Exception.LinePosition} {args.Message}");
//}