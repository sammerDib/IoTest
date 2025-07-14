using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using UnitySC.DataAccess.Dto.ModelDto.LocalDto;

namespace UnitySC.PM.ANA.Service.Interface.Recipe
{   
    [DataContract]
    public class ResultFoldersPath
    {
        [DataMember]
        public string RecipeFolderPath { get; set; }

        [DataMember]
        public string ExternalFileFolderName { get; set; }

        [DataMember]
        public string ExternalFilePrefix { get; set; }

        public OutPreRegister PreRegisterResult{ get; set; }


    }
}
