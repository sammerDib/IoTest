using System;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;

namespace UnitySC.GUI.Common.Vendor.UIComponents.MarkupExtensions
{
    public abstract class UpdatableMarkupExtension : MarkupExtension
    {
        private object _targetObject;
        private object _targetProperty;

        protected object TargetObject => _targetObject;
        protected object TargetProperty => _targetProperty;

        public sealed override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget target)
            {
                _targetObject = target.TargetObject;
                _targetProperty = target.TargetProperty;
            }
            return ProvideValueInternal(serviceProvider);
        }

        protected void UpdateValue(object value)
        {
            if (_targetObject != null)
            {
                if (_targetProperty is DependencyProperty)
                {
                    var obj = _targetObject as DependencyObject;
                    var prop = _targetProperty as DependencyProperty;

                    Action updateAction = () =>
                    {
                        obj?.SetValue(prop, value);
                    };

                    // Check whether the target object can be accessed from the
                    // current thread, and use Dispatcher.Invoke if it can't

                    if (obj != null && obj.CheckAccess())
                    {
                        updateAction();
                    }
                    else
                    {
                        obj?.Dispatcher.Invoke(updateAction);
                    }
                }
                else if (_targetProperty != null) // _targetProperty is PropertyInfo
                {
                    var prop = _targetProperty as PropertyInfo;
                    if (prop != null) prop.SetValue(_targetObject, value, null);
                }
            }
        }

        protected abstract object ProvideValueInternal(IServiceProvider serviceProvider);
    }
}
