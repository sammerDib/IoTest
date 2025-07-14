#include "CInterpolatorQuadNN.h"

#pragma unmanaged
#include <thread>
#include <cmath>
#include "kdtree.hpp"
#include <algorithm>

#define _USE_MATH_DEFINES
#include <math.h>

#pragma region CInterpolatorQuadNN_QCorner_INLINES
inline CInterpolatorQuadNN::QCorner::QCorner(input2d& qpos, input2d& qcenter, value2d& qvals) {
    m_pos = qpos;
    m_vals = qvals;
    InitAngletoCenter(qcenter);
}

inline CInterpolatorQuadNN::QCorner::QCorner(input2d& qpos, input2d& qcenter, double& val) {
    m_pos = qpos;
    m_vals.push_back(val); m_vals.push_back(std::nan("1"));
    InitAngletoCenter(qcenter);
}

inline void CInterpolatorQuadNN::QCorner::InitAngletoCenter(input2d& qcenter) {
    m_angleToQCenter = atan2(m_pos[1] - qcenter[1], m_pos[0] - qcenter[0]);
}
#pragma endregion


CInterpolatorQuadNN::CInterpolatorQuadNN()
    : CInterpolatorFNN()
{}

CInterpolatorQuadNN::CInterpolatorQuadNN(const CInterpolatorQuadNN& interp)
    : CInterpolatorFNN(interp)
{}

CInterpolatorQuadNN::~CInterpolatorQuadNN()
{
}


bool CInterpolatorQuadNN::InitSettings(int Nb, double* settingList)
{
    ClearNativeInstance();
    ResetInputsPoints();

    if ((Nb != 4 && Nb != 5) || settingList == nullptr)
        return false;

    // Wafer Angle and center shift
    m_WaferAngle_inRad = M_PI * settingList[0] / 180.0;
    m_WaferTx_inmm = settingList[1];
    m_WaferTy_inmm = settingList[2];
    // Grig Step
    m_GridStep_inmm = settingList[3];
    //  margin [optionnal]
    if (Nb >= 5)
        m_Margin_inmm = settingList[4];

    return true;
}


void CInterpolatorQuadNN::FindQuadCorners(input2d& pos, void* kdQuad)
{
    Kdtree::KdNodeVector* results = (static_cast<Kdtree::KdNodeVector*>(kdQuad));

    static_cast<Kdtree::KdTree*>(_pKdTreeInst_ptr)->k_nearest_neighbors(pos, 1, results);  
    if (results->size() < 1)
        return;

    double minimalDist = 1e-9; //1 nm
    double difX = pos[0] - (*results)[0].point[0];
    double difY = pos[1] - (*results)[0].point[1];
    if (abs(difX) < minimalDist && abs(difY) < minimalDist)
        return;  // consider point equal to the closest quad corner
   
    Kdtree::KdNodePredicate* predicate;
    if (difX < 0.0)
    {
        if (difY < 0.0)
        {
            // Bottom Left Quad to fNN
            struct PredicateBL : Kdtree::KdNodePredicate {
                std::vector<double> point;
                double tolX;
                double tolY;
                PredicateBL(std::vector<double> p, double angle_rd, double step, double tolerance) {
                    this->point = p;
                    double sign = (angle_rd < 0.0) ? 1.0 : 0.0;
                    tolX = tolerance + sign * abs(step * sin(angle_rd));
                    tolY = tolerance + (1.0 - sign) * abs( step * sin(angle_rd) );
                }

                bool operator()(const Kdtree::KdNode& kn) const {
                    return ((this->point[0] + tolX) > kn.point[0]) && (this->point[1] + tolY) > kn.point[1];
                }
            };
            predicate = new PredicateBL((*results)[0].point, m_WaferAngle_inRad, m_GridStep_inmm, m_Margin_inmm);
        }
        else
        {
            // Top Left Quad to fNN
            struct PredicateTL : Kdtree::KdNodePredicate {
                std::vector<double> point;
                double tolX;
                double tolY;
                PredicateTL(std::vector<double> p, double angle_rd, double step, double tolerance) {
                    this->point = p;
                    double sign = (angle_rd < 0.0) ? 1.0 : 0.0;
                    tolX = tolerance + (1.0 - sign) * abs(step * sin(angle_rd));
                    tolY = tolerance + sign * abs(step * sin(angle_rd));
                }

                bool operator()(const Kdtree::KdNode& kn) const {
                    return ((this->point[0] + tolX) > kn.point[0]) && (this->point[1] - tolY) < kn.point[1];
                }
            };
            predicate = new PredicateTL((*results)[0].point, m_WaferAngle_inRad, m_GridStep_inmm, m_Margin_inmm);
        }
    }
    else
    {
        if (difY < 0.0)
        {
            // Bottom Right Quad to fNN
            struct PredicateBR : Kdtree::KdNodePredicate {
                std::vector<double> point;
                double tolX;
                double tolY;
                PredicateBR(std::vector<double> p, double angle_rd, double step, double tolerance) {
                    this->point = p;
                    double sign = (angle_rd < 0.0) ? 1.0 : 0.0;
                    tolX = tolerance + (1.0 - sign) * abs(step * sin(angle_rd));
                    tolY = tolerance + sign * abs(step * sin(angle_rd));
                }

                bool operator()(const Kdtree::KdNode& kn) const {
                    return ((this->point[0] - tolX) < kn.point[0]) && (this->point[1] + tolY) > kn.point[1];
                }
            };
            predicate = new PredicateBR((*results)[0].point, m_WaferAngle_inRad, m_GridStep_inmm, m_Margin_inmm);
        }
        else
        {
            // Top Right Quad to fNN
            struct PredicateTR : Kdtree::KdNodePredicate {
                std::vector<double> point;
                double tolX;
                double tolY;
                PredicateTR(std::vector<double> p, double angle_rd, double step, double tolerance) {
                    this->point = p;
                    double sign = (angle_rd < 0.0) ? 1.0 : 0.0;
                    tolX = tolerance + sign * abs( step * sin(angle_rd));
                    tolY = tolerance + (1.0 - sign) * abs(step * sin(angle_rd));
                }

                bool operator()(const Kdtree::KdNode& kn) const {
                    return ((this->point[0] - tolX) < kn.point[0]) && (this->point[1] - tolY) < kn.point[1];
                }
            };
            predicate = new PredicateTR((*results)[0].point, m_WaferAngle_inRad, m_GridStep_inmm, m_Margin_inmm);
        }
    }

    static_cast<Kdtree::KdTree*>(_pKdTreeInst_ptr)->k_nearest_neighbors(pos, 4, results, predicate);
    
    delete predicate;
    predicate = nullptr;
}

