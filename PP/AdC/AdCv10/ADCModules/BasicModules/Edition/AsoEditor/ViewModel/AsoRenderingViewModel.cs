using System.Collections.Generic;

using AdcBasicObjects;

using ADCEngine;

using BasicModules.Edition.Rendering;
using BasicModules.Edition.Rendering.Message;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.Tools;

namespace BasicModules.AsoEditor
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class AsoRenderingViewModel : ClassificationViewModel
    {
        private int _nbDefects;

        public AsoRenderingViewModel(ModuleBase module) :
            base(module)
        {

            List<ClassDefectResult> resultDefects = new List<ClassDefectResult>();

            foreach (var defectClass in ((AsoEditorModule)Module).paramDefectClasses.DefectClassToCategoryMap.Values)
            {
                resultDefects.Add(new ClassDefectResult(defectClass.DefectLabel, null));
            }

            foreach (var defectClass in ((AsoEditorModule)Module).paramDefectClasses.VidToCategoryMap.Values)
            {
                resultDefects.Add(new ClassDefectResult(defectClass.DefectCategory, null));
            }

            Init(resultDefects);

            ClassLocator.Default.GetInstance<IMessenger>().Register<AsoResultMessage>(this, (r, asoResult) =>
            {
                ModuleBase newModule = asoResult.Module;

                // Instance de recette différente entre l'affichage et l'exécution
                if ((Module != newModule) && (Module.Id == newModule.Id) && Module is AsoEditorModule)
                {
                    Module = newModule;
                }

                UpdateAsoView();
            });
        }

        public override void Clean()
        {
            base.Clean();
            Cleanup();
            _nbDefects = 0;
            ClassLocator.Default.GetInstance<IMessenger>().Unregister<AsoResultMessage>(this);
        }

        public void UpdateAsoView()
        {
            if (((AsoEditorModule)Module).ClusterList != null)
                UpdateDefectView();
        }

        private void UpdateDefectView()
        {
            List<Cluster> clusters = ((AsoEditorModule)Module).ClusterList;

            if (clusters == null)
                return;

            lock (((AsoEditorModule)Module).ClusterList)
            {
                Cluster cluster;
                for (int id = _nbDefects; id < clusters.Count; id++)
                {
                    cluster = clusters[id];
                    _nbDefects++;

                    DefectResult resultDefect = new DefectResult();
                    resultDefect.ClassName = ((AsoEditorModule)Module).GetClusterCategory(cluster);
                    resultDefect.Id = cluster.Index;
                    resultDefect.MicronRect = cluster.micronQuad.SurroundingRectangle;
                    AddDefect(resultDefect);
                }
            }
        }

        public void Cleanup()
        {
            Defects.Clear();
            Classes.Clear();
        }
    }
}
