using System;
using System.Collections.Generic;

using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;

namespace UnitySC.Shared.Dataflow.Shared
{
    public class DataflowRecipe
    {
        private object _startedDataflowLock = new object();
        public object StartedDataflowLock { get { return _startedDataflowLock; } }

        public Guid KeyForAllVersion { get; set; }
        public int Version { get; set; }

        public DataflowRecipeComponent DataflowRecipeComponent { get; set; }

        public DataflowRecipeInfo DataflowRecipeInfo { get; set; }


        /// <summary>
        /// the key is the actor's identifier (id)
        /// </summary>
        public SortedDictionary<string, DataflowActor> Actors { get; } = new SortedDictionary<string, DataflowActor>();

        ///// <summary>
        ///// List of started dataflows indexed by waferId
        ///// </summary>
        public SortedDictionary<Guid, DataflowInstance> StartedDataflow { get; } = new SortedDictionary<Guid, DataflowInstance>();
    }
}
