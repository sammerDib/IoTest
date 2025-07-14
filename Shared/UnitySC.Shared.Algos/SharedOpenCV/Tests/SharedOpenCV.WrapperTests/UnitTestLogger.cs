using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using System.Collections.Generic;

using UnitySCSharedAlgosOpenCVWrapper;

namespace SharedOpenCV.WrapperTests
{
    /// <summary>
    /// Test class used to store received messages in a dictionary where key is Severity
    /// </summary>
    public class LogMessageConsumer
    {
        public LogMessageConsumer() { Messages = new Dictionary<Severity, List<String>>(); }
        public void SpoolMessage(object sender, EventArgs e)
        {
            AlgoEventArgs aea = (AlgoEventArgs)e;
            if (!Messages.ContainsKey(aea.Severity))
            {
                Messages[aea.Severity] = new List<String>();
            }
            Messages[aea.Severity].Add(aea.Message);
        }
        public Dictionary<Severity, List<String>> Messages;
    }

    [TestClass]
    public class UnitTestLogger
    {
        [TestMethod]
        public void Different_log_levels_can_be_used()
        {
            var queue = new UnitySCSharedAlgosOpenCVWrapper.ManagedEventQueue();

            var spooler = new LogMessageConsumer();
            queue.AddMessageEventHandler(spooler.SpoolMessage);

            // when
            queue.Post("verbose", Severity.Verbose);
            queue.Post("debug", Severity.Debug);
            queue.Post("Info", Severity.Info);
            queue.Post("warning", Severity.Warning);
            queue.Post("error", Severity.Error);
            queue.Post("fatal", Severity.Fatal);

            queue.RemoveMessageEventHandler(spooler.SpoolMessage);

            // then
            Assert.AreEqual(1, spooler.Messages[Severity.Verbose].Count);
            Assert.AreEqual(1, spooler.Messages[Severity.Debug].Count);
            Assert.AreEqual(1, spooler.Messages[Severity.Info].Count);
            Assert.AreEqual(1, spooler.Messages[Severity.Warning].Count);
            Assert.AreEqual(1, spooler.Messages[Severity.Error].Count);
            Assert.AreEqual(1, spooler.Messages[Severity.Fatal].Count);

        }

        [TestMethod]
        public void Expects_logs_to_not_forward_after_handler_unregistration()
        {
            // given log message has been posted, and handler is unregistered
            var queue = new UnitySCSharedAlgosOpenCVWrapper.ManagedEventQueue();
            var logMessageConsumer = new LogMessageConsumer();
            queue.AddMessageEventHandler(logMessageConsumer.SpoolMessage);
            queue.Post("first message", Severity.Verbose);

            queue.RemoveMessageEventHandler(logMessageConsumer.SpoolMessage);

            Assert.AreEqual(1, logMessageConsumer.Messages[Severity.Verbose].Count,
               "spooler should not receive new log after unregistration");

            // when
            queue.Post("second message", Severity.Verbose);

            // then
            Assert.AreEqual(1, logMessageConsumer.Messages[Severity.Verbose].Count,
                "spooler should not receive new log after unregistration");

        }
    }
}
