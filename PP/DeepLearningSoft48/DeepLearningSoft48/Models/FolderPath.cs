using System;
using System.Collections.Generic;

namespace DeepLearningSoft48.Models
{
    [Serializable]
    public class FolderPath
    {
        public string Path { get; set; }
        public List<Wafer> Wafers { get; set; }
    }
}
