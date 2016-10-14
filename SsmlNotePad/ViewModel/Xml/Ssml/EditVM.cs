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
    public class EditVM : DependencyObject
    {
        public EditVM()
        {
            TextBox textBox = new TextBox();
            textBox.AcceptsReturn = true;
            textBox.AcceptsTab = true;
            textBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            textBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            textBox.TextWrapping = TextWrapping.Wrap;
            textBox.LayoutUpdated += TextContentControl_LayoutUpdated;
            textBox.SelectionChanged += TextContentControl_SelectionChanged;
            textBox.TextChanged += TextContentControl_TextChanged;
            textBox.SizeChanged += TextContentControl_SizeChanged;
            TextContentControl = textBox;
            TextContentStyle = textBox.Style;
            TextWrapping = textBox.TextWrapping;
            CaretIndex = textBox.CaretIndex;
            SelectionStart = textBox.SelectionStart;
            SelectionLength = textBox.SelectionLength;
            MarkupInfo = new ReadOnlyObservableCollection<Ssml.SsmlMarkupVM>(_markupInfo);
        }

        private void UpdateOffsets()
        {
            TextBox textBox = TextContentControl;
            HorizontalOffset = textBox.HorizontalOffset;
            VerticalOffset = textBox.VerticalOffset;
            ExtentWidth = textBox.ExtentWidth;
            ExtentHeight = textBox.ExtentHeight;
        }

        private void TextContentControl_SizeChanged(object sender, SizeChangedEventArgs e) { UpdateOffsets(); }

        private void TextContentControl_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = TextContentControl;
            SelectionStart = textBox.SelectionStart;
            SelectionLength = textBox.SelectionLength;
            UpdateSsmlMarkupXml();
        }

        private void TextContentControl_SelectionChanged(object sender, RoutedEventArgs e)
        {
            TextBox textBox = TextContentControl;
            SelectionStart = textBox.SelectionStart;
            SelectionLength = textBox.SelectionLength;
        }

        private void TextContentControl_LayoutUpdated(object sender, EventArgs e)
        {
            FirstVisibleLineIndex = TextContentControl.GetFirstVisibleLineIndex();
            LastVisibleLineIndex = TextContentControl.GetLastVisibleLineIndex();
            UpdateOffsets();
        }

        #region TextContentStyle Property Members

        public const string DependencyPropertyName_TextContentStyle = "TextContentStyle";

        /// <summary>
        /// Identifies the <seealso cref="TextContentStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextContentStyleProperty = DependencyProperty.Register(DependencyPropertyName_TextContentStyle, typeof(Style), typeof(EditVM),
                new PropertyMetadata(null,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as EditVM).TextContentStyle_PropertyChanged((Style)(e.OldValue), (Style)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as EditVM).TextContentStyle_PropertyChanged((Style)(e.OldValue), (Style)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as EditVM).TextContentStyle_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public Style TextContentStyle
        {
            get
            {
                if (CheckAccess())
                    return (Style)(GetValue(TextContentStyleProperty));
                return Dispatcher.Invoke(() => TextContentStyle);
            }
            set
            {
                if (CheckAccess())
                    SetValue(TextContentStyleProperty, value);
                else
                    Dispatcher.Invoke(() => TextContentStyle = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="TextContentStyle"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="Style"/> value before the <seealso cref="TextContentStyle"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="Style"/> value after the <seealso cref="TextContentStyle"/> property was changed.</param>
        protected virtual void TextContentStyle_PropertyChanged(Style oldValue, Style newValue)
        {
            if ((newValue == null) ? TextContentControl.Style != null : TextContentControl.Style == null || !ReferenceEquals(TextContentControl.Style, newValue))
                TextContentControl.Style = newValue;
        }

        /// <summary>
        /// This gets called whenever <seealso cref="TextContentStyle"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual Style TextContentStyle_CoerceValue(object baseValue)
        {
            Style style = baseValue as Style;
            if (style == null || (style.TargetType != null && !style.TargetType.IsAssignableFrom(typeof(TextBox))))
                return TextContentControl.Style;
            Setter[] toRemove = style.Setters.OfType<Setter>().Where(s => ReferenceEquals(s.Property, TextBox.TextProperty) || ReferenceEquals(s.Property, TextBox.AcceptsReturnProperty) ||
                ReferenceEquals(s.Property, TextBox.AcceptsTabProperty) || ReferenceEquals(s.Property, TextBox.HorizontalScrollBarVisibilityProperty) ||
                ReferenceEquals(s.Property, TextBox.VerticalScrollBarVisibilityProperty) || ReferenceEquals(s.Property, TextBox.TextWrappingProperty)).ToArray();
            foreach (Setter s in toRemove)
                style.Setters.Remove(s);
            return style;
        }

        #endregion

        #region FirstVisibleLineIndex Property Members

        public const string PropertyName_FirstVisibleLineIndex = "FirstVisibleLineIndex";

        private static readonly DependencyPropertyKey FirstVisibleLineIndexPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_FirstVisibleLineIndex, typeof(int), typeof(EditVM),
                new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <seealso cref="FirstVisibleLineIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FirstVisibleLineIndexProperty = FirstVisibleLineIndexPropertyKey.DependencyProperty;

        /// <summary>
        /// The zero-based index for the first visible line in the text box.
        /// </summary>
        public int FirstVisibleLineIndex
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(FirstVisibleLineIndexProperty));
                return Dispatcher.Invoke(() => FirstVisibleLineIndex);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(FirstVisibleLineIndexPropertyKey, value);
                else
                    Dispatcher.Invoke(() => FirstVisibleLineIndex = value);
            }
        }

        #endregion

        #region LastVisibleLineIndex Property Members

        public const string PropertyName_LastVisibleLineIndex = "LastVisibleLineIndex";

        private static readonly DependencyPropertyKey LastVisibleLineIndexPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_LastVisibleLineIndex, typeof(int), typeof(EditVM),
                new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <seealso cref="LastVisibleLineIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LastVisibleLineIndexProperty = LastVisibleLineIndexPropertyKey.DependencyProperty;

        /// <summary>
        /// The zero-based index for the first visible line in the text box.
        /// </summary>
        public int LastVisibleLineIndex
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(LastVisibleLineIndexProperty));
                return Dispatcher.Invoke(() => LastVisibleLineIndex);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(LastVisibleLineIndexPropertyKey, value);
                else
                    Dispatcher.Invoke(() => LastVisibleLineIndex = value);
            }
        }

        #endregion

        #region ExtentWidth Property Members

        public const string PropertyName_ExtentWidth = "ExtentWidth";

        private static readonly DependencyPropertyKey ExtentWidthPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ExtentWidth, typeof(double), typeof(EditVM),
            new PropertyMetadata(0.0));

        /// <summary>
        /// Identifies the <seealso cref="ExtentWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExtentWidthProperty = ExtentWidthPropertyKey.DependencyProperty;

        /// <summary>
        /// A floating-point value that specifies the horizontal size of the visible content area, in device-independent units (1/96th inch per unit).
        /// </summary>
        public double ExtentWidth
        {
            get
            {
                if (CheckAccess())
                    return (double)(GetValue(ExtentWidthProperty));
                return Dispatcher.Invoke(() => ExtentWidth);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(ExtentWidthPropertyKey, value);
                else
                    Dispatcher.Invoke(() => ExtentWidth = value);
            }
        }

        #endregion

        #region ExtentHeight Property Members

        public const string PropertyName_ExtentHeight = "ExtentHeight";

        private static readonly DependencyPropertyKey ExtentHeightPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ExtentHeight, typeof(double), typeof(EditVM),
            new PropertyMetadata(0.0));

        /// <summary>
        /// Identifies the <seealso cref="ExtentHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExtentHeightProperty = ExtentHeightPropertyKey.DependencyProperty;

        /// <summary>
        /// A floating-point value that specifies the vertical size of the visible content area, in device-independent units (1/96th inch per unit).
        /// </summary>
        public double ExtentHeight
        {
            get
            {
                if (CheckAccess())
                    return (double)(GetValue(ExtentHeightProperty));
                return Dispatcher.Invoke(() => ExtentHeight);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(ExtentHeightPropertyKey, value);
                else
                    Dispatcher.Invoke(() => ExtentHeight = value);
            }
        }

        #endregion

        #region CaretIndex Property Members

        public const string DependencyPropertyName_CaretIndex = "CaretIndex";

        /// <summary>
        /// Identifies the <seealso cref="CaretIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CaretIndexProperty = DependencyProperty.Register(DependencyPropertyName_CaretIndex, typeof(int), typeof(EditVM),
                new PropertyMetadata(0,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as EditVM).CaretIndex_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as EditVM).CaretIndex_PropertyChanged((int)(e.OldValue), (int)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as EditVM).CaretIndex_CoerceValue(baseValue)));

        /// <summary>
        /// The zero-based insertion position index of the caret.
        /// </summary>
        public int CaretIndex
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(CaretIndexProperty));
                return Dispatcher.Invoke(() => CaretIndex);
            }
            set
            {
                if (CheckAccess())
                    SetValue(CaretIndexProperty, value);
                else
                    Dispatcher.Invoke(() => CaretIndex = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="CaretIndex"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="int"/> value before the <seealso cref="CaretIndex"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="int"/> value after the <seealso cref="CaretIndex"/> property was changed.</param>
        protected virtual void CaretIndex_PropertyChanged(int oldValue, int newValue)
        {
            // TODO: Implement EditVM.CaretIndex_PropertyChanged(int, int)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="CaretIndex"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual int CaretIndex_CoerceValue(object baseValue)
        {
            // TODO: Implement EditVM.CaretIndex_CoerceValue(DependencyObject, object)
            return (int)baseValue;
        }

        #endregion

        #region HorizontalOffset Property Members

        public const string DependencyPropertyName_HorizontalOffset = "HorizontalOffset";

        /// <summary>
        /// Identifies the <seealso cref="HorizontalOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.Register(DependencyPropertyName_HorizontalOffset, typeof(double), typeof(EditVM),
                new PropertyMetadata(0.0,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as EditVM).HorizontalOffset_PropertyChanged((double)(e.OldValue), (double)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as EditVM).HorizontalOffset_PropertyChanged((double)(e.OldValue), (double)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as EditVM).HorizontalOffset_CoerceValue(baseValue)));

        /// <summary>
        /// A floating-point value that specifies the horizontal scroll position, in device-independent units (1/96th inch per unit).
        /// </summary>
        public double HorizontalOffset
        {
            get
            {
                if (CheckAccess())
                    return (double)(GetValue(HorizontalOffsetProperty));
                return Dispatcher.Invoke(() => HorizontalOffset);
            }
            set
            {
                if (CheckAccess())
                    SetValue(HorizontalOffsetProperty, value);
                else
                    Dispatcher.Invoke(() => HorizontalOffset = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="HorizontalOffset"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="double"/> value before the <seealso cref="HorizontalOffset"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="double"/> value after the <seealso cref="HorizontalOffset"/> property was changed.</param>
        protected virtual void HorizontalOffset_PropertyChanged(double oldValue, double newValue)
        {
            // TODO: Implement EditVM.HorizontalOffset_PropertyChanged(double, double)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="HorizontalOffset"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual double HorizontalOffset_CoerceValue(object baseValue)
        {
            // TODO: Implement EditVM.HorizontalOffset_CoerceValue(DependencyObject, object)
            return (double)baseValue;
        }

        #endregion

        #region VerticalOffset Property Members

        public const string DependencyPropertyName_VerticalOffset = "VerticalOffset";

        /// <summary>
        /// Identifies the <seealso cref="VerticalOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.Register(DependencyPropertyName_VerticalOffset, typeof(double), typeof(EditVM),
                new PropertyMetadata(0.0,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as EditVM).VerticalOffset_PropertyChanged((double)(e.OldValue), (double)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as EditVM).VerticalOffset_PropertyChanged((double)(e.OldValue), (double)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as EditVM).VerticalOffset_CoerceValue(baseValue)));

        /// <summary>
        /// A floating-point value that specifies the vertical scroll position, in device-independent
        //     units (1/96th inch per unit).
        /// </summary>
        public double VerticalOffset
        {
            get
            {
                if (CheckAccess())
                    return (double)(GetValue(VerticalOffsetProperty));
                return Dispatcher.Invoke(() => VerticalOffset);
            }
            set
            {
                if (CheckAccess())
                    SetValue(VerticalOffsetProperty, value);
                else
                    Dispatcher.Invoke(() => VerticalOffset = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="VerticalOffset"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="double"/> value before the <seealso cref="VerticalOffset"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="double"/> value after the <seealso cref="VerticalOffset"/> property was changed.</param>
        protected virtual void VerticalOffset_PropertyChanged(double oldValue, double newValue)
        {
            // TODO: Implement EditVM.VerticalOffset_PropertyChanged(double, double)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="VerticalOffset"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual double VerticalOffset_CoerceValue(object baseValue)
        {
            // TODO: Implement EditVM.VerticalOffset_CoerceValue(DependencyObject, object)
            return (double)baseValue;
        }

        #endregion

        #region SelectionStart Property Members

        public const string DependencyPropertyName_SelectionStart = "SelectionStart";

        /// <summary>
        /// Identifies the <seealso cref="SelectionStart"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionStartProperty = DependencyProperty.Register(DependencyPropertyName_SelectionStart, typeof(int), typeof(EditVM),
                new PropertyMetadata(0,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as EditVM).SelectionStart_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as EditVM).SelectionStart_PropertyChanged((int)(e.OldValue), (int)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as EditVM).SelectionStart_CoerceValue(baseValue)));

        /// <summary>
        /// The character index for the beginning of the current selection.
        /// </summary>
        public int SelectionStart
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(SelectionStartProperty));
                return Dispatcher.Invoke(() => SelectionStart);
            }
            set
            {
                if (CheckAccess())
                    SetValue(SelectionStartProperty, value);
                else
                    Dispatcher.Invoke(() => SelectionStart = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="SelectionStart"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="int"/> value before the <seealso cref="SelectionStart"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="int"/> value after the <seealso cref="SelectionStart"/> property was changed.</param>
        protected virtual void SelectionStart_PropertyChanged(int oldValue, int newValue)
        {
            // TODO: Implement EditVM.SelectionStart_PropertyChanged(int, int)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="SelectionStart"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual int SelectionStart_CoerceValue(object baseValue)
        {
            // TODO: Implement EditVM.SelectionStart_CoerceValue(DependencyObject, object)
            return (int)baseValue;
        }

        #endregion

        #region SelectionLength Property Members

        public const string DependencyPropertyName_SelectionLength = "SelectionLength";

        /// <summary>
        /// Identifies the <seealso cref="SelectionLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionLengthProperty = DependencyProperty.Register(DependencyPropertyName_SelectionLength, typeof(int), typeof(EditVM),
                new PropertyMetadata(0,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as EditVM).SelectionLength_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as EditVM).SelectionLength_PropertyChanged((int)(e.OldValue), (int)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as EditVM).SelectionLength_CoerceValue(baseValue)));

        /// <summary>
        /// The number of characters in the current selection in the text box. The default is 0.
        /// </summary>
        public int SelectionLength
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(SelectionLengthProperty));
                return Dispatcher.Invoke(() => SelectionLength);
            }
            set
            {
                if (CheckAccess())
                    SetValue(SelectionLengthProperty, value);
                else
                    Dispatcher.Invoke(() => SelectionLength = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="SelectionLength"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="int"/> value before the <seealso cref="SelectionLength"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="int"/> value after the <seealso cref="SelectionLength"/> property was changed.</param>
        protected virtual void SelectionLength_PropertyChanged(int oldValue, int newValue)
        {
            // TODO: Implement EditVM.SelectionLength_PropertyChanged(int, int)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="SelectionLength"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual int SelectionLength_CoerceValue(object baseValue)
        {
            // TODO: Implement EditVM.SelectionLength_CoerceValue(DependencyObject, object)
            return (int)baseValue;
        }

        #endregion

        #region TextWrapping Property Members

        public const string DependencyPropertyName_TextWrapping = "TextWrapping";

        /// <summary>
        /// Identifies the <seealso cref="TextWrapping"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register(DependencyPropertyName_TextWrapping, typeof(TextWrapping), typeof(EditVM),
                new PropertyMetadata(TextWrapping.Wrap,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as EditVM).TextWrapping_PropertyChanged((TextWrapping)(e.OldValue), (TextWrapping)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as EditVM).TextWrapping_PropertyChanged((TextWrapping)(e.OldValue), (TextWrapping)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as EditVM).TextWrapping_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public TextWrapping TextWrapping
        {
            get
            {
                if (CheckAccess())
                    return (TextWrapping)(GetValue(TextWrappingProperty));
                return Dispatcher.Invoke(() => TextWrapping);
            }
            set
            {
                if (CheckAccess())
                    SetValue(TextWrappingProperty, value);
                else
                    Dispatcher.Invoke(() => TextWrapping = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="TextWrapping"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="TextWrapping"/> value before the <seealso cref="TextWrapping"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="TextWrapping"/> value after the <seealso cref="TextWrapping"/> property was changed.</param>
        protected virtual void TextWrapping_PropertyChanged(TextWrapping oldValue, TextWrapping newValue)
        {
            if (TextContentControl.TextWrapping != newValue)
                TextContentControl.TextWrapping = newValue;

            TextContentControl.HorizontalScrollBarVisibility = (newValue == TextWrapping.NoWrap) ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled;
        }

        /// <summary>
        /// This gets called whenever <seealso cref="TextWrapping"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual TextWrapping TextWrapping_CoerceValue(object baseValue)
        {
            TextWrapping? textWrapping = baseValue as TextWrapping?;
            if (textWrapping.HasValue && textWrapping.Value != TextWrapping.WrapWithOverflow)
                return textWrapping.Value;

            return TextWrapping.Wrap;
        }

        #endregion

        #region TextContentControl Property Members

        public const string PropertyName_TextContentControl = "TextContentControl";

        private static readonly DependencyPropertyKey TextContentControlPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_TextContentControl, typeof(TextBox), typeof(EditVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="TextContentControl"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextContentControlProperty = TextContentControlPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public TextBox TextContentControl
        {
            get
            {
                if (CheckAccess())
                    return (TextBox)(GetValue(TextContentControlProperty));
                return Dispatcher.Invoke(() => TextContentControl);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(TextContentControlPropertyKey, value);
                else
                    Dispatcher.Invoke(() => TextContentControl = value);
            }
        }

        #endregion

        #region OpenSpeakTagContent Property Members

        public const string DependencyPropertyName_OpenSpeakTagContent = "OpenSpeakTagContent";

        /// <summary>
        /// Identifies the <seealso cref="OpenSpeakTagContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OpenSpeakTagContentProperty = DependencyProperty.Register(DependencyPropertyName_OpenSpeakTagContent, typeof(string), typeof(EditVM),
                new PropertyMetadata("xml:lang=\"en-US\"",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as EditVM).OpenSpeakTagContent_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as EditVM).OpenSpeakTagContent_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as EditVM).OpenSpeakTagContent_CoerceValue(baseValue)));

        /// <summary>
        /// Extra attributes in the open 'speak' tag 
        /// </summary>
        public string OpenSpeakTagContent
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(OpenSpeakTagContentProperty));
                return Dispatcher.Invoke(() => OpenSpeakTagContent);
            }
            set
            {
                if (CheckAccess())
                    SetValue(OpenSpeakTagContentProperty, value);
                else
                    Dispatcher.Invoke(() => OpenSpeakTagContent = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="OpenSpeakTagContent"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="OpenSpeakTagContent"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="OpenSpeakTagContent"/> property was changed.</param>
        protected virtual void OpenSpeakTagContent_PropertyChanged(string oldValue, string newValue) { UpdateSsmlMarkupXml(); }

        /// <summary>
        /// This gets called whenever <seealso cref="OpenSpeakTagContent"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string OpenSpeakTagContent_CoerceValue(object baseValue)
        {
            string s = baseValue as string;
            if (s == null || s.Length == 0)
                return "";

            return Common.XmlHelper.NormalizeNewLines(s);
        }

        #endregion
        
        #region Xml validation members

        private void UpdateSsmlMarkupXml()
        {
            lock (_tokenSourceSyncRoot)
            {
                if (_generateLineNumbersTokenSource != null && !_generateLineNumbersTokenSource.IsCancellationRequested)
                    _generateLineNumbersTokenSource.Cancel();
                if (_updateSsmlMarkupTokenSource != null && !_updateSsmlMarkupTokenSource.IsCancellationRequested)
                    _updateSsmlMarkupTokenSource.Cancel();
                IsValidating = true;
                _updateSsmlMarkupTokenSource = new CancellationTokenSource();
                _generateLineNumbersTokenSource = new CancellationTokenSource();
                _getLineInfoTask = Task<Text.MultiLine>.Factory.StartNew(o =>
                {
                    CancellationToken token = (CancellationToken)o;
                    if (token.IsCancellationRequested)
                        return new Text.MultiLine();

                    string openSpeakTagContent = OpenSpeakTagContent;
                    string xml = Dispatcher.Invoke(() => TextContentControl.Text);
                    if (openSpeakTagContent.Length > 0 && !Char.IsWhiteSpace(openSpeakTagContent[0]))
                        openSpeakTagContent = " " + openSpeakTagContent;
                    xml = String.Format("<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\"{0}>{1}</speak>", openSpeakTagContent, xml);
                    return (token.IsCancellationRequested) ? new Text.MultiLine() : new Text.MultiLine(xml);
                }, _updateSsmlMarkupTokenSource.Token, _updateSsmlMarkupTokenSource.Token);
                _parseTagIndexesTask = _getLineInfoTask.ContinueWith(ValidateXml, _updateSsmlMarkupTokenSource.Token, _updateSsmlMarkupTokenSource.Token);
                _generateLineNumbersTask = _getLineInfoTask.ContinueWith(GenerateLineNumbers, _generateLineNumbersTokenSource.Token, _generateLineNumbersTokenSource.Token);
                Task.Factory.StartNew(o =>
                {
                    object[] args = o as object[];
                    Task task = args[0] as Task;
                    try
                    {
                        if (!task.IsCompleted)
                            task.Wait();
                    }
                    catch { }
                    lock (_tokenSourceSyncRoot)
                    {
                        if (ReferenceEquals(_updateSsmlMarkupTokenSource, args[2]))
                        {
                            _updateSsmlMarkupTokenSource = null;
                            IsValidating = false;
                        }
                    }
                    (args[2] as CancellationTokenSource).Dispose();
                    task = args[1] as Task;
                    try
                    {
                        if (!task.IsCompleted)
                            task.Wait();
                    }
                    catch { }
                    lock (_tokenSourceSyncRoot)
                    {
                        if (ReferenceEquals(_generateLineNumbersTokenSource, args[3]))
                            _generateLineNumbersTokenSource = null;
                    }
                    (args[3] as CancellationTokenSource).Dispose();
                }, new object[] { _parseTagIndexesTask, _generateLineNumbersTask, _updateSsmlMarkupTokenSource, _generateLineNumbersTokenSource });
            }
        }

        Task<Text.MultiLine> _getLineInfoTask = Task.FromResult(new Text.MultiLine());
        Task _parseTagIndexesTask = null, _generateLineNumbersTask = null;
        private CancellationTokenSource _updateSsmlMarkupTokenSource = null;
        private CancellationTokenSource _generateLineNumbersTokenSource = null;
        private object _tokenSourceSyncRoot = new object();

        public void WaitForTasks()
        {
            if (_parseTagIndexesTask != null)
                _parseTagIndexesTask.Wait();
        }

        class ValidationResults : List<XmlValidationMessageVM>
        {
            private Dispatcher _dispatcher;
            private Text.MultiLine _lines;

            public ValidationResults(Text.MultiLine lines) { _lines = lines; }

            public ValidationResults(Dispatcher dispatcher, Text.MultiLine lineIndexes)
            {
                this._dispatcher = dispatcher;
                this._lines = lineIndexes;
            }

            public void SchemaSet_ValidationEventHandler(object sender, ValidationEventArgs e)
            {
                int count = _lines.Count;
                if (e.Exception.LineNumber < 1 || e.Exception.LineNumber > count)
                    _dispatcher.Invoke(() => Add(new XmlValidationMessageVM(e.Message, e.Exception, e.Severity, null)));
                else
                {
                    int startIndex = e.Exception.LineNumber - 3;
                    if (startIndex < 0)
                        startIndex = 0;
                    int endIndex = e.Exception.LineNumber + 1;
                    if (endIndex > count)
                        endIndex = count;
                    _dispatcher.Invoke(() => Add(new XmlValidationMessageVM(e.Message, e.Exception, e.Severity, new StringLines(_lines.Skip(startIndex).Take(endIndex - startIndex).ToArray()))));
                }
            }

            internal void Add(XmlSchemaException exception)
            {
                int count = _lines.Count;
                if (exception.LineNumber < 1 || exception.LineNumber > count)
                    _dispatcher.Invoke(() => Add(new XmlValidationMessageVM(exception.Message, exception)));
                else
                {
                    int startIndex = exception.LineNumber - 3;
                    if (startIndex < 0)
                        startIndex = 0;
                    int endIndex = exception.LineNumber + 1;
                    if (endIndex > count)
                        endIndex = count;
                    _dispatcher.Invoke(() => Add(new XmlValidationMessageVM(exception.Message, exception, new StringLines(_lines.Skip(startIndex).Take(endIndex - startIndex).ToArray()))));
                }
            }

            internal void Add(XmlException exception)
            {
                int count = _lines.Count;
                if (exception.LineNumber < 1 || exception.LineNumber > count)
                    _dispatcher.Invoke(() => Add(new XmlValidationMessageVM(exception.Message, exception)));
                else
                {
                    int startIndex = exception.LineNumber - 3;
                    if (startIndex < 0)
                        startIndex = 0;
                    int endIndex = exception.LineNumber + 1;
                    if (endIndex > count)
                        endIndex = count;
                    _dispatcher.Invoke(() => Add(new XmlValidationMessageVM(exception.Message, exception, new StringLines(_lines.Skip(startIndex).Take(endIndex - startIndex).ToArray()))));
                }
            }

            internal void Add(Exception exception, int lastLineNumber, int lastLinePosition)
            {
                int count = _lines.Count;
                if (lastLineNumber < 1 || lastLineNumber > count)
                    _dispatcher.Invoke(() => Add(new XmlValidationMessageVM(exception.Message, exception)));
                else
                {
                    int startIndex = lastLineNumber - 3;
                    if (startIndex < 0)
                        startIndex = 0;
                    int endIndex = lastLineNumber + 1;
                    if (endIndex > count)
                        endIndex = count;
                    _dispatcher.Invoke(() => Add(new XmlValidationMessageVM(exception.Message, exception, lastLineNumber, lastLinePosition, new StringLines(_lines.Skip(startIndex).Take(endIndex - startIndex).ToArray()))));
                }
            }
        }

        //class ValidationError
        //{
        //    public Exception Exception { get; private set; }
        //    public string Message { get; private set; }
        //    public XmlSeverityType Severity { get; private set; }
        //    public int LineNumber { get; private set; }
        //    public int LinePosition { get; private set; }

        //    public ValidationError(XmlSchemaException exception, XmlSeverityType severity = XmlSeverityType.Error, string message = null)
        //    {
        //        if (exception == null)
        //            Initialize(exception, 0, 0, severity, message);
        //        else
        //            Initialize(exception, exception.LineNumber, exception.LinePosition, severity, message);
        //    }

        //    public ValidationError(XmlException exception, XmlSeverityType severity = XmlSeverityType.Error, string message = null)
        //    {
        //        if (exception == null)
        //            Initialize(exception, 0, 0, severity, message);
        //        else
        //            Initialize(exception, exception.LineNumber, exception.LinePosition, severity, message);
        //    }

        //    public ValidationError(Exception exception, int lineNumber = -1, int linePosition = -1, XmlSeverityType severity = XmlSeverityType.Error, string message = null)
        //    {
        //        Initialize(exception, lineNumber, linePosition, severity, message);
        //    }

        //    private void Initialize(Exception exception, int lineNumber, int linePosition, XmlSeverityType severity, string message)
        //    {
        //        Severity = severity;
        //        Exception = exception;
        //        LineNumber = lineNumber;
        //        LinePosition = linePosition;
        //        Message = (String.IsNullOrWhiteSpace(message)) ? ((exception == null) ? severity.ToString("F") : ((String.IsNullOrWhiteSpace(exception.Message)) ?
        //            String.Format("{0} {1}", severity.ToString("F"), exception.GetType().Name) : exception.Message)) : message;
        //    }
        //}

        private void GenerateLineNumbers(Task<Text.MultiLine> task, object state)
        {
            // TODO: Implement this
        }

        private void ValidateXml(Task<Text.MultiLine> task, object state)
        {
            CancellationToken token = (CancellationToken)state;

            if (token.IsCancellationRequested)
                return;

            if (!task.IsCompleted)
                task.Wait();

            if (token.IsCancellationRequested)
                return;

            Text.MultiLine lineIndexes = task.Result;
            string xml = lineIndexes.ToString();

            ValidationResults validationResults = new ValidationResults(Dispatcher, lineIndexes);
            int lastLineNumber = 0;
            int lastLinePosition = 0;
            Collection<SsmlMarkupVM> results = new Collection<SsmlMarkupVM>();
            try
            {
                using (StringReader stringReader = new StringReader(xml))
                {
                    XmlSchemaSet schemaSet = Common.XmlHelper.CreateSsmlSchemaSet();
                    schemaSet.ValidationEventHandler += validationResults.SchemaSet_ValidationEventHandler;
                    XmlReaderSettings settings = new XmlReaderSettings
                    {
                        CheckCharacters = false,
                        DtdProcessing = DtdProcessing.Ignore,
                        ValidationType = ValidationType.Schema,
                        Schemas = schemaSet,
                        CloseInput = false
                    };

                    if (token.IsCancellationRequested)
                        return;

                    settings.ValidationEventHandler += validationResults.SchemaSet_ValidationEventHandler;
                    using (XmlReader xmlReader = XmlReader.Create(stringReader, settings, App.AppSettingsViewModel.BaseUriPath))
                    {
                        IXmlLineInfo lineInfo = xmlReader as IXmlLineInfo;
                        xmlReader.Read();
                        lastLineNumber = lineInfo.LineNumber;
                        lastLinePosition = lineInfo.LinePosition;
                        SsmlMarkupVM current;
                        throw new NotImplementedException();
                        //while ((current = SsmlMarkupVM.Create(Dispatcher, token, xmlReader, lineIndexes, ref lastLineNumber, ref lastLinePosition)) != null)
                        //{
                        //    if (token.IsCancellationRequested)
                        //        return;
                        //    results.Add(current);
                        //}
                    }
                }
            }
            catch (XmlSchemaException exc) { validationResults.Add(exc); }
            catch (XmlException exc) { validationResults.Add(exc); }
            catch (Exception exception) { validationResults.Add(exception, lastLineNumber, lastLinePosition); }

            if (token.IsCancellationRequested)
                return;

            Dispatcher.Invoke(() =>
            {
                _markupInfo.Clear();
                foreach (SsmlMarkupVM item in results)
                    _markupInfo.Add(item);
            });
            // TODO: Populate results and validationResults
        }

        #endregion

        public SsmlMarkupVM FindElementAtCharIndex(int charIndex)
        {
            if (!CheckAccess())
                return Dispatcher.Invoke(() => FindElementAtCharIndex(charIndex));
            
            return _markupInfo.Select(e => e.FindAtCharIndex(charIndex)).FirstOrDefault(e => e != null);
        }

        #region IsValidating Property Members

        public const string PropertyName_IsValidating = "IsValidating";

        private static readonly DependencyPropertyKey IsValidatingPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsValidating, typeof(bool), typeof(EditVM),
                new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <seealso cref="IsValidating"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsValidatingProperty = IsValidatingPropertyKey.DependencyProperty;

        /// <summary>
        /// This will be true while an XML change is being validated in the background.
        /// </summary>
        public bool IsValidating
        {
            get
            {
                if (CheckAccess())
                    return (bool)(GetValue(IsValidatingProperty));
                return Dispatcher.Invoke(() => IsValidating);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(IsValidatingPropertyKey, value);
                else
                    Dispatcher.Invoke(() => IsValidating = value);
            }
        }

        #endregion

        #region MarkupInfo Property Members

        private ObservableCollection<SsmlMarkupVM> _markupInfo = new ObservableCollection<SsmlMarkupVM>();

        public const string PropertyName_MarkupInfo = "MarkupInfo";

        private static readonly DependencyPropertyKey MarkupInfoPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_MarkupInfo, typeof(ReadOnlyObservableCollection<SsmlMarkupVM>), typeof(EditVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="MarkupInfo"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MarkupInfoProperty = MarkupInfoPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyObservableCollection<SsmlMarkupVM> MarkupInfo
        {
            get
            {
                if (CheckAccess())
                    return (ReadOnlyObservableCollection<SsmlMarkupVM>)(GetValue(MarkupInfoProperty));
                return Dispatcher.Invoke(() => MarkupInfo);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(MarkupInfoPropertyKey, value);
                else
                    Dispatcher.Invoke(() => MarkupInfo = value);
            }
        }

        #endregion

    }
}
