#include <opencv2/core/persistence.hpp>
#include <opencv2/core/types.hpp>
#include <opencv2/imgproc.hpp>
#include <vector>

class CircularTheoricalGrid {
public:
    CircularTheoricalGrid();

    /*
     * Creates a die grid from data obtainable by hand on a wafer picture
     *
     * @param waferCirlce - list of points along the border of the wafer. At least 3 points are required
     * @param dieSize - The size of a die
     * @param diePosition - The top-left position of one of the dies (anyone is good)
     *
     * Note that the further away from each-other are the points along the border of the wafer, the better the results
     */
    CircularTheoricalGrid(
        std::vector<cv::Point2d> const& waferCircle,
        cv::Size2d dieSize,
        cv::Point2d diePosition
    );

    CircularTheoricalGrid(
        cv::Point waferCenter,
        int waferRadius,
        cv::Size2d dieSize,
        cv::Point2d diePosition
    );

    void DrawParts(cv::Mat& img, cv::Scalar color);
    
    // List all the dies/reticles fully inside the CircularTheoricalGrid
    std::vector<cv::Rect2i> AsRects();
    // List all the dies/reticles that are partially in the CircularTheoricalGrid
    std::vector<cv::Rect2i> BorderDies();
    // Assuming the Grid is a grid of reticles, apply a layout for one kind of die
    // inside the reticle and return the roi of all the dies described by this layout
    std::vector<cv::Rect2d> ApplySublayout(const std::vector<cv::Rect2d>& sublayout);

    bool operator==(const CircularTheoricalGrid& other);
    bool operator!=(const CircularTheoricalGrid& other);
    
    // to write into cv::FileStorage with operator<<
    void write(cv::FileStorage& fs) const;
    // to read from cv::FileStorage with operator>>
    void read(const cv::FileNode& node);

    cv::Point2d waferCenter;
    double waferRadius;
    cv::Size2d dieSize;
    cv::Point2d origin;

private:
    double FirstRow();
};

std::vector<std::vector<cv::Rect>> generateWaferWithDiesAndTheoreticalGrid(
    cv::Mat& generatedWaferImg,
    cv::Size imgSize,
    int reticleStep,
    const std::vector<cv::Rect>& reticleDies,
    const std::vector<cv::Scalar>& dieColor);

// Allows writing into cv::FileStorage with operator<<
// 
// Refer to https://docs.opencv.org/4.x/dd/d74/tutorial_file_input_output_with_xml_yml.html for more details
void write(cv::FileStorage& fs, const std::string&, const CircularTheoricalGrid& grid);
// Allows reading from cv::FileStorage with operator>>
void read(const cv::FileNode& node, CircularTheoricalGrid& grid, const CircularTheoricalGrid& defaultValue = CircularTheoricalGrid());
