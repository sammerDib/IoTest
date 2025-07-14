using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.Shared.Logger;

namespace UnitySC.PM.EME.Service.Host.Test
{
    [TestClass]
    public class ProgramTests
    {
        private Mock<ILogger<object>> _loggerMock;

        [TestInitialize]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<object>>();
        }

        [TestMethod]
        public void Initialize_ShouldLogApplicationStart()
        {
            // Act
            Program.Initialize(_loggerMock.Object);

            // Assert
            _loggerMock.Verify(logger => logger.Information(It.IsAny<string>()), Times.Exactly(3));
        }
        [TestMethod]
        public void ExceptionHandler_ShouldWriteErrorFile()
        {
            // Arrange
            var errorFilePath = ConsoleSignalHandler.ErrorFilePath;
            if (File.Exists(errorFilePath))
            {
                File.Delete(errorFilePath);
            }

            // Act
            Program.ExceptionHandler(this, new UnhandledExceptionEventArgs(new Exception(), false));

            // Assert
            Assert.IsTrue(File.Exists(errorFilePath));
            var lines = File.ReadAllLines(errorFilePath);
            Assert.IsTrue(lines[0].Contains("EMERA server fatal error."));
        }              
    }
}
