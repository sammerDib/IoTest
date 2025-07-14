using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

using Agileo.EquipmentModeling;
using Agileo.GUI.Components;

using Humanizer;

using UnitsNet;
using UnitsNet.Units;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.MarkDownViewer;
using UnitySC.GUI.Common.Vendor.UIComponents.Extensions;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.Command.Parameter;

using SeverityMessageViewerViewModel = UnitySC.GUI.Common.Vendor.UIComponents.Components.SeverityMessageViewer.SeverityMessageViewerViewModel;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.Command
{
    public sealed class DeviceCommandViewModel : Notifier, IDisposable
    {
        public static List<long> DefaultTimeOuts { get;} = new() { 10, 30, 60, 1000, 2000, 5000, 10000, 20000, 60000 };

        public DeviceCommandViewModel(Agileo.EquipmentModeling.Device device, DeviceCommand command, ICommand send, ICommand abort)
        {
            Device = device;
            Model = command;
            SendCommand = send;
            AbortCommand = abort;
            AbortCommandVisibility = device.DeviceType.SupportInterruption(InterruptionKind.Abort)
                ? Visibility.Visible
                : Visibility.Collapsed;

            foreach (var parameter in command.Parameters)
            {
                ParameterViewModels.Add(NewParameterViewModel(parameter));
            }

            CommandContext.TimeOut = (long)Model.Timeout.Value;

            UpdateProperties();
        }

        public Agileo.EquipmentModeling.Device Device { get; }

        public DeviceCommand Model { get; }

        public ICommand SendCommand { get; }

        public ICommand AbortCommand { get; }

        public Visibility AbortCommandVisibility { get; }
        
        public List<ParameterViewModel> ParameterViewModels { get; } = new();

        public SeverityMessageViewerViewModel MessagesViewerViewModel { get; } = new();

        public Dictionary<string, object> CommandParameters { get; } = new();

        public DeviceCommandContextParametersViewModel CommandContext { get; } = new();

        public string TimeoutLabel => $"Timeout({UnitAbbreviationsCache.Default.GetUnitAbbreviations(DurationUnit.Second, System.Globalization.CultureInfo.CurrentUICulture).First()})";

        private ParameterViewModel NewParameterViewModel(Agileo.EquipmentModeling.Parameter parameter)
        {
            switch (parameter.Type)
            {
                case CSharpType type:
                    var platformType = type.PlatformType;

                    if (platformType.IsEnum)
                    {
                        return new EnumerableParameterViewModel(parameter, this);
                    }

                    if (platformType == typeof(string))
                    {
                        return new StringParameterViewModel(parameter, this);
                    }

                    if (platformType.Assembly == typeof(Acceleration).Assembly
                        || parameter.Unit != null)
                    {
                        return new QuantityParameterViewModel(parameter, this);
                    }

                    if (platformType == typeof(IMaterialLocationContainer))
                    {
                        return new MaterialLocationContainerChoiceViewModel(parameter, this);
                    }

                    return new NumericParameterViewModel(parameter, this, platformType);
                case MaterialLocationType _:
                    return new MaterialLocationChoiceViewModel(parameter, this);
                default:
                    return null;
            }
        }

        public void UpdateProperties() => MarkDownToolTip = CreateToolTip();

        public string HumanizedName => Model.GetHumanizedName();

        private MarkDownViewerViewModel _markDownToolTip;

        public MarkDownViewerViewModel MarkDownToolTip
        {
            get => _markDownToolTip;
            set => SetAndRaiseIfChanged(ref _markDownToolTip, value);
        }

        private MarkDownViewerViewModel CreateToolTip()
        {
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(Model.DocumentationAsMarkdown))
            {
                sb.AppendLine(Model.DocumentationAsMarkdown);
            }

            if (Model.PreConditions.Count > 0 || Model.PostConditions.Count > 0)
            {
                if (sb.Length > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("---");
                    sb.AppendLine();
                }

                if (Model.PreConditions.Count > 0)
                {
                    sb.AppendLine("#### pre-conditions:");
                    foreach (var condition in Model.PreConditions)
                    {
                        AppendConditionDocumentation(condition, sb);
                    }
                }

                if (Model.PostConditions.Count > 0)
                {
                    sb.AppendLine("#### post-conditions:");
                    foreach (var condition in Model.PostConditions)
                    {
                        AppendConditionDocumentation(condition, sb);
                    }
                }
            }

            if (sb.Length > 0)
            {
                return new MarkDownViewerViewModel(sb.ToString());
            }

            return null;
        }

        private static void AppendConditionDocumentation(CommandCondition condition, StringBuilder sb)
        {
            string documentationAsMarkdown = condition.DocumentationAsMarkdown;
            if (string.IsNullOrEmpty(documentationAsMarkdown) && condition.Behavior is CSharpCommandConditionBehavior behavior)
            {
                string[] splits = behavior.BehaviorImplementation.Split(',');
                if (splits.Length >= 2)
                {
                    documentationAsMarkdown = splits[0]
                        .Split('.')
                        .Last()
                        .Trim(' ')
                        .Humanize(LetterCasing.Sentence);
                }
            }

            if (!string.IsNullOrEmpty(documentationAsMarkdown))
            {
                sb.Append("* ");
                sb.AppendLine(documentationAsMarkdown);
            }
        }

        public void Dispose()
        {
            MessagesViewerViewModel?.Dispose();

            foreach (var parameter in ParameterViewModels.OfType<IDisposable>())
            {
                parameter.Dispose();
            }
        }
    }
}
