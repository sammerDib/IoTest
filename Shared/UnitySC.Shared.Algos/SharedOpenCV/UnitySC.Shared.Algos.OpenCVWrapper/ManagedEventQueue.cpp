#include "ManagedEventQueue.h"

#pragma managed
using namespace System;
using namespace System::Runtime::InteropServices;

namespace UnitySCSharedAlgosOpenCVWrapper {

  ManagedEventQueue::ManagedEventQueue() {
    _referenceOnCppEventQueue = Events::EventQueue::Instance();
    _eventForwarders = new std::vector<EventForwarder *>();
  }

  ManagedEventQueue::~ManagedEventQueue() {
    for (auto forwarder : *_eventForwarders) {
      delete forwarder;
    }
    delete _eventForwarders;
  }

  void ManagedEventQueue::AddMessageEventHandler(System::EventHandler ^ delegate) {

    EventForwarder *forwarder = new EventForwarder(delegate);
    _eventForwarders->push_back(forwarder);
    _referenceOnCppEventQueue->AddEventHandler(forwarder);
  }

  void ManagedEventQueue::RemoveMessageEventHandler(System::EventHandler ^ delegateToRemove) {

    std::vector<EventForwarder *>::iterator eventForwarderIt;
    for (eventForwarderIt = _eventForwarders->begin(); eventForwarderIt != _eventForwarders->end(); ++eventForwarderIt) {

      if ((*eventForwarderIt)->Delegate() == delegateToRemove) {
        _referenceOnCppEventQueue->RemoveEventHandler(*eventForwarderIt);
        break;
      }
    }
    if (eventForwarderIt != _eventForwarders->end()) {
      _eventForwarders->erase(eventForwarderIt);
    }
  }

  void ManagedEventQueue::Post(String ^ message, Severity severity) {

    std::string cmessage = (const char *)(Marshal::StringToHGlobalAnsi(message)).ToPointer();

    Events::Severity sev;
    switch (severity) {
    case Severity::Debug:
      sev = Events::Severity::Debug;
      break;
    case Severity::Error:
      sev = Events::Severity::Error;
      break;
    case Severity::Fatal:
      sev = Events::Severity::Fatal;
      break;
    case Severity::Info:
      sev = Events::Severity::Info;
      break;
    case Severity::Verbose:
      sev = Events::Severity::Verbose;
      break;
    case Severity::Warning:
      sev = Events::Severity::Warning;
      break;
    default:
      throw gcnew ApplicationException("Unknown severity to convert from CSharp to C++");
    }
    _referenceOnCppEventQueue->Post(Events::Event(cmessage, sev));
  }
} 