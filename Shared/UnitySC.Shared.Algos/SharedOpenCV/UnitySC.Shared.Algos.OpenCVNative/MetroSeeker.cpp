#include <vector>
#include "MetroSeeker.hpp"
#include <C1DSignalAnalysis.hpp>

#include "lmfit.hpp"

#include <fstream>
#include <filesystem>

#pragma unmanaged
double gaussProbabilityDensityFunction(const double t, const double* p)
{
    // p : 0 , 1 , 2 
    //     A , mu , sigma

    return (p[0] / p[2] * sqrtl(2.0 * CV_PI)) * exp(-(t - p[1]) * (t - p[1]) / (2.0 * p[2] * p[2]));
}

using namespace cv;
using namespace metroCD;
using namespace lmfit;

metroCD::Seeker::Seeker()
    :MetroBase(MType::Seeker)
{

}

metroCD::Seeker::Seeker(std::shared_ptr<SeekerInputs> inputsParams)
    :MetroBase(MType::Seeker, inputsParams, cv::Mat())
{
    InitSeeker();
}

metroCD::Seeker::Seeker(std::shared_ptr<SeekerInputs> inputsParams, cv::Mat inputImage)
    :MetroBase(MType::Seeker, inputsParams, inputImage)
{
    InitSeeker();
}


void metroCD::Seeker::InitSeeker()
{
    auto seekin = *((SeekerInputs*)_inputsPrm.get());

    // create rotated rect of the seeker box in image coordinates reférential (pixels)
    Point2f centerrect((float)((seekin.End.x + seekin.Origin.x) * 0.5), (float)((seekin.End.y + seekin.Origin.y) * 0.5));
    Size2f sizerect((float)(std::sqrtl(std::powl(seekin.End.x - seekin.Origin.x, 2.0) + std::powl(seekin.End.y - seekin.Origin.y, 2.0))), (float)seekin.Width);
    float angle_deg = (float)(atan2(seekin.End.y - seekin.Origin.y, seekin.End.x - seekin.Origin.x) * 180.0 / CV_PI);
    _rotatedRect = RotatedRect(centerrect, sizerect, angle_deg);

    // create affine transform matrix allowing to pass from image worl to seeker box world
    cv::Point2f inrc[4];
    _rotatedRect.points(inrc);
    cv::Point2f in[3];
    in[0] = inrc[1];
    in[1] = inrc[0];
    in[2] = inrc[2];

    cv::Point2f dst[3];
    dst[0] = cv::Point2f(0.f, 0.f);
    dst[1] = cv::Point2f(0.f, sizerect.height);
    dst[2] = cv::Point2f(sizerect.width, 0.f);

    _affineTransformMat = cv::getAffineTransform(in, dst);
}

void metroCD::Seeker::DrawOverlay(cv::Mat& colorimg, uint flags)
{
    if(_status < MStatus::INITIALIZED)
        return;

    auto seekin = *((SeekerInputs*)_inputsPrm.get());

    cv::Point2f vertices[4];
    _rotatedRect.points(vertices);

    if (HasDrawFlag(flags, MetroDrawFlag::DrawSeekers))
    {
        line(colorimg, seekin.Origin, seekin.End, Scalar(0, 240, 255), 1);
        line(colorimg, vertices[1], vertices[0], Scalar(255, 0, 0), 1);
        drawMarker(colorimg, vertices[0], Scalar(160, 70, 160), MARKER_TILTED_CROSS, 1);
        drawMarker(colorimg, seekin.End, Scalar(160, 70, 160), MARKER_TILTED_CROSS, 1);
    }

    if (_outputs.get())
    {
        auto seekout = *((SeekerOutputs*)_outputs.get());

        if (HasDrawFlag(flags, MetroDrawFlag::DrawSkipDetection))
        {
            for (int i = seekin.SearchEdgeNumber; i < seekout.Results.size(); i++)
            {
                drawMarker(colorimg, seekout.Results[i], Scalar(20, 90, 220), MARKER_TILTED_CROSS, 3);
            }
        }

        if (HasDrawFlag(flags, MetroDrawFlag::DrawDetection))
        {
            for (int i = 0; i < seekin.SearchEdgeNumber; i++)
            {
                if (i < seekout.Results.size())
                {
                    drawMarker(colorimg, seekout.Results[i], Scalar(0, 255, 0), MARKER_TILTED_CROSS, 5);
                }
            }
        }
    }
}


