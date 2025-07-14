using AutoMapper;

using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.DataAccess.Service.Implementation
{
    /// <summary>
    /// Data acess service base
    /// </summary>
    public abstract class DataAccesServiceBase : BaseService
    {
        public IMapper Mapper { get; private set; }

        internal BuisnessLayer Buisness { get; private set; }

        public DataAccesServiceBase(ILogger logger) : base(logger, ExceptionType.DataAccessException)
        {
            Mapper = SQLMapper.GetMapping();
            Buisness = new BuisnessLayer(logger, Mapper);
        }

        /// <summary>
        /// Check if database version is up to date
        /// </summary>
        /// <returns> true if version is valid</returns>
        public Response<bool> CheckDatabaseVersion()
        {
            return InvokeDataResponse(messageContainer =>
            {
                return DataAccessHelper.CheckDatabaseVersion(messageContainer, Mapper);
            });
        }
    }
}
