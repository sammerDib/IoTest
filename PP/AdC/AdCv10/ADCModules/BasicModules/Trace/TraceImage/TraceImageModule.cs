using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Controls;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

namespace BasicModules.Trace
{
    ///////////////////////////////////////////////////////////////////////
    // a Module to trace ImageBase objects
    ///////////////////////////////////////////////////////////////////////
    public class TraceImageModule : ModuleBase
    {
        // buffer circulaire pour stocker les traces
        //..........................................
        private CircularBuffer<TraceImage> _traceCircularBuffer = new CircularBuffer<TraceImage>(100);

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly IntParameter paramMaxStoredImages;


        //=================================================================
        // Constructeur
        //=================================================================
        public TraceImageModule(IModuleFactory facotory, int id, Recipe recipe)
            : base(facotory, id, recipe)
        {
            paramMaxStoredImages = new IntParameter(this, "MaxStoredImages");
            paramMaxStoredImages.Value = 100;
            if (_vm == null)
                _vm = new TraceImageViewModel(this);
        }

        //=================================================================
        // Usercontrol et ViewModel
        //=================================================================
        private static TraceImageViewModel _vm;
        private static UserControl _control = null;

        public override UserControl GetUI()
        {
            if (_control == null)
            {
                _control = new TraceImageUserControl();
                _control.DataContext = _vm;
            }
            return _control;
        }

        //=================================================================
        // 
        //=================================================================
        public IEnumerable<TraceImage> GetTraceList()
        {
            lock (_traceCircularBuffer)
            {
                return new List<TraceImage>(_traceCircularBuffer);
            }
        }

        public TraceImage GetLastTrace()
        {
            lock (_traceCircularBuffer)
            {
                TraceImage trace = _traceCircularBuffer.Tail();
                return trace;
            }
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            _vm.TraceImageModule = this;
            _vm.Ejected = false;
            _vm.Paused = false;
            _vm.ImageSourceList = new List<object>(Parents);
            _traceCircularBuffer = new CircularBuffer<TraceImage>(paramMaxStoredImages, allowOverflow: true);
        }


        //=================================================================
        // 
        //=================================================================
        private bool IsRecording()
        {
            return !_vm.Paused || _traceCircularBuffer.Size < _traceCircularBuffer.Capacity;
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("trace " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            lock (_traceCircularBuffer)
            {
                if (IsRecording())
                {
                    IImage image = (IImage)obj;
                    using (TraceImage trace = new TraceImage(parent, image))
                        _traceCircularBuffer.Put(trace);
                }
            }
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            if (_vm.Ejected)
            {
                // Nettoyage
                //..........
                _traceCircularBuffer.Clear();

                // Propagation du Stop aux modules fils
                //.....................................
                base.OnStopping(oldState);
            }
            else
            {
                PauseVM();
            }
        }

        //=================================================================
        // 
        //=================================================================
        public void Eject()
        {
            if (State >= eModuleState.Stopping)
            {
                // Nettoyage
                //..........
                _traceCircularBuffer.Clear();

                // Propagation du Stop aux modules fils
                //.....................................
                base.OnStopping(State);
            }
        }

        //=================================================================
        //
        //=================================================================
        public override void Abort()
        {
            EjectVM();
            base.Abort();
        }

        //=================================================================
        //
        //=================================================================
        private void PauseVM()
        {
            if (_control != null)
            {
                _control.Dispatcher.Invoke(new Action(() =>
                        _vm.Paused = true
                    ));
            }
        }

        private void EjectVM()
        {
            if (_control != null)
            {
                _control.Dispatcher.Invoke(new Action(() =>
                        _vm.Ejected = true
                    ));
            }
        }

    }
}
