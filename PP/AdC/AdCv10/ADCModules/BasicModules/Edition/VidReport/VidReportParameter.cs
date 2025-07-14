using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Xml;

using ADCEngine;

using AdcTools;

namespace BasicModules.VidReport
{
    public class VidReportParameter : ClassificationParameterBase
    {
        public CustomExceptionDictionary<string, ReportClass> ReportClasses { get; private set; }

        //=================================================================
        // Constructeur
        //=================================================================
        public VidReportParameter(VidReportModule module, string name) :
            base(module, name)
        {
            ReportClasses = new CustomExceptionDictionary<string, ReportClass>(exceptionKeyName: "Defect Label");
        }

        //=================================================================
        // XML
        //=================================================================
        public override void Load(XmlNodeList xmlNodes)
        {
            XmlNode node = ReadParameter("Class", xmlNodes);
            foreach (XmlNode childnode in node)
            {
                ReportClass category = Serializable.LoadFromXml<ReportClass>(childnode);
                ReportClasses.Add(category.DefectLabel, category);
            }
        }

        public override XmlElement Save(XmlNode xmlNode)
        {
            XmlElement node = SaveParameter(xmlNode, "Class", this);

            foreach (ReportClass defectCategory in ReportClasses.Values)
                defectCategory.SerializeAsChildOf(node);

            return null;
        }

        //=================================================================
        // IHM
        //=================================================================
        private VidReportViewModel _parameterViewModel;
        private VidReportControl _parameterUI;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterViewModel = new VidReportViewModel(this);
                    _parameterUI = new VidReportControl(_parameterViewModel);
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
            HashSet<string> defectClassList = FindAvailableDefectLabelsWithDummyDefect();

            #region PLSEditor Obsolete to refacto full feature
            //var Pls = (PLSEditor.PLSEditorModule)Module.FindAncestors(mod => mod is PLSEditor.PLSEditorModule).FirstOrDefault();
            //if (Pls != null)
            //{
            //    //defectClassList.Add(Pls.DefectAll.label);
            //    defectClassList.Add(Pls.DefectAdders.label);
            //    defectClassList.Add(Pls.DefectCommons.label);
            //    defectClassList.Add(Pls.DefectRemoved.label);
            //}
            #endregion

            //-------------------------------------------------------------
            // Vérifie que toutes les classes de défauts sont bien gérées
            //-------------------------------------------------------------
            foreach (string label in defectClassList)
            {
                ReportClass reportClass;
                bool found = ReportClasses.TryGetValue(label, out reportClass);
                if (!found)
                {
                    reportClass = new ReportClass();
                    reportClass.DefectLabel = label;

                    ReportClasses.Add(reportClass.DefectLabel, reportClass);
                }
            }

            //-------------------------------------------------------------
            // Supprime les classes en trop dans le paramètre et la collection
            //-------------------------------------------------------------
            foreach (ReportClass reportClass in ReportClasses.Values.ToList())
            {
                bool found = defectClassList.Contains(reportClass.DefectLabel);
                if (!found)
                    ReportClasses.Remove(reportClass.DefectLabel);
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
            HashSet<string> defectClassList = FindAvailableDefectLabelsWithDummyDefect();

            //-------------------------------------------------------------
            // Vérifie que toutes les classes de défauts sont bien gérées
            //-------------------------------------------------------------
            int count = 0;
            string error = null;

            foreach (string label in defectClassList)
            {
                ReportClass reportClass;
                bool found = ReportClasses.TryGetValue(label, out reportClass);
                if (!found)
                {
                    if (count++ < 5)
                    {
                        if (count > 1)
                            error += Environment.NewLine;
                        error += "New defect class:" + label;
                    }
                    else
                        return "New defect classes, review configuration";
                }
                else
                {
                    if (reportClass.VID < 0 || reportClass.VidLabel == null || reportClass.VidLabel == "")
                        return "Invalid VID for defect class: " + reportClass.DefectLabel;
                }
            }
            if (error != null)
                return "Review configuration," + error;

            //-------------------------------------------------------------
            // Supprime les classes en trop dans le paramètre et la collection
            //-------------------------------------------------------------
            foreach (ReportClass reportClass in ReportClasses.Values.ToList())
            {
                bool found = defectClassList.Contains(reportClass.DefectLabel);
                if (!found)
                    return "Defect classes have changed, review configuration";
            }

            //-------------------------------------------------------------
            // Sinon tout va bien
            //-------------------------------------------------------------
            return null;
        }

        public override bool HasSameValue(object obj)
        {
            var parameter = obj as VidReportParameter;
            return parameter != null &&
                   ReportClasses.DictionaryEqual(parameter.ReportClasses);
        }

    }
}
