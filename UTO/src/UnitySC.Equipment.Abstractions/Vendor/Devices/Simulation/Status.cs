using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.Simulation
{
    public abstract class Status : INotifyPropertyChanged
    {
        #region Constructor

        protected Status(string name)
        {
            Name = name;
        }

        #endregion Constructor

        #region Properties

        public string Name { get; }

        #endregion Properties

        #region Methods

        /// <summary>Sets and raises the value if changed.</summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="backingField">A reference to the backing field of the property.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="propertyName">
        ///     The name of the property, usually automatically provided through the CallerMemberName
        ///     attribute.
        /// </param>
        /// <returns><c>true</c> if the value has changed; otherwise <c>false</c>.</returns>
        protected bool SetAndRaiseIfChanged<T>(
            ref T backingField,
            T newValue,
            [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, newValue))
            {
                return false;
            }

            backingField = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged
    }
}
