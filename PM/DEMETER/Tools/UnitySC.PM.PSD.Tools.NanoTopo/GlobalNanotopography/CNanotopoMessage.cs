using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Drawing.Imaging;

namespace UnitySC.PM.PSD.Tools.NanoTopo.GlobalNanotopography
{
    //public enum enumWaferStatus { wsUnprocessed = 0, wsProcessing, wsProcessed, wsProcessError };
    //public enum enumCommandExchange { ceGetVersion = 0, ceGetWaferStatus, ceGetWaferResult, ceResetWaferStatus, ceAbort };
    //public enum enumStatusExchange { saUnknown = 0, saOk, saError };
    //public enum enumError { eNoError=0, eUndefineError=1, eSetupError=2, eProcessError=4, eFullDiskError=8}

    ///**
    //*****************************************************************************************
    //* \class CMessage
    //* \brief Classe d'échange de messages avec le PC Supervision, permet de transmettre les statuts et les resultats des wafers processés
    //* 
    //* 
    //*****************************************************************************************/
    //[Serializable]
    //public class CNanotopoMessage
    //{        
    //    [Serializable]
    //    public class CProcessParameters
    //    {
    //        // Ajouter ici les parametres recettes utiles
    //        public CProcessParameters() { }            
    //    }

    //    [Serializable]
    //    public class CWaferStatusByLoadport
    //    {
    //        enumWaferStatus[] WaferStatus;
    //    }

    //    public CProcessParameters Parameters;
    //    public CWaferStatusByLoadport[] WaferStatusByLoadport;


    //    public enumCommandExchange Command;
    //    public enumStatusExchange Status;
    //    public enumError Error;
    //    public String Description;

        
    //}

}
