using System.Collections.Generic;
using System.Linq;

using DeepLearningSoft48.Models;
using DeepLearningSoft48.Models.DefectAnnotations;
using DeepLearningSoft48.ViewModels.DefectAnnotations;

using UnitySC.Shared.Tools;

namespace DeepLearningSoft48.Services
{
    /// <summary>
    /// WaferServices allows seamless communication and operations between all the relevant ViewModels for all the different necessary information such as
    ///  FolderPath 
    ///  List of Wafers, as well as their respective Defect Annotations and all the operations performed on them such as
    ///     Adding a DefectAnnotation on a SelectedWafer
    ///     Modifying (updating) a DefectAnnotation on a SelectedWafer
    ///     Deleting a DefectAnnotation on a SelectedWafer
    ///     Clearing all DefectAnnotations on a SelectedWafer
    /// </summary>
    public class WaferService
    {
        #region PROPERTIES
        public static List<Wafer> DeserialisedWafers = new List<Wafer>();
        public static string DeserialisedFolderPath = "";
        private static Wafer s_selectedWafer;
        public static Wafer SelectedWafer
        {
            get { return s_selectedWafer; }
            set
            {
                if (s_selectedWafer != value)
                {
                    // Find the corresponding selected wafer from the DeserialisedWafers collection
                    s_selectedWafer = DeserialisedWafers.FirstOrDefault(wafer => wafer.BaseName.Equals(value.BaseName));
                }
            }
        }
        private static Mapper s_mapper = ClassLocator.Default.GetInstance<Mapper>();
        private static DefectAnnotation s_previousDefectAnnotation;
        public static bool IsClearCommanded = false;

        #endregion

        #region METHODS

