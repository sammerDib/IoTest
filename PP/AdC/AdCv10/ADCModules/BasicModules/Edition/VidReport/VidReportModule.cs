using ADCEngine;

using UnitySC.Shared.Tools;

namespace BasicModules.VidReport
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class VidReportModule : ModuleBase
    {
        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly VidReportParameter paramCategories;

        //=================================================================
        // Constructeur
        //=================================================================
        public VidReportModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramCategories = new VidReportParameter(this, "Categories");
        }

        ////=================================================================
        //// 
        ////=================================================================
        //public HashSet<int> GetVids()
        //{
        //    HashSet<int> list = new HashSet<int>();

        //    foreach (ReportClass cat in paramCategories.ReportClasses.Values)
        //        list.Add(cat.VID);

        //    return list;
        //}

        public override void Process(ModuleBase parent, ObjectBase obj)
        {

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
        protected override void OnStopping(eModuleState oldState)
        {
            base.OnStopping(oldState);
        }
    }
}
