using System;
using System.Collections;
using System.ComponentModel;


namespace AdcTools
{
    ///////////////////////////////////////////////////////////////////////
    // Un wrapper pour mettre un dictionnaire dans une PropertyGrid.
    ///////////////////////////////////////////////////////////////////////
    public class DictionaryPropertyGridAdapter : ICustomTypeDescriptor
    {
        private IDictionary _dictionary;

        public DictionaryPropertyGridAdapter(IDictionary d)
        {
            _dictionary = d;
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }

        PropertyDescriptorCollection
            System.ComponentModel.ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(null);
        }

        private PropertyDescriptorCollection _propertyDescriptorCollection = null;
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            if (_propertyDescriptorCollection == null)
            {
                ArrayList properties = new ArrayList();
                foreach (DictionaryEntry e in _dictionary)
                    properties.Add(new DictionaryPropertyDescriptor(_dictionary, e.Key));

                PropertyDescriptor[] props =
                    (PropertyDescriptor[])properties.ToArray(typeof(PropertyDescriptor));

                _propertyDescriptorCollection = new PropertyDescriptorCollection(props);
            }
            return _propertyDescriptorCollection;
        }

        public override string ToString()
        {
            return "Count=" + _dictionary.Count;
        }
    }

    ///////////////////////////////////////////////////////////////////////
    // Propriété custom du DictionaryPropertyGridAdapter
    ///////////////////////////////////////////////////////////////////////
    internal class DictionaryPropertyDescriptor : PropertyDescriptor
    {
        private IDictionary _dictionary;
        private object _key;

        internal DictionaryPropertyDescriptor(IDictionary d, object key)
            : base(key.ToString(), null)
        {
            _dictionary = d;
            _key = key;
        }

        public override Type PropertyType
        {
            get { return _dictionary[_key].GetType(); }
        }

        public override void SetValue(object component, object value)
        {
            _dictionary[_key] = value;
        }

        public override object GetValue(object component)
        {
            return _dictionary[_key];
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type ComponentType
        {
            get { return null; }
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override void ResetValue(object component)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override string ToString()
        {
            return _dictionary[_key].ToString();
        }
    }
}
