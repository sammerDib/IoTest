using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.DataAccess.SQL;
using UnitySC.Shared.Data;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.DataAccess.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class DBMaintenanceService : DataAccesDuplexServiceBase<IDBMaintenanceServiceCallback>, IDBMaintenanceService
    {
        protected static IMessenger Messenger => ClassLocator.Default.GetInstance<IMessenger>();
        private const string DbBackupName = "Database.bak";
        private const string ExternalFilesFolderName = "ExternalRecipeFiles";
        private const string TemporaryBackupDirectorySuffix = "-bak";

        public DBMaintenanceService(ILogger logger) : base(logger)
        {
            Messenger.Register<DbOperationProgressMessage>(this, (r, m) => DbOperationProgressChanged(m));
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                base.Unsubscribe();
            });
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                base.Subscribe();
            });
        }

        public void DbOperationProgressChanged(DbOperationProgressMessage progressMessage)
        {
            InvokeCallback(callBack => callBack.DbOperationProgressChanged(progressMessage));
        }

        public Response<bool> BackupDB()
        {
            return InvokeDataResponse(() =>
            {
                _logger.Debug($"Create a database backup");

                string conString = DataAccessConfiguration.Instance.DbConnectionString;
                string backupPath = null;
                string backupExternalFilesPath = null;
                try
                {
                    Messenger.Send(new DbOperationProgressMessage { Message = "Database Backup started", Progress = 1d });
                    string backupRootPath;
                    var ecsb = new System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder(conString);
                    using (var sqlCon = new SqlConnection(ecsb.ProviderConnectionString))
                    {
                        sqlCon.Open();

                        backupRootPath = Path.Combine(DataAccessConfiguration.Instance.DBBackupFolder, $"DB_{sqlCon.Database}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}");
                        backupPath = Path.Combine(backupRootPath, DbBackupName);
                        Messenger.Send(new DbOperationProgressMessage { Message = "Create backup directory", Progress = 1d });
                        Directory.CreateDirectory(Path.GetDirectoryName(backupPath));

                        string backupQuery = $"BACKUP DATABASE {sqlCon.Database} TO DISK = '{backupPath}'";
                        using (var command = new SqlCommand(backupQuery, sqlCon))
                        {
                            Messenger.Send(new DbOperationProgressMessage { Message = "Create database backup", Progress = 1d });
                            command.ExecuteNonQuery();
                            _logger.Debug("DB Backup completed successfully.");
                            Messenger.Send(new DbOperationProgressMessage { Message = "Database Backup completed successfully", Progress = 1d });
                        }
                    }

                    // Backup of the external files
                    string externalFilePath = DataAccessConfiguration.Instance.RootExternalFilePath;
                    backupExternalFilesPath = Path.Combine(backupRootPath, ExternalFilesFolderName);
                    Messenger.Send(new DbOperationProgressMessage { Message = "Backup of the external files : preparing to process", Progress = 1d });
                    int nbFilesInProgressBar = 0;
                    int nbFiles = DirectoryTools.CountFilesInDirectoryAndSendProgressMessage(externalFilePath,
                        (n) => MessengerSendDbProgressMessageDuringCountFiles(ref nbFilesInProgressBar, n));
                    Messenger.Send(new DbOperationProgressMessage { Message = $"Backup of the external files : {nbFiles} files to process", Progress = 5d });
                    double stepProgress = 1d / nbFiles * 90d;
                    DirectoryTools.CopyDirectoryAndSendProgressMessage(externalFilePath, 
                        backupExternalFilesPath, 
                        true, 
                        () => MessengerSendDbProgressMessageDuringCopyFiles(ref nbFiles, stepProgress));
                }
                catch (Exception ex)
                {
                    _logger.Debug("Backup failed: " + ex.Message);
                    if (!backupExternalFilesPath.IsNullOrEmpty() && Directory.Exists(backupExternalFilesPath))
                    {
                        _logger.Debug($"Deleting backup external files directory ({backupExternalFilesPath})");
                        Directory.Delete(backupExternalFilesPath, true);       
                    }
                    if (!backupPath.IsNullOrEmpty() && Directory.Exists(Path.GetDirectoryName(backupPath)))
                    {
                        _logger.Debug($"Deleting database backup path ({backupPath})");
                        Directory.Delete(Path.GetDirectoryName(backupPath), true);
                    }
                    
                    return false;
                }
                return true;
            });
        }

        public Response<List<string>> GetBackupsList()
        {
            return InvokeDataResponse(() =>
            {
                try
                {
                    var availableBackups = Directory.GetDirectories(DataAccessConfiguration.Instance.DBBackupFolder).ToList();
                    return availableBackups;
                }
                catch (Exception)
                {
                    return null;
                }
            });
        }

        public Response<bool> RepairDB(int userId)
        {
            return InvokeDataResponse(() =>
            {
                try
                {
                    _logger.Debug($"Repair the database");
                    using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                    {
                        // We check that all steps of archived products are archived 
                        var stepsThatShouldBeArchived = unitOfWork.StepRepository.CreateQuery(true).Where(x => x.Product.IsArchived && !x.IsArchived).ToList();

                        if (stepsThatShouldBeArchived.Count > 0)
                        {
                            foreach (var step in stepsThatShouldBeArchived)
                            {
                                step.IsArchived = true;
                            }
                            DataAccessHelper.LogInDatabase(unitOfWork,
                                           userId,
                                           Dto.Log.ActionTypeEnum.Edit,
                                           Dto.Log.TableTypeEnum.Step,
                                           $"Database has been repaired : {stepsThatShouldBeArchived.Count} steps modified", _logger);
                        }
                        unitOfWork.Save();

                        // We check that all recipes of archived steps are archived 
                        var recipesThatShouldBeArchived = unitOfWork.RecipeRepository.CreateQuery(true).Where(x => x.Step.IsArchived && !x.IsArchived).ToList();

                        if (recipesThatShouldBeArchived.Count > 0)
                        {

                            foreach (var recipe in recipesThatShouldBeArchived)
                            {
                                recipe.IsArchived = true;
                            }
                            DataAccessHelper.LogInDatabase(unitOfWork,
                                           userId,
                                           Dto.Log.ActionTypeEnum.Edit,
                                           Dto.Log.TableTypeEnum.Recipe,
                                           $"Database has been repaired : {recipesThatShouldBeArchived.Count} recipes modified",_logger);
                        }
                        unitOfWork.Save();

                        // We check that all dataflow recipes of archived steps are archived 
                        var dfRecipesThatShouldBeArchived = unitOfWork.DataflowRepository.CreateQuery(true).Where(x => x.Step.IsArchived && !x.IsArchived).ToList();

                        if (dfRecipesThatShouldBeArchived.Count > 0)
                        {

                            foreach (var dfRecipe in dfRecipesThatShouldBeArchived)
                            {
                                dfRecipe.IsArchived = true;
                            }
                            DataAccessHelper.LogInDatabase(unitOfWork,
                                           userId,
                                           Dto.Log.ActionTypeEnum.Edit,
                                           Dto.Log.TableTypeEnum.Dataflow,
                                           $"Database has been repaired : {dfRecipesThatShouldBeArchived.Count} dataflow recipes modified", _logger);
                        }
                        unitOfWork.Save();
                        return true;
                    }
                }
                catch (Exception)
                {

                    _logger.Debug($"Repair database Failed");
                    return false;
                }
            });
        }

        public Response<bool> RestoreDB(string dbToRestore)
        {
            return InvokeDataResponse(() =>
            {
                _logger.Debug($"Restore the database from the backup: {dbToRestore}");

                string conString = DataAccessConfiguration.Instance.DbConnectionString;
                string externalFilesPath = DataAccessConfiguration.Instance.RootExternalFilePath;
                string backupExternalFilesPath = Path.Combine(dbToRestore, ExternalFilesFolderName);
                var ecsb = new System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder(conString);
                using (var sqlCon = new SqlConnection(ecsb.ProviderConnectionString))
                {
                    string databaseName = sqlCon.Database;
                    try
                    {
                        sqlCon.Open();
                        Messenger.Send(new DbOperationProgressMessage { Message = "Evaluate directories for external files", Progress = 1d });
                        var directories = Directory.EnumerateDirectories(externalFilesPath);
                        if (directories.Any(dirName => dirName.EndsWith(TemporaryBackupDirectorySuffix)))
                        {
                            //Another backup is proceeding, we must not do anything, and abort restore
                            _logger.Error($"Another backup is already in progress, cannot proceed.");
                            return false;
                        }

                        // Renaming current directories under ExternalFiles with -bak suffix to allow for restoration if restore fails
                        Messenger.Send(new DbOperationProgressMessage { Message = "Rename current directories", Progress = 4d });
                        foreach (string recipeDir in directories)
                        {
                            Directory.Move(recipeDir, recipeDir + TemporaryBackupDirectorySuffix);
                        }

                        // Copying backup external files to ensure backup can proceed without finishing in an incorrect state
                        Messenger.Send(new DbOperationProgressMessage { Message = "Backup of the external files : preparing to process", Progress = 5d });
                        int nbFilesInProgressBar = 0;
                        int nbFiles = DirectoryTools.CountFilesInDirectoryAndSendProgressMessage(backupExternalFilesPath,
                            (n) => MessengerSendDbProgressMessageDuringCountFiles(ref nbFilesInProgressBar, n));
                        Messenger.Send(new DbOperationProgressMessage { Message = $"Backup of the external files : {nbFiles} files to process", Progress = 5d });
                        double stepProgress = 1d / nbFiles * 80d;
                        DirectoryTools.CopyDirectoryAndSendProgressMessage(backupExternalFilesPath, 
                            externalFilesPath, 
                            true,
                            () => MessengerSendDbProgressMessageDuringCopyFiles(ref nbFiles, stepProgress));

                        string backupPath = Path.Combine(dbToRestore, DbBackupName);
                        string restoreQuery = $"RESTORE DATABASE [{sqlCon.Database}] FROM DISK = '{backupPath}' WITH REPLACE";
                        string disconnectUsersQuery = $"ALTER DATABASE [{sqlCon.Database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";

                        Messenger.Send(new DbOperationProgressMessage { Message = "Disconnect active users from the database", Progress = 1d });
                        // Disconnect active users from the database
                        using (var disconnectCommand = new SqlCommand(disconnectUsersQuery, sqlCon))
                        {
                            disconnectCommand.ExecuteNonQuery();
                        }

                        Messenger.Send(new DbOperationProgressMessage { Message = "Execute query on database", Progress = 1d });
                        string switchToMasterQuery = "USE master";
                        using (var switchToMasterCommand = new SqlCommand(switchToMasterQuery, sqlCon))
                        {
                            switchToMasterCommand.ExecuteNonQuery();
                        }

                        Messenger.Send(new DbOperationProgressMessage { Message = "Restore database", Progress = 1d });
                        using (var command = new SqlCommand(restoreQuery, sqlCon))
                        {
                            command.ExecuteNonQuery();
                        }

                        _logger.Debug("Restore completed successfully.");

                        // Remove the recovering dir as restore succeeded
                        Messenger.Send(new DbOperationProgressMessage { Message = "Remove the recovering dir", Progress = 1 });
                        foreach (string backupDir in Directory.EnumerateDirectories(externalFilesPath).Where(dirName => dirName.EndsWith(TemporaryBackupDirectorySuffix)))
                        {
                            Directory.Delete(backupDir, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Debug("Error during restore: " + ex.Message);
                        try
                        {
                            RestoreOriginalExternalFiles(externalFilesPath);
                        }
                        catch (Exception restoreException)
                        {
                            _logger.Fatal(restoreException, "Failed to restore original files. The database and external files are out of sync. Manual intervention is required to fix the issue. DataAccess service will now exit.");
                            Environment.Exit(1);
                        }
                        
                        return false;
                    }
                    finally
                    {
                        string reconnectUsersQuery = $"ALTER DATABASE [{databaseName}] SET MULTI_USER";
                        // Reconnect users to the database
                        using (var reconnectCommand = new SqlCommand(reconnectUsersQuery, sqlCon))
                        {
                            reconnectCommand.ExecuteNonQuery();
                        }
                    }
                }
                return true;
            });
        }

        private void RestoreOriginalExternalFiles(string externalFilesPath)
        {
            _logger.Debug($"Removing External files coming from backup.");
            foreach (string comingFromBackupDir in Directory.EnumerateFiles(externalFilesPath).Where(dirName => !dirName.EndsWith(TemporaryBackupDirectorySuffix)))
            {
                Directory.Delete(comingFromBackupDir);
            }
                        
            _logger.Debug($"Restoring original external files.");
            foreach (string backupDir in Directory.EnumerateDirectories(externalFilesPath).Where(dirName => dirName.EndsWith(TemporaryBackupDirectorySuffix)))
            {
                string originalDirName = backupDir.Substring(0, externalFilesPath.Length - 4);
                Directory.Move(backupDir, originalDirName);
            }
        }

        private void MessengerSendDbProgressMessageDuringCountFiles(ref int nbTotalFiles, int nbFilesInDirectory)
        {
            nbTotalFiles += nbFilesInDirectory;
            Messenger.Send(new DbOperationProgressMessage { Message = $"Counting the external files : {nbTotalFiles} files found", Progress = 0.005 });
        }

        private void MessengerSendDbProgressMessageDuringCopyFiles(ref int nbFiles, double stepProgress)
        {
            Messenger.Send(new DbOperationProgressMessage { Message = $"Backup of the external files : {--nbFiles} files to process", Progress = stepProgress });
        }
    }
}
