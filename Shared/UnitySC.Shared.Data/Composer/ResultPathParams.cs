using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.Shared.Data.Composer
{
    public class ResultPathParams : IParamComposerObject
    {
        public const string FmtDatetime = "yyyyMMdd_HHmmss";
        public const string FmtSlot = "00";

        public static readonly ResultPathParams Empty = new ResultPathParams();

        private readonly string[] _labels;

        public ResultPathParams()
        {
            _labels = new string[]
            {
                "ToolName",
                "ToolId",               // Id in Database.Tool
                "ToolKey",
                "ChamberName",
                "ChamberId",            // Id in Database.Chamber
                "ChamberKey",
                "JobName",
                "JobId",                // Id in Database.Job
                "LotName",
                "RecipeName",           // DataFlow Recipe name 
                "StartProcessDate",
                "WaferName",
                "Slot",
                "RunIter",
                "ResultType",
                "ResultTypeExt",
                "ActorType",
                "ResultFmt",
                "Index",
                "Label",
                "ProductName",
                "StepName",
                "Side",                  // Front or Back or Unknown  
                "LoadPort"
            };
        }
      
        public string[] ToParamLabels()
        {
            return _labels;
        }

        public object[] ToParamObjects()
        {
            return new object[]
             {
                ToolName,
                ToolId,
                ToolKey,
                ChamberName,
                ChamberId,
                ChamberKey,
                JobName,
                JobId,
                LotName,
                RecipeName,
                StartProcessDate.ToString(FmtDatetime),
                WaferName,
                Slot.ToString(FmtSlot),
                RunIter,
                ResultType,
                ResultTypeExt,
                ActorType,
                ResultFmt,
                Index,
                Label,
                ProductName,
                StepName,
                Side,
                LoadPort
             };
        }

        #region Properties

        public string ToolName { get; set; } = string.Empty;
        public int ToolId { get; set; } = -1;
        public int ToolKey { get; set; } = -1;
        public string ChamberName { get; set; } = string.Empty;
        public int ChamberId { get; set; } = -1;
        public int ChamberKey { get; set; } = -1;
        public string JobName { get; set; } = string.Empty;
        public int JobId { get; set; } = -1;
        public string LotName { get; set; } = string.Empty;
        public string RecipeName { get; set; } = string.Empty; // This is the JOB RecipeName
        public DateTime StartProcessDate { get; set; } = DateTime.MinValue;
        public string WaferName { get; set; } = string.Empty;
        public int Slot { get; set; } = 0;
        public int RunIter { get; set; } = 0;
        public ResultType ResultType { get; set; } = ResultType.NotDefined; //not defined as a format ASE by default , since this is a non register type, it allows to detect initialization failure
        public string ResultTypeExt { get { return (ResultType.GetResultCategory() == ResultCategory.Result) ? ResultType.GetExt() : string.Empty; } } // avoid exeption in case of Acquisition result 
        public ActorType ActorType { get { return ResultType.GetActorType(); } }
        public ResultFormat ResultFmt { get { return ResultType.GetResultFormat(); } }
        public int Index { get; set; } = 0;
        public string Label { get; set; } = string.Empty;  // mainly used for acquisition result, label is display in viewer (selection of any acsuqition kind)
        public string ProductName { get; set; } = string.Empty;
        public string StepName { get; set; } = string.Empty;
        public Side Side { get { return ResultType.GetSide(); } }  // Front or Back or Unknown  
        public int LoadPort { get; set; } = -1;


        #endregion
    }
}