std::vector<CInterpolatorQuadNN::QCorner> CInterpolatorQuadNN::OrderQuadCorners(void* kdQuad)
{
    Kdtree::KdNodeVector quad = *(static_cast<Kdtree::KdNodeVector*>(kdQuad));

    input2d qcenter;
    qcenter.push_back((quad[0].point[0] + quad[1].point[0] + quad[2].point[0] + quad[3].point[0]) * 0.25);
    qcenter.push_back((quad[0].point[1] + quad[1].point[1] + quad[2].point[1] + quad[3].point[1]) * 0.25);
    
    std::vector<QCorner> corners;
    if (_useV1)
    {
        for (auto it = quad.begin(); it != quad.end(); ++it)
        {
            corners.push_back(QCorner(it->point, qcenter, *(static_cast<double*>(it->data))));
        }
    }
    else
    {
        for (auto it = quad.begin(); it != quad.end(); ++it)
        {
            corners.push_back(QCorner(it->point, qcenter, *(static_cast<value2d*>(it->data))));
        }
    }

    // order corner clockwise
    sort(corners.begin(), corners.end(),
    [](const CInterpolatorQuadNN::QCorner p1, const CInterpolatorQuadNN::QCorner& p2) -> bool
    {
        return p1.m_angleToQCenter < p2.m_angleToQCenter;
    });
    return corners;
}

