using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.EP.Mountains.Interface
{
    public class MountainsConfiguration
    {
        static public MountainsConfiguration Init(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("MountainsConfiguration file is missing");

            var config = XML.Deserialize<MountainsConfiguration>(path);
            return config;
        }

        public string ActiveXProcessName { get; set; }

        public bool ActiveXIsVisible { get; set; }

        public string StatisticsDocumentFolderPath {get;set;}

        public string TemplatesFolderPath { get; set; }

        public bool IsHostedByPM { get; set; }

        public ServiceAddress Address { get; set; }
    }
}
