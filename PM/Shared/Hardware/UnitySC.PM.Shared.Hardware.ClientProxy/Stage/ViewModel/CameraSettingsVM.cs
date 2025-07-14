using System;
using System.Windows;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Stage.ViewModel
{
    public class CameraSettingsVM : ViewModelBase
    {
        public CameraSettingsVM()
        {
            if (!IsInDesignMode)
                throw new ApplicationException("This constructor is for design mode only.");
        }

        private delegate void Delegate();

        //=================================================================
        // Commandes
        //=================================================================

        #region Commandes

        private RelayCommand _sendCommandR;

        public RelayCommand SendCommandR
        {
            get
            {
                return _sendCommandR ?? (_sendCommandR = new RelayCommand(
              () =>
              {
                  //do things
                  MessageBox.Show("Debug!");
              },
              () => { return true; }));
            }
        }

        #endregion Commandes

        //=================================================================
        // Utilitaires
        //=================================================================
    }
}