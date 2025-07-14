#pragma once

#include <EventQueue.hpp>
#include <TypeAdapters.h>
#include <vcclr.h>

using namespace System::Threading;
using namespace System::Collections::Generic;
using namespace System;

namespace AlgosLibrary {

    public enum class Severity : unsigned char {
        Debug = 0,
        Verbose = 2,
        Info = 4,
        Warning = 8,
        Error = 16,
        Fatal = 32
    };

    /**
     * Matches the content of native Events::Event
     *
     */
    public ref class AlgoEventArgs : public EventArgs {

    public:
        AlgoEventArgs(String^ message, Severity severity) : Message(message), Severity(severity) { Message = message; }

        const String^ Message;
        const Severity Severity;
    };

    /**
     * Bridges the gap beetween C++ and C# event handlers
     *
     * As a Messaging::EventHandler, it can be used as an event handler of the native event queue.
     * Once it receive a message, it converts it to the C# format (AlgoEventArgs) and forward it
     * to the C# delegate provided in its constructor.
     *
     */
    public class EventForwarder : public Events::EventHandler {

        public:
            EventForwarder(System::EventHandler^ delegate) { _delegate = delegate; }

            void HandleEvent(Events::Event const& e) {
                String^ aStr = gcnew String(e.message.c_str());
                Severity sev;
                switch (e.severity) {
                case Events::Severity::Debug:
                    sev = Severity::Debug;
                    break;
                case Events::Severity::Error:
                    sev = Severity::Error;
                    break;
                case Events::Severity::Fatal:
                    sev = Severity::Fatal;
                    break;
                case Events::Severity::Info:
                    sev = Severity::Info;
                    break;
                case Events::Severity::Verbose:
                    sev = Severity::Verbose;
                    break;
                case Events::Severity::Warning:
                    sev = Severity::Warning;
                    break;
                }
                AlgoEventArgs^ arg = gcnew AlgoEventArgs(aStr, sev);
                _delegate->Invoke(_delegate, arg);
            }

            System::EventHandler^ Delegate() { return _delegate; }

        private: gcroot<System::EventHandler^> _delegate;
    };

    /*!
     * Acts as a wrapper around the native event queue.
     * One can add C# event handler to the native queue, and events
     * will be automatically adapted thanks to EventForwarder.
     */
    public ref class ManagedEventQueue {

    public:
        ManagedEventQueue();
        ~ManagedEventQueue();

        /*!
         * Registers an event handler, which will be called each time
         * an event is posted in the managed event queue.
         */
        void AddMessageEventHandler(System::EventHandler^ delegate);

        /*!
         * Remove an event handler from the managed queue event. It will
         * no longer receive any events.
         */
        void RemoveMessageEventHandler(System::EventHandler^ newDelegate);

        /*!
         * Posting here will post on the native side, ie native event handler will be notified
         * as well as Csharp event handlers.
         */
        void Post(String^ message, Severity severity);

    private:
        Events::EventQueue* _referenceOnCppEventQueue;
        std::vector<EventForwarder*>* _eventForwarders;
    };
}
