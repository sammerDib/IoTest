using System.Linq;
using System.Runtime.Serialization;

using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.DataAccess.Dto
{
    public partial class Recipe
    {
        [DataMember]
        public ActorType Type
        {
            get
            {
                return (ActorType)ActorType;
            }
            set
            {
                ActorType = (int)value;
            }
        }

        public void AddInput(ResultType resultType)
        {
            Inputs.Add(new Dto.Input() { Recipe = this, ResultType = (int)resultType });
        }

        public void AddOutput(ResultType resultType)
        {
            Outputs.Add(new Dto.Output() { Recipe = this, ResultType = (int)resultType });
        }

        public DataflowRecipeComponent ToDataflowRecipeComponent()
        {
            //note de RTI : quid des recipes archived ici ?
            var result = new DataflowRecipeComponent
            {
                Name = Name,
                Key = KeyForAllVersion,
                ActorType = Type,
                Version = Version,
                Comment = Comment,
                Inputs = Inputs.Select(x => (ResultType)x.ResultType).ToList(),
                Outputs = Outputs.Select(x => (ResultType)x.ResultType).ToList(),
                IsShared = IsShared,
                IsValidated = IsValidated
            };
            return result;
        }
    }
}
