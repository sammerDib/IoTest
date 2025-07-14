using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.Shared.UI.ViewModel
{
    /// <summary>
    /// View Model Base with Extension
    /// </summary>
    public class ViewModelBaseExt : ObservableRecipient
    {
        private IEnumerable<PropertyInfo> _relayCommands = null;

        protected void UpdateAllCanExecutes()
        {
            Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
            {
                var typeMvvm = typeof(RelayCommand);
                var typeUnity = typeof(AutoRelayCommand);

                var ObjectType = this.GetType();

                if (_relayCommands == null)
                {
                    var propertyInfos = ObjectType.GetProperties();
                    _relayCommands = propertyInfos.Where(p => p.PropertyType.Name.Contains(typeMvvm.Name) || p.PropertyType.Name.Contains(typeUnity.Name));
                }

                foreach (var relayCommand in _relayCommands)
                {
                    var notifyCanExecuteChanged = relayCommand.PropertyType.GetMethod("NotifyCanExecuteChanged");
                    notifyCanExecuteChanged?.Invoke(relayCommand.GetValue(this), null);
                }
            }));
        }
    }
}
