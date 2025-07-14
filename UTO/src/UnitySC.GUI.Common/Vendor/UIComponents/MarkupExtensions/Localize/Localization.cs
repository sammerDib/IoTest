using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

using Agileo.Common.Localization;
using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.UIComponents.MarkupExtensions.Localize
{
    /// <summary>
    /// Implements a markup extension that returns the key translation of a static field and property reference.
    /// </summary>
    public class StaticExtension : DynamicExtension
    {
        /// <summary>
        /// Initialize a new instance of <see cref="StaticExtension"/>
        /// </summary>
        public StaticExtension()
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="StaticExtension"/>
        /// </summary>
        public StaticExtension(string member) : base(member)
        {
        }

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var length = Member.LastIndexOf('.');
            if (length < 0)
            {
                throw new ArgumentException($"Cannot resolve symbol '{Member}'");
            }

            var qualifiedTypeName = Member.Substring(0, length);
            if (string.IsNullOrEmpty(qualifiedTypeName))
            {
                throw new ArgumentException($"Cannot resolve symbol '{Member}'");
            }

            var key = Member.Substring(length + 1, Member.Length - length - 1);
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"Cannot resolve symbol '{Member}'");
            }

            InternalMember = key;
            return base.ProvideValue(serviceProvider);
        }
    }

    /// <summary>
    /// The localized markup extension returns a localized
    /// resource for a specified resource key.
    /// </summary>
    /// <remarks>
    /// If the target object and property are dependency object (property), this extension
    /// can update the target property when a culture is changed during runtime.
    /// Thus, the language of the application can be changed without the application restart.
    /// However, if this condition is not met, this functionality is unavailable and you should
    /// use <see cref="DynamicExtension" /> class instead.
    /// </remarks>
    [Localizability(LocalizationCategory.NeverLocalize)]
    public class DynamicExtension : MarkupExtension
    {
        private string _member;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicExtension" /> class.
        /// </summary>
        public DynamicExtension()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicExtension" /> class.
        /// </summary>
        public DynamicExtension(string member)
        {
            _member = member ?? throw new ArgumentNullException(nameof(member));
            InternalMember = member;
        }

        /// <summary>
        /// Gets or sets the member to translate.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        [ConstructorArgument("member")]
        public string Member
        {
            get => _member;
            set
            {
                _member = value ?? throw new ArgumentNullException(nameof(value));
                InternalMember = value;
            }
        }

        /// <summary>
        /// Gets or sets a format string that is used to format the value.
        /// </summary>
        /// <value>The format string</value>
        public string Format { get; set; }

        /// <summary>
        /// Gets or sets the default value that is used when the key was not found
        /// or the localized value is null.
        /// </summary>
        /// <value>The default value</value>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the value converter that is used to convert the value.
        /// </summary>
        /// <value>An IValueConverter instance</value>
        public IValueConverter Converter { get; set; }

        /// <summary>
        /// Gets or sets the internal value of the member.
        /// </summary>
        /// <value>
        /// The internal value of the member.
        /// </value>
        protected string InternalMember { get; set; }

        /// <summary>
        /// When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        /// <returns>
        /// The object value to set on the property where the extension is applied.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Notifier.IsInDesignModeStatic)
            {
                var value = LocalizationManager.GetString(InternalMember);
                return string.IsNullOrEmpty(Format)
                    ? value
                    : string.Format(LocalizationManager.Instance.CurrentCulture, Format, value);
            }

            var localizeData = new LocalizeData(InternalMember);
            var binding = new Binding("Value.Value")
            {
                Source = localizeData,
                StringFormat = Format,
                Converter = Converter,
                FallbackValue = DefaultValue ?? $"?{InternalMember}?"
            };

            // If key is unknown, must return DefaultValue.
            // If DefaultValue is not specified, return alarmed key
            var localizedValue = localizeData.Value;
            return localizedValue.IsExist
                ? binding.ProvideValue(serviceProvider)
                : DefaultValue ?? $"?{localizedValue.Key}?";
        }
    }
}
