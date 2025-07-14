using System;
using System.Collections.Generic;
using System.Drawing;

using ADCEngine;

using BasicModules.Edition.Rendering;
using BasicModules.Edition.Rendering.Message;

using CommunityToolkit.Mvvm.Messaging;

using Format001;

using UnitySC.Shared.Tools;

namespace BasicModules.KlarfEditor
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class KlarfRenderingViewModel : ClassificationViewModel
    {
        private int _nbDefects;

        public KlarfRenderingViewModel(ModuleBase module) :
            base(module)
        {
            List<ClassDefectResult> resultDefects = new List<ClassDefectResult>();

            foreach (var roughBin in ((KlarfEditorModule)Module).ParamRoughBins.RoughBins.Values)
            {
                resultDefects.Add(new ClassDefectResult(roughBin.DefectLabel, roughBin.RoughBinNum));
            }
            Init(resultDefects);

            ClassLocator.Default.GetInstance<IMessenger>().Register<KlarfResultMessage>(this, (r, klarfResult) =>
            {
                ModuleBase newModule = klarfResult.Module;

                // Instance de recette différente entre l'affichage et l'exécution
                if ((Module != newModule) && (Module.Id == newModule.Id) && Module is KlarfEditorModule)
                {
                    Module = newModule;
                }

                UpdateKlarfView();
            });
        }

        public override void Clean()
        {
            base.Clean();
            Cleanup();
            _nbDefects = 0;
            ClassLocator.Default.GetInstance<IMessenger>().Unregister<KlarfResultMessage>(this);
        }

        public void UpdateKlarfView()
        {
            if (((KlarfEditorModule)Module).MyDataKlarf != null)
                UpdateDefectView();
        }

        private void UpdateDefectView()
        {
            DataKlarf infoKlarf = ((KlarfEditorModule)Module).MyDataKlarf;

            if (infoKlarf == null)
                return;

            lock (((KlarfEditorModule)Module).Syncklarf)
            {
                PrmDefect defect;
                for (int id = _nbDefects; id < infoKlarf.DefectList.Count; id++)
                {
                    defect = infoKlarf.DefectList[id];
                    if (defect != null)
                    {

                        _nbDefects++;
                        DefectRoughBin defectRoughBin = GetDefectRoughBin(defect);
                        if (defectRoughBin != null)
                        {

                            DefectResult resultDefect = new DefectResult();
                            resultDefect.ClassName = defectRoughBin.DefectLabel;
                            resultDefect.RoughBinNum = defectRoughBin.RoughBinNum;
                            resultDefect.Id = (int)defect.Get("DEFECTID");
                            // Klarf defect size 2000 um 
                            var rectangleF = new RectangleF();
                            rectangleF.Location = defect.SurroundingRectangleMicron.Location;

                            if (defect.Get("XINDEX") != null) // Die ?
                            {
                                rectangleF.X = (float)((double)defect.Get("XREL") + (int)(defect.Get("XINDEX")) * (infoKlarf.DiePitch.x));
                                rectangleF.Y = (float)((double)defect.Get("YREL") + (int)defect.Get("YINDEX") * infoKlarf.DiePitch.y);
                            }
                            rectangleF.Width = 2000;
                            rectangleF.Height = 2000;
                            resultDefect.MicronRect = rectangleF;
                            //resultDefect.MicronRect = defect.SurroundingRectangleMicron;



                            //d.XWAFERCENTER = d.XABS - SampleCenterLocation.X;
                            //d.YWAFERCENTER = d.YABS - SampleCenterLocation.Y;
                            //d.polar_r = (float)Math.Sqrt(d.YWAFERCENTER * d.YWAFERCENTER + d.XWAFERCENTER * d.XWAFERCENTER);
                            //d.polar_theta = (float)Math.Atan2(d.YWAFERCENTER, d.XWAFERCENTER);



                            AddDefect(resultDefect);
                        }
                        else
                        {
                            Module.Recipe.log("UpdateDefectView : defectRoughBin  is null ");

                        }
                    }
                    else
                    {
                        Module.Recipe.log("UpdateDefectView : defect is null");
                    }
                }
            }
        }

        private DefectRoughBin GetDefectRoughBin(PrmDefect defect)
        {
            if (defect == null)
                Module.Recipe.log("GetDefectRoughBin : defect param is null");

            foreach (DefectRoughBin RoughBin in ((KlarfEditorModule)Module).ParamRoughBins.RoughBins.Values)
            {
                if (RoughBin.RoughBinNum == (int)defect.Get("ROUGHBINNUMBER"))
                {
                    return RoughBin;
                }
            }


            // Pas matché pourquoi
            throw new ApplicationException("KlarfRenderingViewModel : The roughbin number's defect " + (int)defect.Get("ROUGHBINNUMBER") + " doesn't match with RoughBins of Module klarf");

            //return null;
        }

        public void Cleanup()
        {
            Defects.Clear();
            Classes.Clear();
        }
    }
}
