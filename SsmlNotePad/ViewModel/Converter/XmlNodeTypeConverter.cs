using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    /// <summary>
    /// Base class for converting <seealso cref="System.Xml.XmlNodeType"/> value to a <typeparam name="T" /> value.
    /// </summary>
    public abstract class XmlNodeTypeConverter<T> : DependencyObject, IValueConverter
    {
        #region NullSource Property Members

        /// <summary>
        /// Defines the name for the <see cref="NullSource"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_NullSource = "NullSource";

        /// <summary>
        /// Identifies the <see cref="NullSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NullSourceProperty = DependencyProperty.Register(DependencyPropertyName_NullSource, typeof(T),
            typeof(XmlNodeTypeConverter<T>), new PropertyMetadata(default(T)));

        /// <summary>
        /// <typeparamref name="T"/> to return when the binding source produces a null value.
        /// </summary>
        public T NullSource
        {
            get { return (T)(GetValue(NullSourceProperty)); }
            set { SetValue(NullSourceProperty, value); }
        }

        #endregion

        #region None Property Members

        /// <summary>
        /// Defines the name for the <see cref="None"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_None = "None";

        /// <summary>
        /// Identifies the <see cref="None"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NoneProperty = DependencyProperty.Register(DependencyPropertyName_None, typeof(T),
            typeof(XmlNodeTypeConverter<T>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="System.Xml.XmlNodeType.None"/>.
        /// </summary>
        public T None
        {
            get { return (T)(GetValue(NoneProperty)); }
            set { SetValue(NoneProperty, value); }
        }

        #endregion

        #region Element Property Members

        /// <summary>
        /// Defines the name for the <see cref="Element"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_Element = "Element";

        /// <summary>
        /// Identifies the <see cref="Element"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ElementProperty = DependencyProperty.Register(DependencyPropertyName_Element, typeof(T),
            typeof(XmlNodeTypeConverter<T>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="System.Xml.XmlNodeType.Element"/>.
        /// </summary>
        public T Element
        {
            get { return (T)(GetValue(ElementProperty)); }
            set { SetValue(ElementProperty, value); }
        }

        #endregion

        #region Attribute Property Members

        /// <summary>
        /// Defines the name for the <see cref="Attribute"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_Attribute = "Attribute";

        /// <summary>
        /// Identifies the <see cref="Attribute"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AttributeProperty = DependencyProperty.Register(DependencyPropertyName_Attribute, typeof(T),
            typeof(XmlNodeTypeConverter<T>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="System.Xml.XmlNodeType.Attribute"/>.
        /// </summary>
        public T Attribute
        {
            get { return (T)(GetValue(AttributeProperty)); }
            set { SetValue(AttributeProperty, value); }
        }

        #endregion

        #region Text Property Members

        /// <summary>
        /// Defines the name for the <see cref="Text"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_Text = "Text";

        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(DependencyPropertyName_Text, typeof(T),
            typeof(XmlNodeTypeConverter<T>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="System.Xml.XmlNodeType.Text"/>.
        /// </summary>
        public T Text
        {
            get { return (T)(GetValue(TextProperty)); }
            set { SetValue(TextProperty, value); }
        }

        #endregion

        #region CDATA Property Members

        /// <summary>
        /// Defines the name for the <see cref="CDATA"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_CDATA = "CDATA";

        /// <summary>
        /// Identifies the <see cref="CDATA"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CDATAProperty = DependencyProperty.Register(DependencyPropertyName_CDATA, typeof(T),
            typeof(XmlNodeTypeConverter<T>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="System.Xml.XmlNodeType.CDATA"/>.
        /// </summary>
        public T CDATA
        {
            get { return (T)(GetValue(CDATAProperty)); }
            set { SetValue(CDATAProperty, value); }
        }

        #endregion

        #region EntityReference Property Members

        /// <summary>
        /// Defines the name for the <see cref="EntityReference"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_EntityReference = "EntityReference";

        /// <summary>
        /// Identifies the <see cref="EntityReference"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EntityReferenceProperty = DependencyProperty.Register(DependencyPropertyName_EntityReference, typeof(T),
            typeof(XmlNodeTypeConverter<T>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="System.Xml.XmlNodeType.EntityReference"/>.
        /// </summary>
        public T EntityReference
        {
            get { return (T)(GetValue(EntityReferenceProperty)); }
            set { SetValue(EntityReferenceProperty, value); }
        }

        #endregion

        #region Entity Property Members

        /// <summary>
        /// Defines the name for the <see cref="Entity"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_Entity = "Entity";

        /// <summary>
        /// Identifies the <see cref="Entity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EntityProperty = DependencyProperty.Register(DependencyPropertyName_Entity, typeof(T),
            typeof(XmlNodeTypeConverter<T>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="System.Xml.XmlNodeType.Entity"/>.
        /// </summary>
        public T Entity
        {
            get { return (T)(GetValue(EntityProperty)); }
            set { SetValue(EntityProperty, value); }
        }

        #endregion

        #region ProcessingInstruction Property Members

        /// <summary>
        /// Defines the name for the <see cref="ProcessingInstruction"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_ProcessingInstruction = "ProcessingInstruction";

        /// <summary>
        /// Identifies the <see cref="ProcessingInstruction"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ProcessingInstructionProperty = DependencyProperty.Register(DependencyPropertyName_ProcessingInstruction, typeof(T),
            typeof(XmlNodeTypeConverter<T>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="System.Xml.XmlNodeType.ProcessingInstruction"/>.
        /// </summary>
        public T ProcessingInstruction
        {
            get { return (T)(GetValue(ProcessingInstructionProperty)); }
            set { SetValue(ProcessingInstructionProperty, value); }
        }

        #endregion

        #region Comment Property Members

        /// <summary>
        /// Defines the name for the <see cref="Comment"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_Comment = "Comment";

        /// <summary>
        /// Identifies the <see cref="Comment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommentProperty = DependencyProperty.Register(DependencyPropertyName_Comment, typeof(T),
            typeof(XmlNodeTypeConverter<T>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="System.Xml.XmlNodeType.Comment"/>.
        /// </summary>
        public T Comment
        {
            get { return (T)(GetValue(CommentProperty)); }
            set { SetValue(CommentProperty, value); }
        }

        #endregion

        #region Document Property Members

        /// <summary>
        /// Defines the name for the <see cref="Document"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_Document = "Document";

        /// <summary>
        /// Identifies the <see cref="Document"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register(DependencyPropertyName_Document, typeof(T),
            typeof(XmlNodeTypeConverter<T>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="System.Xml.XmlNodeType.Document"/>.
        /// </summary>
        public T Document
        {
            get { return (T)(GetValue(DocumentProperty)); }
            set { SetValue(DocumentProperty, value); }
        }

        #endregion

        #region DocumentType Property Members

        /// <summary>
        /// Defines the name for the <see cref="DocumentType"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_DocumentType = "DocumentType";

        /// <summary>
        /// Identifies the <see cref="DocumentType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DocumentTypeProperty = DependencyProperty.Register(DependencyPropertyName_DocumentType, typeof(T),
            typeof(XmlNodeTypeConverter<T>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="System.Xml.XmlNodeType.DocumentType"/>.
        /// </summary>
        public T DocumentType
        {
            get { return (T)(GetValue(DocumentTypeProperty)); }
            set { SetValue(DocumentTypeProperty, value); }
        }

        #endregion

        #region DocumentFragment Property Members

        /// <summary>
        /// Defines the name for the <see cref="DocumentFragment"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_DocumentFragment = "DocumentFragment";

        /// <summary>
        /// Identifies the <see cref="DocumentFragment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DocumentFragmentProperty = DependencyProperty.Register(DependencyPropertyName_DocumentFragment, typeof(T),
            typeof(XmlNodeTypeConverter<T>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="System.Xml.XmlNodeType.DocumentFragment"/>.
        /// </summary>
        public T DocumentFragment
        {
            get { return (T)(GetValue(DocumentFragmentProperty)); }
            set { SetValue(DocumentFragmentProperty, value); }
        }

        #endregion

        #region Notation Property Members

        /// <summary>
        /// Defines the name for the <see cref="Notation"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_Notation = "Notation";

        /// <summary>
        /// Identifies the <see cref="Notation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NotationProperty = DependencyProperty.Register(DependencyPropertyName_Notation, typeof(T),
            typeof(XmlNodeTypeConverter<T>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="System.Xml.XmlNodeType.Notation"/>.
        /// </summary>
        public T Notation
        {
            get { return (T)(GetValue(NotationProperty)); }
            set { SetValue(NotationProperty, value); }
        }

        #endregion

        #region Whitespace Property Members

        /// <summary>
        /// Defines the name for the <see cref="Whitespace"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_Whitespace = "Whitespace";

        /// <summary>
        /// Identifies the <see cref="Whitespace"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WhitespaceProperty = DependencyProperty.Register(DependencyPropertyName_Whitespace, typeof(T),
            typeof(XmlNodeTypeConverter<T>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="System.Xml.XmlNodeType.Whitespace"/>.
        /// </summary>
        public T Whitespace
        {
            get { return (T)(GetValue(WhitespaceProperty)); }
            set { SetValue(WhitespaceProperty, value); }
        }

        #endregion

        #region SignificantWhitespace Property Members

        /// <summary>
        /// Defines the name for the <see cref="SignificantWhitespace"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_SignificantWhitespace = "SignificantWhitespace";

        /// <summary>
        /// Identifies the <see cref="SignificantWhitespace"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SignificantWhitespaceProperty = DependencyProperty.Register(DependencyPropertyName_SignificantWhitespace, typeof(T),
            typeof(XmlNodeTypeConverter<T>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="System.Xml.XmlNodeType.SignificantWhitespace"/>.
        /// </summary>
        public T SignificantWhitespace
        {
            get { return (T)(GetValue(SignificantWhitespaceProperty)); }
            set { SetValue(SignificantWhitespaceProperty, value); }
        }

        #endregion

        #region EndElement Property Members

        /// <summary>
        /// Defines the name for the <see cref="EndElement"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_EndElement = "EndElement";

        /// <summary>
        /// Identifies the <see cref="EndElement"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EndElementProperty = DependencyProperty.Register(DependencyPropertyName_EndElement, typeof(T),
            typeof(XmlNodeTypeConverter<T>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="System.Xml.XmlNodeType.EndElement"/>.
        /// </summary>
        public T EndElement
        {
            get { return (T)(GetValue(EndElementProperty)); }
            set { SetValue(EndElementProperty, value); }
        }

        #endregion

        #region EndEntity Property Members

        /// <summary>
        /// Defines the name for the <see cref="EndEntity"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_EndEntity = "EndEntity";

        /// <summary>
        /// Identifies the <see cref="EndEntity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EndEntityProperty = DependencyProperty.Register(DependencyPropertyName_EndEntity, typeof(T),
            typeof(XmlNodeTypeConverter<T>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="System.Xml.XmlNodeType.EndEntity"/>.
        /// </summary>
        public T EndEntity
        {
            get { return (T)(GetValue(EndEntityProperty)); }
            set { SetValue(EndEntityProperty, value); }
        }

        #endregion

        #region XmlDeclaration Property Members

        /// <summary>
        /// Defines the name for the <see cref="XmlDeclaration"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_XmlDeclaration = "XmlDeclaration";

        /// <summary>
        /// Identifies the <see cref="XmlDeclaration"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty XmlDeclarationProperty = DependencyProperty.Register(DependencyPropertyName_XmlDeclaration, typeof(T),
            typeof(XmlNodeTypeConverter<T>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Value to return when source value is <see cref="System.Xml.XmlNodeType.XmlDeclaration"/>.
        /// </summary>
        public T XmlDeclaration
        {
            get { return (T)(GetValue(XmlDeclarationProperty)); }
            set { SetValue(XmlDeclarationProperty, value); }
        }

        #endregion

        /// <summary>
        /// Converts a <seealso cref="FindReplaceDisplayMode"/> value to a <typeparamref name="T"/> value.
        /// </summary>
        /// <param name="value">The <seealso cref="FindReplaceDisplayMode"/> produced by the binding source.</param>
        /// <param name="parameter">Parameter passed by the binding source.</param>
        /// <param name="culture">Culture specified through the binding source.</param>
        /// <returns><seealso cref="FindReplaceDisplayMode"/>value converted to a <typeparamref name="T"/> value.</returns>
        public T Convert(System.Xml.XmlNodeType? value, object parameter, CultureInfo culture)
        {
            if (!value.HasValue)
                return NullSource;

            switch (value.Value)
            {
                case System.Xml.XmlNodeType.Element:
                    return Element;
                case System.Xml.XmlNodeType.Attribute:
                    return Attribute;
                case System.Xml.XmlNodeType.Text:
                    return Text;
                case System.Xml.XmlNodeType.CDATA:
                    return CDATA;
                case System.Xml.XmlNodeType.EntityReference:
                    return EntityReference;
                case System.Xml.XmlNodeType.Entity:
                    return Entity;
                case System.Xml.XmlNodeType.ProcessingInstruction:
                    return ProcessingInstruction;
                case System.Xml.XmlNodeType.Comment:
                    return Comment;
                case System.Xml.XmlNodeType.Document:
                    return Document;
                case System.Xml.XmlNodeType.DocumentType:
                    return DocumentType;
                case System.Xml.XmlNodeType.DocumentFragment:
                    return DocumentFragment;
                case System.Xml.XmlNodeType.Notation:
                    return Notation;
                case System.Xml.XmlNodeType.Whitespace:
                    return Whitespace;
                case System.Xml.XmlNodeType.SignificantWhitespace:
                    return SignificantWhitespace;
                case System.Xml.XmlNodeType.EndElement:
                    return EndElement;
                case System.Xml.XmlNodeType.EndEntity:
                    return EndEntity;
                case System.Xml.XmlNodeType.XmlDeclaration:
                    return XmlDeclaration;
            }

            return None;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value as System.Xml.XmlNodeType?, parameter, culture);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
