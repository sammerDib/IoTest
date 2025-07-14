using System;
using System.Collections.Generic;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.ResultUI.ASO.ViewModel;
using UnitySC.Shared.ResultUI.Common.ViewModel;
using UnitySC.Shared.ResultUI.Common.ViewModel.Dummy;
using UnitySC.Shared.ResultUI.Common.ViewModel.Acquisition;
using UnitySC.Shared.ResultUI.HAZE.ViewModel.Stats;
using UnitySC.Shared.ResultUI.HAZE.ViewModel.WaferDetails;
using UnitySC.Shared.ResultUI.Klarf.ViewModel;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Stats.Pillar;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Stats.Trench;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Stats.Bow;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Stats.Topography;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Stats.Nanotopo;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Stats.PeriodicStruct;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Stats.Step;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Stats.Thickness;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Stats.Tsv;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Pillar;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Trench;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Bow;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Topography;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Nanotopo;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.PeriodicStruct;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Step;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Thickness;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Tsv;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Stats.Warp;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Warp;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.EdgeTrim;
using UnitySC.Shared.ResultUI.Metro.ViewModel.Stats.EdgeTrim;

namespace UnitySC.Result.CommonUI
{
    public sealed class ViewerVMBuilder
    {
        private static ViewerVMBuilder s_instance;
        private static readonly object s_syncRoot = new object();
        private readonly Dictionary<ResultFormat, ResultWaferVM> _dicAcquistionFormatVM = new Dictionary<ResultFormat, ResultWaferVM>();

        public static ViewerVMBuilder Singleton
        {
            get
            {
                lock (s_syncRoot)
                {
                    return s_instance ?? (s_instance = new ViewerVMBuilder());
                }
            }
        }

        private ViewerVMBuilder()
        {
            // Build all colorMaps
            var unused = Shared.Data.ColorMap.ColorMapHelper.ColorMaps;

            //Set Deployment Key for Arction components -- à offusquer
            string deploymentKey = "lgCAAMDQWCaC7NkBJABVcGRhdGVhYmxlVGlsbD0yMDI1LTEwLTE1I1JldmlzaW9uPTAI/z8AAAAA/AEy9k+WSPYWl8F5j8DR3/xHV8rmwsD6bgDd0+Kv9Aki7UvzB0KJxrUPqzZmcZ1hEhm6bFr+fezEYuukPWEkI7pybF86LTroOuA934Gci/KuDUrhHiaqtxFeaR30Gcgr25NjTyEpauRATjQ4BFk32TnkLwotmJoCv+HYAJkkvd85VCzS0o5fd4w99JHK3/XtyJSYL8/OCCrqumTQZm5A8s7q95M8AfxmeLTEUjPFJp/k+m0oTPHF4er+PTE/m1R/r1+yL6ZeiCzkuFB5m4vLE1vxa7ZEp0aRQ01Xw+0LPPBusgBj4089eXfVWH3DsnFfDmPrFn63MByaFqpzT/hK4J0EiXGqHRaGz8CCiRVxAO3mAT7DirAypxLrrF+142Z3f3iQnd88mRsFiTN2rqfbZDFmPPaK2j4LwDwqKiaVCOz6ISQpG8W7UOMSZjX1KnMiS+FdQRYJJPuuE0WGRMutSyrNHGawAsMY6J4hOh4hDsJsRgN3onrFG+pCHwFG/fUD154=";
            // Setting Deployment Key for fully bindable chart
            LightningChartLib.WPF.ChartingMVVM.LightningChart.SetDeploymentKey(deploymentKey);
            LightningChartLib.WPF.Charting.LightningChart.SetDeploymentKey(deploymentKey);

            _dicAcquistionFormatVM.Add(ResultFormat.FullImage, new FullImageVM(null));
            _dicAcquistionFormatVM.Add(ResultFormat.FullImage_3D, new FullImageVM(null));
            _dicAcquistionFormatVM.Add(ResultFormat.MosaicImage, new FullImageVM(null));
            _dicAcquistionFormatVM.Add(ResultFormat.MosaicImage_3D, new FullImageVM(null));
        }

