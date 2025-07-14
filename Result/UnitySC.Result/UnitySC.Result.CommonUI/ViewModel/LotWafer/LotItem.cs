using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.Result.CommonUI.ViewModel.LotWafer
{
    public class LotItem
    {
        #region properties
        public bool IsAcquisition { get; private set; }

        public long Id { get; private set; }
      
        public int State { get; set; }
      
        public int InternalState { get; set; }

        public ResultType ResultTypeEnum { get; private set; }

        public string WaferName { get; private set; }

        public string ResPath { get; private set; }

        public string ResThumbnailPath { get; private set; }

        #endregion //properties

        #region constructor
        public LotItem()
        {
            ResultTypeEnum = ResultType.NotDefined;
        }

        public LotItem(DataAccess.Dto.ResultItem resItem)
        {
            IsAcquisition = false;

            Id = resItem.Id;
            ResultTypeEnum = resItem.ResultTypeEnum;
            
            State = resItem.State;
            InternalState = resItem.InternalState;

            WaferName = resItem.Result.WaferResult.WaferName;

            ResPath = resItem.ResPath;
            ResThumbnailPath = resItem.ResThumbnailPath;
        }

        public LotItem(DataAccess.Dto.ResultAcqItem resAcqItem)
        {
            IsAcquisition = true;

            Id = resAcqItem.Id;
            ResultTypeEnum = resAcqItem.ResultTypeEnum;

            State = resAcqItem.State;
            InternalState = resAcqItem.InternalState;

            WaferName = resAcqItem.ResultAcq.WaferResult.WaferName;

            ResPath = resAcqItem.ResPath;
            ResThumbnailPath = resAcqItem.ResThumbnailPath;
        }
        #endregion //constructor
    }
}