/// <summary>
///  retreive s and t parameter of the  x, y point to interpolate within a Quad
///               s  
///          <--------->          
///         Q4---------|-----Q3
///          |         |      |
///      t ^ |-------(x,y)----|
///        | |         |      |  
///         Q1---------|-----Q2      
/// 
/// if return 2
///     Soluce 2 can occur when x,y is a quite close to a quad corner or in certain case of interpolation. it should be use.
/// if return 1
///     Soluce 2 is left empty, only result in soluce 1
/// </summary>
/// <param name="orderedQCorners"></param>
/// <param name="x"> x coordinate to interpolate</param>
/// <param name="y"> Y coordinate to interpolate</param>
/// <param name="sout"> Soluce1 : s percent from low Left corner quad to Right corners of quad</param>
/// <param name="tout"> Soluce1 : t percent from low Left corner quad to Top corners of quad</param>
/// <param name="s2out"> Soluce2 : s percent from low Left corner quad to Right corners of quad</param>
/// <param name="t2out"> Soluce2 : s percent from low Left corner quad to Right corners of quad</param>
/// <returns></returns>
int CInterpolatorQuadNN::InverseBilerp(std::vector<CInterpolatorQuadNN::QCorner>& orderedQCorners, double x, double y, double* sout, double* tout, double* s2out, double* t2out)
{
    int t_valid, t2_valid;

    // lower left - 00 - idxQC0
    double x0 = orderedQCorners[0].m_pos[0]; double y0 = orderedQCorners[0].m_pos[1];
    // lower right - 01 - idxQC1
    double x1 = orderedQCorners[1].m_pos[0]; double y1 = orderedQCorners[1].m_pos[1];
    // upper left - 10 - idxQC3
    double x2 = orderedQCorners[3].m_pos[0]; double y2 = orderedQCorners[3].m_pos[1];
    // upper right - 11 - idxQC2
    double x3 = orderedQCorners[2].m_pos[0]; double y3 = orderedQCorners[2].m_pos[1];

    double a = cross2(x0 - x, y0 - y, x0 - x2, y0 - y2);
    double b1 = cross2(x0 - x, y0 - y, x1 - x3, y1 - y3);
    double b2 = cross2(x1 - x, y1 - y, x0 - x2, y0 - y2);
    double c = cross2(x1 - x, y1 - y, x1 - x3, y1 - y3);
    double b = 0.5 * (b1 + b2);

    double s, s2, t, t2;

    double am2bpc = a - 2 * b + c;
    
    int num_valid_s = 0; // number of available solution found for s

    if (equals(am2bpc, 0, 1e-10))
    {
        if (equals(a - c, 0, 1e-10))
        {
            // Quad might be aligned cant comput inverse bilerp
            return 0;
        }
        s = a / (a - c);
        if (in_range(s, 0, 1, 1e-10))
            num_valid_s = 1;
    }
    else
    {
        double sqrtbsqmac = sqrt(b * b - a * c);
        s = ((a - b) - sqrtbsqmac) / am2bpc;
        s2 = ((a - b) + sqrtbsqmac) / am2bpc;
        num_valid_s = 0;
        if (in_range(s, 0, 1, 1e-10))
        {
            num_valid_s++;
            if (in_range(s2, 0, 1, 1e-10))
                num_valid_s++;
        }
        else
        {
            if (in_range(s2, 0, 1, 1e-10))
            {
                num_valid_s++;
                s = s2;
            }
        }
    }

    if (num_valid_s == 0)
        return 0; //no solution - exit

    t_valid = 0;
    if (num_valid_s >= 1)
    {
        double tdenom_x = (1 - s) * (x0 - x2) + s * (x1 - x3);
        double tdenom_y = (1 - s) * (y0 - y2) + s * (y1 - y3);
        t_valid = 1;
        if (equals(tdenom_x, 0, 1e-10) && equals(tdenom_y, 0, 1e-10)){
            t_valid = 0;
        }
        else
        {
            // Choose the more robust denominator
            if (fabs(tdenom_x) > fabs(tdenom_y)){
                t = ((1 - s) * (x0 - x) + s * (x1 - x)) / (tdenom_x);
            }
            else {
                t = ((1 - s) * (y0 - y) + s * (y1 - y)) / (tdenom_y);
            }
            // decomment this if you don't allow extrapolation outer the quads
            //if (!in_range(t, 0, 1, 1e-10))
            //    t_valid = 0;
        }
    }

    // Same thing for s2 and t2
    t2_valid = 0;
    if (num_valid_s == 2)
    {
        double tdenom_x = (1 - s2) * (x0 - x2) + s2 * (x1 - x3);
        double tdenom_y = (1 - s2) * (y0 - y2) + s2 * (y1 - y3);
        t2_valid = 1;
        if (equals(tdenom_x, 0, 1e-10) && equals(tdenom_y, 0, 1e-10)){
            t2_valid = 0;
        }
        else
        {
            // Choose the more robust denominator 
            if (fabs(tdenom_x) > fabs(tdenom_y)){
                t2 = ((1 - s2) * (x0 - x) + s2 * (x1 - x)) / (tdenom_x);
            }
            else {
                t2 = ((1 - s2) * (y0 - y) + s2 * (y1 - y)) / (tdenom_y);
            }
            // decomment this if you don't allow extrapolation outer the quads
            //if (!in_range(t2, 0, 1, 1e-10))
            //    t2_valid = 0;
        }
    }

    // Final cleanup, prefered s2 & t2 if s & t not valid  
    if (t2_valid && !t_valid)
    {
        s = s2;
        t = t2;
        t_valid = t2_valid;
        t2_valid = 0;
    }

    // update Outputs
    if (t_valid)
    {
        *sout = s;
        *tout = t;
    }

    if (t2_valid)
    {
        *s2out = s2;
        *t2out = t2; 
    }//warning s2out & t2out undefined if only 1 solution is found

    return t_valid + t2_valid;
}

