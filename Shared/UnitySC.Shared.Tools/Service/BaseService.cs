using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnitySC.Shared.Logger;

namespace UnitySC.Shared.Tools.Service
{
    public abstract class BaseService
    {
        protected readonly ILogger _logger;
        private readonly ExceptionType _serviceExceptionType;

        public BaseService(ILogger logger, ExceptionType serviceExceptionType)
        {
            _logger = logger;
            _serviceExceptionType = serviceExceptionType;
        }

        public virtual void Init()
        {
        }

        public virtual void Shutdown()
        {
        }

        /// <summary>
        /// Generic methode to invoke a WCF method with a specific response
        /// </summary>
        /// <typeparam name="T">Type of the response object</typeparam>
        /// <param name="serviceCallback">Callback of the invocation</param>
        protected Response<T> InvokeDataResponse<T>(Func<T> serviceCallback)
        {
            var response = new Response<T>();

            // Execution du service
            try
            {
                response.Result = serviceCallback();
            }
            catch (Exception e)
            {
                _logger.Error(e, "Invoke error");

                response.Exception = CreateNestedException(e);
            }
            return response;
        }

        protected async Task<Response<T>> InvokeDataResponseAsync<T>(Func<Task<T>> asyncServiceCallback)
        {
            var response = new Response<T>();

            try
            {
                response.Result = await asyncServiceCallback();
            }
            catch (Exception e)
            {
                _logger.Error(e, "Invoke error");
                response.Exception = CreateNestedException(e);
            }

            return response;
        }

        /// <summary>
        /// Generic methode to invoke a WCF method with a specific response
        /// </summary>
        /// <typeparam name="T">Type of the response object</typeparam>
        /// <param name="serviceCallback">Callback of the invocation</param>

        protected Response<VoidResult> InvokeVoidResponse<T>(Func<T> serviceCallback)
        {
            var response = new Response<VoidResult>();

            // Execution du service
            try
            {
                serviceCallback();
                response.Result = new VoidResult();
            }
            catch (Exception e)
            {
                _logger.Error(e, "Invoke error");

                response.Exception = CreateNestedException(e);
            }
            return response;
        }

        protected async Task<Response<VoidResult>> InvokeVoidResponseAsync<T>(Func<Task<T>> asyncServiceCallback)
        {
            var response = new Response<VoidResult>();

            try
            {
                await asyncServiceCallback();
                response.Result = new VoidResult();
            }
            catch (Exception e)
            {
                _logger.Error(e, "Invoke error");

                response.Exception = CreateNestedException(e);
            }

            return response;
        }

        /// <summary>
        /// Generic methode to invoke a WCF method with a specific response
        /// </summary>
        /// <typeparam name="T">Type of the response object</typeparam>
        /// <param name="serviceCallback">Callback of the invocation</param>
        /// <returns></returns>
        protected Response<T> InvokeDataResponse<T>(Func<List<Message>, T> serviceCallback)
        {
            var response = new Response<T>();
            var messages = new List<Message>();
            //Execution du service
            try
            {
                response.Result = serviceCallback(messages);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Invoke error");
                response.Exception = CreateNestedException(e);
            }
            finally
            {
                response.Messages = messages;
            }
            return response;
        }

        protected async Task<Response<T>> InvokeDataResponseAsync<T>(Func<List<Message>, Task<T>> asyncServiceCallback)
        {
            var response = new Response<T>();
            var messages = new List<Message>();

            try
            {
                response.Result = await asyncServiceCallback(messages);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Invoke error");

                response.Exception = CreateNestedException(e);
            }
            finally
            {
                response.Messages = messages;
            }

            return response;
        }

        /// <summary>
        /// Generic methode to invoke a WCF method with a void return
        /// </summary>
        /// <typeparam name="T">Type of the response object</typeparam>
        /// <param name="serviceCallback">Callback of the invocation</param>
        /// <returns></returns>
        protected Response<VoidResult> InvokeVoidResponse(Action<List<Message>> serviceCallback)
        {
            var response = new Response<VoidResult>();
            var messages = new List<Message>();

            //Execution du service
            try
            {
                serviceCallback(messages);
                response.Result = new VoidResult();
            }
            catch (Exception e)
            {
                _logger.Error(e, "Invoke error");

                response.Exception = CreateNestedException(e);
            }
            finally
            {
                response.Messages = messages;
            }
            return response;
        }

        protected async Task<Response<VoidResult>> InvokeVoidResponseAsync(Func<List<Message>, Task> asyncServiceCallback)
        {
            var response = new Response<VoidResult>();
            var messages = new List<Message>();

            try
            {
                await asyncServiceCallback(messages);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Invoke error");

                response.Exception = CreateNestedException(e);
            }
            finally
            {
                response.Messages = messages;
            }

            return response;
        }

        private ExceptionService CreateNestedException(Exception e)
        {
            if (e == null)
            {
                return null;
            }

            var exceptionService = new ExceptionService(e.Message, _serviceExceptionType, e.StackTrace)
            {
                InnerException = e.InnerException?.Message + e.InnerException?.InnerException?.Message
            };
            return exceptionService;
        }
    }
}
