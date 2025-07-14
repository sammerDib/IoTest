#include "pch.h"
#include "CppUnitTest.h"

#include "kdtree.hpp"
#include <string>

#define _USE_MATH_DEFINES
#include <math.h> // for PI

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace SharedCpp_NativeTests
{
    TEST_CLASS(KDTreeTests)
    {
    private:
        static constexpr double _tolerance = 1e-6;

    public:
        TEST_METHOD(SimpleQuad_FirstNearest)
        {
            double inputpoints[9][2] = {
            { -75.0, 75.0 },   { 0.0, 75.0 },   { 75.0, 75.0  },
            { -75.0, 0.0 },    { 0.0, 0.0 },    { 75.0, 0.0  },
            { -75.0, -75.0 },  { 0.0, -75.0 },  { 75.0, -75.0  } };

            Kdtree::KdNodeVector nodes;
            for (int i = 0; i < 9; ++i) {
                std::vector<double> point(2);
                point[0] = inputpoints[i][0];
                point[1] = inputpoints[i][1];
                nodes.push_back(Kdtree::KdNode(point, 0, i));
            }

            Kdtree::KdTree tree(&nodes); // build tree
            std::vector<double> test_point(2);

            Kdtree::KdNodeVector result;

            //1 first nearest nearbord
            test_point[0] = -75.0;    test_point[1] = 75.0;
            tree.k_nearest_neighbors(test_point, 1, &result);
            Assert::AreEqual((int)result.size(), 1);
            Assert::AreEqual(result[0].index, 0);
           
            for (int i = 0; i < 9; ++i) 
            {
                for (int x = -2; x <= 2; x++)
                {
                    test_point[0] = inputpoints[i][0] + x;
                    for (int y = -2; y <= 2; y++)
                    {
                        test_point[1] = inputpoints[i][1] + y;
                        tree.k_nearest_neighbors(test_point, 1, &result);
                        Assert::AreEqual((int)result.size(), 1);
                        Assert::AreEqual(result[0].index, i);
                    }
                }
            }
        }

        TEST_METHOD(Simple_FirstNearest_Middle)
        {
            double inputpoints[9][2] = {
            { -75.0, 75.0 },   { 0.0, 75.0 },   { 75.0, 75.0  },
            { -75.0, 0.0 },    { 0.0, 0.0 },    { 75.0, 0.0  },
            { -75.0, -75.0 },  { 0.0, -75.0 },  { 75.0, -75.0  } };

            Kdtree::KdNodeVector nodes;
            for (int i = 0; i < 9; ++i) {
                std::vector<double> point(2);
                point[0] = inputpoints[i][0];
                point[1] = inputpoints[i][1];
                nodes.push_back(Kdtree::KdNode(point, 0, i));
            }

            Kdtree::KdTree tree(&nodes); // build tree
            std::vector<double> test_point(2);

            Kdtree::KdNodeVector result;

            //1 first nearest nearbord in the middle return which one ? 4 ? 5 ? 1 ? or 2 ? 
            test_point[0] = 37.5;    test_point[1] = 37.5;
            tree.k_nearest_neighbors(test_point, 1, &result);
            Assert::AreEqual((int)result.size(), 1);
            Assert::AreEqual(result[0].index, 4);

            //1 first nearest nearbord in the middle return which one ? 4 ? 3 ? 6 ? or 7 ? 
            test_point[0] = -37.5;    test_point[1] = -37.5;
            tree.k_nearest_neighbors(test_point, 1, &result);
            Assert::AreEqual((int)result.size(), 1);
            Assert::AreEqual(result[0].index, 4);

            //1 first nearest nearbord in the middle return which one ? 4 ? 3 ? 0 ? or 1 ? 
            test_point[0] = -37.5;    test_point[1] = 37.5;
            tree.k_nearest_neighbors(test_point, 1, &result);
            Assert::AreEqual((int)result.size(), 1);
            Assert::AreEqual(result[0].index, 4);

            //1 first nearest nearbord in the middle return which one ? 4 ? 5 ? 7 ? or 8 ? 
            test_point[0] = 37.5;    test_point[1] = -37.5;
            tree.k_nearest_neighbors(test_point, 1, &result);
            Assert::AreEqual((int)result.size(), 1);
            Assert::AreEqual(result[0].index, 4);

            //1 first nearest nearbord almost in the middle return which one ? 4 ? 5 ? 1 ? or 2 ? 
            test_point[0] = 37.50001;    test_point[1] = 37.50001;
            tree.k_nearest_neighbors(test_point, 1, &result);
            Assert::AreEqual((int)result.size(), 1);
            Assert::AreEqual(result[0].index, 2);

            /// ==> he is returning the most near parent root

        }

        TEST_METHOD(Simple_FirstNearest_Middle_nonBalanced_tree)
        {
            double inputpoints[12][2] = {
            { -75.0, 75.0 },   { 0.0, 75.0 },   { 75.0, 75.0  },
            { -75.0, 0.0 },    { 0.0, 0.0 },    { 75.0, 0.0  }, 
            { -75.0, -75.0 },  { 0.0, -75.0 },  { 75.0, -75.0  },
                                                                   { 155.0, 75.0  },
                                                                   { 155.0, 0.0  },
                                                                   { 155.0, -90.0  }
            };

            Kdtree::KdNodeVector nodes;
            for (int i = 0; i < 12; ++i) {
                std::vector<double> point(2);
                point[0] = inputpoints[i][0];
                point[1] = inputpoints[i][1];
                nodes.push_back(Kdtree::KdNode(point, 0, i));
            }

            Kdtree::KdTree tree(&nodes); // build tree
            std::vector<double> test_point(2);

            Kdtree::KdNodeVector result;

            //1 first nearest nearbord in the middle return which one ? 4 ? 5 ? 1 ? or 2 ? 
            test_point[0] = 37.5;    test_point[1] = 37.5;
            tree.k_nearest_neighbors(test_point, 1, &result);
            Assert::AreEqual((int)result.size(), 1);
            Assert::AreEqual(result[0].index, 2);

            //1 first nearest nearbord in the middle return which one ? 4 ? 3 ? 6 ? or 7 ? 
            test_point[0] = -37.5;    test_point[1] = -37.5;
            tree.k_nearest_neighbors(test_point, 1, &result);
            Assert::AreEqual((int)result.size(), 1);
            Assert::AreEqual(result[0].index, 4);

            //1 first nearest nearbord in the middle return which one ? 4 ? 3 ? 0 ? or 1 ? 
            test_point[0] = -37.5;    test_point[1] = 37.5;
            tree.k_nearest_neighbors(test_point, 1, &result);
            Assert::AreEqual((int)result.size(), 1);
            Assert::AreEqual(result[0].index, 4);

            //1 first nearest nearbord in the middle return which one ? 4 ? 5 ? 7 ? or 8 ? 
            test_point[0] = 37.5;    test_point[1] = -37.5;
            tree.k_nearest_neighbors(test_point, 1, &result);
            Assert::AreEqual((int)result.size(), 1);
            Assert::AreEqual(result[0].index, 4);

            //1 first nearest nearbord almost in the middle return which one ? 4 ? 5 ? 1 ? or 2 ? 
            test_point[0] = 37.50001;    test_point[1] = -37.50001;
            tree.k_nearest_neighbors(test_point, 1, &result);
            Assert::AreEqual((int)result.size(), 1);
            Assert::AreEqual(result[0].index, 8);
        }

        TEST_METHOD(SimpleQuad_QuadNearest)
        {
            double inputpoints[9][2] = {
            { -75.0, 75.0 },   { 0.0, 75.0 },   { 75.0, 75.0  },
            { -75.0, 0.0 },    { 0.0, 0.0 },    { 75.0, 0.0  },
            { -75.0, -75.0 },  { 0.0, -75.0 },  { 75.0, -75.0  } };

            Kdtree::KdNodeVector nodes;
            for (int i = 0; i < 9; ++i) {
                std::vector<double> point(2);
                point[0] = inputpoints[i][0];
                point[1] = inputpoints[i][1];
                nodes.push_back(Kdtree::KdNode(point, 0, i));
            }

            Kdtree::KdTree tree(&nodes); // build tree
            std::vector<double> test_point(2);

            Kdtree::KdNodeVector result;

            //1 first nearest nearbord
            test_point[0] = -70.0;    test_point[1] = 82.0;
            tree.k_nearest_neighbors(test_point, 4, &result);
            Assert::AreEqual((int)result.size(), 4);
            Assert::AreEqual(result[0].index, 0);
            Assert::AreEqual(result[1].index, 1);
            Assert::AreEqual(result[2].index, 3);
            Assert::AreEqual(result[3].index, 4);


            // TOP LEFT -----------------------
            test_point[0] = -0.2;    test_point[1] = 0.2;
            tree.k_nearest_neighbors(test_point, 4, &result);
            Assert::AreEqual((int)result.size(), 4);
            Assert::AreEqual(result[0].index, 4);
            Assert::AreEqual(result[1].index, 1);
            Assert::AreEqual(result[2].index, 3);
            Assert::AreEqual(result[3].index, 5); /// pas celui q'uon voudrait en fait  
            // angle par rapport au plus proche pour définir qual quadrant (gérer un tolérance et ajouter angle
            struct PredicateTL : Kdtree::KdNodePredicate {
                std::vector<double> point;
                double tol;
                PredicateTL(std::vector<double> p, double tolerance) {
                    this->point = p;
                    tol = tolerance;
                }
                
                bool operator()(const Kdtree::KdNode& kn) const {
                    //return this->point[1] > kn.point[1];// only search for points with smaller y-coordinate
                     return ((this->point[0] + tol ) > kn.point[0]) && (this->point[1] - tol) < kn.point[1];
                }
            };
            PredicateTL predTL(result[0].point, 1.0);
            tree.k_nearest_neighbors(test_point, 4, &result, &predTL);
            Assert::AreEqual((int)result.size(), 4);
            Assert::AreEqual(result[0].index, 4);
            Assert::AreEqual(result[1].index, 1);
            Assert::AreEqual(result[2].index, 3);
            Assert::AreEqual(result[3].index, 0); /// OK

            // BOTTOM LEFT -----------------------
            test_point[0] = -0.2;    test_point[1] = -0.2;
            tree.k_nearest_neighbors(test_point, 4, &result);
            Assert::AreEqual((int)result.size(), 4);
            Assert::AreEqual(result[0].index, 4);
            Assert::AreEqual(result[1].index, 7);
            Assert::AreEqual(result[2].index, 3);
            Assert::AreEqual(result[3].index, 5); /// pas celui q'uon voudrait en fait  

            struct PredicateBL : Kdtree::KdNodePredicate {
                std::vector<double> point;
                double tol;
                PredicateBL(std::vector<double> p, double tolerance) {
                    this->point = p;
                    tol = tolerance;
                }

                bool operator()(const Kdtree::KdNode& kn) const {
                    return ((this->point[0] + tol) > kn.point[0]) && (this->point[1] + tol) > kn.point[1];
                }
            };
            PredicateBL predBL(result[0].point, 1.0);
            tree.k_nearest_neighbors(test_point, 4, &result, &predBL);
            Assert::AreEqual((int)result.size(), 4);
            Assert::AreEqual(result[0].index, 4);
            Assert::AreEqual(result[1].index, 7);
            Assert::AreEqual(result[2].index, 3);
            Assert::AreEqual(result[3].index, 6);

            // TOP RIGHT -----------------------
            test_point[0] = 0.2;    test_point[1] = 0.2;
            tree.k_nearest_neighbors(test_point, 4, &result);
            Assert::AreEqual((int)result.size(), 4);
            Assert::AreEqual(result[0].index, 4);
            Assert::AreEqual(result[1].index, 1);
            Assert::AreEqual(result[2].index, 5);
            Assert::AreEqual(result[3].index, 3); /// pas celui q'uon voudrait en fait  

            struct PredicateTR : Kdtree::KdNodePredicate {
                std::vector<double> point;
                double tol;
                PredicateTR(std::vector<double> p, double tolerance) {
                    this->point = p;
                    tol = tolerance;
                }

                bool operator()(const Kdtree::KdNode& kn) const {
                    return ((this->point[0] - tol) < kn.point[0]) && (this->point[1] - tol) < kn.point[1];
                }
            };
            PredicateTR predTR(result[0].point, 1.0);
            tree.k_nearest_neighbors(test_point, 4, &result, &predTR);
            Assert::AreEqual((int)result.size(), 4);
            Assert::AreEqual(result[0].index, 4);
            Assert::AreEqual(result[1].index, 1);
            Assert::AreEqual(result[2].index, 5);
            Assert::AreEqual(result[3].index, 2); //OK

            // BOTTOM RIGHT -----------------------
            test_point[0] = 0.2;    test_point[1] = -0.2;
            tree.k_nearest_neighbors(test_point, 4, &result);
            Assert::AreEqual((int)result.size(), 4);
            Assert::AreEqual(result[0].index, 4);
            Assert::AreEqual(result[1].index, 7);
            Assert::AreEqual(result[2].index, 5);
            Assert::AreEqual(result[3].index, 3); /// pas celui q'uon voudrait en fait  

            struct PredicateBR : Kdtree::KdNodePredicate {
                std::vector<double> point;
                double tol;
                PredicateBR(std::vector<double> p, double tolerance) {
                    this->point = p;
                    tol = tolerance;
                }

                bool operator()(const Kdtree::KdNode& kn) const {
                    return ((this->point[0] - tol) < kn.point[0]) && (this->point[1] + tol) > kn.point[1];
                }
            };
            PredicateBR predBR(result[0].point, 1.0);
            Kdtree::KdNodePredicate* pred = (Kdtree::KdNodePredicate*)&predBR;
            tree.k_nearest_neighbors(test_point, 4, &result, pred);
            Assert::AreEqual((int)result.size(), 4);
            Assert::AreEqual(result[0].index, 4);
            Assert::AreEqual(result[1].index, 7);
            Assert::AreEqual(result[2].index, 5);
            Assert::AreEqual(result[3].index, 8); // OK
        }

        TEST_METHOD(SimpleQuad_QuadNearest_AnglePlus)
        {
            double inputpoints[9][2] = {
            { -75.0, 75.0 },   { 0.0, 75.0 },   { 75.0, 75.0  },
            { -75.0, 0.0 },    { 0.0, 0.0 },    { 75.0, 0.0  },
            { -75.0, -75.0 },  { 0.0, -75.0 },  { 75.0, -75.0  } };


            double step = inputpoints[1][0] - inputpoints[0][0]; //75;
            double margin = 0.05;
            double angleWaferRot_dg = 10.0;
            double angleWaferRot_rd = angleWaferRot_dg * M_PI / 180.0;
            double waferCenterX = 0.525;
            double waferCenterY = -1.24;

            double cosA = cos(angleWaferRot_rd);
            double sinA = sin(angleWaferRot_rd);
            for (int i = 0; i < 9; ++i) {
                double x = inputpoints[i][0]; double y = inputpoints[i][1];          
                inputpoints[i][0] = waferCenterX + (x - waferCenterX) * cosA + (y - waferCenterY) * sinA;
                inputpoints[i][1] = waferCenterY - (x - waferCenterX) * sinA + (y - waferCenterY) * cosA;
            }

            Kdtree::KdNodeVector nodes;
            for (int i = 0; i < 9; ++i) {
                std::vector<double> point(2);
                point[0] = inputpoints[i][0];
                point[1] = inputpoints[i][1];
                nodes.push_back(Kdtree::KdNode(point, 0, i));
            }

            Kdtree::KdTree tree(&nodes); // build tree
            std::vector<double> test_point(2);

            Kdtree::KdNodeVector result;

            //1 first nearest nearbord
            test_point[0] = -70.0;    test_point[1] = 82.0;
            tree.k_nearest_neighbors(test_point, 4, &result);
            Assert::AreEqual((int)result.size(), 4);
            Assert::AreEqual(result[0].index, 0);
            Assert::AreEqual(result[1].index, 3);
            Assert::AreEqual(result[2].index, 1);
            Assert::AreEqual(result[3].index, 4);


            // TOP LEFT -----------------------
            test_point[0] = -0.2;    test_point[1] = 0.2;
            tree.k_nearest_neighbors(test_point, 4, &result);
            Assert::AreEqual((int)result.size(), 4);
            Assert::AreEqual(result[0].index, 4);
            Assert::AreEqual(result[1].index, 3);
            Assert::AreEqual(result[2].index, 1); 
            Assert::AreEqual(result[3].index, 7); /// pas celui q'uon voudrait en fait 
            struct PredicateTL : Kdtree::KdNodePredicate {
                std::vector<double> point;
                double tolX;
                double tolY;
                PredicateTL(std::vector<double> p, double angle_rd, double step, double tolerance) {
                    this->point = p;
                    tolX = tolerance + step * sin(angle_rd);
                    tolY = tolerance ;
                }

                bool operator()(const Kdtree::KdNode& kn) const {
                    return ((this->point[0] + tolX) > kn.point[0]) && (this->point[1] - tolY) < kn.point[1];
                }
            };
            PredicateTL predTL(result[0].point, angleWaferRot_rd, step, margin);
            tree.k_nearest_neighbors(test_point, 4, &result, &predTL);
            Assert::AreEqual((int)result.size(), 4);
            Assert::AreEqual(result[0].index, 4);
            Assert::AreEqual(result[1].index, 3);
            Assert::AreEqual(result[2].index, 1);
            Assert::AreEqual(result[3].index, 0); /// OK

            // BOTTOM LEFT -----------------------
            test_point[0] = -0.2;    test_point[1] = -0.2;
            tree.k_nearest_neighbors(test_point, 4, &result);
            Assert::AreEqual((int)result.size(), 4);
            Assert::AreEqual(result[0].index, 4);
            Assert::AreEqual(result[1].index, 3);   
            Assert::AreEqual(result[2].index, 7);
            Assert::AreEqual(result[3].index, 1); /// pas celui q'uon voudrait en fait

            struct PredicateBL : Kdtree::KdNodePredicate {
                std::vector<double> point;
                double tolX;
                double tolY;
                PredicateBL(std::vector<double> p, double angle_rd, double step, double tolerance) {
                    this->point = p;
                    tolX = tolerance;
                    tolY = tolerance + step * sin(angle_rd);
                }

                bool operator()(const Kdtree::KdNode& kn) const {
                    return ((this->point[0] + tolX) > kn.point[0]) && (this->point[1] + tolY) > kn.point[1];
                }
            };
            PredicateBL predBL(result[0].point, angleWaferRot_rd, step, margin);
            tree.k_nearest_neighbors(test_point, 4, &result, &predBL);
            Assert::AreEqual((int)result.size(), 4);
            Assert::AreEqual(result[0].index, 4);
            Assert::AreEqual(result[1].index, 3);
            Assert::AreEqual(result[2].index, 7);
            Assert::AreEqual(result[3].index, 6);

            // TOP RIGHT -----------------------
            test_point[0] = 0.2;    test_point[1] = 0.2;
            tree.k_nearest_neighbors(test_point, 4, &result);
            Assert::AreEqual((int)result.size(), 4);
            Assert::AreEqual(result[0].index, 4);
            Assert::AreEqual(result[1].index, 1);  
            Assert::AreEqual(result[2].index, 3);/// pas celui q'uon voudrait en fait 
            Assert::AreEqual(result[3].index, 5); 

            struct PredicateTR : Kdtree::KdNodePredicate {
                std::vector<double> point;
                double tolX;
                double tolY;
                PredicateTR(std::vector<double> p, double angle_rd, double step, double tolerance) {
                    this->point = p;
                    tolX = tolerance;
                    tolY = tolerance + step * sin(angle_rd);
                }

                bool operator()(const Kdtree::KdNode& kn) const {
                    return ((this->point[0] - tolX) < kn.point[0]) && (this->point[1] - tolY) < kn.point[1];
                }
            };
            PredicateTR predTR(result[0].point, angleWaferRot_rd, step, margin);
            tree.k_nearest_neighbors(test_point, 4, &result, &predTR);
            Assert::AreEqual((int)result.size(), 4);
            Assert::AreEqual(result[0].index, 4);
            Assert::AreEqual(result[1].index, 1);
            Assert::AreEqual(result[2].index, 5);
            Assert::AreEqual(result[3].index, 2);

            // BOTTOM RIGHT -----------------------
            test_point[0] = 0.2;    test_point[1] = -0.2;
            tree.k_nearest_neighbors(test_point, 4, &result);
            Assert::AreEqual((int)result.size(), 4);
            Assert::AreEqual(result[0].index, 4);
            Assert::AreEqual(result[1].index, 7); 
            Assert::AreEqual(result[2].index, 5);
            Assert::AreEqual(result[3].index, 3); /// pas celui q'uon voudrait en fait 

            struct PredicateBR : Kdtree::KdNodePredicate {
                std::vector<double> point;
                double tolX;
                double tolY;
                PredicateBR(std::vector<double> p, double angle_rd, double step, double tolerance) {
                    this->point = p;
                    tolX = tolerance + step * sin(angle_rd);
                    tolY = tolerance;
                }

                bool operator()(const Kdtree::KdNode& kn) const {
                    return ((this->point[0] - tolX) < kn.point[0]) && (this->point[1] + tolY) > kn.point[1];
                }
            };
            PredicateBR predBR(result[0].point, angleWaferRot_rd, step, margin);
            Kdtree::KdNodePredicate* pred = (Kdtree::KdNodePredicate*)&predBR;
            tree.k_nearest_neighbors(test_point, 4, &result, pred);
            Assert::AreEqual((int)result.size(), 4);
            Assert::AreEqual(result[0].index, 4);
            Assert::AreEqual(result[1].index, 7);
            Assert::AreEqual(result[2].index, 5);
            Assert::AreEqual(result[3].index, 8);
        }

        TEST_METHOD(SimpleQuad_QuadNearest_AngleMinus)
        {
            double inputpoints[9][2] = {
            { -75.0, 75.0 },   { 0.0, 75.0 },   { 75.0, 75.0  },
            { -75.0, 0.0 },    { 0.0, 0.0 },    { 75.0, 0.0  },
            { -75.0, -75.0 },  { 0.0, -75.0 },  { 75.0, -75.0  } };


            double step = 1.1 * (inputpoints[1][0] - inputpoints[0][0]); //75;
            double margin = 0.05;
            double angleWaferRot_dg = -10.0;
            double angleWaferRot_rd = angleWaferRot_dg * M_PI / 180.0;
            double waferCenterX = 0.525;
            double waferCenterY = -1.24;

            double cosA = cos(angleWaferRot_rd);
            double sinA = sin(angleWaferRot_rd);
            for (int i = 0; i < 9; ++i) {
                double x = inputpoints[i][0]; double y = inputpoints[i][1];
                inputpoints[i][0] = waferCenterX + (x - waferCenterX) * cosA + (y - waferCenterY) * sinA;
                inputpoints[i][1] = waferCenterY - (x - waferCenterX) * sinA + (y - waferCenterY) * cosA;
            }

            Kdtree::KdNodeVector nodes;
            for (int i = 0; i < 9; ++i) {
                std::vector<double> point(2);
                point[0] = inputpoints[i][0];
                point[1] = inputpoints[i][1];
                nodes.push_back(Kdtree::KdNode(point, 0, i));
            }

            Kdtree::KdTree tree(&nodes); // build tree
            std::vector<double> test_point(2);

            Kdtree::KdNodeVector result;

            //1 first nearest nearbord
            test_point[0] = -70.0;    test_point[1] = 82.0;
            tree.k_nearest_neighbors(test_point, 4, &result);
            Assert::AreEqual((int)result.size(), 4);
            Assert::AreEqual(result[0].index, 0);
            Assert::AreEqual(result[1].index, 1);
            Assert::AreEqual(result[2].index, 3);
            Assert::AreEqual(result[3].index, 4);


            // TOP LEFT -----------------------
            test_point[0] = -0.2;    test_point[1] = 0.2;
            tree.k_nearest_neighbors(test_point, 4, &result);
            Assert::AreEqual((int)result.size(), 4);
            Assert::AreEqual(result[0].index, 4);
            Assert::AreEqual(result[1].index, 1);
            Assert::AreEqual(result[2].index, 5);/// pas celui q'uon voudrait en fait 
            Assert::AreEqual(result[3].index, 3); 
            struct PredicateTL : Kdtree::KdNodePredicate {
                std::vector<double> point;
                double tolX;
                double tolY;
                PredicateTL(std::vector<double> p, double angle_rd, double step, double tolerance) {
                    this->point = p;
                    tolX = tolerance;
                    tolY = tolerance + step * sin(angle_rd) * -1.0 ; 
                }

                bool operator()(const Kdtree::KdNode& kn) const {
                    return ((this->point[0] + tolX) > kn.point[0]) && (this->point[1] - tolY) < kn.point[1];
                }
            };
            PredicateTL predTL(result[0].point, angleWaferRot_rd, step, margin);
            tree.k_nearest_neighbors(test_point, 4, &result, &predTL);
            Assert::AreEqual((int)result.size(), 4);
            Assert::AreEqual(result[0].index, 4);
            Assert::AreEqual(result[1].index, 1);
            Assert::AreEqual(result[2].index, 3);
            Assert::AreEqual(result[3].index, 0); /// OK

            // BOTTOM LEFT -----------------------
            test_point[0] = -0.2;    test_point[1] = -0.2;
            tree.k_nearest_neighbors(test_point, 4, &result);
            Assert::AreEqual((int)result.size(), 4);
            Assert::AreEqual(result[0].index, 4);
            Assert::AreEqual(result[1].index, 7);
            Assert::AreEqual(result[2].index, 3);
            Assert::AreEqual(result[3].index, 5); /// pas celui q'uon voudrait en fait

            struct PredicateBL : Kdtree::KdNodePredicate {
                std::vector<double> point;
                double tolX;
                double tolY;
                PredicateBL(std::vector<double> p, double angle_rd, double step, double tolerance) {
                    this->point = p;
                    tolX = tolerance + step * sin(angle_rd) * -1.0;
                    tolY = tolerance ;
                }

                bool operator()(const Kdtree::KdNode& kn) const {
                    return ((this->point[0] + tolX) > kn.point[0]) && (this->point[1] + tolY) > kn.point[1];
                }
            };
            PredicateBL predBL(result[0].point, angleWaferRot_rd, step, margin);
            tree.k_nearest_neighbors(test_point, 4, &result, &predBL);
            Assert::AreEqual((int)result.size(), 4);
            Assert::AreEqual(result[0].index, 4);
            Assert::AreEqual(result[1].index, 7);
            Assert::AreEqual(result[2].index, 3);
            Assert::AreEqual(result[3].index, 6);

            // TOP RIGHT -----------------------
            test_point[0] = 0.2;    test_point[1] = 0.2;
            tree.k_nearest_neighbors(test_point, 4, &result);
            Assert::AreEqual((int)result.size(), 4);
            Assert::AreEqual(result[0].index, 4);
            Assert::AreEqual(result[1].index, 5);
            Assert::AreEqual(result[2].index, 1);
            Assert::AreEqual(result[3].index, 7);/// pas celui q'uon voudrait en fait 

            struct PredicateTR : Kdtree::KdNodePredicate {
                std::vector<double> point;
                double tolX;
                double tolY;
                PredicateTR(std::vector<double> p, double angle_rd, double step, double tolerance) {
                    this->point = p;
                    tolX = tolerance + step * sin(angle_rd) * -1.0;
                    tolY = tolerance;
                }

                bool operator()(const Kdtree::KdNode& kn) const {
                    return ((this->point[0] - tolX) < kn.point[0]) && (this->point[1] - tolY) < kn.point[1];
                }
            };
            PredicateTR predTR(result[0].point, angleWaferRot_rd, step, margin);
            tree.k_nearest_neighbors(test_point, 4, &result, &predTR);
            Assert::AreEqual((int)result.size(), 4);
            Assert::AreEqual(result[0].index, 4);
            Assert::AreEqual(result[1].index, 5);
            Assert::AreEqual(result[2].index, 1);
            Assert::AreEqual(result[3].index, 2);

            // BOTTOM RIGHT -----------------------
            test_point[0] = 0.2;    test_point[1] = -0.2;
            tree.k_nearest_neighbors(test_point, 4, &result);
            Assert::AreEqual((int)result.size(), 4);
            Assert::AreEqual(result[0].index, 4);
            Assert::AreEqual(result[1].index, 5);
            Assert::AreEqual(result[2].index, 7);
            Assert::AreEqual(result[3].index, 1); /// pas celui q'uon voudrait en fait 

            struct PredicateBR : Kdtree::KdNodePredicate {
                std::vector<double> point;
                double tolX;
                double tolY;
                PredicateBR(std::vector<double> p, double angle_rd, double step, double tolerance) {
                    this->point = p;
                    tolX = tolerance ;
                    tolY = tolerance + step * sin(angle_rd) * -1.0;
                }

                bool operator()(const Kdtree::KdNode& kn) const {
                    return ((this->point[0] - tolX) < kn.point[0]) && (this->point[1] + tolY) > kn.point[1];
                }
            };
            PredicateBR predBR(result[0].point, angleWaferRot_rd, step, margin);
            Kdtree::KdNodePredicate* pred = (Kdtree::KdNodePredicate*)&predBR;
            tree.k_nearest_neighbors(test_point, 4, &result, pred);
            Assert::AreEqual((int)result.size(), 4);
            Assert::AreEqual(result[0].index, 4);
            Assert::AreEqual(result[1].index, 5);
            Assert::AreEqual(result[2].index, 7);
            Assert::AreEqual(result[3].index, 8);
        }
    };
}