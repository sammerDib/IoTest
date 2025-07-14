using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Service.Interface.Compatibility.Capability;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.Compatibility
{
    public class CompatibilityManager
    {
        private const string ComapatibilityFolderName = "Compatibility";
        private const string ProbeCompatibilityFileName = "ProbeCompatibility.xml";
        private ILogger _logger;
        private string _compatibilityFolderPath;
        private string _probeCompatibilityFilePath;
        public ProbeCompatibility Probe { get; set;}

        public CompatibilityManager(ILogger<CompatibilityManager> logger)
        {
            _logger = logger;
            Init();
        }

        private void Init()
        {
            _compatibilityFolderPath = Path.Combine(Path.GetDirectoryName(PathHelper.GetExecutingAssemblyPath()), ComapatibilityFolderName);
            if (!Directory.Exists(_compatibilityFolderPath))
                Directory.CreateDirectory(_compatibilityFolderPath);
            _probeCompatibilityFilePath = Path.Combine(_compatibilityFolderPath, ProbeCompatibilityFileName);
            LoadProbe();    
        }

        public void SaveProbe()
        {
            _logger.Debug("Save ProbeCompatibility");
            if (Probe == null)
                throw new InvalidOperationException("ProbeCompatibility is null, can't save it");
            else
                XML.Serialize(Probe, _probeCompatibilityFilePath);
        }

        public void LoadProbe()
        {
            _logger.Debug("Load ProbeCompatibility");
            if (!File.Exists(_probeCompatibilityFilePath))
                _logger.Information($"probe compatibility file is missing: {_probeCompatibilityFilePath}");
            else
                Probe = XML.Deserialize<ProbeCompatibility>(_probeCompatibilityFilePath);
        }
    }
}
