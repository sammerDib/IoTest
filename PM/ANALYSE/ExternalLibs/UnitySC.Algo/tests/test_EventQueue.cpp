
#include <EventQueue.hpp>
#include <gtest/gtest.h>

class EventSpooler : public Events::EventHandler {
  int _handled;

public:
  EventSpooler() : _handled(0) {}
  int HowManyHandled() { return _handled; }
  void HandleEvent(Events::Event const &e) override { _handled++; }
};

TEST(EventQueue, Expect_Event_To_Be_dispatched_as_soon_as_posted) {

  Events::Event e("kikoo", Events::Severity::Debug);

  EventSpooler counter;

  Events::EventQueue::Instance()->AddEventHandler(&counter);

  Events::EventQueue::Instance()->Post(e);

  bool handlerHasProcessedTheEvent = (counter.HowManyHandled() == 1);

  Events::EventQueue::Instance()->RemoveEventHandler(&counter);

  EXPECT_TRUE(handlerHasProcessedTheEvent);
}
