using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.ResultUI.Common.Enums;
using UnitySC.Shared.ResultUI.Common.Message;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.Dummy
{
    public class DummyStatsVM : LotStatsVM
    {
        private string _label;

        public string DummyStatsLabel
        {
            get => _label; set { if (_label != value) { _label = value; OnPropertyChanged(); } }
        }

        private ImageSource _img;

        public ImageSource DummyStatsImage
        {
            get
            {
                if (_img == null)
                {
                    _img = (ImageSource)Application.Current.FindResource("WorkInProgressImage");
                }
                return _img;
            }
            set { if (_img != value) { _img = value; OnPropertyChanged(); } }
        }
        
        public void OnChangeSelectedResultFullName(DisplaySelectedResultFullNameMessage msg)
        {
            SelectedResultFullName = msg.SelectedResultFullName;
        }

        public DummyStatsVM()
        {
            DummyStatsLabel = "DUMMY RESULT LOT STATS VIEW";
            LotViews = StatsFactory.GetEnumsWithDescriptions<LotView>();
            LotSelectedView = new KeyValuePair<LotView, string>();
            Messenger.Register<DisplaySelectedResultFullNameMessage>(this, (r, m) => OnChangeSelectedResultFullName(m));
        }

        protected override void OnDeactivated()
        {
            Messenger.Unregister<DisplaySelectedResultFullNameMessage>(this);
            base.OnDeactivated();
        }

        public override void UpdateStats(object stats) // to define more accuretly
        {
        }

        public override void SelectLotView(object lotview)
        {
            var lv = (LotView)lotview;
            LotSelectedView = LotViews.FirstOrDefault(x => x.Key == lv);
        }
    }
}