/// <summary>
///  performing bilinear interpolation on values in quad,
///     s and t parameter in percent of the  x, y point to interpolate within a Quad
///               s  
///          <--------->          
///         Q4---------|-----Q3
///          |         |      |
///      t ^ |-------(x,y)----|
///        | |         |      |  
///         Q1---------|-----Q2      
/// 
/// </summary>
/// <param name="orderedQCorners"></param>
/// <param name="s">  coordinate to interpolate</param>
/// <param name="t">  coordinate to interpolate</param>
/// <param name="v1"> value 1 interpolated </param>
/// <param name="v2"> value  interpolated if(!_useV1) </param>
/// <returns></returns>
void CInterpolatorQuadNN::Bilerp(std::vector<QCorner>& orderedQCorners, double s, double t, double* v1, double* v2)
{
    double* pv[2] = { v1, v2 };
    int nbVal = _useV1 ? 1 : 2;
    for (int i = 0; i < nbVal; i++)
    {
        double valtop = dblLerp(orderedQCorners[3].m_vals[i], orderedQCorners[2].m_vals[i], s);
        double valbottom = dblLerp(orderedQCorners[0].m_vals[i], orderedQCorners[1].m_vals[i], s);
        *(pv[i]) = dblLerp(valbottom, valtop, t);
    }
}

void CInterpolatorQuadNN::computeBilinear(void* kdquad, double x, double y, double& v1out, double& v2out)
{
    auto orderedQuad = OrderQuadCorners(kdquad);

    double s2, s=0.0;
    double t2, t=0.0;
    int nbSoluce = InverseBilerp(orderedQuad, x, y, &s, &t, &s2, &t2);
    switch (nbSoluce)
    {
    case 1 :
        Bilerp(orderedQuad, s, t, &v1out, &v2out);
        break;
    case 2 :
        Bilerp(orderedQuad, s2, t2, &v1out, &v2out);
        break;
    case 0 :
     [[fallthrough]];
    default:
        // should be treated maybe like an afterwards
        v1out = std::nan("1");
        v2out = std::nan("1");
        break;
    }
}


double CInterpolatorQuadNN::Interpolate(double x, double y)
{
    if (_isComputed)
    {
        input2d Pti = { x,y };
        Kdtree::KdNodeVector results;
        
        FindQuadCorners(Pti, &results);

        if(results.size() == 0)
            return std::nan("1");

        if (results.size() != 4)
        {
            // if not quad return nearest neightbord
            if (_useV1)
                return *(static_cast<double*>(results[0].data));
            else
                return (*(static_cast<value2d*>(results[0].data)))[0];
        }
      
        double tmp, val;
        computeBilinear(&results, x, y, val, tmp);
        if (isnan(val))
        {
            // cannot extrapolate should be outside quad use nearest neighbor
            if (_useV1)
                return *(static_cast<double*>(results[0].data));
            else
                return (*(static_cast<value2d*>(results[0].data)))[0];
        }  
        return val;
    }
    return std::nan("1");
}

/// <summary>
/// Interpolate point with coordinate x, y and place its result in out1 and out2, if only one value avalaible, nan is set to out2
/// </summary>
/// <param name="x"></param>
/// <param name="y"></param>
/// <param name="out1"></param>
/// <param name="out2"></param>
void  CInterpolatorQuadNN::Interpolate(double x, double y, double& out1, double& out2)
{
    out1 = std::nan("1");
    out2 = std::nan("1");

    if (_isComputed)
    {
        input2d Pti = { x,y };
        Kdtree::KdNodeVector results;
  
        FindQuadCorners(Pti, &results);

        if (results.size() == 0)
            return;

        if (results.size() != 4)
        {
            if (_useV1)
            {
                out1 = *(static_cast<double*>(results[0].data));
                out2 = std::nan("1");
            }
            else
            {
                value2d values2 = *(static_cast<value2d*>(results[0].data));
                out1 = values2[0];
                out2 = values2[1];
            }
        }
        else
        {
            computeBilinear(&results, x, y, out1, out2);
            if (isnan(out1))
                out1 = (*(static_cast<value2d*>(results[0].data)))[0];  // cannot extrapolate should be outside quad so use nearest neighbor
            if (!_useV1 && isnan(out2))
                out2 = (*(static_cast<value2d*>(results[0].data)))[1];  // cannot extrapolate should be outside quad so use nearest neighbor
        }
    }
}




