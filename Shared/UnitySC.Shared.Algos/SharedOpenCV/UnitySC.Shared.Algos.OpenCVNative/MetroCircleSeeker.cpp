
#include <math.h>   
#include <filesystem>
#include "MetroCircleSeeker.hpp"
#include <HyperAccurateCircleFitter.hpp>

#pragma unmanaged

using namespace metroCD;

metroCD::CircleSeeker::CircleSeeker()
    :MetroBase(MType::Circle)
{

}

metroCD::CircleSeeker::CircleSeeker(std::shared_ptr<CircleSeekerInputs> inputsParams)
    :MetroBase(MType::Circle, inputsParams, cv::Mat())
{
    InitSeeker();
}

metroCD::CircleSeeker::CircleSeeker(std::shared_ptr<CircleSeekerInputs> inputsParams, cv::Mat inputImage)
    :MetroBase(MType::Circle, inputsParams, inputImage)
{
    InitSeeker();
}

void metroCD::CircleSeeker::InitSeeker()
{
    auto circleSeekin = *((CircleSeekerInputs*)_inputsPrm.get()); 

    double safeMargin_px = 2.0;
    double radius = circleSeekin.Radius;
    double radius_searchmin = std::max(0.0,  circleSeekin.Radius - circleSeekin.RadiusToleranceSearch - safeMargin_px);
    double radius_searchmax = circleSeekin.Radius + circleSeekin.RadiusToleranceSearch + safeMargin_px;

    int nbSeeker = circleSeekin.SeekerNumber;
    if (nbSeeker <= 0)
    {
        // calculate auto nb seeker depanding of perimeter to search
        double perimeter = 2.0 * CV_PI * radius;
        const double minimalArcLength = 6.0;
        nbSeeker = std::min(128, std::max( 4, (int) std::ceil(perimeter / minimalArcLength))); //[4 .. 128]
    }

    double angleStep_dg = 360.0 / nbSeeker;
    double angleStep_rd = angleStep_dg * CV_PI / 180.0;

    double width = circleSeekin.SeekerWidth;
    if (width == 0.0)
    {
        //calculate auto width of seeker regarding coverage and circular segmant
        //https://fr.wikipedia.org/wiki/Segment_circulaire; 

        double teta_rd = angleStep_rd;

        // hauteur du segment circulaire
        double h = radius * (1.0 - std::cos(0.5 * teta_rd));
        const double hMax = 0.6;
        if (h > hMax)
        {
            // compute a teta under H max 
            teta_rd = 2.0 * std::acos(1.0 - ((hMax - 0.05) / radius));
        }

        // c = longueur de la corde
        double c = 2.0 * radius * std::sin(0.5 * teta_rd);
        width = std::max(3.0, std::ceil(c));
    }

    // create seeker ring around center
    double startAngle_dg = 0.0;
    double startAngle_rd = startAngle_dg * CV_PI / 180.0;
    for (int i = 0; i < nbSeeker; i++)
    {
        double current_angl_rd = startAngle_rd + (double)i * angleStep_rd;
        current_angl_rd *= -1.0; // we inverse the angle since Y positive in image are to the bottom in order to have a trigo angle information for teh seeker oriented angle (anti clockwise for degrees)
        cv::Point2d origin(
            radius_searchmin * std::cos(current_angl_rd) + circleSeekin.Center.x,
            radius_searchmin * std::sin(current_angl_rd) + circleSeekin.Center.y);
        cv::Point2d end(
            radius_searchmax * std::cos(current_angl_rd) + circleSeekin.Center.x,
            radius_searchmax * std::sin(current_angl_rd) + circleSeekin.Center.y);

        auto seekorientin = std::make_shared<SeekerInputs>(origin, end, width, (SeekMode)circleSeekin.SeekMode, circleSeekin.KernelSize, circleSeekin.EdgeLocalizePreference, circleSeekin.SeekScale);
        // mainly for unexpected debug purpose
        seekorientin->SetSignalAnalysisAdvancedParam(circleSeekin.SigAnalysisThreshold, circleSeekin.SigAnalysisPeakWindowSize);
  
        CircleSeeker::SeekerOriented seekorient;
        seekorient.angle = startAngle_dg + (double)i * angleStep_dg;
        seekorient.seeker = Seeker(seekorientin, _inputImage);
        _seekersRing.push_back(seekorient);
    }
}

