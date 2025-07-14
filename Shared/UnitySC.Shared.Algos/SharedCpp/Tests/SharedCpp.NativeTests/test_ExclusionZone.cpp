#include "pch.h"
#include "CppUnitTest.h"
#include "CustomTypesToString.h"

#include "ExclusionZone.h"
#include "Profile.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace SharedCpp_NativeTests
{
    TEST_CLASS(ExclusionZoneTests)
    {
    public:
        TEST_METHOD(ApplyExclusionZone_OneZone)
        {
            Profile profile({ { 0.0, 0.0 }, { 0.1, 0.0 }, { 0.2, 0.0 }, { 0.3, 0.0 }, { 0.4, 0.0 },
                {0.5, 10}, { 0.6, 10 }, { 0.7, 10 }, { 0.8, 10 }, { 0.9, 10 }, { 1.0, 10 } });

            ExclusionZone exclusionZone(0.45, 0.16, 0.25);

            Profile expected({ 
                { 0.0, 0.0 }, { 0.1, 0.0 }, { 0.2, 0.0 }, { 0.8, 10 }, { 0.9, 10 }, { 1.0, 10 } });

            profile.RemoveExclusionZone(exclusionZone);

            Assert::AreEqual(expected, profile);
        }

        TEST_METHOD(ApplyExclusionZone_LeftOnly)
        {
            Profile profile({ { 0.0, 0.0 }, { 0.1, 0.0 }, { 0.2, 0.0 }, { 0.3, 0.0 }, { 0.4, 0.0 },
                {0.5, 10}, { 0.6, 10 }, { 0.7, 10 }, { 0.8, 10 }, { 0.9, 10 }, { 1.0, 10 } });

            ExclusionZone exclusionZone(0.45, 0.16, 0.0);

            Profile expected({
                { 0.0, 0.0 }, { 0.1, 0.0 }, { 0.2, 0.0 }, 
                {0.5, 10}, { 0.6, 10 }, { 0.7, 10 }, { 0.8, 10 }, { 0.9, 10 }, { 1.0, 10 } });

            profile.RemoveExclusionZone(exclusionZone);

            Assert::AreEqual(expected, profile);
        }

        TEST_METHOD(ApplyExclusionZone_RightOnly)
        {
            Profile profile({ { 0.0, 0.0 }, { 0.1, 0.0 }, { 0.2, 0.0 }, { 0.3, 0.0 }, { 0.4, 0.0 },
                {0.5, 10}, { 0.6, 10 }, { 0.7, 10 }, { 0.8, 10 }, { 0.9, 10 }, { 1.0, 10 } });

            ExclusionZone exclusionZone(0.45, 0.0, 0.25);

            Profile expected({
                { 0.0, 0.0 }, { 0.1, 0.0 }, { 0.2, 0.0 }, { 0.3, 0.0 }, { 0.4, 0.0 },
                { 0.8, 10 }, { 0.9, 10 }, { 1.0, 10 } });

            profile.RemoveExclusionZone(exclusionZone);

            Assert::AreEqual(expected, profile);
        }

        TEST_METHOD(ApplyExclusionZone_MultipleZones)
        {
            Profile profile({ {-1.0, 10}, {-0.9, 10}, {-0.8, 10}, {-0.7, 10}, {-0.6, 10}, { -0.5, 10 },
            {-0.4, 0.0}, {-0.3, 0.0}, { -0.2, 0.0 }, { -0.1, 0.0 }, { 0.0, 0.0 }, { 0.1, 0.0 }, { 0.2, 0.0 }, { 0.3, 0.0 }, { 0.4, 0.0 },
            {0.5, 10}, { 0.6, 10 }, { 0.7, 10 }, { 0.8, 10 }, { 0.9, 10 }, { 1.0, 10 } });

            ExclusionZone start(-1.0, 0.0, 0.15);
            ExclusionZone left(-0.5, 0.15, 0.15);
            ExclusionZone right(0.4, 0.15, 0.15);
            ExclusionZone end(1.0, 0.15, 0.0);

            profile.RemoveExclusionZone(start);
            profile.RemoveExclusionZone(left);
            profile.RemoveExclusionZone(right);
            profile.RemoveExclusionZone(end);

            Profile expected({ {-0.8, 10}, {-0.7, 10},
                {-0.3, 0.0}, {-0.2, 0.0}, {-0.1, 0.0}, {0.0, 0.0}, {0.1, 0.0}, {0.2, 0.0},
                {0.6, 10}, {0.7, 10}, {0.8, 10} });

            Assert::AreEqual(expected, profile);
        }

        TEST_METHOD(ApplyExclusionZone_None)
        {
            Profile profile({ { 0.0, 0.0 }, { 0.1, 0.0 }, { 0.2, 0.0 }, { 0.3, 0.0 }, { 0.4, 0.0 },
                {0.5, 10}, { 0.6, 10 }, { 0.7, 10 }, { 0.8, 10 }, { 0.9, 10 }, { 1.0, 10 } });

            ExclusionZone exclusionZone(0.45, 0.01, 0.01);

            Profile expected({ { 0.0, 0.0 }, { 0.1, 0.0 }, { 0.2, 0.0 }, { 0.3, 0.0 }, { 0.4, 0.0 },
                {0.5, 10}, { 0.6, 10 }, { 0.7, 10 }, { 0.8, 10 }, { 0.9, 10 }, { 1.0, 10 } });

            profile.RemoveExclusionZone(exclusionZone);

            Assert::AreEqual(expected, profile);
        }

        TEST_METHOD(ApplyExclusionZone_LowerLeft)
        {
            Profile profile({ { 0.0, 0.0 }, { 0.1, 0.0 }, { 0.2, 0.0 }, { 0.3, 0.0 } });
            ExclusionZone exclusionZone(0.15, 100, 0.01);

            Profile expected({ { 0.2, 0.0 }, { 0.3, 0.0 } });

            profile.RemoveExclusionZone(exclusionZone);

            Assert::AreEqual(expected, profile);
        }

        TEST_METHOD(ApplyExclusionZone_HigherRight)
        {
            Profile profile({ { 0.0, 0.0 }, { 0.1, 0.0 }, { 0.2, 0.0 }, { 0.3, 0.0 } });
            ExclusionZone exclusionZone(0.15, 0.01, 100.0);

            Profile expected({ { 0.0, 0.0 }, { 0.1, 0.0 } });

            profile.RemoveExclusionZone(exclusionZone);

            Assert::AreEqual(expected, profile);
        }
    };
}