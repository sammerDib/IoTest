using System.Runtime.Serialization;

using UnitySC.Shared.Data;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Referentials.Interface
{
    /// <summary>
    /// Settings to create the die referential, allowing to move inside a given die
    /// </summary>
    [DataContract]
    public class DieReferentialSettings : ReferentialSettingsBase
    {
        public DieReferentialSettings(Angle dieGridAngle, DieDimensionalCharacteristic dieDimensionalCharacteristic, XYPosition dieGridTopLeft, Matrix<bool> presenceGrid) : base(ReferentialTag.Die)
        {
            DieGridAngle = dieGridAngle;
            DieGridDimensions = dieDimensionalCharacteristic;
            DieGridTopLeft = dieGridTopLeft;
            PresenceGrid = presenceGrid;
        }

        public void ApplyMarkAlignement(double shiftX, double shiftY, Angle angle)
        {
            DieGridTopLeft.X = DieGridTopLeft.X + shiftX;
            DieGridTopLeft.Y = DieGridTopLeft.Y + shiftY;
            DieGridAngle = DieGridAngle + angle;
        }

        /// <summary>
        /// Angle rotation against perfect 270° axis
        /// A negative value indicate a clockwise rotation, where positive value indicates anti-clockwise rotation
        /// </summary>
        [DataMember]
        public Angle DieGridAngle { get; set; }

        /// <summary>
        /// Die and street dimensions into the die grid
        /// </summary>
        [DataMember]
        public DieDimensionalCharacteristic DieGridDimensions { get; set; }

        /// <summary>
        /// Top left corner of the die grid
        /// </summary>
        [DataMember]
        public XYPosition DieGridTopLeft { get; set; }

        /// <summary>
        /// If array contains true value, the die is inside usable wafer. If false, it's not.
        /// </summary>
        [DataMember]
        public Matrix<bool> PresenceGrid { get; set; }


        public override string ToString()
        {
            return $" DieGridAngle : {DieGridAngle} DieWidth : {DieGridDimensions.DieWidth} DieHeight : {DieGridDimensions.DieHeight} StreetWidth : {DieGridDimensions.StreetWidth} StreetHeight : {DieGridDimensions.StreetHeight}";
        }

    }
}
