#include "CameraCalibration.hpp"
#include "CImageTypeConvertor.hpp"
#include "CImageOperators.hpp"
#include <opencv2/calib3d.hpp>
#include "ReportingUtils.hpp"
#include "AngularReflectionsCalibration.hpp"
#include "C2DSignalAnalysis.hpp"
#include <iostream>

using namespace signal_2D;
using namespace std;
using namespace cv;

class ParallelFresnelCoefComputation : public ParallelLoopBody
{
private:
    Mat& CMNormalized;
    Mat& Mask;
    Mat& IncidentAnglesInRadian;
    Mat& FresnelWafer;

    Mat Normal;
    Mat VerticalWaferJones;
    map<int, std::complex<float>> IndexNByWavelengthTable;
    vector<int> Wavelengths;

public:
    ParallelFresnelCoefComputation(Mat& CMNormalized, Mat& mask, Mat& incidentAnglesInRadian, Mat& rScreenToWafer, Mat& tScreenToWafer, Mat& fresnelWafer, map<int, std::complex<float>> indexNByWavelengthTable, vector<int> wavelengthsList, bool useVerticalScreenPolarization = true)
        : CMNormalized(CMNormalized), Mask(mask), IncidentAnglesInRadian(incidentAnglesInRadian), FresnelWafer(fresnelWafer), Wavelengths(wavelengthsList), IndexNByWavelengthTable(indexNByWavelengthTable)
    {
        // normal to wafer, considered flat
        Normal = Mat::zeros(1, 3, CV_32F);
        Normal.at<float>(0, 0) = 0;
        Normal.at<float>(0, 1) = 0;
        Normal.at<float>(0, 2) = 1;

        // create the initial polarization vector  (vertical or horizontal or a composition of both)
        Mat verticalScreenJones = Mat::zeros(1, 3, CV_32F);

        if (useVerticalScreenPolarization)
        {
            verticalScreenJones.at<float>(0, 0) = 0;
            verticalScreenJones.at<float>(0, 1) = 1;
            verticalScreenJones.at<float>(0, 2) = 0;
        }
        else
        {
            verticalScreenJones.at<float>(0, 0) = 1;
            verticalScreenJones.at<float>(0, 1) = 0;
            verticalScreenJones.at<float>(0, 2) = 0;
        }

        // transform vector screen polarization in the screen coordinate referential to the wafer coordinate referential
        VerticalWaferJones = verticalScreenJones * rScreenToWafer;
    }

