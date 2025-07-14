using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.Logger;

namespace UnitySC.Shared.Tools.Service
{
    public class ServiceInvoker<TService>
    {
        protected ILogger _logger;
        protected string _endPoint;
        protected TService _serviceInstance;
        protected ChannelFactory<TService> _channelFactory;
        protected int _nbTry = 3;
        protected int _currentTryIndex = 0;
        protected IMessenger _messenger;
        protected ServiceAddress _customAddress;

        /// <summary>
        /// Service invoker
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="logger"></param>
        /// <param name="customAddress">override Host and port defined in app.config</param>
        public ServiceInvoker(string endPoint, ILogger<TService> logger, IMessenger messenger = null, ServiceAddress customAddress = null)
        {
            _endPoint = endPoint;
            _logger = logger;
            _messenger = messenger;
            _customAddress = customAddress;
        }

        protected virtual void InitConnexion()
        {
            _channelFactory = new ChannelFactory<TService>(_endPoint);
            CreateChannel();
        }

        protected void CreateChannel()
        {
            if (_customAddress is null)
                _serviceInstance = _channelFactory.CreateChannel();
            else
            {
                var uriBuilder = new UriBuilder(_channelFactory.Endpoint.Address.Uri)
                {
                    Host = _customAddress.Host,
                    Port = _customAddress.Port
                };
                var res = new EndpointAddress(uriBuilder.Uri);
                _serviceInstance = _channelFactory.CreateChannel(res);
            }
        }

        public void ResetConnexion()
        {
            _currentTryIndex = 0;
            InitConnexion();
        }

