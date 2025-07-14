using System;
using System.ComponentModel;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.ClientProxy.Chamber;
using UnitySC.PM.Shared.UI.Main;

namespace UnitySC.PM.DMT.CommonUI.ViewModel.Hardware
{
    public class HandlingVM : ObservableRecipient, IMenuContentViewModel, IDisposable
    {
        public HandlingVM()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;
        }

        public bool IsEnabled => true;

        public bool CanClose()
        {
            return true;
        }

        public void Dispose()
        {
        }

        public void Refresh()
        {
        }
    }
}
