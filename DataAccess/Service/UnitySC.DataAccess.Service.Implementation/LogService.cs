using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.DataAccess.SQL;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.DataAccess.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class LogService : DataAccesServiceBase, ILogService
    {
        public LogService(ILogger logger) : base(logger)
        {
        }

        public Response<VoidResult> Connect(Dto.User user)
        {
            _logger.Debug($"Connect {user.Name}");
            return InvokeVoidResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    DataAccessHelper.LogInDatabase(unitOfWork, user.Id, Dto.Log.ActionTypeEnum.Connect, Dto.Log.TableTypeEnum.User, $"User connection {user.Name}", _logger);
                    unitOfWork.Save();
                }
            });
        }

        public Response<VoidResult> Disconnect(Dto.User user)
        {
            _logger.Debug($"Disconnect {user.Name}");
            return InvokeVoidResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    DataAccessHelper.LogInDatabase(unitOfWork, user.Id, Dto.Log.ActionTypeEnum.Disconnect, Dto.Log.TableTypeEnum.User, $"User deconnection {user}", _logger);
                    unitOfWork.Save();
                }
            });
        }

        /// <summary>
        /// Récupération des logs de la base de données
        /// </summary>
        /// <param name="userId">Filtre sur utilisateur si défini </param>
        /// <param name="action"> Filtre sur action si défini</param>
        /// <param name="table">Filtre sur table si défini</param>
        /// <param name="startDate">Filtre sur date de début si défini</param>
        /// <param name="endDate">Filtre sur date de fin si défini</param>
        /// <param name="detailFilter">Filtre sur le contenu du detail si défini</param>
        /// <returns></returns>
        public Response<List<Dto.Log>> GetLogs(int? userId = null, Dto.Log.ActionTypeEnum? action = null, Dto.Log.TableTypeEnum? table = null, DateTime? startDate = null, DateTime? endDate = null, string detailFilter = null)
        {
            _logger.Debug($"Get logs");
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    int? actionType = (int?)action;
                    int? tableType = (int?)table;
                    return Mapper.Map<List<Dto.Log>>(unitOfWork.LogRepository.CreateQuery(false, i => i.User).Where(x =>
                        (!userId.HasValue || x.UserId == userId)                                                 // Filtre sur utilisateur si défini
                        && (!actionType.HasValue || x.ActionType == actionType || x.ActionType == null)          // Filtre sur action si défini
                        && (!tableType.HasValue || x.TableType == tableType || x.TableType == null)              // Filtre sur table si défini
                        && (string.IsNullOrEmpty(detailFilter) || x.Detail.Contains(detailFilter))               // Filtre sur le contenu du detail si défini
                        && (!startDate.HasValue && !endDate.HasValue                                             // Filtre sur les dates si défini
                        || (startDate.HasValue && !endDate.HasValue && x.Date >= startDate)                      // Filtre sur date de début si défini
                        || (!startDate.HasValue && endDate.HasValue && x.Date <= endDate)                        // Filtre sur date de fin si défini
                        || (startDate.HasValue && endDate.HasValue && x.Date <= endDate && x.Date >= startDate)) // Filtre sur date de début et de fin si défini
                        ).Take(1000).ToList());
                }
            });
        }
    }
}