        public ResultWaferVM BuildResultVM(ResultType restype, Shared.Format.Base.IResultDisplay resultDisplay)
        {
            ResultWaferVM vm;

            int extId = restype.GetResultExtensionId();
            if (extId == ResultType.ADC_Klarf.GetResultExtensionId())
            {
                vm = new KlarfResultVM(resultDisplay);
            }
            else if (extId == ResultType.ADC_ASO.GetResultExtensionId())
            {
                vm = new AsoResultVM(resultDisplay);
            }
            else if (extId == ResultType.ADC_Haze.GetResultExtensionId())
            {
                vm = new HazeResultVM(resultDisplay);
            }
            else if (extId == ResultType.ANALYSE_TSV.GetResultExtensionId())
            {
                vm = new TsvResultVM(resultDisplay);
            }
            else if (extId == ResultType.ANALYSE_NanoTopo.GetResultExtensionId())
            {
                vm = new NanotopoResultVM(resultDisplay);
            }
            else if (extId == ResultType.ANALYSE_Thickness.GetResultExtensionId())
            {
                vm = new ThicknessResultVM(resultDisplay);
            }
            else if (extId == ResultType.ANALYSE_Topography.GetResultExtensionId())
            {
                vm = new TopographyResultVM(resultDisplay);
            }
            else if (extId == ResultType.ANALYSE_Step.GetResultExtensionId())
            {
                vm = new StepResultVM(resultDisplay);
            }
            else if (extId == ResultType.ANALYSE_Trench.GetResultExtensionId())
            {
                vm = new TrenchResultVM(resultDisplay);
            }
            else if (extId == ResultType.ANALYSE_Bow.GetResultExtensionId())
            {
                vm = new BowResultVM(resultDisplay);
            }
            else if (extId == ResultType.ANALYSE_Pillar.GetResultExtensionId())
            {
                vm = new PillarResultVM(resultDisplay);
            }
            else if (extId == ResultType.ANALYSE_PeriodicStructure.GetResultExtensionId())
            {
                vm = new PeriodicStructResultVM(resultDisplay);
            }
            else if (extId == ResultType.ANALYSE_Warp.GetResultExtensionId())
            {
                vm = new WarpResultVM(resultDisplay);
            }
            else if (extId == ResultType.ANALYSE_EdgeTrim.GetResultExtensionId())
            {
                vm = new EdgeTrimResultVM(resultDisplay);
            }
            else
            {
                vm = new DummyResultVM { DummyLabel = $"Dummy Result wafer from : {restype}" };
            }

            return vm;
        }

    public LotStatsVM BuildStatVM(ResultType rtyp)
    {
        LotStatsVM vm;

        switch (rtyp.GetResultFormat())
        {
            case ResultFormat.Klarf:
                vm = new KlarfStatsVM();
                break;

            case ResultFormat.ASO:
                vm = new AsoStatsVM();
                break;

            case ResultFormat.Haze:
                vm = new HazeStatsVM();
                break;

            case ResultFormat.Metrology:
                switch (rtyp)
                {
                    case ResultType.ANALYSE_TSV:
                        vm = new TsvStatsVM();
                        break;
                    case ResultType.ANALYSE_NanoTopo:
                        vm = new NanotopoStatsVM();
                        break;
                    case ResultType.ANALYSE_Thickness:
                        vm = new ThicknessStatsVM();
                        break;
                    case ResultType.ANALYSE_Topography:
                        vm = new TopographyStatsVM();
                        break;
                    case ResultType.ANALYSE_Step:
                        vm = new StepStatsVM();
                        break;
                    case ResultType.ANALYSE_Trench:
                        vm = new TrenchStatsVM();
                        break;
                    case ResultType.ANALYSE_Bow:
                        vm = new BowStatsVM();
                        break;
                    case ResultType.ANALYSE_Warp:
                        vm = new WarpStatsVM();
                        break;
                    case ResultType.ANALYSE_Pillar:
                        vm = new PillarStatsVM();
                        break;
                    case ResultType.ANALYSE_PeriodicStructure:
                        vm = new PeriodicStructStatsVM();
                        break;
                    case ResultType.ANALYSE_EdgeTrim:
                        vm = new EdgeTrimStatsVM();
                        break;
                    default:
                        vm = new DummyStatsVM { DummyStatsLabel = $"Dummy lot stats from : {rtyp}", };
                        break;
                }
                break;

            default:
                vm = new DummyStatsVM { DummyStatsLabel = $"Dummy lot stats from : {rtyp}" };
                break;
        }
        return vm;
    }

    public Dictionary<ResultType, ResultWaferVM> BuildDicoResultVM(Shared.Format.Base.IResultDataFactory resultFactory)
    {
        var dico = new Dictionary<ResultType, ResultWaferVM>();
        foreach (var rtyp in (ResultType[])Enum.GetValues(typeof(ResultType)))
        {
            Shared.Format.Base.IResultDisplay resdisplay = null;
            switch (rtyp.GetResultCategory())
            {
                case ResultCategory.Result:
                    try
                    {
                        resdisplay = resultFactory.GetDisplayFormat(rtyp);
                    }
                    catch (NotImplementedException exnotimp)
                    {
                        string smsg = exnotimp.Message;
                    }
                    dico.Add(rtyp, BuildResultVM(rtyp, resdisplay));
                    break;
                case ResultCategory.Acquisition:
                    switch (rtyp.GetResultFormat())
                    {
                        case ResultFormat.FullImage:
                            dico.Add(rtyp, _dicAcquistionFormatVM[ResultFormat.FullImage]);
                            break;
                        case ResultFormat.FullImage_3D:
                            dico.Add(rtyp, _dicAcquistionFormatVM[ResultFormat.FullImage_3D]);
                            break;
                        case ResultFormat.MosaicImage:
                            dico.Add(rtyp, _dicAcquistionFormatVM[ResultFormat.MosaicImage]);
                            break;
                        case ResultFormat.MosaicImage_3D:
                            dico.Add(rtyp, _dicAcquistionFormatVM[ResultFormat.MosaicImage_3D]);
                            break;
                    }
                    break;
            }
        }
        return dico;
    }

    public Dictionary<ResultType, LotStatsVM> BuildDicoStatsVM()
    {
        var dico = new Dictionary<ResultType, LotStatsVM>();
        foreach (var rtyp in (ResultType[])Enum.GetValues(typeof(ResultType)))
        {
            if (rtyp.GetResultCategory() == ResultCategory.Result)
            {
                dico.Add(rtyp, BuildStatVM(rtyp));
            }
        }
        return dico;
    }
}
}
