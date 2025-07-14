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
    public class ClassificationWithDebugModule : ImageModuleBase, IClassifierModule
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
        public ClassificationWithDebugModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            // ligne commentée par Olivier , ca empechait la compilation -> il manquait une definition pour ma nouvelle classe dans ClassifaicationParameter 
            // je ne sais pas ce qu'elle fait
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
            // on a 1 cluster en input
            List<string> list = new List<string>();


            // on itère sur la liste des classes de defaut
            foreach (DefectClass defectClass in paramClassification.DefectClassList)
            {
                // bMatch  = ?
                bool bMatch = true;
                // on itere sur les caractéristiques de capture utilisées dans le definition du défaut
                foreach (ComparatorBase cmp in defectClass.compartorList)
                {
                    // cmp.characteristic c'est le nom de la carac
                    // value c'est la valeur que je veux recup
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

            // l'étape de classification est finie
            // maintenant je peux faire mon biz pour recup les infos que je veux

            // Si je comprend bien cluster c'est une image surchargée avec des propiété comme pixelRect qui decrivent quelle zone dans l'image initiale est extraite
            // donc je devrais pouvoir obtenir les coordoonée des bords de ce rectangle avec les attributs cluster.pixelRect.Left etc..
            // Je vais utiliser des balises au début et a la fin pour me simplifier la vie pour le parsing du fichier log plus tard 
            log("<clusterinfo>" + cluster.ToString() + " is at " + "{'X'=[" + cluster.pixelRect.Left + "," + cluster.pixelRect.Right + "]," + "'Y'=[" + cluster.pixelRect.Top + "," + cluster.pixelRect.Bottom + "]}" + " </clusterinfo>");

            // il faut que j'itere sur toutes les characteristique qui existent pour ce cluster
            // pour lister les key d'un dictionnaire : foreach (string key in AuthorList.Keys)
            foreach (Characteristic characteristic in cluster.characteristics.Keys)
            {
                object value = cluster.characteristics[characteristic];
                log("<clusterinfo>" + cluster.ToString() + " has " + characteristic + "=" + value + " </clusterinfo>");

            }


            // On insère les nouvelles classes avant les classes déjà existantes.
            cluster.defectClassList.InsertRange(0, list);

            return !cluster.defectClassList.IsEmpty();
        }

    }
}
