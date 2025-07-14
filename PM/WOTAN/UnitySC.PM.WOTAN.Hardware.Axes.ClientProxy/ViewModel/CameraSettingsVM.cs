using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Axes.ViewModel
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


        #endregion

        //=================================================================
        // Utilitaires
        //=================================================================
        #region Utilitaires
        
        #endregion

    }
}
