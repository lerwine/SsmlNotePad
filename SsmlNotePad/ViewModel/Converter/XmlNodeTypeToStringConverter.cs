using System;
using System.Windows.Data;
using System.Xml;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    [ValueConversion(typeof(XmlNodeType?), typeof(string))]
    public class XmlNodeTypeToStringConverter : XmlNodeTypeConverter<string>
    {
    }
}
