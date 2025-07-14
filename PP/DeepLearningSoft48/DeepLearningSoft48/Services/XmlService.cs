using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

using DeepLearningSoft48.Models;
using DeepLearningSoft48.Models.DefectAnnotations;

using UnitySC.Shared.Tools.Collection;

namespace DeepLearningSoft48.Services
{
    /// <summary>
    /// XmlService provides all the necessary functions to Serialise and Deserialise all the relevant information, such as: 
    ///  Modules
    ///  DefectCategories
    ///  Wafers (and their respective: FolderPath, DefectAnnotations)
    /// </summary>
    public class XmlService
    {
        #region SERIALISATION

        /// <summary>
        /// Serialise a list of modules
        /// </summary>
        public static void SerializeListModules(List<Module> list, string filename = "SavedModules.xml")
        {
            var serializer = new XmlSerializer(typeof(List<Module>), new XmlRootAttribute("Modules"));
            using (TextWriter writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, list);
            }
        }

        /// <summary>
        /// Serialise list of DefectCategories
        /// </summary>
        public static void SerializeListDefectCategories(List<DefectCategoryPair> list, string filename = "SavedDefectCategories.xml")
        {
            var serializer = new XmlSerializer(typeof(List<DefectCategoryPair>), new XmlRootAttribute("DefectCategories"));
            using (TextWriter writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, list);
            }
        }

        /// <summary>
        /// Serialise list of Wafers
        /// </summary>
        public static void SerializeListWafers(List<Wafer> wafersList = null, string path = null, Wafer selectedWafer = null, string filename = "SavedWafers.xml")
        {
            var wafers = new Wafers
            {
                FolderPath = new FolderPath
                {
                    Path = path,
                    Wafers = wafersList
                }
            };

            var serializer = new XmlSerializer(typeof(Wafers), new XmlRootAttribute("Wafers"));

            // If first time launching app and no wafer has been selected yet.
            if (selectedWafer == null)
            {
                using (TextWriter writer = new StreamWriter(filename))
                {
                    serializer.Serialize(writer, wafers);
                }
            }

            else
            {

                foreach (var wafer in wafersList)
                {
                    List<DefectAnnotation> waferDefects = wafer.DefectsAnnotationsList;

                    if (!waferDefects.IsNullOrEmpty())
                    {
                        foreach (DefectAnnotation waferDefect in wafer.DefectsAnnotationsList)
                        {
                            if (!waferDefects.Contains(waferDefect))
                                waferDefects.Add(waferDefect);
                        }

                        wafer.DefectsAnnotationsList = new List<DefectAnnotation>(waferDefects);
                    }

                    else if (waferDefects.IsNullOrEmpty())
                    {
                        waferDefects = wafer.DefectsAnnotationsList;
                        wafer.DefectsAnnotationsList = new List<DefectAnnotation>(waferDefects);
                    }
                }

                // Make sure file isn't being used by another process, otherwise kill the respective process, then try to serialise again
                try
                {
                    using (TextWriter writer = new StreamWriter(filename))
                    {
                        serializer.Serialize(writer, wafers);
                    }
                }
                catch (IOException)
                {
                    // File is already in use, terminate the process
                    string processName = GetProcessUsingFile(filename);
                    if (!string.IsNullOrEmpty(processName))
                    {
                        Console.WriteLine($"Terminating the process '{processName}' using the file...");
                        TerminateProcess(processName);
                        using (TextWriter writer = new StreamWriter(filename))
                        {
                            serializer.Serialize(writer, wafers);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Unable to determine the process using the file.");
                    }
                }
            }
        }

        #endregion

        #region DESERIALISATION

        /// <summary>
        /// Deserialise list of modules
        /// </summary>
        public static List<Module> DeserializeListModules(string filename = "SavedModules.xml")
        {
            try
            {
                List<Module> list;
                var deserializer = new XmlSerializer(typeof(List<Module>), new XmlRootAttribute("Modules"));
                using (var reader = new StreamReader(filename))
                {
                    list = (List<Module>)deserializer.Deserialize(reader);
                }
                return list;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("SavedModules.xml file not found: " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Deserialise list of DefectCategories
        /// </summary>
        public static List<DefectCategoryPair> DeserializeListDefectCategories(string filename = "SavedDefectCategories.xml")
        {
            try
            {
                List<DefectCategoryPair> list;
                var deserializer = new XmlSerializer(typeof(List<DefectCategoryPair>), new XmlRootAttribute("DefectCategories"));
                using (var reader = new StreamReader(filename))
                {
                    list = (List<DefectCategoryPair>)deserializer.Deserialize(reader);
                }
                return list;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("SavedDefectCategories.xml file not found: " + ex.Message);
                throw;
            }

        }

        /// <summary>
        /// Deserialise list of Wafers (as well as their respective FolderPath and DefectAnnotations)
        /// </summary>
        public static (string folderPath, List<Wafer> wafers) DeserializeListWafers(string filename = "SavedWafers.xml")
        {
            try
            {
                var serializer = new XmlSerializer(typeof(Wafers), new XmlRootAttribute("Wafers"));
                using (var reader = new StreamReader(filename))
                {
                    var wafers = (Wafers)serializer.Deserialize(reader);

                    // Iterate over each wafer and fetch defect annotations
                    foreach (var wafer in wafers.FolderPath.Wafers)
                    {
                        var defectAnnotations = new List<DefectAnnotation>();

                        if (wafer.DefectsAnnotationsList != null)
                        {
                            foreach (var annotation in wafer.DefectsAnnotationsList)
                            {
                                switch (annotation)
                                {
                                    case BoundingBox boundingBox:
                                        defectAnnotations.Add(boundingBox);
                                        break;

                                    case LineAnnotation lineAnnotation:
                                        defectAnnotations.Add(lineAnnotation);
                                        break;

                                    case PolygonAnnotation polygonAnnotation:
                                        defectAnnotations.Add(polygonAnnotation);
                                        break;

                                    case PolylineAnnotation polylineAnnotation:
                                        defectAnnotations.Add(polylineAnnotation);
                                        break;
                                }
                            }
                        }
                        wafer.DefectsAnnotationsList = defectAnnotations;
                    }
                    return (folderPath: wafers.FolderPath.Path, wafers: wafers.FolderPath.Wafers);
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("SavedWafers.xml file not found: " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("deserialization error: " + ex.Message);
                throw;
            }
        }

        #endregion

        #region HELPERS

        public static string GetProcessUsingFile(string filename)
        {
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                try
                {
                    using (FileStream fileStream = File.Open(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        Console.WriteLine("The file is not locked by any other process.");
                    }
                }
                catch (IOException ex)
                {
                    if (IsFileLocked(ex))
                        return process.ProcessName;
                }
            }

            return null;
        }

        public static bool IsFileLocked(Exception exception)
        {
            int errorCode = System.Runtime.InteropServices.Marshal.GetHRForException(exception) & ((1 << 16) - 1);
            return errorCode == 32 || errorCode == 33;
        }

        public static void TerminateProcess(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process process in processes)
                process.Kill();
        }

        #endregion
    }
}
