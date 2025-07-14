using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Xml;

using AdcBasicObjects;

using ADCEngine;

namespace BasicModules.MilCharacterization
{
    public class MilCharacterizationParameter : ParameterBase
    {
        //=================================================================
        // Propriétées
        //=================================================================

        // La liste des caractéristiques
        //..............................
        public List<Characteristic> ClusterCharacteristicList { get; private set; }

        /// <summary>
        /// Liste des caractéristiques disponible pour l'aide
        /// </summary>
        public override List<string> ValueList
        {
            get
            {
                return ((MilCharacterizationModule)Module).SupportedCharacteristics.Select(x => x.Name).ToList();
            }
        }

        //=================================================================
        // Constructeur
        //=================================================================
        public MilCharacterizationParameter(MilCharacterizationModule module, string name)
            : base(module, name)
        {
            ClusterCharacteristicList = new List<Characteristic>()
            {
                ClusterCharacteristics.Area, ClusterCharacteristics.Length, ClusterCharacteristics.BlobAverageGreyLevel, ClusterCharacteristics.AbsolutePosition
            };
        }

        //=================================================================
        // XML
        //=================================================================
        public override void Load(XmlNodeList xmlNodes)
        {
            MilCharacterizationModule module = (MilCharacterizationModule)Module;

            //-------------------------------------------------------------
            // Recuperation des caracteristiques à partitr du XML
            //-------------------------------------------------------------
            ClusterCharacteristicList.Clear();

            foreach (XmlNode node in xmlNodes)
            {
                if (node.Attributes["Label"].Value == "Characteristic")
                {
                    // Lecture de la carac à partir du XML
                    //....................................
                    string str = node.Attributes["Value"].Value;
                    Characteristic carac = module.SupportedCharacteristics.Find(x => x.Name == str);
                    if (carac == null)
                        throw new ApplicationException("unknown characteristic " + str);

                    ClusterCharacteristicList.Add(carac);
                }
            }
        }

        public override XmlElement Save(XmlNode xmlNode)
        {
            foreach (Characteristic carac in ClusterCharacteristicList)
                SaveParameter(xmlNode, "Characteristic", carac);

            return null;
        }

        public override bool HasSameValue(object obj)
        {
            var parameter = obj as MilCharacterizationParameter;
            return parameter != null &&
                ClusterCharacteristicList.ValuesEqual(parameter.ClusterCharacteristicList);
        }

        //=================================================================
        // IHM
        //=================================================================
        private MilCharacterizationControl _parameterUI;
        private MilCharacterizationViewModel _parameterVM;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterVM = new MilCharacterizationViewModel(this);
                    _parameterUI = new MilCharacterizationControl();
                    _parameterUI.DataContext = _parameterVM;
                }

                return _parameterUI;
            }
        }

    }
}
