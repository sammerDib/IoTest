using System;
using System.Collections.Generic;

using Agileo.Common.Tracing;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.UserMessages;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.UTO.Controller.Views.Panels.Integration
{
    public class MessengerHandler
    {
        private readonly object _messageLock = new();
        private const string MessageSource = "UnitySC";
        private readonly List<BusinessPanel> _subscribedPanels = new();

        public void SubscribePanel(BusinessPanel panel)
        {
            lock (_messageLock)
            {
                _subscribedPanels.Add(panel);
            }
        }

        public void Initialize()
        {
            var messenger = ClassLocator.Default.GetInstance<IMessenger>();
            messenger.Register<Message>(this, (r, m) => HandleMessage(m));
        }

        private void HandleMessage(Message message)
        {
            lock (_messageLock)
            {
                GUI.Common.Vendor.Helpers.DispatcherHelper.DoInUiThread(
                    () =>
                    {
                        var messageLevel = GetTraceLevelType(message.Level);
                        var messageContent = string.IsNullOrEmpty(message.Source) ? message.UserContent : $"[{message.Source}] {message.UserContent}";

                        if (string.IsNullOrWhiteSpace(message.AdvancedContent))
                        {
                            GUI.Common.App.Instance.Tracer.Trace(MessageSource, messageLevel, messageContent);
                        }
                        else
                        {
                            GUI.Common.App.Instance.Tracer.Trace(MessageSource, messageLevel, messageContent, new TraceParam(message.AdvancedContent));
                        }

                        var userMessageLevel = GetMessageLevel(message.Level);

                        foreach (var subscribedPanel in _subscribedPanels)
                        {
                            var userMessage = new UserMessage(userMessageLevel, new InvariantText(messageContent))
                            {
                                CanUserCloseMessage = true,
                                SecondsDuration = userMessageLevel == Agileo.GUI.Services.Popups.MessageLevel.Error ? null : 5
                            };

                            subscribedPanel.Messages.HideAll();
                            subscribedPanel.Messages.Show(userMessage);
                        }
                    });
            }
        }

        private static TraceLevelType GetTraceLevelType(MessageLevel level)
        {
            return level switch
            {
                MessageLevel.None => TraceLevelType.Info,
                MessageLevel.Information => TraceLevelType.Info,
                MessageLevel.Question => TraceLevelType.Info,
                MessageLevel.Success => TraceLevelType.Info,
                MessageLevel.Warning => TraceLevelType.Warning,
                MessageLevel.Error => TraceLevelType.Error,
                MessageLevel.Fatal => TraceLevelType.Fatal,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static Agileo.GUI.Services.Popups.MessageLevel GetMessageLevel(MessageLevel level)
        {
            return level switch
            {
                MessageLevel.None => Agileo.GUI.Services.Popups.MessageLevel.NotAssigned,
                MessageLevel.Information => Agileo.GUI.Services.Popups.MessageLevel.Info,
                MessageLevel.Question => Agileo.GUI.Services.Popups.MessageLevel.Info,
                MessageLevel.Success => Agileo.GUI.Services.Popups.MessageLevel.Success,
                MessageLevel.Warning => Agileo.GUI.Services.Popups.MessageLevel.Warning,
                MessageLevel.Error => Agileo.GUI.Services.Popups.MessageLevel.Error,
                MessageLevel.Fatal => Agileo.GUI.Services.Popups.MessageLevel.Error,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
