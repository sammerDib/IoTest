using System;

namespace DeepLearningSoft48.Modules.Parameters
{
    /// <summary>
    /// Generic representation of a parameter of a module.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ParameterTemplate<T> : ParameterBase
    {
        //=================================================================
        // Value and Type Properties
        //=================================================================

        private Type _type = null;
        public Type Type
        {
            get
            {
                if (_type == null)
                    _type = _value.GetType();
                return _type;
            }
            protected set
            {
                _type = value;
            }
        }

        protected T _value;
        public virtual T Value
        {
            get
            {
                return _value;
            }
            set
            {
                T oldvalue = _value;
                if (oldvalue.Equals(value) == false)
                {
                    _value = value;

                    if (_value.Equals(oldvalue) == false)
                    {
                        if (ValueChanged != null)
                            ValueChanged(_value);
                        OnPropertyChanged();
                    }
                }
            }
        }

        //=================================================================
        // Constructor
        //=================================================================
        public ParameterTemplate(ModuleBase module, string name)
            : base(module, name)
        {
        }

        //=================================================================
        // Callbacks for ValueChanged
        //=================================================================
        public delegate void ValueChangedEventHandler(T t);
        public event ValueChangedEventHandler ValueChanged;

        //=================================================================
        // Conversion Operator
        //=================================================================
        public static implicit operator T(ParameterTemplate<T> p)
        {
            return p.Value;
        }

        //=================================================================
        // ToString Method
        //=================================================================
        public override string ToString()
        {
            return "Param " + Name + "=" + Value.ToString();
        }
    }
}
