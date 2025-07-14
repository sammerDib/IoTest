using System;

using AutoMapper;

using UnitySC.DataAccess.SQL;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.DataAccess.Service.Implementation
{
    /// <summary>
    /// Data acess service base
    /// </summary>
    public abstract class DataAccesDuplexServiceBase<T> : DuplexServiceBase<T>
    {
        public IMapper Mapper { get; private set; }

        internal BuisnessLayer Buisness { get; private set; }

        public DataAccesDuplexServiceBase(ILogger logger) : base(logger, ExceptionType.DataAccessException)
        {
            Mapper = SQLMapper.GetMapping();
            Buisness = new BuisnessLayer(logger, Mapper);
        }

        /// <summary>
        /// Log in database
        /// </summary>
        internal void LogInDatabase(UnitOfWorkUnity unitOfWork, int userId, Dto.Log.ActionTypeEnum? action, Dto.Log.TableTypeEnum? table, string detail)
        {
            var log = new SQL.Log()
            {
                UserId = userId,
                ActionType = (int?)action,
                TableType = (int?)table,
                Date = DateTime.Now,
                Detail = detail
            };

            unitOfWork.LogRepository.Add(log);
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return InvokeVoidResponse<object>(() =>
            {
                Subscribe();
                return null;
            });
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return InvokeVoidResponse<object>(() =>
            {
                Unsubscribe();
                return null;
            });
        }
    }
}
