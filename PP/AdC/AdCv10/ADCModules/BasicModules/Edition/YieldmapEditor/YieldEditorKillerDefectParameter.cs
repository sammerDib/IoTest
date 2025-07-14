
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Xml;

using ADCEngine;

using AdcTools;

namespace BasicModules.YieldmapEditor
{
    public class YieldEditorKillerDefectParameter : ClassificationParameterBase
    {
        public Dictionary<string, PrmDefectKiller> DefectKillerStatus { get; private set; }

        //=================================================================
        // Constructeur
        //=================================================================
        public YieldEditorKillerDefectParameter(YieldmapEditorModule module, string name) :
            base(module, name)
        {
            DefectKillerStatus = new Dictionary<string, PrmDefectKiller>();

        }

        //=================================================================
        // XML
        //=================================================================
        public override void Load(XmlNodeList xmlNodes)
        {
            XmlNode node = ReadParameter("Killerstatus", xmlNodes);
            foreach (XmlNode childnode in node.ChildNodes)
            {
                PrmDefectKiller kstatus = Serializable.LoadFromXml<PrmDefectKiller>(childnode);
                DefectKillerStatus.Add(kstatus.DefectLabel, kstatus);
            }
        }


        public override XmlElement Save(XmlNode xmlNode)
        {
            XmlElement node = SaveParameter(xmlNode, "Killerstatus", this);

            foreach (PrmDefectKiller kStatus in DefectKillerStatus.Values)
                kStatus.SerializeAsChildOf(node);

            return null;
        }

        //=================================================================
        // IHM
        //=================================================================
        private YieldEditorKillerDefectViewModel _parameterViewModel;
        private YieldEditorKillerDefectControl _parameterUI;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterViewModel = new YieldEditorKillerDefectViewModel(this);
                    _parameterUI = new YieldEditorKillerDefectControl(_parameterViewModel);
                }
                return _parameterUI;
            }
        }

        //=================================================================
        // 
        //=================================================================
        public void Synchronize()
        {
            //-------------------------------------------------------------
            // Récupération des infos des autres modules
            //-------------------------------------------------------------
            HashSet<string> DefectLabelList = FindAvailableDefectLabels();

            #region PLSEditor Obsolete to refacto full feature
            // var Pls = (PLSEditor.PLSEditorModule)Module.FindAncestors(mod => mod is PLSEditor.PLSEditorModule).FirstOrDefault();
            // if (Pls != null)
            // {
            //     //DefectLabelList.Add(Pls.DefectAll.label);
            //     DefectLabelList.Add(Pls.DefectAdders.label);
            //     DefectLabelList.Add(Pls.DefectCommons.label);
            //     DefectLabelList.Add(Pls.DefectRemoved.label);
            // }
            #endregion

            //-------------------------------------------------------------
            // Supprime les classes de défauts qui ne servent plus 
            //-------------------------------------------------------------
            foreach (string label in DefectKillerStatus.Keys.ToList())
            {
                bool useful = DefectLabelList.Contains(label);
                if (!useful)
                    DefectKillerStatus.Remove(label);
            }

            //-------------------------------------------------------------
            // Vérifie que toutes les classes de défauts sont bien gérées
            //-------------------------------------------------------------
            foreach (string label in DefectLabelList)
            {
                bool missing = !DefectKillerStatus.ContainsKey(label);
                if (missing)
                {
                    PrmDefectKiller kstatus = new PrmDefectKiller(label, 0);
                    DefectKillerStatus.Add(kstatus.DefectLabel, kstatus);
                }
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
            HashSet<string> DefectLabelList = FindAvailableDefectLabels();

            //-------------------------------------------------------------
            // Est-ce que des classes ont été ajoutées ?
            //-------------------------------------------------------------
            int count = 0;
            string error = null;

            foreach (string label in DefectLabelList)
            {
                bool missing = !DefectKillerStatus.ContainsKey(label);
                if (missing)
                {
                    if (count++ < 5)
                    {
                        if (count > 1)
                            error += Environment.NewLine;

                        error += "New defect class: " + label;
                    }
                    else
                        return "Defect classes have changed, review configuration";
                }
            }

            if (error != null)
                return "Review configuration," + error;

            //-------------------------------------------------------------
            // Est-ce que des classes ont été supprimées ?
            //-------------------------------------------------------------
            foreach (string label in DefectKillerStatus.Keys.ToList())
            {
                bool useful = DefectLabelList.Contains(label);
                if (!useful)
                    return "Defect classes have changed, review configuration";
            }

            //-------------------------------------------------------------
            // Sinon tout va bien
            //-------------------------------------------------------------
            return null;
        }

        public override bool HasSameValue(object obj)
        {
            var parameter = obj as YieldEditorKillerDefectParameter;
            return parameter != null &&
                   DefectKillerStatus.DictionaryEqual(parameter.DefectKillerStatus);
        }

    }
}
