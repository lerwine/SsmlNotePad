using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Schema;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Xml.Ssml
{
    public class SsmlMarkupVM : DependencyObject
    {
        #region Localname Property Members

        public const string PropertyName_Localname = "Localname";

        private static readonly DependencyPropertyKey LocalnamePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Localname, typeof(string), typeof(SsmlMarkupVM),
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <seealso cref="Localname"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LocalnameProperty = LocalnamePropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string Localname
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(LocalnameProperty));
                return Dispatcher.Invoke(() => Localname);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(LocalnamePropertyKey, value);
                else
                    Dispatcher.Invoke(() => Localname = value);
            }
        }

        #endregion

        #region NamespaceURI Property Members

        public const string PropertyName_NamespaceURI = "NamespaceURI";

        private static readonly DependencyPropertyKey NamespaceURIPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_NamespaceURI, typeof(string), typeof(SsmlMarkupVM),
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <seealso cref="NamespaceURI"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NamespaceURIProperty = NamespaceURIPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string NamespaceURI
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(NamespaceURIProperty));
                return Dispatcher.Invoke(() => NamespaceURI);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(NamespaceURIPropertyKey, value);
                else
                    Dispatcher.Invoke(() => NamespaceURI = value);
            }
        }

        #endregion

        #region OuterStartIndex Property Members

        public const string PropertyName_OuterStartIndex = "OuterStartIndex";

        private static readonly DependencyPropertyKey OuterStartIndexPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_OuterStartIndex, typeof(int), typeof(SsmlMarkupVM),
                new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <seealso cref="OuterStartIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OuterStartIndexProperty = OuterStartIndexPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public int OuterStartIndex
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(OuterStartIndexProperty));
                return Dispatcher.Invoke(() => OuterStartIndex);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(OuterStartIndexPropertyKey, value);
                else
                    Dispatcher.Invoke(() => OuterStartIndex = value);
            }
        }

        #endregion

        #region OuterLength Property Members

        public const string PropertyName_OuterLength = "OuterLength";

        private static readonly DependencyPropertyKey OuterLengthPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_OuterLength, typeof(int), typeof(SsmlMarkupVM),
                new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <seealso cref="OuterLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OuterLengthProperty = OuterLengthPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public int OuterLength
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(OuterLengthProperty));
                return Dispatcher.Invoke(() => OuterLength);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(OuterLengthPropertyKey, value);
                else
                    Dispatcher.Invoke(() => OuterLength = value);
            }
        }

        #endregion

        #region InnerStartIndex Property Members

        public const string PropertyName_InnerStartIndex = "InnerStartIndex";

        private static readonly DependencyPropertyKey InnerStartIndexPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_InnerStartIndex, typeof(int), typeof(SsmlMarkupVM),
                new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <seealso cref="InnerStartIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerStartIndexProperty = InnerStartIndexPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public int InnerStartIndex
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(InnerStartIndexProperty));
                return Dispatcher.Invoke(() => InnerStartIndex);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(InnerStartIndexPropertyKey, value);
                else
                    Dispatcher.Invoke(() => InnerStartIndex = value);
            }
        }

        #endregion

        #region InnerLength Property Members

        public const string PropertyName_InnerLength = "InnerLength";

        private static readonly DependencyPropertyKey InnerLengthPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_InnerLength, typeof(int), typeof(SsmlMarkupVM),
                new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <seealso cref="InnerLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerLengthProperty = InnerLengthPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public int InnerLength
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(InnerLengthProperty));
                return Dispatcher.Invoke(() => InnerLength);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(InnerLengthPropertyKey, value);
                else
                    Dispatcher.Invoke(() => InnerLength = value);
            }
        }

        #endregion

        #region InnerElements Property Members

        private ObservableCollection<SsmlMarkupVM> _innerElements = new ObservableCollection<SsmlMarkupVM>();

        public const string PropertyName_InnerElements = "InnerElements";

        private static readonly DependencyPropertyKey InnerElementsPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_InnerElements, typeof(ReadOnlyObservableCollection<SsmlMarkupVM>), typeof(SsmlMarkupVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="InnerElements"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerElementsProperty = InnerElementsPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyObservableCollection<SsmlMarkupVM> InnerElements
        {
            get
            {
                if (CheckAccess())
                    return (ReadOnlyObservableCollection<SsmlMarkupVM>)(GetValue(InnerElementsProperty));
                return Dispatcher.Invoke(() => InnerElements);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(InnerElementsPropertyKey, value);
                else
                    Dispatcher.Invoke(() => InnerElements = value);
            }
        }

        #endregion

        public SsmlMarkupVM FindAtCharIndex(int charIndex)
        {
            if (!CheckAccess())
                return Dispatcher.Invoke(() => FindAtCharIndex(charIndex));

            if (charIndex < OuterStartIndex || charIndex >= (OuterStartIndex + OuterLength))
                return null;

            if (_innerElements.Count == 0 || charIndex < InnerStartIndex || charIndex >= (InnerStartIndex + InnerLength))
                return this;
            SsmlMarkupVM result = _innerElements.Select(e => e.FindAtCharIndex(charIndex)).FirstOrDefault(e => e != null);
            return result ?? this;
        }
        
        internal static IEnumerable<SsmlMarkupVM> Read(Dispatcher dispatcher, CancellationToken token, XmlReader xmlReader, Text.MultiLine lineIndexes, ref int lastLineNumber, ref int lastLinePosition)
        {
            throw new NotImplementedException();
        }
    }
}