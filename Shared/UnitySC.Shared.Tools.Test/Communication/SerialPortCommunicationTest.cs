using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using SimpleInjector;

using UnitySC.Shared.Logger;
using UnitySC.Shared.Test.Tools;
using UnitySC.Shared.Tools.Communication;

namespace UnitySC.Shared.Tools.Test.Communication
{
    [TestClass]
    public class SerialPortCommunicationTest
    {
        private const int WriteTimeoutMs = 100;
        private static readonly TimeSpan WriteTimeout = TimeSpan.FromMilliseconds(WriteTimeoutMs);
        private const int ResponseHandlingTimeoutMs = 100;
        private static readonly TimeSpan ResponseHandlingTimeout = TimeSpan.FromMilliseconds(ResponseHandlingTimeoutMs);
        private CancellationTokenSource _cancellationTokenSource;

        private Mock<ISerialPort> _serialPortMock;
        private SerialPortCommunication _comm;

        [TestInitialize]
        public void Initialize()
        {
            ClassLocator.ExternalInit(new Container(), true);
            ClassLocator.Default.Register(Mock.Of<ILogger>);
            _cancellationTokenSource = new CancellationTokenSource();

            _serialPortMock = new Mock<ISerialPort>();
            _comm = new SerialPortCommunication("COM3", 3000, _serialPortMock.Object);
            _comm.ResponseTimeout = TimeSpan.FromSeconds(1);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _cancellationTokenSource.Cancel();
        }

        [TestMethod]
        public void ConnectAndDisconnect()
        {
            using (_comm)
            {
                _comm.Connect();
                _serialPortMock.Verify(port => port.Open());
                _comm.Disconnect();
                _serialPortMock.Verify(port => port.Close());
            }

            _serialPortMock.Verify(port => port.Dispose());
        }

        [TestMethod]
        public void Connect_When_UnauthorizedAccessException()
        {
            // Given
            _serialPortMock.Setup(port => port.PortName).Returns("COM3");
            _serialPortMock.Setup(port => port.Open()).Throws(new UnauthorizedAccessException("unauthorized"));

            // When
            Action action = () => _comm.Connect();

            // Then
            action.Should()
                .Throw<UnauthorizedAccessException>()
                .WithMessage("SerialPort COM3 is currently used by another process*")
                .WithInnerException<UnauthorizedAccessException>()
                .WithMessage("unauthorized");
        }

        [TestMethod]
        public void Command()
        {
            // Given
            var command = new SerialPortCommand<string> { Message = "my-command", AcknowlegedResponsePattern = null };

            // When
            _comm.Command(command);

            // Then
            _serialPortMock.Verify(port => port.Write("my-command"));
        }

        [DataTestMethod]
        [DataRow(".*response", "the response")]
        [DataRow("\u0002\u0006\u0003", "\u0002\u0006\u0003")]
        [Description("Command() with acknowledgment waits until the expected response is received")]
        public void Command_with_ack(string ackResponsePattern, string serialPortResponse)
        {
            // Given
            var command = new SerialPortCommand<string>
            {
                Message = "my-command",
                AcknowlegedResponsePattern = new Regex(ackResponsePattern)
            };

            // When
            var task = Task.Run(() => _comm.Command(command), _cancellationTokenSource.Token);

            // Then
            _serialPortMock.WaitUntil(port => port.Write(It.IsAny<string>()),
                TimeSpan.FromMilliseconds(WriteTimeoutMs),
                _cancellationTokenSource.Token
            );
            _serialPortMock.Setup(port => port.ReadExisting()).Returns(serialPortResponse);
            task.IsCompleted.Should().BeFalse();

            _serialPortMock.Raise(port => port.DataReceived += null);
            task.Wait(ResponseHandlingTimeout);
            task.IsFaulted.Should().BeFalse();
        }

        [TestMethod]
        [Description("Command() with acknowledgment throws exception when not expected response is received")]
        public void Command_with_not_expected_response()
        {
            // Given
            var command = new SerialPortCommand<string>
            {
                Message = "my-command",
                AcknowlegedResponsePattern = new Regex("expected response")
            };

            _serialPortMock.Setup(port => port.Write(It.IsAny<string>()))
                .Callback(() =>
                    {
                        _serialPortMock.Setup(port => port.ReadExisting()).Returns("another response");
                        _serialPortMock.Raise(port => port.DataReceived += null);
                    }
                );

            // When
            Action action = () => _comm.Command(command);

            // Then
            action.Should()
                .Throw<Exception>()
                .WithMessage("Command 'my-command' failed");
        }

