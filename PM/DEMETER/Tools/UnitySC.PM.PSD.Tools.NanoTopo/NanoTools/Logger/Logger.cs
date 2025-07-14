using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.Common;

namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.Logger
{

    /**
        ******************************************************************************************
        *
        * \Class Logger
        * \brief  Gère les logs des différents composants de l'application en faisant des backups si nécessaire

        * 
        ******************************************************************************************/
    public class Logger:LoggerObservable,LoggerObserver
    {
        #region Attributs privés
        private String m_sLogPath;
        private int m_nbBackupLogs;
        private int m_sizeInBytes;
        private int m_logLevel;
        private object m_logAccessLock;
        private string[] m_sLogSourceName;
        #endregion

        #region Attributs protégés
        protected Hashtable _observerContainer = new Hashtable();
        #endregion

        #region Méthodes publiques

        /**
        ******************************************************************************************
        *
        * \brief Constructeur,
        * \param GUI Interface graphique à notifier
        * \param logPath Chemin vers le dossier de stockage des logs
        * \param NbBackupLogs Nombre de fichiers log à conserver durant le fonctionnement
        * \param SizeInMBytes Taille d'un fichier log à déplacer dans le dossier de backup
        * 
        ******************************************************************************************/
        public Logger(String logPath, int NbBackupLogs, int SizeInMBytes,int LogLevel,string [] LogSourceName)
        {
            m_logAccessLock = new Object();
            m_nbBackupLogs = NbBackupLogs;
            m_sLogPath = logPath;
            m_sizeInBytes = SizeInMBytes * 1024 * 1024;
            m_logLevel = LogLevel;

            int nElt = LogSourceName.Length;
            m_sLogSourceName = new string[nElt + 1];
            m_sLogSourceName[0] = "Logger";

            for (int i = 1; i < (nElt+1); i++)
            {
                m_sLogSourceName[i] = LogSourceName[i - 1];
            }

            foreach( String LS in m_sLogSourceName)
            {
                Directory.CreateDirectory(logPath + @"\backup\"+LS);
            }   
        }

        /**
        ******************************************************************************************
        *
        * \brief Ecrit une entrée log dans le fichier approprié
        * \param Source Source de l'entrée log
        * \param EventType Nature de l'entrée à écrire
        * \param sEntry Descriptif de l'entrée
        * 
        ******************************************************************************************/
        public void WriteLogEntry(DateTime Time,int Source, TypeOfEvent EventType, String sEntry)
        {
            if ((int)EventType >= m_logLevel)
            {
                lock (m_logAccessLock)
                {

                    try
                    {
                        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                        String FilePath = m_sLogPath;
                        String Entry = Time.ToString("MMM-dd-yyyy hh:mm.ss.fff tt - ");

                        switch (EventType)
                        {
                            case TypeOfEvent.TIMING:
                                Entry += "Timing : ";
                                break;
                            case TypeOfEvent.ACTION:
                                Entry += "Action : ";
                                break;
                            case TypeOfEvent.ERROR:
                                Entry += "Error : ";
                                break;
                            case TypeOfEvent.INFO:
                                Entry += "Info : ";
                                break;
                            case TypeOfEvent.WARNING:
                                Entry += "Warning : ";
                                break;
                            default:
                                Entry += EventType.ToString() +" : ";
                                break;
                        }
                        Entry += sEntry;

                        String _File = GetLogFile(Source);
                        StreamWriter SW = new StreamWriter(_File, true);
                        SW.WriteLine(Entry);

                        SW.Close();
                    }
                    catch (Exception ex)
                    {
                        WriteEntry(0, TypeOfEvent.ERROR, ex.Message);
                    }
                }
            }
        }

        /**
        ******************************************************************************************
        *
        * \brief Gère la sélection du fichier de log en fonction de la source et de la taille du fichier courant
        * \param Source Source de l'entrée log    
        * 
        ******************************************************************************************/
        private String GetLogFile(int Source)
        {
            String CurrentDate = DateTime.Now.ToString("MMM-dd-yyyy_hh-mm-ss tt");
            String[] LogFiles;
            String FinalFile="";

            if(Source >= m_sLogSourceName.Length)
                Source = 0;


            CheckBackupedLogs(m_sLogPath + @"\Backup\" + m_sLogSourceName[Source], m_sLogSourceName[Source] + @"*.log");
            LogFiles = Directory.GetFiles(m_sLogPath, m_sLogSourceName[Source] + "_*.log");

            if (LogFiles.Length == 0)
            {
                FinalFile = m_sLogPath + @"\" + m_sLogSourceName[Source] + "_" + CurrentDate + ".log";
            }
            else
            {
                FileInfo _File = new FileInfo(LogFiles[0]);
                if (_File.Length >= m_sizeInBytes)
                {
                    _File.MoveTo(m_sLogPath + @"\backup\" + m_sLogSourceName[Source] + @"\" + _File.Name);
                    FinalFile = m_sLogPath + @"\" + m_sLogSourceName[Source] + "_" + CurrentDate + ".log";
                    WriteEntry(0, TypeOfEvent.INFO, _File.Name + " backuped to " + FinalFile);
                }
                else
                {
                    FinalFile = _File.FullName;
                }
            }
            return FinalFile;
        }

        /**
        ******************************************************************************************
        *
        * \brief Vérifie le nombre de logs sauvegardés, et supprime le nombre de logs nécessaires pour être conforme aux paramètres
        * \param SourceFolder Chemin vers les logs sauvegardés
        * \param FilePattern pattern type des fichiers à gérer
        * 
        ******************************************************************************************/
        private void CheckBackupedLogs(String SourceFolder,String FilePattern)
        {
            DirectoryInfo Di = new DirectoryInfo(SourceFolder);
            List<FileInfo> FoundLogs = new List<FileInfo>();
                FoundLogs.AddRange(Di.GetFiles(FilePattern, SearchOption.TopDirectoryOnly));
            // Les fichiers de log respectifs sont connus --> il suffit de contrôler leur nombre

            int NbLogsToDelete = FoundLogs.Count - m_nbBackupLogs;

            while (NbLogsToDelete >= 0)
            {
                DeleteOldestLog(FoundLogs);
                NbLogsToDelete--;
            }
        }

        /**
        ******************************************************************************************
        *
        * \brief Supprime le fichier log le plus ancien
        * \param LogFileList Liste des FileInfo représentant les logs  
        * \return True si le fichier a été correctement supprimé, false sinon
        * 
        ******************************************************************************************/
        private bool DeleteOldestLog(List<FileInfo> LogFileList)
        {
            if (LogFileList.Count == 0)
            {
                return false;
            }

            if (LogFileList.Count == 1)
            {
                try
                {
                    File.Delete(LogFileList[0].FullName);
                    LogFileList.RemoveAt(0);
                    return true;
                }
                catch
                {
                    //il y a eu une erreur, il faudra afficher la popup
                    return false;
                }

            }

            FileInfo FirstLog = LogFileList[0];
            try
            {
                DateTime FirstLogDateTime = DateTime.ParseExact(LogFileList[0].Name.Substring(LogFileList[0].Name.LastIndexOf('_') + 1, LogFileList[0].Name.LastIndexOf('.') - 1 - LogFileList[0].Name.LastIndexOf('_')), "MMM-dd-yyyy-hh mm ss tt", null);
                for (int i = 1; i < LogFileList.Count; i++)
                {
                    DateTime Date = DateTime.ParseExact(LogFileList[i].Name.Substring(LogFileList[i].Name.LastIndexOf('_') + 1, LogFileList[i].Name.LastIndexOf('.') - 1 - LogFileList[i].Name.LastIndexOf('_')), "MMM-dd-yyyy-hh mm ss tt", null);
                    TimeSpan TSpan = FirstLogDateTime.Subtract(Date);

                    if (TSpan.Ticks > 0)
                    {
                        FirstLogDateTime = Date;
                        FirstLog = LogFileList[i];
                    }
                }
                LogFileList.Remove(FirstLog);
                File.Delete(FirstLog.FullName);
                return true;
            }
            catch(Exception Ex)
            {
                WriteEntry(0, TypeOfEvent.ERROR, Ex.Message);
                return false;
            }
            
        }
        #endregion

        #region IObservable Membres

        public void RegisterLogger(LoggerObserver anObserver)
        {
            _observerContainer.Add(anObserver, anObserver);
        }
        public void UnRegisterLogger(LoggerObserver anObserver)
        {
            _observerContainer.Remove(anObserver);
        }

        private void WriteEntry(int Source, TypeOfEvent EventType, String Entry)
        {
            DateTime NowDT = DateTime.Now;
            //Ici il s'agit de notifier l'interface graphique que quelque chose ne va pas 
            foreach(LoggerObserver Observer in _observerContainer)
            {
                Observer.WriteLogEntry(NowDT,Source, EventType, Entry);
            }
        }

        #endregion
    }
}
