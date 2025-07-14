using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ADCEngine;
using AdcTools;
using AdcBasicObjects;
using System.Threading;

namespace GlobaltopoModule
{

    public class GTVidBowWarpReportModule : ModuleBase
    {
        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly GTVidBowWarpParameter paramMeasureReports;

        //=================================================================
        // Constructeur
        //=================================================================
        public GTVidBowWarpReportModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {

            paramMeasureReports = new GTVidBowWarpParameter(this, "VidBowWarpPrm");
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();
        }
        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("Process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            GTMeasure mes = (GTMeasure)obj;

            //-------------------------------------------------------------
            // Stockage des Mesures
            //-------------------------------------------------------------
           // mes.AddRef();
           // lock (_ListMeasure)
           //     _ListMeasure.Add(mes);
        } 
    }
}
