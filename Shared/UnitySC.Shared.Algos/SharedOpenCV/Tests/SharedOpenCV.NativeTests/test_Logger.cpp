#include "CppUnitTest.h"

#pragma unmanaged

#include "CEventHandler.hpp"
#include "LoggerOCV.hpp"
#include <map>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace SharedOpenCVNativeTests
{
    class EventMsgSpooler : public Events::EventHandler {
    public:
        EventMsgSpooler() { Messages = std::map<Events::Severity, std::vector<std::string>>(); }
        void HandleEvent(Events::Event const& e) override { Messages[e.severity].push_back(e.message); }
        std::map<Events::Severity, std::vector<std::string>> Messages;
    };

	TEST_CLASS(LoggerOCVTests)
	{
	public:
		
		TEST_METHOD(Expect_each_message_to_be_sent_once)
		{
            Logger::WriteMessage("Expect_each_message_to_be_sent_once\n");
            EventMsgSpooler eventSpooler;

            Events::EventQueue::Instance()->AddEventHandler(&eventSpooler);

            LoggerOCV::Verbose("Verbose");
            LoggerOCV::Debug("Debug");
            LoggerOCV::Info("Info");
            LoggerOCV::Warning("Warning");
            LoggerOCV::Error("Error");
            LoggerOCV::Fatal("Fatal");

            Events::EventQueue::Instance()->RemoveEventHandler(&eventSpooler);


            Assert::IsFalse(eventSpooler.Messages.size() == 0, L"Message Spooler is empty !!!");

            Assert::AreEqual(6, (int) eventSpooler.Messages.size());

            Assert::AreEqual((int) eventSpooler.Messages[Events::Severity::Verbose].size(), 1);
            Assert::AreEqual((int) eventSpooler.Messages[Events::Severity::Debug].size(), 1);
            Assert::AreEqual((int) eventSpooler.Messages[Events::Severity::Info].size(), 1);
            Assert::AreEqual((int) eventSpooler.Messages[Events::Severity::Warning].size(), 1);
            Assert::AreEqual((int) eventSpooler.Messages[Events::Severity::Error].size(), 1);
            Assert::AreEqual((int) eventSpooler.Messages[Events::Severity::Fatal].size(), 1);
            Logger::WriteMessage("Done\n");
		}

        TEST_METHOD(Several_severities_should_be_useable)
        {
            Logger::WriteMessage("Several_severities_should_be_useable\n");

            EventMsgSpooler handler;

            Events::EventQueue::Instance()->AddEventHandler(&handler);

            LoggerOCV::Verbose("Verbose");
            LoggerOCV::Debug("Debug");
            LoggerOCV::Info("Info");
            LoggerOCV::Warning("Warning");
            LoggerOCV::Error("Error");
            LoggerOCV::Fatal("Fatal");

            Events::EventQueue::Instance()->RemoveEventHandler(&handler);

            Assert::IsFalse(handler.Messages.size() == 0, L"Message Spooler is empty !!!");

            Assert::AreEqual(6, (int)handler.Messages.size());

            Assert::AreEqual(handler.Messages[Events::Severity::Verbose].at(0), std::string("Verbose"));
            Assert::AreEqual(handler.Messages[Events::Severity::Debug].at(0), std::string("Debug"));
            Assert::AreEqual(handler.Messages[Events::Severity::Info].at(0), std::string("Info"));
            Assert::AreEqual(handler.Messages[Events::Severity::Warning].at(0), std::string("Warning"));
            Assert::AreEqual(handler.Messages[Events::Severity::Error].at(0), std::string("Error"));
            Assert::AreEqual(handler.Messages[Events::Severity::Fatal].at(0), std::string("Fatal"));

            Logger::WriteMessage("Done\n");
        }
	};
}
