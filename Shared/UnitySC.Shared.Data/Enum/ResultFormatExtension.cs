using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UnitySC.Shared.Data.Enum
{
    public static class ResultFormatExtension
    {
        // extension to resformat
        private static readonly Dictionary<int, string> s_resextid_to_ext = new Dictionary<int, string>()
        { 
             //Inspection - PostProcessing Results
            { PMEnumHelper.GetResultExtensionId(ResultType.ADC_Klarf),     "001" }, // Klarf,
            { PMEnumHelper.GetResultExtensionId(ResultType.ADC_ASE),       "ase" }, // ASE,
            { PMEnumHelper.GetResultExtensionId(ResultType.ADC_ASO),       "aso" }, // ASO,
            { PMEnumHelper.GetResultExtensionId(ResultType.ADC_DFHaze),    "aze" }, // DFHaze,
            { PMEnumHelper.GetResultExtensionId(ResultType.ADC_Crown),     "crw" }, // Crown,
            { PMEnumHelper.GetResultExtensionId(ResultType.ADC_YieldMap),  "yld" }, // YieldMap,
            { PMEnumHelper.GetResultExtensionId(ResultType.ADC_EyeEdge),   "edg" }, // EyeEdge,
            { PMEnumHelper.GetResultExtensionId(ResultType.ADC_GlobalTopo),"gtr" }, // GlobalTopo,
            { PMEnumHelper.GetResultExtensionId(ResultType.ADC_HeightMes), "hmr" }, // HeightMes,
            { PMEnumHelper.GetResultExtensionId(ResultType.ADC_Haze),      "haze" },// Haze  (lightspeedd)
            
            //Metrology Analyse
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_Thickness),  "anathick" },     // Thickness,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_TSV),        "anatsv" },       // TSV,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_Trench),     "anatch" },       // Trench,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_Pillar),     "anapil" },       // Pillar,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_Step),       "anastp" },       // Step,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_PeriodicStructure), "anaps" }, // PeriodicStructure,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_NanoTopo),   "anantp" },       // NanoTopo,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_Overlay),    "anaovl" },       // Overlay,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_CD),         "anacd" },        // CD,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_EBR),        "anaebr" },       // EBR,          
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_Topography), "anatopo" },      // Topography,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_Bow),        "anabow" },       // Bow,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_Warp),       "anawarp" },      // Warp,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_EdgeTrim),   "anaedt" },       // EdgeTrim,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_Roughness),  "anargh" },       // EdgeTrim,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_XYCalibration), "anaxycal" },  // XYCalibration,

            //Result category : Unknow, Acquisition & Config should not be treated as a Result
        };

        // Extension to ResultType
        private static readonly Dictionary<string, ResultType> s_resExt_to_resultType = new Dictionary<string, ResultType>
        { 
             //Inspection - PostProcessing Results
            { "001" , ResultType.ADC_Klarf     }, // Klarf,
            { "ase" , ResultType.ADC_ASE       }, // ASE,
            { "aso" , ResultType.ADC_ASO       }, // ASO,
            { "aze" , ResultType.ADC_DFHaze    }, // DFHaze,
            { "crw" , ResultType.ADC_Crown     }, // Crown,
            { "yld" , ResultType.ADC_YieldMap  }, // YieldMap,
            { "edg" , ResultType.ADC_EyeEdge   }, // EyeEdge,
            { "gtr" , ResultType.ADC_GlobalTopo}, // GlobalTopo,
            { "hmr" , ResultType.ADC_HeightMes }, // HeightMes,
            { "haze", ResultType.ADC_Haze      }, // Haze  (lightspeedd)
            
            //Metrology Analyse
            { "anathick", ResultType.ANALYSE_Thickness        }, // Thickness,
            { "anatsv",   ResultType.ANALYSE_TSV              }, // TSV,
            { "anatch",   ResultType.ANALYSE_Trench           }, // Trench,
            { "anapil",   ResultType.ANALYSE_Pillar           }, // Pillar,
            { "anastp",   ResultType.ANALYSE_Step             }, // Step,
            { "anaps",    ResultType.ANALYSE_PeriodicStructure}, // PeriodicStructure,
            { "anantp",   ResultType.ANALYSE_NanoTopo         }, // NanoTopo,
            { "anaovl",   ResultType.ANALYSE_Overlay          }, // Overlay,
            { "anacd",    ResultType.ANALYSE_CD               }, // CD,
            { "anaebr",   ResultType.ANALYSE_EBR              }, // EBR,          
            { "anatopo",  ResultType.ANALYSE_Topography       }, // Topography,
            { "anabow",   ResultType.ANALYSE_Bow              }, // Bow,
            { "anawarp",  ResultType.ANALYSE_Warp             }, // Warp,
            { "anaedt",   ResultType.ANALYSE_EdgeTrim         }, // EdgeTrim,
            { "anargh",   ResultType.ANALYSE_Roughness        }  // EdgeTrim
        };

        // extensionFormatID to Label
        private static readonly Dictionary<int, string> s_resextid_to_LabelName = new Dictionary<int, string>()
        { 
             //Inspection - PostProcessing Results
            { PMEnumHelper.GetResultExtensionId(ResultType.ADC_Klarf),     "Klarf Defects" }, // Klarf,
            { PMEnumHelper.GetResultExtensionId(ResultType.ADC_ASE),       "ASE" }, // ASE,
            { PMEnumHelper.GetResultExtensionId(ResultType.ADC_ASO),       "ASO Clusters" }, // ASO,
            { PMEnumHelper.GetResultExtensionId(ResultType.ADC_DFHaze),    "DF Haze" }, // DFHaze,
            { PMEnumHelper.GetResultExtensionId(ResultType.ADC_Crown),     "Crown" }, // Crown,
            { PMEnumHelper.GetResultExtensionId(ResultType.ADC_YieldMap),  "Yieldmap" }, // YieldMap,
            { PMEnumHelper.GetResultExtensionId(ResultType.ADC_EyeEdge),   "Edge" }, // EyeEdge,
            { PMEnumHelper.GetResultExtensionId(ResultType.ADC_GlobalTopo),"Globaltopo" }, // GlobalTopo,
            { PMEnumHelper.GetResultExtensionId(ResultType.ADC_HeightMes), "Height Measures" }, // HeightMes,
            { PMEnumHelper.GetResultExtensionId(ResultType.ADC_Haze),      "Haze" },// Haze  (lightspeedd)
            
            //Metrology Analyse
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_Thickness),  "Thickness" },  // Thickness,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_TSV),        "TSV" },        // TSV,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_Trench),     "Trench" },     // Trench,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_Pillar),     "Pillar" },     // Pillar,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_Step),       "Step" },       // Step,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_PeriodicStructure), "Periodic Structure" }, // PeriodicStructure,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_NanoTopo),   "NanoTopo" },  // Rougthness,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_Overlay),    "Overlay" },   // Overlay,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_CD),         "Critical Dim." }, // CD,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_EBR),        "EBR" },       // EBR,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_Topography), "Topography" },// Topography,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_Bow),        "Bow" },       // Bow,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_Warp),       "Warp" },      // Warp,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_EdgeTrim),   "EdgeTrim" },  // EdgeTrim,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_Roughness),  "Roughness" },  // Roughness,
            { PMEnumHelper.GetResultExtensionId(ResultType.ANALYSE_XYCalibration), "XYCalibration" },  // XYCalibration,

            //Result category : Unknow, Acquisition & Config should not be treated as a Result
        };

        public static string GetExt(this ResultType resType)
        {
            if (System.Enum.IsDefined(typeof(ResultType), resType))
            {
                if (ResultCategory.Result == PMEnumHelper.GetResultCategory(resType))
                    return GetExt(PMEnumHelper.GetResultExtensionId(resType));
                throw new ArgumentException($"Bad Result type for Result format Extensions : use category {PMEnumHelper.GetResultCategory(resType)},  Only category Result is allowed ");
            }
            throw new ArgumentException("Unkown Result type for Result format Extensions");
        }

        public static string GetExt(int resTypeIdx)
        {
            if (s_resextid_to_ext.ContainsKey(resTypeIdx))
                return s_resextid_to_ext[resTypeIdx];
            throw new ArgumentException("Result type Index overflow : Result Extension ID does not exist");
        }

        public static ResultType GetResultTypeFromExtension(string sExtension)
        {
            string sSeachExt = sExtension.Trim(new char[] { '.', ' ' });
            sSeachExt = sSeachExt.ToLower();

            if (s_resExt_to_resultType.TryGetValue(sSeachExt, out var resultType))
            {
                return resultType;
            }
            return ResultType.NotDefined;
        }

        public static ResultFormat GetTypeFromExtension(string sExtension)
        {
            int resextid = GetExtIdFromExtension(sExtension);
            return PMEnumHelper.GetResultFormatFromResultExtensionId(resextid);
        }

        public static int GetExtIdFromExtension(string sExtension)
        {
            string sSeachExt = sExtension.Trim(new char[] { '.', ' ' });
            sSeachExt = sSeachExt.ToLower();

            if (s_resextid_to_ext.ContainsValue(sSeachExt))
            {
                int resextid = s_resextid_to_ext.FirstOrDefault(x => x.Value == sSeachExt).Key;
                return resextid;
            }
            throw new ArgumentException(string.Format("Result format did not exist for extension {0}", sExtension));
        }

        public static int GetExtIdFromExtensionOrDefault(string sExtension)
        {
            string sSeachExt = sExtension.Trim(new char[] { '.', ' ' });
            sSeachExt = sSeachExt.ToLower();

            if (s_resextid_to_ext.ContainsValue(sSeachExt))
            {
                int resextid = s_resextid_to_ext.FirstOrDefault(x => x.Value == sSeachExt).Key;
                return resextid;
            }
            return -1;
        }

        public static bool IsValidExtensionId(int extensionId)
        {
            return s_resextid_to_ext.ContainsKey(extensionId);
        }

        public static string GetLabelName(this ResultType resType)
        {
            if (System.Enum.IsDefined(typeof(ResultType), resType))
            {
                switch (PMEnumHelper.GetResultCategory(resType))
                {
                    case ResultCategory.Result:
                        return GetLabelName(PMEnumHelper.GetResultExtensionId(resType));
                    case ResultCategory.Acquisition:
                        return GetAcquisitionLabelName(resType);
                    default:
                        throw new ArgumentException($"Bad Result type ({resType}) for Result format Extensions for label name: use category {PMEnumHelper.GetResultCategory(resType)},  Only category Result or Acquisition is allowed ");
                }
            }
            throw new ArgumentException("Unknown Result type for Result format Extensions for label name");
        }

        public static string GetLabelName(int resTypeIdx)
        {
            if (s_resextid_to_LabelName.ContainsKey(resTypeIdx))
                return s_resextid_to_LabelName[resTypeIdx];
            throw new ArgumentException("Result type Index overflow : Result Extension ID does not exist for label name");
        }

        private static string GetAcquisitionLabelName(ResultType resTyp)
        {
            int numVal = (int)resTyp;
            // category et existence ont été testé aupravant dans appel à get label
            string AcqLabelName;
            switch (resTyp.GetActorType())
            {
                case ActorType.DEMETER:
                    if (!System.Enum.IsDefined(typeof(Module.DMTResult), numVal))
                        throw new ArgumentException($"DMTResult for such a result type is not defined <{resTyp}>");
                    AcqLabelName = ((Module.DMTResult)numVal).ToString();
                    break;
                case ActorType.HeLioS:
                    if (!System.Enum.IsDefined(typeof(Module.HLSResult), numVal))
                        throw new ArgumentException($"HLSResult for such a result type is not defined <{resTyp}>");
                    AcqLabelName = ((Module.HLSResult)numVal).ToString();
                    break;
                default:
                    throw new ArgumentException($"Acquisition Result type Label for <{resTyp.GetActorType()}> is not handled");

            }
            return AcqLabelName;

        }

        public static string DefaultLabelName(this ResultType resTyp, byte idx)
        {
            return idx == 0 ? ResultFormatExtension.GetLabelName(resTyp) : $"{ResultFormatExtension.GetLabelName(resTyp)} {idx}";
        }

        public static bool CheckResultTypeFileConsistency(ResultType restyp, string resFilePath)
        {
            string resExtension = Path.GetExtension(resFilePath);
            int fmtextid = GetExtIdFromExtension(resExtension.Trim().ToLower());
            return restyp.GetResultExtensionId() == fmtextid;
        }

    }
}
