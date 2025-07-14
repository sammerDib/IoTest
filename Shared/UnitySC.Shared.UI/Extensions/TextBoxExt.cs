using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace UnitySC.Shared.UI.Extensions
{
    public static class TextBoxExt
    {
        #region OnLostFocusUpdateTarget

        public static readonly DependencyProperty
            OnLostFocusUpdateTargetProperty = DependencyProperty.RegisterAttached
            (
                "OnLostFocusUpdateTarget",
                typeof(bool?),
                typeof(TextBoxExt),
                new UIPropertyMetadata(null, OnOnLostFocusUpdateTargetPropertyChanged)
            );

        public static bool GetOnLostFocusUpdateTarget(DependencyObject obj)
        {
            return (bool)obj.GetValue(OnLostFocusUpdateTargetProperty);
        }

        public static void SetOnLostFocusUpdateTarget(DependencyObject obj, bool value)
        {
            obj.SetValue(OnLostFocusUpdateTargetProperty, value);
        }

        private static void OnOnLostFocusUpdateTargetPropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var tb = dpo as TextBox;
            if (args.NewValue == null) { return; }
            //
            if (tb != null)
            {
                //
                if ((bool)args.NewValue == true)
                {
                    tb.LostKeyboardFocus += OnLostKeyboardFocusUpdateTargetOnly;
                    tb.LostKeyboardFocus -= OnLostKeyboardFocusDefault;
                }
                else
                {
                    if (tb.GetBindingExpression(TextBox.TextProperty) != null)
                    {
                        if (tb.GetBindingExpression(TextBox.TextProperty).ParentBinding.UpdateSourceTrigger != UpdateSourceTrigger.Explicit)
                        {
                            throw new Exception("The UpdateSourceTrigger must be explicit");
                            // Clone current binding
                            //Binding NewBinding = BindingUtils.CloneBinding(BindingOperations.GetBinding(tb, TextBox.TextProperty));
                            //NewBinding.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
                            //// Assign new binding
                            //tb.SetBinding(TextBox.TextProperty, NewBinding);
                        }
                    }
                    tb.LostKeyboardFocus -= OnLostKeyboardFocusUpdateTargetOnly;
                    tb.LostKeyboardFocus += OnLostKeyboardFocusDefault;
                }
            }
        }

        private static void OnLostKeyboardFocusDefault(object sender, KeyboardFocusChangedEventArgs e)
        {
            var tb = (TextBox)sender;
            if (tb != null)
            {
                if (tb.GetBindingExpression(TextBox.TextProperty) != null)
                {
                    if (tb.GetBindingExpression(TextBox.TextProperty).HasError == true)
                    {
                        if (tb.GetBindingExpression(TextBox.TextProperty).ParentBinding.Mode != BindingMode.OneWayToSource)
                        {
                            tb.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                        }
                    }
                    else
                    {
                        if ((tb.GetBindingExpression(TextBox.TextProperty).ParentBinding.Mode != BindingMode.OneWay) &&
                            (tb.GetBindingExpression(TextBox.TextProperty).ParentBinding.Mode != BindingMode.OneTime))
                        {
                            if (tb.GetBindingExpression(TextBox.TextProperty).IsDirty == true)
                            {
                                tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                            }
                        }
                    }
                }
            }
        }

        private static void OnLostKeyboardFocusUpdateTargetOnly(object sender, KeyboardFocusChangedEventArgs e)
        {
            var tb = (TextBox)sender;
            if (tb != null)
            {
                if (tb.GetBindingExpression(TextBox.TextProperty) != null)
                {
                    if (tb.GetBindingExpression(TextBox.TextProperty).ParentBinding.Mode != BindingMode.OneWayToSource)
                    {
                        tb.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                    }
                }
            }
        }

        #endregion OnLostFocusUpdateTarget

        //

        #region OnEnterAndEscUpdate

        #region OnEnterAndEscUpdate_ICommand

        // IComand
        public static readonly DependencyProperty
        OnEnterAndEscUpdateCommandProperty = DependencyProperty.RegisterAttached
        (
            "OnEnterAndEscUpdateCommand",
            typeof(ICommand),
            typeof(TextBoxExt),
            new UIPropertyMetadata(null)
        );

        public static ICommand GetOnEnterAndEscUpdateCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(OnEnterAndEscUpdateCommandProperty);
        }

        public static void SetOnEnterAndEscUpdateCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(OnEnterAndEscUpdateCommandProperty, value);
        }

        //
        // ICommand Parameter
        public static readonly DependencyProperty
        OnEnterAndEscUpdateCommandParameterProperty = DependencyProperty.RegisterAttached
        (
            "OnEnterAndEscUpdateCommandParameter",
            typeof(object),
            typeof(TextBoxExt),
            new UIPropertyMetadata(null)
        );

        public static object GetOnEnterAndEscUpdateCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(OnEnterAndEscUpdateCommandParameterProperty);
        }

        public static void SetOnEnterAndEscUpdateCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(OnEnterAndEscUpdateCommandParameterProperty, value);
        }

        #endregion OnEnterAndEscUpdate_ICommand

        //
        //
        //
        public static readonly DependencyProperty
            OnEnterAndEscUpdateProperty = DependencyProperty.RegisterAttached
            (
                "OnEnterAndEscUpdate",
                typeof(bool),
                typeof(TextBoxExt),
                new UIPropertyMetadata(false, OnOnEnterAndEscUpdatePropertyChanged)
            );

        public static bool GetOnEnterAndEscUpdate(DependencyObject obj)
        {
            return (bool)obj.GetValue(OnEnterAndEscUpdateProperty);
        }

        public static void SetOnEnterAndEscUpdate(DependencyObject obj, bool value)
        {
            obj.SetValue(OnEnterAndEscUpdateProperty, value);
        }

        private static void OnOnEnterAndEscUpdatePropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var tb = dpo as TextBox;
            if (tb != null)
            {
                if ((bool)args.NewValue == true)
                {
                    if (tb.GetBindingExpression(TextBox.TextProperty) != null)
                    {
                        if (tb.GetBindingExpression(TextBox.TextProperty).ParentBinding.UpdateSourceTrigger != UpdateSourceTrigger.Explicit)
                        {
                            //throw new Exception("The UpdateSourceTrigger must be explicit");
                            // Clone current binding
                            //Binding NewBinding = BindingUtils.CloneBinding(BindingOperations.GetBinding(tb, TextBox.TextProperty));
                            //NewBinding.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
                            //// Assign new binding
                            //tb.SetBinding(TextBox.TextProperty, NewBinding);
                        }
                    }
                    tb.KeyUp += KeyUp;
                }
                else
                {
                    tb.KeyUp -= KeyUp;
                }
            }
        }

        private static void KeyUp(object sender, KeyEventArgs e)
        {
            var tb = (TextBox)sender;
            if (tb == null) { return; }
            if (tb.GetBindingExpression(TextBox.TextProperty) == null) { return; }

            if (e.Key == Key.Escape)
            {
                if (tb.GetBindingExpression(TextBox.TextProperty).ParentBinding.Mode != BindingMode.OneWayToSource)
                {
                    tb.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                }
                return;
            }
            //
            if (e.Key == Key.Enter)
            {
                if (tb.GetBindingExpression(TextBox.TextProperty).HasError == true)
                {
                    if (tb.GetBindingExpression(TextBox.TextProperty).ParentBinding.Mode != BindingMode.OneWayToSource)
                    {
                        tb.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                    }
                }
                else
                {
                    if ((tb.GetBindingExpression(TextBox.TextProperty).ParentBinding.Mode != BindingMode.OneWay) &&
                        (tb.GetBindingExpression(TextBox.TextProperty).ParentBinding.Mode != BindingMode.OneTime))
                    {
                        if (tb.GetBindingExpression(TextBox.TextProperty).IsDirty == true)
                        {
                            tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                            var Cmd2Do = GetOnEnterAndEscUpdateCommand(tb);
                            if (Cmd2Do != null)
                            {
                                Cmd2Do.Execute(GetOnEnterAndEscUpdateCommandParameter(tb));
                            }
                        }
                    }
                }

                var request = new TraversalRequest(FocusNavigationDirection.Next)
                {
                    Wrapped = true
                };
                tb.MoveFocus(request);
                return;
            }
            //
            // Data is simply inputed
            tb.GetBindingExpression(TextBox.TextProperty).ValidateWithoutUpdate();
            //
        }

        #endregion OnEnterAndEscUpdate
    }
}
