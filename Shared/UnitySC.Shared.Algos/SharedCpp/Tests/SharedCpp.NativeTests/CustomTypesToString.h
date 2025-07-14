#pragma once
#include "CppUnitTest.h"

#include "ProfileStepAnalyser.h"

namespace Microsoft::VisualStudio::CppUnitTestFramework
{
    template<>
    std::wstring ToString<ProfileStepAnalyserResult::Status>(const ProfileStepAnalyserResult::Status& status)
    {
        switch (status)
        {
        case ProfileStepAnalyserResult::Status::OK:
            return L"OK";
        case ProfileStepAnalyserResult::Status::EMPTY_PROFILE:
            return L"EMPTY_PROFILE";
        case ProfileStepAnalyserResult::Status::EMPTY_PROFILE_NAN:
            return L"EMPTY_PROFILE_NAN";
        case ProfileStepAnalyserResult::Status::PROFILE_TOO_SMALL_AFTER_STD_DEV_FILTERING:
            return L"PROFILE_TOO_SMALL_AFTER_STD_DEV_FILTERING";
        default:
            return L"UNKNOWN_STATUS";
        }
    }

    template<>
    std::wstring ToString<Profile>(const Profile& profile)
    {
        return L"Profile (" + std::to_wstring(profile.Size()) + L" points)";
    }
}