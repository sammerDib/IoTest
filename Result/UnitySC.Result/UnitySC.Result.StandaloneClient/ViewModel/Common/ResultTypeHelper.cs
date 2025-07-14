using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.IO;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.Result.StandaloneClient.ViewModel.Common
{
    public static class ResultTypeHelper
    {
        private static Dictionary<ResultType, string> s_resultTypeNameDictionary = new Dictionary<ResultType, string>();
        private static Dictionary<ResultType, SolidColorBrush> s_resultTypeColors = new Dictionary<ResultType, SolidColorBrush>()
        {
            {ResultType.NotDefined, new SolidColorBrush(Color.FromRgb(33, 33, 33))},
            {ResultType.Empty, new SolidColorBrush(Color.FromRgb(33, 33, 33))},

            {ResultType.ANALYSE_Step, new SolidColorBrush(Color.FromRgb(255, 143, 0))},
            {ResultType.ANALYSE_Bow, new SolidColorBrush(Color.FromRgb(244, 81, 30))},
            {ResultType.ANALYSE_NanoTopo, new SolidColorBrush(Color.FromRgb(63, 81, 181))},
            {ResultType.ANALYSE_PeriodicStructure, new SolidColorBrush(Color.FromRgb(92, 107, 192))},
            {ResultType.ANALYSE_Pillar, new SolidColorBrush(Color.FromRgb(41, 121, 255))},
            {ResultType.ANALYSE_Roughness, new SolidColorBrush(Color.FromRgb(141, 110, 99))},
            {ResultType.ANALYSE_TSV, new SolidColorBrush(Color.FromRgb(171, 71, 188))},
            {ResultType.ANALYSE_Thickness, new SolidColorBrush(Color.FromRgb(84, 110, 122))},
            {ResultType.ANALYSE_Topography, new SolidColorBrush(Color.FromRgb(56, 142, 60))},
            {ResultType.ANALYSE_Trench, new SolidColorBrush(Color.FromRgb(0, 150, 136))},
            {ResultType.ANALYSE_CD, new SolidColorBrush(Color.FromRgb(189, 0, 96))},
            {ResultType.ANALYSE_EBR, new SolidColorBrush(Color.FromRgb(121, 85, 72))},
            {ResultType.ANALYSE_Warp, new SolidColorBrush(Color.FromRgb(84, 110, 122))},
            {ResultType.ANALYSE_EdgeTrim, new SolidColorBrush(Color.FromRgb(116, 131, 175))},
            {ResultType.ANALYSE_XYCalibration, new SolidColorBrush(Color.FromRgb(26, 118, 192))},
            {ResultType.ANALYSE_Overlay, new SolidColorBrush(Color.FromRgb(126, 50, 196))},

            {ResultType.ADC_Haze, new SolidColorBrush(Color.FromRgb(230, 74, 25))},
            {ResultType.ADC_Klarf, new SolidColorBrush(Color.FromRgb(233, 30, 99))},
            {ResultType.ADC_ASO, new SolidColorBrush(Color.FromRgb(76, 175, 80))},
            {ResultType.ADC_ASE, new SolidColorBrush(Color.FromRgb(0, 188, 202))},
            {ResultType.ADC_Crown, new SolidColorBrush(Color.FromRgb(255, 160, 0))},
            {ResultType.ADC_EyeEdge, new SolidColorBrush(Color.FromRgb(96, 125, 139))},
            {ResultType.ADC_DFHaze, new SolidColorBrush(Color.FromRgb(230, 74, 25))},
            {ResultType.ADC_GlobalTopo, new SolidColorBrush(Color.FromRgb(200, 74, 105))},
            {ResultType.ADC_HeightMes, new SolidColorBrush(Color.FromRgb(100, 150, 20))},
            {ResultType.ADC_YieldMap, new SolidColorBrush(Color.FromRgb(160, 20, 200))},
        };

        static ResultTypeHelper()
        {

            string curpath = Directory.GetCurrentDirectory();
            var xmlpath = Path.Combine(curpath, "Settings","ResTypeDisplaySettings.xml");
            if (File.Exists(xmlpath))
            {
                try
                {
                    var dico = new ResultTypeDisplaySettings(xmlpath);
                    foreach (var set in dico.Settings)
                    {
                        if (!s_resultTypeNameDictionary.ContainsKey(set.ResType))
                            s_resultTypeNameDictionary.Add(set.ResType, set.Lbl);
                        else
                            s_resultTypeNameDictionary[set.ResType] = set.Lbl;

                        if (!s_resultTypeColors.ContainsKey(set.ResType))
                            s_resultTypeColors.Add(set.ResType, new SolidColorBrush(Color.FromRgb((byte)set.R, (byte)set.G, (byte)set.B)));
                        else
                            s_resultTypeColors[set.ResType] = new SolidColorBrush(Color.FromRgb((byte)set.R, (byte)set.G, (byte)set.B));

                    }
                }
                catch
                { }
            }

            foreach (var rtyp in (ResultType[])Enum.GetValues(typeof(ResultType)))
            {
                if(rtyp.GetResultCategory() != ResultCategory.Result)
                    continue;

                if (!s_resultTypeNameDictionary.ContainsKey(rtyp))
                {
                    string ext;
                    try
                    {
                        ext = rtyp.GetExt();
                    }
                    catch
                    {
                        ext = rtyp.ToString();
                    }
                    s_resultTypeNameDictionary.Add(rtyp, ext);
                }

                if (!s_resultTypeColors.ContainsKey(rtyp))
                {
                    s_resultTypeColors.Add(rtyp, new SolidColorBrush(Color.FromRgb(25, 25, 25)));
                }
            }
        }

        public static string GetDisplayName(this ResultType type) => s_resultTypeNameDictionary.TryGetValue(type, out string name) ? name : s_resultTypeNameDictionary[ResultType.NotDefined];

        public static SolidColorBrush GetColor(this ResultType type) => s_resultTypeColors.TryGetValue(type, out var color) ? color : s_resultTypeColors[ResultType.NotDefined];
    }
}