        /// <summary>
        /// Try to invoke a service method and manage the exception from the response.
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="serviceMethodExpr">Expression to call the service method</param>
        /// <returns>The TResult response if invoke mehtod doesn't thrown, null otherwise</returns>
        public virtual Response<TResult> TryInvokeAndGetMessages<TResult>(Expression<Func<TService, Response<TResult>>> serviceMethodExpr, bool silentLogger = true)
        {
            try
            {
                var completeResponse = InternalInvokeAndGetMessages<TResult>(serviceMethodExpr, silentLogger);
                return completeResponse;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Try to invoke an asynchronous service method and manage the exception from the response.
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="asyncServiceMethodExpr">Expression to call the service method</param>
        /// <returns>an awaitable Task&gt;TResult&lt; response if invoke mehtod doesn't throw, null otherwise</returns>
        public virtual async Task<Response<TResult>> TryInvokeAndGetMessagesAsync<TResult>(
            Expression<Func<TService, Task<Response<TResult>>>> asyncServiceMethodExpr, bool silentLogger = true)
        {
            try
            {
                var completeResponse = await InternalInvokeAndGetMessagesAsync(asyncServiceMethodExpr, silentLogger);
                return completeResponse;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Invoke a service method and manage the exception from the response.
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="serviceMethodExpr">Expression to call the service method</param>
        /// <returns>The TResult response if invoke mehtod doesn't thrown, exception otherwise</returns>
        public virtual Response<TResult> InvokeAndGetMessages<TResult>(Expression<Func<TService, Response<TResult>>> serviceMethodExpr, bool silentLogger = true)
        {
            var completeResponse = InternalInvokeAndGetMessages<TResult>(serviceMethodExpr, silentLogger);
            return completeResponse;
        }

        /// <summary>
        /// Invoke a service method and manage the exception from the response.
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="serviceMethodExpr">Expression to call the service method</param>
        /// <returns></returns>
        public virtual async Task<Response<TResult>> InvokeAndGetMessagesAsync<TResult>(Expression<Func<TService, Response<TResult>>> serviceMethodExpr, bool silentLogger = true)
        {
            var completeResponse = await Task.Run(() => InvokeAndGetMessages<TResult>(serviceMethodExpr, silentLogger));
            return completeResponse;
        }

        /// <summary>
        /// Invoke an asynchronous service method and manage the exception from the response.
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="asyncServiceMethodExpr">Expression to call the service method</param>
        /// <returns></returns>
        public virtual async Task<Response<TResult>> InvokeAndGetMessagesAsync<TResult>(
            Expression<Func<TService, Task<Response<TResult>>>> asyncServiceMethodExpr, bool silentLogger = true)
        {
            return await InternalInvokeAndGetMessagesAsync(asyncServiceMethodExpr, silentLogger);
        }

        /// <summary>
        /// Invoke a service method and manage the exception from the response.
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="serviceMethodExpr">Expression to call the service method</param>
        /// <returns></returns>
        public virtual TResult Invoke<TResult>(Expression<Func<TService, Response<TResult>>> serviceMethodExpr, bool silentLogger = true)
        {
            var completeResponse = InternalInvokeAndGetMessages<TResult>(serviceMethodExpr, silentLogger);
            return completeResponse.Result;
        }

        /// <summary>
        /// Invoke a service method and manage the exception from the response.
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="serviceMethodExpr">Expression to call the service method</param>
        /// <returns></returns>
        public virtual async Task<TResult> InvokeAsync<TResult>(Expression<Func<TService, Response<TResult>>> serviceMethodExpr, bool silentLogger = true)
        {
            var result = await Task.Run(() => Invoke<TResult>(serviceMethodExpr, silentLogger));
            return result;
        }

        /// <summary>
        /// Invoke an asynchronous service method and manage the exception from the response.
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="asyncServiceMethodExpr">Expression to call the service method</param>
        /// <returns></returns>
        public virtual async Task<TResult> InvokeAsync<TResult>(
            Expression<Func<TService, Task<Response<TResult>>>> asyncServiceMethodExpr, bool silentLogger = true)
        {
            var result = await InternalInvokeAndGetMessagesAsync(asyncServiceMethodExpr, silentLogger);
            return result.Result;
        }
        
        /// <summary>
        /// Invoke a service method and manage the exception from the response.
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="serviceMethodExpr">Expression to call the service method</param>
        /// <returns></returns>
        internal Response<TResult> InternalInvokeAndGetMessages<TResult>(Expression<Func<TService, Response<TResult>>> serviceMethodExpr, bool silentLogger)
        {
            ConnectOrReconnectIfFaulted();

            var tService = typeof(TService);
            string methodName = CheckServiceMethodExpressionAndGetMethodName(serviceMethodExpr, silentLogger, tService);
            
            var serviceMethodFunc = serviceMethodExpr.Compile();

            try
            {
                Response<TResult> response;
                using (var scope = new OperationContextScope((IClientChannel)_serviceInstance))
                {
                    response = serviceMethodFunc(_serviceInstance);
                }

                if (_logger != null)
                {
                    LogMessagesAndExceptions(silentLogger, response, tService, methodName);
                }

                if (response.Exception != null)
                {
                    AddExceptionMessagesToResponseMessages(response);
                }

                if (_messenger != null && response.Messages != null)
                {
                    SendMessagesUsingMessenger(response);
                }

                ManageException(response);
                _currentTryIndex = 0;

                return new Response<TResult>()
                {
                    Messages = response.Messages,
                    Result = response.Result
                };
            }
            catch (FaultException expt)
            {
                _currentTryIndex = 0;
                InitConnexion();
                throw new Exception($"An error occured when invoking {tService.Name}/{methodName}", expt);
            }
            catch (CommunicationException ce)
            {
                bool isChannelOpened = IsChannelOpened();
#if DEBUG
                _nbTry = 1;
#endif
                if (_currentTryIndex < _nbTry && !isChannelOpened)
                {
                    //Manage communication errors
                    _logger.Error($"An error occured when invoking {tService.Name}/{methodName}");
                    _logger.Error(ce.Message);
                    _logger.Information("New try: {TryNumber}", _currentTryIndex);

                    _currentTryIndex++;
                    // Need to wait until next try and avoid to consumme too much cpu
                    Thread.Sleep(2000);
                    //Try to re-create the channel
                    InitConnexion();
                    ////Send an event to recreate other channels like duplex
                    // Todo

                    return InvokeAndGetMessages<TResult>(serviceMethodExpr);
                }

                throw new Exception($"An error occured when invoking {tService.Name}/{methodName}", ce);
            }
        }

        /// <summary>
        /// Invoke an asynchronous service method and manage the exception from the response.
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="serviceMethodExpr">Expression to call the service method</param>
        /// <returns></returns>
        internal async Task<Response<TResult>> InternalInvokeAndGetMessagesAsync<TResult>(Expression<Func<TService, Task<Response<TResult>>>> asyncServiceMethodExpr, bool silentLogger)
        {
            ConnectOrReconnectIfFaulted();

            var tService = typeof(TService);
            string methodName =
                CheckAsyncServiceMethodExpressionAndGetMethodName(asyncServiceMethodExpr, silentLogger, tService);

            var serviceMethodFunc = asyncServiceMethodExpr.Compile();

            try
            {
                Response<TResult> response;
                using (var scope = new OperationContextScope((IClientChannel)_serviceInstance))
                {
                    response = await serviceMethodFunc(_serviceInstance);
                }

                if (_logger != null)
                {
                    LogMessagesAndExceptions(silentLogger, response, tService, methodName);
                }

                if (response.Exception != null)
                {
                    AddExceptionMessagesToResponseMessages(response);
                }

                if (_messenger != null && response.Messages != null)
                {
                    SendMessagesUsingMessenger(response);
                }

                ManageException(response);
                _currentTryIndex = 0;

                return new Response<TResult>()
                {
                    Messages = response.Messages,
                    Result = response.Result
                };
            }
            catch (FaultException expt)
            {
                _currentTryIndex = 0;
                InitConnexion();
                throw new Exception($"An error occured when invoking {tService.Name}/{methodName}", expt);
            }
            catch (CommunicationException ce)
            {
                bool isChannelOpened = IsChannelOpened();
#if DEBUG
                _nbTry = 1;
#endif
                if (_currentTryIndex < _nbTry && !isChannelOpened)
                {
                    //Manage communication errors
                    _logger.Error($"An error occured when invoking {tService.Name}/{methodName}");
                    _logger.Error(ce.Message);
                    _logger.Information("New try: {TryNumber}", _currentTryIndex);

                    _currentTryIndex++;
                    // Need to wait until next try and avoid to consumme too much cpu
                    Thread.Sleep(2000);
                    //Try to re-create the channel
                    InitConnexion();
                    ////Send an event to recreate other channels like duplex
                    // Todo

                    return await InvokeAndGetMessagesAsync(asyncServiceMethodExpr);
                }

                throw new Exception($"An error occured when invoking {tService.Name}/{methodName}", ce);
            }
        }
        

        private void SendMessagesUsingMessenger<TResult>(Response<TResult> response)
        {
            foreach (var message in response.Messages)
            {
                // System.Windows.Application.Current is null for the unit tests
                if (System.Windows.Application.Current == null)
                    Dispatcher.CurrentDispatcher.Invoke(() => _messenger.Send(message));
                else
                    System.Windows.Application.Current.Dispatcher.Invoke(() => _messenger.Send(message));
            }
        }

        private static void AddExceptionMessagesToResponseMessages<TResult>(Response<TResult> response)
        {
            if (response.Messages == null)
                response.Messages = new List<Message>();
            response.Messages.Add(new Message(MessageLevel.Error, response.Exception.Message, response.Exception.StackTrace));
        }

        private void LogMessagesAndExceptions<TResult>(bool silentLogger, Response<TResult> response, Type tService, string methodName)
        {
            var logMessageAndException = new StringBuilder();
            if (response.Messages != null && response.Messages.Any())
            {
                logMessageAndException.AppendLine();
                foreach (var message in response.Messages)
                {
                    logMessageAndException.AppendFormat("- [Message-{0}] {1} {2} {3}", message.Level, message.Source, message.UserContent, message.AdvancedContent);
                }
            }
            if (response.Exception != null)
            {
                logMessageAndException.AppendLine();
                logMessageAndException.AppendFormat("[{0}] {1}", response.Exception.Type, response.Exception.Message);
            }

            if (false == silentLogger)
            {
                // Not silent logger so log it

                if (_logger.IsVerboseEnabled())
                {
                    string verboseLog = string.Concat(string.Format("[Service-Response] {0}:{1} Result: {2}", tService.Name, methodName, response.Result), logMessageAndException.ToString());
                    _logger.Verbose(verboseLog);
                }
                else
                {
                    string debugLog = string.Concat(string.Format("[Service-Response] {0}:{1} ResultType: {2}", tService.Name, methodName, response.Result != null ? response.Result.GetType().Name : null), logMessageAndException.ToString());
                    _logger.Debug(debugLog);
                }
            }
        }

        private string CheckServiceMethodExpressionAndGetMethodName<TResult>(
            Expression<Func<TService, Response<TResult>>> serviceMethodExpr, bool silentLogger, Type tService)
        {
            bool isMethodCall = ExpressionHelper.IsMethodCall(serviceMethodExpr);
            if (!isMethodCall)
            {
                throw new Exception($"The expression must contains only one method call. Exemple: (service) => service.MyMethod(myRequest)");
            }

            string methodName = ExpressionHelper.GetMethodName(serviceMethodExpr);
            var methodArguments = ExpressionHelper.GetMethodArguments(serviceMethodExpr);

            if (methodArguments != null && methodArguments.Count() > 0)
            {
                //if (methodArguments.Count > 1)
                //{
                //    throw new Exception($"The service method {tService.Name}/{methodName} must have 0 to 1 parameter");
                //}

                object dataArgument = Expression.Lambda(methodArguments[0]).Compile().DynamicInvoke();
                if (false == silentLogger)
                {
                    // Not silent logger so log it
                    if (_logger.IsVerboseEnabled())
                    {
                        _logger.Verbose("[Service-Request] {ServiceName}:{ServiceMethodName} {@ServiceRequest}", tService.Name, methodName, dataArgument);
                    }
                    else
                    {
                        _logger.Debug("[Service-Request] {ServiceName}:{ServiceMethodName} RequestType {@ServiceRequestType}", tService.Name, methodName, dataArgument != null ? dataArgument.GetType().Name : string.Empty);
                    }
                }
            }
            else
            {
                if (false == silentLogger)
                {
                    // Not silent logger so log it
                    _logger.Debug("[Service-Request] {ServiceName}:{ServiceMethodName}", tService.Name, methodName);
                }
            }

            return methodName;
        }
        
        private string CheckAsyncServiceMethodExpressionAndGetMethodName<TResult>(
            Expression<Func<TService, Task<Response<TResult>>>> asyncServiceMethodExpr, bool silentLogger, Type tService)
        {
            bool isMethodCall = ExpressionHelper.IsMethodCall(asyncServiceMethodExpr);
            if (!isMethodCall)
            {
                throw new Exception($"The expression must contains only one method call. Exemple: (service) => service.MyMethod(myRequest)");
            }

            string methodName = ExpressionHelper.GetMethodName(asyncServiceMethodExpr);
            var methodArguments = ExpressionHelper.GetMethodArguments(asyncServiceMethodExpr);

            if (methodArguments != null && methodArguments.Count() > 0)
            {
                //if (methodArguments.Count > 1)
                //{
                //    throw new Exception($"The service method {tService.Name}/{methodName} must have 0 to 1 parameter");
                //}

                object dataArgument = Expression.Lambda(methodArguments[0]).Compile().DynamicInvoke();
                if (false == silentLogger)
                {
                    // Not silent logger so log it
                    if (_logger.IsVerboseEnabled())
                    {
                        _logger.Verbose("[Service-Request] {ServiceName}:{ServiceMethodName} {@ServiceRequest}", tService.Name, methodName, dataArgument);
                    }
                    else
                    {
                        _logger.Debug("[Service-Request] {ServiceName}:{ServiceMethodName} RequestType {@ServiceRequestType}", tService.Name, methodName, dataArgument != null ? dataArgument.GetType().Name : string.Empty);
                    }
                }
            }
            else
            {
                if (false == silentLogger)
                {
                    // Not silent logger so log it
                    _logger.Debug("[Service-Request] {ServiceName}:{ServiceMethodName}", tService.Name, methodName);
                }
            }

            return methodName;
        }

        private void ConnectOrReconnectIfFaulted()
        {
            if ((_serviceInstance == null) || ((_channelFactory != null) && (_channelFactory.State == CommunicationState.Closed)))
                InitConnexion();
            else
            if (((IClientChannel)_serviceInstance).State == CommunicationState.Faulted)
            {
                try
                {
                    ((IClientChannel)_serviceInstance).Abort();
                    ((IClientChannel)_serviceInstance).Close();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "An error occured when trying to abort+close current channel in faulted state. Exception : " + ex.Message + ex.StackTrace);
                }
                InitConnexion();
            }
        }

        /// <summary>
        /// Manage the exception returned by the service
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="response"></param>
        internal void ManageException<TResult>(Response<TResult> response)
        {
            if (response.Exception != null)
            {
                throw new Exception(string.Format("[{0}] Service : {1}", response.Exception.Type, response.Exception.Message));
            }
        }

        /// <summary>
        /// Get if the channel is opened
        /// </summary>
        /// <returns></returns>
        public bool IsChannelOpened()
        {
            bool isOpened = false;

            if (_serviceInstance != null)
            {
                var communicationState = ((IClientChannel)_serviceInstance).State;
                isOpened = communicationState == CommunicationState.Opened
                    || communicationState == CommunicationState.Opening
                    || communicationState == CommunicationState.Created;
            }

            return isOpened;
        }

        public void DisposeChannel()
        {
            if (IsChannelOpened())
            {
                try
                {
                    _channelFactory.Close();
                }
                catch (CommunicationException)
                {
                    if (_channelFactory != null)
                    {
                        _channelFactory.Abort();
                    }
                }
                catch (TimeoutException)
                {
                    if (_channelFactory != null)
                    {
                        _channelFactory.Abort();
                    }
                }
                catch (Exception)
                {
                    if (_channelFactory != null)
                    {
                        _channelFactory.Abort();
                    }
                }
            }
        }
    }
}
