using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.Xml;
using Matrox.MatroxImagingLibrary;
using AdcModuleBase;
using AdcTools;
using libMIL;
using System.Windows.Controls;


namespace BasicModules
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    class ThresholdModule : ModuleBase
    {
        private ThresholdParameters _parameters = new ThresholdParameters();

        //=================================================================
        // 
        //=================================================================
        public ThresholdModule(IModuleFactory factory, int id, IRecipe recipe)
            : base(factory, id, recipe)
        {
        }

        //=================================================================
        // 
        //=================================================================
        public override void Init(XmlNodeList parametersNodes)
        {
            log("Démarage");
            _parameters.Init(parametersNodes);
        }

        //=================================================================
        //
        //=================================================================
        protected override void SaveParameters(XmlNode parametersNode)
        {
            _parameters.SaveParameters(parametersNode);
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            log("Theshold " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            IImage image = (IImage)obj;
            MilImage milImage = image.GetMilImage();
            milImage.Binarize(milImage, MIL.M_IN_RANGE, _parameters.LowCond, _parameters.HighCond);

            ProcessChildren(obj);
        }


        //=================================================================
        // 
        //=================================================================
        public override UserControl GetUIParameters()
        {
            UserControl control = new ThresholdControl();
            control.DataContext = _parameters;
            return control;
        }

    }
}
