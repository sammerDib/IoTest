#include "CppUnitTest.h"

#include <opencv2/opencv.hpp>
#include <opencv2/core.hpp>
#include <filesystem>

#include "MetroBase.hpp"
#include "MetroSeeker.hpp"
#include "MetroCircleSeeker.hpp"
using namespace metroCD;
using namespace cv;
using namespace std;

#pragma unmanaged

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

#define TEST_DATA_PATH std::string(".\\..\\..\\Tests\\Data\\")

//#define USE_OUTPUT 1
#define TEST_OUTPUT_DATA_PATH  std::string("C:\\Work\\Data\\TestsMetrologyCD\\")

namespace SharedOpenCVNativeTests
{
    TEST_CLASS(MetrologyCDTest)
    {
    private:
        static constexpr double s_precision = 1e-1;

    public:
        void drawLinePointsOnImage(cv::Mat& img, std::vector<cv::Point2d> ptLine, cv::Scalar sccolor) {
            for (size_t i = 0; i < ptLine.size(); ++i)
            {
                cv::line(img, ptLine[i], ptLine[(i + 1) % ptLine.size()], sccolor, 1, cv::LINE_AA);
            }
            cv::line(img, ptLine[0], ptLine[2], cv::Scalar(0.0, 250.0, 0.0), 1, cv::LINE_AA);
            cv::line(img, ptLine[1], ptLine[3], cv::Scalar(0.0, 250.0, 0.0), 1, cv::LINE_AA);
        }

        void drawRawContoursOnImage(cv::Mat& img, std::vector<std::vector<cv::Point>> contours) {
            for (size_t i = 0; i < contours.size(); ++i) {
                cv::drawContours(img, contours[i], -1, cv::Scalar(0, 0, 255), 1, cv::LINE_AA);
            }
        }

        Mat MakeVerticalStepImage(cv::Size size, int stepStart_x, int stepBottom_y, int stepTop_y, bool BlackToWhite = true, int noiseRange = 0)
        {
            Mat stepImage(size, CV_8U);

            int firstval = BlackToWhite ? stepBottom_y : stepTop_y;
            int lastval = BlackToWhite ? stepTop_y : stepBottom_y;

            Mat stepLine(1, size.width, CV_8U);
            uchar* ptr = stepLine.ptr<uchar>(0);
            for (int i = 0; i < stepLine.cols; i++)
            {
                *ptr++ = (i <= stepStart_x) ? firstval : lastval;
            }
            copyMakeBorder(stepLine, stepImage, 0, size.height - 1, 0, 0, BORDER_REPLICATE);

            if (noiseRange != 0)
            {
                cv::theRNG().state = cv::getTickCount(); // allowing to change seed
                cv::Mat noise = Mat::zeros(size, CV_8U);
                cv::randu(noise, 0, noiseRange);
                stepImage = stepImage + noise;
            }
            return stepImage;
        }

        Mat MakeVerticalStepRampImage(cv::Size size, int stepStart_x, int stepEnd_x,  int stepBottom_y, int stepTop_y, bool BlackToWhite = true, int noiseRange = 0)
        {
            Mat stepRampImage(size, CV_8U);

            int firstval = BlackToWhite ? stepBottom_y : stepTop_y;
            int lastval = BlackToWhite ? stepTop_y : stepBottom_y;

            double slope = (double)(lastval - firstval) / (double)(stepEnd_x - stepStart_x);
            double offset = (double)firstval;

            Mat stepLine(1, size.width, CV_8U);
            uchar* ptr = stepLine.ptr<uchar>(0);
            for (int i = 0; i < stepLine.cols; i++)
            {
                if (i <= stepStart_x)
                {
                    *ptr++ = (uchar)firstval;
                }
                else if(i <= stepEnd_x)
                {
                    // we are in ramp
                    *ptr++ = (uchar)((double)(i - stepStart_x) * slope + offset);
                }
                else
                {
                    *ptr++ = (uchar)lastval;
                }
            }
            copyMakeBorder(stepLine, stepRampImage, 0, size.height - 1, 0, 0, BORDER_REPLICATE);

            if (noiseRange != 0)
            {
                cv::theRNG().state = cv::getTickCount(); // allowing to change seed
                cv::Mat noise = Mat::zeros(size, CV_8U);
                cv::randu(noise, 0, noiseRange);
                stepRampImage = stepRampImage + noise;
            }
            return stepRampImage;
        }

        Mat MakeVerticalStepSigmoidImage(cv::Size size, double sigSlope, int sigMiddle_x,  int stepBottom_y, int stepTop_y, bool BlackToWhite = true, int noiseRange = 0)
        {
            // Fit Bolzmann(sigmoidal)sigmoidal
            // y = bottom_y + [  (topY-Bottom_y) / ( 1 + exp( (ptInflexion - x) / slope ) )  ]

            Mat stepSigmoidImage(size, CV_8U);

            double slope = BlackToWhite ? sigSlope : -sigSlope;

            double bot = (double) stepBottom_y;
            double num = (double) stepTop_y - bot;
            double pivotX = (double) sigMiddle_x;

            Mat stepLine(1, size.width, CV_8U);
            uchar* ptr = stepLine.ptr<uchar>(0);
            for (int i = 0; i < stepLine.cols; i++)
            {
                double dSig = bot + (num / (1.0 + std::exp((pivotX - (double)i) / slope)));
                *ptr++ = (uchar)dSig;
            }
            copyMakeBorder(stepLine, stepSigmoidImage, 0, size.height - 1, 0, 0, BORDER_REPLICATE);

            if (noiseRange != 0)
            {
                cv::theRNG().state = cv::getTickCount(); // allowing to change seed
                cv::Mat noise = Mat::zeros(size, CV_8U);
                cv::randu(noise, 0, noiseRange);
                stepSigmoidImage = stepSigmoidImage + noise;
            }
            return stepSigmoidImage;
        }

        TEST_CLASS_INITIALIZE(MetroCDClassInitialize)
        {
#ifdef USE_OUTPUT
           /// create out put directories
            try
            {
                std::filesystem::create_directories(TEST_OUTPUT_DATA_PATH);
            }
            catch (const std::exception& ee)
            {
                auto msg = ee.what();
            }
#endif // USE_OUTPUT           
        }

        TEST_METHOD(_00_MetroSeeker_Status)
        {
            // create an image where a vertical step is at x =   
            int stepstart_x = 152;
            int colsSize = 565;
            int rowsSize = 430;

            Mat stepImage = MakeVerticalStepImage(Size(colsSize, rowsSize), stepstart_x, 0, 220);

            int PosY = 165;
            int OriginX = 54;
            int OriginY = PosY;
            int EndX = 396;
            int EndY = PosY;

            const int kernelSize = 3;
            SeekLocInEdge seekLoc = SeekLocInEdge::AtPeak;
            //1
            auto seekin = make_shared<SeekerInputs>(cv::Point2d(OriginX, PosY), cv::Point2d(EndX, OriginY), 125.0, SeekMode::BlackToWhite, kernelSize, seekLoc, SeekScale::NoScale);
            Assert::IsNotNull(seekin.get(), L"In Not Null 1");
            SeekerInputs* pIn = (SeekerInputs*)seekin.get();
            Assert::IsNotNull(pIn, L"SeekIn Not Null 1");
            Assert::AreEqual((double)OriginX, seekin->Origin.x, s_precision, L"In Origin X");
            Assert::AreEqual((double)OriginY, seekin->Origin.y, s_precision, L"In Origin Y");
            Assert::AreEqual((double)EndX, seekin->End.x, s_precision, L"In End X");
            Assert::AreEqual((double)EndY, seekin->End.y, s_precision, L"In End Y");
            Assert::AreEqual(125.0, seekin->Width, s_precision, L"In Width");
            Assert::AreEqual((int)SeekMode::BlackToWhite, (int)seekin->SeekMode, L"In SeekMode");
            Assert::AreEqual((int)pow(2, (int)SeekScale::NoScale), seekin->SeekScale, L"In SeekScale");

            Seeker skempty;
            Assert::AreEqual((int)skempty.GetType(), (int)MType::Seeker, L"MTyp failed 0");
            Assert::AreEqual((int)skempty.GetStatus(), (int)MStatus::NOT_INITIALIZE, L"Status failed 0");

            Seeker skjustin(seekin);
            Assert::AreEqual((int)skjustin.GetType(), (int)MType::Seeker, L"MTyp failed 1");
            Assert::AreEqual((int)skjustin.GetStatus(), (int)MStatus::INITIALIZED, L"Status failed 1");
            skjustin.SetInputImage(stepImage);
            Assert::AreEqual((int)skjustin.GetStatus(), (int)MStatus::IDLE, L"Status failed 1 - 2");

            Seeker sk(seekin, stepImage);

            Assert::AreEqual((int)sk.GetType(), (int)MType::Seeker, L"MTyp failed");
            Assert::AreEqual((int)sk.GetStatus(), (int)MStatus::IDLE, L"Status failed");

            auto seekout = std::static_pointer_cast<SeekerOutputs>(sk.Compute());
#ifdef USE_OUTPUT            
            cv::imwrite(TEST_OUTPUT_DATA_PATH + "StatusImage.png", stepImage);
            auto inputcolor = cv::imread(TEST_OUTPUT_DATA_PATH + "StatusImage.png", cv::IMREAD_COLOR);
            sk.DrawOverlay(inputcolor, AllMetroDrawFlags);
            cv::imwrite(TEST_OUTPUT_DATA_PATH + "StatusImage.png", inputcolor);
            seekout->ReportFile(TEST_OUTPUT_DATA_PATH + "StatusImage.csv", 0);
#endif  
            Assert::IsNotNull(seekout.get(), L"Out Not Null 1");
            Assert::AreEqual(1, (int)seekout->Results.size(), L"Out edge number not found 1");
            Assert::AreEqual((double)stepstart_x+0.5, seekout->Results[0].x, s_precision, L"Out edge position x 1 ");
            Assert::AreEqual((double)PosY, seekout->Results[0].y, s_precision, L"Out edge position y 1 ");

        }

        TEST_METHOD(_00_MetroSeeker_ShouldReturnEmptyorFailResult)
        {
            // create an image where a vertical step is at x =   
            int stepstart_x = 152;
            int colsSize = 565;
            int rowsSize = 430;

            Mat stepImage = MakeVerticalStepImage(Size(colsSize, rowsSize), stepstart_x, 0, 220, true, 0);
#ifdef USE_OUTPUT
            cv::imwrite(TEST_OUTPUT_DATA_PATH + "StepImage.png", stepImage);
            auto inputcolor = cv::imread(TEST_OUTPUT_DATA_PATH + "StepImage.png", cv::IMREAD_COLOR);
#endif  
            // seeker is outside step nothing should be detected
            int PosY = 300;
            int OriginX = 3;
            int OriginY = PosY;
            int EndX = 33;
            int EndY = PosY;

            auto seekin = make_shared<SeekerInputs>(cv::Point2d(OriginX, PosY), cv::Point2d(EndX, OriginY), 20.0, SeekMode::BlackToWhite);
            Assert::IsNotNull(seekin.get(), L"In Not Null 1");
            SeekerInputs* pIn = (SeekerInputs*)seekin.get();
            Assert::IsNotNull(pIn, L"SeekIn Not Null 1");

            Seeker sk(seekin, stepImage);
            auto seekout = std::static_pointer_cast<SeekerOutputs>(sk.Compute());
#ifdef USE_OUTPUT
            sk.DrawOverlay(inputcolor, AllMetroDrawFlags);
            cv::imwrite(TEST_OUTPUT_DATA_PATH + "StepFailImageResult.png", inputcolor);
            seekout->ReportFile(TEST_OUTPUT_DATA_PATH + "StepFailImageResult.csv", 0);
#endif

            Assert::IsNotNull(seekout.get(), L"Out Not Null 1");
            Assert::IsFalse(seekout->IsSuccess(), L"seeker should be fail");
            Assert::AreEqual(0, (int)seekout->Results.size(), L"Out edge number not found ");
        }

        TEST_METHOD(_00_MetroSeeker_ShouldReturnEmptyorFailResult_withBackgroundNoise)
        {
            // create an image where a vertical step is at x =   
            int stepstart_x = 152;
            int colsSize = 565;
            int rowsSize = 430;

            Mat stepImage = MakeVerticalStepImage(Size(colsSize, rowsSize), stepstart_x, 0, 220, true, 0);
#ifdef USE_OUTPUT
            cv::imwrite(TEST_OUTPUT_DATA_PATH + "StepImage.png", stepImage);
            auto inputcolor = cv::imread(TEST_OUTPUT_DATA_PATH + "StepImage.png", cv::IMREAD_COLOR);
#endif  
            // seeker is outside step nothing should be detected
            int PosY = 300;
            int OriginX = 3;
            int OriginY = PosY;
            int EndX = 33;
            int EndY = PosY;

            auto seekin = make_shared<SeekerInputs>(cv::Point2d(OriginX, PosY), cv::Point2d(EndX, OriginY), 20.0, SeekMode::BlackToWhite);
            Assert::IsNotNull(seekin.get(), L"In Not Null 1");
            SeekerInputs* pIn = (SeekerInputs*)seekin.get();
            Assert::IsNotNull(pIn, L"SeekIn Not Null 1");

            Seeker sk(seekin, stepImage);
            auto seekout = std::static_pointer_cast<SeekerOutputs>(sk.Compute());
#ifdef USE_OUTPUT
            sk.DrawOverlay(inputcolor, AllMetroDrawFlags);
            cv::imwrite(TEST_OUTPUT_DATA_PATH + "StepFailImageResulNoise.png", inputcolor);
            seekout->ReportFile(TEST_OUTPUT_DATA_PATH + "StepFailImageResultNoise.csv", 0);
#endif

            Assert::IsNotNull(seekout.get(), L"Out Not Null 1");
            Assert::IsFalse(seekout->IsSuccess(), L"seeker should be fail");
            Assert::AreEqual(0, (int)seekout->Results.size(), L"Out edge number not found ");
        }
        
        TEST_METHOD(_01_MetroSeeker_SimpleHiContrastSingleStep_BToW_MultiScale)
        {
            // create an image where a vertical step is at x = 256  
            int stepstart_x = 256;
            int colsSize = 565;
            int rowsSize = 430;

            double expectedMiddleResult_x = (double)stepstart_x + 0.5;

            Mat stepImage = MakeVerticalStepImage(Size(colsSize, rowsSize), stepstart_x, 0, 220, true, 20);

#ifdef USE_OUTPUT
            cv::imwrite(TEST_OUTPUT_DATA_PATH + "StepImage.png", stepImage);
            auto inputcolor = cv::imread(TEST_OUTPUT_DATA_PATH + "StepImage.png", cv::IMREAD_COLOR);
#endif  
            int PosY = 56;
            int OriginX = 63;
            int OriginY = PosY;
            int EndX = 466;
            int EndY = PosY;

            const int kernelSize = 3;
            SeekLocInEdge seekLoc = SeekLocInEdge::AtPeak;
            //1
            auto seekin = make_shared<SeekerInputs>(cv::Point2d(OriginX, PosY), cv::Point2d(EndX, OriginY), 60.0, SeekMode::BlackToWhite, kernelSize, seekLoc, SeekScale::NoScale);
            Assert::IsNotNull(seekin.get(), L"In Not Null 1");
            SeekerInputs* pIn = (SeekerInputs*)seekin.get();
            Assert::IsNotNull(pIn, L"SeekIn Not Null 1");
            Assert::AreEqual((double)OriginX, seekin->Origin.x, s_precision, L"In Origin X");
            Assert::AreEqual((double)OriginY, seekin->Origin.y, s_precision, L"In Origin Y");
            Assert::AreEqual((double)EndX, seekin->End.x, s_precision, L"In End X");
            Assert::AreEqual((double)EndY, seekin->End.y, s_precision, L"In End Y");
            Assert::AreEqual(60.0, seekin->Width, s_precision, L"In Width");
            Assert::AreEqual((int)SeekMode::BlackToWhite,(int)seekin->SeekMode, s_precision, L"In SeekMode");
            Assert::AreEqual((int)pow(2, (int)SeekScale::NoScale), seekin->SeekScale, s_precision, L"In SeekScale");

            Seeker sk(seekin, stepImage);
            auto seekout = std::static_pointer_cast<SeekerOutputs>(sk.Compute());

#ifdef USE_OUTPUT
            sk.DrawOverlay(inputcolor, AllMetroDrawFlags);
            cv::imwrite(TEST_OUTPUT_DATA_PATH + "StepImageResult.png", inputcolor);
#endif
            Assert::IsNotNull(seekout.get(), L"Out Not Null 1");                  
            Assert::AreEqual(1, (int)seekout->Results.size(), L"Out edge number not found 1");
            Assert::AreEqual((double)stepstart_x+0.5, seekout->Results[0].x, s_precision, L"Out edge position x 1 ");
            Assert::AreEqual((double)PosY, seekout->Results[0].y, s_precision, L"Out edge position y 1 ");

      
            //2 - scale2
            seekin = make_shared<SeekerInputs>(cv::Point2d(OriginX, PosY), cv::Point2d(EndX, OriginY), 60.0, SeekMode::BlackToWhite, kernelSize, seekLoc, SeekScale::Scale2);
            Assert::AreEqual((int)pow(2, (int)SeekScale::Scale2), seekin->SeekScale, L"In SeekScale 2");
            seekout = std::static_pointer_cast<SeekerOutputs>(sk.Compute());
            Assert::AreEqual(1, (int)seekout->Results.size(), L"Out edge number not found 2");
            Assert::AreEqual((double)expectedMiddleResult_x, seekout->Results[0].x, s_precision, L"Out edge position x 2 ");
            Assert::AreEqual((double)PosY, seekout->Results[0].y, s_precision, L"Out edge position y 2 ");

            //3 - scale4
            seekin = make_shared<SeekerInputs>(cv::Point2d(OriginX, PosY), cv::Point2d(EndX, OriginY), 60.0, SeekMode::BlackToWhite, kernelSize, seekLoc, SeekScale::Scale4);
            Assert::AreEqual((int)pow(2, (int)SeekScale::Scale4), seekin->SeekScale, L"In SeekScale 4");
            seekout = std::static_pointer_cast<SeekerOutputs>(sk.Compute());
            Assert::AreEqual(1, (int)seekout->Results.size(), L"Out edge number not found 3");
            Assert::AreEqual((double)expectedMiddleResult_x, seekout->Results[0].x, s_precision, L"Out edge position x 3");
            Assert::AreEqual((double)PosY, seekout->Results[0].y, s_precision, L"Out edge position y 3");

            //4 - scale8
            seekin = make_shared<SeekerInputs>(cv::Point2d(OriginX, PosY), cv::Point2d(EndX, OriginY), 60.0, SeekMode::BlackToWhite, kernelSize, seekLoc, SeekScale::Scale8);
            Assert::AreEqual((int)pow(2, (int)SeekScale::Scale8), seekin->SeekScale, L"In SeekScale 8");
            seekout = std::static_pointer_cast<SeekerOutputs>(sk.Compute());
            Assert::AreEqual(1, (int)seekout->Results.size(), L"Out edge number not found 4");
            Assert::AreEqual((double)expectedMiddleResult_x, seekout->Results[0].x, s_precision, L"Out edge position x 4");
            Assert::AreEqual((double)PosY, seekout->Results[0].y, s_precision, L"Out edge position y 4");
        }

        TEST_METHOD(_02_MetroSeeker_SimpleHiContrastSingleRamp_BToW)
        {
            // create an image where a vertical step ramp start at x = 246 and end at 265  
            int rampstart_x = 246;
            int rampend_x = 265;
            int stepTop_y = 220;
            int stepBottom_y = 2;
            Assert::IsTrue(rampstart_x < rampend_x);

            double expectedMiddleResult_x = (double)(rampstart_x + rampend_x) * 0.5;

            int colsSize = 565;
            int rowsSize = 430;

            Mat stepImage = MakeVerticalStepRampImage(Size(colsSize, rowsSize), rampstart_x, rampend_x, stepBottom_y, stepTop_y, true, 12);
#ifdef USE_OUTPUT
            cv::imwrite(TEST_OUTPUT_DATA_PATH + "RampImage.png", stepImage);
            auto inputcolor = cv::imread(TEST_OUTPUT_DATA_PATH + "RampImage.png", cv::IMREAD_COLOR);
#endif  
           
            int PosY = 56;
            int OriginX = 63;
            int OriginY = PosY;
            int EndX = 466;
            int EndY = PosY;

            const int kernelSize = 3;
            SeekLocInEdge seekLoc = SeekLocInEdge::AtPeak;
            //1
            auto seekin = make_shared<SeekerInputs>(cv::Point2d(OriginX, PosY), cv::Point2d(EndX, OriginY), 60.0, SeekMode::BlackToWhite, kernelSize, seekLoc, SeekScale::NoScale);
            Assert::IsNotNull(seekin.get(), L"In Not Null 1");
            SeekerInputs* pIn = (SeekerInputs*)seekin.get();
            Assert::IsNotNull(pIn, L"SeekIn Not Null 1");
            Assert::AreEqual((double)OriginX, seekin->Origin.x, s_precision, L"In Origin X");
            Assert::AreEqual((double)OriginY, seekin->Origin.y, s_precision, L"In Origin Y");
            Assert::AreEqual((double)EndX, seekin->End.x, s_precision, L"In End X");
            Assert::AreEqual((double)EndY, seekin->End.y, s_precision, L"In End Y");
            Assert::AreEqual(60.0, seekin->Width, s_precision, L"In Width");
            Assert::AreEqual((int)SeekMode::BlackToWhite, (int)seekin->SeekMode, s_precision, L"In SeekMode");
            Assert::AreEqual((int)pow(2, (int)SeekScale::NoScale), seekin->SeekScale, s_precision, L"In SeekScale");

            Seeker sk(seekin, stepImage);
            auto seekout = std::static_pointer_cast<SeekerOutputs>(sk.Compute());
#ifdef USE_OUTPUT
            sk.DrawOverlay(inputcolor, AllMetroDrawFlags);
            cv::imwrite(TEST_OUTPUT_DATA_PATH + "RampImageResult.png", inputcolor);
#endif  

            Assert::IsNotNull(seekout.get(), L"Out Not Null 1");
            Assert::AreEqual(1, (int)seekout->Results.size(), L"Out edge number not found 1");
            Assert::AreEqual((double)expectedMiddleResult_x, seekout->Results[0].x, s_precision, L"Out edge position x 1 ");
            Assert::AreEqual((double)PosY, seekout->Results[0].y, s_precision, L"Out edge position y 1 ");

          
/* // RTi : need further work to mak scale functionnal everywhare
            //2 - scale2
            seekin = make_shared<SeekerInputs>(cv::Point2d(OriginX, PosY), cv::Point2d(EndX, OriginY), 60.0, SeekMode::BlackToWhite, 1, SeekScale::Scale2);
            Assert::AreEqual((int)pow(2, (int)SeekScale::Scale2), seekin->SeekScale, L"In SeekScale 2");
            seekout = std::static_pointer_cast<SeekerOutputs>(sk.Compute());
            Assert::AreEqual(1, (int)seekout->Results.size(), L"Out edge number not found 2");
            Assert::AreEqual((double)expectedMiddleResult_x, seekout->Results[0].x, s_precision, L"Out edge position x 2 ");
            Assert::AreEqual((double)PosY, seekout->Results[0].y, s_precision, L"Out edge position y 2 ");

            //3 - scale4
            seekin = make_shared<SeekerInputs>(cv::Point2d(OriginX, PosY), cv::Point2d(EndX, OriginY), 60.0, SeekMode::BlackToWhite, 1, SeekScale::Scale4);
            Assert::AreEqual((int)pow(2, (int)SeekScale::Scale4), seekin->SeekScale, L"In SeekScale 4");
            seekout = std::static_pointer_cast<SeekerOutputs>(sk.Compute());
            Assert::AreEqual(1, (int)seekout->Results.size(), L"Out edge number not found 3");
            Assert::AreEqual((double)expectedMiddleResult_x, seekout->Results[0].x, s_precision, L"Out edge position x 3");
            Assert::AreEqual((double)PosY, seekout->Results[0].y, s_precision, L"Out edge position y 3");

            //4 - scale8
            seekin = make_shared<SeekerInputs>(cv::Point2d(OriginX, PosY), cv::Point2d(EndX, OriginY), 60.0, SeekMode::BlackToWhite, 1, SeekScale::Scale8);
            Assert::AreEqual((int)pow(2, (int)SeekScale::Scale8), seekin->SeekScale, L"In SeekScale 8");
            seekout = std::static_pointer_cast<SeekerOutputs>(sk.Compute());
            Assert::AreEqual(1, (int)seekout->Results.size(), L"Out edge number not found 4");
            Assert::AreEqual((double)expectedMiddleResult_x, seekout->Results[0].x, s_precision, L"Out edge position x 4");
            Assert::AreEqual((double)PosY, seekout->Results[0].y, s_precision, L"Out edge position y 4");

#ifdef USE_OUTPUT
            sk.DrawOverlay(inputcolor, AllMetroDrawFlags);
            cv::imwrite(TEST_OUTPUT_DATA_PATH + "RampImageResultScale8.png", inputcolor);
#endif
       */
        }

        TEST_METHOD(_02_MetroSeeker_SimpleHiContrastSingleRamp_BToW_PrefBottom)
        {
            // create an image where a vertical step ramp start at x = 246 and end at 265  
            int rampstart_x = 246;
            int rampend_x = 265;
            int stepTop_y = 220;
            int stepBottom_y = 2;
            Assert::IsTrue(rampstart_x < rampend_x);

            double expectedMiddleResult_x = rampstart_x;

            int colsSize = 565;
            int rowsSize = 430;

            Mat stepImage = MakeVerticalStepRampImage(Size(colsSize, rowsSize), rampstart_x, rampend_x, stepBottom_y, stepTop_y, true, 12);
#ifdef USE_OUTPUT
            cv::imwrite(TEST_OUTPUT_DATA_PATH + "RampImageBot.png", stepImage);
            auto inputcolor = cv::imread(TEST_OUTPUT_DATA_PATH + "RampImageBot.png", cv::IMREAD_COLOR);
#endif  
            int PosY = 56;
            int OriginX = 63;
            int OriginY = PosY;
            int EndX = 466;
            int EndY = PosY;

            const int kernelSize = 3;
            SeekLocInEdge seekLoc = SeekLocInEdge::MinusSigma; // preference Bottom edge

            //1
            auto seekin = make_shared<SeekerInputs>(cv::Point2d(OriginX, PosY), cv::Point2d(EndX, OriginY), 60.0, SeekMode::BlackToWhite, kernelSize, seekLoc, SeekScale::NoScale);
            Assert::IsNotNull(seekin.get(), L"In Not Null 1");
            SeekerInputs* pIn = (SeekerInputs*)seekin.get();
            Assert::IsNotNull(pIn, L"SeekIn Not Null 1");
            Assert::AreEqual((double)OriginX, seekin->Origin.x, s_precision, L"In Origin X");
            Assert::AreEqual((double)OriginY, seekin->Origin.y, s_precision, L"In Origin Y");
            Assert::AreEqual((double)EndX, seekin->End.x, s_precision, L"In End X");
            Assert::AreEqual((double)EndY, seekin->End.y, s_precision, L"In End Y");
            Assert::AreEqual(60.0, seekin->Width, s_precision, L"In Width");
            Assert::AreEqual((int)SeekMode::BlackToWhite, (int)seekin->SeekMode, s_precision, L"In SeekMode");
            Assert::AreEqual((int)pow(2, (int)SeekScale::NoScale), seekin->SeekScale, s_precision, L"In SeekScale");

            Seeker sk(seekin, stepImage);
            auto seekout = std::static_pointer_cast<SeekerOutputs>(sk.Compute());

#ifdef USE_OUTPUT
            sk.DrawOverlay(inputcolor, AllMetroDrawFlags);
            cv::imwrite(TEST_OUTPUT_DATA_PATH + "RampImageBotResult.png", inputcolor);
#endif  

            Assert::IsNotNull(seekout.get(), L"Out Not Null 1");
            Assert::AreEqual(1, (int)seekout->Results.size(), L"Out edge number not found 1");
            Assert::AreEqual((double)expectedMiddleResult_x, seekout->Results[0].x, 30.0 * s_precision, L"Out edge position x 1 ");
            Assert::AreEqual((double)PosY, seekout->Results[0].y, s_precision, L"Out edge position y 1 ");
        }

        TEST_METHOD(_02_MetroSeeker_SimpleHiContrastSingleRamp_BToW_PrefTop)
        {
            // create an image where a vertical step ramp start at x = 246 and end at 265  
            int rampstart_x = 246;
            int rampend_x = 265;
            int stepTop_y = 220;
            int stepBottom_y = 2;
            Assert::IsTrue(rampstart_x < rampend_x);

            double expectedResult_x = rampend_x;

            int colsSize = 565;
            int rowsSize = 430;

            Mat stepImage = MakeVerticalStepRampImage(Size(colsSize, rowsSize), rampstart_x, rampend_x, stepBottom_y, stepTop_y, true, 22);
#ifdef USE_OUTPUT
            cv::imwrite(TEST_OUTPUT_DATA_PATH + "RampImageTop.png", stepImage);
            auto inputcolor = cv::imread(TEST_OUTPUT_DATA_PATH + "RampImageTop.png", cv::IMREAD_COLOR);
#endif  
            int PosY = 56;
            int OriginX = 63;
            int OriginY = PosY;
            int EndX = 466;
            int EndY = PosY;

            const int kernelSize = 3;
            SeekLocInEdge seekLoc = SeekLocInEdge::PlusSigma; // preference Top edge

            //1
            auto seekin = make_shared<SeekerInputs>(cv::Point2d(OriginX, PosY), cv::Point2d(EndX, OriginY), 60.0, SeekMode::BlackToWhite, kernelSize, seekLoc, SeekScale::NoScale);
            Assert::IsNotNull(seekin.get(), L"In Not Null 1");
            SeekerInputs* pIn = (SeekerInputs*)seekin.get();
            Assert::IsNotNull(pIn, L"SeekIn Not Null 1");
            Assert::AreEqual((double)OriginX, seekin->Origin.x, s_precision, L"In Origin X");
            Assert::AreEqual((double)OriginY, seekin->Origin.y, s_precision, L"In Origin Y");
            Assert::AreEqual((double)EndX, seekin->End.x, s_precision, L"In End X");
            Assert::AreEqual((double)EndY, seekin->End.y, s_precision, L"In End Y");
            Assert::AreEqual(60.0, seekin->Width, s_precision, L"In Width");
            Assert::AreEqual((int)SeekMode::BlackToWhite, (int)seekin->SeekMode, s_precision, L"In SeekMode");
            Assert::AreEqual((int)pow(2, (int)SeekScale::NoScale), seekin->SeekScale, s_precision, L"In SeekScale");

            Seeker sk(seekin, stepImage);
            auto seekout = std::static_pointer_cast<SeekerOutputs>(sk.Compute());

#ifdef USE_OUTPUT
            sk.DrawOverlay(inputcolor, AllMetroDrawFlags);
            cv::imwrite(TEST_OUTPUT_DATA_PATH + "RampImageTopResult.png", inputcolor);
#endif  

            Assert::IsNotNull(seekout.get(), L"Out Not Null 1");
            Assert::AreEqual(1, (int)seekout->Results.size(), L"Out edge number not found 1");
            Assert::AreEqual((double)expectedResult_x, seekout->Results[0].x, 30.0 * s_precision, L"Out edge position x 1 ");
            Assert::AreEqual((double)PosY, seekout->Results[0].y, s_precision, L"Out edge position y 1 ");
        }

        TEST_METHOD(_03_MetroSeeker_SingleSigmoidStep_BToW)
        {
            // create an image where a vertical step ramp start at x = 246 and end at 265  
            int sigmoidMiddle_x = 256;
            double sigSlope = 20.1;
            int stepTop_y = 220;
            int stepBottom_y = 2;

            double expectedMiddleResult_x = (double)sigmoidMiddle_x;

            int colsSize = 565;
            int rowsSize = 430;

            Mat stepImage = MakeVerticalStepSigmoidImage(Size(colsSize, rowsSize), sigSlope, sigmoidMiddle_x, stepBottom_y, stepTop_y, true, 15);
#ifdef USE_OUTPUT
            cv::imwrite(TEST_OUTPUT_DATA_PATH + "SigmoidImage.png", stepImage);
            auto inputcolor = cv::imread(TEST_OUTPUT_DATA_PATH + "SigmoidImage.png", cv::IMREAD_COLOR);
#endif  
            int PosY = 56;
            int OriginX = 63;
            int OriginY = PosY;
            int EndX = 466;
            int EndY = PosY;

            const int kernelSize = 3;
            SeekLocInEdge seekLoc = SeekLocInEdge::AtPeak;

            //1
            auto seekin = make_shared<SeekerInputs>(cv::Point2d(OriginX, PosY), cv::Point2d(EndX, OriginY), 60.0, SeekMode::BlackToWhite, kernelSize, seekLoc, SeekScale::NoScale);
            Assert::IsNotNull(seekin.get(), L"In Not Null 1");
            SeekerInputs* pIn = (SeekerInputs*)seekin.get();
            Assert::IsNotNull(pIn, L"SeekIn Not Null 1");
            Assert::AreEqual((double)OriginX, seekin->Origin.x, s_precision, L"In Origin X");
            Assert::AreEqual((double)OriginY, seekin->Origin.y, s_precision, L"In Origin Y");
            Assert::AreEqual((double)EndX, seekin->End.x, s_precision, L"In End X");
            Assert::AreEqual((double)EndY, seekin->End.y, s_precision, L"In End Y");
            Assert::AreEqual(60.0, seekin->Width, s_precision, L"In Width");
            Assert::AreEqual((int)SeekMode::BlackToWhite, (int)seekin->SeekMode, s_precision, L"In SeekMode");
            Assert::AreEqual((int)pow(2, (int)SeekScale::NoScale), seekin->SeekScale, s_precision, L"In SeekScale");

            Seeker sk(seekin, stepImage);
            auto seekout = std::static_pointer_cast<SeekerOutputs>(sk.Compute());
#ifdef USE_OUTPUT
            sk.DrawOverlay(inputcolor, AllMetroDrawFlags);
            cv::imwrite(TEST_OUTPUT_DATA_PATH + "SigmoidImageResult.png", inputcolor);
#endif  
            Assert::IsNotNull(seekout.get(), L"Out Not Null 1");
            Assert::AreEqual(1, (int)seekout->Results.size(), L"Out edge number not found 1");
            Assert::AreEqual((double)expectedMiddleResult_x, seekout->Results[0].x, 8.5*s_precision, L"Out edge position x 1 ");
            Assert::AreEqual((double)PosY, seekout->Results[0].y, s_precision, L"Out edge position y 1 ");
        }
        TEST_METHOD(_04_MetroSeeker_RandomSingleStep_BtoW)
        {
            std::srand((unsigned int)std::time(nullptr));

            int colsSize = 1100;
            int rowsSize = 800;
            // create an image where a random vertical step in middle zone  
            //val = rand() % (high - low + 1) + low
            int stepstart_x = rand() % (colsSize / 2 + 1) + colsSize / 4;

            double expectedMiddleResult_x = (double)stepstart_x + 0.5;

            Mat stepImage = MakeVerticalStepImage(Size(colsSize, rowsSize), stepstart_x, 4, 211, true, 4);

#ifdef USE_OUTPUT
            cv::imwrite(TEST_OUTPUT_DATA_PATH + "StepImageRandom.png", stepImage);
            auto inputcolor = cv::imread(TEST_OUTPUT_DATA_PATH + "StepImageRandom.png", cv::IMREAD_COLOR);
#endif  

            int PosY = rand() % (rowsSize / 2 + 1) + rowsSize / 4 ;
            int OriginX = std::min(rand() % (colsSize / 4 + 1), stepstart_x - 2);
            int OriginY = PosY;
            int EndX = std::max(rand() % (colsSize / 4) + (3 * colsSize / 4), stepstart_x + 2);
            int EndY = PosY;
            double witdh = std::min(rowsSize / 4, rand() % 190 + 20);

            const int kernelSize = 3;
            SeekLocInEdge seekLoc = SeekLocInEdge::AtPeak;

            //1
            auto seekin = make_shared<SeekerInputs>(cv::Point2d(OriginX, OriginY), cv::Point2d(EndX, EndY), witdh, SeekMode::BlackToWhite, kernelSize, seekLoc, SeekScale::NoScale);
            Assert::IsNotNull(seekin.get(), L"In Not Null 1");
            SeekerInputs* pIn = (SeekerInputs*)seekin.get();
            Assert::IsNotNull(pIn, L"SeekIn Not Null 1");
            Assert::AreEqual((double)OriginX, seekin->Origin.x, s_precision, L"In Origin X");
            Assert::AreEqual((double)OriginY, seekin->Origin.y, s_precision, L"In Origin Y");
            Assert::AreEqual((double)EndX, seekin->End.x, s_precision, L"In End X");
            Assert::AreEqual((double)EndY, seekin->End.y, s_precision, L"In End Y");
            Assert::AreEqual(witdh, seekin->Width, s_precision, L"In Width");
            Assert::AreEqual((int)SeekMode::BlackToWhite, (int)seekin->SeekMode, L"In SeekMode");
            Assert::AreEqual((int)pow(2, (int)SeekScale::NoScale), seekin->SeekScale, L"In SeekScale");

            Seeker sk(seekin, stepImage);
            auto seekout = std::static_pointer_cast<SeekerOutputs>(sk.Compute());
            Assert::IsNotNull(seekout.get(), L"Out Not Null 1");

#ifdef USE_OUTPUT
            sk.DrawOverlay(inputcolor, AllMetroDrawFlags);
            cv::imwrite(TEST_OUTPUT_DATA_PATH + "StepImageRandomResult.png", inputcolor);
#endif  
            Assert::AreEqual(1, (int)seekout->Results.size(), L"Out edge number not found 1");
            Assert::AreEqual((double)expectedMiddleResult_x, seekout->Results[0].x, s_precision, L"Out edge position x 1 ");
            Assert::AreEqual((double)PosY, seekout->Results[0].y, s_precision, L"Out edge position y 1 ");
        }
        TEST_METHOD(_05_MetroSeeker_RandomSingleStep_WtoB)
        {
            std::srand((unsigned int)std::time(nullptr));

            int colsSize = 1100;
            int rowsSize = 800;
            // create an image where a random vertical step in middle zone  
            //val = rand() % (high - low + 1) + low
            int stepstart_x = rand() % (colsSize / 2 + 1) + colsSize / 4;

            double expectedMiddleResult_x = (double)stepstart_x + 0.5;

            Mat stepImage = MakeVerticalStepImage(Size(colsSize, rowsSize), stepstart_x, 12, 215, false, 0);

#ifdef USE_OUTPUT
            cv::imwrite(TEST_OUTPUT_DATA_PATH + "StepImageRandomWtoB.png", stepImage);
            auto inputcolor = cv::imread(TEST_OUTPUT_DATA_PATH + "StepImageRandomWtoB.png", cv::IMREAD_COLOR);
#endif
            int PosY = rand() % (rowsSize / 2 + 1) + rowsSize / 4;
            int OriginX = std::min(rand() % (colsSize / 4 + 1), stepstart_x - 2);
            int OriginY = PosY;
            int EndX = std::max(rand() % (colsSize / 4 ) + (3 * colsSize / 4), stepstart_x + 2);;
            int EndY = PosY;
            double witdh = std::min(rowsSize / 4, rand() % 190 + 20);

            const int kernelSize = 3;
            SeekLocInEdge seekLoc = SeekLocInEdge::AtPeak;
            //1
            SeekScale skscale = SeekScale::NoScale;
            auto seekin = make_shared<SeekerInputs>(cv::Point2d(OriginX, OriginY), cv::Point2d(EndX, EndY), witdh, SeekMode::WhiteToBlack, kernelSize, seekLoc, skscale);
            Assert::IsNotNull(seekin.get(), L"In Not Null 1");
            SeekerInputs* pIn = (SeekerInputs*)seekin.get();
            Assert::IsNotNull(pIn, L"SeekIn Not Null 1");
            Assert::AreEqual((double)OriginX, seekin->Origin.x, s_precision, L"In Origin X");
            Assert::AreEqual((double)OriginY, seekin->Origin.y, s_precision, L"In Origin Y");
            Assert::AreEqual((double)EndX, seekin->End.x, s_precision, L"In End X");
            Assert::AreEqual((double)EndY, seekin->End.y, s_precision, L"In End Y");
            Assert::AreEqual(witdh, seekin->Width, s_precision, L"In Width");
            Assert::AreEqual((int)SeekMode::WhiteToBlack, (int)seekin->SeekMode, L"In SeekMode");
            Assert::AreEqual((int)pow(2, (int)skscale), seekin->SeekScale, L"In SeekScale");

            Seeker sk(seekin, stepImage);
            auto seekout = std::static_pointer_cast<SeekerOutputs>(sk.Compute());
            Assert::IsNotNull(seekout.get(), L"Out Not Null 1");
#ifdef USE_OUTPUT
            sk.DrawOverlay(inputcolor, AllMetroDrawFlags);
            cv::imwrite(TEST_OUTPUT_DATA_PATH + "StepImageRandomWtoB.png", inputcolor);
            seekout->ReportFile(TEST_OUTPUT_DATA_PATH + "StepImageRandomWtoB.csv", 0);
#endif

            //Assert::AreEqual(1, (int)seekout->Results.size(), L"Out edge number not found 1");
            Assert::AreEqual((double)expectedMiddleResult_x, seekout->Results[0].x, s_precision, L"Out edge position x 1 ");
            Assert::AreEqual((double)PosY, seekout->Results[0].y, s_precision, L"Out edge position y 1 ");
        }

        TEST_METHOD(_06_MetroSeeker_RandomSingleStep_Rotation)
        {
            std::srand((unsigned int)std::time(nullptr));

            int colsSize = 1100;
            int rowsSize = 800;
            // create an image where a random vertical step in middle zone  
            //val = rand() % (high - low + 1) + low
            int stepstart_x = rand() % (colsSize / 2 + 1) + colsSize / 4;
            double expectedMiddleResult_x = (double)stepstart_x + 0.5;

            Mat stepImage = MakeVerticalStepImage(Size(colsSize, rowsSize), stepstart_x, 4, 211, true, 0);
#ifdef USE_OUTPUT
           cv::imwrite(TEST_OUTPUT_DATA_PATH + "StepImageRandomRot.png", stepImage);
           auto inputcolor = cv::imread(TEST_OUTPUT_DATA_PATH + "StepImageRandomRot.png", cv::IMREAD_COLOR);
#endif

            int PosY = rand() % (rowsSize / 2 + 1) + rowsSize / 4;
            int OriginX = std::min(rand() % (colsSize / 4 + 1), stepstart_x - 2);
            int OriginY = PosY + (rand() % 20 - 10);
            int EndX = std::max(rand() % (colsSize / 4) + (3 * colsSize / 4), stepstart_x + 2);
            int EndY = PosY + (rand() % 20 - 10);
            double witdh = std::min(rowsSize / 4, rand() % 190 + 20);

            const int kernelSize = 3;
            SeekLocInEdge seekLoc = SeekLocInEdge::AtPeak;
            //1
            auto seekin = make_shared<SeekerInputs>(cv::Point2d(OriginX, OriginY), cv::Point2d(EndX, EndY), witdh, SeekMode::BlackToWhite, kernelSize, seekLoc, SeekScale::NoScale);
            Assert::IsNotNull(seekin.get(), L"In Not Null 1");
            SeekerInputs* pIn = (SeekerInputs*)seekin.get();
            Assert::IsNotNull(pIn, L"SeekIn Not Null 1");
            Assert::AreEqual((double)OriginX, seekin->Origin.x, s_precision, L"In Origin X");
            Assert::AreEqual((double)OriginY, seekin->Origin.y, s_precision, L"In Origin Y");
            Assert::AreEqual((double)EndX, seekin->End.x, s_precision, L"In End X");
            Assert::AreEqual((double)EndY, seekin->End.y, s_precision, L"In End Y");
            Assert::AreEqual(witdh, seekin->Width, s_precision, L"In Width");
            Assert::AreEqual((int)SeekMode::BlackToWhite, (int)seekin->SeekMode, L"In SeekMode");
            Assert::AreEqual((int)pow(2, (int)SeekScale::NoScale), seekin->SeekScale, L"In SeekScale");

            Seeker sk(seekin, stepImage);
            auto seekout = std::static_pointer_cast<SeekerOutputs>(sk.Compute());
            Assert::IsNotNull(seekout.get(), L"Out Not Null 1");

#ifdef USE_OUTPUT
           sk.DrawOverlay(inputcolor, AllMetroDrawFlags);
           cv::imwrite(TEST_OUTPUT_DATA_PATH + "StepImageRandomRot.png", inputcolor);
#endif
            Assert::AreEqual(1, (int)seekout->Results.size(), L"Out edge number not found 1");
            Assert::AreEqual((double)expectedMiddleResult_x, seekout->Results[0].x,  s_precision, L"Out edge position x 1 ");
        }

        TEST_METHOD(_00_MetroSeeker_FromTSVImage)
        {
            Logger::WriteMessage("Seeker from image \n");
            std::string image_path = TEST_DATA_PATH + std::string("4_circles_of_30_pixels_in_diameter.png");

            cv::Mat input = cv::imread(image_path, cv::IMREAD_GRAYSCALE);
            Assert::IsFalse(input.empty());


            cv::Mat inputcolor = cv::imread(image_path, cv::IMREAD_COLOR);
            Assert::IsFalse(inputcolor.empty());
            Assert::AreEqual(3, inputcolor.channels(), L"color image channles");

            const int kernelSize = 3;
            SeekLocInEdge seekLoc = SeekLocInEdge::AtPeak;

            auto seekin = make_shared<SeekerInputs>(cv::Point2d(644, 516), cv::Point2d(666, 502), 10.0, SeekMode::BlackToWhite, kernelSize, seekLoc, SeekScale::NoScale);
            Seeker sk(seekin, input);

            sk.Compute();

#ifdef USE_OUTPUT
            inputcolor = cv::imread(image_path, cv::IMREAD_COLOR);
            sk.DrawOverlay(inputcolor, AllMetroDrawFlags);
            cv::imwrite(TEST_OUTPUT_DATA_PATH + "SeekerTSVResult.png", inputcolor);
#endif
        }

        TEST_METHOD(_00_MetroSeeker_FromWaferEdge)
        {
            Logger::WriteMessage("Seeker from image \n");
            std::string image_path = TEST_DATA_PATH + std::string("EdgeDetection_3_2X_VIS_X_139077_Y_56190.png");

            cv::Mat input = cv::imread(image_path, cv::IMREAD_GRAYSCALE);
            Assert::IsFalse(input.empty());


            cv::Mat inputcolor= cv::imread(image_path, cv::IMREAD_COLOR);
            Assert::IsFalse(inputcolor.empty());
            Assert::AreEqual(3, inputcolor.channels(), L"color image channels");
  
            const int kernelSize = 3;
            SeekLocInEdge seekLoc = SeekLocInEdge::AtPeak;
            auto seekin = make_shared<SeekerInputs>(cv::Point2d(344, 732), cv::Point2d(904, 513), 160.0, SeekMode::BlackToWhite, kernelSize, seekLoc, SeekScale::NoScale);
            //zauto seekin = make_shared<SeekerInputs>(cv::Point2d(904, 513), cv::Point2d(344, 732) , 160.0, SeekMode::BlackToWhite,  kernelSize, seekLoc, SeekScale::NoScale);
            //auto seekin = make_shared<SeekerInputs>(cv::Point2d(904, 513), cv::Point2d(344, 732), 160.0, SeekMode::WhiteToBlack,  kernelSize, seekLoc, SeekScale::NoScale);

            //auto seekin = make_shared<SeekerInputs>(cv::Point2d(850, 273), cv::Point2d(320, 438), 60.0, SeekMode::BlackToWhite,  kernelSize, seekLoc, SeekScale::NoScale);
            //seekin->SigAnalysis_Threshold = 20.0;
            //seekin->SigAnalysis_Sample_pct = 0.25;
            Seeker sk(seekin, input);

#ifdef USE_OUTPUT
            sk.DrawOverlay(inputcolor, AllMetroDrawFlags);
            cv::imwrite(TEST_OUTPUT_DATA_PATH + "SeekerOverlay.png", inputcolor);
#endif

            auto seekout = std::static_pointer_cast<SeekerOutputs>(sk.Compute());
            Assert::IsNotNull(seekout.get(), L"Out Not Null 1");

#ifdef USE_OUTPUT
            inputcolor = cv::imread(image_path, cv::IMREAD_COLOR);
            sk.DrawOverlay(inputcolor, AllMetroDrawFlags);
            cv::imwrite(TEST_OUTPUT_DATA_PATH + "SeekerOverlayResult.png", inputcolor);
#endif

            double expectedResult_x = (double)484.913;
            double expectedResult_y = (double)676.893;

            // 1 good - 2 skipped
            Assert::AreEqual(3, (int)seekout->Results.size(), L"Out edge number not found 1");

            Assert::AreEqual((double)expectedResult_x, seekout->Results[0].x, s_precision, L"Out edge position x 1 ");
            Assert::AreEqual((double)expectedResult_y, seekout->Results[0].y, s_precision, L"Out edge position y 1 ");
        }


        TEST_METHOD(_00_MetroCircleSeeker_FromTSVImage)
        {
            Logger::WriteMessage("Circle Seeker from image TSV 30pixels \n");
            std::string image_path = TEST_DATA_PATH + std::string("4_circles_of_30_pixels_in_diameter.png");

            cv::Mat input = cv::imread(image_path, cv::IMREAD_GRAYSCALE);
            Assert::IsFalse(input.empty());

            cv::Mat inputcolor = cv::imread(image_path, cv::IMREAD_COLOR);
            Assert::IsFalse(inputcolor.empty());
            Assert::AreEqual(3, inputcolor.channels(), L"color image channles");

            double expectedRadious_px = 13.64;

            cv::Point2d center = cv::Point2d(644, 516);
            double radius_px = 15.0;
            double radius_Tolerance_px = 8.0;
            int seekernumber = 6; //auto == 0 
            double seekerwidth = 5.0; //auto == 0
            auto mode = CircleSeekMode::BlackToWhite;
            const int kernelSize = 3;
            SeekLocInEdge seekLoc = SeekLocInEdge::AtPeak;

            auto seekin = make_shared<CircleSeekerInputs>(center, radius_px, radius_Tolerance_px, seekernumber, seekerwidth, mode, kernelSize, seekLoc);
            CircleSeeker sk(seekin, input);

            auto seekout = std::static_pointer_cast<CircleSeekerOutputs>(sk.Compute());

#ifdef USE_OUTPUT
            inputcolor = cv::imread(image_path, cv::IMREAD_COLOR);
            //auto drawFlags = AllMetroDrawFlags;
            auto drawFlags = (uint)MetroDrawFlag::DrawFit | (uint)MetroDrawFlag::DrawDetection | (uint)MetroDrawFlag::DrawSkipDetection;
            sk.DrawOverlay(inputcolor, drawFlags);
            cv::imwrite(TEST_OUTPUT_DATA_PATH + "SeekerCircleTSVResult.png", inputcolor);
#endif
            Assert::AreEqual(expectedRadious_px, seekout->FoundRadius, s_precision, L"Out radius ");

        }
    };
}