void metroCD::CircleSeeker::DrawOverlay(cv::Mat& colorimg, uint flags)
{
    if (_status < MStatus::INITIALIZED)
        return;

    int i = 0;
    uint flagswithoutDetection = flags & ~((uint)MetroDrawFlag::DrawDetection);
    for (auto it = _seekersRing.begin(); it != _seekersRing.end(); it++, i++) 
    {
        it->seeker.DrawOverlay(colorimg, flagswithoutDetection);
    }

    if (_outputs.get())
    {
        auto circleseekout = *((CircleSeekerOutputs*)_outputs.get()); 
        
        auto ptCenterAdjust = cv::Point((int)(circleseekout.FoundCenter.x + 0.5), (int)(circleseekout.FoundCenter.y + 0.5));
     
        if (HasDrawFlag(flags, MetroDrawFlag::DrawCenterFit))
        {
            // draw fit circle center
            drawMarker(colorimg, ptCenterAdjust, cv::Scalar(0, 25, 255), cv::MARKER_TILTED_CROSS, 1);
        }

        if (HasDrawFlag(flags, MetroDrawFlag::DrawFit))
        {
            //draw fit circle
            circle(colorimg, ptCenterAdjust, (int)(circleseekout.FoundRadius + 0.5), cv::Scalar(0, 25, 255), 1);
        }
    }

    // to avoid dection to be hide by fit 
    if (HasDrawFlag(flags, MetroDrawFlag::DrawDetection))
    {
        auto drawDetectFlagOnly = (uint) MetroDrawFlag::DrawDetection;
        for (auto it = _seekersRing.begin(); it != _seekersRing.end(); it++, i++)
        {
            it->seeker.DrawOverlay(colorimg, drawDetectFlagOnly);
        }
    }
}

std::shared_ptr<MOutputs> metroCD::CircleSeeker::Compute()
{
    auto circleSeekin = *((CircleSeekerInputs*)_inputsPrm.get());

    if (GetStatus() != MStatus::IDLE)
    {
        // we assume here _inputImage is valid 
        for (int i = 0; i < _seekersRing.size(); i++)
        {
            _seekersRing[i].seeker.SetInputImage(_inputImage);
        }
    }

    // creation des ouutputs
    _outputs = std::make_shared<CircleSeekerOutputs>();
    auto outPtr = (CircleSeekerOutputs*)_outputs.get();

    std::vector<cv::Point2d> points;
    points.reserve(_seekersRing.size());

    // we are good to compute
    if (GetStatus() == MStatus::IDLE)
    {
        // find edge in our angle seeker ring
        for (int i = 0; i < _seekersRing.size(); i++)
        {
            //to-do handle abort

            auto outAngleSeeker = std::static_pointer_cast<SeekerOutputs>(_seekersRing[i].seeker.Compute());
            outPtr->SeekersOutputs.push_back(std::make_tuple(outAngleSeeker, _seekersRing[i].angle));

            if (outAngleSeeker->IsSuccess())
            {
                // To geré le cas des multiple edge detrection 
                // we assume we only detect one edge
                points.push_back(outAngleSeeker->Results.front());
            }

#ifdef csvOUTPUT_COMPUTE
            std::ostringstream stringStream;
            stringStream << csv_OUTPUT_PATH + "SeekReportLine";
            std::string src = stringStream.str() + ".csv";
            stringStream << "_" << _seekersRing[i].angle;
            std::string dst = stringStream.str() + ".csv";
            std::remove(dst.c_str());
            std::rename(src.c_str(), dst.c_str());
#endif

        }
    }

    // circle fit with valid edged
    HyperAccurateCircleFitter circlefitter;
    ICircleFitter::Result circlefitresult;
    auto circlestatus = circlefitter.Fit(points, circlefitresult);

    if (circlefitresult.success)
    {
        outPtr->FoundCenter = circlefitresult.center;
        outPtr->FoundRadius = circlefitresult.radius;
    }
    else
    {
        std::ostringstream messageStream;
        messageStream << "cannot fit circle : " << circlestatus.message;
        outPtr->SetErrorMessage(messageStream.str());
    }

    return _outputs;
}

void metroCD::CircleSeekerOutputs::ReportFile(std::string ReportPath, uint flags)
{
    try
    {
        for (int i = 0; i < SeekersOutputs.size(); i++)
        {
            auto angle = std::get<1>(SeekersOutputs[i]);
            auto outptr = std::get<0>(SeekersOutputs[i]);

            std::ostringstream sSuffix ;
            //sSuffix << std::setprecision(4) << angle << ".csv";
            sSuffix << angle << ".csv";
            std::filesystem::path reppath = ReportPath;
            reppath.replace_extension(sSuffix.str());

            outptr->ReportFile(reppath.generic_string().c_str(), flags);
        }
    }
#pragma warning( push )
#pragma warning( disable : 4101)
    catch (const std::exception& e)
    {
        // what should we dooooo
    }
#pragma warning( pop ) 
}
