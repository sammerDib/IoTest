#include <EventQueue.hpp>
#include <Logger.hpp>
#include <WaferImageGenerator.hpp>

#include <fstream>
#include <map>
#include <opencv2/imgproc.hpp>

#ifndef NDEBUG
#include <opencv2/highgui/highgui.hpp>
#endif

namespace {

#ifndef NDEBUG
  void Display(std::string const &title, cv::Mat content) { cv::imshow(title, content); }
#else
  void Display(std::string const &title, cv::Mat content) {}
#endif

  cv::Mat CreateImageAt(int index, WaferImageGenerator::Parameters const &parameters) {
    cv::Mat image = cv::Mat::zeros(parameters.imageSize, CV_8U);

    cv::Point2d pixelSize(parameters.pixelSize);

    cv::Point2d imageCentroid = parameters.imagesToTake.at(index).centroid;

    imageCentroid -= parameters.waferShift;

    cv::Point2d imageOriginInChuckReferential;

    imageOriginInChuckReferential.x = imageCentroid.x - (((std::floor(image.cols / 2)) * pixelSize.x) - pixelSize.x / 2);
    imageOriginInChuckReferential.y = imageCentroid.y + (((std::floor(image.rows / 2)) * pixelSize.y) - pixelSize.y / 2);

    // compute notch center position
    double rotationInRadians = parameters.waferRotationAngle / 180.0 * CV_PI;
    double notchCenterThetaInRadians = (3.0 * CV_PI / 2.0) + rotationInRadians;

    cv::Point2d notchCenter;
    notchCenter.x = (parameters.waferRadius + parameters.notchCenterDistanceFromWaferBorder) * std::cos(notchCenterThetaInRadians);
    notchCenter.y = (parameters.waferRadius + parameters.notchCenterDistanceFromWaferBorder) * std::sin(notchCenterThetaInRadians);

    for (int column = 0; column < parameters.imageSize.width; ++column) {
      for (int row = 0; row < parameters.imageSize.height; ++row) {

        cv::Point2d currentPixelInChuckReferential;
        currentPixelInChuckReferential.x = imageOriginInChuckReferential.x + (column * pixelSize.x);
        currentPixelInChuckReferential.y = imageOriginInChuckReferential.y - (row * pixelSize.y);

        double distanceFromPixelToCenter = cv::norm(currentPixelInChuckReferential);
        bool pixelIsInTheWafer = (distanceFromPixelToCenter <= parameters.waferRadius);

        double distanceFromPixelToNotchCenter = cv::norm(currentPixelInChuckReferential - notchCenter);
        bool pixelIsInTheNotch = (distanceFromPixelToNotchCenter <= parameters.notchRadius);

        if (pixelIsInTheWafer && !pixelIsInTheNotch) {
          image.at<uchar>(cv::Point2d(column, row)) = (*parameters.waferPixelIntensityGenerator)(currentPixelInChuckReferential);
        } else {
          image.at<uchar>(cv::Point2d(column, row)) = (*parameters.chuckPixelIntensityGenerator)(currentPixelInChuckReferential);
        }
      }
    }
    return image;
  }

  void LogWaferParameters(WaferImageGenerator::Parameters const &parameters) {
    std::string message;
    std::stringstream str_strm;
    str_strm << "[WaferImageGenerator::Generate] Wafer with scale  " << parameters.pixelSize;
    message = str_strm.str();
    Logger::Info(message);

    str_strm.str("");
    str_strm << "[WaferImageGenerator::Generate] Wafer with radius  " << parameters.waferRadius << " µm";
    message = str_strm.str();
    Logger::Info(message);

    str_strm.str("");
    str_strm << "[WaferImageGenerator::Generate] Wafer with shift  " << parameters.waferShift;
    message = str_strm.str();
    Logger::Info(message);
  }

  void LogImageGeneration(WaferImageGenerator::Image const &image) {
    std::stringstream str_strm;
    std::string message;
    str_strm << "[WaferImageGenerator::Generate] at ";
    str_strm << image.centroid;
    message = str_strm.str();
    Logger::Info(message);
  }
} // namespace

WaferImageGenerator::Result WaferImageGenerator::Generate(Parameters const &parameters, bool displayImage) {

  Result result;
  result.pixelSize = parameters.pixelSize;
  result.pixelShift = parameters.waferShift;
  result.radius = parameters.waferRadius;

  int imageIndex = 0;
  LogWaferParameters(parameters);

  for (auto &imageToTake : parameters.imagesToTake) {

    LogImageGeneration(imageToTake);

    cv::Mat image = CreateImageAt(imageIndex, parameters);
    if (nullptr != parameters.postProcess) {
      (*parameters.postProcess)(image);
    }
    result.takenImages.emplace_back(imageToTake, image, result.pixelSize);
    if (displayImage) {
      Display(imageToTake.label, result.takenImages[imageIndex].content);
    }
    imageIndex++;
  }

#ifndef NDEBUG
  if (displayImage) {
    cv::waitKey();
  }
#endif

  return result;
}

void WaferImageGenerator::Result::WriteToDisk(std::string const &directory) const {

  for (TakenImage takenImage : takenImages) {

    std::string xPos = std::to_string(takenImage.centroid.x);
    std::string yPos = std::to_string(takenImage.centroid.y);
    std::string imageName = "IMG-" + takenImage.label + "-_X_" + xPos + "_Y_" + yPos + ".png";

    cv::imwrite(directory + "/" + imageName, takenImage.content);
  }

  std::ofstream out(directory + "/Params.dat");
  out << "Camera.PixToMm_X=" << pixelSize.x / 1000 << std::endl;
  out << "Camera.PixToMm_Y=" << pixelSize.x / 1000 << std::endl;
  out.close();

  //
}

WaferImageGenerator::Parameters::Parameters() {

  // notch center is about 800µm from the wafer border (ie 0.8mm)
  // see SEMI M1-0918
  notchCenterDistanceFromWaferBorder = 800;

  waferPixelIntensityGenerator = std::make_shared<PixelGenerator>([](cv::Point const &location) { return 240; });
  chuckPixelIntensityGenerator = std::make_shared<PixelGenerator>([](cv::Point const &location) { return 10; });
  postProcess = std::make_shared<WaferImageGenerator::Parameters::PostProc>([](cv::Mat image) {
    cv::GaussianBlur(image, image, cv::Size(5, 5), 10);
    cv::medianBlur(image, image, 5);
  });
}
