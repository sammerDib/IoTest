using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Collection
{
    public class TimedBindingList<T> : BindingList<T>, IDisposable
    {
        private const int TimedBindingListInterval = 200;

        // The lock object used to prevent multi access of the instance.
        private readonly object _locker = new object();

        private readonly LinkedList<T> _addCache = new LinkedList<T>();

        private readonly LinkedList<T> _removeCache = new LinkedList<T>();

        private readonly DispatcherTimer _timer;
        private bool _disposedValue;

        public TimedBindingList()
        {
            AllowEdit = true;
            AllowNew = true;
            AllowRemove = true;

            _timer = new DispatcherTimer(DispatcherPriority.Render, Application.Current.Dispatcher)
            {
                Interval = TimeSpan.FromMilliseconds(TimedBindingListInterval)
            };

            _timer.Tick += TimerTick;
            _timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            lock (_locker)
            {
                foreach (var item in _removeCache)
                {
                    Remove(item);
                }

                _removeCache.Clear();

                foreach (var item in _addCache)
                {
                    Add(item);
                }

                _addCache.Clear();
            }
        }

        /// <summary>
        /// Add an item to the cache, it will be added to the collection on the next refresh tick.
        /// </summary>
        public void AddInCacheList(T item)
        {
            lock (_locker)
            {
                _addCache.AddLast(item);
            }
        }

        /// <summary>
        /// Reset the entire content of the list with the content specified in parameter.
        /// </summary>
        public void Reset(IEnumerable<T> items)
        {
            lock (_locker)
            {
                RaiseListChangedEvents = false;

                _addCache.Clear();
                _removeCache.Clear();
                Clear();

                foreach (var item in items)
                {
                    Add(item);
                }

                RaiseListChangedEvents = true;
                ResetBindings();
            }
        }

        /// <summary>
        /// Add an item to the remove cache, it will be removed to the collection on the next refresh tick.
        /// </summary>
        public void RemoveFromCache(T item)
        {
            lock (_locker)
            {
                _addCache.Remove(item);
                _removeCache.AddLast(item);
            }
        }

        /// <summary>
        /// Clear the entire collection and caches.
        /// </summary>
        public void ClearAll()
        {
            lock (_locker)
            {
                _addCache.Clear();
                _removeCache.Clear();

                Clear();
            }
        }

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _timer.Tick -= TimerTick;
                    _timer.Stop();
                    Application.Current.Dispatcher.Invoke(ClearAll);
                }
                
                _disposedValue = true;
            }
        }
        
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable
    }
}
