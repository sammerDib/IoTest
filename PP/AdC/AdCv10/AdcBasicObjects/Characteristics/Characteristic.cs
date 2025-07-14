using System;
using System.Collections.Generic;

using ADCEngine;


namespace AdcBasicObjects
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Classe de base des caractéristiques des blobs/clusters.
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    public class Characteristic : IValueComparer
    {
        /// <summary>
        /// La liste de toutes les caractéristiques
        /// </summary>
        public static List<Characteristic> List;

        public string Name { get; private set; }
        public string Unit; // TODO
        public Type Type { get; private set; }  // The data type of the characteristic (ex: double, Rectangle...)

        //=================================================================
        // Contructeur
        //=================================================================
        public Characteristic(Type type, string name)
        {
            Characteristic.List.Add(this);
            Type = type;
            Name = name;
        }

        //=================================================================
        // 
        //=================================================================
        public override string ToString()
        {
            return Name;
        }

        //=================================================================
        // 
        //=================================================================
        public static Characteristic Parse(string str)
        {
            Characteristic carac = Characteristic.List.Find(x => x.Name == str);
            if (carac == null)
                throw new ApplicationException("unknown characteristic " + str);

            return carac;
        }

        public bool HasSameValue(object obj)
        {
            var charact = obj as Characteristic;
            return charact != null && Name == charact.Name;
        }

        private bool _isInitialized = Init();
        public static bool Init()
        {
            if (List != null)
                return false;

            List = new List<Characteristic>();
            BlobCharacteristics.Init();
            Cluster2DCharacteristics.Init();
            Blob2DCharacteristics.Init();
            Cluster3DCharacteristics.Init();
            Blob3DCharacteristics.Init();
            ClusterCharacteristics.Init();
            ClusterPSLCharacteristics.Init();
            SizingCharacteristics.Init();
            return true;
        }

    }
}
