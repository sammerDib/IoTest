using AdcBasicObjects;

using ADCEngine;

namespace BasicModules.BorderRemoval
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class EdgeModule_Col_Exclusion : ImageModuleBase
    {
        //[System.Reflection.Obfuscation(Exclude = true)]
        public enum eColumnStart { Column_0, Column_1, Column_2, Column_3 }



        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly EnumParameter<eColumnStart> paramColumnStart;
        public readonly IntParameter ParamNbColum;

        //=================================================================
        // Constructeur
        //=================================================================
        public EdgeModule_Col_Exclusion(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramColumnStart = new EnumParameter<eColumnStart>(this, "First column to inspect");
            ParamNbColum = new IntParameter(this, "Nombre de column to inspect", 1, 4);
            ParamNbColum.Value = 4;
        }



        //=================================================================
        // Process
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            //-------------------------------------------------------------
            // Est-ce une image pour ce DataLoader ?
            //-------------------------------------------------------------
            MosaicImage image = (MosaicImage)obj;

            if (image == null)
                return;

            if (image.Column > -1)
            {
                // on ne filtre que les layers du top et bottom
                if ((image.Layer.name == "TopSensor") || (image.Layer.name == "BottomSensor"))
                {
                    //if (((int)paramColumnStart.Value <= image.Column) && (image.Column <= (int)paramColumnStart.Value + ParamNbColum - 1))
                    //{
                    //    // On vire cette image
                    //    return ; 
                    //}

                }
            }
            ProcessChildren(obj);
        }

    }


}
