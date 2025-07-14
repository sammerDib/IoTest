using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ADCv9
{
    class ImageMemoryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public double Minimum { get { return 0; } }

        private double _maximum = 1;
        public double Maximum
        {
            get
            {
                // compute the next highest power of 2 of 32-bit
                long v = (long)_maximum;
                v--;
                for (int i = 0; i < 48; i++ )
                    v |= v >> i;
                v++;
                return v;
            }
            set
            {
                _maximum = value;
                NotifyPropertyChanged("Maximum");
                NotifyPropertyChanged("Text");
            }
        }

        public double _value = 0;
        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                if (_value > _maximum)
                    Maximum = _value;
                NotifyPropertyChanged("Value");
                NotifyPropertyChanged("Text");
            }
        }

        public string Text
        {
            get
            {
                string v = AdcTools.SizeStringFormatter.SizeSuffix((long)_value);
                string m = AdcTools.SizeStringFormatter.SizeSuffix((long)_maximum);
                return v + " / " + m;
            }
        }

    }
}
