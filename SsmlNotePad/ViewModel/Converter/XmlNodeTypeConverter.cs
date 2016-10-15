using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    public abstract class XmlNodeTypeConverter<T> : DependencyObject, IValueConverter
    {
        #region NullValue Property Members

        public const string DependencyPropertyName_NullValue = "NullValue";

        /// <summary>
        /// Identifies the <see cref="NullValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NullValueProperty = DependencyProperty.Register(DependencyPropertyName_NullValue, typeof(T),
            typeof(XmlNodeTypeConverter<TabControl>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is null.
        /// </summary>
        public T NullValue
        {
            get
            {
                if (CheckAccess())
                    return (T)(GetValue(NullValueProperty));
                return Dispatcher.Invoke(() => NullValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(NullValueProperty, value);
                else
                    Dispatcher.Invoke(() => NullValue = value);
            }
        }

        #endregion

        #region NoneValue Property Members

        public const string DependencyPropertyName_NoneValue = "NoneValue";

        /// <summary>
        /// Identifies the <see cref="NoneValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NoneValueProperty = DependencyProperty.Register(DependencyPropertyName_NoneValue, typeof(T),
            typeof(XmlNodeTypeConverter<TabControl>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="XmlNodeType.None"/>.
        /// </summary>
        public T NoneValue
        {
            get
            {
                if (CheckAccess())
                    return (T)(GetValue(NoneValueProperty));
                return Dispatcher.Invoke(() => NoneValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(NoneValueProperty, value);
                else
                    Dispatcher.Invoke(() => NoneValue = value);
            }
        }

        #endregion

        #region ElementValue Property Members

        public const string DependencyPropertyName_ElementValue = "ElementValue";

        /// <summary>
        /// Identifies the <see cref="ElementValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ElementValueProperty = DependencyProperty.Register(DependencyPropertyName_ElementValue, typeof(T),
            typeof(XmlNodeTypeConverter<TabControl>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="XmlNodeType.Element"/>.
        /// </summary>
        public T ElementValue
        {
            get
            {
                if (CheckAccess())
                    return (T)(GetValue(ElementValueProperty));
                return Dispatcher.Invoke(() => ElementValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(ElementValueProperty, value);
                else
                    Dispatcher.Invoke(() => ElementValue = value);
            }
        }

        #endregion

        #region AttributeValue Property Members

        public const string DependencyPropertyName_AttributeValue = "AttributeValue";

        /// <summary>
        /// Identifies the <see cref="AttributeValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AttributeValueProperty = DependencyProperty.Register(DependencyPropertyName_AttributeValue, typeof(T),
            typeof(XmlNodeTypeConverter<TabControl>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="XmlNodeType.Attribute"/>.
        /// </summary>
        public T AttributeValue
        {
            get
            {
                if (CheckAccess())
                    return (T)(GetValue(AttributeValueProperty));
                return Dispatcher.Invoke(() => AttributeValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(AttributeValueProperty, value);
                else
                    Dispatcher.Invoke(() => AttributeValue = value);
            }
        }

        #endregion

        #region TextValue Property Members

        public const string DependencyPropertyName_TextValue = "TextValue";

        /// <summary>
        /// Identifies the <see cref="TextValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextValueProperty = DependencyProperty.Register(DependencyPropertyName_TextValue, typeof(T),
            typeof(XmlNodeTypeConverter<TabControl>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="XmlNodeType.Text"/>.
        /// </summary>
        public T TextValue
        {
            get
            {
                if (CheckAccess())
                    return (T)(GetValue(TextValueProperty));
                return Dispatcher.Invoke(() => TextValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(TextValueProperty, value);
                else
                    Dispatcher.Invoke(() => TextValue = value);
            }
        }

        #endregion

        #region CDATAValue Property Members

        public const string DependencyPropertyName_CDATAValue = "CDATAValue";

        /// <summary>
        /// Identifies the <see cref="CDATAValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CDATAValueProperty = DependencyProperty.Register(DependencyPropertyName_CDATAValue, typeof(T),
            typeof(XmlNodeTypeConverter<TabControl>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="XmlNodeType.CDATA"/>.
        /// </summary>
        public T CDATAValue
        {
            get
            {
                if (CheckAccess())
                    return (T)(GetValue(CDATAValueProperty));
                return Dispatcher.Invoke(() => CDATAValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(CDATAValueProperty, value);
                else
                    Dispatcher.Invoke(() => CDATAValue = value);
            }
        }

        #endregion

        #region EntityReferenceValue Property Members

        public const string DependencyPropertyName_EntityReferenceValue = "EntityReferenceValue";

        /// <summary>
        /// Identifies the <see cref="EntityReferenceValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EntityReferenceValueProperty = DependencyProperty.Register(DependencyPropertyName_EntityReferenceValue, typeof(T),
            typeof(XmlNodeTypeConverter<TabControl>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="XmlNodeType.EntityReference"/>.
        /// </summary>
        public T EntityReferenceValue
        {
            get
            {
                if (CheckAccess())
                    return (T)(GetValue(EntityReferenceValueProperty));
                return Dispatcher.Invoke(() => EntityReferenceValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(EntityReferenceValueProperty, value);
                else
                    Dispatcher.Invoke(() => EntityReferenceValue = value);
            }
        }

        #endregion

        #region EntityValue Property Members

        public const string DependencyPropertyName_EntityValue = "EntityValue";

        /// <summary>
        /// Identifies the <see cref="EntityValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EntityValueProperty = DependencyProperty.Register(DependencyPropertyName_EntityValue, typeof(T),
            typeof(XmlNodeTypeConverter<TabControl>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="XmlNodeType.Entity"/>.
        /// </summary>
        public T EntityValue
        {
            get
            {
                if (CheckAccess())
                    return (T)(GetValue(EntityValueProperty));
                return Dispatcher.Invoke(() => EntityValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(EntityValueProperty, value);
                else
                    Dispatcher.Invoke(() => EntityValue = value);
            }
        }

        #endregion

        #region ProcessingInstructionValue Property Members

        public const string DependencyPropertyName_ProcessingInstructionValue = "ProcessingInstructionValue";

        /// <summary>
        /// Identifies the <see cref="ProcessingInstructionValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ProcessingInstructionValueProperty = DependencyProperty.Register(DependencyPropertyName_ProcessingInstructionValue, typeof(T),
            typeof(XmlNodeTypeConverter<TabControl>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="XmlNodeType.ProcessingInstruction"/>.
        /// </summary>
        public T ProcessingInstructionValue
        {
            get
            {
                if (CheckAccess())
                    return (T)(GetValue(ProcessingInstructionValueProperty));
                return Dispatcher.Invoke(() => ProcessingInstructionValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(ProcessingInstructionValueProperty, value);
                else
                    Dispatcher.Invoke(() => ProcessingInstructionValue = value);
            }
        }

        #endregion

        #region CommentValue Property Members

        public const string DependencyPropertyName_CommentValue = "CommentValue";

        /// <summary>
        /// Identifies the <see cref="CommentValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommentValueProperty = DependencyProperty.Register(DependencyPropertyName_CommentValue, typeof(T),
            typeof(XmlNodeTypeConverter<TabControl>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="XmlNodeType.Comment"/>.
        /// </summary>
        public T CommentValue
        {
            get
            {
                if (CheckAccess())
                    return (T)(GetValue(CommentValueProperty));
                return Dispatcher.Invoke(() => CommentValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(CommentValueProperty, value);
                else
                    Dispatcher.Invoke(() => CommentValue = value);
            }
        }

        #endregion

        #region DocumentValue Property Members

        public const string DependencyPropertyName_DocumentValue = "DocumentValue";

        /// <summary>
        /// Identifies the <see cref="DocumentValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DocumentValueProperty = DependencyProperty.Register(DependencyPropertyName_DocumentValue, typeof(T),
            typeof(XmlNodeTypeConverter<TabControl>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="XmlNodeType.Document"/>.
        /// </summary>
        public T DocumentValue
        {
            get
            {
                if (CheckAccess())
                    return (T)(GetValue(DocumentValueProperty));
                return Dispatcher.Invoke(() => DocumentValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(DocumentValueProperty, value);
                else
                    Dispatcher.Invoke(() => DocumentValue = value);
            }
        }

        #endregion

        #region DocumentTypeValue Property Members

        public const string DependencyPropertyName_DocumentTypeValue = "DocumentTypeValue";

        /// <summary>
        /// Identifies the <see cref="DocumentTypeValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DocumentTypeValueProperty = DependencyProperty.Register(DependencyPropertyName_DocumentTypeValue, typeof(T),
            typeof(XmlNodeTypeConverter<TabControl>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="XmlNodeType.DocumentType"/>.
        /// </summary>
        public T DocumentTypeValue
        {
            get
            {
                if (CheckAccess())
                    return (T)(GetValue(DocumentTypeValueProperty));
                return Dispatcher.Invoke(() => DocumentTypeValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(DocumentTypeValueProperty, value);
                else
                    Dispatcher.Invoke(() => DocumentTypeValue = value);
            }
        }

        #endregion

        #region DocumentFragmentValue Property Members

        public const string DependencyPropertyName_DocumentFragmentValue = "DocumentFragmentValue";

        /// <summary>
        /// Identifies the <see cref="DocumentFragmentValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DocumentFragmentValueProperty = DependencyProperty.Register(DependencyPropertyName_DocumentFragmentValue, typeof(T),
            typeof(XmlNodeTypeConverter<TabControl>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="XmlNodeType.DocumentFragment"/>.
        /// </summary>
        public T DocumentFragmentValue
        {
            get
            {
                if (CheckAccess())
                    return (T)(GetValue(DocumentFragmentValueProperty));
                return Dispatcher.Invoke(() => DocumentFragmentValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(DocumentFragmentValueProperty, value);
                else
                    Dispatcher.Invoke(() => DocumentFragmentValue = value);
            }
        }

        #endregion

        #region NotationValue Property Members

        public const string DependencyPropertyName_NotationValue = "NotationValue";

        /// <summary>
        /// Identifies the <see cref="NotationValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NotationValueProperty = DependencyProperty.Register(DependencyPropertyName_NotationValue, typeof(T),
            typeof(XmlNodeTypeConverter<TabControl>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="XmlNodeType.Notation"/>.
        /// </summary>
        public T NotationValue
        {
            get
            {
                if (CheckAccess())
                    return (T)(GetValue(NotationValueProperty));
                return Dispatcher.Invoke(() => NotationValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(NotationValueProperty, value);
                else
                    Dispatcher.Invoke(() => NotationValue = value);
            }
        }

        #endregion

        #region WhitespaceValue Property Members

        public const string DependencyPropertyName_WhitespaceValue = "WhitespaceValue";

        /// <summary>
        /// Identifies the <see cref="WhitespaceValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WhitespaceValueProperty = DependencyProperty.Register(DependencyPropertyName_WhitespaceValue, typeof(T),
            typeof(XmlNodeTypeConverter<TabControl>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="XmlNodeType.Whitespace"/>.
        /// </summary>
        public T WhitespaceValue
        {
            get
            {
                if (CheckAccess())
                    return (T)(GetValue(WhitespaceValueProperty));
                return Dispatcher.Invoke(() => WhitespaceValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(WhitespaceValueProperty, value);
                else
                    Dispatcher.Invoke(() => WhitespaceValue = value);
            }
        }

        #endregion

        #region SignificantWhitespaceValue Property Members

        public const string DependencyPropertyName_SignificantWhitespaceValue = "SignificantWhitespaceValue";

        /// <summary>
        /// Identifies the <see cref="SignificantWhitespaceValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SignificantWhitespaceValueProperty = DependencyProperty.Register(DependencyPropertyName_SignificantWhitespaceValue, typeof(T),
            typeof(XmlNodeTypeConverter<TabControl>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="XmlNodeType.SignificantWhitespace"/>.
        /// </summary>
        public T SignificantWhitespaceValue
        {
            get
            {
                if (CheckAccess())
                    return (T)(GetValue(SignificantWhitespaceValueProperty));
                return Dispatcher.Invoke(() => SignificantWhitespaceValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(SignificantWhitespaceValueProperty, value);
                else
                    Dispatcher.Invoke(() => SignificantWhitespaceValue = value);
            }
        }

        #endregion

        #region EndElementValue Property Members

        public const string DependencyPropertyName_EndElementValue = "EndElementValue";

        /// <summary>
        /// Identifies the <see cref="EndElementValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EndElementValueProperty = DependencyProperty.Register(DependencyPropertyName_EndElementValue, typeof(T),
            typeof(XmlNodeTypeConverter<TabControl>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="XmlNodeType.EndElement"/>.
        /// </summary>
        public T EndElementValue
        {
            get
            {
                if (CheckAccess())
                    return (T)(GetValue(EndElementValueProperty));
                return Dispatcher.Invoke(() => EndElementValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(EndElementValueProperty, value);
                else
                    Dispatcher.Invoke(() => EndElementValue = value);
            }
        }

        #endregion

        #region EndEntityValue Property Members

        public const string DependencyPropertyName_EndEntityValue = "EndEntityValue";

        /// <summary>
        /// Identifies the <see cref="EndEntityValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EndEntityValueProperty = DependencyProperty.Register(DependencyPropertyName_EndEntityValue, typeof(T),
            typeof(XmlNodeTypeConverter<TabControl>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="XmlNodeType.EndEntity"/>.
        /// </summary>
        public T EndEntityValue
        {
            get
            {
                if (CheckAccess())
                    return (T)(GetValue(EndEntityValueProperty));
                return Dispatcher.Invoke(() => EndEntityValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(EndEntityValueProperty, value);
                else
                    Dispatcher.Invoke(() => EndEntityValue = value);
            }
        }

        #endregion

        #region XmlDeclarationValue Property Members

        public const string DependencyPropertyName_XmlDeclarationValue = "XmlDeclarationValue";

        /// <summary>
        /// Identifies the <see cref="XmlDeclarationValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty XmlDeclarationValueProperty = DependencyProperty.Register(DependencyPropertyName_XmlDeclarationValue, typeof(T),
            typeof(XmlNodeTypeConverter<TabControl>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="XmlNodeType.XmlDeclaration"/>.
        /// </summary>
        public T XmlDeclarationValue
        {
            get
            {
                if (CheckAccess())
                    return (T)(GetValue(XmlDeclarationValueProperty));
                return Dispatcher.Invoke(() => XmlDeclarationValue);
            }
            set
            {
                if (CheckAccess())
                    SetValue(XmlDeclarationValueProperty, value);
                else
                    Dispatcher.Invoke(() => XmlDeclarationValue = value);
            }
        }

        #endregion

        public T Convert(XmlNodeType? value, object parameter, CultureInfo culture)
        {
            if (!value.HasValue)
                return NullValue;

            switch (value.Value)
            {
                case XmlNodeType.Element:
                    return ElementValue;
                case XmlNodeType.Attribute:
                    return AttributeValue;
                case XmlNodeType.Text:
                    return TextValue;
                case XmlNodeType.CDATA:
                    return CDATAValue;
                case XmlNodeType.EntityReference:
                    return EntityReferenceValue;
                case XmlNodeType.Entity:
                    return EntityValue;
                case XmlNodeType.ProcessingInstruction:
                    return ProcessingInstructionValue;
                case XmlNodeType.Comment:
                    return CommentValue;
                case XmlNodeType.Document:
                    return DocumentValue;
                case XmlNodeType.DocumentType:
                    return DocumentTypeValue;
                case XmlNodeType.DocumentFragment:
                    return DocumentFragmentValue;
                case XmlNodeType.Notation:
                    return NotationValue;
                case XmlNodeType.Whitespace:
                    return WhitespaceValue;
                case XmlNodeType.SignificantWhitespace:
                    return SignificantWhitespaceValue;
                case XmlNodeType.EndElement:
                    return EndElementValue;
                case XmlNodeType.EndEntity:
                    return EndEntityValue;
                case XmlNodeType.XmlDeclaration:
                    return XmlDeclarationValue;
            }

            return NoneValue;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value as XmlNodeType?, parameter, culture);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
