using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Xml.Serialization;

using DeepLearningSoft48.Models.DefectAnnotations;
using DeepLearningSoft48.ViewModels.DefectAnnotations;

using UnitySC.Shared.LibMIL;

namespace DeepLearningSoft48.Models
{
    /// <summary>
    /// Model Class of the Wafer object
    /// </summary>
    [Serializable]
    public class Wafer
    {
        //------------------------------------------------------------
        // Properties
        //------------------------------------------------------------
        [XmlAttribute("baseName")]
        public string BaseName { get; set; }

        /// <summary>
        /// The list contains tuples which correspond to (LayerName, MilImages list for this layer).
        ///                                                             |-> the first image is the original image and then all possible processed images.
        /// </summary>
        [XmlIgnore]
        public Dictionary<string, List<MilImage>> WaferImagesLists { get; set; }

        /// <summary>
        /// When a wafer is locked, we are not able to annotate it anymore or even process its images.
        /// </summary>
        [XmlAttribute("isLocked")]
        public bool IsLocked { get; set; }

        [XmlArray("DefectAnnotations")]
        public List<DefectAnnotation> DefectsAnnotationsList;

        private ObservableCollection<DefectAnnotationVM> _defectsAnnotationsCollection;
        [XmlIgnore]
        public ObservableCollection<DefectAnnotationVM> DefectsAnnotationsCollection
        {
            get
            {
                return _defectsAnnotationsCollection;
            }
            set
            {
                _defectsAnnotationsCollection = value;
            }

        }

        //------------------------------------------------------------
        // Constructor
        //------------------------------------------------------------
        public Wafer(string baseName, Dictionary<string, List<MilImage>> waferImagesLists)
        {
            BaseName = baseName;
            WaferImagesLists = waferImagesLists;
            IsLocked = false;
            DefectsAnnotationsList = new List<DefectAnnotation>();
            DefectsAnnotationsCollection = new ObservableCollection<DefectAnnotationVM>();
            _defectsAnnotationsCollection.CollectionChanged += OnDefectsCollectionChanged;
        }

        public Wafer()
        {
            DefectsAnnotationsCollection = new ObservableCollection<DefectAnnotationVM>();
            _defectsAnnotationsCollection.CollectionChanged += OnDefectsCollectionChanged;
        }

        //------------------------------------------------------------
        // Helpers
        //------------------------------------------------------------
        private void OnDefectsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) { }

        /// <summary>
        /// Override the 'Equals' method to allow for proper comparision of wafer objects.
        /// Notably useful when we want to check whether a List of wafers contains a specific wafer object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Wafer wafer = (Wafer)obj;
            return BaseName.Equals(wafer.BaseName); // Two wafer objects will be considered equal if they have the same BaseName
        }

        /// <summary>
        /// Override the 'GetHashCode' method to allow for proper comparision of wafer objects.
        /// Notably useful when we want to check whether a List of wafers contains a specific wafer object.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + BaseName.GetHashCode();
                return hash;
            }
        }
    }
}
