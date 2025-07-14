using System;
using System.Collections.Generic;

namespace UnitySC.Shared.Tools
{
    public class NotificationTemplate<T>
    {
        #region Private Members

        private readonly object _lock = new object();
        private List<T> _listenerList = new List<T>();
        private int _subscriberCount = 0;

        #endregion Private Members

        public void InvokeCallback(Action<T> pNotifyListener)
        {
            if (!IsThereSubscribers) return;
            lock (_lock)
            {
                var _ListenerToDeleteList = new List<T>();
                // Execute callback for each subscribed listener
                _listenerList.ForEach(callback =>
                {
                    if (callback != null)
                        pNotifyListener(callback);
                    else
                        _ListenerToDeleteList.Add(callback);
                });
                for (int i = 0; i < _ListenerToDeleteList.Count; i++)
                {
                    _listenerList.Remove(_ListenerToDeleteList[i]);
                }
            }
        }

        public List<T> SubscribersList { get => _listenerList; set => _listenerList = value; }

        public bool IsThereSubscribers { get => _subscriberCount > 0; }

        public bool Register(T serviceCallback)
        {
            if (serviceCallback == null) return false;
            lock (_lock)
            {
                bool alreadyPresent = _listenerList.Contains(serviceCallback);

                if (alreadyPresent)
                {
                    return false;
                }
                else
                {
                    _listenerList.Add(serviceCallback);
                    _subscriberCount = _listenerList.Count;
                    return true;
                }
            }
        }

        public void Unregister(T serviceCallback)
        {
            lock (_lock)
            {
                if (_listenerList.Contains(serviceCallback))
                    _listenerList.Remove(serviceCallback);
                _subscriberCount = _listenerList.Count;
            }
        }

        public void UnregisterAll()
        {
            lock (_lock)
            {
                _listenerList.Clear();
                _subscriberCount = _listenerList.Count;
            }
        }
    }

    // NotificationTemplate with single listner
    public interface INotificationTemplate_SL<T>
    {
        void NotifySubscriber(Action<T> pNotifyListener);
        bool Register(T serviceCallback);
        void Unregister(T serviceCallback);
    }

    // NotificationTemplate with single listner
    public class NotificationTemplate_SL<T> : INotificationTemplate_SL<T>
    {
        #region Private Members

        private readonly object _lock = new object();
        private T _subscriber;

        #endregion Private Members

        public void NotifySubscriber(Action<T> pNotifySubscriber)
        {
            if (!IsThereSubscriber) return;
            lock (_lock)
            {
                if (_subscriber != null)
                    pNotifySubscriber(_subscriber);
            }
        }

        public T Subscriber { get => _subscriber; }

        public bool IsThereSubscriber { get => (_subscriber != null); }

        public bool Register(T serviceCallback)
        {
            if (serviceCallback == null) return false;
            lock (_lock)
            {
                bool alreadyPresent = (_subscriber != null);

                if (alreadyPresent)
                {
                    return false;
                }
                else
                {
                    _subscriber = serviceCallback;
                    return true;
                }
            }
        }

        public void Unregister(T serviceCallback)
        {
            lock (_lock)
            {
                if (_subscriber != null)
                    _subscriber = default(T);
            }
        }

    }
}