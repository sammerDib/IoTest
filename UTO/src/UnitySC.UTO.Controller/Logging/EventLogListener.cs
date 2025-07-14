using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

using Agileo.Common.Tracing;

using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;

namespace UnitySC.UTO.Controller.Logging
{
    public sealed class EventLogListener : IListener
    {
        private readonly Timer _timer;
        private bool _activityLock;
        
        private readonly ILog _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="EventLogListener"/>.
        /// </summary>
        public EventLogListener(string configPath)
        {
            _logger = LogManager.GetLogger("Logger");
            XmlConfigurator.Configure(new FileInfo(configPath));

            var dt = DateTime.Now.AddHours(1).Subtract(TimeSpan.FromMinutes(DateTime.Now.Minute)).Subtract(TimeSpan.FromSeconds(DateTime.Now.Second));
            _timer = new Timer(TimerCallback, null, dt - DateTime.Now, TimeSpan.FromHours(1));
        }

        public string Name { get; set; } = nameof(EventLogListener);

        public void Close() => LogManager.Shutdown();

        public void DoLog(TraceLine traceLine)
        {
            if (traceLine.Source != EventLogConstant.EventLog)
            {
                return;
            }
            
            _logger.Info(traceLine.Text);
            _activityLock = true;
        }

        #region Event handler

        void TimerCallback(Object state)
        {
            if (!_activityLock)
            {
                CreateEmptyLogFile();
            }
            _activityLock = false;

            Compressing();
            CleaningFile(EventLogConstant.EventLogFolderPath, "*.gzip", false, 10);
        }
        #endregion

        #region private

        private void CreateEmptyLogFile()
        {
            var fileName = $"EventLog{DateTime.Now.Subtract(TimeSpan.FromMinutes(1)):yyyyMMddHH}.txt";
            var filePath = Path.Combine(EventLogConstant.EventLogFolderPath, fileName);

            using StreamWriter sw = new StreamWriter(filePath);
            sw.Write(EventLogConstant.Header);
        }

        private void Compressing()
        {
            foreach (var fileName in Directory.GetFiles(EventLogConstant.EventLogFolderPath, "*.txt").ToList())
            {
                if (!fileName.Contains("EventLog.txt") && !File.Exists($"{fileName}.gzip"))
                {
                    var bytes = File.ReadAllBytes(fileName);
                    using (FileStream fs = new FileStream($"{fileName}.gzip", FileMode.CreateNew))
                    using (GZipStream zipStream = new GZipStream(fs, CompressionMode.Compress, false))
                    {
                        zipStream.Write(bytes, 0, bytes.Length);
                    }
                    File.Delete(fileName);
                }
            }
        }

        private void CleaningFile(string filePath, string searchPattern, bool includeSubDirectories, int retentionPeriod)
        {
            if (!includeSubDirectories)
            {
                var files = Directory.GetFiles(filePath, searchPattern).ToList();
                CheckAndDeleteFiles(files, retentionPeriod);
            }
            else
            {
                foreach (var subDirectory in Directory.GetDirectories(filePath))
                {
                    var files = Directory.GetFiles(subDirectory, searchPattern).ToList();
                    CheckAndDeleteFiles(files, retentionPeriod);
                }
            }
        }

        private void CheckAndDeleteFiles(List<string> files, int retentionPeriod)
        {
            foreach (var filename in files)
            {
                if (DateTime.UtcNow - File.GetCreationTimeUtc(filename) >= TimeSpan.FromDays(retentionPeriod))
                {
                    File.Delete(filename);
                }
            }
        }

        #endregion
    }

    public class HeaderOnceAppender : RollingFileAppender
    {
        protected override void WriteHeader()
        {
            try
            {
                if (LockingModel.AcquireLock().Length == 0)
                {
                    base.WriteHeader();
                }
            }
            finally
            {
                LockingModel.ReleaseLock();
            }
        }
    }

    class UtoPatternLayout : PatternLayout
    {
        public override string Header
        {
            get
            {
                return $"{EventLogConstant.Header}{Environment.NewLine}";
            }
        }
    }
}
