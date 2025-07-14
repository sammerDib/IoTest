#include <Logger.hpp>

#include <gtest/gtest.h>

namespace {
  class EventSpooler : public Events::EventHandler {
  public:
    EventSpooler() { Messages = std::map<Events::Severity, std::vector<std::string>>(); }
    void HandleEvent(Events::Event const &e) override { Messages[e.severity].push_back(e.message); }
    std::map<Events::Severity, std::vector<std::string>> Messages;
  };
} // namespace
TEST(Logger, Expect_each_message_to_be_sent_once) {

  EventSpooler eventSpooler;

  Events::EventQueue::Instance()->AddEventHandler(&eventSpooler);

  Logger::Verbose("Verbose");
  Logger::Debug("Debug");
  Logger::Info("Info");
  Logger::Warning("Warning");
  Logger::Error("Error");
  Logger::Fatal("Fatal");

  Events::EventQueue::Instance()->RemoveEventHandler(&eventSpooler);

  ASSERT_EQ(6, eventSpooler.Messages.size());

  ASSERT_EQ(eventSpooler.Messages[Events::Severity::Verbose].size(), 1);
  ASSERT_EQ(eventSpooler.Messages[Events::Severity::Debug].size(), 1);
  ASSERT_EQ(eventSpooler.Messages[Events::Severity::Info].size(), 1);
  ASSERT_EQ(eventSpooler.Messages[Events::Severity::Warning].size(), 1);
  ASSERT_EQ(eventSpooler.Messages[Events::Severity::Error].size(), 1);
  ASSERT_EQ(eventSpooler.Messages[Events::Severity::Fatal].size(), 1);
}

TEST(Logger, Several_severities_should_be_useable) {

  EventSpooler handler;

  Events::EventQueue::Instance()->AddEventHandler(&handler);

  Logger::Verbose("Verbose");
  Logger::Debug("Debug");
  Logger::Info("Info");
  Logger::Warning("Warning");
  Logger::Error("Error");
  Logger::Fatal("Fatal");

  Events::EventQueue::Instance()->RemoveEventHandler(&handler);

  ASSERT_EQ(6, handler.Messages.size());

  ASSERT_EQ(handler.Messages[Events::Severity::Verbose].at(0), "Verbose");
  ASSERT_EQ(handler.Messages[Events::Severity::Debug].at(0), "Debug");
  ASSERT_EQ(handler.Messages[Events::Severity::Info].at(0), "Info");
  ASSERT_EQ(handler.Messages[Events::Severity::Warning].at(0), "Warning");
  ASSERT_EQ(handler.Messages[Events::Severity::Error].at(0), "Error");
  ASSERT_EQ(handler.Messages[Events::Severity::Fatal].at(0), "Fatal");
}