    virtual void operator()(const Range& range) const CV_OVERRIDE
    {
        for (int r = range.start; r < range.end; r++)
        {
            int i = r / CMNormalized.cols;
            int j = (r % CMNormalized.cols);

            if (Mask.ptr<uchar>(i)[j] != 0)
            {
                // 1. Defines a Jones set of vectors for the incident ray.

                // for each pixel, search incident ray
                Mat incidentRay = Mat::zeros(1, 3, CV_32F);
                incidentRay.at<float>(0, 0) = -CMNormalized.ptr<Vec3f>(i)[j][0];
                incidentRay.at<float>(0, 1) = -CMNormalized.ptr<Vec3f>(i)[j][1];
                incidentRay.at<float>(0, 2) = CMNormalized.ptr<Vec3f>(i)[j][2];

                // Each ray starting from the screen with its own angle has its own Jones vector.
                // The Jones basis has X normal to the incident plane and Y in the incident plane (to facilitate the Fresnel calculation).
                cv::Mat incidentJonesX = Normal.cross(incidentRay);
                float norm = sqrt(incidentJonesX.at<float>(0, 0) * incidentJonesX.at<float>(0, 0) + incidentJonesX.at<float>(0, 1) * incidentJonesX.at<float>(0, 1) + incidentJonesX.at<float>(0, 2) * incidentJonesX.at<float>(0, 2));
                incidentJonesX.at<float>(0, 0) /= norm;
                incidentJonesX.at<float>(0, 1) /= norm;
                incidentJonesX.at<float>(0, 2) /= norm;

                cv::Mat incidentJonesY = -incidentJonesX.cross(incidentRay);
                float norm2 = sqrt(incidentJonesY.at<float>(0, 0) * incidentJonesY.at<float>(0, 0) + incidentJonesY.at<float>(0, 1) * incidentJonesY.at<float>(0, 1) + incidentJonesY.at<float>(0, 2) * incidentJonesY.at<float>(0, 2));
                incidentJonesY.at<float>(0, 0) /= norm2;
                incidentJonesY.at<float>(0, 1) /= norm2;
                incidentJonesY.at<float>(0, 2) /= norm2;

                // 2. Project the polarization vector in the Jones basis of the current ray

                // This is the Jones vector for the ray.It will be used to compute the ratio of energy in both Fresnel coefficients.
                float rayJonesX = static_cast<float> ( VerticalWaferJones.dot(incidentJonesX) );
                float rayJonesY = static_cast<float> ( VerticalWaferJones.dot(incidentJonesY) );
                // the Z component has no physical meaning, thus the Jones X and Y components are normalized :
                float normRayJones = sqrt(rayJonesX * rayJonesX + rayJonesY * rayJonesY);
                rayJonesX /= normRayJones;
                rayJonesY /= normRayJones;

                rayJonesX = pow(rayJonesX, 2.0f);
                rayJonesY = pow(rayJonesY, 2.0f);

                // 3. At the wafer surface, extract the Fresnel coefficient Fp and Fs for the current incident angle, using the complex refractive indices.

                float fresnelS = 0;
                float fresnelP = 0;
                for (int wavelength : Wavelengths)
                {
                    std::complex<float> indexN = IndexNByWavelengthTable.at(wavelength);
                    float angle = IncidentAnglesInRadian.ptr<float>(i)[j];

                    std::complex<float> kz1 = abs(cos(angle));
                    std::complex<float> kz2 = sqrt((indexN * indexN) - std::complex<float>( (float)(pow(sin(angle), 2))));

                    // Compute Fresnel for polarization P (TM)
                    std::complex<float> R1 = ((indexN * indexN * kz1) - kz2) / ((indexN * indexN * kz1) + kz2);
                    R1 = R1 * std::conj(R1);
                    float fresnelCoefP = R1.real();

                    // Compute Fresnel for polarization S (TE)
                    std::complex<float> R2 = (kz1 - kz2) / (kz1 + kz2);
                    R2 = R2 * std::conj(R2);
                    float fresnelCoefS = R2.real();

                    fresnelP += fresnelCoefP;
                    fresnelS += fresnelCoefS;
                }

                // Compute the ratio of energy reflected from the surface and going towards the camera due to the wafer material,
                // made ease thanks to the convention wa have chosen : R = JonesX^2 * Fs + JonesX^2 * Fp
                float fresnelWaferValue = rayJonesX * fresnelS + rayJonesY * fresnelP;
                fresnelWaferValue /= Wavelengths.size();
                FresnelWafer.ptr<float>(i)[j] = fresnelWaferValue;
            }
        }
    }
};

class ParallelCorrectionComputation : public ParallelLoopBody
{
private:
    Mat& CameraImg;
    Mat& FresnelCoef;
    Mat& Mask;
    Mat& ScreenContribution;
    Mat& Correction;

public:
    ParallelCorrectionComputation(Mat& cameraImg, Mat& fresnelCoef, Mat& mask, Mat& screenContribution, Mat& correction)
        : CameraImg(cameraImg), FresnelCoef(fresnelCoef), Mask(mask), ScreenContribution(screenContribution), Correction(correction)
    {
    }

    virtual void operator()(const Range& range) const CV_OVERRIDE
    {
        for (int r = range.start; r < range.end; r++)
        {
            int i = r / CameraImg.cols;
            int j = (r % CameraImg.cols);

            if (Mask.ptr<uchar>(i)[j] != 0)
            {
                float imgValue = CameraImg.ptr<float>(i)[j];
                float fresnelValue = FresnelCoef.ptr<float>(i)[j];
                ScreenContribution.ptr<float>(i)[j] = imgValue / fresnelValue;
                Correction.ptr<float>(i)[j] = 1 / ScreenContribution.ptr<float>(i)[j];
            }
        }
    }
};

class ParallelIncidentAnglesComputation : public ParallelLoopBody
{
private:
    Mat& CMNormalized;
    Mat& IncidentAnglesInRadian;
    Mat& Mask;
    Mat m;

public:
    ParallelIncidentAnglesComputation(Mat& CMNormalized, Mat& incidentAnglesInRadian, Mat& mask)
        : CMNormalized(CMNormalized), IncidentAnglesInRadian(incidentAnglesInRadian), Mask(mask)
    {
        m = Mat::zeros(1, 3, CV_32F);
        m.at<float>(0, 0) = 0;
        m.at<float>(0, 1) = 0;
        m.at<float>(0, 2) = -1;
    }

