
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Xml;

using ADCEngine;

using AdcTools;

namespace BasicModules.KlarfEditor
{
    public class KlarfEditorRoughBinParameter : ClassificationParameterBase
    {
        public Dictionary<string, DefectRoughBin> RoughBins { get; private set; }

        //=================================================================
        // Constructeur
        //=================================================================
        public KlarfEditorRoughBinParameter(ModuleBase module, string name) :
            base(module, name)
        {
            RoughBins = new Dictionary<string, DefectRoughBin>();

        }

        //=================================================================
        // XML
        //=================================================================
        public override void Load(XmlNodeList xmlNodes)
        {
            XmlNode node = ReadParameter("Roughbin", xmlNodes);
            foreach (XmlNode childnode in node.ChildNodes)
            {
                DefectRoughBin roughbin = Serializable.LoadFromXml<DefectRoughBin>(childnode);
                RoughBins.Add(roughbin.DefectLabel, roughbin);
            }
        }

        public override XmlElement Save(XmlNode xmlNode)
        {
            XmlElement node = SaveParameter(xmlNode, "Roughbin", this);

            foreach (DefectRoughBin roughbin in RoughBins.Values)
                roughbin.SerializeAsChildOf(node);

            return null;
        }

        //=================================================================
        // IHM
        //=================================================================
        private KlarfEditorRoughBinViewModel _parameterViewModel;
        private KlarfEditorRoughBinControl _parameterUI;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterViewModel = new KlarfEditorRoughBinViewModel(this);
                    _parameterUI = new KlarfEditorRoughBinControl(_parameterViewModel);
                }
                return _parameterUI;
            }
        }

        //=================================================================
        // 
        //=================================================================
        public override string Validate()
        {
            //-------------------------------------------------------------
            // Récupération des infos des autres modules
            //-------------------------------------------------------------
            HashSet<string> DefectLabelList = FindAvailableDefectLabelsWithDummyDefect();

            //-------------------------------------------------------------
            // Vérifie que toutes les classes de défauts sont bien gérées
            //-------------------------------------------------------------
            int count = 0;
            string error = null;

            foreach (string label in DefectLabelList)
            {
                bool missing = !RoughBins.ContainsKey(label);
                if (missing)
                {
                    if (count++ < 5)
                    {
                        if (count > 1)
                            error += Environment.NewLine;
                        error += "New defect class: " + label;
                    }
                    else
                        return "New defect classes, review rough bins";
                }
            }

            if (error != null)
                return "Review rough bins," + error;

            //-------------------------------------------------------------
            // Classes qui ont été supprimées en classif
            //-------------------------------------------------------------
            foreach (DefectRoughBin rbin in RoughBins.Values.ToList())
            {
                bool exist = DefectLabelList.Contains(rbin.DefectLabel);
                if (!exist)
                    return "Defect classes have changed, review rough bins";
            }

            //-------------------------------------------------------------
            // Check l'unicité des RoughtBin
            //-------------------------------------------------------------            
            bool rexist = false;
            List<DefectRoughBin> DefectRoughBin = RoughBins.Values.ToList();
            for (int j = 0; j < DefectRoughBin.Count; j++)
            {
                DefectRoughBin roughtbinj = DefectRoughBin[j];
                for (int i = j + 1; i < DefectRoughBin.Count; i++)
                {
                    DefectRoughBin roughtbini = DefectRoughBin[i];

                    if (roughtbinj.RoughBinNum == roughtbini.RoughBinNum)
                        rexist = true;
                }

                if (rexist)
                    continue;
            }

            if (rexist)
                return "RoughtBin must be unique, review rough bins";


            //-------------------------------------------------------------
            // Sinon tout va bien
            //-------------------------------------------------------------
            return null;
        }

        //=================================================================
        // 
        //=================================================================
        public void Synchronize()
        {
            bool changed = false;

            HashSet<string> DefectLabelList = FindAvailableDefectLabelsWithDummyDefect();


            #region PLSEditor Obsolete to refacto full feature
            // var plsAncestor = (PLSEditor.PLSEditorModule)Module.FindAncestors(mod => mod is PLSEditor.PLSEditorModule).FirstOrDefault();
            // HashSet<string> DefectLabelList = new HashSet<string>();
            // //-------------------------------------------------------------
            // // Récupération des infos des autres modules
            // //-------------------------------------------------------------
            // if (plsAncestor == null)
            // {
            //     DefectLabelList = FindAvailableDefectLabelsWithDummyDefect();
            // }
            // else
            // //-------------------------------------------------------------
            // // Récupération des infos du Pls s'il est présent
            // //-------------------------------------------------------------
            // if (!(Module is PLSEditor.PLSEditorModule))
            // {
            //     DefectLabelList.UnionWith(plsAncestor.ListDefectLabel);
            // }
            #endregion

            //-------------------------------------------------------------
            // Supprime les classes qui ont été supprimées en classif
            //-------------------------------------------------------------
            foreach (DefectRoughBin rbin in RoughBins.Values.ToList())
            {
                bool exist = DefectLabelList.Contains(rbin.DefectLabel);
                if (!exist)
                {
                    RoughBins.Remove(rbin.DefectLabel);
                    changed = true;
                }
            }

            //-------------------------------------------------------------
            // Vérifie que toutes les classes de défauts sont bien gérées
            //-------------------------------------------------------------
            foreach (string label in DefectLabelList)
            {
                bool missing = !RoughBins.ContainsKey(label);
                if (missing)
                {
                    DefectRoughBin rbin = new DefectRoughBin(label, 0);
                    RoughBins.Add(rbin.DefectLabel, rbin);
                    changed = true;
                }
            }

            if (changed)
                ReportChange();
        }

        public override bool HasSameValue(object obj)
        {
            var parameter = obj as KlarfEditorRoughBinParameter;
            return parameter != null &&
                   RoughBins.DictionaryEqual(parameter.RoughBins);
        }

    }
}
