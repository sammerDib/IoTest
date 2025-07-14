using System.Collections.Generic;
using System.Linq;

using Agileo.Common.Localization;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.Semi.Communication.Abstractions.E5;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Filters;
using UnitySC.GUI.Common.Vendor.UIComponents.GuiExtended;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.SequenceLog
{
    public abstract class BaseSequenceLogPanel : BusinessPanel
    {
        #region Fields

        private const int MaximumLinesNumber = 1000;

        private const string StreamPrefix = "S";

        private const string FunctionPrefix = "F";

        #endregion

        #region Properties

        private SecsMessage _selectedMessage;

        public SecsMessage SelectedMessage
        {
            get => _selectedMessage;
            set => SetAndRaiseIfChanged(ref _selectedMessage, value);
        }

        public BusinessPanelCommand ClearMessagesCommand { get; }

        public BusinessPanelCheckToggleCommand AutoScrollCommand { get; }

        public RealTimeDataTableSource<SecsMessage> SequenceTableSource { get; } = new()
        {
            MaxItemNumber = MaximumLinesNumber
        };

        #endregion

        #region Constructors

        static BaseSequenceLogPanel()
        {
            DataTemplateGenerator.Create(typeof(BaseSequenceLogPanel), typeof(SequenceLogPanelView));
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(SequenceLogResources)));
        }

        protected BaseSequenceLogPanel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
            ClearMessagesCommand = new BusinessPanelCommand(
                nameof(SequenceLogResources.SEQUENCE_LOG_CLEAR),
                new DelegateCommand(ClearMessagesCommandExecute, () => SequenceTableSource.Any()),
                PathIcon.Clear);
            Commands.Add(ClearMessagesCommand);

            AutoScrollCommand = new BusinessPanelCheckToggleCommand(
                nameof(SequenceLogResources.SEQUENCE_LOG_SCROLL_TO_END),
                () => { },
                () => { },
                PathIcon.ScrollToEnd)
            {
                IsChecked = true
            };
            Commands.Add(AutoScrollCommand);

            SequenceTableSource.Search.AddSearchDefinition(
                new InvariantText(nameof(Message.Name)),
                getComparableStringFunc: item => item.Name, true);

            SequenceTableSource.Search.AddSearchDefinition(
                new InvariantText(nameof(Message.StreamAndFunction)),
                item => item.StreamAndFunction, true);

            SequenceTableSource.Search.AddSearchDefinition(
                new InvariantText(nameof(Message.Txid)),
                item => item.Txid.ToString(), true);

            SequenceTableSource.Filter.Add(
                new FilterCollection<SecsMessage, string>(
                    nameof(SequenceLogResources.SEQUENCE_LOG_STREAM_FILTER),
                    GetMessagesStreams,
                    GetMessageStream));

            SequenceTableSource.Filter.Add(
                new FilterCollection<SecsMessage, string>(
                    nameof(SequenceLogResources.SEQUENCE_LOG_FUNCTION_FILTER),
                    GetMessagesFunctions,
                    GetMessageFunction));

            SequenceTableSource.Filter.Add(new FilterSwitch<SecsMessage>(nameof(SequenceLogResources.SEQUENCE_LOG_EXCHANGE_OUTGOING),
                message => message.Direction == Direction.EquipmentToHost));

            SequenceTableSource.Filter.Add(new FilterSwitch<SecsMessage>(nameof(SequenceLogResources.SEQUENCE_LOG_EXCHANGE_INCOMING),
                message => message.Direction == Direction.HostToEquipment));

            SequenceTableSource.Filter.Add(new FilterPeriodRange<SecsMessage>(
                nameof(SequenceLogResources.SEQUENCE_LOG_DATE_FILTER),
                message => (message.Time, message.Time))
            {
                UseHoursMinutesSeconds = true
            });
        }

        #endregion

        #region Methods

        #region Filters

        #region Stream Filter

        private static string GetMessageStream(SecsMessage message)
        {
            if (!string.IsNullOrEmpty(message.Stream.ToString()))
            {
                return StreamPrefix + message.Stream;
            }

            return string.Empty;
        }

        private IEnumerable<string> GetMessagesStreams()
        {
            var loc = new List<string>();
            loc.AddRange(SequenceTableSource.ToList().FindAll(_ => true).Select(m => m.Stream.ToString()).Distinct());
            return loc.Select(stream => StreamPrefix + stream).OrderBy(s => s);
        }

        #endregion

        #region Function filter

        private static string GetMessageFunction(SecsMessage message)
        {
            if (!string.IsNullOrEmpty(message.Function.ToString()))
            {
                return FunctionPrefix + message.Function;
            }

            return string.Empty;
        }

        private IEnumerable<string> GetMessagesFunctions()
        {
            var loc = new List<string>();
            loc.AddRange(SequenceTableSource.ToList().FindAll(_ => true).Select(m => m.Function.ToString()).Distinct());
            return loc.Select(function => FunctionPrefix + function).OrderBy(s => s);
        }

        #endregion

        #endregion

        private void ClearMessagesCommandExecute()
        {
            Popups.Show(new Popup(new LocalizableText(nameof(SequenceLogResources.SEQUENCE_LOG_POPUP_CLEAR_TITLE)))
            {
                AutoDisposeOnClosed = true,
                Message = new LocalizableText(nameof(SequenceLogResources.SEQUENCE_LOG_POPUP_CLEAR_MESSAGE)),
                Commands =
                {
                    new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)),
                    new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_POPUP_YES), new DelegateCommand(
                        () =>
                        {
                            SequenceTableSource.Clear();
                            SequenceTableSource.UpdateFilterPossibleValues();
                        }))
                }
            });
        }

        protected SecsMessage ToSecsMessage(Message message)
        {
            return new SecsMessage
            {
                Txid = message.Txid,
                Direction = message.Direction,
                Function = message.Function,
                StreamAndFunction = message.StreamAndFunction,
                Stream = message.Stream,
                Name = message.Name,
                Time = message.Time,
                Smn = message.ToSMN()
            };
        }

        #endregion
    }
}

