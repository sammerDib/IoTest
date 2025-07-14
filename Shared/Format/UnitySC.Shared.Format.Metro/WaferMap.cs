using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro
{
    public class WaferMap
    {
        [XmlElement("Angle")]
        public Angle RotationAngle { get; set; }

        [XmlElement("DieWidth")]
        public Length DieSizeWidth { get; set; }

        [XmlElement("DieHeight")]
        public Length DieSizeHeight { get; set; }

        [XmlElement("PitchWidth")]
        public Length DiePitchWidth { get; set; }

        [XmlElement("PitchHeight")]
        public Length DiePitchHeight { get; set; }

        [XmlElement("GridXPosition")]
        public Length DieGridTopLeftXPosition { get; set; }

        [XmlElement("GridYPosition")]
        public Length DieGridTopLeftYPosition { get; set; }

        [XmlElement("DieRefCol")]
        public int DieReferenceColumnIndex { get; set; }

        [XmlElement("DieRefRow")]
        public int DieReferenceRowIndex { get; set; }

        public List<DiesPresenceInRows> DiesPresence { get; set; }

        [XmlIgnore]
        public int NbRows => DiesPresence?.Count() ?? 0;

        [XmlIgnore]
        public int NbColumns => NbRows > 0 ? DiesPresence[0].Dies.Length : 0;

        public void SetDiesPresences(bool[][] presences)
        {
            DiesPresence = new List<DiesPresenceInRows>();
            int nbRows = presences.Count();
            int nbCols = presences[0].Count();

            for (int i = 0; i < nbRows; i++)
            {
                var diesPresenceInRows = new DiesPresenceInRows() { RowIndex = i };
                bool[] rowPesences = new bool[nbCols];
                for (int j = 0; j < nbCols; j++)
                {
                    rowPesences[j] = presences[i][j];
                }
                diesPresenceInRows.Dies = string.Join(string.Empty, rowPesences.Select(x => x ? "1" : "0"));
                DiesPresence.Add(diesPresenceInRows);
            }
        }

        public double WaferRelativeXPositionLeftOfColumn(int columnIndex)
        {
            return DieGridTopLeftXPosition.Millimeters + DiePitchWidth.Millimeters * ConvertColumnFromDieReference(columnIndex);
        }

        public double WaferRelativeYPositionTopOfRow(int rowIndex)
        {
            return DieGridTopLeftYPosition.Millimeters - DiePitchHeight.Millimeters * ConvertRowFromDieReference(rowIndex);
        }

        public int ConvertRowFromDieReference(int row)
        {
            return ConvertRowFromDieReference(row, DieReferenceRowIndex);
        }

        public int ConvertRowToDieReference(int row)
        {
            return ConvertRowToDieReference(row, DieReferenceRowIndex);
        }

        public int ConvertColumnFromDieReference(int column)
        {
            return ConvertColumnFromDieReference(column, DieReferenceColumnIndex);
        }

        public int ConvertColumnToDieReference(int column)
        {
            return ConvertColumnToDieReference(column, DieReferenceColumnIndex);
        }

        public static int ConvertRowFromDieReference(int row, int rowReferenceDie)
        {
            return rowReferenceDie - row;
        }

        public static int ConvertRowToDieReference(int row, int rowReferenceDie)
        {
            return rowReferenceDie - row;
        }

        public static int ConvertColumnFromDieReference(int column, int columnReferenceDie)
        {
            return column + columnReferenceDie;
        }

        public static int ConvertColumnToDieReference(int column, int columnReferenceDie)
        {
            return column - columnReferenceDie;
        }
    }

    public class DiesPresenceInRows
    {
        [XmlAttribute("Dies")]
        public string Dies { get; set; }

        [XmlAttribute("RowIndex")]
        public int RowIndex { get; set; }
    }
}
