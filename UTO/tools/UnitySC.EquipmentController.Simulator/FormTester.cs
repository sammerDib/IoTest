using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;

using Agileo.Common.Tracing;
using Agileo.Drivers;
using Agileo.SemiDefinitions;

using UnitySC.EquipmentController.Simulator.Driver;
using UnitySC.EquipmentController.Simulator.EquipmentData;

using UnitsNet;

using UnitySC.EFEM.Controller.HostInterface;
using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.Equipment.Abstractions.Devices.Efem.Enums;

using LightState = Agileo.SemiDefinitions.LightState;
using UnitySC.Shared.Tools.Units;

using Angle = UnitsNet.Angle;

// ReSharper disable LocalizableElement
// ReSharper disable UnusedParameter.Local // TODO Remove when all commands would be implemented

namespace UnitySC.EquipmentController.Simulator
{
    public partial class FormTester : Form, IListener
    {
        #region Fields

        private const string Separator = "\r\n\r\n----------------------------------------\r\n\r\n";

        private readonly Dictionary<string, Color> _lpLightActiveColor;

        private Timer _refreshStatusesTimer;

        private EfemDriver _efemDriver;

        private readonly EfemData _efemData = new EfemData();

        private LoadPortData _selectedLoadPortData;

        #endregion Fields

        public FormTester()
        {
            InitializeComponent();

            Text += " - Rev. " + Application.ProductVersion;

            // Setup TraceManager
            TraceManager.Instance().Setup(new TracingConfig(), new DataLogFiltersConfig());
            TraceManager.Instance().RegisterTracer(nameof(EfemDriver));
            TraceManager.Instance().AddListener(this);

            // Initialize timer responsible to refresh all statuses
            _refreshStatusesTimer          = new Timer();
            _refreshStatusesTimer.Interval = 10;
            _refreshStatusesTimer.Tick    += RefreshStatusesTimerTick;

            _lpLightActiveColor = new Dictionary<string, Color>();
        }

        #region HMI

        private void ShowError(Exception ex)
        {
            if (Created == false || Disposing) { return; }

            Invoke(
                new Action<Exception>(
                    delegate(Exception exception)
                    {
                        tbLog.Text = DateTime.Now.ToString("HH:mm:ss:fff\t", CultureInfo.InvariantCulture)
                                     + "An error has occurred - "
                                     + exception.GetType().Name
                                     + "\r\n"
                                     + exception.Message
                                     + Separator
                                     + tbLog.Text;
                        MessageBox.Show(this, "An error has occurred.\r\n\r\n" + exception, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }),
                ex.InnerException ?? ex);
        }

        private void AppendMessage(
            string message,
            [CallerMemberName] string source = "",
            params object[] additionalData)
        {
            if (Created == false || Disposing == true) { return; }

            string separator = additionalData.Length <= 1
                ? string.Empty
                : @"
[Details]
";
            string datas = string.Join(", ",
                Array.ConvertAll(additionalData, new Converter<object, string>(data => { return data.ToString(); })));

            var line = string.Format(CultureInfo.InvariantCulture, "{0,-12} {1, -15}  {2}{3}", source, message,
                separator, datas);

            Invoke(new Action<string>(delegate(string msg) { tbLog.AppendText(msg + Environment.NewLine); }), line);
        }

        private static Color ConvertToColor(LocationSenseType isMaterialOn)
        {
            if (isMaterialOn == LocationSenseType.Yes) { return Color.Lime; }
            else { return SystemColors.Control; }
        }

        private static Color ConvertToColor(AlignerStatus alignerStatus)
        {
            DeviceState state;

            switch (alignerStatus)
            {
                case AlignerStatus.Idle:
                    state = DeviceState.Idle;
                    break;
                case AlignerStatus.Error:
                    state = DeviceState.Engineering;
                    break;
                case AlignerStatus.Unknown:
                    state = DeviceState.Initializing;
                    break;
                case AlignerStatus.Moving:
                    state = DeviceState.Executing;
                    break;
                case AlignerStatus.Disable:
                    state = DeviceState.NotRun;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(alignerStatus), alignerStatus, null);
            }

            return ConvertToColor(state);
        }

        private static Color ConvertToColor(LoadPortStatus loadPortStatus)
        {
            DeviceState state;
            
            switch (loadPortStatus)
            {
                case LoadPortStatus.Idle:
                case LoadPortStatus.Loaded:
                case LoadPortStatus.DoorClose:
                    state = DeviceState.Idle;
                    break;
                case LoadPortStatus.Error:
                    state = DeviceState.Engineering;
                    break;
                case LoadPortStatus.Unknown:
                    state = DeviceState.Initializing;
                    break;
                case LoadPortStatus.Moving:
                    state = DeviceState.Executing;
                    break;
                case LoadPortStatus.Disable:
                    state = DeviceState.NotRun;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(loadPortStatus), loadPortStatus, null);
            }

            return ConvertToColor(state);
        }

        private static Color ConvertToColor(RobotStatus robotStatus)
        {
            DeviceState state;
            
            switch (robotStatus)
            {
                case RobotStatus.Idle:
                    state = DeviceState.Idle;
                    break;
                case RobotStatus.Error:
                    state = DeviceState.Engineering;
                    break;
                case RobotStatus.Unknown:
                    state = DeviceState.Initializing;
                    break;
                case RobotStatus.Moving:
                    state = DeviceState.Executing;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(robotStatus), robotStatus, null);
            }

