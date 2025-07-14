using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;

using Agileo.Common.Localization;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;
using Agileo.Semi.Gem300.Abstractions.E87;
using Agileo.Semi.Gem300.Abstractions.E90;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Filters;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree;
using UnitySC.GUI.Common.Vendor.UIComponents.GuiExtended;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.SubstrateTracking
{
    public class SubstrateTrackingPanelModel : BusinessPanel
    {
        #region Fields

        private readonly Dictionary<string, IE39ObjectViewModel> _locations = new();

        #endregion

        #region Constructors

        static SubstrateTrackingPanelModel()
        {
            DataTemplateGenerator.Create(typeof(SubstrateTrackingPanelModel), typeof(SubstrateTrackingView));
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(SubstrateTrackingRessources)));
        }

        public SubstrateTrackingPanelModel() : this($"{nameof(SubstrateTrackingPanelModel)} DesignTime Constructor")
        {
        }

        public SubstrateTrackingPanelModel(string relativeId, IIcon icon = null) : base(relativeId, icon)
        {
            #region Substrates

            SubstratesSource = new DataTableSource<Substrate>();

            _ = SubstratesSource.Search.AddSearchDefinition(
                nameof(SubstrateTrackingRessources.SUB_TRACK_ID),
                sub => sub.ObjID,
                true);
            _ = SubstratesSource.Search.AddSearchDefinition(
                nameof(SubstrateTrackingRessources.SUB_TRACK_ACQUIRED_ID),
                sub => sub.AcquiredID,
                true);
            _ = SubstratesSource.Search.AddSearchDefinition(
                nameof(SubstrateTrackingRessources.SUB_TRACK_LOT_ID),
                sub => sub.LotID,
                true);
            _ = SubstratesSource.Search.AddSearchDefinition(
                nameof(SubstrateTrackingRessources.SUB_TRACK_POSITION),
                sub => sub.SubstPosInBatch,
                true);
            _ = SubstratesSource.Search.AddSearchDefinition(
                nameof(SubstrateTrackingRessources.SUB_TRACK_STATE),
                sub => sub.SubstState.ToString(),
                true);
            _ = SubstratesSource.Search.AddSearchDefinition(
                nameof(SubstrateTrackingRessources.SUB_TRACK_PROC_STATE),
                sub => sub.SubstProcState.ToString(),
                true);
            _ = SubstratesSource.Search.AddSearchDefinition(
                nameof(SubstrateTrackingRessources.SUB_TRACK_SOURCE),
                sub => sub.SubstSource,
                true);
            _ = SubstratesSource.Search.AddSearchDefinition(
                nameof(SubstrateTrackingRessources.SUB_TRACK_DESTINATION),
                sub => sub.SubstDestination,
                true);
            _ = SubstratesSource.Search.AddSearchDefinition(
                nameof(SubstrateTrackingRessources.SUB_TRACK_USAGE),
                sub => sub.SubstUsage.ToString(),
                true);
            _ = SubstratesSource.Search.AddSearchDefinition(
                nameof(SubstrateTrackingRessources.SUB_TRACK_TYPE),
                sub => sub.SubstType.ToString(),
                true);

            var timePeriodFilter = new FilterPeriodRange<Substrate>(
                nameof(SubstrateTrackingRessources.SUB_TRACK_FILTER_PERIOD),
                GetPeriod)
            {
                UseHoursMinutesSeconds = true
            };
            SubstratesSource.Filter.Add(timePeriodFilter);

            var locationFilter = new FilterCollection<Substrate, string>(
                nameof(SubstrateTrackingRessources.SUB_TRACK_FILTER_LOCID),
                GetLocationIds,
                GetLocationId);
            SubstratesSource.Filter.Add(locationFilter);

            var wentByLocationFilter = new FilterCollectionForEnumerable<Substrate, string>(
                nameof(SubstrateTrackingRessources.SUB_TRACK_FILTER_WENTBY_LOC),
                GetLocationIds,
                GetDistinctLocationIds);
            SubstratesSource.Filter.Add(wentByLocationFilter);

            #endregion

            #region Locations

            LocationsSource = new DataTreeSource<IE39ObjectViewModel>(item => item is CarrierViewModel carrier
                ? carrier.Children
                : null)
            {
                AutoExpandOnNodeAdded = false
            };

            LocationsSource.Search.AddSearchDefinition(
                nameof(SubstrateTrackingRessources.SUB_TRACK_ID),
                location => location.Id,
                true);

            LocationsSource.Sort.AddSortDefinition(
                nameof(LocationViewModel.Id),
                node => node.Model.Id);

            #endregion

            ResetSubstrates();
            ResetLocations();

            Commands.Add(new BusinessPanelCheckToggleCommand(
                nameof(SubstrateTrackingRessources.SUB_TRACK_COMPARE),
                () =>
                {
                    SelectionMode = SelectionMode.Single;
                    SelectedSubstrate = null;
                    SelectedSubstrateList = null;
                },
                () =>
                {
                    SelectionMode = SelectionMode.Multiple;
                    SelectedSubstrate = null;
                    SelectedSubstrateList = null;
                },
                PathIcon.Analysis)
            {
                IsChecked = false
            });
        }

        #endregion

        #region Properties

        public static IE90Standard E90Standard => App.ControllerInstance.GemController.E90Std;

        public static IE87Standard E87Standard => App.ControllerInstance.GemController.E87Std;

        public DataTableSource<Substrate> SubstratesSource { get; }

        public DataTreeSource<IE39ObjectViewModel> LocationsSource { get; }

        private SelectionMode _selectionMode = SelectionMode.Single;

        public SelectionMode SelectionMode
        {
            get => _selectionMode;
            set => SetAndRaiseIfChanged(ref _selectionMode, value);
        }

        private Substrate _selectedSubstrate;

        public Substrate SelectedSubstrate
        {
            get => _selectedSubstrate;
            set
            {
                if (SetAndRaiseIfChanged(ref _selectedSubstrate, value))
                {
                    OnSelectedSubstrateChanged();
                }
            }
        }

        private List<Substrate> _selectedSubstrates = new();

        public List<Substrate> SelectedSubstrateList
        {
            get => _selectedSubstrates;
            set => SetAndRaiseIfChanged(ref _selectedSubstrates, value);
        }

        private string _historyTitle;

        public string HistoryTitle
        {
            get => _historyTitle;
            private set => SetAndRaiseIfChanged(ref _historyTitle, value);
        }

        private HistoryChartType _chartView = HistoryChartType.Condensed;

        public HistoryChartType ChartView
        {
            get => _chartView;
            set => SetAndRaiseIfChanged(ref _chartView, value);
        }

        #endregion

        #region Private methods

        #region Helpers

        private static string GetCarrierId(string locationId)
        {
            foreach (var carrierId in E87Standard.Carriers.Select(carrier => carrier.ObjID))
            {
                var substrateLocations = E90Standard.GetCarrierSubstrateLocations(carrierId);
                if (substrateLocations.Any(s => s.ObjID.Equals(locationId)))
                {
                    return carrierId;
                }
            }

            return null;
        }

        #endregion

        #region Substrates

        public void ResetSubstrates() => SubstratesSource.Reset(E90Standard.Substrates);

        #endregion

        #region Locations

        public void ResetLocations()
        {
            _locations.Clear();

            // Add batch locations at root
            var batchLocations = E90Standard.BatchLocations
                .Select(batch => new LocationViewModel(batch.ObjID, batch.BatchLocState))
                .ToList();

            // Add all batch locations to indexed dictionary
            foreach (var batchLocation in batchLocations)
            {
                _locations.Add(batchLocation.Id, batchLocation);
            }

            LocationsSource.Reset(batchLocations);

            foreach (var location in E90Standard.SubstrateLocations)
            {
                AddLocation(location.ObjID, location.SubstLocState);
            }
        }

        private void UpdateLocation(string locationId, LocationState state)
        {
            if (!_locations.TryGetValue(locationId, out var location)
                || location is not LocationViewModel locationViewModel)
            {
                return;
            }

            locationViewModel.State = state;

            var carrierViewModel = _locations.Values.OfType<CarrierViewModel>()
                .FirstOrDefault(carrier => carrier.Children.Contains(locationViewModel));

            carrierViewModel?.RaiseStateChanged();
        }

        private void AddLocation(string locationId, LocationState locationState)
        {
            var locationViewModel = new LocationViewModel(locationId, locationState);
            _locations.Add(locationId, locationViewModel);

            // If no associated carrier, add to the root
            var carrierId = GetCarrierId(locationId);
            if (string.IsNullOrWhiteSpace(carrierId))
            {
                LocationsSource.Add(locationViewModel);
                return;
            }

            // Get related carrier treeNode or create it if it does not exist
            TreeNode<IE39ObjectViewModel> carrierNode;
            if (_locations.TryGetValue(carrierId, out var carrierModel))
            {
                carrierNode = LocationsSource.GetTreeNode(carrierModel);
            }
            else
            {
                carrierNode = AddCarrier(carrierId);
                carrierModel = carrierNode.Model;
            }

            if (carrierModel is CarrierViewModel carrier)
            {
                carrier.Children.Add(locationViewModel);
                carrier.RaiseStateChanged();
            }

            LocationsSource.Add(locationViewModel, carrierNode);
        }

        private void RemoveLocation(string locationId)
        {
            if (!_locations.TryGetValue(locationId, out var iLocation) ||
                iLocation is not LocationViewModel location)
            {
                return;
            }

            var node = LocationsSource.GetTreeNode(location);
            var parent = node.Parent;

            LocationsSource.Remove(location);

            if (parent is { Model: CarrierViewModel carrier })
            {
                // Remove from parent viewModel
                carrier.Children.Remove(location);

                // Auto remove empty carrier
                if (!parent.HasChild)
                {
                    LocationsSource.Remove(parent);
                    _locations.Remove(parent.Model.Id);
                }
                else
                {
                    carrier.RaiseStateChanged();
                }
            }

            _locations.Remove(locationId);
        }

        #endregion

        #region Carriers

        private TreeNode<IE39ObjectViewModel> AddCarrier(string carrierId)
        {
            var carrier = new CarrierViewModel(carrierId);
            _locations.Add(carrierId, carrier);
            return LocationsSource.Add(carrier);
        }

        #endregion

        private void OnSelectedSubstrateChanged()
        {
            DateTime selectedStartDate;
            DateTime selectedEndDate;

            if (_selectedSubstrate?.SubstHistory?.Count > 0)
            {
                selectedStartDate = _selectedSubstrate.SubstHistory[0].TimeIn;
                selectedEndDate = _selectedSubstrate.SubstHistory[0].TimeOut ?? DateTime.Now;
            }
            else
            {
                selectedStartDate = DateTime.Now;
                selectedEndDate = DateTime.Now;
            }

            TimeSpan totalTime = selectedEndDate - selectedStartDate;
            var hours = $"{totalTime.Hours} {SubstrateTrackingRessources.SUB_TRACK_HOUR} ";
            var min = $"{totalTime.Minutes} {SubstrateTrackingRessources.SUB_TRACK_MIN} ";
            var sec = $"{totalTime.Seconds} {SubstrateTrackingRessources.SUB_TRACK_SECOND}";
            var timeText = "";
            if (totalTime.Hours > 0)
            {
                timeText += hours;
            }

            if (totalTime.Minutes > 0)
            {
                timeText += min;
            }

            if (totalTime.Seconds > 0)
            {
                timeText += sec;
            }

            HistoryTitle =
                $"-  {_selectedSubstrate?.ObjID}  -  {SubstrateTrackingRessources.SUB_TRACK_START}: {selectedStartDate.ToString("G", CultureInfo.CurrentUICulture)}  -  {SubstrateTrackingRessources.SUB_TRACK_END}: {selectedEndDate.ToString("G", CultureInfo.CurrentUICulture)}  -  {SubstrateTrackingRessources.SUB_TRACK_TOTAL}: {timeText}";
        }

        #endregion

        #region Filters

        private static (DateTime? startTime, DateTime? endTime) GetPeriod(Substrate substrate)
        {
            if (substrate.SubstHistory is not { Count: > 0 })
            {
                return (null, null);
            }

            var startHist = substrate.SubstHistory[0].TimeIn;
            var endHist = substrate.SubstHistory.Last().TimeOut ?? DateTime.Now;

            return (startHist, endHist);
        }

        private static string GetLocationId(Substrate substrate)
        {
            if (!string.IsNullOrEmpty(substrate.BatchLocID))
            {
                return substrate.BatchLocID;
            }

            if (!string.IsNullOrEmpty(substrate.SubstLocID))
            {
                return substrate.SubstLocID;
            }

            return string.Empty;
        }

        private static IEnumerable<string> GetLocationIds()
        {
            var loc = new List<MaterialLocation>();
            loc.AddRange(E90Standard.SubstrateLocations);
            loc.AddRange(E90Standard.BatchLocations);
            return loc.Select(materialLocation => materialLocation.ObjID);
        }

        private static IEnumerable<string> GetDistinctLocationIds(Substrate substrate)
        {
            return substrate.SubstHistory.Select(hist => hist.LocationId).Distinct();
        }

        #endregion

        #region Event handlers

        private void OnSubstrateStateChanged(object sender, SubstrateEventArgs e)
        {
            var substrate = SubstratesSource.FirstOrDefault(s => s.ObjID.Equals(e.Substrate.ObjID));
            SubstratesSource.Remove(substrate);
            SubstratesSource.Add(e.Substrate);
        }

        #region Substrates

        private void OnSubstrateInstantiated(object sender, SubstrateEventArgs e)
        {
            SubstratesSource.Add(e.Substrate);
        }

        private void OnSubstrateDisposed(object sender, SubstrateEventArgs e)
        {
            var substrate = SubstratesSource.FirstOrDefault(s => s.ObjID.Equals(e.Substrate.ObjID));
            SubstratesSource.Remove(substrate);
        }

        #endregion

        #region Location Instanciated

        private void OnBatchLocationInstantiated(object sender, BatchLocationEventArgs e)
        {
            AddLocation(e.BatchLocation.ObjID, e.BatchLocation.BatchLocState);
        }

        private void OnSubstrateLocationInstantiated(object sender, SubstrateLocationEventArgs e)
        {
            AddLocation(e.SubstrateLocation.ObjID, e.SubstrateLocation.SubstLocState);
        }

        #endregion

        #region Location Disposed

        private void OnBatchLocationDisposed(object sender, BatchLocationEventArgs e)
        {
            RemoveLocation(e.BatchLocation.ObjID);
        }

        private void OnSubstrateLocationDisposed(object sender, SubstrateLocationEventArgs e)
        {
            RemoveLocation(e.SubstrateLocation.ObjID);
        }

        #endregion

        #region Location State Changed

        private void OnBatchLocationStateChanged(object sender, BatchLocationEventArgs e)
        {
            UpdateLocation(e.BatchLocation.ObjID, e.BatchLocation.BatchLocState);
        }

        private void OnSubstrateLocationStateChanged(object sender, SubstrateLocationEventArgs e)
        {
            UpdateLocation(e.SubstrateLocation.ObjID, e.SubstrateLocation.SubstLocState);
        }

        #endregion

        #endregion

        #region Overrides of BusinessPanel

        public override void OnHide()
        {
            base.OnHide();
            SelectedSubstrateList = new List<Substrate>();
        }

        public override void OnSetup()
        {
            base.OnSetup();

            if (E90Standard == null) return;

            E90Standard.SubstrateInstantiated += OnSubstrateInstantiated;
            E90Standard.SubstrateDisposed += OnSubstrateDisposed;
            E90Standard.SubstrateProcessingStateChanged += OnSubstrateStateChanged;
            E90Standard.SubstrateReadingStateChanged += OnSubstrateStateChanged;
            E90Standard.SubstrateTransportStateChanged += OnSubstrateStateChanged;

            E90Standard.BatchLocationInstantiated += OnBatchLocationInstantiated;
            E90Standard.BatchLocationDisposed += OnBatchLocationDisposed;
            E90Standard.BatchLocationStateChanged += OnBatchLocationStateChanged;

            E90Standard.SubstrateLocationInstantiated += OnSubstrateLocationInstantiated;
            E90Standard.SubstrateLocationDisposed += OnSubstrateLocationDisposed;
            E90Standard.SubstrateLocationStateChanged += OnSubstrateLocationStateChanged;
        }

        protected override void Dispose(bool disposing)
        {
            if (E90Standard != null && disposing)
            {
                E90Standard.SubstrateInstantiated -= OnSubstrateInstantiated;
                E90Standard.SubstrateDisposed -= OnSubstrateDisposed;
                E90Standard.SubstrateProcessingStateChanged -= OnSubstrateStateChanged;
                E90Standard.SubstrateReadingStateChanged -= OnSubstrateStateChanged;
                E90Standard.SubstrateTransportStateChanged -= OnSubstrateStateChanged;

                E90Standard.BatchLocationInstantiated -= OnBatchLocationInstantiated;
                E90Standard.BatchLocationDisposed -= OnBatchLocationDisposed;
                E90Standard.BatchLocationStateChanged -= OnBatchLocationStateChanged;

                E90Standard.SubstrateLocationInstantiated -= OnSubstrateLocationInstantiated;
                E90Standard.SubstrateLocationDisposed -= OnSubstrateLocationDisposed;
                E90Standard.SubstrateLocationStateChanged -= OnSubstrateLocationStateChanged;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
