using System;

using Agileo.Common.Localization;
using Agileo.GUI.Components;
using Agileo.MessageDataBus;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.TagsSpy
{
    public class TagsSpyViewModel : Notifier
    {
        #region Properties

        public TagsSpyListViewModel TagsSpyList { get; }
        public TagsSpyRealTimeViewModel TagsSpyRealTime { get; }

        #endregion

        #region Constructor

        static TagsSpyViewModel()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(TagsSpyResources)));
        }

        public TagsSpyViewModel(IMessageDataBus messageDataBus)
        {
            if (messageDataBus is null)
            {
                throw new ArgumentNullException(nameof(messageDataBus));
            }

            TagsSpyList = new TagsSpyListViewModel(messageDataBus, OnTagAddedToSpy, OnTagRemovedFromSpy);
            TagsSpyRealTime = new TagsSpyRealTimeViewModel();
        }

        #endregion Constructor

        #region Privates

        private void OnTagAddedToSpy(BaseTag tag)
        {
            TagsSpyRealTime.AddTag(tag);
        }

        private void OnTagRemovedFromSpy(BaseTag tag)
        {
            TagsSpyRealTime.RemoveTag(tag);
        }

        #endregion
    }
}
