using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.DataAccess.Dto.ModelDto.LocalDto
{
    [DataContract]
    public class DataflowRecipeComponent
    {
        [DataMember]
        public Guid Key { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public ActorType ActorType { get; set; }

        [DataMember]
        public bool IsShared { get; set; }

        [DataMember]
        public bool IsValidated { get; set; }

        [DataMember]
        public List<DataflowRecipeAssociation> ChildRecipes = new List<DataflowRecipeAssociation>();

        [DataMember]
        public DataflowRecipeComponentState State { get; set; }

        [DataMember]
        public string ActorID { get; set; }

        [DataMember]
        public int Version { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public List<ResultType> Inputs { get; set; }

        [DataMember]
        public List<ResultType> Outputs { get; set; }

        public ActorCategory ActorCategory => ActorType.GetCatgory();

        public IEnumerable<DataflowRecipeComponent> AllChilds()
        {
            var childs = new List<DataflowRecipeComponent>();
            RecipeComponentChilds(childs, this);
            return childs;
        }

        /// <summary>
        /// Get childsrecursive
        /// </summary>
        /// <param name="childs"></param>
        /// <param name="dataflowRecipeComponent"></param>
        private void RecipeComponentChilds(List<DataflowRecipeComponent> childs, DataflowRecipeComponent dataflowRecipeComponent)
        {
            if (!childs.Any(x => x.Key == dataflowRecipeComponent.Key))
                childs.Add(dataflowRecipeComponent);

            foreach (var component in dataflowRecipeComponent.ChildRecipes.Select(x => x.Component))
            {
                RecipeComponentChilds(childs, component);
            }
        }
    }
}
