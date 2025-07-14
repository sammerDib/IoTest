using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

using Agileo.SemiDefinitions;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation.CarrierConfigurations.Models
{
    /// <summary>
    /// Represent a Model to store by Serialization the carrier configuration to be used by a simulator
    /// </summary>
    [Serializable]
    public class CarrierConfigurations
    {
        #region Variable
        private readonly string _simulatorSerializationPath = "SimulatorConfig.cfg";

        [field: NonSerialized]
        public event EventHandler CarrierConfigChanged;
        private Dictionary<string, CarrierConfiguration> _carrierConfigList;

        #endregion

        #region Constructors and Static access

        /// <summary>
        /// Constructor by default of SimulatorEquipmentData
        /// </summary>
        public CarrierConfigurations()
        {
            _carrierConfigList = new Dictionary<string, CarrierConfiguration>();
        }

        public static CarrierConfigurations Deserialize(string path)
        {
            CarrierConfigurations carriersConfiguration;
            if (File.Exists(path))
            {
                FileStream fileStream = null;
                try
                {
                    fileStream = new FileStream(path, FileMode.Open);
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    carriersConfiguration = (CarrierConfigurations)binaryFormatter.Deserialize(fileStream);
                }
                catch
                {
                    carriersConfiguration = new CarrierConfigurations();
                    carriersConfiguration.Add("200mm, 13", "idCarrierId1", SampleDimension.S200mm, 13);
                    carriersConfiguration.Add("200mm, 25", "idCarrierId2", SampleDimension.S200mm, 25);
                    carriersConfiguration.Add("300mm, 13", "idCarrierId3", SampleDimension.S300mm, 13);
                    carriersConfiguration.Add("300mm, 25", "idCarrierId4", SampleDimension.S300mm, 25);
                }
                finally
                {
                    if (fileStream != null)
                        fileStream.Close();
                }
            }
            else
            {
                carriersConfiguration = new CarrierConfigurations();
                carriersConfiguration.Add("200mm, 13", "idCarrierId1", SampleDimension.S200mm, 13);
                carriersConfiguration.Add("200mm, 25", "idCarrierId2", SampleDimension.S200mm, 25);
                carriersConfiguration.Add("300mm, 13", "idCarrierId3", SampleDimension.S300mm, 13);
                carriersConfiguration.Add("300mm, 25", "idCarrierId4", SampleDimension.S300mm, 25);
            }
            return carriersConfiguration;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the simulator's dictionary of carrier
        /// </summary>
        public Dictionary<string, CarrierConfiguration> CarrierConfigs
        {
            get
            {
                return _carrierConfigList;
            }
            set
            {
                _carrierConfigList = value;
                if (CarrierConfigChanged != null)
                    CarrierConfigChanged(this, null);
            }
        }

        /// <summary>
        /// Gets the simulator's list of carrier name
        /// </summary>
        public List<string> CarrierConfigListNames => _carrierConfigList.Keys.ToList();

        /// <summary>
        /// Gets the simulator's list dimension who can be supported on the simulator
        /// </summary>
        public IList<string> ListDimension
        {
            get
            {
                IList<string> list = new List<string>();
                list.Add(SampleDimension.S100mm.ToString());
                list.Add(SampleDimension.S150mm.ToString());
                list.Add(SampleDimension.S200mm.ToString());
                list.Add(SampleDimension.S300mm.ToString());
                return list;
            }
        }

        /// <summary>
        /// Get a clone of the carrier defines by his ID
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public CarrierConfiguration Get(string configId)
        {
            return CarrierConfigs[configId].Clone() as CarrierConfiguration;
        }
        #endregion Properties

        #region Carrier Configuration
        /// <summary>
        /// Add a new carrier configuration
        /// </summary>
        /// <param name="configId"></param>
        /// <param name="defaultCarrierId"></param>
        /// <param name="carrierType"></param>
        /// <param name="slotNumber"></param>
        public void Add(string configId, string defaultCarrierId, SampleDimension carrierType, byte slotNumber)
        {
            if (_carrierConfigList.ContainsKey(configId))
                return;

            CarrierConfiguration cassette = new CarrierConfiguration(defaultCarrierId, slotNumber, carrierType);
            _carrierConfigList.Add(configId, cassette);
            CarrierConfigChanged?.Invoke(this, null);

        }
        /// <summary>
        ///  Remove a carrier configuration
        /// </summary>
        /// <param name="configId"></param>
        public void Remove(string configId)
        {
            _carrierConfigList.Remove(configId);
            CarrierConfigChanged?.Invoke(this, null);
        }
        /// <summary>
        /// Update carrier configuration
        /// </summary>
        /// <param name="configId"></param>
        /// <param name="updated"></param>
        public void Update(string configId, CarrierConfiguration updated)
        {
            CarrierConfigs[configId] = updated;
            CarrierConfigChanged?.Invoke(this, null);
        }
        /// <summary>
        /// Save all simulator's configurations
        /// </summary>
        public void Save()
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            string file = AppDomain.CurrentDomain.BaseDirectory + _simulatorSerializationPath;
            FileStream fileStream = new FileStream(file, FileMode.Create);
            binaryFormatter.Serialize(fileStream, this);
            fileStream.Close();
        }
        #endregion Carrier Configuration
    }
}
