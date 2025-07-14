using System.Runtime.Serialization;

namespace UnitySC.DataAccess.Dto.ModelDto.LocalDto
{
    [DataContract]
    public class OutPreRegister
    {
        [DataMember]
        public string ResultPathRoot { get; set; } = string.Empty; //ex : \\<PCNAME>\\ADC_RESULTS\\BF2D\\

        [DataMember]
        public string ResultFileName { get; set; } = string.Empty; //ex : ECLIPSE_87920_D06132017_EDGE_LGLASS_W_LP2_S5_0  ou   S5_OcrId5_0

        [DataMember]
        public int RunIter;

        [DataMember]
        public long InternalDBResId; // from result Table - same as InPreRegister.ParentResultId - should be re-used in case of multiple results to register (same wafer/recipe/chamber) 

        [DataMember]
        public long InternalDBResItemId; // from resultItem Table - should be re-used and passesd to IRegisterResultService.UpdateResultState 
    }

    [DataContract]
    public class OutPreRegisterAcquisition
    {
        [DataMember]
        public long InternalDBResId; // from resultAcq Table - same as InPreRegister.ParentResultId - should be re-used in case of multiple results to register (same wafer/recipe/chamber) 

        [DataMember]
        public long InternalDBResItemId; // from resultAcqItem Table - should be re-used and passesd to IRegisterResultService.UpdateResultState 

        [DataMember]
        public InPreRegisterAcquisition Inputs; // should contains updated Filename and Paths in which file should be oe has been copied 
    }
}
