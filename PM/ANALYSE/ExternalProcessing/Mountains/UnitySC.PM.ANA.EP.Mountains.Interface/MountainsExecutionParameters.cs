using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.PM.ANA.EP.Mountains.Interface
{
    [DataContract]
    public class MountainsExecutionParameters
    {
        /// <summary>
        ///     Statistics file path (*.mnst)
        /// </summary>
        [DataMember]
        public string StatisticsDocumentFilePath { get; set; }

        /// <summary>
        ///     Open the statistcs file
        /// </summary>
        [DataMember]
        public bool OpenStatistics { get; set; }

        /// <summary>
        ///     Creat the statistics file
        /// </summary>
        [DataMember] 
        public bool UseStatistics { get; set; }

        /// <summary>
        ///     Print PDF of result
        /// </summary>
        [DataMember] 
        public bool PrintPDF { get; set; }

        /// <summary>
        ///     Save csv result
        /// </summary>
        [DataMember] 
        public bool SaveCSV { get; set; }

        /// <summary>
        ///     Save result file (*.mnt)
        /// </summary>
        [DataMember]
        public bool SaveResultFile { get; set; }

        /// <summary>
        /// Mountains Template file path
        /// </summary>
        [DataMember] 
        public string TemplateFile { get; set; }

        /// <summary>
        /// Result folder path
        /// </summary>
        [DataMember] 
        public string ResultFolderPath { get; set; }

        /// <summary>
        /// Result File Name
        /// </summary>
        [DataMember]
        public string ResultFileName { get; set; }

        /// <summary>
        /// Measure point data
        /// </summary>
        [DataMember] 
        public PointData PointData { get; set; }
        
    }
}