            return ConvertToColor(state);
        }

        private static Color ConvertToColor(DeviceState state)
        {
            switch (state)
            {
                case DeviceState.Engineering:
                    return Color.Red;

                case DeviceState.Initializing:
                    return Color.Goldenrod;

                case DeviceState.Executing:
                    return Color.Blue;

                case DeviceState.Idle:
                    return Color.Lime;

                case DeviceState.NotRun:
                    return Color.Red;

                default:
                    return SystemColors.Control;
            }
        }

        private static Color ConvertToColor(bool condition)
        {
            if (condition) { return Color.Lime; }
            else { return SystemColors.Control; }
        }

        private void FillComboBoxes()
        {
            // TODO
        }

        private void UpdateAvailableSlots(ComboBox cbLocation, ref ComboBox cbSlot)
        {
            cbSlot.Items.Clear();

            if (cbLocation.SelectedItem == null || string.IsNullOrEmpty(cbLocation.SelectedItem.ToString()))
                return;

            if (!Enum.TryParse(cbLocation.SelectedItem.ToString(), out Constants.Stage designatedStage))
                return;

            // If the selected stage is a LoadPort
            if (designatedStage >= Constants.Stage.LP1 && designatedStage <= Constants.Stage.LP4)
            {
                var loadPortData = _efemData.LoadPortsData[Constants.ToPort(designatedStage)];

                // TODO: to be used while not possible to have at the same time an emulated LP and an emulated robot.
                if (loadPortData.MappingData == null || loadPortData.MappingData.Count == 0)
                {
                    for (var slotId = 1; slotId <= 25; ++slotId)
                    {
                        cbSlot.Items.Add(slotId);
                    }
                }
                else
                {
                    for (var slotId = 0; slotId < loadPortData.MappingData.Count; ++slotId)
                    {
                        cbSlot.Items.Add(slotId + 1);
                    }
                }
            }
            else
            {
                // Stage Tilt or TiltPort (e.g.: Aligner).
                cbSlot.Items.Add(1);
            }
        }

        private void RefreshStatusesTimerTick(object sender, EventArgs e)
        {
            RefreshStatuses();
        }

        private void RefreshStatuses()
        {
            if (_efemDriver.IsCommunicationEnabled)
            {
                tbEFEMDeviceState.Text      = "Connected";
                tbEFEMDeviceState.BackColor = Color.Lime;
            }
            else
            {
                tbEFEMDeviceState.Text      = "Not connected";
                tbEFEMDeviceState.BackColor = Color.Red;
            }

            tbEFEMPressure.Text = _efemData.Pressure + " mPa";

            // Aligner statuses
            tbAlignerWaferPresence.BackColor = ConvertToColor(_efemData.AlignerData.IsAlignerCarrierPresent);
            tbAlignerDeviceState.BackColor   = ConvertToColor(_efemData.AlignerData.AlignerStatus);
            tbAlignerDeviceState.Text        = _efemData.AlignerData.AlignerStatus.ToString();
            tbOCRWaferIDFrontSide.Text       = _efemData.AlignerData.WaferIdFrontSide;
            tbOCRWaferIDBackSide.Text        = _efemData.AlignerData.WaferIdBackSide;

            if (_selectedLoadPortData != null)
            {
                // Update LP General statuses
                tbLPDeviceState.BackColor = ConvertToColor(_selectedLoadPortData.LoadPortStatus);
                tbLPDeviceState.Text      = _selectedLoadPortData.LoadPortStatus.ToString();

                // Update Carrier presence statuses
                tbLPPresence.BackColor  = ConvertToColor(_selectedLoadPortData.IsCarrierPresent);
                tbLPPlacement.BackColor = ConvertToColor(_selectedLoadPortData.IsCarrierCorrectlyPlaced);
                tbLPInAccess.BackColor  = ConvertToColor(_selectedLoadPortData.IsHandOffBtnPressed);
                tbLPPresence.Text       = _selectedLoadPortData.IsCarrierPresent ? "Present" : "Absent";
                tbLPPlacement.Text      = _selectedLoadPortData.IsCarrierCorrectlyPlaced ? "Placed" : "Invalid placement";
                tbLPInAccess.Text       = _selectedLoadPortData.IsHandOffBtnPressed ? "HandOff pressed" : "HandOff not pressed";
                tbLPWaferSize.Text      = _selectedLoadPortData.WaferSize == 0 ? "Unknown Dimension" : _selectedLoadPortData.WaferSize.ToString() + " Inch";
                tbLPCarrierID.Text      = _selectedLoadPortData.CarrierID;
                tbLPCarrierType.Text = _selectedLoadPortData.CarrierType.ToString();

                // Update mapping data
                if (_selectedLoadPortData.MappingData?.Count > 0)
                {
                    // Crate a local copy of the list and reverse slot states list to have the slot list from the upper to the lower.
                    var reversedSlotStatesList = _selectedLoadPortData.MappingData.ToList();
                    reversedSlotStatesList.Reverse();

                    string mappingAsText = string.Empty;
                    reversedSlotStatesList.ForEach(slotState => mappingAsText += slotState.ToString() + Environment.NewLine);

                    // Replace mapping text only if needed
                    if (!mappingAsText.Equals(tbLPMapping.Text))
                    {
                        tbLPMapping.Text = mappingAsText;

                        if (cbRobotSourceLocation.SelectedItem != null
                            && cbLLLoadPorts.SelectedItem.ToString().Equals(cbRobotSourceLocation.SelectedItem.ToString()))
                        {
                            UpdateAvailableSlots(cbRobotSourceLocation, ref cbRobotSourceSlot);
                        }

                        if (cbRobotDestinationLocation.SelectedItem != null
                            && cbLLLoadPorts.SelectedItem.ToString().Equals(cbRobotDestinationLocation.SelectedItem.ToString()))
                        {
                            UpdateAvailableSlots(cbRobotDestinationLocation, ref cbRobotDestinationSlot);
                        }
                    }
                }
                else
                {
                    tbLPMapping.ResetText();
                }
            }
            else
            {
                tbLPMapping.ResetText();
            }

            // Robot statuses
            tbRobotDeviceState.BackColor           = ConvertToColor(_efemData.RobotData.RobotStatus);
            tbRobotDeviceState.Text                = _efemData.RobotData.RobotStatus.ToString();
            tbRobotSpeed.Text                      = _efemData.RobotData.RobotSpeed.ToString();
            tbRobotUpperArmWaferPresence.BackColor = ConvertToColor(_efemData.RobotData.IsPresentOnUpperArm);
            tbRobotLowerArmWaferPresence.BackColor = ConvertToColor(_efemData.RobotData.IsPresentOnLowerArm);
            tbRobotUpperArmWaferPresence.Text      = "LastPos=" + _efemData.RobotData.StageUpperArm.ToString() 
                                                           + ':' + _efemData.RobotData.SlotUpperArm.ToString("00");
            tbRobotLowerArmWaferPresence.Text      = "LastPos=" + _efemData.RobotData.StageLowerArm.ToString() 
                                                           + ':' + _efemData.RobotData.SlotLowerArm.ToString("00");

            // Refresh OCR recipes only if needed
            if ((cbOcrFrontSideRecipes.Items.Count == 0 || cbOcrBackSideRecipes.Items.Count == 0) && _efemData.OcrData.ReceivedSide.HasValue)
            {
                cbOcrWaferSide_SelectedIndexChanged(cbOcrWaferSide, EventArgs.Empty);
                _efemData.OcrData.ReceivedSide = null;
            }

            // FFU
            nudFfuSpeed.Value = _efemData.FfuSpeed;
        }

        private void UpdateTextBoxAccordingStatus(TextBox textBox, DeviceState state)
        {
            textBox.Text = state.ToString();

            switch (state)
            {
                case DeviceState.Engineering:
                    textBox.BackColor = Color.Red;
                    break;
                case DeviceState.Initializing:
                    textBox.BackColor = Color.Goldenrod;
                    break;
                case DeviceState.Executing:
                    textBox.BackColor = Color.Blue;
                    break;
                case DeviceState.Idle:
                    textBox.BackColor = Color.Lime;
                    break;
                case DeviceState.NotRun:
                    textBox.BackColor = Color.Red;
                    break;
                default:
                    textBox.BackColor = SystemColors.Control;
                    break;
            }
        }

        /// <summary>
        /// Updates the HMI by hiding controls which do not match current configuration.
        /// </summary>
        private void UpdateHmi()
        {
            // Fill combo boxes
            FillComboBoxes();
        }

        /// <summary>
        /// Sets Text, BackColor, ForeColor properties of the specified TextBox depending on current LightState.
        /// </summary>
        /// <param name="textBox">The TextBox to update.</param>
        /// <param name="color">The color to set in TextBox.BackColor property if the specified state is On or Flashing.</param>
        /// <param name="state">The current hardware IO state.</param>
        /// <param name="text">The text to set in TextBox.Text property.</param>
        private void SetTowerLightStatus(TextBox textBox, Color color, LightState state, string text)
        {
            switch (state)
            {
                case LightState.On:
                case LightState.Flashing:
                case LightState.FlashingSlow:
                case LightState.FlashingFast:
                    textBox.BackColor = color;
                    if (textBox == tbBlueLightStatus)
                    {
                        textBox.ForeColor = Color.White;
                    }

                    break;
                default:
                    textBox.BackColor = SystemColors.Control;
                    textBox.ForeColor = Color.Black;
                    break;
            }

            textBox.Text = string.Format(CultureInfo.InvariantCulture, "{0} [{1}]", text, state);
        }

        /// <summary>
        /// Sets Text and BackColor properties of the specified TextBox depending on current LightState.
        /// </summary>
        /// <param name="textBox">The TextBox to update.</param>
        /// <param name="state">The current hardware IO state.</param>
        /// <param name="text">The text to set in TextBox.Text property.</param>
        private void SetLoadPortLightStatus(TextBox textBox, LightState state, string text)
        {
            switch (state)
            {
                case LightState.On:
                case LightState.Flashing:
                case LightState.FlashingSlow:
                case LightState.FlashingFast:
                    textBox.BackColor = _lpLightActiveColor[textBox.Name];
                    break;
                default:
                    textBox.BackColor = SystemColors.Control;
                    break;
            }

            textBox.Text = string.Format(CultureInfo.InvariantCulture, "{0} [{1}]", text, state);
        }

        #endregion HMI

        #region Equipment Event Handlers

        private void EFEMFacade_CommandStarted(object sender, CommandEventArgs e)
        {
            string details;
            if (e.Status == CommandStatusCode.Ok)
            {
                details = string.Format(CultureInfo.InvariantCulture, "Status = OK");
            }
            else
            {
                details = e.ToString();
            }

            AppendMessage(string.Format(CultureInfo.InvariantCulture, "Activity {0} Started.", e.Name),
                nameof(EfemDriver), details);
        }

        private void EFEMFacade_CommandDone(object sender, CommandEventArgs e)
        {
            string details;
            if (e.Status == CommandStatusCode.Ok)
            {
                details = string.Format(CultureInfo.InvariantCulture, "Status = OK");
            }
            else
            {
                details = e.ToString();
            }

            AppendMessage(string.Format(CultureInfo.InvariantCulture, "Activity {0} Done.", e.Name), nameof(EfemDriver),
                details);
        }

        private void EfemDriverMessageExchanged(object sender, MessageExchangedEventArgs e)
        {
            Invoke(new Action(delegate
            {
                var msg = $"{e.DateTime:yyyy-MM-dd HH:mm:ss.fff}\t{(e.IsOut ? "SEND\t" : "RECEIVED")}\t{e.Message}";
                tbComLogs.AppendText(msg + Environment.NewLine);
            }));
        }

        #endregion Equipment Event Handlers

        #region Start

        private void Create_Click(object sender, EventArgs e)
        {
            try
            {
                _efemDriver = new EfemDriver(tbIPAddress.Text, Convert.ToInt32(tbPort.Text), TimeSpan.FromSeconds(5), _efemData);
                _efemDriver.MessageExchanged += EfemDriverMessageExchanged;

                tbIPAddress.Enabled = false;
                tbPort.Enabled = false;

                // Init LoadPorts elements
                for (var unit = Constants.Port.LP1; unit <= Constants.Port.LP4; unit++)
                {
                    cbLLLoadPorts.Items.Add(unit.ToString());
                    cbIOLoadPorts.Items.Add(unit.ToString());
                    cbLoadPorts.Items.Add(unit.ToString());
                }

                // Init Robot elements
                for (var i = 0; i <= 9; ++i)
                    cbSelectRobotSpeed.Items.Add(i.ToString());
                for (var i = 'A'; i <= 'J'; ++i)
                    cbSelectRobotSpeed.Items.Add(i.ToString());
                foreach (var stage in Enum.GetValues(typeof(Constants.Stage)).Cast<Constants.Stage>())
                {
                    cbRobotSourceLocation.Items.Add(stage.ToString());
                }
                UpdateAvailableSlots(cbRobotSourceLocation, ref cbRobotSourceSlot);
                
                cbRobotArm.Items.Add(Constants.Arm.Lower.ToString());
                cbRobotArm.Items.Add(Constants.Arm.Upper.ToString());

                // Init OCR elements
                foreach (var side in Enum.GetValues(typeof(SubstrateSide)).Cast<SubstrateSide>())
                {
                    cbOcrWaferSide.Items.Add(side);
                }

                cbOcrWaferSide.SelectedItem = SubstrateSide.Front;

                foreach (var type in Enum.GetValues(typeof(SubstrateTypeRDID)).Cast<SubstrateTypeRDID>())
                {
                    cbOcrWaferType.Items.Add(type);
                }

                cbOcrWaferType.SelectedItem = SubstrateTypeRDID.NormalWafer;

                RefreshStatuses();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void bDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                _refreshStatusesTimer.Stop();
                _efemDriver.Disconnect();
                RefreshStatuses();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void bConnect_Click(object sender, EventArgs e)
        {
            try
            {
                _efemDriver.EnableCommunications();
                _refreshStatusesTimer.Start();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        #endregion Start

        #region Control

        private void Clear_Click(object sender, EventArgs e)
        {
            tbLog.Text = string.Empty;
        }

        private void ClearComLogs_Click(object sender, EventArgs e)
        {
            tbComLogs.Text = string.Empty;
        }

        private void bClearPostman_Click(object sender, EventArgs e)
        {
            tbPostman.Text = string.Empty;
        }

        #region Aligner

        private void AlignerInit_Click(object sender, EventArgs e)
        {
            if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.Initialize(Constants.Unit.Aligner);
            });
        }

        private void bChuckOnAligner_Click(object sender, EventArgs e)
        {
            if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.ChuckOnAligner();
            });
        }

        private void bChuckOffAligner_Click(object sender, EventArgs e)
        {
            if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.ChuckOffAligner();
            });
        }

        private void AlignerAlign_Click(object sender, EventArgs e)
        {
            if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                if (Angle.TryParse(tbAlignerAlignAngle.Text + Angle.GetAbbreviation(UnitsNet.Units.AngleUnit.Degree), out Angle angle))
                {
                    _efemDriver.Align(angle);
                }
                else
                {
                    throw new FormatException($"Align angle '{tbAlignerAlignAngle.Text}' not supported by Align command. "
                                              + $"Value of type double is required.");
                }
            });
        }

        private void AlignerCentering_Click(object sender, EventArgs e)
        {
            if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() => { _efemDriver.Centering(); });
        }

        #endregion Aligner

        #region Robot

        private void RobotInit_Click(object sender, EventArgs e)
        {
            if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.Initialize(Constants.Unit.Robot);
            });
        }

        private void RobotHome_Click(object sender, EventArgs e)
        {
            if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.Home();
            });
        }

        private void RobotPreparePick_Click(object sender, EventArgs e)
        {
            if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            if (cbRobotSourceLocation.SelectedItem == null)
            {
                AppendMessage("Please select a location before preparing a pick.");
                return;
            }

            if (!Enum.TryParse(cbRobotSourceLocation.SelectedItem.ToString(), out Constants.Stage stage))
            {
                AppendMessage($"Unable to recognize the selected source location: \"{cbRobotSourceLocation.SelectedItem}\".");
                return;
            }

            if (cbRobotSourceSlot.SelectedItem == null)
            {
                AppendMessage("Please select a slot before preparing a pick.");
                return;
            }

            if (!uint.TryParse(cbRobotSourceSlot.SelectedItem.ToString(), out uint slot))
            {
                AppendMessage($"Unable to recognize the selected source slot: \"{cbRobotSourceSlot.SelectedItem}\".");
                return;
            }

            if (cbRobotArm.SelectedItem == null)
            {
                AppendMessage("Please select an arm before preparing a pick.");
                return;
            }

            if (!Enum.TryParse(cbRobotArm.SelectedItem.ToString(), out Constants.Arm arm))
            {
                AppendMessage($"Unable to recognize the selected arm: \"{cbRobotArm.SelectedItem}\".");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.PreparePick(arm, stage, slot);
            });
        }

        private void bPreparePlace_Click(object sender, EventArgs e)
        {
            if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            if (cbRobotSourceLocation.SelectedItem == null)
            {
                AppendMessage("Please select a location before preparing a place.");
                return;
            }

            if (!Enum.TryParse(cbRobotSourceLocation.SelectedItem.ToString(), out Constants.Stage stage))
            {
                AppendMessage($"Unable to recognize the selected source location: \"{cbRobotSourceLocation.SelectedItem}\".");
                return;
            }

            if (cbRobotSourceSlot.SelectedItem == null)
            {
                AppendMessage("Please select a slot before preparing a place.");
                return;
            }

            if (!uint.TryParse(cbRobotSourceSlot.SelectedItem.ToString(), out uint slot))
            {
                AppendMessage($"Unable to recognize the selected source slot: \"{cbRobotSourceSlot.SelectedItem}\".");
                return;
            }

            if (cbRobotArm.SelectedItem == null)
            {
                AppendMessage("Please select an arm before preparing a place.");
                return;
            }

            if (!Enum.TryParse(cbRobotArm.SelectedItem.ToString(), out Constants.Arm arm))
            {
                AppendMessage($"Unable to recognize the selected arm: \"{cbRobotArm.SelectedItem}\".");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.PreparePlace(arm, stage, slot);
            });
        }

        private void RobotPick_Click(object sender, EventArgs e)
        {
            if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            if (cbRobotSourceLocation.SelectedItem == null)
            {
                AppendMessage("Please select a location before picking.");
                return;
            }

            if (!Enum.TryParse(cbRobotSourceLocation.SelectedItem.ToString(), out Constants.Stage stage))
            {
                AppendMessage($"Unable to recognize the selected source location: \"{cbRobotSourceLocation.SelectedItem}\".");
                return;
            }

            if (cbRobotSourceSlot.SelectedItem == null)
            {
                AppendMessage("Please select a slot before picking.");
                return;
            }

            if (!uint.TryParse(cbRobotSourceSlot.SelectedItem.ToString(), out uint slot))
            {
                AppendMessage($"Unable to recognize the selected source slot: \"{cbRobotSourceSlot.SelectedItem}\".");
                return;
            }

            if (cbRobotArm.SelectedItem == null)
            {
                AppendMessage("Please select an arm before picking.");
                return;
            }

            if (!Enum.TryParse(cbRobotArm.SelectedItem.ToString(), out Constants.Arm arm))
            {
                AppendMessage($"Unable to recognize the selected arm: \"{cbRobotArm.SelectedItem}\".");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.Pick(arm, stage, slot);
            });
        }

        private void RobotPlace_Click(object sender, EventArgs e)
        {
            if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            if (cbRobotSourceLocation.SelectedItem == null)
            {
                AppendMessage("Please select a location before unloading a wafer.");
                return;
            }

            if (!Enum.TryParse(cbRobotSourceLocation.SelectedItem.ToString(), out Constants.Stage stage))
            {
                AppendMessage($"Unable to recognize the selected source location: \"{cbRobotSourceLocation.SelectedItem}\".");
                return;
            }

            if (cbRobotSourceSlot.SelectedItem == null)
            {
                AppendMessage("Please select a slot before unloading a wafer.");
                return;
            }

            if (!uint.TryParse(cbRobotSourceSlot.SelectedItem.ToString(), out uint slot))
            {
                AppendMessage($"Unable to recognize the selected source slot: \"{cbRobotSourceSlot.SelectedItem}\".");
                return;
            }

            if (cbRobotArm.SelectedItem == null)
            {
                AppendMessage("Please select an arm before unloading a wafer.");
                return;
            }

            if (!Enum.TryParse(cbRobotArm.SelectedItem.ToString(), out Constants.Arm arm))
            {
                AppendMessage($"Unable to recognize the selected arm: \"{cbRobotArm.SelectedItem}\".");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.Place(arm, stage, slot);
            });
        }

        private void bClampWaferOnArm_Click(object sender, EventArgs e)
        {
            if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            if (cbRobotArm.SelectedItem == null)
            {
                AppendMessage("Please select an arm before clamping a wafer on arm.");
                return;
            }

            if (!Enum.TryParse(cbRobotArm.SelectedItem.ToString(), out Constants.Arm arm))
            {
                AppendMessage($"Unable to recognize the selected arm: \"{cbRobotArm.SelectedItem}\".");
                return;
            }


            TryCatch(() =>
            {
                _efemDriver.Clamp(arm);
            });
        }

        private void bUnclampWaferOnArm_Click(object sender, EventArgs e)
        {
            if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            if (cbRobotArm.SelectedItem == null)
            {
                AppendMessage("Please select an arm before unclamping a wafer on arm.");
                return;
            }

            if (!Enum.TryParse(cbRobotArm.SelectedItem.ToString(), out Constants.Arm arm))
            {
                AppendMessage($"Unable to recognize the selected arm: \"{cbRobotArm.SelectedItem}\".");
                return;
            }


            TryCatch(() =>
            {
                _efemDriver.Unclamp(arm);
            });
        }

        private void RobotTransfer_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                throw new NotImplementedException();
            });
        }

        private void bSetRobotSpeed_Click(object sender, EventArgs e)
        {
            if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            if (cbSelectRobotSpeed.SelectedItem == null || string.IsNullOrEmpty(cbSelectRobotSpeed.SelectedItem.ToString()))
            {
                AppendMessage("Please select a speed in combobox before to send new speed to robot.");
                return;
            }

            var spd = cbSelectRobotSpeed.SelectedItem.ToString();

            if (string.IsNullOrEmpty(spd))
            {
                AppendMessage("Please select a speed in combobox before to send new speed to robot.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.SetRobotSpeed(spd[0]);
            });
        }

        private void bGetWaferPresenceOnArm_Click(object sender, EventArgs e)
        {
            if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.GetWaferPresenceOnArm();
            });
        }

        private void RobotSetSettings_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                throw new NotImplementedException();
            });
        }

        private void RobotGetSettings_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                throw new NotImplementedException();
            });
        }

        private void RobotSourceLocation_SelectedValueChanged(object sender, EventArgs e)
        {
            UpdateAvailableSlots(cbRobotSourceLocation, ref cbRobotSourceSlot);
        }

        private void RobotDestinationLocation_SelectedValueChanged(object sender, EventArgs e)
        {
            UpdateAvailableSlots(cbRobotDestinationLocation, ref cbRobotDestinationSlot);
        }

        #endregion Robot

        #region LP

        private void LPInit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbLLLoadPorts.Text))
            {
                AppendMessage("Please select a LoadPort first.");
                return;
            }
            else if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.Initialize((Constants.Unit)Enum.Parse(typeof(Constants.Unit),
                    cbLLLoadPorts.Text));
            });
        }

        private void LPClamp_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                throw new NotImplementedException();
            });
        }

        private void LPDock_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbLLLoadPorts.Text))
            {
                AppendMessage("Please select a LoadPort first.");
                return;
            }
            else if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.Dock((Constants.Port)Enum.Parse(typeof(Constants.Port), cbLLLoadPorts.Text));
            });
        }

        private void LPUndock_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbLLLoadPorts.Text))
            {
                AppendMessage("Please select a LoadPort first.");
                return;
            }
            else if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.Undock((Constants.Port)Enum.Parse(typeof(Constants.Port), cbLLLoadPorts.Text));
            });
        }

        private void bLPGetSize_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbLLLoadPorts.Text))
            {
                AppendMessage("Please select a LoadPort first.");
                return;
            }
            else if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.GetWaferSize((Constants.Port)Enum.Parse(typeof(Constants.Port), cbLLLoadPorts.Text));
            });
        }

        private void bLPGetCarrierType_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbLLLoadPorts.Text))
            {
                AppendMessage("Please select a LoadPort first.");
                return;
            }
            else if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.GetCarrierType((Constants.Port)Enum.Parse(typeof(Constants.Port), cbLLLoadPorts.Text));
            });
        }

        private void bLPUnclamp_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbLLLoadPorts.Text))
            {
                AppendMessage("Please select a LoadPort first.");
                return;
            }
            else if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.ClampLp((Constants.Port)Enum.Parse(typeof(Constants.Port), cbLLLoadPorts.Text),true);
            });
        }

        private void bLPClamp_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbLLLoadPorts.Text))
            {
                AppendMessage("Please select a LoadPort first.");
                return;
            }
            else if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.ClampLp((Constants.Port)Enum.Parse(typeof(Constants.Port), cbLLLoadPorts.Text), false);
            });
        }

        private void LPReadCarrierID_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbLLLoadPorts.Text))
            {
                AppendMessage("Please select a LoadPort first.");
                return;
            }
            else if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.ReadCarrierID((Constants.Port)Enum.Parse(typeof(Constants.Port), cbLLLoadPorts.Text));
            });
        }

        private void LPMoveToReadPosition_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                throw new NotImplementedException();
            });
        }

        private void LPMoveToWritePosition_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                throw new NotImplementedException();
            });
        }

        private void LPOpen_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                throw new NotImplementedException();
            });
        }

        private void LPInAccess_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbLLLoadPorts.Text))
            {
                AppendMessage("Please select a LoadPort first.");
                return;
            }
            else if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.Dock((Constants.Port)Enum.Parse(typeof(Constants.Port), cbLLLoadPorts.Text));
            });
        }

        private void LPMap_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbLLLoadPorts.Text))
            {
                AppendMessage("Please select a LoadPort first.");
                return;
            }
            else if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.PerformWaferMapping((Constants.Port)Enum.Parse(typeof(Constants.Port), cbLLLoadPorts.Text));
            });
        }

        private void LPLastMapping_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbLLLoadPorts.Text))
            {
                AppendMessage("Please select a LoadPort first.");
                return;
            }
            else if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.GetMappingPattern((Constants.Port)Enum.Parse(typeof(Constants.Port), cbLLLoadPorts.Text));
            });
        }

        private void LPClose_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbLLLoadPorts.Text))
            {
                AppendMessage("Please select a LoadPort first.");
                return;
            }
            else if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.CloseDoor((Constants.Port)Enum.Parse(typeof(Constants.Port), cbLLLoadPorts.Text));
                
            });
        }

        private void LPRelease_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbLLLoadPorts.Text))
            {
                AppendMessage("Please select a LoadPort first.");
                return;
            }
            else if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.Undock((Constants.Port)Enum.Parse(typeof(Constants.Port), cbLLLoadPorts.Text));
            });
        }

        private void CarrierIDEnter_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                throw new NotImplementedException();
            });
        }

        private void bEnableE84_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(cbLLLoadPorts.Text))
            {
                AppendMessage("Please select a LoadPort first.");
                return;
            }
            else if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.EnableE84((Constants.Port)Enum.Parse(typeof(Constants.Port), cbLLLoadPorts.Text));
            });
        }

        private void bDisableE84_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbLLLoadPorts.Text))
            {
                AppendMessage("Please select a LoadPort first.");
                return;
            }
            else if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.DisableE84((Constants.Port)Enum.Parse(typeof(Constants.Port), cbLLLoadPorts.Text));
            });
        }

        private void bResetE84_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbLLLoadPorts.Text))
            {
                AppendMessage("Please select a LoadPort first.");
                return;
            }
            else if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.ResetE84((Constants.Port)Enum.Parse(typeof(Constants.Port), cbLLLoadPorts.Text));
            });
        }

        private void bAbortE84_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbLLLoadPorts.Text))
            {
                AppendMessage("Please select a LoadPort first.");
                return;
            }
            else if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.AbortE84((Constants.Port)Enum.Parse(typeof(Constants.Port), cbLLLoadPorts.Text));
            });
        }

        private void GetE84Outputs_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbLLLoadPorts.Text))
            {
                AppendMessage("Please select a LoadPort first.");
                return;
            }
            else if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.GetE84OutputSignals((Constants.Port)Enum.Parse(typeof(Constants.Port), cbLLLoadPorts.Text));
            });
        }

        private void E84Unload_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                throw new NotImplementedException();
            });
        }

        private void E84Stop_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                throw new NotImplementedException();
            });
        }

        private void GetE84Inputs_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbLLLoadPorts.Text))
            {
                AppendMessage("Please select a LoadPort first.");
                return;
            }
            else if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.GetE84InputSignals((Constants.Port)Enum.Parse(typeof(Constants.Port), cbLLLoadPorts.Text));
            });
        }

        private void ClearErrors_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                throw new NotImplementedException();
            });
        }

        #endregion LP

        #region EFEM

        private void EfemInitialization_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                _efemDriver.Initialize();
            });
        }

        private void EmergencyStop_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                throw new NotImplementedException();
            });
        }

        private void EfemStop_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                throw new NotImplementedException();
            });
        }

        private void EfemPause_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                throw new NotImplementedException();
            });
        }

        private void EfemResume_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                throw new NotImplementedException();
            });
        }

        private void EfemGetSize_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                throw new NotImplementedException();
            });
        }

        private void EfemSetSize_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                throw new NotImplementedException();
            });
        }

        private void bEfemGetStat_Click(object sender, EventArgs e)
        {
            if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.GetStat();
            });
        }

        private void bEfemGetPressure_Click(object sender, EventArgs e)
        {
            if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.GetPressure();
            });
        }

        private void bEfemSetTime_Click(object sender, EventArgs e)
        {
            if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.SetTime(tbEFEMTime.Text);
            });
        }
        #endregion EFEM

        #region ProcessModule

        private void PMWaferDeposit_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                throw new NotImplementedException();
            });
        }

        #endregion ProcessModule

        #region High Level Management

        private void HLInit_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                throw new NotImplementedException();
            });
        }

        private void HLLoadWaferMacro_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                throw new NotImplementedException();
            });
        }

        private void HLUnloadWaferMacro_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                throw new NotImplementedException();
            });
        }

        #endregion High Level Management

        #region OCR (substrate id reader)

        private void bOcrGetRecipes_Click(object sender, EventArgs e)
        {
            if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.GetRecipeNames((SubstrateSide)cbOcrWaferSide.SelectedItem);
            });
        }

        private void bOcrRead_Click(object sender, EventArgs e)
        {
            if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                var frontSideRecipe = cbOcrFrontSideRecipes.SelectedItem != null
                    ? GetRecipeName(cbOcrFrontSideRecipes.SelectedItem.ToString()) : string.Empty;

                var backSideRecipe = cbOcrBackSideRecipes.SelectedItem != null
                    ? GetRecipeName(cbOcrBackSideRecipes.SelectedItem.ToString()) : string.Empty;

                _efemDriver.ReadId((SubstrateSide)cbOcrWaferSide.SelectedItem, frontSideRecipe, backSideRecipe, (SubstrateTypeRDID)cbOcrWaferType.SelectedItem);
            });
        }

        private string GetRecipeName(string tabItem)
        {
            var separator = "] ";
            var nbCharToRemove = tabItem.IndexOf(separator);
            var recipeName = tabItem.Remove(0, nbCharToRemove + separator.Length);
            return recipeName;
        }
        #endregion OCR (substrate id reader)

        #region FFU

        private void bFfuStop_Click(object sender, EventArgs e)
        {
            if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.SetFfuRpm(0);
            });
        }

        private void bFfuSetRpm_Click(object sender, EventArgs e)
        {
            if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.SetFfuRpm((int)nudFfuSetPoint.Value);
            });
        }

        private void bFfuGetRpm_Click(object sender, EventArgs e)
        {
            if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.GetFfuRpm();
            });
        }

        #endregion FFU

        #endregion Control

        #region IOs

        #region Buzzer

        private void Buzzer_Click(object sender, EventArgs e)
        {
            BuzzerState state = BuzzerState.Undetermined;

            if (sender == bBuzzerOn)
            {
                state = BuzzerState.On;
            }
            else if (sender == bBuzzerOff)
            {
                state = BuzzerState.Off;
            }

            if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.SetBuzzer(state);
            });
        }

        #endregion Buzzer

        #region Tower Lights

        private void Light_Click(object sender, EventArgs e)
        {
            if (sender == bGreenLightOn)
            {
                tbGreenLightStatus.Text = "Green [ON]";
                _efemData.SignalTowerData.GreenLight = UnitySC.EFEM.Controller.HostInterface.Enums.LightState.On;
            }
            else if (sender == bGreenLightOff)
            {
                tbGreenLightStatus.Text = "Green [OFF]"; 
                _efemData.SignalTowerData.GreenLight = UnitySC.EFEM.Controller.HostInterface.Enums.LightState.Off;
            }
            else if (sender == bGreenLightBlink)
            {
                tbGreenLightStatus.Text = "Green [FLASHING]";
                _efemData.SignalTowerData.GreenLight = UnitySC.EFEM.Controller.HostInterface.Enums.LightState.Flashing;
            }

            if (sender == bBlueLightOn)
            {
                tbBlueLightStatus.Text = "Blue [ON]";
                _efemData.SignalTowerData.BlueLight = UnitySC.EFEM.Controller.HostInterface.Enums.LightState.On;
            }
            else if (sender == bBlueLightOff)
            {
                tbBlueLightStatus.Text = "Blue [OFF]";
                _efemData.SignalTowerData.BlueLight = UnitySC.EFEM.Controller.HostInterface.Enums.LightState.Off;
            }
            else if (sender == bBlueLightBlink)
            {
                tbBlueLightStatus.Text = "Blue [FLASHING]";
                _efemData.SignalTowerData.BlueLight = UnitySC.EFEM.Controller.HostInterface.Enums.LightState.Flashing;
            }

            if (sender == bOrangeLightOn)
            {
                tbOrangeLightStatus.Text = "Orange [ON]";
                _efemData.SignalTowerData.OrangeLight = UnitySC.EFEM.Controller.HostInterface.Enums.LightState.On;
            }
            else if (sender == bOrangeLightOff)
            {
                tbOrangeLightStatus.Text = "Orange [OFF]";
                _efemData.SignalTowerData.OrangeLight = UnitySC.EFEM.Controller.HostInterface.Enums.LightState.Off;
            }
            else if (sender == bOrangeLightBlink)
            {
                tbOrangeLightStatus.Text = "Orange [FLASHING]";
                _efemData.SignalTowerData.OrangeLight = UnitySC.EFEM.Controller.HostInterface.Enums.LightState.Flashing;
            }

            if (sender == bRedLightOn)
            {
                tbRedLightStatus.Text = "Red [ON]";
                _efemData.SignalTowerData.RedLight = UnitySC.EFEM.Controller.HostInterface.Enums.LightState.On;
            }
            else if (sender == bRedLightOff)
            {
                tbRedLightStatus.Text = "Red [OFF]";
                _efemData.SignalTowerData.RedLight = UnitySC.EFEM.Controller.HostInterface.Enums.LightState.Off;
            }
            else if (sender == bRedLightBlink)
            {
                tbRedLightStatus.Text = "Red [FLASHING]";
                _efemData.SignalTowerData.RedLight = UnitySC.EFEM.Controller.HostInterface.Enums.LightState.Flashing;
            }

            TryCatch(() =>
            {
                _efemDriver.SetLightTowerState(
                    _efemData.SignalTowerData.RedLight,
                    _efemData.SignalTowerData.OrangeLight,
                    _efemData.SignalTowerData.GreenLight,
                    _efemData.SignalTowerData.BlueLight);
            });
        }

        #endregion Tower Lights

        #region Load Port Indicators

        private void LoadLight_Click(object sender, EventArgs e)
        {
            _loadLightState = sender == bLoadLightOn;
            SetLpLight();
        }

        private void UnloadLight_Click(object sender, EventArgs e)
        {
            _unloadLightState = sender == bUnloadLightOn;
            SetLpLight();
        }

        private void ManualLight_Click(object sender, EventArgs e)
        {
            _manualLightState = sender == bManualLightOn;
            SetLpLight();
        }

        private void OpAccessLight_Click(object sender, EventArgs e)
        {
            //SetSignalOutput(LPIndicators.HandOff, (Button)sender);
        }

        private void AutoLight_Click(object sender, EventArgs e)
        {
            //SetSignalOutput(LPIndicators.Auto, (Button)sender);
        }

        private void ReserveLight_Click(object sender, EventArgs e)
        {
            //SetSignalOutput(LPIndicators.Reserve, (Button)sender);
        }

        private void AlarmLight_Click(object sender, EventArgs e)
        {
            //SetSignalOutput(LPIndicators.Alarm, (Button)sender);
        }

        #endregion Load Port Indicators

        #endregion IOs

        #region LoadPorts

        private bool _loadLightState;
        private bool _unloadLightState;
        private bool _manualLightState;

        private void bSetE84Timeout_Click(object sender, EventArgs e)
        {
            if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.SetE84Timeouts((int)nudTP1.Value, (int)nudTP2.Value, (int)nudTP3.Value, (int)nudTP4.Value, (int)nudTP5.Value);
            });
        }

        private void bSetCarrierType_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                _efemDriver.SetCarrierType(Constants.ToPort(GetSelectedLpIndex()), (uint)nudCarrierType.Value);
            });
        }

        private void LPManual_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                _efemDriver.DisableE84(Constants.ToPort(GetSelectedLpIndex()));
            });
        }

        private void LPAuto_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                _efemDriver.EnableE84(Constants.ToPort(GetSelectedLpIndex()));
            });
        }

        private void LPInService_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                throw new NotImplementedException();
            });
        }

        private void bLPE84ManualHandling_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                _efemDriver.DisableE84(Constants.ToPort(GetSelectedLpIndex()));
            });
        }

        private void bLPE84AutoHandling_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                _efemDriver.EnableE84(Constants.ToPort(GetSelectedLpIndex()));
            });
        }

        private void bLPE84Reset_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                _efemDriver.ResetE84(Constants.ToPort(GetSelectedLpIndex()));
            });
        }

        private void bLPE84Abort_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                _efemDriver.AbortE84(Constants.ToPort(GetSelectedLpIndex()));
            });
        }

        private void LPOutOfService_Click(object sender, EventArgs e)
        {
            TryCatch(() =>
            {
                throw new NotImplementedException();
            });
        }

        private void SetLpLight()
        {
            if (string.IsNullOrEmpty(cbIOLoadPorts.Text))
            {
                AppendMessage("Please select a LoadPort first.");
                return;
            }
            else if (!_efemDriver.IsCommunicationEnabled)
            {
                AppendMessage("Please connect to EFEM before trying to send a command.");
                return;
            }

            TryCatch(() =>
            {
                _efemDriver.SetLpLight((Constants.Port)Enum.Parse(typeof(Constants.Port), cbIOLoadPorts.Text), _loadLightState, _unloadLightState, _manualLightState);
            });
        }
        #endregion LoadPorts

        private void FormTester_Load(object sender, EventArgs e)
        {
            tbIPAddress.Text = EfemDriverConfig.Default.IpAddress;
            tbPort.Text      = EfemDriverConfig.Default.TcpPort.ToString();
        }

        private void FormTester_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                TraceManager.Instance().RemoveListener(this);

                Invoke(
                    new Action(delegate()
                    {
                        //if (EfemFacade.Instance == null)
                        //    return;

                        //EfemFacade.Stop();
                        //EfemFacade.Dispose();

                        if (_efemDriver != null)
                        {
                            _efemDriver.MessageExchanged -= EfemDriverMessageExchanged;
                            _efemDriver.Disconnect();

                            _efemDriver = null;
                        }
                    }),
                    null);
            }
            catch (Exception)
            {
                // ignored
            }

            // Close application created to load the default simulator interactions view
            if (System.Windows.Application.Current != null)
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void bSend_Click(object sender, EventArgs e)
        {
            if (sender == bSend)
            {
                _efemDriver.SendCustomMessage(tbMessageToSend.Text);
            }
            else if (sender == bSend2)
            {
                _efemDriver.SendCustomMessage(tbMessageToSend2.Text);
            }
            else if (sender == bSend3)
            {
                _efemDriver.SendCustomMessage(tbMessageToSend3.Text);
            }
        }

        #region CB Selection Event Handlers

        private void cbLLLoadPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            var typedSender = sender as ComboBox;
            if (typedSender != cbLLLoadPorts)
                return;

            try
            {
                var selectedValue = typedSender.SelectedItem.ToString();

                if (string.IsNullOrEmpty(selectedValue))
                {
                    _selectedLoadPortData = null;
                }
                else
                {
                    var selectedPort = (Constants.Port)Enum.Parse(typeof(Constants.Port), selectedValue);
                    _selectedLoadPortData = _efemData.LoadPortsData[selectedPort];
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void cbOcrWaferSide_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var side = (SubstrateSide)cbOcrWaferSide.SelectedItem;
                switch (side)
                {
                    case SubstrateSide.Front:
                        cbOcrFrontSideRecipes.Items.Clear();
                        foreach (var kvp in _efemData.OcrData.OcrRecipesFront)
                        {
                            cbOcrFrontSideRecipes.Items.Add($"[{kvp.Key:d2}] {kvp.Value}");
                        }

                        break;
                    case SubstrateSide.Back:
                        cbOcrBackSideRecipes.Items.Clear();
                        foreach (var kvp in _efemData.OcrData.OcrRecipesBack)
                        {
                            cbOcrBackSideRecipes.Items.Add($"[{kvp.Key:d2}] {kvp.Value}");
                        }

                        break;
                    case SubstrateSide.Both:
                        cbOcrFrontSideRecipes.Items.Clear();
                        cbOcrBackSideRecipes.Items.Clear();
                        foreach (var kvp in _efemData.OcrData.OcrRecipesFront)
                        {
                            cbOcrFrontSideRecipes.Items.Add($"[{kvp.Key:d2}] {kvp.Value}");
                        }

                        foreach (var kvp in _efemData.OcrData.OcrRecipesBack)
                        {
                            cbOcrBackSideRecipes.Items.Add($"[{kvp.Key:d2}] {kvp.Value}");
                        }
                        break;
                    case SubstrateSide.Frame:
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (cbOcrFrontSideRecipes.Items.Count > 0)
                {
                    cbOcrFrontSideRecipes.SelectedItem = cbOcrFrontSideRecipes.Items[0];
                }

                if (cbOcrBackSideRecipes.Items.Count > 0)
                {
                    cbOcrBackSideRecipes.SelectedItem = cbOcrBackSideRecipes.Items[0];
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        #endregion CB Selection Event Handlers

        #region Helpers

        private byte GetSelectedLpIndex()
        {
            byte lpIndex = 1;
            if (tcMain.SelectedTab.Name == "tControl")
            {
                if (!byte.TryParse(cbLLLoadPorts.SelectedItem.ToString().Substring(2), out lpIndex))
                {
                    throw new FormatException(string.Format(CultureInfo.InvariantCulture, "'{0}' not supported.",
                        cbLLLoadPorts.SelectedItem.ToString()));
                }
            }
            else if (tcMain.SelectedTab.Name == "tIO")
            {
                if (!byte.TryParse(cbIOLoadPorts.SelectedItem.ToString().Substring(2), out lpIndex))
                {
                    throw new FormatException(string.Format(CultureInfo.InvariantCulture, "'{0}' not supported.",
                        cbIOLoadPorts.SelectedItem.ToString()));
                }
            }
            else if (tcMain.SelectedTab.Name == "tLoadPorts")
            {
                if (!byte.TryParse(cbLoadPorts.SelectedItem.ToString().Substring(2), out lpIndex))
                {
                    throw new FormatException(string.Format(CultureInfo.InvariantCulture, "'{0}' not supported.",
                        cbLoadPorts.SelectedItem.ToString()));
                }
            }

            return lpIndex;
        }

        private SampleDimension GetWaferSize(TransferLocation location, RobotArm arm = RobotArm.Undefined)
        {
            /*
            switch (location)
            {
                case TransferLocation.PreAlignerA:
                    return Aligner.Location.Substrate.MaterialDimension;
                case TransferLocation.LoadPort1:
                    return Lp1.Carrier.SampleSize;
                case TransferLocation.LoadPort2:
                    return Lp2.Carrier.SampleSize;
                case TransferLocation.LoadPort3:
                    return Lp3.Carrier.SampleSize;
                case TransferLocation.ProcessModuleA:
                    return ProcessModule.Location.Substrate.MaterialDimension;
                case TransferLocation.Robot:
                    switch (arm)
                    {
                        case RobotArm.Arm1:
                            return Robot.LocationOnArm1.Substrate.MaterialDimension;
                        case RobotArm.Arm2:
                            return Robot.LocationOnArm2.Substrate.MaterialDimension;
                    }
                    break;
            }
            */
            // Should never happened
            return default;
        }

        private void TryCatch(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void TryCatchAsync(Task task)
        {
            task.ContinueWith(
                t =>
                {
                    foreach (Exception ex in t.Exception.InnerExceptions)
                    {
                        ShowError(ex.InnerException);
                    }
                },
                TaskContinuationOptions.OnlyOnFaulted);
        }

        #endregion Helpers

        #region IListener

        public void DoLog(TraceLine traceLine)
        {
            AppendMessage(traceLine.Text, string.Empty);
        }



        #endregion IListener
    }
}