std::pair<int, int> SetPeakWindow(const std::vector<signal_1D::Spike>& spikes, std::vector<signal_1D::Spike>::iterator& itpeak, std::vector<double>& signal, int windowMaxPeakSize)
{
    int halfwindowMaxPeakSize = windowMaxPeakSize / 2;
    size_t nWinStart = 0;
    size_t nWinEnd = signal.size() - 1;

    if (spikes.begin() == itpeak)
    {
        // current peak is the first peak
        nWinStart = max((size_t)0, itpeak->Index - halfwindowMaxPeakSize);
    }
    else
    {
        auto prevPeak = std::prev(itpeak);
        nWinStart = std::max((itpeak->Index + prevPeak->Index) / 2, itpeak->Index - halfwindowMaxPeakSize);
    }

    auto nextPeak = std::next(itpeak);
    if (spikes.end() == nextPeak)
    {
        // current peak is the last peak
        nWinEnd = std::min(nWinEnd, itpeak->Index + halfwindowMaxPeakSize);
    }
    else
    {
        nWinEnd = std::min((itpeak->Index + nextPeak->Index) / 2, itpeak->Index + halfwindowMaxPeakSize);
        nWinEnd = std::min(nWinEnd, signal.size() - 1);
    }

    return std::make_pair(nWinStart, nWinEnd);
}

//#define LM_USERTOL    30*DBL_EPSILON 
//const lm_control_struct _lm_control_peaks = { LM_USERTOL, LM_USERTOL, LM_USERTOL, LM_USERTOL, 100., 300, 1, NULL, 0, -1, -1 };
// 
// return vector order A,Mu,simgma, status, fnorm (residue), nbiterdone
std::vector<double> FindPeakFit(const signal_1D::Spike& peak, const std::vector<double>& signal, const std::vector<double>& paramInit, int windowPeakStart, int windowPeakEnd)
{
    int nindx = peak.Index;
    int nWinStart = max(0, windowPeakStart);
    int nWinEnd = min((int)(signal.size()) - 1, windowPeakEnd);

    std::vector<double> x(nWinEnd - nWinStart);
    std::generate(x.begin(), x.end(), [d = (double)nWinStart]() mutable { return d++; });
    std::vector<double> window(signal.begin() + nWinStart, signal.begin() + nWinEnd);
    
    // Params gauss : A, mu, Sigma
    std::vector<double> par0{ paramInit[0], (double)nindx, paramInit[2] };
    lm_control_struct control = lm_control_double;
    auto result = lmfit::fit_curve(par0, x, window, &gaussProbabilityDensityFunction, control);

    std::vector<double> fitResult = result.par;
    fitResult.push_back((double)result.status.outcome);
    fitResult.push_back(result.status.fnorm);
    fitResult.push_back((double)result.status.nfev);

    return fitResult;

}

// sort peak first
bool compSpikeType(signal_1D::Spike& a, signal_1D::Spike& b) { return (int)a.Type > (int)b.Type; };