        /// <summary>
        /// Fill each wafer's property if a wafer has been selected, further updating them if a change occured.
        /// </summary>
        public static void FillWafersProperties()
        {
            foreach (Wafer wafer in DeserialisedWafers)
            {
                if (wafer.BaseName.Equals(SelectedWafer.BaseName))
                {
                    // Set the Selected Wafer to the same instance of 'wafer'
                    SelectedWafer = wafer;

                    // Fill the Selected Wafer (& wafer) DefectAnnotationsCollection used for the canvas
                    foreach (DefectAnnotation annotation in wafer.DefectsAnnotationsList)
                    {
                        switch (annotation)
                        {
                            case BoundingBox boundingBox when !wafer.DefectsAnnotationsCollection.Contains(s_mapper.AutoMap.Map<BoundingBoxVM>(boundingBox)):
                                wafer.DefectsAnnotationsCollection.Add(s_mapper.AutoMap.Map<BoundingBoxVM>(boundingBox));
                                break;

                            case PolylineAnnotation polyline when !wafer.DefectsAnnotationsCollection.Contains(s_mapper.AutoMap.Map<PolylineAnnotationVM>(polyline)):
                                wafer.DefectsAnnotationsCollection.Add(s_mapper.AutoMap.Map<PolylineAnnotationVM>(polyline));
                                break;

                            case LineAnnotation line when !wafer.DefectsAnnotationsCollection.Contains(s_mapper.AutoMap.Map<LineAnnotationVM>(line)):
                                wafer.DefectsAnnotationsCollection.Add(s_mapper.AutoMap.Map<LineAnnotationVM>(line));
                                break;

                            case PolygonAnnotation polygon when !wafer.DefectsAnnotationsCollection.Contains(s_mapper.AutoMap.Map<PolygonAnnotationVM>(polygon)):
                                wafer.DefectsAnnotationsCollection.Add(s_mapper.AutoMap.Map<PolygonAnnotationVM>(polygon));
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Store the previous (older) properties the Defect Annotation before it is modified.
        /// This is necessary to help us find it in the DefectAnnotation list of the SelectedWafer, remove it then replace it with the newer one. 
        /// </summary>
        public static void SetPreviousDefectAnnotation(DefectAnnotation previousDefectAnnotation)
        {
            s_previousDefectAnnotation = previousDefectAnnotation;
        }

        /// <summary>
        /// Add a DefectAnnotation to the SelectedWafer's DefectsAnnotationsCollection, then apply.
        /// </summary>
        public static void AddDefectAnnotation(DefectAnnotationVM defectAnnotationVM)
        {
            if (!SelectedWafer.DefectsAnnotationsCollection.Contains(defectAnnotationVM))
                SelectedWafer.DefectsAnnotationsCollection.Add(defectAnnotationVM);

            foreach (Wafer wafer in DeserialisedWafers)
            {
                if (wafer.BaseName.Equals(SelectedWafer.BaseName) && !wafer.DefectsAnnotationsCollection.Contains(defectAnnotationVM))
                    wafer.DefectsAnnotationsCollection.Add(defectAnnotationVM);
            }

            ApplyAnnotations();
        }

        /// <summary>
        /// Remove a DefectAnnotation from the SelectedWafer's DefectsAnnotationsCollection, then apply.
        /// </summary>
        public static void RemoveDefectAnnotation(DefectAnnotationVM defectAnnotationVM)
        {
            SelectedWafer.DefectsAnnotationsCollection.Remove(defectAnnotationVM);

            SelectedWafer.DefectsAnnotationsList.RemoveAll(annotation =>
            {
                switch (annotation)
                {
                    case BoundingBox boundingBox when BoundingBoxPropertiesMatch(boundingBox, defectAnnotationVM):
                    case PolylineAnnotation polylineAnnotation when PolylineAnnotationPropertiesMatch(polylineAnnotation, defectAnnotationVM):
                    case LineAnnotation lineAnnotation when LineAnnotationPropertiesMatch(lineAnnotation, defectAnnotationVM):
                    case PolygonAnnotation polygonAnnotation when PolygonAnnotationPropertiesMatch(polygonAnnotation, defectAnnotationVM):
                        return true; // true for the matching cases, indicate to the 'RemoveAll' that the 'annotation' should be removed

                    default:
                        return false; // if the annotation doesn't match, indicate false to the 'RemoveAll': the 'annotation' shouldn't be removed
                }
            });

            foreach (Wafer wafer in DeserialisedWafers)
            {
                if (wafer.BaseName.Equals(SelectedWafer.BaseName))
                {
                    wafer.DefectsAnnotationsCollection.Remove(defectAnnotationVM);
                    wafer.DefectsAnnotationsList.RemoveAll(annotation =>
                    {
                        switch (annotation)
                        {
                            case BoundingBox boundingBox when BoundingBoxPropertiesMatch(boundingBox, defectAnnotationVM):
                            case PolylineAnnotation polylineAnnotation when PolylineAnnotationPropertiesMatch(polylineAnnotation, defectAnnotationVM):
                            case LineAnnotation lineAnnotation when LineAnnotationPropertiesMatch(lineAnnotation, defectAnnotationVM):
                            case PolygonAnnotation polygonAnnotation when PolygonAnnotationPropertiesMatch(polygonAnnotation, defectAnnotationVM):
                                return true; // true for the matching cases, indicate to the 'RemoveAll' that the 'annotation' should be removed

                            default:
                                return false; // if the annotation doesn't match, indicate false to the 'RemoveAll': the 'annotation' shouldn't be removed
                        }
                    });
                }
            }

            ApplyAnnotations();
        }

        /// <summary>
        /// Clear ALL DefectAnnotations on the SelectedWafer's DefectsAnnotationsCollection and DefectsAnnotationsList, then apply.
        /// </summary>
        public static void ClearAllDefectAnnotations()
        {
            SelectedWafer.DefectsAnnotationsCollection.Clear();
            SelectedWafer.DefectsAnnotationsList.Clear();

            foreach (Wafer wafer in DeserialisedWafers)
            {
                if (wafer.BaseName.Equals(SelectedWafer.BaseName))
                {
                    wafer.DefectsAnnotationsCollection.Clear();
                    wafer.DefectsAnnotationsList.Clear();
                }
            }

            ApplyAnnotations();
            IsClearCommanded = true;
        }

        /// <summary>
        /// Apply the DefectAnnotations on the SelectedWafer's DefectsAnnotationsList by iterating over the deserialised wafers' DefectsAnnotationsCollection.
        /// </summary>
        public static void ApplyAnnotations()
        {
            foreach (Wafer wafer in DeserialisedWafers)
            {
                if (wafer.BaseName.Equals(SelectedWafer.BaseName))
                {
                    wafer.DefectsAnnotationsList.Clear();

                    foreach (DefectAnnotationVM annotationVM in wafer.DefectsAnnotationsCollection)
                    {
                        switch (annotationVM)
                        {
                            case BoundingBoxVM bboxVM when !wafer.DefectsAnnotationsList.Contains(s_mapper.AutoMap.Map<BoundingBox>(bboxVM)):
                                wafer.DefectsAnnotationsList.Add(s_mapper.AutoMap.Map<BoundingBox>(bboxVM));
                                break;

                            case PolylineAnnotationVM polylineVM when !wafer.DefectsAnnotationsList.Contains(s_mapper.AutoMap.Map<PolylineAnnotation>(polylineVM)):
                                wafer.DefectsAnnotationsList.Add(s_mapper.AutoMap.Map<PolylineAnnotation>(polylineVM));
                                break;

                            case LineAnnotationVM lineVM when !wafer.DefectsAnnotationsList.Contains(s_mapper.AutoMap.Map<LineAnnotation>(lineVM)):
                                wafer.DefectsAnnotationsList.Add(s_mapper.AutoMap.Map<LineAnnotation>(lineVM));
                                break;

                            case PolygonAnnotationVM polygonVM when !wafer.DefectsAnnotationsList.Contains(s_mapper.AutoMap.Map<PolygonAnnotation>(polygonVM)):
                                wafer.DefectsAnnotationsList.Add(s_mapper.AutoMap.Map<PolygonAnnotation>(polygonVM));
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Serialise all DefectAnnotations' addition, modification and removal upon press of Save button.
        /// </summary>
        public static void SaveProgress(string customFileName)
        {
            // Serialise
            if (DeserialisedWafers != null && DeserialisedFolderPath != null && SelectedWafer != null)
                XmlService.SerializeListWafers(DeserialisedWafers, DeserialisedFolderPath, SelectedWafer, customFileName);
        }

        /// <summary>
        /// Update a DefectAnnotation within the SelectedWafer's DefectsAnnotationsList.
        /// </summary>
        public static void UpdateAnnotation(DefectAnnotation newDefectAnnotation, Wafer selectedWafer)
        {
            if (newDefectAnnotation != null && selectedWafer != null && selectedWafer.DefectsAnnotationsList != null && DeserialisedWafers != null)
            {
                foreach (Wafer wafer in DeserialisedWafers)
                {
                    // Replace the Old (Previous) Defect Annotation (s_previousDefectAnnotation) by the New Defect Annotation (newDefectAnnotation) within the SelectedWafer's DefectsAnnotations List
                    if (wafer.BaseName.Equals(selectedWafer.BaseName))
                    {
                        foreach (DefectAnnotation defectAnnotation in wafer.DefectsAnnotationsList)
                        {
                            if (defectAnnotation.Equals(s_previousDefectAnnotation) && !wafer.DefectsAnnotationsList.Contains(newDefectAnnotation))
                            {
                                wafer.DefectsAnnotationsList.Remove(defectAnnotation);
                                wafer.DefectsAnnotationsList.Add(newDefectAnnotation);
                                break;
                            }
                        }
                        selectedWafer.DefectsAnnotationsList = wafer.DefectsAnnotationsList;
                    }
                }
            }
        }
        #endregion

        #region HELPERS

        /// <summary>
        /// Check if a DefectAnnotationVM instance matches a BoundingBox instance.
        /// </summary>
        private static bool BoundingBoxPropertiesMatch(BoundingBox boundingBox, DefectAnnotationVM defectAnnotationVM)
        {
            // Compare relevant properties of boundingBox and defectAnnotationVM
            // Return true if they match, false otherwise
            return defectAnnotationVM.OriginX.Equals(boundingBox.OriginX)
                && defectAnnotationVM.OriginY.Equals(boundingBox.OriginY)
                && defectAnnotationVM.Width.Equals(boundingBox.Width)
                && defectAnnotationVM.Height.Equals(boundingBox.Height)
                && defectAnnotationVM.Category.Equals(boundingBox.Category)
                && defectAnnotationVM.Source.Equals(boundingBox.Source)
                && defectAnnotationVM.Type.Equals(boundingBox.Type);
        }

        /// <summary>
        /// Check if a DefectAnnotationVM instance matches a PolylineAnnotation instance.
        /// </summary>
        private static bool PolylineAnnotationPropertiesMatch(PolylineAnnotation polylineAnnotation, DefectAnnotationVM defectAnnotationVM)
        {
            // Compare relevant properties of polylineAnnotation and defectAnnotationVM
            // Return true if they match, false otherwise
            return defectAnnotationVM.OriginX.Equals(polylineAnnotation.OriginX)
               && defectAnnotationVM.OriginY.Equals(polylineAnnotation.OriginY)
               && defectAnnotationVM.Width.Equals(polylineAnnotation.Width)
               && defectAnnotationVM.Height.Equals(polylineAnnotation.Height)
               && defectAnnotationVM.Category.Equals(polylineAnnotation.Category)
               && defectAnnotationVM.Source.Equals(polylineAnnotation.Source)
               && defectAnnotationVM.Type.Equals(polylineAnnotation.Type);
        }

        /// <summary>
        /// Check if a DefectAnnotationVM instance matches a LineAnnotation instance.
        /// </summary>
        private static bool LineAnnotationPropertiesMatch(LineAnnotation lineAnnotation, DefectAnnotationVM defectAnnotationVM)
        {
            // Compare relevant properties of lineAnnotation and defectAnnotationVM
            // Return true if they match, false otherwise
            return defectAnnotationVM.OriginX.Equals(lineAnnotation.OriginX)
               && defectAnnotationVM.OriginY.Equals(lineAnnotation.OriginY)
               && defectAnnotationVM.Width.Equals(lineAnnotation.Width)
               && defectAnnotationVM.Height.Equals(lineAnnotation.Height)
               && defectAnnotationVM.Category.Equals(lineAnnotation.Category)
               && defectAnnotationVM.Source.Equals(lineAnnotation.Source)
               && defectAnnotationVM.Type.Equals(lineAnnotation.Type);
        }

        /// <summary>
        /// Check if a DefectAnnotationVM instance matches a PolygonAnnotation instance.
        /// </summary>
        private static bool PolygonAnnotationPropertiesMatch(PolygonAnnotation polygonAnnotation, DefectAnnotationVM defectAnnotationVM)
        {
            // Compare relevant properties of polygonAnnotation and defectAnnotationVM
            // Return true if they match, false otherwise
            return defectAnnotationVM.OriginX.Equals(polygonAnnotation.OriginX)
                && defectAnnotationVM.OriginY.Equals(polygonAnnotation.OriginY)
                && defectAnnotationVM.Width.Equals(polygonAnnotation.Width)
                && defectAnnotationVM.Height.Equals(polygonAnnotation.Height)
                && defectAnnotationVM.Category.Equals(polygonAnnotation.Category)
                && defectAnnotationVM.Source.Equals(polygonAnnotation.Source)
                && defectAnnotationVM.Type.Equals(polygonAnnotation.Type);
        }
        #endregion
    }
}
