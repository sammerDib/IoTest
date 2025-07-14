using System;
using System.Threading;

using ADCEngine;
using ADCEngine.Parameters;

using CrownMeasurementModule.CrownProfile;
using CrownMeasurementModule.Objects;

using LibProcessing;

using static CrownMeasurementModule.Objects.ProfileMeasure;
using static CrownMeasurementModule.Objects.ProfileMeasure.CrownProfileModule_ProfileStat;

namespace CrownMeasurementModule.CrownAnalyse
{
    public class CrownAnalyseModule : ModuleBase
    {
        private static ProcessingClass _processClass = new ProcessingClassMil();

        //=================================================================
        // Paramètres EBR
        //=================================================================
        public readonly SeparatorParameter paramSeparatorEBR;
        public readonly BoolParameter paramEBR;
        [ExportableParameter(false)]
        public readonly EnumParameter<enTransitionNumber> paramFirstEdgeExternal;
        [ExportableParameter(false)]
        public readonly EnumParameter<enTransitionNumber> paramFirstEdgeInternal;
        [ExportableParameter(false)]
        public readonly EnumParameter<enTransitionNumber> paramSecondEdgeExternal;
        [ExportableParameter(false)]
        public readonly EnumParameter<enTransitionNumber> paramSecondEdgeInternal;

        public readonly SeparatorParameter paramSeparatorTaikoRing;
        public readonly BoolParameter paramTaikoRing;
        [ExportableParameter(false)]
        public readonly EnumParameter<enTransitionNumber> paramRingExternal;
        [ExportableParameter(false)]
        public readonly EnumParameter<enTransitionNumber> paramRingInternal;

        public readonly SeparatorParameter paramSeparatorTaikoShoulder;
        public readonly BoolParameter paramTaikoShoulder;
        [ExportableParameter(false)]
        public readonly EnumParameter<enTransitionNumber> paramShoulderExternal;
        [ExportableParameter(false)]
        public readonly EnumParameter<enTransitionNumber> paramShoulderInternal;

        public readonly SeparatorParameter paramSeparatorRingScan;
        public readonly BoolParameter paramRingScan;
        [ExportableParameter(false)]
        public readonly EnumParameter<enTransitionNumber> paramFirstFrontDetect;
        [ExportableParameter(false)]
        public readonly EnumParameter<enTransitionNumber> paramLastFrontDetect;

        public readonly SeparatorParameter paramSeparatorGlassCarrier;
        public readonly BoolParameter paramGlassCarrier;



        //=================================================================
        // Constructeur
        //=================================================================
        public CrownAnalyseModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramSeparatorEBR = new SeparatorParameter(this, "EBR ");
            paramEBR = new BoolParameter(this, "EBR");
            paramEBR.ValueChanged +=
                (used) =>
                {
                    paramFirstEdgeExternal.IsEnabled = used;
                    paramFirstEdgeInternal.IsEnabled = used;
                    paramSecondEdgeExternal.IsEnabled = used;
                    paramSecondEdgeInternal.IsEnabled = used;
                };
            paramFirstEdgeExternal = new EnumParameter<enTransitionNumber>(this, "FirstEdgeExternal");
            paramFirstEdgeInternal = new EnumParameter<enTransitionNumber>(this, "FirstEdgeInternal");
            paramSecondEdgeExternal = new EnumParameter<enTransitionNumber>(this, "SecondEdgeExternal");
            paramSecondEdgeInternal = new EnumParameter<enTransitionNumber>(this, "SecondEdgeInternal");
            //--------------
            paramSeparatorTaikoRing = new SeparatorParameter(this, "Taiko Ring");
            paramTaikoRing = new BoolParameter(this, "TaikoRing");
            paramTaikoRing.ValueChanged +=
                (used) =>
                {
                    paramRingExternal.IsEnabled = used;
                    paramRingInternal.IsEnabled = used;
                };
            paramRingExternal = new EnumParameter<enTransitionNumber>(this, "TaikoRingExternal");
            paramRingInternal = new EnumParameter<enTransitionNumber>(this, "TaikoRingInternal");
            //--------------
            paramSeparatorTaikoShoulder = new SeparatorParameter(this, "Taiko Shoulder");
            paramTaikoShoulder = new BoolParameter(this, "TaikoShoulder");
            paramTaikoShoulder.ValueChanged +=
                (used) =>
                {
                    paramShoulderExternal.IsEnabled = used;
                    paramShoulderInternal.IsEnabled = used;
                };
            paramShoulderExternal = new EnumParameter<enTransitionNumber>(this, "TaikoShoulderExternal");
            paramShoulderInternal = new EnumParameter<enTransitionNumber>(this, "TaikoShoulderInternal");
            //---------------
            paramSeparatorRingScan = new SeparatorParameter(this, "Ring Scan");
            paramRingScan = new BoolParameter(this, "RingScan");
            paramRingScan.ValueChanged +=
                (used) =>
                {
                    paramFirstFrontDetect.IsEnabled = used;
                    paramLastFrontDetect.IsEnabled = used;
                };
            paramFirstFrontDetect = new EnumParameter<enTransitionNumber>(this, "FirstFrontDetect");
            paramLastFrontDetect = new EnumParameter<enTransitionNumber>(this, "LastFrontDetect");
            //---------------
            paramSeparatorGlassCarrier = new SeparatorParameter(this, "Carrier");
            paramGlassCarrier = new BoolParameter(this, "GlassCarrier");
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            ProfileMeasure profileMeasure = (ProfileMeasure)obj;

