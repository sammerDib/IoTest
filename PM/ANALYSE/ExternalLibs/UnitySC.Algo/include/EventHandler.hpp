#pragma once
#include <Event.hpp>

namespace Events {
  class EventHandler {
public:
  virtual void HandleEvent(Events::Event const &e) = 0;
};
} // namespace Event
