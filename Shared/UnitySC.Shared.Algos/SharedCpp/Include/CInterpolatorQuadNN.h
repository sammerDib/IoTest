#pragma once
#include "CInterpolatorFNN.h"

#pragma unmanaged
class CInterpolatorQuadNN : public CInterpolatorFNN
{
public:
    class QCorner
    {
        public :
            input2d     m_pos;
            value2d     m_vals;
            double      m_angleToQCenter;
       public:
            QCorner(input2d& qpos, input2d& qcenter, value2d& qvals);
            QCorner(input2d& qpos, input2d& qcenter, double& val);
       private :
            void InitAngletoCenter(input2d& qcenter);
    };

public: 
    CInterpolatorQuadNN();
    CInterpolatorQuadNN(const CInterpolatorQuadNN& qnn);
    virtual ~CInterpolatorQuadNN();

    virtual bool InitSettings(int Nb, double* settingList);


    virtual double Interpolate(double x, double y);
    virtual void Interpolate(double x, double y, double& out1, double& out2);

private :
    void computeBilinear(void* kdquad, double x, double y, double& v1out, double& v2out);
    
    void FindQuadCorners(input2d& pos, void* kdQuad);
    std::vector<QCorner> OrderQuadCorners(void* kdQuad);
    void Bilerp(std::vector<QCorner>& orderedQCorners, double s, double t, double* x, double* y);
    int InverseBilerp(std::vector<CInterpolatorQuadNN::QCorner>& orderedQCorners, double x, double y, double* sout, double* tout, double* s2out, double* t2out);
   
    inline double dblLerp(double a, double b, double t) {
        return (a + t * (b - a));
        // note rti si c++20 use optimized std::lerp(a,b,t)
    }
    inline int equals(double a, double b, double tolerance) {
        return (a == b) || ((a <= (b + tolerance)) && (a >= (b - tolerance)));
    }

    inline double cross2(double x0, double y0, double x1, double y1){
        return x0 * y1 - y0 * x1;
    }

    inline int in_range(double val, double range_min, double range_max, double tol){
        return ((val + tol) >= range_min) && ((val - tol) <= range_max);
    }

private:
    double m_WaferAngle_inRad = 0.0;
    double m_WaferTx_inmm = 0.0;
    double m_WaferTy_inmm = 0.0;
    double m_GridStep_inmm = 0.015;
    double m_Margin_inmm = 0.005;

};

