using System;
using System.Collections.Generic;

using AcquisitionAdcExchange;

using ADCEngine;

using BasicModules.DataLoader;

using UnitySC.Shared.Data.Enum;

namespace DataLoaderModule_LightSpeed
{
    public class LightSpeedHazeModule : FullImageDataLoaderBase
    {
        public enum eDirection { Forward, Backward, Total };
        public override string DisplayName { get { return Factory.ModuleName + "-" + Id + "\n" + LayerName; } }
        public override string DisplayNameInParamView { get { return Factory.Label + "\n Haze"; } }
        public override string LayerName { get { return "Haze -" + paramDirection.Value.ToString(); } }

        public override ActorType DataLoaderActorType => ActorType.HeLioS;
        public override IEnumerable<ResultType> CompatibleResultTypes => GetExpectedResultTypes();

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly EnumParameter<eDirection> paramDirection;

        //=================================================================
        // Constructeur
        //=================================================================
        public LightSpeedHazeModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramDirection = new EnumParameter<eDirection>(this, "Direction");
        }

        //=================================================================
        //
        //=================================================================
        public override bool FilterImage(ResultType resultType, int NoCumn = -1)
        {
            if (resultType.GetActorType() != ActorType.HeLioS)
                return false;

            return GetExpectedResultTypes().Contains(resultType);
        }

        private List<ResultType> GetExpectedResultTypes()
        {
            List<ResultType> expectedResTypes = new List<ResultType>();
            if (paramDirection.Value == eDirection.Forward) //aka WIDE
            {
                expectedResTypes.Add(ResultType.HLS_Haze_WideFW); // (int)eChannelID.Haze_Forward;

            }
            else if (paramDirection.Value == eDirection.Backward) // AKA narrow
            {
                expectedResTypes.Add(ResultType.HLS_Haze_NarrowBW); // (int)eChannelID.Haze_Backward;

            }
            else if (paramDirection.Value == eDirection.Total) // both 
            {
                expectedResTypes.Add(ResultType.HLS_Haze_Total); // (int)eChannelID.Haze_Tot;
            }
            else
            {
                throw new ApplicationException("unkown image type: " + paramDirection.Value);
            }

            return expectedResTypes;
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();
            Layer.name = "Haze";
        }

    }
}
