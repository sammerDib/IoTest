#include "CppUnitTest.h"

#pragma unmanaged

#include "CEventQueue.hpp"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace SharedOpenCVNativeTests
{
    class EventSpooler : public Events::EventHandler {
        int _handled;

    public:
        EventSpooler() : _handled(0) {}
        int HowManyHandled() { return _handled; }
        void HandleEvent(Events::Event const& e) override { _handled++; }
    };

	TEST_CLASS(EventTests)
	{
	public:
		
		TEST_METHOD(Expect_Event_To_Be_dispatched_as_soon_as_posted)
		{
            Logger::WriteMessage("create Event kikko Debug \n");
            Events::Event e("kikoo", Events::Severity::Debug);
            EventSpooler counter;
            Events::EventQueue::Instance()->AddEventHandler(&counter);
            Logger::WriteMessage("Post Event\n");
            Events::EventQueue::Instance()->Post(e);
            bool handlerHasProcessedTheEvent = (counter.HowManyHandled() == 1);
            Logger::WriteMessage("Event posted\n");
            Events::EventQueue::Instance()->RemoveEventHandler(&counter);
            Logger::WriteMessage("remove handler\n");
            Assert::IsTrue(handlerHasProcessedTheEvent, L"handler has not processed the event");
            Logger::WriteMessage("Done\n");
            
		}
	};
}
