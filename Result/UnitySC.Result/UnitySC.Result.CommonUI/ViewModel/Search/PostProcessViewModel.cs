using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.Result.CommonUI.ViewModel.Search
{
    public class PostProcessViewModel : PPViewModel
    {
        public WaferResultData[] ResultData { get; set; }

        public PostProcessViewModel() : base()
        {
            ResultType = ResultType.ADC_Klarf;
            ResultData = new WaferResultData[25];
        }

        public PostProcessViewModel(ResultType resTyp, WaferResultData[] resdata) : base(resTyp)
        {
            ResultData = resdata;
        }

        public PostProcessViewModel(string labelName, ResultType resTyp, WaferResultData[] resdata) : base(labelName, resTyp)
        {
            ResultData = resdata;
        }
    }
}