std::shared_ptr<MOutputs> metroCD::Seeker::Compute()
{
    _outputs = std::make_shared<SeekerOutputs>();
    try
    {
        auto seekin = *((SeekerInputs*)_inputsPrm.get());

        // extrat seeker box from input image
        // TODO : handle clipping when box is outside image
        cv::Mat seekerBox;
        cv::warpAffine(_inputImage, seekerBox, _affineTransformMat, _rotatedRect.size, cv::INTER_LANCZOS4);
        // create row vector line 
        cv::Mat seekerLine;
        cv::reduce(seekerBox, seekerLine, 0, cv::ReduceTypes::REDUCE_AVG, CV_64F);
        if (seekin.SeekScale > 1)
        {
            // apply scaling we artficially increase vector line to have a gretaer signal to inspect
            cv::resize(seekerLine, seekerLine, cv::Size(), seekin.SeekScale, 1.0, cv::INTER_LANCZOS4);
        }

        // let border be the same in all directions
        int kernelsize = seekin.KernelSize;
        int halfKernel = (kernelsize - 1) / 2;

        double step = 1.0 / (double)seekin.SeekScale;
        double offset = -1.0 * (step * ((double)seekin.SeekScale - 1.0)) * 0.5;


        // enlarge vector to deal with padding du to kernel convolution
        int border = halfKernel;
        cv::copyMakeBorder(seekerLine, seekerLine, 0, 0, border, border, cv::BORDER_REPLICATE);

        double* ptr = seekerLine.ptr<double>(0);
        std::vector<double> firstderivative;
        firstderivative.reserve(seekerLine.cols);

        double kFactor = 0.0;
        switch (seekin.SeekMode)
        {
        case SeekMode::BlackToWhite: // up front
            kFactor = 1.0;
            break;
        case SeekMode::WhiteToBlack: // down front
            kFactor = -1.0;
            break;
        case SeekMode::CorrelationModel:
            // TODO handle model correlation
            break;
        }

        //Generate kernel row vector
        cv::Mat kernel(1, kernelsize, CV_64F);
        double* ptrKernel = kernel.ptr<double>(0);
        for (int k = 0; k < kernelsize; k++)
        {
            *ptrKernel = k < halfKernel ? -kFactor : (k > halfKernel ? kFactor : 0.0);
            ++ptrKernel;
        }

        //Convolve kernel edge revealer
        ptr = ptr + halfKernel;
        for (int i = halfKernel; i < seekerLine.cols - halfKernel; i++)
        {
            double dValue = 0.0;
            double* p = (ptr - halfKernel);
            ptrKernel = kernel.ptr<double>(0);
            for (int k = 0; k < kernelsize; k++)
            {
                dValue += *p++ * *ptrKernel++;
            }
            ++ptr;
            firstderivative.push_back(dValue);
        }

        Mat mDeriv(1, (int)firstderivative.size(), CV_64F, firstderivative.data());

        std::vector<double> firstderivativeEnlarge(mDeriv.begin<double>(), mDeriv.end<double>());
        signal_1D::SignalStats signalAnalyzed;
        double sigthreshold = seekin.SigAnalysisThreshold;
        auto spikes = signal_1D::FindPeaks(firstderivativeEnlarge, sigthreshold);
        //std::list<signal_1D::Spike> spikesLow = signal_1D::FindDrops(firstderivativeEnlarge, sigthreshold);
        //spikes.splice(spikes.end(), spikesLow);

        signalAnalyzed.Spikes.clear();
        signalAnalyzed.MovingMeans.clear();
        signalAnalyzed.MovingStddev.clear();

        // to do handle multiple SeekLocInEdge
        SeekLocInEdge Loc = seekin.EdgeLocalizePreference;
        double dSigmaCoef = (double)Loc;

        std::vector<double> par_init{ sigthreshold * 0.5, 0.0, 10.0 };

        std::vector< std::vector<double> > peaksFitResults; // A, Mu, Sigma, status, fnorm, nbIter
        peaksFitResults.reserve(spikes.size() + 1);
        for (auto it = spikes.begin(); it != spikes.end(); ++it)
        {
            if ((*it).Type == signal_1D::SpikeType::Peak)
            {
                // set correct window size aurond this peak
                const auto [windowPeakStart, windowPeakEnd] = SetPeakWindow(spikes, it, firstderivativeEnlarge, seekin.SigAnalysisPeakWindowSize * (int)seekin.SeekScale);
                //const int windowPeakStart = it->Index - (seekin.SigAnalysisPeakWindowSize / 2), windowPeakEnd = it->Index + (seekin.SigAnalysisPeakWindowSize / 2);
                auto pfit = FindPeakFit(*it, firstderivativeEnlarge, par_init, windowPeakStart, windowPeakEnd);

                // check fit viability
                // - status outcome should have converged hence < 6, if nb iter max hasbeen reach (==5) we control residue
                // - A amplitude should be positive and have a minimal height (at least 1)
                //-  mu mean should be closed to the search zone

                bool StatusPassed = (pfit[3] < 5) ? true : (pfit[3] < 6) && ( (pfit[4] <= 5.00) || std::abs(pfit[1] - (*it).Index) <= 1.0) ;
                if ( StatusPassed &&
                    1.0 <= pfit[0] &&
                    windowPeakStart - 0.5 <= pfit[1] && pfit[1] <= windowPeakEnd + 0.5)
                {
                    // add to fit vector
                    peaksFitResults.push_back(pfit);
                }
                else
                {
                    // non viable peak fit make it a drop
                    (*it).Type = signal_1D::SpikeType::Drop;
                }
            }

            signalAnalyzed.Spikes.push_back(*it);
        }

        // on veut avoir peak devant drop derrière
        std::sort(signalAnalyzed.Spikes.begin(), signalAnalyzed.Spikes.end(), compSpikeType);


        // creation des ouutputs
        auto outPtr = (SeekerOutputs*)_outputs.get();
        outPtr->SeekLine.reserve(seekerLine.cols);
        outPtr->SeekLineTreated.reserve(firstderivativeEnlarge.size());
        outPtr->SeekLineSpikes.reserve(signalAnalyzed.Spikes.size());
        outPtr->SeekFits = peaksFitResults;

        std::vector<cv::Point2d> peaksInbox;
        peaksInbox.reserve(signalAnalyzed.Spikes.size());
        outPtr->Results.reserve(signalAnalyzed.Spikes.size());

        // Remplissage des outputs
        for (int i = 0; i < signalAnalyzed.Spikes.size(); i++)
        {
            if (signalAnalyzed.Spikes[i].Type == signal_1D::SpikeType::Peak)
            {
                // to do : en cas e changemnt de funciton d'evaluation par exemple lorentzian au lieu de gaussian il fudrat adpater la recupe des parametre ici
                double dXGaussianSeekLoc = (peaksFitResults[i])[1];           // get mu -- in case of gaussian evaluator
                dXGaussianSeekLoc += dSigmaCoef * (peaksFitResults[i])[2];    // get sigma -- in case of gaussian evaluator

                double tx = (signalAnalyzed.Spikes[i].Index * step) + offset;
                outPtr->SeekLineSpikes.push_back(Point2d(tx, signalAnalyzed.Spikes[i].Value));

                tx = (dXGaussianSeekLoc * step) + offset;
                // convert peak into point in seeker box referential
                peaksInbox.push_back(Point2d(tx, seekin.Width * 0.5));

                (outPtr->SeekFits[i]).push_back(tx);
            }
            else // drop
            {
                double tx = (signalAnalyzed.Spikes[i].Index * step) + offset;
                outPtr->SeekLineSpikes.push_back(Point2d(tx, signalAnalyzed.Spikes[i].Value));
            }
        }

        if (peaksInbox.size() > 0)
        {
            //convert peaks in seeker box into inputimage refernetial coordiantes
            cv::Mat transformMatInv;
            cv::invertAffineTransform(_affineTransformMat, transformMatInv);
            cv::transform(peaksInbox, outPtr->Results, transformMatInv);

        }
        else
        {
            std::ostringstream messageStream;
            messageStream << "No edge peak found" ;
            outPtr->SetErrorMessage(messageStream.str());
        }


        // edge line signal extracted for reports 
        for (int i = 0; i < firstderivativeEnlarge.size(); i++)
        {
            double tx = (i * step) + offset;
            outPtr->SeekLineTreated.push_back(Point2d(tx, firstderivativeEnlarge[i]));
        }

        // profile line  extracted for reports 
        ptr = seekerLine.ptr<double>(0);
        for (int i = 0; i < seekerLine.cols; i++)
        {
            double tx = ((i - border) * step) + offset;
            outPtr->SeekLine.push_back(Point2d(tx, *ptr++));
        }

#ifdef csvOUTPUT_COMPUTE
        try
        {
            std::filesystem::create_directories(csv_OUTPUT_PATH);
        }
        catch (const std::exception& ee)
        {
            auto msg = ee.what();
        }

        outPtr->ReportFile(csv_OUTPUT_PATH + "SeekReportLine.csv", 0u);
#endif

      
    }
    catch (cv::Exception& opencvExp)
    {
        auto outPtr = (SeekerOutputs*)_outputs.get();
        std::ostringstream messageStream;
        messageStream << "OpenCV exception : " << opencvExp.what();
        outPtr->SetErrorMessage(messageStream.str());
    }
    catch (std::exception& e)
    {
        auto outPtr = (SeekerOutputs*)_outputs.get();
        std::ostringstream messageStream;
        messageStream << "std exception : " << e.what();
        outPtr->SetErrorMessage(messageStream.str());
    }  
    catch(...)
    {
        auto outPtr = (SeekerOutputs*)_outputs.get();
        std::ostringstream messageStream;
        messageStream << "Unknow exception raised" ;
        outPtr->SetErrorMessage(messageStream.str());      
    }
    
    return _outputs;
}

