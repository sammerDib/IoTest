#pragma once

#define NATIVE_CODE
#include <EventQueue.hpp>

#include <Event.hpp>
#include <EventHandler.hpp>
#include <algorithm>
#include <mutex>
#include <queue>

namespace Events {

  void EventQueue::Post(Event const &e) {
    {
      std::lock_guard<std::mutex> lock(_mutex);
      _events.emplace(e);
    }
    DispatchEvents();
  }

  void EventQueue::AddEventHandler(EventHandler *handler) {
    std::lock_guard<std::mutex> lock(_mutex);
    _eventHandlers.push_back(handler);
  }
  void EventQueue::RemoveEventHandler(EventHandler *handler) {
    std::lock_guard<std::mutex> lock(_mutex);
    auto it = std::remove(_eventHandlers.begin(), _eventHandlers.end(), handler);
    if (it != _eventHandlers.end()) {
      _eventHandlers.erase(it, _eventHandlers.end());
    }
  }

  void EventQueue::DispatchEvents() {

    std::queue<Event> eventsCopy;
    {
      std::lock_guard<std::mutex> lock(_mutex);
      eventsCopy = std::queue<Event>(_events);
      _events = std::queue<Event>();
    }

    while (!eventsCopy.empty()) {
      Event e = eventsCopy.front();
      eventsCopy.pop();
      for (auto handler : _eventHandlers) {
        handler->HandleEvent(e);
      }
    }
  }

  int EventQueue::Size() {
    std::lock_guard<std::mutex> lock(_mutex);
    return _events.size();
  }

  EventQueue EventQueue::_instance;
} // namespace Events