    virtual void operator()(const Range& range) const CV_OVERRIDE
    {
        for (int r = range.start; r < range.end; r++)
        {
            int i = r / CMNormalized.cols;
            int j = (r % CMNormalized.cols);

            if (Mask.ptr<uchar>(i)[j] != 0)
            {
                Mat CM = Mat::zeros(1, 3, CV_32F);
                float x = CMNormalized.ptr<Vec3f>(i)[j][0];
                float y = CMNormalized.ptr<Vec3f>(i)[j][1];
                float z = CMNormalized.ptr<Vec3f>(i)[j][2];
                CM.at<float>(0, 0) = x;
                CM.at<float>(0, 1) = y;
                CM.at<float>(0, 2) = z;
                float angle = static_cast<float>( CM.dot(m) ); // (m.at<float>(0, 0) * CMNormalized.at<Vec3f>(r, c)[0]) + (m.at<float>(0, 1) * CMNormalized.at<Vec3f>(r, c)[1]) + (m.at<float>(0, 2) * CMNormalized.at<Vec3f>(r, c)[2]);
                float angleRadian = acos(angle);
                IncidentAnglesInRadian.ptr<float>(i)[j] = angleRadian;
            }
        }
    }
};

namespace psd {
    namespace {
        /**  Returns interpolated value at x from parallel arrays ( xData, yData )
        *  Assumes that xData has at least two elements, is sorted and is strictly monotonic increasing
        * boolean argument extrapolate determines behaviour beyond ends of array (if needed)
        */
        std::complex<double> interpolate(vector<int>& xData, vector<std::complex<double>>& yData, int x);

        /**  Polynomial fit of an image using a grid / mesh / window bit by bit
        *  each grid of the image is either fitted using a 2nd order polynomial (for the wafer borders) 
        *  or a 4th degree polynomial (for the inside of the wafer)
        * @param image : image to use for the fit
        * @param mask : wafer mask (to not include the exterior of the wafer in the fit)
        * @param meshSize : size of each mesh of the polynomial fit
        * @param patternThreshold : threshold value to ignore the patterns on the wafer
        */
        cv::Mat MeshedPolynomialFit(cv::Mat image, cv::Mat mask, int meshSize, double patternThreshold = 0.80);
    }

    cv::Mat ApplyUniformityCorrection(cv::Mat& brightfield, cv::Mat& correctionImage, cv::Mat& mask, int targetSaturationLevel, float acceptablePercentageOfSaturatedPixels)
    {
        cv::Mat brightField32Bit;
        brightfield.convertTo(brightField32Bit, CV_32FC1);

        if (correctionImage.type() != CV_32FC1)
        {
            correctionImage.convertTo(correctionImage, CV_32FC1);
        }

        double correctionMax;
        cv::minMaxLoc(correctionImage, NULL, &correctionMax, NULL, NULL, mask);
        
        correctionImage = correctionImage / correctionMax;

        cv::Mat correctedImage32bit = brightField32Bit.mul(correctionImage);
        double imageSaturation = img_operators::ComputeGreyLevelSaturation(correctedImage32bit, mask, acceptablePercentageOfSaturatedPixels);
        double coefficient = targetSaturationLevel / imageSaturation;
        
        cv::Mat correctedImage;
        correctedImage32bit.convertTo(correctedImage, CV_8UC1, coefficient, 0);

        return correctedImage;
    }

