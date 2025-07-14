using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Resources;
using System.Windows.Markup;

namespace UnitySC.Shared.UI.Extensions
{
    // Here is an usage example

    // ResourceAssemblyName is optional but it will improve performance

    //<ComboBox VerticalAlignment = "Top"
    //            SelectedValuePath="Enum"
    //            DisplayMemberPath="Translation"
    //            ItemsSource="{sharedExtensions:EnumValuesExtension EnumType=measuressettings:TSVShape, ResourceAssemblyName='UnitySC.PM.ANA.Client.CommonUI'}"
    //            SelectedValue="{Binding Shape}" />

    public class EnumValuesExtension : MarkupExtension
    {
        private Type _enumType;
        private string _resourceName;

        // Enumeration type
        public Type EnumType
        {
            set { _enumType = value; }
        }

        // Name of the assembly containing the string resources
        public string ResourceAssemblyName
        {
            set { _resourceName = value; }
        }

        public EnumValuesExtension()
        {
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // Enumeration type not passed through XAML
            if (_enumType == null)
                throw new ArgumentNullException("EnumType (Property not set)");
            if (!_enumType.IsEnum)
                throw new ArgumentNullException("Property EnumType must be an enum");
            // Bindable properties list
            var list = new List<dynamic>();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (string enumName in System.Enum.GetNames(_enumType))
            {
                string translation = string.Empty;

                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                //stopwatch.Stop();
                //var elapsed2 = stopwatch.ElapsedMilliseconds;
                foreach (var assembly in assemblies)
                {
                    if (!string.IsNullOrEmpty(_resourceName) && (assembly.GetName().Name != _resourceName))
                        continue;
                    if (string.IsNullOrEmpty(_resourceName) && (!assembly.GetName().Name.Contains("UnitySC")))
                        continue;

                    var rm = new ResourceManager(assembly.GetName().Name + ".Properties.Resources", assembly);
                    try
                    {
                        translation = rm.GetString(enumName);
                    }
                    catch (MissingManifestResourceException)
                    {
                    }
                    if (!string.IsNullOrEmpty(translation))
                        break;
                }

                if (string.IsNullOrEmpty(translation))
                    translation = enumName;
                list.Add(GetNamed(translation, enumName));
            }

            stopwatch.Stop();
            long elapsed = stopwatch.ElapsedMilliseconds;
            return list;
        }

        // Create one item which will fill our ComboBox ItemSource list
        private dynamic GetNamed(string translation, string enumName)
        {
            // We create a bindable context
            dynamic bindableResult = new ExpandoObject();
            // This dynamically created property will be
            // bindable from XAML (through DisplayMemberPath or wherever)
            bindableResult.Translation = translation;
            // We're setting the value, which will be passed to SelectedItem
            // of the ComboBox
            bindableResult.Enum = enumName;
            return bindableResult;
        }

        // Create one empty item which will fill our ComboBox ItemSource list
        private dynamic GetEmpty()
        {
            dynamic bindableResult = new ExpandoObject();
            bindableResult.Translation = string.Empty;
            bindableResult.Enum = null;
            return bindableResult;
        }
    }
}
