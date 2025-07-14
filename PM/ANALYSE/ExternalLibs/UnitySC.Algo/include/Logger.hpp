#pragma once
#include <EventQueue.hpp>

/*!
 * High level class for logging trough the generic EventQueue
 */
class Logger {
public:
  static void Verbose(std::string const &message) { Events::EventQueue::Instance()->Post(Events::Event(message, Events::Severity::Verbose)); }
  static void Debug(std::string const &message) { Events::EventQueue::Instance()->Post(Events::Event(message, Events::Severity::Debug)); }
  static void Warning(std::string const &message) { Events::EventQueue::Instance()->Post(Events::Event(message, Events::Severity::Warning)); }
  static void Info(std::string const &message) { Events::EventQueue::Instance()->Post(Events::Event(message, Events::Severity::Info)); }
  static void Error(std::string const &message) { Events::EventQueue::Instance()->Post(Events::Event(message, Events::Severity::Error)); }
  static void Fatal(std::string const &message) { Events::EventQueue::Instance()->Post(Events::Event(message, Events::Severity::Fatal)); }
};