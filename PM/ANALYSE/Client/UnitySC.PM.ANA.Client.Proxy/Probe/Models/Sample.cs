using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.Service.Interface;

namespace UnitySC.PM.ANA.Client.Proxy.Probe
{
    public class Sample : ObservableObject, IProbeSample
    {
        public string Name { get; set; }
        public string Info { get; set; }

        private ObservableCollection<SampleLayer> _observableLayers;

        public ObservableCollection<SampleLayer> ObservableLayers
        {
            get
            {
                if (_observableLayers == null)
                    _observableLayers = new ObservableCollection<SampleLayer>();
                return _observableLayers;
            }
            set
            {
                _observableLayers = value;
                OnPropertyChanged();
            }
        }

        public List<ProbeSampleLayer> Layers
        {
            get
            {
                if (ObservableLayers != null)
                {
                    List<ProbeSampleLayer> layers = new List<ProbeSampleLayer>();
                    foreach (var layer in ObservableLayers)
                    {
                        layers.Add(new ProbeSampleLayer(layer.Thickness, layer.Tolerance, layer.RefractionIndex, layer.Type));
                    }
                    return layers.ToList();
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    List<SampleLayer> layers = new List<SampleLayer>();
                    foreach (var layer in value)
                    {
                        layers.Add(new SampleLayer(layer.Thickness, layer.Tolerance, layer.RefractionIndex, layer.Type));
                    }
                    ObservableLayers = new ObservableCollection<SampleLayer>(layers.ToList());
                }
            }
        }
    }
}
