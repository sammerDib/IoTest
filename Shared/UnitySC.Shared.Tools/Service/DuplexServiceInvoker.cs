using System;
using System.Linq.Expressions;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.Logger;

namespace UnitySC.Shared.Tools.Service
{
    /// <summary>
    /// Class used to invoke a duplex service.
    /// It manage errors, channel lifetime and connection retry
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public class DuplexServiceInvoker<TService> : ServiceInvoker<TService>
    {
        private readonly InstanceContext _instanceContext;
        private readonly Expression<Func<TService, Response<VoidResult>>> _subscribe;

        /// <summary>
        /// Duplex service invoker
        /// </summary>
        /// <param name="instanceContextForCallBack"></param>
        /// <param name="endPoint"></param>
        /// <param name="logger"></param>
        /// <param name="messenger"></param>
        /// <param name="subscribe">Subscribe is the function used to subscribe to the callback channel It will be called automatically when the connection will have to be re-created</param>
        /// <param name="customAddress">override Host and port defined in app.config</param>
        public DuplexServiceInvoker(InstanceContext instanceContextForCallBack, string endPoint, ILogger<TService> logger, IMessenger messenger = null, Expression<Func<TService, Response<VoidResult>>> subscribe = null, ServiceAddress customAddress = null) :
            base(endPoint, logger, messenger, customAddress)
        {
            _instanceContext = instanceContextForCallBack;

            _subscribe = subscribe;
        }

        protected override void InitConnexion()
        {
            _channelFactory = new DuplexChannelFactory<TService>(_instanceContext, _endPoint);
            CreateChannel();

            if (_subscribe != null)
            {
                int tempTryIndex = _currentTryIndex + 1;
                InvokeAndGetMessages(_subscribe);
                _currentTryIndex = tempTryIndex;
            }
        }
    }
}
