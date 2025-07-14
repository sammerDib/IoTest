using System;
using System.Configuration;
using System.IO;
using System.Windows;

using AdcTools;

using Microsoft.Win32;

using Serilog;

using UnitySC.Shared.Tools;

namespace ADC.Services
{
    public class DatabaseService
    {
        /// <summary>
        /// Charge en mémoire la base de donnée exportée
        /// </summary>
        [Obsolete]
        public bool LoadDatabase()
        {
            bool result = false;
/*
            bool useExportedDataBase = Convert.ToBoolean(ConfigurationManager.AppSettings["DatabaseConfig.UseExportedDatabase"]);
            if (useExportedDataBase)
            {
                string pathExportedDataBase = ConfigurationManager.AppSettings["DatabaseConfig.ExportedDatabaseFile"];
                if (File.Exists(pathExportedDataBase))
                {
                    try
                    {
                        string dataBaseContent = File.ReadAllText(pathExportedDataBase);
                        ClassLocator.Default.GetInstance<IImportExportService>().Import(dataBaseContent);
                        Log.Information("Load external database");
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        ExceptionMessageBox.Show("Error in exported database loading. Please reimport the exported database", ex);
                    }
                }
                else
                {
                    Log.Warning("Exported database in missing.");
                    AttentionMessageBox.Show("Exported database in missing. The database is required (File-> Database Import)");
                }
            }
*/
            return result;
        }


        /// <summary>
        /// Import un fichier de base de donnée exportée
        /// </summary>
        [Obsolete]
        public void ImportDatabase()
        {
          /*  bool useExportedDataBase = Convert.ToBoolean(ConfigurationManager.AppSettings["DatabaseConfig.UseExportedDatabase"]);
            if (!useExportedDataBase)
            {
                Log.Warning("Adc started with remote database. Database Import is not available");
            }
            else
            {
                string pathExportedDataBase = ConfigurationManager.AppSettings["DatabaseConfig.ExportedDatabaseFile"];
                OpenFileDialog openFileDlg = new OpenFileDialog();

                openFileDlg.Filter = "Database files (*.adcdb)|*.adcdb";

                if (openFileDlg.ShowDialog() == true)
                {
                    try
                    {
                        File.Copy(openFileDlg.FileName, pathExportedDataBase, true);
                        if (LoadDatabase())
                            MessageBox.Show("Import database successful !");
                    }
                    catch (Exception ex)
                    {
                        ExceptionMessageBox.Show("Error in database copy", ex);
                    }
                }
            }*/
        }
    }
}
