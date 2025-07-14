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
    public class BF3DParameter : ParameterBase
    {
        public CustomExceptionDictionary<string, DataCollect_3D> DataCollect_3DList { get; private set; }

        //=================================================================
        // Constructeur
        //=================================================================
        public BF3DParameter(BF3DVIDModule module, string name) :
            base(module, name)
        {
            DataCollect_3DList = new CustomExceptionDictionary<string, DataCollect_3D>(exceptionKeyName: "Data Collection");
        }

        //=================================================================
        // XML
        //=================================================================
        public override void Load(XmlNodeList xmlNodes)
        {
            XmlNode node = ReadParameter("DataCollect_3D", xmlNodes);
            foreach (XmlNode childnode in node)
            {
                DataCollect_3D dtaCategory = Serializable.LoadFromXml<DataCollect_3D>(childnode);
                if (!DataCollect_3DList.ContainsKey(dtaCategory.DataName))
                    DataCollect_3DList.Add(dtaCategory.DataName, dtaCategory);
            }
        }

        public override XmlElement Save(XmlNode xmlNode)
        {
            XmlElement node = SaveParameter(xmlNode, "DataCollect_3D", this);

            foreach (DataCollect_3D dtaCategory in DataCollect_3DList.Values)
                dtaCategory.SerializeAsChildOf(node);

            return null;
        }

        //=================================================================
        // IHM
        //=================================================================
        private BF3DParameterViewModel _parameterViewModel;
        private BF3DParameterView _parameterUI;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterViewModel = new BF3DParameterViewModel(this);
                    _parameterUI = new BF3DParameterView(_parameterViewModel);
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
            HashSet<string> localDataCollect_3DList = GetAvailabledataColection();

            //-------------------------------------------------------------
            // Vérifie que toutes les variables sont bien gérées
            //-------------------------------------------------------------
            foreach (string dtalocal_name in localDataCollect_3DList)
            {
                DataCollect_3D dtaLocal; //string dtalocal_outname; 
                bool found = DataCollect_3DList.TryGetValue(dtalocal_name, out dtaLocal);
                if (!found)
                {
                    dtaLocal = new DataCollect_3D();
                    dtaLocal.DataName = dtalocal_name;

                    DataCollect_3DList.Add(dtaLocal.DataName, dtaLocal);
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
            HashSet<string> localDataCollect_3DList = GetAvailabledataColection();

            //-------------------------------------------------------------
            // Vérifie que toutes les variables sont bien gérées
            //-------------------------------------------------------------
            foreach (string dtalocal_name in localDataCollect_3DList)
            {
                DataCollect_3D dtaLocal; //string dtalocal_outname; 
                bool found = DataCollect_3DList.TryGetValue(dtalocal_name, out dtaLocal);
                if (!found)
                {
                    dtaLocal = new DataCollect_3D();
                    dtaLocal.DataName = dtalocal_name;

                    DataCollect_3DList.Add(dtaLocal.DataName, dtaLocal);
                }
            }
        }

        private HashSet<string> GetAvailabledataColection()
        {
            HashSet<string> DataCollect_3D = new HashSet<string>();

            List<enDataType_3D> dataTypeList = Enum.GetValues(typeof(enDataType_3D)).Cast<enDataType_3D>().ToList();

            foreach (enDataType_3D dataname in dataTypeList)
            {
                DataCollect_3D.Add(dataname.ToString());
            }

            return DataCollect_3D;
        }

        //=================================================================
        // 
        //=================================================================
        public override bool HasSameValue(object obj)
        {
            var parameter = obj as BF3DParameter;
            return parameter != null &&
                DataCollect_3DList.DictionaryEqual(parameter.DataCollect_3DList);
        }
        //=================================================================
        // 
        //=================================================================
        public void AddData(DataCollect_3D dtaCategory, string DataLabel)
        {
            DataCollect_3DList.Add(DataLabel, dtaCategory);
        }

    }
}
