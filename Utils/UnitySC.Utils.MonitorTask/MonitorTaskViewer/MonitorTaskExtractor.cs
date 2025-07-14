using System;
using System.Collections.Generic;
using System.Linq;
using UnitySC.Shared.Tools.MonitorTasks;
using System.Text;
using System.Threading.Tasks;

namespace MonitorTaskViewer
{
    public class MonitorTaskExtractor
    {
        private List<MonitorTaskItem> _tsklist;
        public List<MonitorTaskItem> UnpaireddTask;

        public List<MonitorTaskObject> _Plan;
        public string Fmt = String.Empty;
        static public double YCoef = -0.1;
        public double MaxTime { get; set; }
        public double GreaterYOrder { get { return (_Plan == null) ? 1 : YCoef * (_Plan.Count + 1); } }

        public MonitorTaskExtractor(List<MonitorTaskItem> tsklist, string fmt)
        {
            Fmt = fmt;
            _tsklist = new List<MonitorTaskItem>(tsklist.Count);
            foreach (MonitorTaskItem it in tsklist)
            {
                _tsklist.Add(it.Clone() as MonitorTaskItem);
            }
            UnpaireddTask = new List<MonitorTaskItem>();
        }

        public void Proceed()
        {

            _Plan = new List<MonitorTaskObject>((int)(_tsklist.Count / 2 + 1));
            UnpaireddTask.Clear();

            MaxTime = -1.0;
            // on appaire chaque task
            while (_tsklist.Count != 0)
            {
                MonitorTaskItem itcur = _tsklist.FirstOrDefault(x => x.Moment == TaskMoment.Start);
                if (itcur != null)
                {
                    _tsklist.Remove(itcur);

                    MonitorTaskItem itpaired = _tsklist.FirstOrDefault(x => (x.FullLabel == itcur.FullLabel) && (x.Moment == TaskMoment.End));
                    if (itpaired != null)
                    {
                        _tsklist.Remove(itpaired);

                        MonitorTaskObject obj = new MonitorTaskObject();
                        obj.Lbl = itcur.FullLabel;
                        switch (Fmt)
                        {
                            case "mS":
                                obj.TimeStart = itcur.TSpan.TotalMilliseconds;
                                obj.TimeEnd = itpaired.TSpan.TotalMilliseconds;
                                break;
                            case "S":
                                obj.TimeStart = itcur.TSpan.TotalSeconds;
                                obj.TimeEnd = itpaired.TSpan.TotalSeconds; break;
                            case "M":
                                obj.TimeStart = itcur.TSpan.TotalMinutes;
                                obj.TimeEnd = itpaired.TSpan.TotalMinutes; break;
                            default:
                                obj.TimeStart = itcur.TSpan.TotalMilliseconds;
                                obj.TimeEnd = itpaired.TSpan.TotalMilliseconds; break;
                        }

                        obj.Duration = itpaired.TSpan - itcur.TSpan;
                        obj.YOrder = YCoef * ((double)_Plan.Count() + 1.0);
                        obj.Idx = _Plan.Count();
                        _Plan.Add(obj);

                        MaxTime = Math.Max(obj.TimeEnd, MaxTime);
                    }
                    else
                    {
                        // start qui n'a pas de pair end
                        UnpaireddTask.Add(itcur);
                    }
                }
                else
                {
                    // No More Task.Start 
                    UnpaireddTask.AddRange(_tsklist);
                    _tsklist.Clear();

                }
            }
        }
    }
}
