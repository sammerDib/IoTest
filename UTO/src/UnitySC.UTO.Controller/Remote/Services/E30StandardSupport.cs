using System;

using Agileo.Common.Logging;
using Agileo.Semi.Gem.Abstractions.E30;

namespace UnitySC.UTO.Controller.Remote.Services
{
    internal abstract class E30StandardSupport : IDisposable
    {
        #region Fields

        protected readonly IE30Standard E30Standard;
        protected readonly ILogger Logger;
        protected Agileo.EquipmentModeling.Equipment Equipment;
        private bool _disposedValue;

        #endregion Fields

        #region Constructors

        protected E30StandardSupport(IE30Standard e30Standard, ILogger logger)
        {
            E30Standard = e30Standard ?? throw new ArgumentNullException(nameof(e30Standard));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion Constructors

        #region Public Methods

        public virtual void OnCreate()
        {
        }

        public virtual void OnSetup(Agileo.EquipmentModeling.Equipment equipment)
        {
            Equipment = equipment;
        }

        public bool IsLocalMode()
        {
            return E30Standard.ControlServices.ControlState != ControlState.Remote;
        }

        #endregion

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // dispose managed code here
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}
