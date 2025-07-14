using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.Shared.FDC
{
    [Serializable]
    public class PersistentFDCsData
    {
        public List<IPersistentFDCData> Data { get; set; }

        public Dictionary<string, FDCStatus> FdcsStatus { get; set; }


        public PersistentFDCsData()
        {
            Data = new List<IPersistentFDCData>();
            FdcsStatus = new Dictionary<string, FDCStatus>();
        }

        public static PersistentFDCsData Load(string fdcPersistentFilePath)
        {
            try
            {
                using (FileStream fileStream = new FileStream(fdcPersistentFilePath, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    var newPersistentFDCsData= (PersistentFDCsData)formatter.Deserialize(fileStream);
                    if (newPersistentFDCsData != null) 
                    { 
                        if (newPersistentFDCsData.Data == null)
                            newPersistentFDCsData.Data= new List<IPersistentFDCData>();
                        if (newPersistentFDCsData.FdcsStatus == null)
                            newPersistentFDCsData.FdcsStatus = new Dictionary<string, FDCStatus>();
                    }

                    return newPersistentFDCsData;
                }
            }
            catch (FileNotFoundException ex)
            {
                var logger = ClassLocator.Default.GetInstance<ILogger>();
                logger.Warning($"Persistent FDCs data : {fdcPersistentFilePath} file is missing : {ex.Message}");
                return null;
            }
            catch (Exception e)
            {
                var logger = ClassLocator.Default.GetInstance<ILogger>();
                logger.Error($"Unable to load the persistent FDCs data : {fdcPersistentFilePath} : {e.Message}");
                return null;
            }
        }

        public void Save(string fdcPersistentFilePath)
        {
             try
            {
                using (FileStream fileStream = new FileStream(fdcPersistentFilePath, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fileStream, this);
                }
               
            }
            catch (Exception e)
            {
                var logger = ClassLocator.Default.GetInstance<ILogger>();
                logger.Error($"Unable to save the persistent FDCs data : {fdcPersistentFilePath} : {e.Message}");
            }
        }
    }
}
