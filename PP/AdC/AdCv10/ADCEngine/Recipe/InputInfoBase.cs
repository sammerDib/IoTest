using System.Collections.Generic;
using System.Xml.Serialization;

using AcquisitionAdcExchange;

using AdcTools;

namespace ADCEngine
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Classe de base pour définir les inputs de la recette (i.e. les images
    /// chargées pour une layer).
    /// Cette classe et ses dérivées définissent le format de sauvegarde des
    /// inputs dans le fichier recette.
    /// 
    /// NB: Ne pas confondre 
    /// - les Layers: Contient les info sur les "couches" de traitements, une
    /// par DataLoader.
    /// - les Inputs: contient les infos sur les "couches" de l'Acquisition,
    /// peut-être associée à 0 ou N DataLoader.
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    public class InputInfoBase : Serializable
    {
        /// <summary>
        /// Liste des DataLoaders associés à cette "input"
        /// </summary>
        [System.Reflection.Obfuscation(Exclude = true)]
        [XmlElement] public List<int> DataLoaderIdList = new List<int>();

        /// <summary> Liste des Contextes-machine </summary>
        // Malheureusement la sérialisation doit se faire à la main pour des problèmes
        // de classes dérivées qui ne sont pas connues.
        [XmlIgnore]
        public List<ContextMachine> ContextMachineList = new List<ContextMachine>();

        public SerializableDictionary<LayerMetaData, string> MetaData = new SerializableDictionary<LayerMetaData, string>();
    }
}
