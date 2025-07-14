using System.Collections.Generic;
using System.Linq;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

namespace BasicModules.Classification
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class ClassificationModule : ImageModuleBase, IClassifierModule
    {
        public List<DefectClass> DefectClassList { get { return paramClassification.DefectClassList; } }
        public List<string> DefectClassLabelList
        {
            get
            {
                List<string> list = new List<string>(
                    from cl in paramClassification.DefectClassList
                    select cl.label
                    );
                return list;
            }
        }

        //=================================================================
        // Paramètres du XML
        //=================================================================
        [ExportableParameter(false)]
        public readonly ClassificationParameter paramClassification;


        //=================================================================
        // Constructeur
        //=================================================================
        public ClassificationModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramClassification = new ClassificationParameter(this, "Classification");
        }


        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("Process " + obj);
            Cluster cluster = (Cluster)obj;
            Interlocked.Increment(ref nbObjectsIn);

            bool classified = Classify(cluster);
            if (!classified)
            {
                logDebug(cluster.ToString() + " not classified");
                return;
            }

            ProcessChildren(obj);
        }

        //=================================================================
        // 
        //=================================================================
        protected bool Classify(Cluster cluster)
        {
            List<string> list = new List<string>();

            foreach (DefectClass defectClass in paramClassification.DefectClassList)
            {
                bool bMatch = true;
                foreach (ComparatorBase cmp in defectClass.compartorList)
                {
                    object value = cluster.characteristics[cmp.characteristic];
                    bMatch = cmp.Test(value);
                    if (!bMatch)
                    {
                        logDebug(cluster.ToString() + " is not " + defectClass.label + " because " + cmp.characteristic + "=" + value + " doesn't match " + cmp);
                        break;
                    }
                }

                if (bMatch)
                {
                    list.Add(defectClass.label);
                    logDebug(cluster.ToString() + " classified as " + defectClass.label);
                }
            }

            // On insère les nouvelles classes avant les classes déjà existantes.
            cluster.defectClassList.InsertRange(0, list);

            return !cluster.defectClassList.IsEmpty();
        }

    }
}
