using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Shared.Data.Enum;

namespace AcquisitionAdcExchange
{
    public static class DataLoaderHelper
    {
        public static string GetResultTypeNameWithCheck(ActorType actorType, ResultType resType)
        {
            if(resType.GetActorType() != actorType)
                throw new ApplicationException($"invalid Result : {resType} for actor <{actorType}>");

            return GetResultTypeName(resType);
        }

        public static string GetResultTypeName(ResultType resType)
        {
            return Enum.GetName(typeof(ResultType), resType);
        }

         public static string GetImageSuffixWithCheck(ActorType actorType, ResultType resType)
        {
            if(resType.GetActorType() != actorType)
                throw new ApplicationException($"invalid Result : {resType} for actor <{actorType}> while getting image suffix");

            return GetImageSuffix(resType);
        }

        public static string GetImageSuffix(ResultType resType)
        {
            // image suffix should not have actor name or side in its name
            // usually standard resulttype enum name is <ACTOR>_<resultName>_<Side> (side coumd be optionnal)
            // 
            string resName = Enum.GetName(typeof(ResultType), resType);
            var fields = resName.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries); 
            if(fields.Length > 1)
                return fields[1];
            
            throw new ApplicationException($"invalid suffix in : {resName}, Bad Result Type naming");
        }

    }
}
