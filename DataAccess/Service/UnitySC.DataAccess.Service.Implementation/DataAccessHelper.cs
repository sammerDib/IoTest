using System;
using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using UnitySC.DataAccess.SQL;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.DataAccess.Service.Implementation
{
    static public class DataAccessHelper
    {
        public static bool CheckDatabaseVersion(List<Message> messageContainer, IMapper mapper)
        {  
            bool bDbVersionIsOK = false;
            using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {
                var dbversion = unitOfWork.DatabaseVersionRepository.CreateQuery().OrderByDescending(x => x.Id).FirstOrDefault();
                if (dbversion == null)
                {
                    messageContainer.Add(new Message(MessageLevel.Error, $"Database version is not set - expect (v{Dto.DatabaseVersion.CurrentVersion}) "));
                }
                else
                {
                    var dtodbversion = mapper.Map<Dto.DatabaseVersion>(dbversion);
                    bDbVersionIsOK = dtodbversion.IsUptoDate();
                    if (bDbVersionIsOK)
                        messageContainer.Add(new Message(MessageLevel.None, $"Database version is up to date (v{Dto.DatabaseVersion.CurrentVersion})"));
                    else
                    {
                        if (dtodbversion.IsNewer())
                            messageContainer.Add(new Message(MessageLevel.Error, $"a Newer Database version has been detected (v{dtodbversion.Version}) - expect (v{Dto.DatabaseVersion.CurrentVersion}) "));
                        else if (dtodbversion.IsOlder())
                            messageContainer.Add(new Message(MessageLevel.Error, $"an older Database version has been detected (v{dtodbversion.Version}) - expect (v{Dto.DatabaseVersion.CurrentVersion}) "));
                        else
                            messageContainer.Add(new Message(MessageLevel.Error, $"Database version is not up to date (v{dtodbversion.Version}) - expect (v{Dto.DatabaseVersion.CurrentVersion}) "));
                    }
                }

            }
            return bDbVersionIsOK;
        }


        /// <summary>
        /// Log in database
        /// </summary>
        public static void LogInDatabase(UnitOfWorkUnity unitOfWork, int userId, Dto.Log.ActionTypeEnum? action, Dto.Log.TableTypeEnum? table, string detail, ILogger logger)
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
            logger.Information($"[LogInDatabase] User:{log.UserId} Action: {log.ActionType} Table: {log.TableType} Detail: {log.Detail}");
        }

        public static void ArchiveAllVerionsOfRecipe(Guid key, int userId, UnitOfWorkUnity unitOfWork, ILogger logger, bool checkIfUsedByDataflow=true)
        {
            if (checkIfUsedByDataflow)
                CheckIfRecipeIsUsedInDataflow(unitOfWork, key);
            var recipes = unitOfWork.RecipeRepository.CreateQuery(true).Where(x => x.KeyForAllVersion == key).ToList();
            foreach (var recipe in recipes)
            {
                recipe.IsArchived = true;
            }

            LogInDatabase(unitOfWork,
               userId,
               Dto.Log.ActionTypeEnum.Remove,
               Dto.Log.TableTypeEnum.Recipe,
               string.Format("ArchiveAllVersion for recipe {0}", key), logger);
        }

        public static void ArchiveAllVerionsOfDataflow(Guid key, int userId, UnitOfWorkUnity unitOfWork, ILogger logger)
        {
            var dataflows = unitOfWork.DataflowRepository.CreateQuery(true).Where(x => x.KeyForAllVersion == key).ToList();
            foreach (var Dataflow in dataflows)
            {
                Dataflow.IsArchived = true;
            }

            DataAccessHelper.LogInDatabase(unitOfWork,
               userId,
               Dto.Log.ActionTypeEnum.Remove,

               Dto.Log.TableTypeEnum.Dataflow,
               string.Format("ArchiveAllVersion for dataflow {0}", key), logger);

           
        }

        public static void CheckIfRecipeIsUsedInDataflow(UnitOfWorkUnity unitOfWork, Guid recipeKey)
        {
            var linkedDataflowKeys = unitOfWork.RecipeDataflowMapRepository.CreateQuery(false).Where(x => x.RecipeKey == recipeKey).Select(x => x.DataflowKey).Distinct().ToList();
            if (unitOfWork.DataflowRepository.CreateQuery(false).Any(x => !x.IsArchived && linkedDataflowKeys.Contains(x.KeyForAllVersion)))
            {
                // /!\ the text used in a dataflow must not be changed because it is used to detect this error
                throw new InvalidOperationException("Can't archive a recipe used in a dataflow");
            }
        }

    }
}
