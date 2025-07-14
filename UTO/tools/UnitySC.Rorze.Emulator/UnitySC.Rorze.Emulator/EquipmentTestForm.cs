using System.Windows.Forms;

using UnitySC.Rorze.Emulator.Common;
using UnitySC.Rorze.Emulator.Equipment.BaseEquipmentControl;

namespace UnitySC.Rorze.Emulator
{
    internal partial class EquipmentTestForm : Form
    {
        public EquipmentTestForm()
        {
            InitializeComponent();

            foreach (TabPage tabPage in deviceSelectionControlTab.TabPages)
            {
                foreach (Control control in tabPage.Controls)
                {
                    if (control is IEquipmentControl equipmentControl)
                    {
                        equipmentControl.AutoResponseEnabled = true;
                    }
                }
            }

            rR75xRobotControl1.WaferPickedByRobot += WaferPickedByRobot;
            rR75xRobotControl1.WaferPlacedByRobot += WaferPlacedByRobot;

            _rv101LoadPortControl1.Setup("127.0.0.1", "12003");
            _rv101LoadPortControl2.Setup("127.0.0.1", "12004");
            rR75xRobotControl1.Setup("127.0.0.1", "12002", false);
            rA420AlignerControl1.Setup("127.0.0.1", "12001", false);
            dio0Control1.Setup("127.0.0.1", "23");
            dio1Control1.Setup("127.0.0.1", "24");
            dio2Control1.Setup("127.0.0.1", "25");
            dio1MediumSizeEfemControl1.Setup("127.0.0.1", "26");
        }

        private void HandleEquipmentTestFormClosing(object sender, FormClosingEventArgs e)
        {
            rR75xRobotControl1.WaferPickedByRobot -= WaferPickedByRobot;
            rR75xRobotControl1.WaferPlacedByRobot -= WaferPlacedByRobot;

            foreach (TabPage tabPage in deviceSelectionControlTab.TabPages)
            {
                foreach (Control control in tabPage.Controls)
                {
                    if (control is IEquipmentControl equipmentControl)
                    {
                        equipmentControl.Clean();
                    }
                }
            }
        }

        #region Event Handlers

        private void WaferPickedByRobot(object sender, WaferMovedEventArgs e)
        {
            if (e.LocationId == Constants.AlignerLocationId300mm
                || e.LocationId == Constants.AlignerLocationId200mm)
            {
                rA420AlignerControl1.RemoveWafer();
            }
        }

        private void WaferPlacedByRobot(object sender, WaferMovedEventArgs e)
        {
            if (e.LocationId == Constants.AlignerLocationId300mm
                || e.LocationId == Constants.AlignerLocationId200mm)
            {
                rA420AlignerControl1.PlaceWafer();
            }
        }

        #endregion
    }
}
