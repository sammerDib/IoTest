using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

namespace AdvancedModules.ClassificationMultiLayer
{
    ///////////////////////////////////////////////////////////////////////
    // Classe de défauts
    ///////////////////////////////////////////////////////////////////////
    [Serializable]
    public class MultiLayerDefectClass : Serializable, IValueComparer
    {
        public string DefectLabel;
        public List<DefectTestType> DefectBranchList = new List<DefectTestType>();
        public int MeasuredBranch;

        [XmlIgnore]
        public Characteristic CharacteristicForAutomaticLayer = ClusterCharacteristics.Area;
        public string CharacteristicNameForAutomaticLayer   // pour la sérialisation
        {
            get
            {
                if (MeasuredBranch >= 0)
                    return "";
                else
                    return CharacteristicForAutomaticLayer.ToString();
            }
            set
            {
                if (value == null || value == "")
                    CharacteristicForAutomaticLayer = ClusterCharacteristics.Area;
                else
                    CharacteristicForAutomaticLayer = Characteristic.Parse(value);
            }
        }

        //=================================================================
        // Constructeur
        //=================================================================
        public MultiLayerDefectClass()
        {
            MeasuredBranch = ClassificationMultiLayerModule.MeasuredBranchAutomatic;
            CharacteristicForAutomaticLayer = ClusterCharacteristics.Area;
        }

        //=================================================================
        // 
        //=================================================================
        public override string ToString()
        {
            return DefectLabel;
        }

        public bool HasSameValue(object obj)
        {
            var @class = obj as MultiLayerDefectClass;
            return @class != null &&
                   DefectLabel == @class.DefectLabel &&
                   DefectBranchList.SequenceEqual(@class.DefectBranchList) &&
                   MeasuredBranch == @class.MeasuredBranch &&
                   CharacteristicForAutomaticLayer == @class.CharacteristicForAutomaticLayer;
        }

    }
}
