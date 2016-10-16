using System;
using System.Windows.Data;
using System.Xml;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    /// <summary>
    /// Converts <seealso cref="XmlNodeType"/> value to a <seealso cref="string"/> value.
    /// </summary>
    [ValueConversion(typeof(XmlNodeType?), typeof(string))]
    public class XmlNodeTypeToStringConverter : XmlNodeTypeConverter<string>
    {
    }
}
