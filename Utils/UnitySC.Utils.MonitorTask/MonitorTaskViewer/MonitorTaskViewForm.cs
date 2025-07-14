using LightningChartLib.WinForms.Charting;
using LightningChartLib.WinForms.Charting.Annotations;
using LightningChartLib.WinForms.Charting.Axes;
using LightningChartLib.WinForms.Charting.SeriesXY;
using LightningChartLib.WinForms.Charting.Views.ViewXY;
using UnitySC.Shared.Tools.MonitorTasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace MonitorTaskViewer
{
    public partial class MonitorTaskViewForm : Form
    {
        MonitorTaskTimer _mtt = null;
        private LightningChart _chart = null;
        private AnnotationXY _valueDisplay = null;
        private PointLineSeries _seriesBeingTracked = null;
        private string _fileonopen = string.Empty;

        public MonitorTaskViewForm(string[] args)
        {
            InitializeComponent();

            _chart = lightningChartUltimate1;
      
            /*      MonitorTskTimer tt = new MonitorTskTimer();
                  tt.FreshStart();

                  tt.Tag_ts(new TimeSpan(0, 0, 0, 0, 003), TskLbl.SpindleAccel, TskMoment.End, 10);

                  tt.Tag_ts(new TimeSpan(0, 0, 0, 0, 532), TskLbl.Acquisition, TskMoment.Start, 0);
                  tt.Tag_ts(new TimeSpan(0, 0, 0, 4, 142), TskLbl.SetUp, TskMoment.Start, 0);

                  tt.Tag_ts(new TimeSpan(0, 0, 0, 5, 142), TskLbl.StageMotionX, TskMoment.Start, 0);
                  tt.Tag_ts(new TimeSpan(0, 0, 0, 6, 422), TskLbl.StageMotionX, TskMoment.End, 0);

                  tt.Tag_ts(new TimeSpan(0, 0, 0, 14, 342), TskLbl.SetUp, TskMoment.End, 0);

                  tt.Tag_ts(new TimeSpan(0, 0, 0, 14, 343), TskLbl.StageMotionX, TskMoment.Start, 1);
                  tt.Tag_ts(new TimeSpan(0, 0, 0, 14, 343), TskLbl.StageMotionZ, TskMoment.Start, 0);

                  tt.Tag_ts(new TimeSpan(0, 0, 0, 15, 02), TskLbl.StageMotionZ, TskMoment.End, 0);
                  tt.Tag_ts(new TimeSpan(0, 0, 0, 16, 422), TskLbl.StageMotionX, TskMoment.End, 1);


                  tt.Tag_ts(new TimeSpan(0, 0, 0, 16, 342), TskLbl.SpectrumGrab, TskMoment.Start, 5);
                  tt.Tag_ts(new TimeSpan(0, 0, 0, 24, 942), TskLbl.SpectrumGrab, TskMoment.End, 5);

                  tt.Tag_ts(new TimeSpan(0, 0, 0, 24, 943), TskLbl.StageMotionX, TskMoment.Start, 2);

                  tt.Tag_ts(new TimeSpan(0, 0, 1, 25, 56), TskLbl.Acquisition, TskMoment.End, 0);

                  tt.SaveMonitorCSV(@"C:\Projets\Lightspeed\FPGATimer\MonitorTaskViewer\Test.mtt");*/

            if (args.Length > 0)
            {
                _fileonopen = args[0];
            }
        }

        private void buttonBrwseCSV_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (!String.IsNullOrEmpty(textBoxCSVPath.Text))
            {
                string sdir = Path.GetDirectoryName(textBoxCSVPath.Text);
                openFileDialog.InitialDirectory = sdir;
            }
            openFileDialog.Filter = "MTT Files|*.mtt|CSV Files|*.csv|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                if (File.Exists(openFileDialog.FileName))
                {
                    textBoxCSVPath.Text = openFileDialog.FileName;
                }
            }
        }

        private void buttonProceed_Click(object sender, EventArgs e)
        {
            if (File.Exists(textBoxCSVPath.Text))
            {
                Properties.Settings.Default.LastCSVPath = textBoxCSVPath.Text;
                Properties.Settings.Default.Save();
            }

            // clear previous data
            dataGridViewUnused.Rows.Clear();
            dataGridViewUnused.Refresh();

            dataGridViewObj.Rows.Clear();
            dataGridViewObj.Refresh();

            _chart.BeginUpdate();
            _chart.ViewXY.PointLineSeries.Clear();
            _chart.EndUpdate();

            _mtt = new MonitorTaskTimer();
            if (_mtt.ReadMonitorCSV(textBoxCSVPath.Text, out string errmsg))
            {
                AxisX axisX = _chart.ViewXY.XAxes[0];
                axisX.Title.Text = "Time in " + _mtt.Fmt;

                buttonProceed.Enabled = false;
                OnProceedMTT();
            }
            else
                MessageBox.Show($"Error while Parsing Monitor Tasks file\n{errmsg}");
        }

        private void CreateChart()
        {
            //Create new chart 
//_chart = new LightningChartUltimate("FOGALE Nanotech/Developer1-Renewed 2016-2017/LightningChartUltimate/E34CJDJ24YVJ3532RWN9KAN2YW2RM52YTNJU");

            //Disable rendering, strongly recommended before updating chart properties
            _chart.BeginUpdate();

            //Chart parent must be set
        //    _chart.Parent = this;

            //Fill parent area with chart
            //_chart.Dock = DockStyle.Fill;

            //Chart name
            _chart.Name = "Tasks";
            _chart.Title.Text = "LS Monitor Tasks";

            //Hide legend box
            //_chart.ViewXY.LegendBoxes.Select(x => x.Visible = false);
            _chart.ViewXY.LegendBoxes.First().Visible = false;

            //Setup x-axis
            AxisX axisX = _chart.ViewXY.XAxes[0];
            axisX.Title.Text = "Time";
            axisX.SetRange(0, 20);
            axisX.ScrollMode = XAxisScrollMode.None;
            axisX.ValueType = AxisValueType.Number;

            //Setup y-axis
            _chart.ViewXY.YAxes[0].SetRange(-10, 0);
            _chart.ViewXY.YAxes[0].Title.Visible = false;
            _chart.ViewXY.YAxes[0].Units.Visible = false;


            //Create annotation to show tracked data value 
            _valueDisplay = new AnnotationXY(_chart.ViewXY, axisX, _chart.ViewXY.YAxes[0]);
            _valueDisplay.LocationCoordinateSystem = CoordinateSystem.RelativeCoordinatesToTarget;
            _valueDisplay.Style = AnnotationStyle.Callout;
            _valueDisplay.AllowUserInteraction = false;
            _valueDisplay.Visible = false;
            _chart.ViewXY.Annotations.Add(_valueDisplay);

            _chart.MouseClick += _chart_MouseClick;
            _chart.MouseMove += _chart_MouseMove;

            //Allow chart rendering
            _chart.EndUpdate();
        }

        private void UpdateChart(MonitorTaskExtractor mtte)
        {
            _chart.BeginUpdate();

            int nNbTotal = mtte._Plan.Count;
        
            int seriesIndex = 0;
            foreach (MonitorTaskObject tobj in mtte._Plan)
            {
                Color color = DefaultColors.SeriesForBlackBackground[seriesIndex % DefaultColors.SeriesForBlackBackground.Length];

                PointLineSeries pls = new PointLineSeries(_chart.ViewXY, _chart.ViewXY.XAxes[0], _chart.ViewXY.YAxes[0]);
                pls.PointsVisible = true;
                pls.Highlight = Highlight.Simple;
                pls.LineStyle.Color = color;
                pls.LineStyle.Width = 5;
                pls.PointStyle.Color1 = color;

                pls.MouseClick += pls_MouseClick;
                pls.Title.Text = tobj.Lbl;
                pls.Tag = (object) tobj;

                SeriesPoint[] points = new SeriesPoint[2];

                points[0].X = tobj.TimeStart;
                points[0].Y = tobj.YOrder;

                points[1].X = tobj.TimeEnd;
                points[1].Y = tobj.YOrder;

                //Assign the data for the point line series 
                pls.Points = points;
                _chart.ViewXY.PointLineSeries.Add(pls);
                seriesIndex++;
            }


            //_chart.ViewXY.ZoomPanOptions.ViewFitYMarginPixels = 20;
            //_chart.ViewXY.ZoomToFit();

            _chart.ViewXY.XAxes[0].SetRange(0.0, mtte.MaxTime * 1.05);
            if (MonitorTaskExtractor.YCoef >= 0)
            {
                _chart.ViewXY.YAxes[0].SetRange(0, mtte.GreaterYOrder);
            }
            else
            {
                _chart.ViewXY.YAxes[0].SetRange(mtte.GreaterYOrder, 0);
            }



            _chart.EndUpdate();
        }

        void pls_MouseClick(object sender, MouseEventArgs e)
        {
            _chart.BeginUpdate();

            PointLineSeries pls = (PointLineSeries)sender;

            if (pls == _seriesBeingTracked)
            {
                _valueDisplay.Visible = false;
                if (_seriesBeingTracked != null)
                {
                    _seriesBeingTracked.Highlight = Highlight.Simple; ; // Enable mouse hover effect
                    _seriesBeingTracked = null;
                }
                _chart.EndUpdate();

                dataGridViewObj.CurrentCell = null;

            }
            else
            {
                if(_seriesBeingTracked != null)
                    _valueDisplay.Visible = false;

                //Set series being tracked 
                _seriesBeingTracked = (PointLineSeries)sender;
                _seriesBeingTracked.Highlight = Highlight.None; // Disable mouse hover effect

                //Set correct Y axis for value display 
                _valueDisplay.AssignYAxisIndex = _seriesBeingTracked.AssignYAxisIndex;

                //Change color for the value display
                _valueDisplay.Fill.Color = ChartTools.CalcGradient(_seriesBeingTracked.LineStyle.Color, Color.White, 90);
                _valueDisplay.Fill.GradientColor = ChartTools.CalcGradient(_seriesBeingTracked.LineStyle.Color, Color.White, 50);

                /*if (_valueDisplay.AssignYAxisIndex == 0)
                    _valueDisplay.LocationRelativeOffset = new PointFloatXY(0, 40);     // Show annotation below series.
                else if (_valueDisplay.AssignYAxisIndex == _chart.ViewXY.PointLineSeries.Count - 1)
                    _valueDisplay.LocationRelativeOffset = new PointFloatXY(0, -40);    // Show annotation above series.
                else
                    _valueDisplay.LocationRelativeOffset = new PointFloatXY(40, 0);     // Show annotation on the right to tracked series point.*/

                _valueDisplay.Visible = true;
                MonitorTaskObject tobj = (MonitorTaskObject)_seriesBeingTracked.Tag;
                _valueDisplay.Text = _seriesBeingTracked.Title.Text + " :\nDuration : " + tobj.Duration.ToString(@"mm\:ss\.fff") + "\n Start : " + tobj.TimeStart + " ms <-> End : " + tobj.TimeEnd + " ms";

                _valueDisplay.TargetAxisValues.SetValues(0.5 * (_seriesBeingTracked.Points[0].X + _seriesBeingTracked.Points[1].X), _seriesBeingTracked.Points[0].Y);

                _chart.EndUpdate();

                if (dataGridViewObj.CurrentCell == null)
                {
                    // search for row index 
                    int rowIndex = -1;
                    var row = dataGridViewObj.Rows.Cast<DataGridViewRow>()
                                .Where(r => ((MonitorTaskObject)r.Tag).Idx == tobj.Idx)
                                .First();
                    rowIndex = row.Index;
                    dataGridViewObj.CurrentCell = dataGridViewObj.Rows[rowIndex].Cells[0];
                    dataGridViewObj.FirstDisplayedScrollingRowIndex = Math.Max(0, rowIndex - 2);
                }
                else if (dataGridViewObj.CurrentCell.RowIndex != tobj.Idx)
                {
                    //dataGridViewObj.Rows[tobj._Idx].Selected = true;
                    // dataGridViewObj.CurrentCell = dataGridViewObj.Rows[tobj._Idx].Cells[0];
                    int rowIndex = -1;
                    var row = dataGridViewObj.Rows.Cast<DataGridViewRow>()
                                .Where(r => ((MonitorTaskObject)r.Tag).Idx == tobj.Idx)
                                .First();
                    rowIndex = row.Index;
                    dataGridViewObj.CurrentCell = dataGridViewObj.Rows[rowIndex].Cells[0];
                    dataGridViewObj.FirstDisplayedScrollingRowIndex = Math.Max(0, rowIndex - 2);
                }

            }
           

            
        }

        void _chart_MouseClick(object sender, MouseEventArgs e)
        {
            //Detect if chart was clicked elsewhere than series. 
            bool overSeries = false;
            foreach (PointLineSeries pls in _chart.ViewXY.PointLineSeries)
            {
                if (pls.IsPositionOver(e.X, e.Y))
                {
                    overSeries = true;
                    break;
                }
            }
            if (!overSeries)
            {
                _valueDisplay.Visible = false;

                if (_seriesBeingTracked != null)
                {
                    _seriesBeingTracked.Highlight = Highlight.Simple; // Enable mouse hover effect
                    _seriesBeingTracked = null;
                    dataGridViewObj.CurrentCell = null;
                }
            }
        }

        void _chart_MouseMove(object sender, MouseEventArgs e)
        {
            if (_valueDisplay.Visible)
               UpdateNearestValue(e);
        }

        void UpdateNearestValue(MouseEventArgs e)
        {
            //Find nearest data point from the series being tracked (clicked previously) 
            _chart.BeginUpdate();

            if (_seriesBeingTracked != null)
            {
                double x;

                _chart.ViewXY.XAxes[0].CoordToValue(e.X, out x, false);

               LineSeriesValueSolveResult res = _seriesBeingTracked.SolveYValueAtXValue(x);
                if (res.SolveStatus == LineSeriesSolveStatus.OK)
                {
                    double nearestX = _seriesBeingTracked.Points[res.NearestDataPointIndex].X;
                    double nearestY = _seriesBeingTracked.Points[res.NearestDataPointIndex].Y;

                    _valueDisplay.Visible = true;
//                    _valueDisplay.Text = "X: " + nearestX.ToString("0.0") + "\nY: " + nearestY.ToString("0.0");
                    _valueDisplay.TargetAxisValues.SetValues(nearestX, nearestY);
                    _valueDisplay.TargetAxisValues.SetValues(x, nearestY);

                }
                else
                {
                    if(Math.Abs(_seriesBeingTracked.Points[0].X-x) < Math.Abs(_seriesBeingTracked.Points[1].X - x))
                        _valueDisplay.TargetAxisValues.SetValues(_seriesBeingTracked.Points[0].X, _seriesBeingTracked.Points[0].Y);
                    else if (Math.Abs(_seriesBeingTracked.Points[0].X - x) > Math.Abs(_seriesBeingTracked.Points[1].X - x))
                        _valueDisplay.TargetAxisValues.SetValues(_seriesBeingTracked.Points[1].X, _seriesBeingTracked.Points[0].Y);
                    else
                        _valueDisplay.TargetAxisValues.SetValues(0.5 * (_seriesBeingTracked.Points[0].X + _seriesBeingTracked.Points[1].X), _seriesBeingTracked.Points[0].Y);
                   // _valueDisplay.Visible = false;
                }
            }

            _chart.EndUpdate();
        }

        public void OnProceedMTT()
        {
           
            if (InvokeRequired)
                BeginInvoke((Action)(() => this.ProceedMTT()));
            else
                ProceedMTT();
        }

        public void ProceedMTT()
        {

            MonitorTaskExtractor mtte = new MonitorTaskExtractor(_mtt.Items, _mtt.Fmt);
            mtte.Proceed();
            //mtte._Plan


            // display summary
            foreach (MonitorTaskObject obj in mtte._Plan)
            {
               /* double dStart, dEnd;
                switch (mtte._fmt)
                {
                    case "mS": dStart = obj.TSpan.TotalMilliseconds; break;
                    case "S":  dStart = uit.TSpan.TotalSeconds; break;
                    case "M":  dStart = uit.TSpan.TotalMinutes; break;
                    default:  dStart = uit.TSpan.TotalMilliseconds; break;
                }*/
                dataGridViewObj.Rows.Add(obj.Lbl, obj.Duration.ToString(@"mm\:ss\.fff"), obj.TimeStart, obj.TimeEnd);
                dataGridViewObj.Rows[dataGridViewObj.Rows.Count-1].Tag = obj;
            }

            // display unused un paired task 
            foreach (MonitorTaskItem uit in  mtte.UnpaireddTask)
            {
                double dtime;
                switch(mtte.Fmt)
                {
                    case "mS": dtime = uit.TSpan.TotalMilliseconds;  break;
                    case "S": dtime = uit.TSpan.TotalSeconds; break;
                    case "M": dtime = uit.TSpan.TotalMinutes; break;
                    default: dtime = uit.TSpan.TotalMilliseconds; break;
                }
                dataGridViewUnused.Rows.Add(uit.Label, uit.Moment.ToString(), dtime, uit.ID);
            }

            UpdateChart(mtte);

            buttonProceed.Enabled = true;
        }


        private void MonitorTaskViewForm_Load(object sender, EventArgs e)
        {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            this.Text += " -- v";
            this.Text += version.ToString();

            dataGridViewUnused.RowHeadersWidth = 4;
            dataGridViewObj.RowHeadersWidth = 4;
            CreateChart();

            textBoxCSVPath.Text = Properties.Settings.Default.LastCSVPath;
            if (!String.IsNullOrEmpty(_fileonopen))
            {
                textBoxCSVPath.Text = _fileonopen;
                buttonProceed_Click(null, null);
            }
        }

        private void MonitorTaskViewForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void dataGridViewObj_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewObj.SelectedRows.Count < 1 || _chart.ViewXY.PointLineSeries.Count() < 1 )
                return;

            int nidxsel = dataGridViewObj.SelectedRows[0].Index;
            var tskobj = (MonitorTaskObject) dataGridViewObj.SelectedRows[0].Tag;
            if (_chart.ViewXY.PointLineSeries.Count() > tskobj.Idx)
            {
                var plsToSelect = _chart.ViewXY.PointLineSeries[tskobj.Idx];
                if (plsToSelect != _seriesBeingTracked)
                    pls_MouseClick(plsToSelect, null);

                FitTaskSelection();
            }
        }

        private void FitTaskSelection()
        {
            if (_seriesBeingTracked != null)
            {
                bool bNeedfitY = false;
                bool bNeedfitX = false;

                var aX = _chart.ViewXY.XAxes[0];
                var aY  =_chart.ViewXY.YAxes[0];
                var tskobj = (MonitorTaskObject) _seriesBeingTracked.Tag;

                // Fit Y
                if (tskobj.YOrder <= aY.Minimum || tskobj.YOrder >= (aY.Maximum + 2 * MonitorTaskExtractor.YCoef))
                {
                    // need fit Y
                    bNeedfitY = true;
                }

                // Fit X
                if ((tskobj.TimeStart >= aX.Minimum && tskobj.TimeStart <= aX.Maximum) && (tskobj.TimeEnd >= aX.Minimum && tskobj.TimeEnd <= aX.Maximum))
                {
                    // la task est dasn le range affiché on ne fit pas
                }
                else
                    bNeedfitX = true;

                _chart.BeginUpdate();
                if (bNeedfitY)
                {
                     //double drangeMid = (aY.Maximum - aY.Minimum) * 0.5;
                     double middle = (aY.Maximum + aY.Minimum) * 0.5;
                     double diff = middle - tskobj.YOrder;

                    aY.Maximum = Math.Min(aY.Maximum - diff, -2 * MonitorTaskExtractor.YCoef);
                    aY.Minimum = aY.Minimum - diff;
                }

                if (bNeedfitX)
                {
                    double drangeMid = (tskobj.TimeEnd - tskobj.TimeStart);
                    double range = (aX.Maximum - aX.Minimum) ;
                    if (range > drangeMid)
                    {
                        // on slide vers le middle
                        double diff = ((aX.Maximum + aX.Minimum) - (tskobj.TimeEnd + tskobj.TimeStart)) * 0.5;
                        aX.Maximum -= diff;
                        aX.Minimum = Math.Max(aX.Minimum - diff, -5000); 
                    }
                    else
                    {
                        // on readapt la window
                        aX.Maximum = tskobj.TimeEnd + 0.1 * drangeMid;
                        aX.Minimum = Math.Max(tskobj.TimeStart - 0.1 * drangeMid, -5000);
                    }
                }

                _chart.EndUpdate();

            }
        }
    }
}
