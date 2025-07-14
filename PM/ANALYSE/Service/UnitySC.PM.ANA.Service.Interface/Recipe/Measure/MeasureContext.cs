using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Algo;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure
{
    /// <summary>
    /// Measure context contains local information about the current measure point. Global settings
    /// that are common between each measure point are not meant to be stored in the context but
    /// passed in the <see cref="MeasureSettings"/>.
    /// </summary>
    [DataContract]
    public class MeasureContext
    {
        public MeasureContext(MeasurePoint measurePoint, DieIndex dieIndex, ResultFoldersPath resultFoldersPath)
        {
            MeasurePoint = measurePoint;
            DieIndex = dieIndex;
            ResultFoldersPath = resultFoldersPath;
        }

        [DataMember]
        public MeasurePoint MeasurePoint { get; }

        [DataMember]
        public DieIndex DieIndex { get; }


        [DataMember]
        public ResultFoldersPath ResultFoldersPath { get; }

        public MeasureContextRepeat ConvertToMeasureContextRepeat(int repeatIndex)
        {
            return new MeasureContextRepeat(MeasurePoint, DieIndex, ResultFoldersPath, repeatIndex);
        }
    }

    [DataContract]
    public class MeasureContextRepeat : MeasureContext
    {
        public MeasureContextRepeat(MeasurePoint measurePoint, DieIndex dieIndex, ResultFoldersPath resultFoldersPath, int repeatIndex): base(measurePoint, dieIndex, resultFoldersPath)
        {
            RepeatIndex = repeatIndex;
        }

        [DataMember]
        public int RepeatIndex { get; }
    }
}
