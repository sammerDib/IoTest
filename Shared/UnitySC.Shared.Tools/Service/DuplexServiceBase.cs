using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Collection;

namespace UnitySC.Shared.Tools.Service
{
    public class DuplexServiceBase<T> : BaseService
    {
        private readonly object _lock = new object();

        /// <summary>
        ///  List des clients ayant souscrit
        /// </summary>
        private readonly List<T> _serviceCallbackList = new List<T>();

        public DuplexServiceBase(ILogger logger, ExceptionType serviceExceptionType) : base(logger, serviceExceptionType)
        {
        }

        public void Subscribe()
        {
            var serviceCallback = OperationContext.Current.GetCallbackChannel<T>();
            Subscribe(serviceCallback);
        }

        public void Subscribe(T serviceCallback)
        {
            lock (_lock)
                _serviceCallbackList.TryAdd(serviceCallback);
        }

        public void Unsubscribe()
        {
            var serviceCallback = OperationContext.Current.GetCallbackChannel<T>();
            Unsubscribe(serviceCallback);
        }

        public void Unsubscribe(T serviceCallback)
        {
            lock (_lock)
                _serviceCallbackList.Remove(serviceCallback);
        }

        protected void InvokeCallback(Action<T> reportOnClient)
        {
            lock (_lock)
            {
                var machineStatusServiceCallbackToRemove = new List<T>();

                // Execute callback for each subscribed client
                _serviceCallbackList.ForEach(callback =>
                {
                    if (((ICommunicationObject)callback).State == CommunicationState.Opened)
                        reportOnClient(callback);
                    else
                        machineStatusServiceCallbackToRemove.Add(callback);
                });

                // Remove disconnected client callbacks
                _serviceCallbackList.RemoveRange(machineStatusServiceCallbackToRemove);
            }
        }

        public int GetNbClientsConnected()
        {
            return _serviceCallbackList.Count(callback => ((ICommunicationObject)callback).State == CommunicationState.Opened);
        }
    }
}
