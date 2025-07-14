#pragma once

#include "CEvent.hpp"
#include "CEventHandler.hpp"
#include <queue>

// see https://stackoverflow.com/questions/28447236/workaround-for-mutex-in-native-lib-for-cli-dll
#ifdef NATIVE_CODE
#include <mutex>
#endif

#ifndef NDEBUG
#include <iostream>
class ConsoleMessagePrinter : Events::EventHandler {
  void HandleEvent(Events::Event const &e) override {

    std::string level;
    switch (e.severity) {
    case Events::Severity::Debug:
      level = "Debug";
      break;
    case Events::Severity::Error:
      level = "Error";
      break;
    case Events::Severity::Fatal:
      level = "Fatal";
      break;
    case Events::Severity::Info:
      level = "Info";
      break;
    case Events::Severity::Verbose:
      level = "Verbose";
      break;
    case Events::Severity::Warning:
      level = "Warning";
      break;
    default:
      level = "unknown";
    }
    std::cout << "[" << level << "]  " << e.message << std::endl;
  }
};
#endif

namespace Events {

  class EventQueue {

  public:
    EventQueue() {
#ifndef NDEBUG
      AddEventHandler((EventHandler *)(&_consolePrinterHandler));
#endif
    }

    static EventQueue *Instance() { return &_instance; }

    void Post(Event const &e);

    void AddEventHandler(EventHandler *handler);
    void RemoveEventHandler(EventHandler *handler);

    void DispatchEvents();

    int Size();

  private:
#ifdef NATIVE_CODE
    std::mutex _mutex;
#endif
#ifndef NDEBUG
    ConsoleMessagePrinter _consolePrinterHandler;
#endif

    std::queue<Event> _events;
    static EventQueue _instance;
    std::vector<EventHandler *> _eventHandlers;
  };
} // namespace Events
