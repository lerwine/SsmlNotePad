using System;
using System.Windows;
using System.Windows.Data;
using System.Xml;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    [ValueConversion(typeof(XmlNodeType?), typeof(Visibility?))]
    public class XmlNodeTypeToVisibilityConverter : XmlNodeTypeConverter<Visibility?>
    {
    }
}
