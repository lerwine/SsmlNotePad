using System;
using System.Windows;
using System.Windows.Data;
using System.Xml;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    /// <summary>
    /// Converts <seealso cref="XmlNodeType"/> value to a <seealso cref="Visibility"/> value.
    /// </summary>
    [ValueConversion(typeof(XmlNodeType?), typeof(Visibility?))]
    public class XmlNodeTypeToVisibilityConverter : XmlNodeTypeConverter<Visibility?>
    {
    }
}
