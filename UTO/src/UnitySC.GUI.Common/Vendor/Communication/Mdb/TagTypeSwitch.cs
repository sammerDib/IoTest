using System;
using System.Collections.Generic;

using Agileo.MessageDataBus;

namespace UnitySC.GUI.Common.Vendor.Communication.Mdb
{
    public class TagTypeSwitch
    {
        private readonly Dictionary<Type, Action<BaseTag>> _matches = new Dictionary<Type, Action<BaseTag>>();

        public TagTypeSwitch Case<T>(Action<ExternalTag<T>> action)
        {
            _matches.Add(typeof(T), x => action(x as ExternalTag<T>));
            return this;
        }

        public TagTypeSwitch Case<T>(Action<Tag<T>> action)
        {
            _matches.Add(typeof(T), x => action(x as Tag<T>));
            return this;
        }

        public bool Switch(BaseTag x)
        {
            if (_matches.TryGetValue(x.ValueType, out var action))
            {
                action(x);
                return true;
            }

            return false;
        }
    }

    public class TagTypeSwitch<T>
    {
        private readonly Dictionary<Type, Func<BaseTag, T>> _matches = new Dictionary<Type, Func<BaseTag, T>>();

        public TagTypeSwitch<T> Case<TTag>(Func<ExternalTag<TTag>, T> action)
        {
            _matches.Add(typeof(TTag), x => action(x as ExternalTag<TTag>));
            return this;
        }

        public TagTypeSwitch<T> Case<TTag>(Func<Tag<TTag>, T> action)
        {
            _matches.Add(typeof(TTag), x => action(x as Tag<TTag>));
            return this;
        }

        public T Switch(BaseTag x, T defaultValue = default)
        {
            return _matches.TryGetValue(x.ValueType, out var action) ? action(x) : defaultValue;
        }
    }
}
