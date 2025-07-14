using System;
using System.IO;

namespace DeepLearningSoft48.Utils
{
    public class RepresentationNameIdentifier
    {
        /// <summary>
        /// Get only the name of the layer among the entire path name.
        /// </summary>
        /// <param name="path"></param>
        /// <returns> RepresentationName </returns>
        public static string GetRepresentationName(string path)
        {
            // Split to take only the last part of the file name
            string[] filename = Path.GetFileName(path).Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            string RepresentationName = filename[8].Split('.')[0];
            Path.GetFileName(path);
            return RepresentationName;
        }
    }
}