void metroCD::SeekerOutputs::ReportFile(std::string ReportPath, uint flags)
{
    try
    {
        std::ofstream strm;
        strm.open(ReportPath, std::ios::out);
        strm << "idx;line_idx;line;deriv_idx;deriv;spike_idx;spikevalue;Fit0;Fit1;Fit2;status;fnorm;nitr;GfitX;GfitY;";
        strm << ";"; // let a blank column

        std::vector<double*> fits;
        fits.reserve(SeekFits.size());
        for (int j = 0; j < (int)SeekFits.size(); j++)
        {
            strm << "GaussFit" << j + 1 << ";";
            double* ppt = new double[3] { SeekFits[j][0], SeekFits[j][1], SeekFits[j][2] };
            fits.push_back(ppt);
        }
        strm << std::endl;

        int maxsize = std::max((int)SeekLine.size(), (int)SeekLineTreated.size());
        for (int i = 0; i < maxsize; i++)
        {
            strm << i << ";";

            if (i < (int)SeekLine.size())
                strm << SeekLine[i].x << ";" << SeekLine[i].y << ";";
            else
                strm << ";" << ";";

            if (i < (int)SeekLineTreated.size())
                strm << SeekLineTreated[i].x << ";" << SeekLineTreated[i].y << ";";
            else
                strm << ";" << ";";

            if (i < (int)SeekLineSpikes.size())
                strm << SeekLineSpikes[i].x << ";" << SeekLineSpikes[i].y << ";";
            else
                strm << ";" << ";";

            if (i < (int)SeekFits.size())
            {
                for (auto it = SeekFits[i].begin(); it != SeekFits[i].end(); ++it)
                {
                    strm << *it << ";";
                }

                double pt[3]{ SeekFits[i][0], SeekFits[i][1], SeekFits[i][2] };
                strm << gaussProbabilityDensityFunction(SeekFits[i][6], pt) << ";";

            }
            else
                strm << ";" << ";" << ";" << ";" << ";" << ";" << ";" << ";";

            strm << ";"; // let a blank column

            if (i < (int)SeekLineTreated.size())
            {
                for (int j = 0; j < (int)SeekFits.size(); j++)
                {
                    strm << gaussProbabilityDensityFunction(SeekLineTreated[i].x, fits[j]) << ";";
                }
            }
            else
            {
                for (int j = 0; j < (int)SeekFits.size(); j++) {
                    strm << ";";
                }
            }

            strm << std::endl; // next line
        }
        strm.close();

        int fsize = (int)fits.size();
        for (int j = 0; j < fsize; j++)
        {
            auto ptr = fits[j];
            if (ptr != nullptr)
            {
                delete[] ptr;
                fits[j] = nullptr;
            }
        }
    }
#pragma warning( push )
#pragma warning( disable : 4101)
    catch (const std::exception& e)
    {
        // handle error 
    }
#pragma warning( pop ) 
}