        [TestMethod]
        [Description("Command() with acknowledgment throws exception when no response is received until timeout")]
        public void Command_With_ack_response_waits_response_with_timeout()
        {
            // Given
            _comm.ResponseTimeout = TimeSpan.FromMilliseconds(20);
            var command = new SerialPortCommand<string>
            {
                Message = "my-command",
                AcknowlegedResponsePattern = new Regex("expected response")
            };

            // When
            Action action = () => _comm.Command(command);

            // Then
            action.Should()
                .Throw<Exception>()
                .WithInnerException<TimeoutException>()
                .WithMessage($"*{_comm.ResponseTimeout.Milliseconds} ms*");
        }

        [TestMethod]
        [Description("Command() does not write any command until last Command() is finished")]
        public async Task Command_Lock()
        {
            // Given
            var firstCommand = Task.Run(
                () => _comm.Command(new SerialPortCommand<string>
                {
                    Message = "command1",
                    AcknowlegedResponsePattern = new Regex("response")
                }
                ),
                _cancellationTokenSource.Token
            );
            _serialPortMock.WaitUntil(port => port.Write("command1"), WriteTimeout, _cancellationTokenSource.Token);

            // When
            var secondCommand = Task.Run(
                () => _comm.Command(new SerialPortCommand<string> { Message = "command2" }),
                _cancellationTokenSource.Token
            );

            // Then
            await Task.Delay(10, _cancellationTokenSource.Token); // Give some time for command2 to be called
            _serialPortMock.Verify(port => port.Write("command2"), Times.Never);
            secondCommand.IsCompleted.Should().BeFalse();

            _serialPortMock.Setup(port => port.ReadExisting()).Returns("response");
            _serialPortMock.Raise(port => port.DataReceived += null);
            firstCommand.Wait(ResponseHandlingTimeout);
            secondCommand.Wait(ResponseHandlingTimeout);
            secondCommand.IsCompleted.Should().BeTrue();
        }

        [TestMethod]
        public void Query()
        {
            // Given
            _comm.ConsistentResponsePattern = new Regex(@".*\r\n$");
            _comm.IgnoredResponsePattern = null;
            _serialPortMock.Setup(port => port.Write(It.IsAny<string>()))
                .Callback(() =>
                    {
                        _serialPortMock.SetupSequence(port => port.ReadExisting()).Returns("key=-99\r\n");
                        _serialPortMock.Raise(port => port.DataReceived += null);
                    }
                );

            // When
            string response = _comm.Query(new SerialPortQuery<string>
            {
                Message = "query-message",
                ResponsePattern = new Regex(@"key=(-?\d+)")
            }
            );

            // Then
            response.Should().Be("-99");
        }

        [TestMethod]
        public void Query_with_bytes_array()
        {
            // Given
            _comm.ConsistentResponsePattern = new Regex(@".*\r\n$");
            _comm.IgnoredResponsePattern = null;
            _serialPortMock.Setup(port => port.Write(It.IsAny<string>()))
                .Callback(() =>
                    {
                        _serialPortMock.SetupSequence(port => port.ReadExisting()).Returns("key=99\r\n");
                        _serialPortMock.Raise(port => port.DataReceived += null);
                    }
                );
            var query = new SerialPortQuery<byte[]>
            {
                Message = new byte[] { 0x41, 0x42, 0x43 }, // Means "ABC" in hexa
                ResponsePattern = new Regex(@"key=(\d+)")
            };

            // When
            string response = _comm.Query(query);

            // Then
            response.Should().Be("99");
            _serialPortMock.Verify(mock => mock.Write("ABC"));
        }

        [TestMethod]
        public void Query_without_capturing_group()
        {
            // Given
            _comm.ConsistentResponsePattern = new Regex(@".*\r\n$");
            _comm.IgnoredResponsePattern = null;
            _serialPortMock.Setup(port => port.Write(It.IsAny<string>()))
                .Callback(() =>
                    {
                        _serialPortMock.SetupSequence(port => port.ReadExisting()).Returns("key=22\r\n");
                        _serialPortMock.Raise(port => port.DataReceived += null);
                    }
                );
            var query = new SerialPortQuery<string> { Message = "query-message", ResponsePattern = new Regex(@"key=\d+") };

            // When & then
            _comm.Invoking(comm => comm.Query(query))
                .Should()
                .Throw<Exception>()
                .WithMessage("Pattern should define only one capturing group.");
        }

