using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.Xml;
using ADCEngine;
using AdcTools;
using System.Windows.Controls;
using LibProcessing;
using AdcBasicObjects;
using BasicModules;

namespace AdvancedModules.Convert16To8
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class Convert16To8Module : ImageModuleBase
    {
        private static ProcessingClass _processClass = new ProcessingClassMil();

		//=================================================================
		// Paramètres du XML
		//=================================================================


		//=================================================================
		// Constructeur
		//=================================================================
		public Convert16To8Module(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
        }

		//=================================================================
		// 
		//=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            IImage image = (IImage)obj;

            image.CurrentProcessingImage.SetMilImage((_processClass.ConvertTo8bit(image.CurrentProcessingImage)).GetMilImage()); 

            ProcessChildren(obj);
		}

	}
}
