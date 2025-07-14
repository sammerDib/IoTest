#pragma once

#include <string>

namespace Events {

    /*
    * Events severity from less to most important
    */
    enum class Severity : unsigned char {
        Debug = 2,
        Verbose = 4, Info = 8,
        Warning = 16,
        Error = 32,
        Fatal = 64
    };

    class Event {

    public:
        static const Event EMPTY_EVENT;

        Event(std::string const& message, Severity severity) : message(message), severity(severity) {}

        const std::string message;
        const Severity severity;

    private:
        Event() : severity(Severity::Debug), message("") {}
    };
} // namespace Events