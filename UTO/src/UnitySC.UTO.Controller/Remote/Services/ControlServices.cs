using System;
using System.Runtime.CompilerServices;

using Agileo.Common.Logging;
using Agileo.Semi.Gem.Abstractions.E30;

namespace UnitySC.UTO.Controller.Remote.Services
{
    internal class ControlServices : E30StandardSupport
    {
        #region Constructor

        public ControlServices(IE30Standard e30Standard, ILogger logger)
            : base(e30Standard, logger)
        {
        }

        #endregion Constructor

        #region Methods

        public void SetCommunicationStateEnable()
        {
            TryCatch(() => E30Standard?.ControlServices.EnableCommunication());
            LogMethodCalled();
        }

        public void SetCommunicationStateDisable()
        {
            TryCatch(() => E30Standard?.ControlServices.DisableCommunication());
            LogMethodCalled();
        }

        public void SetEquipmentStateOffLine()
        {
            TryCatch(() => E30Standard?.ControlServices.SwitchToOffline());
            LogMethodCalled();
        }

        public void SetEquipmentStateOnLine()
        {
            TryCatch(() => E30Standard?.ControlServices.SwitchToOnline());
            LogMethodCalled();
        }

        public void SetLocalMode()
        {
            TryCatch(() => E30Standard?.ControlServices.SwitchToLocal());
            LogMethodCalled();
        }

        public void SetRemoteMode()
        {
            TryCatch(() => E30Standard?.ControlServices.SwitchToRemote());
            LogMethodCalled();
        }

        private void TryCatch(Action activity)
        {
            try
            {
                activity();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void LogMethodCalled([CallerMemberName] string memberName = null)
        {
            Logger.Info($"{memberName} called");
        }

        #endregion Methods

        #region Properties

        public HSMSState HsmsState
        {
            get
            {
                if (E30Standard?.ControlServices != null)
                {
                    return E30Standard.ControlServices.HSMSState;
                }
                return HSMSState.NotConnected;
            }
        }

        public ControlState ControlState
        {
            get
            {
                if (E30Standard?.ControlServices != null)
                {
                    return E30Standard.ControlServices.ControlState;
                }
                return ControlState.OffLine;
            }
        }

        public CommunicationState CommunicationState
        {
            get
            {
                try
                {
                    if (E30Standard?.ControlServices?.CommunicationState != null)
                    {
                        return E30Standard.ControlServices.CommunicationState;
                    }
                }
                catch
                {
                    // ignored
                }

                return CommunicationState.Disabled;
            }
        }

        #endregion Properties

        #region BublingEvents

        public event EventHandler<CommunicationStateChangedEventArgs> OnCommunicationStateChanged
        {
            add { E30Standard.ControlServices.CommunicationStateChanged += value; }
            remove { E30Standard.ControlServices.CommunicationStateChanged -= value; }
        }

        public event EventHandler<ControlStateChangedEventArgs> OnControlStateChanged
        {
            add { E30Standard.ControlServices.ControlStateChanged += value; }
            remove { E30Standard.ControlServices.ControlStateChanged -= value; }
        }

        public event EventHandler<HSMSStateChangedEventArgs> OnHsmsStateChanged
        {
            add { E30Standard.ControlServices.HSMSStateChanged += value; }
            remove { E30Standard.ControlServices.HSMSStateChanged -= value; }
        }

        #endregion BublingEvents
    }
}
