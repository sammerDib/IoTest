using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.TC.Shared
{
    [Serializable]
    public class InitMaterialInfo
    {
        public MaterialPresence MaterialPresence;
        public Length MaterialDimension;
    }

    [Serializable]
    public static class InitMaterialInfoManagementInFile
    {
        public static List<InitMaterialInfo> GetMaterialInfo()
        {
            var MaterialInfos = new List<InitMaterialInfo>();        
            try
            {
                MaterialInfos = XML.Deserialize<List<InitMaterialInfo>>("C:\\Temp\\Init_MaterialInfo.xml");
                return MaterialInfos;
            }
            catch 
            {
                var materialInfo = new InitMaterialInfo() { MaterialPresence = MaterialPresence.NotPresent, MaterialDimension = new Length(300, LengthUnit.Millimeter) };
                MaterialInfos.Add(materialInfo);
                ResetPresence(MaterialInfos);                
            }
            return MaterialInfos;
        }

        public static void ResetPresence(List<InitMaterialInfo> initMatInfos)
        {
            initMatInfos.ForEach(materialInfo => materialInfo.MaterialPresence = MaterialPresence.NotPresent);
            if (!Directory.Exists("C:\\Temp)"))
                Directory.CreateDirectory("C:\\Temp");
            XML.Serialize(initMatInfos, "C:\\Temp\\Init_MaterialInfo.xml");        
        }
        public static void SetPresence(Length slotSize, List<InitMaterialInfo> initMatInfos)
        {
            var materialInfo = initMatInfos.FirstOrDefault(mi=> mi.MaterialDimension == slotSize); 
            if (materialInfo != null)
                materialInfo.MaterialPresence = MaterialPresence.Present;
            if (!Directory.Exists("C:\\Temp)"))
                Directory.CreateDirectory("C:\\Temp");
            XML.Serialize(initMatInfos, "C:\\Temp\\Init_MaterialInfo.xml");
        }
    }


}
