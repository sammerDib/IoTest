using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

using Agileo.GUI.Commands;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation.CarrierConfigurations.Models;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation.CarrierConfigurations.ViewModels
{
    /// <summary>
    /// represent the View Model of a Configuration for a carrier to simulate
    /// </summary>
    [Serializable]
    public class SimulatorCarrierConfigurationViewModel : INotifyPropertyChanged
    {
        #region Variables

        private CarrierConfiguration _model;

        //save slots configuration to the data grid
        Collection<WaferCollection> _collection = new Collection<WaferCollection>();

        #endregion Variables

        #region Constructors
        /// <summary>
        /// Constructor by default
        /// </summary>
        /// <param name="model"></param>
        public SimulatorCarrierConfigurationViewModel(CarrierConfiguration model)
        {
            _model = model;
        }

        #endregion Constructors

        #region ICommands

        #region CorrectAll
        /// <summary>
        /// ICommand to gets or sets value CorrectCarrier button
        /// </summary>
        public ICommand CorrectCarrierCommand
        {
            get
            {
                return new DelegateCommand<string>(_ => CorrectAll());
            }
        }
        /// <summary>
        /// Set SlotState of carrier's slots to Correct state
        /// </summary>
        public void CorrectAll()
        {
            foreach (WaferCollection carrier in _collection)
                carrier.State = Material.SlotState.HasWafer;
        }
        #endregion CorrectAll

        #region EmptyAll
        /// <summary>
        /// ICommand to gets or sets value EmptyCarrier button
        /// </summary>
        public ICommand EmptyCarrierCommand
        {
            get
            {
                return new DelegateCommand<string>(_ => EmptyAll());
            }
        }
        /// <summary>
        /// Set SlotState of carrier's slots to empty state
        /// </summary>
        public void EmptyAll()
        {
            foreach (WaferCollection carrier in _collection)
                carrier.State = Material.SlotState.NoWafer;
        }
        #endregion EmptyAll

        #region RandomCorrect
        /// <summary>
        /// ICommand to gets or sets value RandomCorrect button
        /// </summary>
        public ICommand RandomCorrectCarrierCommand
        {
            get
            {
                return new DelegateCommand<string>(_ => RandomCorrect());
            }
        }
        /// <summary>
        /// Set SlotState of carrier's slots to Correct or empty state
        /// </summary>
        public void RandomCorrect()
        {
            Random rand = new Random();
            foreach (WaferCollection wafer in _collection)
            {
                string[] _enum = Enum.GetNames(typeof(Abstractions.Material.SlotState));
                int randomEnum = rand.Next(0, 3);
                var ret = (Abstractions.Material.SlotState)Enum.Parse(typeof(Abstractions.Material.SlotState), _enum[randomEnum]);
                while ((ret != Material.SlotState.HasWafer) && (ret != Material.SlotState.NoWafer))
                {
                    randomEnum = rand.Next(0, 3);
                    ret = (Abstractions.Material.SlotState)Enum.Parse(typeof(Abstractions.Material.SlotState), _enum[randomEnum]);
                }

                wafer.State = ret;
            }
        }
        #endregion RandomCorrect

        #region RandomIncorrect
        /// <summary>
        /// ICommand to gets or sets value RandomIncorrect button
        /// </summary>
        public ICommand RandomIncorrectCarrierCommand
        {
            get
            {
                return new DelegateCommand<string>(_ => RandomIncorrect());
            }
        }
        /// <summary>
        /// Set SlotState of carrier's slots to Incorrects state
        /// </summary>
        public void RandomIncorrect()
        {
            Random rand = new Random();
            foreach (WaferCollection wafer in _collection)
            {
                string[] _enum = Enum.GetNames(typeof(Abstractions.Material.SlotState));
                int randomEnum = rand.Next(0, 6);
                var ret = (Abstractions.Material.SlotState)Enum.Parse(typeof(Abstractions.Material.SlotState), _enum[randomEnum]);
                while ((ret != Material.SlotState.HasWafer) && (ret != Material.SlotState.CrossWafer) && (ret != Material.SlotState.DoubleWafer))
                {
                    randomEnum = rand.Next(0, 6);
                    ret = (Abstractions.Material.SlotState)Enum.Parse(typeof(Abstractions.Material.SlotState), _enum[randomEnum]);
                }

                wafer.State = ret;
            }
        }
        #endregion RandomIncorrect

        #endregion ICommands

        #region InfoPads

        #region Sum
        // ========================== >      Sum
        /// <summary>
        /// Sum of InfoPad
        /// </summary>
        public byte InfoSomme
        {
            get { return Model.InfoSomme; }
            set { _model.InfoSomme = value; OnPropertyChanged(nameof(InfoSomme)); }
        }
        #endregion Sum

        #region InfoPad7
        // ========================== >      7
        /// <summary>
        /// ICommand to gets or sets value of the specified InfoPad
        /// </summary>
        public ICommand InfoPad7
        {
            get { return new DelegateCommand<string>(_ => Info7()); }
        }
        /// <summary>
        /// Add or delete the specified value to InfoSomme
        /// </summary>
        public void Info7()
        {
            if (Model.InfoChecked[7])
                InfoSomme += Convert.ToByte(Math.Pow(2, 7));
            else
                InfoSomme -= Convert.ToByte(Math.Pow(2, 7));
        }
        /// <summary>
        /// Gets or sets values of the specified InfoPad
        /// </summary>
        public bool Check7
        {
            get { return Model.InfoChecked[7]; }
            set { Model.InfoChecked[7] = value; }
        }
        #endregion InfoPad7

        #region InfoPad6
        // ========================== >      6
        /// <summary>
        /// ICommand to gets or sets value of the specified InfoPad
        /// </summary>
        public ICommand InfoPad6
        {
            get { return new DelegateCommand<string>(_ => Info6()); }
        }
        /// <summary>
        /// Add or delete the specified value to InfoSomme
        /// </summary>
        public void Info6()
        {
            if (Model.InfoChecked[6])
                InfoSomme += Convert.ToByte(Math.Pow(2, 6));
            else
                InfoSomme -= Convert.ToByte(Math.Pow(2, 6));
        }
        /// <summary>
        /// Gets or sets values of the specified InfoPad
        /// </summary>
        public bool Check6
        {
            get { return Model.InfoChecked[6]; }
            set { Model.InfoChecked[6] = value; }
        }
        #endregion InfoPad6

        #region InfoPad5
        // ========================== >      5
        /// <summary>
        /// ICommand to gets or sets value of the specified InfoPad
        /// </summary>
        public ICommand InfoPad5
        {
            get { return new DelegateCommand<string>(_ => Info5()); }
        }

        /// <summary>
        /// Add or delete the specified value to InfoSomme
        /// </summary>
        public void Info5()
        {
            if (Model.InfoChecked[5])
                InfoSomme += Convert.ToByte(Math.Pow(2, 5));
            else
                InfoSomme -= Convert.ToByte(Math.Pow(2, 5));
        }
        /// <summary>
        /// Gets or sets values of the specified InfoPad
        /// </summary>
        public bool Check5
        {
            get { return Model.InfoChecked[5]; }
            set { Model.InfoChecked[5] = value; }
        }
        #endregion InfoPad5

        #region InfoPad4
        // ========================== >      4
        /// <summary>
        /// ICommand to gets or sets value of the specified InfoPad
        /// </summary>
        public ICommand InfoPad4
        {
            get { return new DelegateCommand<string>(_ => Info4()); }
        }

        /// <summary>
        /// Add or delete the specified value to InfoSomme
        /// </summary>
        public void Info4()
        {
            if (Model.InfoChecked[4])
                InfoSomme += Convert.ToByte(Math.Pow(2, 4));
            else
                InfoSomme -= Convert.ToByte(Math.Pow(2, 4));
        }
        /// <summary>
        /// Gets or sets values of the specified InfoPad
        /// </summary>
        public bool Check4
        {
            get { return Model.InfoChecked[4]; }
            set { Model.InfoChecked[4] = value; }
        }
        #endregion InfoPad4

        #region InfoPad3
        // ========================== >      3
        /// <summary>
        /// ICommand to gets or sets value of the specified InfoPad
        /// </summary>
        public ICommand InfoPad3
        {
            get { return new DelegateCommand<string>(_ => Info3()); }
        }

        /// <summary>
        /// Add or delete the specified value to InfoSomme
        /// </summary>
        public void Info3()
        {
            if (Model.InfoChecked[3])
                InfoSomme += Convert.ToByte(Math.Pow(2, 3));
            else
                InfoSomme -= Convert.ToByte(Math.Pow(2, 3));
        }
        /// <summary>
        /// Gets or sets values of the specified InfoPad
        /// </summary>
        public bool Check3
        {
            get { return Model.InfoChecked[3]; }
            set { Model.InfoChecked[3] = value; }
        }
        #endregion InfoPad3

        #region InfoPad2
        // ========================== >      2
        /// <summary>
        /// ICommand to gets or sets value of the specified InfoPad
        /// </summary>
        public ICommand InfoPad2
        {
            get { return new DelegateCommand<string>(_ => Info2()); }
        }

        /// <summary>
        /// Add or delete the specified value to InfoSomme
        /// </summary>
        public void Info2()
        {
            if (Model.InfoChecked[2])
                InfoSomme += Convert.ToByte(Math.Pow(2, 2));
            else
                InfoSomme -= Convert.ToByte(Math.Pow(2, 2));
        }
        /// <summary>
        /// Gets or sets values of the specified InfoPad
        /// </summary>
        public bool Check2
        {
            get { return Model.InfoChecked[2]; }
            set { Model.InfoChecked[2] = value; }
        }
        #endregion InfoPad2

        #region InfoPad1
        // ========================== >      1
        /// <summary>
        /// ICommand to gets or sets value of the specified InfoPad
        /// </summary>
        public ICommand InfoPad1
        {
            get { return new DelegateCommand<string>(_ => Info1()); }
        }

        /// <summary>
        /// Add or delete the specified value to InfoSomme
        /// </summary>
        public void Info1()
        {
            if (Model.InfoChecked[1])
                InfoSomme += Convert.ToByte(Math.Pow(2, 1));
            else
                InfoSomme -= Convert.ToByte(Math.Pow(2, 1));
        }
        /// <summary>
        /// Gets or sets values of the specified InfoPad
        /// </summary>
        public bool Check1
        {
            get { return Model.InfoChecked[1]; }
            set { Model.InfoChecked[1] = value; }
        }
        #endregion InfoPad1

        #region InfoPad0
        // ========================== >      0
        /// <summary>
        /// ICommand to gets or sets value of the specified InfoPad
        /// </summary>
        public ICommand InfoPad0
        {
            get { return new DelegateCommand<string>(_ => Info0()); }
        }

        /// <summary>
        /// Add or delete the specified value to InfoSomme
        /// </summary>
        public void Info0()
        {
            if (Model.InfoChecked[0])
                InfoSomme += Convert.ToByte(Math.Pow(2, 0));
            else
                InfoSomme -= Convert.ToByte(Math.Pow(2, 0));
        }
        /// <summary>
        /// Gets or sets values of the specified InfoPad
        /// </summary>
        public bool Check0
        {
            get { return Model.InfoChecked[0]; }
            set { Model.InfoChecked[0] = value; }
        }
        #endregion InfoPad0

        #endregion InfoPads

        #region Properties
        /// <summary>
        /// Gets the current Carrier Configuration
        /// </summary>
        public CarrierConfiguration Model
        {
            get { return _model; }
        }

        /// <summary>
        /// Gets or sets the selected carrier ID
        /// </summary>
        public string CarrierId
        {
            get { return _model.Id; }
            set { _model.Id = value; OnPropertyChanged(nameof(CarrierId)); }
        }

        /// <summary>
        ///  Gets or sets the selected carrier type
        /// </summary>
        public string CarrierType
        {
            get { return _model.Type.ToString(); }
            set
            {
                SampleDimension result;
                if (Enum.TryParse(value, out result))
                    _model.Type = result;
                OnPropertyChanged(nameof(CarrierType));
            }
        }

        /// <summary>
        /// Gets the list of different possible SlotState
        /// </summary>
        public IList<string> ListStates
        {
            get
            {
                IList<string> list = new List<string>();
                list.Add(Material.SlotState.HasWafer.ToString());
                list.Add(Material.SlotState.CrossWafer.ToString());
                list.Add(Material.SlotState.DoubleWafer.ToString());
                list.Add(Material.SlotState.FrontBow.ToString());
                list.Add(Material.SlotState.NoWafer.ToString());
                return list;
            }
        }

        /// <summary>
        /// Gets the different slots configuration for the selected carrier
        /// </summary>
        public Collection<WaferCollection> Definitions
        {
            get
            {
                _collection.Clear();
                for (int i = Convert.ToInt32(_model.Capacity) - 1; i >= 0; --i)
                {
                    var col = new WaferCollection
                    {
                        Index = _model.IndexSlots[i],
                        State = _model.MappingTable[i],
                        Scribe = _model.MappingScribe[i],
                    };
                    col.PropertyChanged += col_PropertyChanged;
                    _collection.Add(col);
                }

                return _collection;
            }
        }
        #endregion Properties

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        void col_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var carrier = (sender as WaferCollection);
            if (carrier == null)
                return;
            //Update configuration of the current carrierdata
            Model.MappingTable[carrier.Index - 1] = carrier.State;
            Model.MappingScribe[carrier.Index - 1] = carrier.Scribe;
        }
        #endregion INotifyPropertyChanged
    }

    #region Class WaferCollection
    /// <summary>
    /// Class to store the different slots
    /// </summary>
    public class WaferCollection : INotifyPropertyChanged
    {
        private int _index;
        private string _scribe;
        private Abstractions.Material.SlotState _state;

        /// <summary>
        /// Slot's index
        /// </summary>
        public int Index
        {
            get { return _index; }

            set { _index = value; OnPropertyChanged(nameof(Index)); }
        }

        /// <summary>
        /// Slot's SlotState
        /// </summary>
        public Abstractions.Material.SlotState State
        {
            get { return _state; }

            set { _state = value; OnPropertyChanged(nameof(State)); }
        }

        /// <summary>
        /// Slot's scribes
        /// </summary>
        public string Scribe
        {
            get { return _scribe; }

            set { _scribe = value; OnPropertyChanged(nameof(Scribe)); }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        #endregion INotifyPropertyChanged
    }
    #endregion Class WaferCollection
}
