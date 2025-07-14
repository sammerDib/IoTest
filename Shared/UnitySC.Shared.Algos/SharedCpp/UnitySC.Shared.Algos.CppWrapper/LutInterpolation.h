#pragma once
class CLutInterpolation;

#pragma managed
namespace UnitySCSharedAlgosCppWrapper{
    public ref class LutInterpolation
    {
    public:
        LutInterpolation();
        LutInterpolation(CLutInterpolation* native);
        LutInterpolation(const LutInterpolation^& copier);
     
        ~LutInterpolation();
        !LutInterpolation();

        const double Y(const double x);
        const double X(const double y);

        void CopyArraysFrom(const int size, const unsigned long long pX, const unsigned long long pY);

    private:
        CLutInterpolation* _native = nullptr;
    };
}
