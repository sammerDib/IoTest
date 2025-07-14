using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Xml;

using ADCEngine;

using AdcTools;

using AdvancedModules.Edition.VID.View;
using AdvancedModules.Edition.VID.ViewModel;


namespace AdvancedModules.Edition.VID
{
    public class BF2DParameter : ParameterBase
    {
        public CustomExceptionDictionary<string, DataCollect> dataCollectList { get; private set; }

        //=================================================================
        // Constructeur
        //=================================================================
        public BF2DParameter(BF2DVIDModule module, string name) :
            base(module, name)
        {
            dataCollectList = new CustomExceptionDictionary<string, DataCollect>(exceptionKeyName: "Data Collection");
        }

        //=================================================================
        // XML
        //=================================================================
        public override void Load(XmlNodeList xmlNodes)
        {
            XmlNode node = ReadParameter("DataCollect", xmlNodes);
            foreach (XmlNode childnode in node)
            {
                DataCollect dtaCategory = Serializable.LoadFromXml<DataCollect>(childnode);
                if (!dataCollectList.ContainsKey(dtaCategory.DataName))
                    dataCollectList.Add(dtaCategory.DataName, dtaCategory);
            }
        }

        public override XmlElement Save(XmlNode xmlNode)
        {
            XmlElement node = SaveParameter(xmlNode, "DataCollect", this);

            foreach (DataCollect dtaCategory in dataCollectList.Values)
                dtaCategory.SerializeAsChildOf(node);

            return null;
        }

        //=================================================================
        // IHM
        //=================================================================
        private BF2DParameterViewModel _parameterViewModel;
        private BF2DParameterView _parameterUI;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterViewModel = new BF2DParameterViewModel(this);
                    _parameterUI = new BF2DParameterView(_parameterViewModel);
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
            // Récupération de la liste des variables
            //-------------------------------------------------------------
            HashSet<string> localdataCollectList = GetAvailabledataColection();

            //-------------------------------------------------------------
            // Vérifie que toutes les variables sont bien gérées
            //-------------------------------------------------------------
            foreach (string dtalocal_name in localdataCollectList)
            {
                DataCollect dtaLocal; //string dtalocal_outname; 
                bool found = dataCollectList.TryGetValue(dtalocal_name, out dtaLocal);
                if (!found)
                {
                    dtaLocal = new DataCollect();
                    dtaLocal.DataName = dtalocal_name;

                    dataCollectList.Add(dtaLocal.DataName, dtaLocal);
                }
            }

            return null;
        }
        //=================================================================
        // 
        //=================================================================
        public void Synchronize()
        {
            //-------------------------------------------------------------
            // Récupération de la liste des variables
            //-------------------------------------------------------------
            HashSet<string> localdataCollectList = GetAvailabledataColection();

            //-------------------------------------------------------------
            // Vérifie que toutes les variables sont bien gérées
            //-------------------------------------------------------------
            foreach (string dtalocal_name in localdataCollectList)
            {
                DataCollect dtaLocal; //string dtalocal_outname; 
                bool found = dataCollectList.TryGetValue(dtalocal_name, out dtaLocal);
                if (!found)
                {
                    dtaLocal = new DataCollect();
                    dtaLocal.DataName = dtalocal_name;

                    dataCollectList.Add(dtaLocal.DataName, dtaLocal);
                }
            }
        }

        private HashSet<string> GetAvailabledataColection()
        {
            HashSet<string> datacollect = new HashSet<string>();

            List<enDataType> dataTypeList = Enum.GetValues(typeof(enDataType)).Cast<enDataType>().ToList();

            foreach (enDataType dataname in dataTypeList)
            {
                datacollect.Add(dataname.ToString());
            }

            return datacollect;
        }

        //=================================================================
        // 
        //=================================================================
        public override bool HasSameValue(object obj)
        {
            var parameter = obj as BF2DParameter;
            return parameter != null &&
                dataCollectList.DictionaryEqual(parameter.dataCollectList);
        }
        //=================================================================
        // 
        //=================================================================
        public void AddData(DataCollect dtaCategory, string DataLabel)
        {
            dataCollectList.Add(DataLabel, dtaCategory);
        }

    }
}
