using System;
using System.Collections.Generic;
using System.ServiceModel;

using AdcRobotExchange;

namespace ADCEngine
{
    ///////////////////////////////////////////////////////////////////////
    ///<summary>
    /// Surcouche pour accèder à ITransferToRobot afin d'ignorer les erreurs 
    /// et ne pas bloquer l'ADC.
    ///</summary>
    ///////////////////////////////////////////////////////////////////////
    public class TransferToRobotStub : ITransferToRobot
    {
        /// <summary> Doit-on se connecter au robot ? </summary>
        private bool isEnabled;
        private ITransferToRobot Implementation;
        private ChannelFactory<ITransferToRobot> TransferToRobotChannelFactory;

        ///=================================================================
        ///<summary>
        /// Attach une implementation au stub
        ///</summary>
        ///=================================================================
        public void Attach(ITransferToRobot implementation)
        {
            Implementation = implementation;
            isEnabled = true;
        }

        ///=================================================================
        ///<summary>
        /// Connection au service WCF
        ///</summary>
        ///=================================================================
        public void Connect()
        {
            // Création de la ChannelFactory
            //..............................
            TransferToRobotChannelFactory = new ChannelFactory<ITransferToRobot>("ITransferToRobot");
            ADC.log("Connecting to robot service on \"" + TransferToRobotChannelFactory.Endpoint.Address + "\"");

            // Création du Channel
            //....................
            Implementation = TransferToRobotChannelFactory.CreateChannel();
            ((ICommunicationObject)Implementation).Faulted += TransferToRobot_Faulted;
            isEnabled = true;
        }

        ///=================================================================
        ///<summary>
        /// Arrête le client WCF
        ///</summary>
        ///=================================================================
        public void Disconnect()
        {
            ICommunicationObject com = Implementation as ICommunicationObject;
            if (com != null)
            {
                ADC.log("Close connection to robot service");
                com.Abort();
                Implementation = null;
            }
            isEnabled = false;
        }

        ///=================================================================
        ///<summary>
        /// Reconnecte le client WCF si besoin
        ///</summary>
        ///=================================================================
        public void VerifyConnection()
        {
            if (!isEnabled)
                return;

            if (Implementation == null)
            {
                Implementation = TransferToRobotChannelFactory.CreateChannel();
                ((ICommunicationObject)Implementation).Faulted += TransferToRobot_Faulted;
            }
        }

        //=================================================================
        //
        //=================================================================
        private void TransferToRobot_Faulted(object sender, EventArgs e)
        {
            if (sender != Implementation)
                throw new ApplicationException("WCF fault on unkown object: " + sender);

            ICommunicationObject com = (ICommunicationObject)sender;
            com.Faulted -= new EventHandler(TransferToRobot_Faulted);
            com.Abort();

            Implementation = null;
        }

        //=================================================================
        // Implémentation de ITransferToRobot
        //=================================================================
        public void TransferVids(string Toolname, string UniqueID, List<VidBase> VidList)
        {
            try
            {
                if (!isEnabled)
                    return;
                VerifyConnection();
                Implementation.TransferVids(Toolname, UniqueID, VidList);
            }
            catch (Exception ex)
            {
                // Ne pas bloquer l'ADC si le robot est inaccessible
                ADC.logWarning("Failed to transfer VIDs to Robot: " + ex.Message);
            }
        }

        public void TransferInputList(string Toolname, string UniqueID, List<AdcInput> AdcInputList)
        {
            try
            {
                if (!isEnabled)
                    return;
                VerifyConnection();
                Implementation.TransferInputList(Toolname, UniqueID, AdcInputList);
            }
            catch (Exception ex)
            {
                // Ne pas bloquer l'ADC si le robot est inaccessible
                ADC.logWarning("Failed to transfer InputList to Robot: " + ex.Message);
            }
        }

        public void TransferWaferReport(string Toolname, string UniqueID, WaferReport WaferReport)
        {
            bool robotEnable = false;
            try
            {
                if (!isEnabled)
                    return;
                VerifyConnection();
                robotEnable = true;
            }
            catch (Exception ex)
            {
                // Ne pas bloquer l'ADC si le robot est inaccessible
                ADC.logWarning("No connection with the Host : " + ex.Message);
            }
            try
            {
                if (!isEnabled)
                    return;
                if (robotEnable)
                    Implementation.TransferWaferReport(Toolname, UniqueID, WaferReport);
            }
            catch (Exception ex)
            {
                // Ne pas bloquer l'ADC si le robot est inaccessible
                ADC.logWarning("Failed to transfer WaferReport to Robot: " + ex.Message);
            }
        }

        public void TransferDataBaseStatus(string Toolname, eDataBaseType DBType, bool Connected)
        {
            try
            {
                if (!isEnabled)
                    return;
                VerifyConnection();
                Implementation.TransferDataBaseStatus(Toolname, DBType, Connected);
            }
            catch (Exception ex)
            {
                // Ne pas bloquer l'ADC si le robot est inaccessible
                ADC.logWarning("Failed to transfer DataBase Status to Robot: " + ex.Message);
            }
        }


    }
}