    cv::Mat AngularReflectionsCalibration(cv::Mat& cameraImg, cv::Mat& mask, cv::Mat& CMNormalized, std::map<int, std::complex<double>>& refractiveIndexByWavelengthTable, std::vector<int>& wavelengthsList, ExtrinsicScreenParameters extrinsicScreenParam, double patternThresholdForPolynomialFit, bool useVerticalScreenPolarization, std::string directoryPathToStoreReport) {

        //setting every 0 value to 1 (to prevent dividing by zero in the later steps)

        cv::Mat zeroPixelsMask;
        cv::inRange(cameraImg, cv::Scalar(0), cv::Scalar(0), zeroPixelsMask);
        cameraImg.setTo(cv::Scalar(1), zeroPixelsMask);

        Mat cameraImg32;
        cameraImg.convertTo(cameraImg32, CV_32FC1);
        double cameraImgMin, cameraImgMax;
        minMaxLoc(cameraImg32, &cameraImgMin, &cameraImgMax);
        cameraImg32 /= cameraImgMax;


        // interpolate refractive index
        map<int, std::complex<float>> indexNByWavelengthTable;
        for (int wavelength : wavelengthsList)
        {
            std::vector<int> wavelengthTable;
            std::vector<std::complex<double>> refractiveIndex;
            for (std::map<int, std::complex<double>>::iterator it = refractiveIndexByWavelengthTable.begin(); it != refractiveIndexByWavelengthTable.end(); ++it) {
                wavelengthTable.push_back(it->first);
                refractiveIndex.push_back(it->second);
            }
            std::complex<double> index = interpolate(wavelengthTable, refractiveIndex, wavelength);
            indexNByWavelengthTable.insert(std::map<int, std::complex<double>>::value_type(wavelength, index));
        }

        // incident angle in radian
        cv::Mat incidentAnglesInRadian = Mat::zeros(CMNormalized.size(), CV_32F);
        ParallelIncidentAnglesComputation obj(CMNormalized, incidentAnglesInRadian, mask);
        parallel_for_(Range(0, CMNormalized.size().height * CMNormalized.size().width), obj);

        cv::Mat fresnelWafer = Mat::zeros(CMNormalized.size(), CV_32F);
        Mat RScreenToWafer32;
        extrinsicScreenParam.RScreenToWafer.convertTo(RScreenToWafer32, CV_32FC1);
        Mat TScreenToWafer32;
        extrinsicScreenParam.TScreenToWafer.convertTo(TScreenToWafer32, CV_32FC1);
        ParallelFresnelCoefComputation obj1(CMNormalized, mask, incidentAnglesInRadian, RScreenToWafer32, TScreenToWafer32, fresnelWafer, indexNByWavelengthTable, wavelengthsList, useVerticalScreenPolarization);
        parallel_for_(Range(0, CMNormalized.size().height * CMNormalized.size().width), obj1);

        cv::Mat screenContribution = Mat::zeros(cameraImg.size(), CV_32F);
        cv::Mat correction = Mat::zeros(cameraImg.size(), CV_32F);
        ParallelCorrectionComputation obj2(cameraImg32, fresnelWafer, mask, screenContribution, correction);
        parallel_for_(Range(0, cameraImg.size().height * cameraImg.size().width), obj2);


        // Fit the correction image to get a better result
        int meshSize = correction.rows / 25;
        cv::Mat polynomialCorrection = MeshedPolynomialFit(correction, mask, meshSize, patternThresholdForPolynomialFit);

        if (directoryPathToStoreReport != "")
        {
            cv::imwrite(directoryPathToStoreReport + "\\Fresnel.tif", fresnelWafer);
            cv::imwrite(directoryPathToStoreReport + "\\ScreenContribution.tif", screenContribution);
            cv::imwrite(directoryPathToStoreReport + "\\Correction.tif", correction);
            cv::imwrite(directoryPathToStoreReport + "\\PolynomialCorrection.tif", polynomialCorrection);
        }

        return polynomialCorrection;
    }

    namespace {
        std::complex<double> interpolate(vector<int>& xData, vector<std::complex<double>>& yData, int x)
        {
            int size = (int) xData.size(); // cast in int warning : conversion from 'size_t' to 'int', possible loss of data

            // find left end of interval for interpolation (special case: beyond right end)
            int i = 0;
            if (x >= xData[size - 2])
            {
                i = size - 2;
            }
            else
            {
                while (x > xData[i + 1]) i++;
            }

            // points on either side (unless beyond ends)
            std::complex<double> xL = xData[i], yL = yData[i], xR = xData[i + 1], yR = yData[i + 1];

            // gradient
            std::complex<double> dydx = (yR - yL) / (xR - xL);

            // linear interpolation
            return yL + dydx * ((double)x - xL);
        }

        cv::Mat CreatePatternMask(cv::Mat image, cv::Mat waferMask, double patternThreshold = 0.80)
        {
            //Masking out the patterns with a basic threshold (we don't want them messing up the polynomial fit)
            cv::Mat patternMask;
            cv::threshold(image, patternMask, patternThreshold, 1.0, cv::THRESH_BINARY);
            patternMask.convertTo(patternMask, CV_8UC1, 255);
            cv::dilate(patternMask, patternMask, cv::Mat());

            cv::Mat invertedPatternMask;
            cv::bitwise_not(patternMask, invertedPatternMask);
            invertedPatternMask /= 255;

            //We remove the outside of the wafer from the previous mask
            cv::Mat maskRoiNoPatterns = invertedPatternMask.mul(waferMask);
            return maskRoiNoPatterns;
        }