        [TestMethod]
        public void Query_without_any_many_capturing_groups()
        {
            // Given
            _comm.ConsistentResponsePattern = new Regex(@".*\r\n$");
            _comm.IgnoredResponsePattern = null;
            _serialPortMock.Setup(port => port.Write(It.IsAny<string>()))
                .Callback(() =>
                    {
                        _serialPortMock.SetupSequence(port => port.ReadExisting()).Returns("key=22\r\n");
                        _serialPortMock.Raise(port => port.DataReceived += null);
                    }
                );
            var query = new SerialPortQuery<string> { Message = "query-message", ResponsePattern = new Regex(@"key=(\d+)-(\d)") };

            // When
            Action action = () => _comm.Query(query);

            // Then
            action.Should()
                .Throw<Exception>()
                .WithMessage("Pattern should define only one capturing group.");
        }

        [TestMethod]
        [Description("Response received by chunk")]
        public void Query_with_responses_received_by_chunk()
        {
            // Given
            _comm.ConsistentResponsePattern = new Regex(@"^key=.+\r\n$");
            _serialPortMock.Setup(port => port.Write(It.IsAny<string>()))
                .Callback(() =>
                    {
                        _serialPortMock.SetupSequence(port => port.ReadExisting())
                            .Returns("ke")
                            .Returns("y=exp")
                            .Returns("ected value\r\n");
                        _serialPortMock.Raise(port => port.DataReceived += null);
                        _serialPortMock.Raise(port => port.DataReceived += null);
                        _serialPortMock.Raise(port => port.DataReceived += null);
                    }
                );

            // When
            string response = _comm.Query(new SerialPortQuery<string>
            {
                Message = "query-message",
                ResponsePattern = new Regex(@"key=(.+)\r\n"),
            }
            );

            // Then
            response.Should().Be("expected value");
        }

        [TestMethod]
        [Description("Relevant and ignored responses received by chunk")]
        public void Query_with_responses_received_by_chunk_after_ignored_message()
        {
            // Given
            _comm.ConsistentResponsePattern = new Regex(@".*\r\n$");
            _comm.IgnoredResponsePattern = new Regex("error : C?CW Limit!!");
            _serialPortMock.Setup(port => port.Write(It.IsAny<string>()))
                .Callback(() =>
                    {
                        _serialPortMock.SetupSequence(port => port.ReadExisting())
                            .Returns("error : CW Lim")
                            .Returns("it!!\r\nUx.1=8\r\n");
                        _serialPortMock.Raise(port => port.DataReceived += null);
                        _serialPortMock.Raise(port => port.DataReceived += null);
                    }
                );

            // When
            string response = _comm.Query(new SerialPortQuery<string>
            {
                Message = "query-message",
                ResponsePattern = new Regex(@"Ux.1=(-?\d+)"),
            }
            );

            // Then
            response.Should().Be("8");
        }

        [TestMethod]
        [Description(
            "Relevant and ignored responses received by chunk, with ignored response received after relevant one"
        )]
        public void Query_with_ignored_received_after()
        {
            // Given
            _comm.ConsistentResponsePattern = new Regex(@".*\r\n$");
            _comm.IgnoredResponsePattern = new Regex("error : C?CW Limit!!");
            _serialPortMock.Setup(port => port.Write(It.IsAny<string>()))
                .Callback(() =>
                    {
                        _serialPortMock.SetupSequence(port => port.ReadExisting())
                            .Returns("Ux.1=")
                            .Returns("8\r\nerror : CCW Lim")
                            .Returns("it!!\r\n");
                        _serialPortMock.Raise(port => port.DataReceived += null);
                        _serialPortMock.Raise(port => port.DataReceived += null);
                        _serialPortMock.Raise(port => port.DataReceived += null);
                    }
                );

            // When
            string response = _comm.Query(new SerialPortQuery<string>
            {
                Message = "query-message",
                ResponsePattern = new Regex(@"Ux.1=(-?\d+)")
            }
            );

            // Then
            response.Should().Be("8");
        }

        // TODO test without ignored pattern

        [TestMethod]
        public void Query_without_ignored_response()
        {
            // Given
            _comm.ConsistentResponsePattern = new Regex(@".*\r\n$");
            _comm.IgnoredResponsePattern = null;
            _serialPortMock.Setup(port => port.Write(It.IsAny<string>()))
                .Callback(() =>
                    {
                        _serialPortMock.SetupSequence(port => port.ReadExisting()).Returns("response 2\r\n");
                        _serialPortMock.Raise(port => port.DataReceived += null);
                    }
                );

            // When
            string response = _comm.Query(new SerialPortQuery<string>
            {
                Message = "query-message",
                ResponsePattern = new Regex(@"response (-?\d+)")
            }
            );

            // Then
            response.Should().Be("2");
        }
    }
}
