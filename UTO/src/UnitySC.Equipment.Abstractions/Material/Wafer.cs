using System;

using UnitsNet;

using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Material;
using UnitySC.Shared.Data;
using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.Equipment.Abstractions.Material
{
    public class Wafer : Substrate
    {
        #region Constructor

        public Wafer(string name)
            : base(name)
        {
            GuidWafer = Guid.NewGuid();
            JobStartTime = DateTime.MinValue;
        }

        #endregion

        #region Properties

        private string _carrierId;
        public string CarrierId
        {
            get => _carrierId;
            set
            {
                _carrierId = value;
                OnPropertyChanged();
            }
        }

        private string _processJobId;
        public string ProcessJobId
        {
            get => _processJobId;
            set
            {
                _processJobId = value;
                OnPropertyChanged();
            }
        }

        private string _controlJobId;
        public string ControlJobId
        {
            get => _controlJobId;
            set
            {
                _controlJobId = value;
                OnPropertyChanged();
            }
        }

        private string _lotId;
        public string LotId
        {
            get => _lotId;
            set
            {
                _lotId = value;
                OnPropertyChanged();
            }
        }

        private Angle _orientationAngle;
        public Angle OrientationAngle
        {
            get => _orientationAngle;
            set
            {
                _orientationAngle = value;
                OnPropertyChanged();
            }
        }

        private string _substrateId;
        public string SubstrateId
        {
            get => _substrateId;
            set
            {
                _substrateId = value;
                OnPropertyChanged();
            }
        }

        private string _acquiredId = string.Empty;
        public string AcquiredId
        {
            get => _acquiredId;
            set
            {
                _acquiredId = value;
                OnPropertyChanged();
            }
        }

        private MaterialType _materialType;
        public MaterialType MaterialType
        {
            get => _materialType;
            set
            {
                _materialType = value;
                OnPropertyChanged();
            }
        }

        private UTOJobProgram _jobProgram;

        public UTOJobProgram JobProgram
        {
            get => _jobProgram;
            set
            {
                _jobProgram = value;
                OnPropertyChanged();
            }
        }

        private Guid _guidWafer;
        public Guid GuidWafer
        {
            get => _guidWafer;
            set
            {
                _guidWafer = value;
                OnPropertyChanged();
            } }

        private JobPosition _jobPosition;
        public JobPosition JobPosition
        {
            get => _jobPosition;
            set
            {
                _jobPosition = value;
                OnPropertyChanged();
            }
        }

        private DateTime _jobStartTime;
        public DateTime JobStartTime
        {
            get => _jobStartTime;
            set
            {
                _jobStartTime = value;
                OnPropertyChanged();
            }
        }

        private string _equipmentID = string.Empty;
        public string EquipmentID
        {
            get => _equipmentID;
            set
            {
                _equipmentID = value;
                OnPropertyChanged();
            }
        }

        private string _deviceID = string.Empty;
        public string DeviceID
        {
            get => _deviceID;
            set
            {
                _deviceID = value;
                OnPropertyChanged();
            }
        }

        public bool IsAligned { get; set; }

        public int ProcessModuleIndex { get; set; }

        #endregion
    }
}
