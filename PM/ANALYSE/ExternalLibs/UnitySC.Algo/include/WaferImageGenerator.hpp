#pragma once

#include <EdgeImage.hpp>
#include <NotchImage.hpp>
#include <map>
#include <opencv2/highgui.hpp>

/*
 * Allow to generate images of wafer which is virtually *much* bigger than the image.
 * One should provide parameters about wafer itself and then generate images at given coordinates.
 *
 *
 */
class WaferImageGenerator {
public:
  enum class ImageType : uchar { EDGE, NOTCH };

  struct Image {
    const std::string label;
    const ImageType type;
    const cv::Point2i centroid;
    Image(std::string label, ImageType type, cv::Point2i centroid) : label(label), type(type), centroid(centroid) {}
    Image(Image const &other) : label(other.label), type(other.type), centroid(other.centroid) {}
  };
  struct TakenImage : public Image {
    const cv::Mat content;
    cv::Point2d pixelSize;
    TakenImage(Image const &source, cv::Mat content, cv::Point2d pixelSize) : Image(source), content(content), pixelSize(pixelSize) {}

    EdgeImage::Pointer ToEdgeImage() {
      EdgeImage::Pointer image;
      image = EdgeImage::New(label, content.clone(), EdgeImage::PixelSize(pixelSize), EdgeImage::ImageCentroid(centroid));
      return image;
    }

    NotchImage::Pointer ToNotchImage() {
      NotchImage::Pointer image;
      image = NotchImage::New(label, content.clone(), EdgeImage::PixelSize(pixelSize), EdgeImage::ImageCentroid(centroid));
      return image;
    }
  };
  struct Result {
    cv::Point2d pixelSize;
    cv::Point2d pixelShift;
    int radius;
    std::vector<TakenImage> takenImages;
    void WriteToDisk(std::string const &directory) const;
  };
  struct Parameters {

    Parameters();

    using PixelGenerator = std::function<uchar(cv::Point2d const &)>;

    using PostProc = std::function<void(cv::Mat &)>;

    // Given in micrometers
    int waferRadius;

    // Given in micrometers. This is the radius of the notch without scaling. (i.e. specified radius)
    int notchRadius;

    double notchCenterDistanceFromWaferBorder;

    // Given in micrometers
    cv::Point2d waferShift;

    // Given in pixels
    cv::Size imageSize;

    // Given in micrometer per pixel
    cv::Point2d pixelSize;

    // given in relative degrees
    // for a wafer, this is a difference relative
    // to 270° (south direction)
    double waferRotationAngle;

    std::vector<Image> imagesToTake;

    std::shared_ptr<PixelGenerator> waferPixelIntensityGenerator;
    std::shared_ptr<PixelGenerator> chuckPixelIntensityGenerator;

    std::shared_ptr<PostProc> postProcess;
  };

  Result Generate(Parameters const &parameters, bool displayImage = false);
};
