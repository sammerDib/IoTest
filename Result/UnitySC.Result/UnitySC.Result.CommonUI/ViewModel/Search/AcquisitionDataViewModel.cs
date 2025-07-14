using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.Result.CommonUI.ViewModel.Search
{
    public class AcquisitionDataViewModel : PPViewModel
    {
        public WaferAcquisitionResult[] AcqData { get; set; }

        public AcquisitionDataViewModel() : base()
        {
            AcqData = new WaferAcquisitionResult[25];
        }

        public AcquisitionDataViewModel(ResultType resTyp, WaferAcquisitionResult[] resdata) : base(resTyp)
        {
            AcqData = new WaferAcquisitionResult[25];
        }

        public AcquisitionDataViewModel(string labelName, ResultType resTyp, WaferAcquisitionResult[] resdata) : base(labelName, resTyp)
        {
            AcqData = resdata;
        }
    }
}
