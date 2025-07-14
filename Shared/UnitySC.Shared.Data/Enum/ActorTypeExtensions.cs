using System.Collections.Generic;

namespace UnitySC.Shared.Data.Enum
{
    public static class ActorTypeExtensions
    {
        private static readonly Dictionary<ActorType, string> s_actor_to_label = new Dictionary<ActorType, string>()
        {
            { ActorType.Unknown , "|Unknown|" },

            { ActorType.DEMETER , "Demeter" },
            { ActorType.BrightField2D , "BF2D" },
            { ActorType.Darkfield , "DV" },      //obsolete
            { ActorType.BrightFieldPattern , "BF" },
            { ActorType.Edge , "Edge" },
            { ActorType.NanoTopography , "Nano" },   //obsolete old nanotopo module
            { ActorType.LIGHTSPEED , "Lightspeed" }, // should later be replace by Hélios
            { ActorType.BrightField3D , "BF3D" },
            { ActorType.EdgeInspect , "Edge (3 Sensors)" }, //obsolete
            { ActorType.ANALYSE , "Analyse" },
            { ActorType.HardwareControl , "HW Ctrl" },
            { ActorType.HeLioS , "Helios" },
            { ActorType.Argos , "Argos" },
            { ActorType.EMERA , "Emera" },
            { ActorType.Wotan , "Wotan" },
            { ActorType.Thor , "Thor" },

            { ActorType.ADC , "|ADC|" },

            { ActorType.DataflowManager , "|DataFlow Mgr|" },
            { ActorType.DataAccess , "|DataAccess Mgr|" }
        };


        public static string GetLabelName(this ActorType actorType)
        {
            if (!s_actor_to_label.ContainsKey(actorType))
                return "<NotDefined>";
            return s_actor_to_label[actorType];
        }

        public static string GetActorLabelName(this ResultType resType)
        {
            return resType.GetActorType().GetLabelName();
        }

    }
}
