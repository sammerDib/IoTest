#include "MetroBase.hpp"

#pragma unmanaged

 metroCD::MetroBase::MetroBase() 
     : MetroBase(MType::NotDefined, std::shared_ptr<MInputs>(), cv::Mat())
{
    _status = MStatus::NOT_INITIALIZE;
}

 metroCD::MetroBase::MetroBase(MType mtype)
     : MetroBase(mtype, std::shared_ptr<MInputs>(), cv::Mat())

 {
     _status = MStatus::NOT_INITIALIZE;
 }

 metroCD::MetroBase::MetroBase(MType mtype, std::shared_ptr<MInputs> inputsParams, cv::Mat inputImage)

 {
     _status = MStatus::NOT_INITIALIZE;
     _type = mtype;
     _outputs = std::shared_ptr<MOutputs>();

     _inputsPrm = inputsParams;
     if (_inputsPrm.get() != nullptr)
         _status = MStatus::INITIALIZED;

     SetInputImage(inputImage);
 }


void metroCD::MetroBase::SetInputImage(cv::Mat img)
{
    _inputImage = img;
    if (! _inputImage.empty() && _status >= MStatus::INITIALIZED)
        _status = MStatus::IDLE;

}

void metroCD::MetroBase::Abort()
{
    // TO do handle a cancel token like or pass a share_ptr as token
    //_status = MStatus::ABORTED;
}
