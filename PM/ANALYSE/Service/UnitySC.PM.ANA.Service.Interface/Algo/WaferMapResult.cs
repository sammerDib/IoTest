using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class WaferMapResult : IFlowResult
    {
        [DataMember]
        [XmlIgnore]
        public FlowStatus Status { get; set; }

        [DataMember]
        public Angle RotationAngle { get; set; }

        [DataMember]
        public DieDimensionalCharacteristic DieDimensions { get; set; }

        [DataMember]
        public XYPosition DieGridTopLeft { get; set; }

        /// <summary>
        /// This property is only needed to reduce the size of the XML serialization,
        /// by serializing <c>DiesPresence</c> into a custom string instead.
        /// </summary>
        /// <remarks>
        /// Unfortunately <c>OnSerializingAttribute</c> and <c>OnDeserializedAttribute</c> are
        /// not taken into account by the <c>XmlSerializer</c>, hence this "trick".
        /// </remarks>
        [XmlElement("DiesPresence")]
        public string DiesPresenceSerialized
        {
            get { return SerializeMatrix(DiesPresence); }
            set { DiesPresence = DeserializeMatrix(value); }
        }

        [DataMember]
        [XmlIgnore]
        public Matrix<bool> DiesPresence { get; set; }

        [DataMember]
        public DieIndex DieReference { get; set; } = new DieIndex(0, 0);

        public int NbRows => DiesPresence?.Rows ?? 0;

        public int NbColumns => DiesPresence?.Columns ?? 0;

        private static string SerializeMatrix(Matrix<bool> matrix)
        {
            if (matrix is null)
                return null;

            var matrixString = new StringBuilder();
            matrixString.AppendLine();
            for (int j = 0; j < matrix.Rows; j++)
            {
                for (int i = 0; i < matrix.Columns; i++)
                {
                    matrixString.Append(matrix.GetValue(j, i) ? "1" : "0");
                }
                // Keep ";" at the end of last row to differenciate the 2 cases below:
                //  - Empty matrix:              Matrix<bool>(0, 0) which gives ""
                //  - Matrix with one empty row: Matrix<bool>(1, 0) which gives ";"
                matrixString.AppendLine(";");
            }
            return matrixString.ToString();
        }

        private static Matrix<bool> DeserializeMatrix(string serializedMatrix)
        {
            if (serializedMatrix is null)
                return null;

            if (string.IsNullOrEmpty(serializedMatrix) || string.IsNullOrWhiteSpace(serializedMatrix))
                return new Matrix<bool>(0, 0);

            if (serializedMatrix.Trim() == ";")
                return new Matrix<bool>(1, 0);

            var matrixRows = serializedMatrix.Split(new char[] { ';', '\n', '\t', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
            var matrix = new Matrix<bool>(matrixRows.Length, matrixRows[0].Length);
            for (int j = 0; j < matrixRows.Length; j++)
            {
                string matrixrow = matrixRows[j].Trim();
                for (int i = 0; i < matrixrow.Length; i++)
                {
                    matrix.SetValue(j, i, matrixrow[i] == '1');
                }
            }
            return matrix;
        }
    }
}
