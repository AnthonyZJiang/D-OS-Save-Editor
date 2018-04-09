using System;
using System.IO;
using System.Text;
using System.Xml;

namespace D_OS_Save_Editor
{
    public class XmlUtilities
    {
        public static bool IsBool(string val)
        {
            return val == "True" | val == "False";
        }

        public static bool IsInt(string val)
        {
            return int.TryParse(val, out _);
        }

        public static bool IsUnint(string val)
        {
            return int.TryParse(val, out var intVal) && intVal >= 0;
        }

        public static bool IsDouble(string val)
        {
            return double.TryParse(val, out _);
        }

        public static string BeautifyXml(XmlNode xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml.OuterXml);
            var builder = new StringBuilder();
            var writer = new StringWriter(builder);
            XmlTextWriter xtw = null;
            try
            {
                xtw = new XmlTextWriter(writer) {Formatting = Formatting.Indented};
                doc.WriteTo(xtw);
            }
            finally
            {
                xtw?.Close();
            }
            return builder.ToString();
        }
    }

    public class XmlValidationException : Exception
    {
        public string Name { get; }
        public string Value { get; }

        public XmlValidationException()
        {
        }

        public XmlValidationException(string name, string value) : base($"Invalid value '{value}' for variable '{name}'.")
        {
            Name = name;
            Value = value;
        }
    }
}