using System;

namespace UnitySC.DataAccess.ResultScanner.Implementation
{
    public class ResultPrioAcq : Dto.ResultAcqItem, IComparable
    {
        public int PrioCounter { get; set; } = 0;

        public ResultPrioAcq()
            : base()
        {
        }

        public ResultPrioAcq(SQL.ResultAcqItem x)
           : base()
        {
            PrioCounter = 0;

            Id = x.Id;
            ResultAcqId = x.ResultAcqId;
            ResultType = x.ResultType;
            FileName = x.FileName;
            Date = x.Date;
            Name = x.Name;
            Idx = x.Idx;
            State = x.State;
            InternalState = x.InternalState;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            var otherItem = obj as ResultPrioAcq;
            if (otherItem != null)
            {
                int nCmp1 = PrioCounter.CompareTo(otherItem.PrioCounter);
                if (nCmp1 == 0)
                    return 0 - Id.CompareTo(otherItem.Id);
                else
                    return nCmp1;
            }
            else
                throw new ArgumentException("Bad Item ResultItem type");
        }
    }
}