        cv::Mat MeshedPolynomialFit(cv::Mat image, cv::Mat mask, int meshSize, double patternThreshold)
        {
            Mat surface = Mat::zeros(image.rows, image.cols, image.type());

            for (int y = 0; y < image.rows - meshSize - 1; y += meshSize)
            {
                for (int x = 0; x < image.cols - meshSize - 1; x += meshSize)
                {
                    cv::Rect roi = cv::Rect(x, y, meshSize, meshSize);
                    cv::Mat waferMaskRoi = mask(roi);
                    cv::Mat invertedMaskRoi;
                    cv::bitwise_not(waferMaskRoi, invertedMaskRoi);
                    cv::Mat imageRoi = image(roi);  

                    cv::Mat maskRoiNoPatterns = CreatePatternMask(imageRoi, waferMaskRoi, patternThreshold);
                    //Getting the mean value of the inside of the wafer without the patterns
                    cv::Scalar roiMean = cv::mean(imageRoi, maskRoiNoPatterns);

                    //Center of the image ~= center of the wafer
                    int halfWidth = image.cols / 2;
                    int halfHeight = image.rows / 2;

                    //Getting a point that is closer to the center (by one mesh on the x and y axis)
                    //to check if we're close to the wafer border
                    int xdiffToCenter = halfWidth - x;
                    int ydiffToCenter = halfHeight - y;
                    cv::Rect insideRoi = cv::Rect((int)std::copysign(1, xdiffToCenter) * meshSize + x, (int)std::copysign(1, ydiffToCenter) * meshSize + y, meshSize, meshSize);

                    cv::Mat borderMask;
                    cv::inRange(mask(roi), cv::Scalar(254.0), cv::Scalar(254.0), borderMask);
                    cv::Mat insideBorderMask;
                    cv::inRange(mask(insideRoi), cv::Scalar(254.0), cv::Scalar(254.0), insideBorderMask);

                    //Edge case where we can't calculate a mean and we're on the wafer border (or close to it)
                    if (roiMean[0] == 0.0 && (cv::countNonZero(borderMask) != 0 || cv::countNonZero(insideBorderMask) != 0))
                    {
                        cv::Mat insideImageRoi = image(insideRoi);
                        cv::Mat insideMaskRoi = mask(insideRoi);
                        cv::Mat insideMaskRoiNoPatterns = CreatePatternMask(insideImageRoi, insideMaskRoi, patternThreshold);
                        roiMean = cv::mean(insideImageRoi, insideMaskRoiNoPatterns);
                    }

                    //Setting the outside of the wafer with the previous mean value (This is to help for the polynomial fit)
                    imageRoi.setTo(roiMean[0], invertedMaskRoi);
                    
                    //Mask for both the inside and outside of the wafer but without patterns
                    cv::Mat maskRoiNoPatternsFull;
                    cv::bitwise_or(maskRoiNoPatterns, invertedMaskRoi, maskRoiNoPatternsFull);

                    cv::Mat surfaceRoi;

                    //When inside the wafer (not on the border) : Fit a 4th order polynomial
                    if (cv::countNonZero(borderMask) == 0)
                    {
                        surfaceRoi = signal_2D::SolveQuarticEquation(imageRoi, maskRoiNoPatternsFull);
                    }
                    //When on the wafer's border : Fit a 2nd order polynomial
                    else
                    {
                        surfaceRoi = signal_2D::SolveQuadraticEquation(imageRoi, maskRoiNoPatternsFull);
                    }
                    surfaceRoi.copyTo(surface(roi));
                }
            }
            //Smoothing out any remaining apparent seams between the meshes
            int kernelSize = meshSize / 4;
            cv::Mat kernel = cv::Mat::ones(cv::Size(kernelSize, kernelSize), surface.type());
            kernel /= kernelSize * kernelSize;
            cv::Mat blurredSurface;
            cv::filter2D(surface, blurredSurface, surface.depth(), kernel);

            return blurredSurface;

        }
    }
}