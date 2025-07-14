#pragma once
class CInterpolator2DBase;

#pragma managed
namespace UnitySCSharedAlgosCppWrapper {

    // SCATTERED DATA INTERPOLATION
    // For Grid M x N with p inputs

    public enum class InterpolateAlgoType
    {
        Unknown, // Not defined / unknown
        MBA,     // Multilevel B-Spiine Approximation  - o(p+MN)
        IDW,     // Inverse Distance Weigthing (Shepard) - o(pMN) 
        fNN,     //  first Nearest Neighbor - o(log p)
        QuadNN,  //  4 Nearest Neighbor + quadratic interpolation - o( p log p)
    };

    public ref class Interpolator2D
    {
    public:
        Interpolator2D(InterpolateAlgoType Algotype);
        Interpolator2D(InterpolateAlgoType Algotype, CInterpolator2DBase* native);
        Interpolator2D(const Interpolator2D^& copier);
        ~Interpolator2D();
        !Interpolator2D();

        bool InitSettings(array<double>^ settingList); //en attendant de passer des strcut pointer marshalisé ... on utilise un tableau de double
        bool SetInputsPoints(array<double>^ coordX, array<double>^ coordY, array<double>^ value);
        bool SetInputsPoints(array<double>^ coordX, array<double>^ coordY, array<double>^ value1, array<double>^ value2);
        bool AddInputsPoint(double coordX, double coordY, double value);
        void ResetInputsPoints();
        bool ComputeData();

        double Interpolate(double x, double y);
        void Interpolate(double x, double y, double % out1, double % out2);

        bool InterpolateGrid(unsigned long long pGridArray, int gridWidth, int gridHeight, double xStart, double xStep, double yStart, double yStep);
        bool InterpolateGrid(unsigned long long pGridArray1, unsigned long long pGridArray2, int gridWidth, int gridHeight, double xStart, double xStep, double yStart, double yStep);

    private:
        CInterpolator2DBase* _native = nullptr;
        InterpolateAlgoType _algotype = InterpolateAlgoType::Unknown;
    };
}