            CrownProfileModule_SettingEdge esEdgeSetting = new CrownProfileModule_SettingEdge();

            InitEdgeSetting(ref esEdgeSetting);

            profileMeasure.EBR = paramEBR.Value;
            profileMeasure.ringScan = paramRingScan.Value;
            profileMeasure.taikoRing = paramTaikoRing.Value;
            profileMeasure.glassDiameters = paramGlassCarrier.Value;
            profileMeasure.taikoShoulder = paramTaikoShoulder.Value;

            foreach (CrownProfileModule_ProfileStat _profile in profileMeasure._profile)
                _profile.SetMeasurementInfo(enAskMeasuremnt.en_EdgeScan, esEdgeSetting, ref profileMeasure.listCenteringCtrlValue);

            foreach (CrownProfileModule_ProfileStat _profile in profileMeasure._profile)
                _profile.SetMeasurementInfo(enAskMeasuremnt.en_EBR, esEdgeSetting, ref profileMeasure.listCenteringCtrlValue);

            foreach (CrownProfileModule_ProfileStat _profile in profileMeasure._profile)
                _profile.SetMeasurementInfo(enAskMeasuremnt.en_TaikoRing, esEdgeSetting, ref profileMeasure.listCenteringCtrlValue);

            foreach (CrownProfileModule_ProfileStat _profile in profileMeasure._profile)
                _profile.SetMeasurementInfo(enAskMeasuremnt.en_GlassDiameter, esEdgeSetting, ref profileMeasure.listCenteringCtrlValue);

            profileMeasure.EdgeSmoothing();

            ProcessChildren(profileMeasure);

        }

        private void InitEdgeSetting(ref CrownProfileModule_SettingEdge esEdgeSetting)
        {
            esEdgeSetting.m_lsEBRSelectedTransition.Clear();
            esEdgeSetting.m_lsEBRSelectedTransition.Add(Convert.ToInt32(paramFirstEdgeExternal.Value));
            esEdgeSetting.m_lsEBRSelectedTransition.Add(Convert.ToInt32(paramFirstEdgeInternal.Value));
            esEdgeSetting.m_lsEBRSelectedTransition.Add(Convert.ToInt32(paramSecondEdgeExternal.Value));
            esEdgeSetting.m_lsEBRSelectedTransition.Add(Convert.ToInt32(paramSecondEdgeInternal.Value));

            esEdgeSetting.m_lsTaikoRingSelectedTransition.Clear();
            esEdgeSetting.m_lsTaikoRingSelectedTransition.Add(Convert.ToInt32(paramRingExternal.Value));
            esEdgeSetting.m_lsTaikoRingSelectedTransition.Add(Convert.ToInt32(paramRingInternal.Value));

            esEdgeSetting.m_lsTaikoShoulderSelectedTransition.Clear();
            esEdgeSetting.m_lsTaikoShoulderSelectedTransition.Add(Convert.ToInt32(paramShoulderExternal.Value));
            esEdgeSetting.m_lsTaikoShoulderSelectedTransition.Add(Convert.ToInt32(paramShoulderInternal.Value));

            esEdgeSetting.m_lsEdgeScanSelectedTransition.Clear();
            esEdgeSetting.m_lsEdgeScanSelectedTransition.Add(Convert.ToInt32(paramFirstFrontDetect.Value));
            esEdgeSetting.m_lsEdgeScanSelectedTransition.Add(Convert.ToInt32(paramLastFrontDetect.Value));
        }
    }
}
