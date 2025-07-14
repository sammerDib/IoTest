using System;
using System.IO;

using UnitySC.Shared.Data;
using UnitySC.Shared.Tools;

namespace UnitySC.PP.Shared.Configuration
{
    /// <summary>
    ///  Définition du fichier XML de configuration des POST PROCESSING modules
    /// </summary>
    [Serializable]
    public class PPConfiguration : ModuleConfiguration
    {
        public new static PPConfiguration Init(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"PPConfiguration file is missing in <{path}>");
            var PPConfig = XML.Deserialize<PPConfiguration>(path);
            return PPConfig;
        }

        // Identity
        public int PPKey = -1;
    }
}
