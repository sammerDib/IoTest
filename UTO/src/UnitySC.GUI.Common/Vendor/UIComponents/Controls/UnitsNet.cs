using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using UnitsNet;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    public class UnitsNet : Control
    {
        #region fields

        private bool _lockValueChange;
        private bool _isInitialStepDone;

        private QuantityType _quantityType;
        private Type _quantityUnit;
        private int _defaultUnitIndex;
        private string _defaultUnit;

        #endregion

        static UnitsNet()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UnitsNet), new FrameworkPropertyMetadata(typeof(UnitsNet)));
        }

        private void Initialize(IQuantity value)
        {
            _quantityType = value.Type;
            _quantityUnit = value.QuantityInfo.UnitType;

            var unitsNames = _quantityUnit.GetEnumNames();
            var defaultAbbreviations = new List<string>();
            unitsNames.ToList().ForEach(unit =>
            {
                var enumIndex = (int)Enum.Parse(_quantityUnit, unit);
                if (enumIndex != 0)
                {
                    // [KQu] get only default abbreviation for each related quantity units
                    defaultAbbreviations.Add(UnitAbbreviationsCache.Default.GetDefaultAbbreviation(_quantityUnit, enumIndex, FormatProvider));
                }
            });

            _defaultUnitIndex = value.QuantityInfo.UnitInfos.Select(x => x.Name).ToList().IndexOf(value.Unit.ToString()) + 1;
            _defaultUnit = Enum.GetName(_quantityUnit, _defaultUnitIndex);

            Abbreviations = defaultAbbreviations;

            if (IsUnitFixed && !string.IsNullOrEmpty(UnitFixedDefaultAbbreviation))
            {
                SelectedAbbreviation = UnitFixedDefaultAbbreviation;
            }
            else
            {
                SelectedAbbreviation =
                    UnitAbbreviationsCache.Default.GetDefaultAbbreviation(_quantityUnit, _defaultUnitIndex,
                        FormatProvider);
            }

            SetValueInternal(value.As(value.Unit));

            _isInitialStepDone = true;
        }

        #region Quantity

        public static readonly DependencyProperty QuantityProperty =
            DependencyProperty.Register("Quantity", typeof(IQuantity), typeof(UnitsNet),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    QuantityDefined, ValidateQuantityChangedCallback));

        public IQuantity Quantity
        {
            get { return (IQuantity)GetValue(QuantityProperty); }
            set { SetValue(QuantityProperty, value); }
        }

        private static void QuantityDefined(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var control = sender as UnitsNet;
            if (control == null) return;

            IQuantity newValue = (IQuantity)args.NewValue;
            if (newValue == null) return;

            if (!control._isInitialStepDone)
            {
                control.Initialize(newValue);
            }
            else
            {
                var newIndex = newValue.QuantityInfo.UnitInfos.Select(x => x.Name).ToList().IndexOf(newValue.Unit.ToString()) + 1;
                if (control._defaultUnitIndex != newIndex)
                {
                    /* Unit changed */
                    control._defaultUnitIndex = newIndex;
                    control._defaultUnit = Enum.GetName(control._quantityUnit, control._defaultUnitIndex);
                }

                var parser = UnitParser.Default;
                var ob = parser.Parse(control.SelectedAbbreviation, control._quantityUnit, control.FormatProvider);

                control.SetValueInternal(newValue.As(control._defaultUnit != null && control._defaultUnit.Equals(ob.ToString()) ? newValue.Unit : ob));
            }
        }

        private static object ValidateQuantityChangedCallback(DependencyObject sender, object value)
        {
            var control = sender as UnitsNet;
            if (control == null) return value;

            IQuantity newValue = (IQuantity)value;
            if (newValue != null)
            {
                /* Control if Quantity is still the same */
                if (control._quantityType != newValue.Type)
                {
                    control._isInitialStepDone = false;
                }
            }
            else
            {
                /* Reset the user control as on first startup */
                control.SetValueInternal(null);
                control.Abbreviations = null;
                control.SelectedAbbreviation = null;
                control._isInitialStepDone = false;
            }

            return value;
        }

        #endregion Quantity

        #region Value

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(double?), typeof(UnitsNet), new PropertyMetadata(null, ValueChangedCallback));

        public double? Value
        {
            get { return GetValue(ValueProperty) as double?; }
            set { SetValue(ValueProperty, value); }
        }

        private void SetValueInternal(double? value)
        {
            _lockValueChange = true;
            try
            {
                Value = value;
            }
            finally
            {
                _lockValueChange = false;
            }
        }

        private static void ValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as UnitsNet;
            if (control == null) return;
            if (control._lockValueChange) return;

            control.UpdateIQuantity((double?)e.NewValue);
            control.RaiseValueChanged();
        }

        private void UpdateIQuantity(double? newValue)
        {
            if (SelectedAbbreviation == null || newValue == null) return;

            Enum ob = UnitParser.Default.Parse(SelectedAbbreviation, _quantityUnit, FormatProvider);

            string fromUnit = ob.ToString();

            if (!_defaultUnit.Equals(ob.ToString()))
            {
                newValue = UnitConverter.ConvertByName(newValue.Value,
                    Enum.GetName(typeof(QuantityType), _quantityType),
                    fromUnit,
                    _defaultUnit);
            }

            var unitMethod = Quantity.GetType().GetMethod("From");
            if (unitMethod != null)
            {
                QuantityValue val = Convert.ToDouble(newValue);

                object[] parameters =
                {
                    val,
                    Enum.ToObject(_quantityUnit, _defaultUnitIndex)
                };
                Quantity = (IQuantity)unitMethod.Invoke(this, parameters);
            }
        }

        #endregion

        #region Abbreviations

        public static readonly DependencyPropertyKey AbbreviationsPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(Abbreviations), typeof(List<string>), typeof(UnitsNet), new FrameworkPropertyMetadata(default(List<string>),
                FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty AbbreviationsProperty = AbbreviationsPropertyKey.DependencyProperty;

        public List<string> Abbreviations
        {
            get { return (List<string>)GetValue(AbbreviationsProperty); }
            protected set { SetValue(AbbreviationsPropertyKey, value); }
        }

        #endregion

        #region Selected Abbreviation

        public static readonly DependencyProperty SelectedAbbreviationProperty = DependencyProperty.Register(
            "SelectedAbbreviation", typeof(string), typeof(UnitsNet), new PropertyMetadata(default(string), SelectedAbbreviationChangedCallback));

        private static void SelectedAbbreviationChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as UnitsNet;
            if (control == null) return;

            if (e.NewValue == null) return;
            if (control.Value == null) return;

            UnitParser parser = UnitParser.Default;
            object ob = parser.Parse(e.NewValue.ToString(), control._quantityUnit, control.FormatProvider);

            var asMethod = control.Quantity.GetType().GetMethod("As", new[] { control._quantityUnit });
            if (asMethod != null)
            {
                object[] parameter =
                {
                    ob
                };
                control.SetValueInternal((double)asMethod.Invoke(control.Quantity, parameter));
            }
        }

        public string SelectedAbbreviation
        {
            get { return (string)GetValue(SelectedAbbreviationProperty); }
            set { SetValue(SelectedAbbreviationProperty, value); }
        }

        #endregion

        #region Increment

        public static readonly DependencyProperty IncrementProperty =
            DependencyProperty.Register("Increment", typeof(double?), typeof(UnitsNet),
                new PropertyMetadata((double?)1));

        public double? Increment
        {
            get { return (double?)GetValue(IncrementProperty); }
            set { SetValue(IncrementProperty, value); }
        }

        #endregion Increment

        #region StringFormat

        public static readonly DependencyProperty StringFormatProperty = DependencyProperty.Register(
            "StringFormat", typeof(string), typeof(UnitsNet), new PropertyMetadata(string.Empty));

        public string StringFormat
        {
            get { return (string)GetValue(StringFormatProperty); }
            set { SetValue(StringFormatProperty, value); }
        }

        #endregion

        #region UnitWidth

        /// <summary>
        /// Identifies the <see cref="UnitWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UnitWidthProperty =
            DependencyProperty.Register(nameof(UnitWidth), typeof(double), typeof(UnitsNet), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Gets or sets the width of the unit part.
        /// </summary>
        public double UnitWidth
        {
            get { return (double)GetValue(UnitWidthProperty); }
            set { SetValue(UnitWidthProperty, value); }
        }

        #endregion UnitWidth

        #region IsUnitFixed

        /// <summary>
        /// Identifies the <see cref="IsUnitFixed"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsUnitFixedProperty =
            DependencyProperty.Register(nameof(IsUnitFixed), typeof(bool), typeof(UnitsNet), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the value indicating whether the <see cref="Quantity"/>'s unit can be changed from UI.
        /// </summary>
        public bool IsUnitFixed
        {
            get { return (bool)GetValue(IsUnitFixedProperty); }
            set { SetValue(IsUnitFixedProperty, value); }
        }

        #endregion IsUnitFixed


        #region UnitFixedDefaultAbbreviation


        public string UnitFixedDefaultAbbreviation
        {
            get { return (string)GetValue(UnitFixedDefaultAbbreviationProperty); }
            set { SetValue(UnitFixedDefaultAbbreviationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UnitFixedDefaultAbbreviation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitFixedDefaultAbbreviationProperty =
            DependencyProperty.Register("UnitFixedDefaultAbbreviation", typeof(string), typeof(UnitsNet),
                new PropertyMetadata(string.Empty, UnitFixedDefaultAbbreviationChanged));



        private static void UnitFixedDefaultAbbreviationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) return;
            var control = d as UnitsNet;
            control?.DefineUnitFixedDefaultAbbreviation();
        }

        private void DefineUnitFixedDefaultAbbreviation()
        {
            if (!string.IsNullOrEmpty(UnitFixedDefaultAbbreviation))
            {
                SelectedAbbreviation = UnitFixedDefaultAbbreviation;
            }
        }

        #endregion UnitFixedDefaultAbbreviation

        #region UnitMargin

        /// <summary>
        /// Identifies the <see cref="UnitMargin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UnitMarginProperty =
            DependencyProperty.Register(nameof(UnitMargin), typeof(Thickness), typeof(UnitsNet),
                new PropertyMetadata(default(Thickness)));

        /// <summary>
        /// Gets or sets the margin of the unit part, to configure distance between numerical value and unit.
        /// </summary>
        public Thickness UnitMargin
        {
            get { return (Thickness)GetValue(UnitMarginProperty); }
            set { SetValue(UnitMarginProperty, value); }
        }

        #endregion UnitMargin

        #region FormatProvider

        public static readonly DependencyProperty FormatProviderProperty = DependencyProperty.Register(
            "FormatProvider", typeof(IFormatProvider), typeof(UnitsNet), new PropertyMetadata(CultureInfo.InvariantCulture, FormatProviderPropertyChangedCallback));

        public IFormatProvider FormatProvider
        {
            get { return (IFormatProvider)GetValue(FormatProviderProperty); }
            set { SetValue(FormatProviderProperty, value); }
        }

        private static void FormatProviderPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as UnitsNet;
            if (control == null) return;

            control._isInitialStepDone = false;
            if (control.Quantity != null)
            {
                control.Initialize(control.Quantity);
            }
        }

        #endregion

        #region Events

        public event EventHandler ValueChanged;

        protected virtual void RaiseValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
