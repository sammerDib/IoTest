#pragma once
#pragma managed

class ExclusionZone;

namespace UnitySCSharedAlgosCppWrapper {
    public ref class ExclusionZone
    {
    public:
        ExclusionZone(const double x, const double left, const double right);
        ExclusionZone(const double left, const double right);
        ~ExclusionZone();
        !ExclusionZone();

        const ::ExclusionZone& GetNative();

    private:
        ::ExclusionZone* _native;
    };
}
