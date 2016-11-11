using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    /// <summary>
    /// Represents a property value.
    /// </summary>
    public class PropertyValueVM : KeyValuePairVM<string, string>
    {
        #region IsNull Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="IsNull"/> dependency property.
        /// </summary>
        public const string PropertyName_IsNull = "IsNull";

        private static readonly DependencyPropertyKey IsNullPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsNull, typeof(bool), typeof(PropertyValueVM),
            new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsNull"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsNullProperty = IsNullPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public bool IsNull
        {
            get { return (bool)(GetValue(IsNullProperty)); }
            private set { SetValue(IsNullPropertyKey, value); }
        }

        #endregion

        #region ClassName Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="ClassName"/> dependency property.
        /// </summary>
        public const string PropertyName_ClassName = "ClassName";

        private static readonly DependencyPropertyKey ClassNamePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ClassName, typeof(string), typeof(PropertyValueVM),
            new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="ClassName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ClassNameProperty = ClassNamePropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string ClassName
        {
            get { return GetValue(ClassNameProperty) as string; }
            private set { SetValue(ClassNamePropertyKey, value); }
        }

        #endregion

        /// <summary>
        /// Initialize a new <see cref="PropertyValueVM"/> object.
        /// </summary>
        /// <param name="name">Name of property.</param>
        /// <param name="valueText">Text to represent the value of a property or null if the property value is null.</param>
        /// <param name="className">Name of class from which the property value originated.</param>
        /// <param name="isNull">true if the object is to represent a null value; otherwise false.</param>
        public PropertyValueVM(string name, string valueText, string className, bool isNull)
            : base(name, valueText ?? "")
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (className == null)
                throw new ArgumentNullException("className");

            if (name.Trim().Length == 0)
                throw new ArgumentException("Name cannot be empty.", "name");

            if (className.Trim().Length == 0)
                throw new ArgumentException("Class name cannot be empty.", "className");

            IsNull = isNull;
        }

        /// <summary>
        /// Initialize a new <see cref="PropertyValueVM"/> object.
        /// </summary>
        /// <param name="name">Name of property.</param>
        /// <param name="valueText">Text to represent the value of a property or null if the property value is null.</param>
        /// <param name="className">Name of class from which the property value originated.</param>
        public PropertyValueVM(string name, string valueText, string className) : this(name, valueText, className, valueText == null) { }

        /// <summary>
        /// Initialize a new <see cref="PropertyValueVM"/> object from a component property and custom value.
        /// </summary>
        /// <param name="component">Component object from which to retrieve the property value.</param>
        /// <param name="descriptor">Descriptor of property associated with value.</param>
        /// <param name="value">Value of property.</param>
        public PropertyValueVM(object component, PropertyDescriptor descriptor, object value)
            : base((descriptor == null) ? "" : ((String.IsNullOrWhiteSpace(descriptor.DisplayName)) ? descriptor.Name : descriptor.DisplayName), AsString(component, descriptor))
        {
            if (component == null)
                throw new ArgumentNullException("component");

            if (descriptor == null)
                throw new ArgumentNullException("descriptor");

            IsNull = value == null;
            ClassName = TypeDescriptor.GetClassName(component);
        }

        /// <summary>
        /// Initialize a new <see cref="PropertyValueVM"/> object from a component property.
        /// </summary>
        /// <param name="component">Component object from which to retrieve the property value.</param>
        /// <param name="descriptor">Descriptor of property to retrieve value from.</param>
        public PropertyValueVM(object component, PropertyDescriptor descriptor) : this(component, descriptor, (component == null || descriptor == null) ? null : descriptor.GetValue(component)) { }

        /// <summary>
        /// Create empty <see cref="PropertyValueVM"/> object.
        /// </summary>
        public PropertyValueVM() : base("", "") { }

        /// <summary>
        /// Create property value view model objects from a component object
        /// </summary>
        /// <param name="component">Component object from which to retrieve property values.</param>
        /// <returns>An enumerable collection of <see cref="PropertyValueVM"/> objects or an empty collection if <paramref cref="component"/> is null.</returns>
        public static IEnumerable<PropertyValueVM> FromComponent(object component)
        {
            if (component == null)
                return new PropertyValueVM[0];

            return FromComponent(TypeDescriptor.GetProperties(component), component);
        }

        /// <summary>
        /// Create specified property value view model objects from a component object
        /// </summary>
        /// <param name="component">Component object from which to retrieve property values.</param>
        /// <param name="propertyName">Name of initial property value to return.</param>
        /// <param name="additionalNames">Names of additional property values to return.</param>
        /// <returns>An enumerable collection of <see cref="PropertyValueVM"/> objects.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="component"/> or <paramref name="propertyName"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="propertyName"/> is empty.</exception>
        public static IEnumerable<PropertyValueVM> FromComponent(object component, string propertyName, params string[] additionalNames)
        {
            if (component == null)
                throw new ArgumentNullException("component");

            if (propertyName == null)
                throw new ArgumentNullException("propertyName");

            if (propertyName.Length == 0)
                throw new ArgumentException("Property name cannot be empty.", "propertyName");

            IEnumerable<string> allNames = new string[] { propertyName };
            if (additionalNames != null)
                allNames = allNames.Concat(additionalNames.Where(s => !String.IsNullOrEmpty(s))).Distinct();

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(component);
            return FromComponent(allNames.Select(n => properties.Find(n, true)), component);
        }

        /// <summary>
        /// Create specified property value view model objects from a component object
        /// </summary>
        /// <param name="component">Component object from which to retrieve property values.</param>
        /// <param name="descriptor">Descriptor of initial property value to return.</param>
        /// <param name="additionalDescriptors">Descriptors of additional property values to return.</param>
        /// <returns>An enumerable collection of <see cref="PropertyValueVM"/> objects.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="component"/> or <paramref name="descriptor"/> is null.</exception>
        public static IEnumerable<PropertyValueVM> FromComponent(object component, PropertyDescriptor descriptor, params PropertyDescriptor[] additionalDescriptors)
        {
            if (component == null)
                throw new ArgumentNullException("component");

            if (descriptor == null)
                throw new ArgumentNullException("descriptor");

            if (additionalDescriptors == null)
                return FromComponent((new PropertyDescriptor[] { descriptor }));

            return FromComponent((new PropertyDescriptor[] { descriptor }).Concat(additionalDescriptors).Distinct());
        }

        private static IEnumerable<PropertyValueVM> FromComponent(PropertyDescriptorCollection collection, object component)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            return FromComponent(collection.OfType<PropertyDescriptor>(), component);
        }

        private static IEnumerable<PropertyValueVM> FromComponent(IEnumerable<PropertyDescriptor> collection, object component)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (component == null)
                throw new ArgumentNullException("component");

            return collection.Select(p => new PropertyValueVM(component, p));
        }

        private static string AsString(object obj, PropertyDescriptor descriptor)
        {
            if (obj == null || (obj = descriptor.GetValue(obj)) == null)
                return "";
            
            if (obj is string)
                return obj as string;

            if (descriptor != null && descriptor.Converter != null && descriptor.Converter.CanConvertTo(typeof(string)))
                return descriptor.Converter.ConvertToInvariantString(obj);

            return obj.ToString();
        }
    }
}
