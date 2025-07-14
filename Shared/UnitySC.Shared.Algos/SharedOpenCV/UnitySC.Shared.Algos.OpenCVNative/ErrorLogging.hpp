#pragma once

#include "LoggerOCV.hpp"
#include <exception>
#include <utility>

#pragma unmanaged
class ErrorLogging {
public:
    template<typename ... Args>
    static void LogErrorAndThrow(Args... messageElements) {
        std::string message = BuildMessage(std::forward<Args>(messageElements)...);
        LoggerOCV::Error(message);
        throw std::exception(message.c_str());
    }

    template<typename ... Args>
    static void LogError(Args... messageElements) {
        std::string message = BuildMessage(std::forward<Args>(messageElements)...);
        LoggerOCV::Error(message);
    }

private:
    template<typename ... Args>
    static std::string BuildMessage(Args... messageElements) {
        std::ostringstream messageStream;
        FillMessageStream(messageStream, messageElements...);
        return messageStream.str();
    }

    template<typename T, typename ... Args>
    static void FillMessageStream(std::ostringstream& stream, T messageElement, Args... messageElements) {
        stream << messageElement;
        FillMessageStream(stream, messageElements...);
    }

    template<typename T>
    static void FillMessageStream(std::ostringstream& stream, T onlyMessageElement) {
        stream << onlyMessageElement;
    }
};